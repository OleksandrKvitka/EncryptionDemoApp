using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Sec;

namespace EncryptionDemoApp
{
    public class Crypto(AsymmetricAlgorythms algorithm, int keySize) : ICrypto
    {
        private readonly AsymmetricAlgorythms asymmetricAlgorythm = algorithm;
        private readonly int keySize = keySize;

        public EncodedData Encrypt(string plainText, byte[] symmetricKey, byte[] publicKey)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedText;
            byte[] aesIV;

            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                aes.Key = symmetricKey;
                aesIV = aes.IV;
                encryptedText = aes.CreateEncryptor().TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
            }

            byte[] encryptedKey = asymmetricAlgorythm switch
            {
                AsymmetricAlgorythms.RSA => RsaEncrypt(symmetricKey, publicKey),
                AsymmetricAlgorythms.ECIES => EciesEncrypt(symmetricKey, publicKey),
                _ => throw new NotImplementedException(),
            };

            return new EncodedData(encryptedText, encryptedKey, aesIV);
        }

        public string Decrypt(EncodedData encodedData, byte[] privateKey)
        {
            // Decrypt the symmetric key using the specified asymmetric algorithm
            byte[] symmetricKey = asymmetricAlgorythm switch
            {
                AsymmetricAlgorythms.RSA => RsaDecrypt(encodedData.EncryptedKey, privateKey),
                AsymmetricAlgorythms.ECIES => EciesDecrypt(encodedData.EncryptedKey, privateKey),
                _ => throw new NotImplementedException(),
            };

            string plainText;
            using (Aes aes = Aes.Create())
            {
                aes.Key = symmetricKey;
                aes.IV = encodedData.AesIV;
                byte[] decryptedBytes = aes.CreateDecryptor().TransformFinalBlock(
                    encodedData.EncryptedData, 0, encodedData.EncryptedData.Length);
                plainText = Encoding.UTF8.GetString(decryptedBytes);
            }

            return plainText;
        }

        private static byte[] RsaEncrypt(byte[] plainText, byte[] key)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPublicKey(key, out _);
                return rsa.Encrypt(plainText, RSAEncryptionPadding.OaepSHA256);
            }
        }

        private byte[] EciesEncrypt(byte[] plainText, byte[] key)
        {
            var curve = keySize switch
            {
                256 => SecNamedCurves.GetByName("secp256r1"),
                384 => SecNamedCurves.GetByName("secp384r1"),
                521 => SecNamedCurves.GetByName("secp521r1"),
                _ => throw new InvalidOperationException("No such curve")
            };
            var domainParameters = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(
                curve.Curve, curve.G, curve.N, curve.H);

            var q = curve.Curve.DecodePoint(key);
            var publicKeyParams = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domainParameters);

            var keyGen = new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();
            var keyGenParams = new Org.BouncyCastle.Crypto.KeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom(), 256);
            keyGen.Init(keyGenParams);
            var ephemeralKeyPair = keyGen.GenerateKeyPair();
            var ephemeralPrivateKey = (Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)ephemeralKeyPair.Private;

            var sharedSecretPoint = publicKeyParams.Q.Multiply(ephemeralPrivateKey.D).Normalize();
            var sharedSecretBytes = sharedSecretPoint.AffineXCoord.GetEncoded();

            var derivedKey = Org.BouncyCastle.Security.DigestUtilities.CalculateDigest("SHA-256", sharedSecretBytes);
            var encryptedSessionKey = new byte[plainText.Length];
            for (int i = 0; i < plainText.Length; i++)
            {
                encryptedSessionKey[i] = (byte)(plainText[i] ^ derivedKey[i % derivedKey.Length]);
            }

            var ephemeralPublicKeyBytes = ((Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters)ephemeralKeyPair.Public).Q.GetEncoded();
            var result = new byte[ephemeralPublicKeyBytes.Length + encryptedSessionKey.Length];

            Array.Copy(ephemeralPublicKeyBytes, 0, result, 0, ephemeralPublicKeyBytes.Length);
            Array.Copy(encryptedSessionKey, 0, result, ephemeralPublicKeyBytes.Length, encryptedSessionKey.Length);

            return result;
        }

        private byte[] RsaDecrypt(byte[] encryptedText, byte[] key)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(key, out _);
                return rsa.Decrypt(encryptedText, RSAEncryptionPadding.OaepSHA256);
            }
        }

        private byte[] EciesDecrypt(byte[] encryptedText, byte[] key)
        {
            var curve = this.keySize switch
            {
                256 => SecNamedCurves.GetByName("secp256r1"),
                384 => SecNamedCurves.GetByName("secp384r1"),
                521 => SecNamedCurves.GetByName("secp521r1"),
                _ => throw new InvalidOperationException("No such curve")
            };
            var domainParameters = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(
                curve.Curve, curve.G, curve.N, curve.H);

            int keySize = 1 + 2 * (curve.Curve.FieldSize / 8);
            byte[] ephemeralPublicKeyBytes = new byte[keySize];
            byte[] encryptedSessionKey = new byte[encryptedText.Length - keySize];

            Array.Copy(encryptedText, 0, ephemeralPublicKeyBytes, 0, keySize);
            Array.Copy(encryptedText, keySize, encryptedSessionKey, 0, encryptedSessionKey.Length);

            var q = curve.Curve.DecodePoint(ephemeralPublicKeyBytes);
            var ephemeralPublicKey = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, domainParameters);

            var d = new Org.BouncyCastle.Math.BigInteger(1, key);
            var privateKeyParams = new Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters(d, domainParameters);

            // Compute the shared secret
            var sharedSecretPoint = ephemeralPublicKey.Q.Multiply(privateKeyParams.D).Normalize();
            var sharedSecretBytes = sharedSecretPoint.AffineXCoord.GetEncoded();

            var derivedKey = Org.BouncyCastle.Security.DigestUtilities.CalculateDigest("SHA-256", sharedSecretBytes);

            var decryptedSessionKey = new byte[encryptedSessionKey.Length];
            for (int i = 0; i < encryptedSessionKey.Length; i++)
            {
                decryptedSessionKey[i] = (byte)(encryptedSessionKey[i] ^ derivedKey[i % derivedKey.Length]);
            }

            return decryptedSessionKey;
        }
    }

    public enum AsymmetricAlgorythms
    {
        RSA,
        ECIES
    }
}


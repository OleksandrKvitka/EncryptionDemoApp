using System.Security.Cryptography;
using SourceAFIS;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace EncryptionDemoApp
{
    public static class KeyGenerator
    {
        public static byte[] GenerateKeyFromFingerprint(IFingerPrintProvider fingerPringProvider)
        {
            var template = new FingerprintTemplate(
                new FingerprintImage(
                    File.ReadAllBytes(fingerPringProvider.GetFingerPrint())));
            var fingerPrintKey = SHA256.HashData(template.ToByteArray());
            return AddSalt(fingerPrintKey);
        }

        private static byte[] AddSalt(byte[] key)
        {
            var salt = GenerateRandomSalt(32);
            using var hmac = new HMACSHA256();
            var hkdf = new Rfc2898DeriveBytes(key, salt, iterations: 10000, hashAlgorithm: HashAlgorithmName.SHA256);
            return hkdf.GetBytes(32);
        }

        private static byte[] GenerateRandomSalt(int size)
        {
            byte[] salt = new byte[size];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        public static (byte[], byte[]) GenerateRSAKeys(int keySize)
        {
            byte[] publicKey;
            byte[] privateKey;
            using (RSA rsa = RSA.Create())
            {
                rsa.KeySize = keySize;
                publicKey = rsa.ExportRSAPublicKey();
                privateKey = rsa.ExportRSAPrivateKey();
            }
            return (publicKey, privateKey);
        }

        public static (byte[] PublicKey, byte[] PrivateKey) GenerateECIESKeys(int keySize)
        {

            var curve = keySize switch
            {
                256 => SecNamedCurves.GetByName("secp256r1"),
                384 => SecNamedCurves.GetByName("secp384r1"),
                521 => SecNamedCurves.GetByName("secp521r1"),
                _ => throw new InvalidOperationException("No such curve")
            };

            var domainParameters = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

            var keyGenerator = new ECKeyPairGenerator();
            var keyGenParams = new ECKeyGenerationParameters(domainParameters, new SecureRandom());
            keyGenerator.Init(keyGenParams);
            AsymmetricCipherKeyPair keyPair = keyGenerator.GenerateKeyPair();

            var privateKey = ((ECPrivateKeyParameters)keyPair.Private).D.ToByteArray();
            var publicKey = ((ECPublicKeyParameters)keyPair.Public).Q.GetEncoded();

            return (publicKey, privateKey);
        }
    }
}
using System.Text;
using System.Security.Cryptography;

namespace EncryptionDemoApp
{
	public class Crypto : ICrypto
    {
        public EncodedData Encrypt(string plainText, byte [] symmetricKey, AsymmetricAlgorythms asymmetricAlgorythms)
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] encryptedText;
            using (Aes aes = Aes.Create())
            {
                aes.Key = symmetricKey;
                aes.GenerateIV();
                encryptedText = aes.CreateEncryptor().TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
            }

            byte[] encryptedKey;
            switch (asymmetricAlgorythms)
            {
                case AsymmetricAlgorythms.RSA:
                    encryptedKey = Rsa(symmetricKey);
                    break;
                case AsymmetricAlgorythms.ECIES:
                    encryptedKey = Ecies(symmetricKey);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new EncodedData(encryptedText, encryptedKey);
        }

        private byte[] Rsa(byte[] symmetricKey)
        {
            //TODO
            throw new NotImplementedException();
        }

        private byte[] Ecies(byte[] symmetricKey)
        {
            //TODO
            throw new NotImplementedException();
        }

        public string Decrypt(EncodedData encodedData)
        {
            throw new NotImplementedException();
        }
    }

    public enum AsymmetricAlgorythms
    {
        RSA,
        ECIES
    }
}


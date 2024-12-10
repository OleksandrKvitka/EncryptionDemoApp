namespace EncryptionDemoApp
{
    public interface ICrypto
    {
        public EncodedData Encrypt(string plainText, byte[] symmetricKey, byte[] publicKey);
        public string Decrypt(EncodedData encodedData, byte[] privateKey);
    }
}

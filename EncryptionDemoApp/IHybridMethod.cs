namespace EncryptionDemoApp
{
    public interface ICrypto
    {
        EncodedData Encrypt(string plainText, byte[] symmetricKey, AsymmetricAlgorythms asymmetricAlgorythms);
        string Decrypt(EncodedData encodedData);
    }
}

namespace EncryptionDemoApp
{
    public class EncodedData(byte[] encryptedData, byte[] encryptedKey, byte[] aesIV, byte[]? nonce = null)
    {
        public byte[] EncryptedData { get; set; } = encryptedData;
        public byte[] EncryptedKey { get; set; } = encryptedKey;
        public byte[] AesIV { get; set; } = aesIV;
        public byte[]? Nonce { get; set; } = nonce;
    }
}

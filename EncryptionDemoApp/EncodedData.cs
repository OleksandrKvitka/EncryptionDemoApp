public class EncodedData
{
    public byte[] EncryptedData { get; set; }
    public byte[] EncryptedKey { get; set; }
    public byte[]? Nonce { get; set; }
    
    public EncodedData(byte[] encryptedData, byte[] encryptedKey, byte[]? nonce = null)
    {
        EncryptedData = encryptedData;
        EncryptedKey = encryptedKey;
        Nonce = nonce;
    }
}
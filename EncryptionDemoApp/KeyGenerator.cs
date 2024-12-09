using System.Security.Cryptography;
using SourceAFIS;

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
    }
}
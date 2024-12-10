using System.Diagnostics;

namespace EncryptionDemoApp
{
    public class PerformanceTest()
    {
        private readonly List<int> rsaKeySizes = [3072, 7680, 15360];
        private readonly List<int> EciesKeySizes = [256, 384, 521];
        private readonly List<double> totalTimes = new List<double>();

        private const int PLAIN_TEXT_SIZE = 2048;

        public void RunRsaTests()
        {
            foreach (int keySize in rsaKeySizes)
            {
                var crypto = new Crypto(AsymmetricAlgorythms.RSA, keySize);
                string plaintext = GeneratePlaintext(PLAIN_TEXT_SIZE);

                var sessionKey = KeyGenerator.GenerateKeyFromFingerprint(new FingerPrintFromFile());
                var stopwatch = Stopwatch.StartNew();
                var (publicKey, privateKey) = KeyGenerator.GenerateRSAKeys(keySize);
                var encryptedData = crypto.Encrypt(plaintext, sessionKey, publicKey);
                var decryptedText = crypto.Decrypt(encryptedData, privateKey);
                stopwatch.Stop();
                double totalTime = stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
                Console.WriteLine(decryptedText == plaintext);
                totalTimes.Add(totalTime);
            }

            using (StreamWriter writer = new StreamWriter($"RSA_performance.csv"))
            {
                writer.WriteLine("Plaintext Size (KB),Total Time (seconds)");
                for (int i = 0; i < rsaKeySizes.Count; i++)
                {
                    writer.WriteLine($"{rsaKeySizes[i]},{totalTimes[i]}");
                }
            }
        }

        public void RunEciesTests()
        {
            foreach (int keySize in EciesKeySizes)
            {
                var crypto = new Crypto(AsymmetricAlgorythms.ECIES, keySize);
                string plaintext = GeneratePlaintext(PLAIN_TEXT_SIZE);

                var sessionKey = KeyGenerator.GenerateKeyFromFingerprint(new FingerPrintFromFile());
                var stopwatch = Stopwatch.StartNew();
                var (publicKey, privateKey) = KeyGenerator.GenerateECIESKeys(keySize);
                var encryptedData = crypto.Encrypt(plaintext, sessionKey, publicKey);
                var decryptedText = crypto.Decrypt(encryptedData, privateKey);
                Console.WriteLine(decryptedText == plaintext);
                stopwatch.Stop();
                double totalTime = stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
                totalTimes.Add(totalTime);
            }

            using (StreamWriter writer = new StreamWriter($"ECIES_performance.csv"))
            {
                writer.WriteLine("Plaintext Size (KB),Total Time (seconds)");
                for (int i = 0; i < EciesKeySizes.Count; i++)
                {
                    writer.WriteLine($"{EciesKeySizes[i]},{totalTimes[i]}");
                }
            }
        }

        private string GeneratePlaintext(int sizeInKb)
        {
            byte[] data = new byte[sizeInKb * 1024];
            new Random().NextBytes(data);
            return Convert.ToBase64String(data);
        }
    }
}

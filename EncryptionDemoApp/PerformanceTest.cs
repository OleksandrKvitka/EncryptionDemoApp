using System.Diagnostics;

public class PerformanceTest
{
    private readonly IHybridMethod hybridEncryption;
    private readonly List<int> plaintextSizes;
    private readonly List<double> totalTimes;

    public PerformanceTest(IHybridMethod encryption)
    {
        hybridEncryption = encryption;
        plaintextSizes = new List<int>();
        totalTimes = new List<double>();
    }

    public void RunTests()
    {
        for (int i = 1; i <= 8; i++)
        {
            int sizeInKb = i * 1024; // Increase the plaintext size in KB
            string plaintext = GeneratePlaintext(sizeInKb);

            // Measure total encryption and decryption time
            var stopwatch = Stopwatch.StartNew();
            var encryptedData = hybridEncryption.Encrypt(plaintext);
            var decryptedText = hybridEncryption.Decrypt(encryptedData);
            stopwatch.Stop();
            double totalTime = stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;

            // Collect data
            plaintextSizes.Add(sizeInKb);
            totalTimes.Add(totalTime);
        }

        // Export data for visualization
        ExportData();
    }

    private string GeneratePlaintext(int sizeInKb)
    {
        byte[] data = new byte[sizeInKb * 1024];
        new Random().NextBytes(data);
        return Convert.ToBase64String(data);
    }

    private void ExportData()
    {
        using (StreamWriter writer = new StreamWriter($"{hybridEncryption.GetType()}_performance.csv"))
        {
            writer.WriteLine("Plaintext Size (KB),Total Time (seconds)");
            for (int i = 0; i < plaintextSizes.Count; i++)
            {
                writer.WriteLine($"{plaintextSizes[i]},{totalTimes[i]}");
            }
        }

        Console.WriteLine($"Data exported to {hybridEncryption.GetType()}_performance.csv");
    }
}
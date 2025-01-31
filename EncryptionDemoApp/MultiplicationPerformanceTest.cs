using System.Diagnostics;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Endo;
using Org.BouncyCastle.Math.EC.Multiplier;

namespace EncryptionDemoApp
{
    public static class MultiplicationPerformanceTests
    {
        public static long MultiplicationTime(ECMultiplier multiplier, ECPoint point, BigInteger scalar)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine($"Multiplier: {multiplier.GetType().Name}");

            try
            {
                var result = multiplier.Multiply(point, scalar);
                stopwatch.Stop();
                var finalTime = stopwatch.ElapsedMilliseconds;

                Console.WriteLine($"Mutiplication result: {result}");
                Console.WriteLine("Time (ms): " + finalTime);
                Console.WriteLine();

                return finalTime;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine();
                return -1;
            }
        }

        public static void TestMultiplierPerformance(string[] multiplierTypes)
        {
            var g = ECNamedCurveTable.GetByName("secp256r1").G;
            var scalar = new BigInteger("1234567890123456789012345678901234567890");
            var results = new Dictionary<string, long>();

            foreach (var type in multiplierTypes)
            {
                var multiplier = MultiplierFactory.CreateMultiplier(type);
                var time = MultiplicationTime(multiplier, g, scalar);
                results[type] = time;
            }

            ExportResultsToCsv(results);
        }


        private static void ExportResultsToCsv(Dictionary<string, long> results)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;
            string dataFolderPath = Path.Combine(projectDirectory, "data");
            var filePath = Path.Combine(dataFolderPath, "multiplication_performance_results.csv");
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine(string.Join(",", results.Keys));
                writer.WriteLine(string.Join(",", results.Values));
            }

            Console.WriteLine($"Results exported to {filePath}");
        }
    }

    public static class MultiplierFactory
    {
        public static ECMultiplier CreateMultiplier(string type)
        {
            return type switch
            {
                "MontgomeryLadder" => new MontgomeryLadderMultiplier(),
                "DoubleAdd" => new DoubleAddMultiplier(),
                "FixedPointComb" => new FixedPointCombMultiplier(),
                "NafL2RMultiplier" => new NafL2RMultiplier(),
                "NafR2LMultiplier" => new NafR2LMultiplier(),
                "WNafL2RMultiplier" => new WNafL2RMultiplier(),
                "MixedNafR2LMultiplier" => new MixedNafR2LMultiplier(),
                "ReferenceMultiplier" => new ReferenceMultiplier(),
                "ZSignedDigitL2RMultiplier" => new ZSignedDigitL2RMultiplier(),
                "ZSignedDigitR2LMultiplier" => new ZSignedDigitR2LMultiplier(),
                _ => throw new ArgumentException("Unknown multiplier type")
            };
        }
    }
}
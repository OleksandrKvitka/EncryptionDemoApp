
namespace EncryptionDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] multiplierTypes = {
                "MontgomeryLadder",
                "DoubleAdd",
                "FixedPointComb",
                "NafL2RMultiplier",
                "NafR2LMultiplier",
                "WNafL2RMultiplier",
                "MixedNafR2LMultiplier",
                "ReferenceMultiplier",
                "ZSignedDigitL2RMultiplier",
                "ZSignedDigitR2LMultiplier"
            };

            MultiplicationPerformanceTests.TestMultiplierPerformance(multiplierTypes);
            Console.WriteLine("Performance results have been exported to multiplication_performance_results.csv");
        }
    }
}



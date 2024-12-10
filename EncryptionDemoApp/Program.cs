
namespace EncryptionDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = new PerformanceTest();
            //tests.RunRsaTests();
            tests.RunEciesTests();
        }
    }
}



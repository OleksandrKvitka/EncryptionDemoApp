namespace EncryptionDemoApp
{
    public class FingerPrintFromFile : IFingerPrintProvider
    {
        private static string FINGER_PRINT_PATH = "data/101_1.tif";

        public string GetFingerPrint()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", FINGER_PRINT_PATH); ;
        }
    }
}

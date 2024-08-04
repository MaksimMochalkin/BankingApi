namespace BankingApi.Helpers
{
    public static class LoggerHelper
    {
        public static string GetLogFilePath()
        {
            var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            var logFileName = DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
            return Path.Combine(logFolder, logFileName);
        }
    }
}

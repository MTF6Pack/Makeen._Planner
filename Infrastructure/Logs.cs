namespace Infrastructure
{
    public static class Logs
    {
        /// <summary>
        /// Writes a general log message to Logs.txt.
        /// </summary>
        public static void Write(string message)
        {
            string logMessage = $"{DateTime.Now} | {message}\n{new string('-', 100)}\n";
            Console.WriteLine(logMessage);

            string logDirectory = GetLogDirectory();
            string logFilePath = Path.Combine(logDirectory, "Logs.txt");

            // Ensure the directory exists
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // File.AppendAllText creates the file if it doesn't exist,
            // but here we explicitly ensure it:
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Dispose();
            }

            File.AppendAllText(logFilePath, logMessage);
        }

        /// <summary>
        /// Writes a message in yellow color.
        /// </summary>
        public static void WriteYellow(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Write(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes an error log message to Errors.txt.
        /// </summary>
        public static void WriteError(string message)
        {
            string logMessage = $"{DateTime.Now} | {message}\n{new string('-', 100)}\n";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(logMessage);
            Console.ResetColor();

            string logDirectory = GetLogDirectory();
            string logFilePath = Path.Combine(logDirectory, "Errors.txt");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Dispose();
            }

            File.AppendAllText(logFilePath, logMessage);
        }

        private static string GetLogDirectory()
        {
            // Use the same log directory as in GlobalExceptionMiddleware for consistency.
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }
    }
}
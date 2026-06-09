using System;
using System.IO;
using System.Text;

namespace AndroidSideloader
{
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR,
        TRACE,
        FATAL
    }

    public static class Logger
    {
        private static readonly object lockObject = new object();
        private static string? logFilePath;
        private static bool initialized = false;

        public static string DefaultLogPath => Path.Combine(AppContext.BaseDirectory, "debuglog.txt");

        public static void Initialize(string? path = null)
        {
            try
            {
                logFilePath = path ?? DefaultLogPath;

                string? logDirectory = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                if (!File.Exists(logFilePath))
                {
                    using (FileStream fs = File.Create(logFilePath)) { }
                }

                initialized = true;
                string time = DateTime.UtcNow.ToString("hh:mm:ss.fff tt (UTC): ");
                Log($"\n\n{time}------------ Logger initialized  ------------", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing logger: {ex.Message}");
            }
        }

        public static bool Log(string text, LogLevel logLevel = LogLevel.INFO, bool ret = true)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= 5)
                return ret;

            if (!initialized)
            {
                Initialize();
            }

            string time = DateTime.UtcNow.ToString("hh:mm:ss.fff tt (UTC): ");
            string newline = text.Length > 40 && text.Contains("\n") ? "\n\n" : "\n";
            string logEntry = time + "[" + logLevel.ToString().ToUpper() + "] [" + GetCallerInfo() + "] " + text + newline;

            try
            {
                lock (lockObject)
                {
                    File.AppendAllText(logFilePath ?? DefaultLogPath, logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log: {ex.Message}");
            }

            return ret;
        }

        private static string GetCallerInfo()
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
            if (stackTrace.FrameCount >= 3)
            {
                var frame = stackTrace.GetFrame(2);
                var method = frame?.GetMethod();
                string? className = method?.DeclaringType?.Name;
                string? methodName = method?.Name;
                return $"{className}.{methodName}";
            }

            return string.Empty;
        }
    }
}

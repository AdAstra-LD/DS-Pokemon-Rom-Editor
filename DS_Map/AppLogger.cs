using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace DSPRE
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Text;

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public static class AppLogger
    {
        private static readonly object _fileLock = new object();
        private static readonly ConcurrentQueue<string> _recentLogBuffer = new ConcurrentQueue<string>();
        private static string _logFilePath;
        private static MainProgram _mainProgram;
        private const int MaxInMemoryLines = 500;
        public static LogLevel MinimumLevel { get; set; } = LogLevel.Debug;


        public static void Initialize(MainProgram program, string logFileName = "application.log", LogLevel minLevel = LogLevel.Debug)
        {
            _mainProgram = program;
            string logDir = Path.Combine(Program.DspreDataPath, "Logs");
            Directory.CreateDirectory(logDir);
            _logFilePath = Path.Combine(logDir, logFileName);
            MinimumLevel = minLevel;
            Log(LogLevel.Info, "Logger initialized.");
        }

        public static void Log(LogLevel level, string message)
        {
            if (level < MinimumLevel)
                return;

            string timestamped = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level.ToString().ToUpper()}] {message}";

            lock (_fileLock)
            {
                File.AppendAllText(_logFilePath, timestamped + Environment.NewLine);
            }

            _recentLogBuffer.Enqueue(timestamped);
            while (_recentLogBuffer.Count > MaxInMemoryLines && _recentLogBuffer.TryDequeue(out _)) { }
        }

        public static void Debug(string message) => Log(LogLevel.Debug, message);
        public static void Info(string message) => Log(LogLevel.Info, message);
        public static void Warn(string message) => Log(LogLevel.Warning, message);
        public static void Error(string message) => Log(LogLevel.Error, message);
        public static void Fatal(string message) => Log(LogLevel.Fatal, message);

        public static string GetRecentLogs()
        {
            return string.Join(Environment.NewLine, _recentLogBuffer);
        }

    }
}

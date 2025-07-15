using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace DSPRE
{
    public static class CrashReporter
    {
        private static MainProgram _mainProgram;

        public static void Initialize(MainProgram program)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            _mainProgram = program;
        }

        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            string crashReport = BuildCrashReport(exception);
            string filePath = GetCrashReportFilePath();

            try
            {
                File.WriteAllText(filePath, crashReport, Encoding.UTF8);
            }
            catch
            {
                try
                {
                    EventLog.WriteEntry("Application", crashReport, EventLogEntryType.Error);
                }
                catch { }
            }
        }

        private static string BuildCrashReport(Exception ex)
        {
            var sb = new StringBuilder();

            sb.AppendLine("===== Crash Report =====");
            sb.AppendLine($"Timestamp: {DateTime.Now}");
            sb.AppendLine($"App Version: {Assembly.GetExecutingAssembly().GetName().Version}");
            sb.AppendLine($"App Path: {AppDomain.CurrentDomain.BaseDirectory}");
            sb.AppendLine($".NET Version: {Environment.Version}");
            sb.AppendLine($"OS: {Environment.OSVersion}");
            sb.AppendLine($"Is 64-bit OS: {Environment.Is64BitOperatingSystem}");
            sb.AppendLine($"Opened ROM Path: {_mainProgram.romInfo.GetRomNameFromWorkdir()}");
            sb.AppendLine();
            sb.AppendLine("===== Recent Logs =====");
            sb.AppendLine(AppLogger.GetRecentLogs());


            if (ex != null)
            {
                sb.AppendLine("Exception:");
                sb.AppendLine(ex.ToString());
            }
            else
            {
                sb.AppendLine("Exception: Unknown");
            }

            return sb.ToString();
        }

        private static string GetCrashReportFilePath()
        {
            string crashDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashReports");
            Directory.CreateDirectory(crashDir);

            string filename = $"Crash_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            return Path.Combine(crashDir, filename);
        }
    }
}

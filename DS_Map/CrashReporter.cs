using System;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE
{
    public static class CrashReporter
    {
        private static MainProgram _mainProgram;

        public static void Initialize(MainProgram program)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            Application.ThreadException += HandleThreadException;

            TaskScheduler.UnobservedTaskException += HandleTaskException;
            _mainProgram = program;
        }

        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WriteCrashReport(e.ExceptionObject as Exception);
        }

        private static void HandleThreadException(object sender, ThreadExceptionEventArgs e)
        {
            WriteCrashReport(e.Exception);
        }

        private static void HandleTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved(); // Prevents app from crashing
            WriteCrashReport(e.Exception);
        }

        private static void WriteCrashReport(Exception ex)
        {
            string crashReport = BuildCrashReport(ex);
            string filePath = GetCrashReportFilePath();

            try
            {
                File.WriteAllText(filePath, crashReport, Encoding.UTF8);
            }
            catch
            {

            }

            DialogResult result = MessageBox.Show(
                   $"An unexpected error occurred and the application crashed.\n\nA crash report was saved here:\n\n\nClick OK to open the folder.",
                   "Application Error",
                   MessageBoxButtons.OKCancel,
                   MessageBoxIcon.Error
               );

            if (result == DialogResult.OK)
            {
                Helpers.ExplorerSelect(filePath);
            }
        }

        private static string BuildCrashReport(Exception ex)
        {
            string romPath = String.Empty;

            var sb = new StringBuilder();

            sb.AppendLine("===== Crash Report =====");
            sb.AppendLine($"Timestamp: {DateTime.Now}");
            sb.AppendLine($"App Version: {Assembly.GetExecutingAssembly().GetName().Version}");
            sb.AppendLine($"App Path: {AppDomain.CurrentDomain.BaseDirectory}");
            sb.AppendLine($".NET Version: {Environment.Version}");
            sb.AppendLine($"OS: {Environment.OSVersion}");
            sb.AppendLine($"Is 64-bit OS: {Environment.Is64BitOperatingSystem}");
            try
            {
                romPath = _mainProgram?.romInfo?.GetRomNameFromWorkdir() ?? "Unknown";
            }
            catch (Exception romEx)
            {
                romPath = $"Failed to retrieve ROM path: {romEx.Message}";
            }
            sb.AppendLine($"Opened ROM Path: {romPath}");
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
            string crashDir = Path.Combine(Program.DspreDataPath, "CrashReports");
            Directory.CreateDirectory(crashDir);

            string filename = $"Crash_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            return Path.Combine(crashDir, filename);
        }
    }
}

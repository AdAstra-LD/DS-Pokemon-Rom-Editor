using System;
using System.IO;
using System.Windows.Forms;
using Velopack;

namespace DSPRE
{
    static class Program
    {
        public static string DspreDataPath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DSPRE");
        public static string DatabasePath = Path.Combine(Program.DspreDataPath, "databases");

        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Directory.Exists(DspreDataPath))
                Directory.CreateDirectory(DspreDataPath);

            VelopackApp.Build().Run();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainProgram mainProgram = new MainProgram();
            CrashReporter.Initialize(mainProgram);
            Application.Run(mainProgram);
        }

        public static void SetupDatabase()
        {   
            // needs to be this verbose (copy instead of move) so this works accross drives
            try
            { 
                string sourceDbPath = Path.Combine(Application.StartupPath, "databases");
                if (Directory.Exists(sourceDbPath) && !SettingsManager.Settings.databasesPulled) {
                    if (!Directory.Exists(DatabasePath)) {
                        Directory.CreateDirectory(DatabasePath);
                    }
                    foreach (string dirPath in Directory.GetDirectories(sourceDbPath, "*", SearchOption.AllDirectories)) {
                        Directory.CreateDirectory(dirPath.Replace(sourceDbPath, DatabasePath));
                    }
                    foreach (string filePath in Directory.GetFiles(sourceDbPath, "*.*", SearchOption.AllDirectories)) {
                        File.Copy(filePath, filePath.Replace(sourceDbPath, DatabasePath), true);
                    }
                    // After successful copy, delete source and update settings
                    Directory.Delete(sourceDbPath, true);
                    SettingsManager.Settings.databasesPulled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy databases: {ex.Message}",
                              "Database Setup Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
    }
}

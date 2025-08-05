using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Velopack;
using LibGit2Sharp;

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

        public static void CloneAndSetupDatabase()
        {
            try
            {
                string repoUrl = "https://github.com/DS-Pokemon-Rom-Editor/scrcmd-database.git";
                

                // Only clone if directory doesn't exist
                if (!Directory.Exists(DatabasePath) && SettingsManager.Settings.databasesPulled == false)
                {
                    Repository.Clone(repoUrl, DatabasePath);
                    SettingsManager.Settings.databasesPulled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to clone database repository: {ex.Message}",
                              "Database Setup Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Velopack;

namespace DSPRE
{
    static class Program
    {
        public static string DspreDataPath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DSPRE");

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
    }
}

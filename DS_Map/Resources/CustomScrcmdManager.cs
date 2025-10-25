using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace DSPRE.Resources
{
    public partial class CustomScrcmdManager : Form
    {

        private static string CustomDBsPath = Path.Combine(Program.DatabasePath, "edited_databases");

        public class CustomScrcmdSetting
        {
            public string JsonPath { get; set; }
        }

        public CustomScrcmdManager()
        {
            InitializeComponent();
            UpdateDataGrid(CustomScrcmdDataGrid);
        }

        private void UpdateDataGrid(DataGridView grid)
        {
            var settings = LoadDBs();
            grid.Rows.Clear();
            foreach (var setting in settings)
            {
                grid.Rows.Add(setting.JsonPath);
            }
        }

        public static List<CustomScrcmdSetting> LoadDBs()
        {
            return Directory.GetFiles(CustomDBsPath, "*.json")
                .Select(filePath => new CustomScrcmdSetting {
                    JsonPath = Path.GetFileName(filePath)
                })
                .ToList();
        }

        private void ImportScrcmdButton_Click(object sender, EventArgs e)
        {
            if (CustomScrcmdDataGrid.SelectedRows.Count == 0) return;
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select a file";
                dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                dialog.InitialDirectory = Program.DatabasePath;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var DBtoreplace = CustomScrcmdDataGrid.SelectedRows[0].Cells[0].Value.ToString();
                    var newDBname = dialog.FileName;

                    File.Delete(Path.Combine(CustomDBsPath, DBtoreplace));
                    File.Copy(newDBname, Path.Combine(CustomDBsPath, DBtoreplace));

                    UpdateDataGrid(CustomScrcmdDataGrid);

                    // Ask user if they want to reload now
                    var result = MessageBox.Show(
                        "Database replaced successfully.\n\n" +
                        "Do you want to reload and reparse all scripts now?\n\n" +
                        "Yes: Reload database and reparse all scripts immediately\n" +
                        "No: Changes will take effect on next ROM load",
                        "Reload Database?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        ReloadAndReparseScripts(Path.Combine(CustomDBsPath, DBtoreplace));
                    }
                }
                else
                {
                    MessageBox.Show("Something went wrong", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void ReloadAndReparseScripts(string databasePath)
        {
            try
            {
                int scriptCount = DSPRE.Filesystem.GetScriptCount();
                List<(int fileID, ushort commandID, long offset)> invalidCommands = null;

                using (var loadingForm = new DSPRE.Editors.Utils.LoadingForm(scriptCount, "Reparsing scripts with new database..."))
                {
                    loadingForm.Shown += (s, e) =>
                    {
                        Task.Run(() =>
                        {
                            invalidCommands = DSPRE.ROMFiles.ScriptFile.ReloadDatabaseAndReparseAll(
                                databasePath,
                                progressCallback: (current, total) =>
                                {
                                    if (loadingForm.IsHandleCreated)
                                    {
                                        loadingForm.Invoke((Action)(() => loadingForm.UpdateProgress(current)));
                                    }
                                });

                            if (loadingForm.IsHandleCreated)
                            {
                                loadingForm.Invoke((Action)(() => loadingForm.Close()));
                            }
                        });
                    };

                    loadingForm.ShowDialog();
                }

                if (invalidCommands != null && invalidCommands.Count > 0)
                {
                    var affectedFiles = invalidCommands.Select(c => c.fileID).Distinct().OrderBy(x => x).ToList();
                    string fileList = string.Join(", ", affectedFiles.Select(f => f.ToString("D4")));
                    var uniqueCommands = invalidCommands.Select(c => c.commandID).Distinct().OrderBy(x => x).ToList();
                    string commandList = string.Join(", ", uniqueCommands.Select(c => $"0x{c:X4}"));

                    MessageBox.Show(
                        $"Database reloaded, but {invalidCommands.Count} script command(s) across {affectedFiles.Count} file(s) still could not be parsed.\n\n" +
                        $"Affected files: {fileList}\n" +
                        $"Unrecognized commands: {commandList}\n\n" +
                        $"Affected script files will be incomplete and read-only.",
                        "Partial Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(
                        "Database reloaded and all scripts reparsed successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reloading database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ExportScrcmdButton_Click(object sender, EventArgs e)
        {
            if (CustomScrcmdDataGrid.SelectedRows.Count == 0) return;

            string selectedName = CustomScrcmdDataGrid.SelectedRows[0].Cells[0].Value?.ToString();

            // Where your edited JSONs live
            string sourcePath = Path.Combine(CustomDBsPath, selectedName);

            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Export JSON";
                dialog.FileName = selectedName;                 
                dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                dialog.DefaultExt = "json";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;                  

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(sourcePath, dialog.FileName, overwrite: true);
                        MessageBox.Show("Export complete.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (IOException ioEx)
                    {
                        MessageBox.Show($"I/O error during export:\n{ioEx.Message}", "Export Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unexpected error:\n{ex.Message}", "Export Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OpenScrcmdFolderButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomDBsPath) || !Directory.Exists(CustomDBsPath))
            {
                MessageBox.Show($"Folder not found:\n{CustomDBsPath}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process.Start("explorer.exe", CustomDBsPath);
        }
    }
}

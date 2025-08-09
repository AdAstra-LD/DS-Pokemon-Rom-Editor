using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
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
                    MessageBox.Show("Database replaced. Restart DSPRE to apply.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Something went wrong", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

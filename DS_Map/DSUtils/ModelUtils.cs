using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace DSPRE {
    public static class ModelUtils {

        public static void ModelToDAE(string modelName, byte[] modelData, byte[] textureData) {
            MessageBox.Show("Choose output folder.\nDSPRE will automatically create a sub-folder in it.", "Awaiting user input", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CommonOpenFileDialog cofd = new CommonOpenFileDialog {
                IsFolderPicker = true,
                Multiselect = false
            };
            if (cofd.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            }

            string outDir = Path.Combine(cofd.FileName, modelName);

            if (Directory.Exists(outDir)) {
                if (Directory.GetFiles(outDir).Length > 0) {
                    DialogResult d = MessageBox.Show($"Directory \"{outDir}\" already exists and is not empty.\nIts contents will be lost.\n\nDo you want to proceed?", "Directory not empty", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (d.Equals(DialogResult.No)) {
                        return;
                    } else {
                        Directory.Delete(outDir, recursive: true);
                    }
                } else {
                    Directory.Delete(outDir, recursive: true);
                }
            }
            string tempNSBMDPath = outDir + "_temp.nsbmd";

            if (textureData != null && textureData.Length > 0) {
                modelData = NSBUtils.BuildNSBMDwithTextures(modelData, textureData);
            }

            File.WriteAllBytes(tempNSBMDPath, modelData);

            /* Check correct creation of temp NSBMD file*/
            if (!File.Exists(tempNSBMDPath)) {
                MessageBox.Show("Expected NSBMD file could not be found.\nAborting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Process apicula = new Process();
            apicula.StartInfo.FileName = @"Tools\apicula.exe";
            apicula.StartInfo.Arguments = $" convert \"{tempNSBMDPath}\" --output \"{outDir}\"";
            apicula.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            apicula.StartInfo.CreateNoWindow = true;
            apicula.Start();
            apicula.WaitForExit();

            if (File.Exists(tempNSBMDPath)) {
                File.Delete(tempNSBMDPath);

                if (File.Exists(tempNSBMDPath)) {
                    MessageBox.Show("Temporary NSBMD file deletion failed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else {
                MessageBox.Show("Temporary NSBMD file corresponding to this map disappeared.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (apicula.ExitCode == 0) {
                MessageBox.Show("NSBMD was exported and converted successfully!", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("NSBMD to DAE conversion failed.", "Apicula error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ModelToGLB(string modelName, byte[] modelData, byte[] textureData) {
            MessageBox.Show("Choose output folder.\nDSPRE will automatically create a sub-folder in it.", "Awaiting user input", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CommonOpenFileDialog cofd = new CommonOpenFileDialog {
                IsFolderPicker = true,
                Multiselect = false
            };
            if (cofd.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            }

            string outDir = Path.Combine(cofd.FileName, modelName);

            if (Directory.Exists(outDir)) {
                if (Directory.GetFiles(outDir).Length > 0) {
                    DialogResult d = MessageBox.Show($"Directory \"{outDir}\" already exists and is not empty.\nIts contents will be lost.\n\nDo you want to proceed?", "Directory not empty", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (d.Equals(DialogResult.No)) {
                        return;
                    } else {
                        Directory.Delete(outDir, recursive: true);
                    }
                } else {
                    Directory.Delete(outDir, recursive: true);
                }
            }
            string tempNSBMDPath = outDir + "_temp.nsbmd";

            if (textureData != null && textureData.Length > 0) {
                modelData = NSBUtils.BuildNSBMDwithTextures(modelData, textureData);
            }

            File.WriteAllBytes(tempNSBMDPath, modelData);

            /* Check correct creation of temp NSBMD file*/
            if (!File.Exists(tempNSBMDPath)) {
                MessageBox.Show("NSBMD file corresponding to this map could not be found.\nAborting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Process apicula = new Process();
            apicula.StartInfo.FileName = @"Tools\apicula.exe";
            apicula.StartInfo.Arguments = $" convert \"{tempNSBMDPath}\" -f glb --output \"{outDir}\"";
            apicula.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            apicula.StartInfo.CreateNoWindow = true;
            apicula.Start();
            apicula.WaitForExit();

            if (File.Exists(tempNSBMDPath)) {
                File.Delete(tempNSBMDPath);

                if (File.Exists(tempNSBMDPath)) {
                    MessageBox.Show("Temporary NSBMD file deletion failed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else {
                MessageBox.Show("Temporary NSBMD file corresponding to this map disappeared.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (apicula.ExitCode == 0) {
                MessageBox.Show("NSBMD was exported and converted successfully!", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("NSBMD to GLB conversion failed.", "Apicula error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
using System;
using System.IO;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles {
    public abstract class RomFile {
        public abstract byte[] ToByteArray();
        public bool SaveToFile(string path, bool showSuccessMessage = true) {
            
            byte[] romFileToByteArray = ToByteArray();
            if (romFileToByteArray is null) {
                AppLogger.Error(GetType().Name + " couldn't be saved!");
                return false;
            }

            File.WriteAllBytes(path, romFileToByteArray);

            if (showSuccessMessage) {
                MessageBox.Show(GetType().Name + " saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return true;
        }
        protected internal bool SaveToFileDefaultDir(DirNames dir, int IDtoReplace, bool showSuccessMessage = true) {
            string path = RomInfo.gameDirs[dir].unpackedDir + "\\" + IDtoReplace.ToString("D4");
            return this.SaveToFile(path, showSuccessMessage);
        }
        protected internal void SaveToFileExplorePath(string fileType, string fileExtension, string suggestedFileName, bool showSuccessMessage = true) {
            fileExtension = "*." + fileExtension;

            SaveFileDialog sf = new SaveFileDialog {
                Filter = $"{fileType} ({fileExtension})|{fileExtension}"
            };

            if (!string.IsNullOrWhiteSpace(suggestedFileName)) {
                sf.FileName = suggestedFileName;
            }

            if (sf.ShowDialog() != DialogResult.OK) {
                return;
            }

            this.SaveToFile(sf.FileName, showSuccessMessage);
        }
    }
}

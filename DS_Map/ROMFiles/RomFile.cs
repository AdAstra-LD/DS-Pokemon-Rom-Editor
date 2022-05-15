using System;
using System.IO;
using System.Windows.Forms;
using static DSPRE.RomInfo;
using Newtonsoft.Json;

namespace DSPRE.ROMFiles {
    public abstract class RomFile {
        public abstract byte[] ToByteArray();
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
        public void SaveBinaryToFile(string path, bool showSuccessMessage = true) {
            
            byte[] romFileToByteArray = ToByteArray();
            if (romFileToByteArray is null) {
                Console.WriteLine(GetType().Name + " couldn't be saved!");
                return;
            }

            File.WriteAllBytes(path, romFileToByteArray);

            if (showSuccessMessage) {
                MessageBox.Show(GetType().Name + " saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void SaveJSONToFile(string path, bool showSuccessMessage = true) {
            
            string romFileToJSON = ToJSON();
            if (string.IsNullOrEmpty(romFileToJSON)) {
                Console.WriteLine(GetType().Name + " couldn't be saved!");
                return;
            }

            File.WriteAllText(path, romFileToJSON);

            if (showSuccessMessage) {
                MessageBox.Show(GetType().Name + " saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected internal void SaveToFileDefaultDir(DirNames dir, int IDtoReplace, bool showSuccessMessage = true) {
            string path = RomInfo.gameDirs[dir].unpackedDir + "\\" + IDtoReplace.ToString("D4");
            this.SaveBinaryToFile(path, showSuccessMessage);
        }
        protected internal void SaveToFileExplorePath(string fileType, string fileExtension, string fileFormat, string suggestedFileName, bool showSuccessMessage = true)
        {
            fileExtension = "*." + fileExtension;

            SaveFileDialog sf = new SaveFileDialog {
                Filter = fileType + ' ' + "(" + fileExtension + ")" + '|' + fileExtension
            };

            if (!string.IsNullOrEmpty(suggestedFileName)) {
                sf.FileName = suggestedFileName;
            }

            if (sf.ShowDialog() != DialogResult.OK) {
                return;
            }

            switch (fileFormat) {
                case "binary":
                    this.SaveBinaryToFile(sf.FileName, showSuccessMessage);
                    break;
                case "json":
                    this.SaveJSONToFile(sf.FileName, showSuccessMessage);
                    break;
            }
        }
    }
}

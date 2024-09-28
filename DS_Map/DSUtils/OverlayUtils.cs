using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static DSPRE.DSUtils;
using static DSPRE.RomInfo;

namespace DSPRE {
    public static class OverlayUtils {
        public static class OverlayTable {
            private const int ENTRY_LEN = 32;

            /**
            * Only checks if the overlay is CONFIGURED as compressed
            **/
            public static bool IsDefaultCompressed(int ovNumber) {
                using (DSUtils.EasyReader f = new EasyReader(RomInfo.overlayTablePath, ovNumber * ENTRY_LEN + 31)) {
                    return (f.ReadByte() & 1) == 1;
                }
            }
            public static void SetDefaultCompressed(int ovNumber, bool compressStatus) {
                DSUtils.WriteToFile(RomInfo.overlayTablePath, new byte[] { compressStatus ? (byte)1 : (byte)0 }, (uint)(ovNumber * ENTRY_LEN + 31)); //overlayNumber * size of entry + offset
            }

            public static uint GetRAMAddress(int ovNumber) {
                using (DSUtils.EasyReader f = new EasyReader(RomInfo.overlayTablePath, ovNumber * ENTRY_LEN + 4)) {
                    return f.ReadUInt32();
                }
            }
            public static uint GetUncompressedSize(int ovNumber) {
                using (DSUtils.EasyReader f = new EasyReader(RomInfo.overlayTablePath, ovNumber * ENTRY_LEN + 8)) {
                    return f.ReadUInt32();
                }
            }

            /**
            * Gets number of overlays
            **/
            public static int GetNumberOfOverlays() {
                using (FileStream fileStream = File.OpenRead(RomInfo.overlayTablePath))
                {
                    // Get the length of the file in bytes
                    return (int)(fileStream.Length / ENTRY_LEN);
                }
            }
        }


        public static string GetPath(int overlayNumber) {
            return $"{workDir}overlay\\overlay_{overlayNumber:D4}.bin";
        }
       
        /**
         * Checks the actual size of the overlay file
         **/
        public static bool IsCompressed(int ovNumber) {
            return (new FileInfo(GetPath(ovNumber)).Length < OverlayTable.GetUncompressedSize(ovNumber));
        }

        public static void RestoreFromCompressedBackup(int overlayNumber, bool eventEditorIsReady) {
            String overlayFilePath = GetPath(overlayNumber);

            if (File.Exists(overlayFilePath + DSUtils.backupSuffix)) {
                if (new FileInfo(overlayFilePath).Length <= new FileInfo(overlayFilePath + DSUtils.backupSuffix).Length) { //if overlay is bigger than its backup
                    Console.WriteLine($"Overlay {overlayNumber} is already compressed.");
                    return;
                } else {
                    File.Delete(overlayFilePath);
                    File.Move(overlayFilePath + DSUtils.backupSuffix, overlayFilePath);
                }
            } else {
                string msg = $"Overlay File {overlayFilePath}{DSUtils.backupSuffix} couldn't be found and restored.";
                Console.WriteLine(msg);

                if (eventEditorIsReady) {
                    MessageBox.Show(msg, "Can't restore overlay from backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public static int Compress(int overlayNumber) {
            string overlayFilePath = GetPath(overlayNumber);

            if (!File.Exists(overlayFilePath)) {
                MessageBox.Show("Overlay to decompress #" + overlayNumber + " doesn't exist",
                    "Overlay not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ERR_OVERLAY_NOTFOUND;
            }

            Process compress = new Process();
            compress.StartInfo.FileName = @"Tools\blz.exe";
            compress.StartInfo.Arguments = "-en " + '"' + overlayFilePath + '"';
            Application.DoEvents();
            compress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            compress.StartInfo.CreateNoWindow = true;
            compress.Start();
            compress.WaitForExit();
            return compress.ExitCode;
        }

        public static int Decompress(string overlayFilePath, bool makeBackup = true) {
            if (!File.Exists(overlayFilePath)) {
                MessageBox.Show($"File to decompress \"{overlayFilePath}\" doesn't exist",
                    "Overlay not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ERR_OVERLAY_NOTFOUND;
            }

            if (makeBackup) {
                if (File.Exists(overlayFilePath + backupSuffix)) {
                    File.Delete(overlayFilePath + backupSuffix);
                }
                File.Copy(overlayFilePath, overlayFilePath + backupSuffix);
            }

            Process decompress = DSUtils.CreateDecompressProcess(overlayFilePath);
            decompress.Start();
            decompress.WaitForExit();
            return decompress.ExitCode;
        }
        public static int Decompress(int overlayNumber, bool makeBackup = true) {
            return Decompress(GetPath(overlayNumber), makeBackup);
        }

    }
}
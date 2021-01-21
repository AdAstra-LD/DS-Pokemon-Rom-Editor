using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace DSPRE {
    static class DSUtils {
        public static string workDir { get; private set; }
        public static void SetWorkDir(string workDir) {
            DSUtils.workDir = workDir;
        }

        public static void CompressOverlay(int overlayNumber) {
            string overlayFilePath = '"' + workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin" + '"';
            Process unpack = new Process();
            unpack.StartInfo.FileName = @"Tools\blz.exe";
            unpack.StartInfo.Arguments = "-en " + overlayFilePath;
            Application.DoEvents();
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            unpack.StartInfo.CreateNoWindow = true;
            unpack.Start();
            unpack.WaitForExit();
        }

        public static bool DecompressArm9() {
            int attempts = 0;
            long arm9Length = new FileInfo(workDir + @"arm9.bin").Length;

            while (attempts < 3 && arm9Length < 0xBC000) {
                attempts++;
                if (attempts > 1) {
                    BinaryWriter arm9Truncate = new BinaryWriter(File.OpenWrite(workDir + @"arm9.bin"));

                    arm9Truncate.BaseStream.SetLength(arm9Length - 0xc);
                    arm9Truncate.Close();
                }
                Process decompress = new Process();
                decompress.StartInfo.FileName = @"Tools\blz.exe";
                decompress.StartInfo.Arguments = @" -d " + '"' + workDir + "arm9.bin" + '"';
                decompress.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                decompress.StartInfo.CreateNoWindow = true;
                decompress.Start();
                decompress.WaitForExit();

                arm9Length = new FileInfo(workDir + @"arm9.bin").Length;
            }

            return (arm9Length > 0xBC000);
        }

        public static int DecompressOverlay(int overlayNumber, bool makeBackup) {
            String overlayFilePath = workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin";

            if (!File.Exists(overlayFilePath)) {
                MessageBox.Show("Overlay to decompress #" + overlayNumber + " doesn't exist",
                    "Overlay not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            if (makeBackup) {
                if (File.Exists(overlayFilePath + ".bak")) {
                    if (new FileInfo(overlayFilePath).Length > new FileInfo(overlayFilePath + ".bak").Length) { //if overlay is bigger than its backup
                        Console.WriteLine("Overlay " + overlayNumber + " is already uncompressed and its compressed backup exists.");
                        return 1;
                    }
                    File.Delete(overlayFilePath + ".bak");
                }
                File.Copy(overlayFilePath, overlayFilePath + ".bak");
            }

            Process unpack = new Process();
            unpack.StartInfo.FileName = @"Tools\blz.exe";
            String arguments = "-d " + '"' + overlayFilePath + '"';
            unpack.StartInfo.Arguments = arguments;
            Application.DoEvents();
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            unpack.StartInfo.CreateNoWindow = false;
            unpack.Start();
            unpack.WaitForExit();
            return unpack.ExitCode;
        }

        public static void RestoreOverlayFromCompressedBackup(int overlayNumber, bool eventEditorIsReady) {
            String overlayFilePath = workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin";

            if (File.Exists(overlayFilePath + ".bak")) {
                if (new FileInfo(overlayFilePath).Length <= new FileInfo(overlayFilePath + ".bak").Length) { //if overlay is bigger than its backup
                    Console.WriteLine("Overlay " + overlayNumber + " is already compressed.");
                    return;
                } else {
                    File.Delete(overlayFilePath);
                    File.Move(overlayFilePath + ".bak", overlayFilePath);
                }
            } else {
                string msg = "Overlay File " + '"' + overlayFilePath + ".bak" + '"' + " couldn't be found and restored.";
                Console.WriteLine(msg);

                if (eventEditorIsReady)
                    MessageBox.Show(msg, "Can't restore overlay from backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }
        public static byte[] ReadFromArm9(long startOffset, long numberOfBytes) {
            return ReadFromFile(workDir + "arm9.bin", startOffset, numberOfBytes);
        }
        public static byte[] ReadFromFile(string path, long startOffset, long numberOfBytes) {
            BinaryReader reader = new BinaryReader(File.OpenRead(path));
            reader.BaseStream.Position = startOffset;
            byte[] buffer = null;

            if (numberOfBytes < 0) {
                buffer = File.ReadAllBytes(path); 
            } else {
                try {
                    buffer = reader.ReadBytes((int)numberOfBytes);
                } catch (EndOfStreamException) {
                    Console.WriteLine("Stream ended");
                } finally {
                    reader.Dispose();
                }
            }
            return buffer;
        }
        public static void WriteToArm9(long startOffset, byte[] bytesToWrite) {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(workDir + @"arm9.bin"))) {
                writer.BaseStream.Position = startOffset;
                writer.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
        }
    }
}

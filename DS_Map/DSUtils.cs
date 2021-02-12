using NarcAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace DSPRE {
    static class DSUtils {
        public static void CompressOverlay(int overlayNumber) {
            string overlayFilePath = '"' + GetOverlayPath(overlayNumber) + '"';
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
            Process decompress = new Process();
            decompress.StartInfo.FileName = @"Tools\blz.exe";
            decompress.StartInfo.Arguments = @" -d " + '"' + RomInfo.arm9Path + '"';
            decompress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            decompress.StartInfo.CreateNoWindow = true;
            decompress.Start();
            decompress.WaitForExit();

            return new FileInfo(RomInfo.arm9Path).Length> 0xBC000;
        }
        public static void editARM9size (int increment) {
            FileStream arm = File.OpenWrite(RomInfo.arm9Path);
            BinaryWriter arm9Truncate = new BinaryWriter(arm);
            arm9Truncate.BaseStream.SetLength(arm.Length + increment);
            arm9Truncate.Close();
        }
        public static bool CompressArm9() {
            Process compress = new Process();
            compress.StartInfo.FileName = @"Tools\blz.exe";
            compress.StartInfo.Arguments = @" -en9 " + '"' + RomInfo.arm9Path + '"';
            compress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            compress.StartInfo.CreateNoWindow = true;
            compress.Start();
            compress.WaitForExit();

            return new FileInfo(RomInfo.arm9Path).Length <= 0xBC000;
        }

        public static int DecompressOverlay(int overlayNumber, bool makeBackup) {
            String overlayFilePath = GetOverlayPath(overlayNumber);
            
            if (!File.Exists(overlayFilePath)) {
                MessageBox.Show("Overlay to decompress #" + overlayNumber + " doesn't exist",
                    "Overlay not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            if (makeBackup) {
                if (File.Exists(overlayFilePath + ".backup")) {
                    if (new FileInfo(overlayFilePath).Length > new FileInfo(overlayFilePath + ".backup").Length) { //if overlay is bigger than its backup
                        Console.WriteLine("Overlay " + overlayNumber + " is already uncompressed and its compressed backup exists.");
                        return 1;
                    }
                    File.Delete(overlayFilePath + ".backup");
                }
                File.Copy(overlayFilePath, overlayFilePath + ".backup");
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
            String overlayFilePath = GetOverlayPath(overlayNumber);

            if (File.Exists(overlayFilePath + ".backup")) {
                if (new FileInfo(overlayFilePath).Length <= new FileInfo(overlayFilePath + ".backup").Length) { //if overlay is bigger than its backup
                    Console.WriteLine("Overlay " + overlayNumber + " is already compressed.");
                    return;
                } else {
                    File.Delete(overlayFilePath);
                    File.Move(overlayFilePath + ".backup", overlayFilePath);
                }
            } else {
                string msg = "Overlay File " + '"' + overlayFilePath + ".backup" + '"' + " couldn't be found and restored.";
                Console.WriteLine(msg);

                if (eventEditorIsReady)
                    MessageBox.Show(msg, "Can't restore overlay from backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }
        public static string GetOverlayPath(int overlayNumber) {
            return RomInfo.workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin";
        }

        public static bool CheckOverlayHasCompressionFlag(int ovNumber) {
            bool result;
            BinaryReader f = new BinaryReader(File.OpenRead(RomInfo.overlayTablePath));
            f.BaseStream.Position = ovNumber * 32 + 31; //overlayNumber * size of entry + offset
            if (f.ReadByte() % 2 == 0)  //even
                result = false; 
            else {
                result = true; //odd
            }
            f.Close();

            return result;
        }
        public static uint GetOverlayUncompressedSize(int ovNumber) {
            BinaryReader f = new BinaryReader(File.OpenRead(RomInfo.overlayTablePath));
            f.BaseStream.Position = ovNumber * 32 + 8; //overlayNumber * size of entry + offset
            return f.ReadUInt32();
        }
        public static void SetOverlayCompressionInTable(int ovNumber, byte compressStatus) {
            if (compressStatus < 0 || compressStatus > 3) {
                Console.WriteLine("Compression status " + compressStatus + " is invalid. No operation performed.");
                return;
            }
            BinaryWriter f = new BinaryWriter(File.OpenWrite(RomInfo.overlayTablePath));
            f.BaseStream.Position = ovNumber * 32 + 31; //overlayNumber * size of entry + offset
            f.Write(compressStatus);
            f.Close();
        }
        public static byte[] ReadFromArm9(uint startOffset, long numberOfBytes) {
            return ReadFromFile(RomInfo.arm9Path, startOffset, numberOfBytes);
        }
        public static byte[] ReadFromFile(string filepath, long startOffset, long numberOfBytes) {
            FileStream f = File.OpenRead(filepath);
            BinaryReader reader = new BinaryReader(f);
            reader.BaseStream.Position = startOffset;
            byte[] buffer = null;

            try {
                if (numberOfBytes < 0) {
                    buffer = reader.ReadBytes((int)(f.Length - reader.BaseStream.Position));
                } else {
                    buffer = reader.ReadBytes((int)numberOfBytes);
                }
            } catch (EndOfStreamException) {
                Console.WriteLine("Stream ended");
            } finally {
                reader.Dispose();
            }

            return buffer;
        }
        public static void WriteToArm9(uint startOffset, byte[] bytesToWrite) {
            WriteToFile(RomInfo.arm9Path, startOffset, bytesToWrite);
        }
        public static void WriteToFile(string filepath, uint startOffset, byte[] bytesToWrite) {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filepath))) {
                writer.BaseStream.Position = startOffset;
                writer.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
        }

        public static void UnpackNarcs(List<int> IDs, ToolStripProgressBar progress) {
            string[] narcPaths = RomInfo.narcPaths;
            string[] extractedNarcDirs = RomInfo.extractedNarcDirs;

            foreach (int id in IDs) {
                (string pathToPacked, string pathToExtracted) = (narcPaths[id], extractedNarcDirs[id]);
                
                DirectoryInfo di = new DirectoryInfo(pathToExtracted);
                if (!di.Exists || di.GetFiles().Length == 0) {
                    Narc.Open(RomInfo.workDir + pathToPacked).ExtractToFolder(pathToExtracted);
                }

                if (progress != null)
                    try {
                        progress.Value++;
                    } catch (ArgumentOutOfRangeException) { }
            }
        }
        public static void ForceUnpackNarcs(List<int> IDs, ToolStripProgressBar progress) {
            string[] narcPaths = RomInfo.narcPaths;
            string[] extractedNarcDirs = RomInfo.extractedNarcDirs;

            foreach (int id in IDs) {
                (string pathToPacked, string pathToExtracted) = (narcPaths[id], extractedNarcDirs[id]);
                Narc.Open(RomInfo.workDir + pathToPacked).ExtractToFolder(pathToExtracted);

                if (progress != null)
                    try {
                        progress.Value++;
                    } catch (ArgumentOutOfRangeException) { }
            }
        }
        public static void UnpackNarc(int id) {
            string[] narcPaths = RomInfo.narcPaths;
            string[] extractedNarcDirs = RomInfo.extractedNarcDirs;

            (string pathToPacked, string pathToExtracted) tuple = (narcPaths[id], extractedNarcDirs[id]);
            DirectoryInfo di = new DirectoryInfo(tuple.Item2);
            if (!di.Exists || di.GetFiles().Length == 0) {
                Narc.Open(RomInfo.workDir + tuple.pathToPacked).ExtractToFolder(tuple.pathToExtracted);
            }
        }
        public static void ForceUnpackNarc(int id) {
            string[] narcPaths = RomInfo.narcPaths;
            string[] extractedNarcDirs = RomInfo.extractedNarcDirs;

            (string pathToPacked, string pathToExtracted) = (narcPaths[id], extractedNarcDirs[id]);
            Narc.Open(RomInfo.workDir + pathToPacked).ExtractToFolder(pathToExtracted);
        }

        public static bool OverlayIsCompressed(int ovNumber) {
            return (new FileInfo(GetOverlayPath(ovNumber)).Length < GetOverlayUncompressedSize(ovNumber));
        }
    }
}

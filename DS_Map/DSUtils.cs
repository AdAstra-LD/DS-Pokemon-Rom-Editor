using NarcAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE {
    static class DSUtils {
        public static void WriteToFile(string filepath, byte[] bytesToWrite, uint writeAt = 0, int readFrom = 0) {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filepath))) {
                writer.BaseStream.Position = writeAt;
                writer.Write(bytesToWrite, readFrom, bytesToWrite.Length - readFrom);
            }
        }
        public static byte[] ReadFromFile(string filepath, long startOffset = 0, long numberOfBytes = 0) {
            FileStream f = File.OpenRead(filepath);
            BinaryReader reader = new BinaryReader(f);
            reader.BaseStream.Position = startOffset;
            byte[] buffer = null;

            try {
                if (numberOfBytes == 0) {
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
        public static string GetOverlayPath(int overlayNumber) {
            return RomInfo.workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin";
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
        public static bool OverlayIsCompressed(int ovNumber) {
            return (new FileInfo(GetOverlayPath(ovNumber)).Length < GetOverlayUncompressedSize(ovNumber));
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
        public static void EditARM9size (int increment) {
            FileStream arm = File.OpenWrite(RomInfo.arm9Path);
            BinaryWriter arm9Truncate = new BinaryWriter(arm);
            arm9Truncate.BaseStream.SetLength(arm.Length + increment);
            arm9Truncate.Close();
        }

        public static byte[] ReadFromArm9(uint startOffset, long numberOfBytes = 0) {
            return ReadFromFile(RomInfo.arm9Path, startOffset, numberOfBytes);
        }
        public static void WriteToArm9(byte[] bytesToWrite, uint writeAt = 0, int readFrom = 0) {
            WriteToFile(RomInfo.arm9Path, bytesToWrite, writeAt, readFrom);
        }

        public static void TryUnpackNarcs(List<DirNames> IDs, ToolStripProgressBar progress = null) {
            foreach (DirNames id in IDs) {

                if (gameDirs.TryGetValue(id, out (string packedPath, string unpackedPath) paths)) {
                    DirectoryInfo di = new DirectoryInfo(paths.unpackedPath);

                    if (!di.Exists || di.GetFiles().Length == 0) {
                        Narc opened = Narc.Open(paths.packedPath);

                        if (opened == null)
                            throw new NullReferenceException();

                        opened.ExtractToFolder(paths.unpackedPath);
                    }

                    if (progress != null) {
                        try {
                            progress.Value++;
                        } catch (ArgumentOutOfRangeException) { }
                    }
                }
            }
        }
        public static void ForceUnpackNarcs(List<DirNames> IDs, ToolStripProgressBar progress = null) {
            foreach (DirNames id in IDs) {

                if (gameDirs.TryGetValue(id, out (string packedPath, string unpackedPath) paths)) {
                    Narc opened = Narc.Open(paths.packedPath);

                    if (opened == null)
                        throw new NullReferenceException();

                    opened.ExtractToFolder(paths.unpackedPath);

                    if (progress != null)
                        try {
                            progress.Value++;
                        } catch (ArgumentOutOfRangeException) { }
                }
            }
        }

        public static byte[] GetModelWithoutTextures (byte[] modelFile) {
            byte[] nsbmdHeaderData;
            uint mdl0Size;
            byte[] mdl0Data;

            using (BinaryReader modelReader = new BinaryReader(new MemoryStream(modelFile))) {
                modelReader.BaseStream.Position = 0x0;
                nsbmdHeaderData = modelReader.ReadBytes(0x8);

                modelReader.BaseStream.Position = 0x1C;
                mdl0Size = modelReader.ReadUInt32(); // Read mdl0 file size

                modelReader.BaseStream.Position = 0x18;
                mdl0Data = modelReader.ReadBytes((int)mdl0Size);
            }

            using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {

                writer.Write(nsbmdHeaderData); // Write first header bytes, same for all NSBMD.
                writer.Write(mdl0Size + 0x14);
                writer.Write((short)0x10); // Writes BMD0 header size (always 16)
                writer.Write((short)0x1); // Write new number of sub-files, since embedded textures are removed
                writer.Write(0x14); // Writes new start offset of MDL0

                writer.Write(mdl0Data); // Writes MDL0;

                return ((MemoryStream)writer.BaseStream).ToArray();
            }
        }
        public static byte[] BuildNSBTXHeader(int texturesSize) {
            MemoryStream ms = new MemoryStream();

            using (BinaryWriter bw = new BinaryWriter(ms)) {
                bw.Write((UInt32)0x30585442); // Write magic code BTX0
                bw.Write((UInt16)0xFEFF); // Byte order
                bw.Write((UInt16)0x0001); // ???
                bw.Write((UInt32)texturesSize); // Write size of textures block
                bw.Write((UInt16)0x0010); //Header size???
                bw.Write((UInt16)0x0001); //Number of blocks???
                bw.Write((UInt32)0x00000014); // Offset to block
            }
            return ms.ToArray();
        }
    }
}

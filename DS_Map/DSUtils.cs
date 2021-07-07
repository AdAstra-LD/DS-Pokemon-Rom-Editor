using LibNDSFormats.NSBMD;
using NarcAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE {
    public static class DSUtils {

        public const int NSBMD_DOESNTHAVE_TEXTURE = 0;
        public const int NSBMD_HAS_TEXTURE = 1;

        public const int ERR_OVERLAY_NOTFOUND = -1;
        public const int ERR_OVERLAY_ALREADY_UNCOMPRESSED = -2;

        public const string backupSuffix = ".backup";

        public static void WriteToFile(string filepath, byte[] bytesToWrite, uint writeAt = 0, int readFrom = 0, bool fromScratch = false) {
            if (fromScratch)
                File.Delete(filepath);

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filepath))) {
                writer.BaseStream.Position = writeAt;
                writer.Write(bytesToWrite, readFrom, bytesToWrite.Length - readFrom);
            }
        }
        public static byte[] ReadFromFile(string filepath, long startOffset = 0, long numberOfBytes = 0) {
            byte[] buffer = null;

            FileStream f = File.OpenRead(filepath);
            using (BinaryReader reader = new BinaryReader(f)) {
                reader.BaseStream.Position = startOffset;

                try {
                    if (numberOfBytes == 0) {
                        buffer = reader.ReadBytes((int)(f.Length - reader.BaseStream.Position));
                    } else {
                        buffer = reader.ReadBytes((int)numberOfBytes);
                    }
                } catch (EndOfStreamException) {
                    Console.WriteLine("Stream ended");
                }
            }

            return buffer;
        }
        public static byte[] ReadFromByteArray(byte[] input, long readFrom = 0, long numberOfBytes = 0) {
            byte[] buffer = null;

            using (BinaryReader reader = new BinaryReader(new MemoryStream(input))) {
                reader.BaseStream.Position = readFrom;

                try {
                    if (numberOfBytes == 0) {
                        buffer = reader.ReadBytes((int)(input.Length - reader.BaseStream.Position));
                    } else {
                        buffer = reader.ReadBytes((int)numberOfBytes);
                    }
                } catch (EndOfStreamException) {
                    Console.WriteLine("Stream ended");
                }
            }
            return buffer;
        }
        public static int DecompressOverlay(int overlayNumber, bool makeBackup = true) {
            string overlayFilePath = GetOverlayPath(overlayNumber);

            if (!File.Exists(overlayFilePath)) {
                MessageBox.Show("Overlay to decompress #" + overlayNumber + " doesn't exist",
                    "Overlay not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ERR_OVERLAY_NOTFOUND;
            }

            if (makeBackup) {
                if (File.Exists(overlayFilePath + backupSuffix)) {
                    File.Delete(overlayFilePath + backupSuffix);
                }
                File.Copy(overlayFilePath, overlayFilePath + backupSuffix);
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
        public static int CompressOverlay(int overlayNumber) {
            string overlayFilePath = GetOverlayPath(overlayNumber);

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
        public static string GetOverlayPath(int overlayNumber) {
            return RomInfo.workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin";
        }
        public static void RestoreOverlayFromCompressedBackup(int overlayNumber, bool eventEditorIsReady) {
            String overlayFilePath = GetOverlayPath(overlayNumber);

            if (File.Exists(overlayFilePath + backupSuffix)) {
                if (new FileInfo(overlayFilePath).Length <= new FileInfo(overlayFilePath + backupSuffix).Length) { //if overlay is bigger than its backup
                    Console.WriteLine("Overlay " + overlayNumber + " is already compressed.");
                    return;
                } else {
                    File.Delete(overlayFilePath);
                    File.Move(overlayFilePath + backupSuffix, overlayFilePath);
                }
            } else {
                string msg = "Overlay File " + '"' + overlayFilePath + backupSuffix + '"' + " couldn't be found and restored.";
                Console.WriteLine(msg);

                if (eventEditorIsReady)
                    MessageBox.Show(msg, "Can't restore overlay from backup", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        
        /**
         * Only checks if the overlay is CONFIGURED as compressed
         **/
        public static bool CheckOverlayHasCompressionFlag(int ovNumber) {
            using (BinaryReader f = new BinaryReader(File.OpenRead(RomInfo.overlayTablePath))) {
                f.BaseStream.Position = ovNumber * 32 + 31; //overlayNumber * size of entry + offset
                return f.ReadByte() % 2 != 0;
            }
        }

        /**
         * Checks the actual size of the overlay file
         **/
        public static bool OverlayIsCompressed(int ovNumber) {
            return (new FileInfo(GetOverlayPath(ovNumber)).Length < GetOverlayUncompressedSize(ovNumber));
        }
        public static uint GetOverlayUncompressedSize(int ovNumber) {
            using (BinaryReader f = new BinaryReader(File.OpenRead(RomInfo.overlayTablePath))) {
                f.BaseStream.Position = ovNumber * 32 + 8; //overlayNumber * size of entry + offset
                return f.ReadUInt32();
            }
        }
        public static uint GetOverlayRAMAddress(int ovNumber) {
            using (BinaryReader f = new BinaryReader(File.OpenRead(RomInfo.overlayTablePath))) {
                f.BaseStream.Position = ovNumber * 32 + 4; //overlayNumber * size of entry + offset
                return f.ReadUInt32();
            }
        }
        public static void SetOverlayCompressionInTable(int ovNumber, byte compressStatus) {
            if (compressStatus < 0 || compressStatus > 3) {
                Console.WriteLine("Compression status " + compressStatus + " is invalid. No operation performed.");
                return;
            }
            using (BinaryWriter f = new BinaryWriter(File.OpenWrite(RomInfo.overlayTablePath))) {
                f.BaseStream.Position = ovNumber * 32 + 31; //overlayNumber * size of entry + offset
                f.Write(compressStatus);
            }
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
        public static void RepackROM(string ndsFileName) {
            Process repack = new Process();
            repack.StartInfo.FileName = @"Tools\ndstool.exe";
            repack.StartInfo.Arguments = "-c " + '"' + ndsFileName + '"'
                + " -9 " + '"' + RomInfo.arm9Path + '"'
                + " -7 " + '"' + RomInfo.workDir + "arm7.bin" + '"'
                + " -y9 " + '"' + RomInfo.workDir + "y9.bin" + '"'
                + " -y7 " + '"' + RomInfo.workDir + "y7.bin" + '"'
                + " -d " + '"' + RomInfo.workDir + "data" + '"'
                + " -y " + '"' + RomInfo.workDir + "overlay" + '"'
                + " -t " + '"' + RomInfo.workDir + "banner.bin" + '"'
                + " -h " + '"' + RomInfo.workDir + "header.bin" + '"';

            Application.DoEvents();
            repack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            repack.StartInfo.CreateNoWindow = true;
            repack.Start();
            repack.WaitForExit();
        }

        public static byte[] ReadFromArm9(uint startOffset, long numberOfBytes = 0) {
            return ReadFromFile(RomInfo.arm9Path, startOffset, numberOfBytes);
        }
        public static void WriteToArm9(byte[] bytesToWrite, uint writeAt = 0, int readFrom = 0) {
            WriteToFile(RomInfo.arm9Path, bytesToWrite, writeAt, readFrom);
        }

        public static byte[] StringToByteArray(String hex) {
            //Ummm what?
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        public static byte[] HexStringToByteArray(string hexString) {
            //FC B5 05 48 C0 46 41 21 
            //09 22 02 4D A8 47 00 20 
            //03 21 FC BD F1 64 00 02 
            //00 80 3C 02
            if (hexString is null)
                return null;

            hexString = hexString.Trim();

            byte[] b = new byte[hexString.Length / 3 + 1];
            for (int i = 0; i < hexString.Length; i += 2) {
                if (hexString[i] == ' ') {
                    hexString = hexString.Substring(1, hexString.Length - 1);
                }

                b[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return b;
        }

        public static void TryUnpackNarcs(List<DirNames> IDs, ToolStripProgressBar progress = null) {
            foreach (DirNames id in IDs) {

                if (gameDirs.TryGetValue(id, out (string packedPath, string unpackedPath) paths)) {
                    DirectoryInfo di = new DirectoryInfo(paths.unpackedPath);

                    if (!di.Exists || di.GetFiles().Length == 0) {
                        Narc opened = Narc.Open(paths.packedPath);

                        if (opened is null)
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

                    if (opened is null)
                        throw new NullReferenceException();

                    opened.ExtractToFolder(paths.unpackedPath);

                    if (progress != null) {
                        try {
                            progress.Value++;
                        } catch (ArgumentOutOfRangeException) { }
                    }
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

            MemoryStream output = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(output)) {

                writer.Write(nsbmdHeaderData); // Write first header bytes, same for all NSBMD.
                writer.Write(mdl0Size + 0x14);
                writer.Write((short)0x10); // Writes BMD0 header size (always 16)
                writer.Write((short)0x1); // Write new number of sub-files, since embedded textures are removed
                writer.Write((uint)0x14); // Writes new start offset of MDL0

                writer.Write(mdl0Data); // Writes MDL0;
            }
            return output.ToArray();
        }
        
        public static byte[] GetTexturesFromTexturedNSBMD(byte[] modelFile) {
            using (BinaryReader byteArrReader = new BinaryReader(new MemoryStream(modelFile))) {
                byteArrReader.BaseStream.Position = 14;
                if (byteArrReader.ReadUInt16() < 2) //No textures
                    return new byte[0];

                byteArrReader.BaseStream.Position = 20;
                int texAbsoluteOffset = byteArrReader.ReadInt32();

                byteArrReader.BaseStream.Position = texAbsoluteOffset + 4;
                uint textureSize = byteArrReader.ReadUInt32();

                byte[] nsbtxHeader = DSUtils.BuildNSBTXHeader(20 + textureSize);
                byte[] texData = DSUtils.ReadFromByteArray(modelFile, readFrom: texAbsoluteOffset);

                byte[] output = new byte[nsbtxHeader.Length + texData.Length];
                Buffer.BlockCopy(nsbtxHeader, 0, output, 0, nsbtxHeader.Length);
                Buffer.BlockCopy(texData, 0, output, nsbtxHeader.Length, texData.Length);
                return output;
            }
        }
        public static int CheckNSBMDHeader(byte[] modelFile) {
            using (BinaryReader byteArrReader = new BinaryReader(new MemoryStream(modelFile))) {
                if (byteArrReader.ReadUInt32() != NSBMD.NDS_TYPE_BMD0) {
                    MessageBox.Show("Please select an NSBMD file.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }

                byteArrReader.BaseStream.Position = 0xE;
                return byteArrReader.ReadInt16() >= 2 ? NSBMD_HAS_TEXTURE : NSBMD_DOESNTHAVE_TEXTURE;
            }
        }
        public static byte[] BuildNSBTXHeader(uint texturesSize) {
            MemoryStream ms = new MemoryStream();

            using (BinaryWriter bw = new BinaryWriter(ms)) {
                bw.Write(Encoding.UTF8.GetBytes("BTX0")); // Write magic code BTX0
                bw.Write((ushort)0xFEFF); // Byte order
                bw.Write((ushort)0x0001); // ???
                bw.Write(texturesSize); // Write size of textures block
                bw.Write((short)0x10); //Header size 
                bw.Write((short)0x01); //Number of sub-files???
                bw.Write((uint)0x14); // Offset to sub-file
            }
            return ms.ToArray();
        }
        public static byte[] BuildNSBMDwithTextures(byte[] nsbmd, byte[] nsbtx) {
            byte[] wholeMDL0 = GetFirstBlock(nsbmd);
            byte[] wholeTEX0 = GetFirstBlock(nsbtx);

            MemoryStream ms = new MemoryStream();
            using (BinaryWriter msWriter = new BinaryWriter(ms)) {
                msWriter.Write(NSBMD.NDS_TYPE_BMD0);
                msWriter.Write(NSBMD.NDS_TYPE_BYTEORDER);
                msWriter.Write(NSBMD.NDS_TYPE_UNK2);

                ushort nBlocks = 2;
                uint modelLength = (uint)(wholeMDL0.Length + NSBMD.HEADERSIZE + 4 * nBlocks);
                msWriter.Write((uint)(modelLength + wholeTEX0.Length));
                msWriter.Write(NSBMD.HEADERSIZE); //Header size, always 16
                msWriter.Write(nBlocks); //Number of blocks, now it's 2 because we are inserting textures

                msWriter.Write((uint)(msWriter.BaseStream.Position + 4 * nBlocks)); //Absolute offset to model data. We are gonna have to write two offsets
                    
                msWriter.Write(modelLength); //Copy offset to TEX0
                msWriter.Write(wholeMDL0);
                msWriter.Write(wholeTEX0);
                
            }
            return ms.ToArray();
        }
        private static byte[] GetFirstBlock(byte[] NSBFile) {
            int blockSize;
            uint offsetToMainBlock;
            using (BinaryReader reader = new BinaryReader(new MemoryStream(NSBFile))) {
                reader.BaseStream.Position = 16;
                offsetToMainBlock = reader.ReadUInt32();

                reader.BaseStream.Position = offsetToMainBlock + 4;
                blockSize = reader.ReadInt32();
            }
            byte[] blockData = new byte[blockSize];
            Buffer.BlockCopy(NSBFile, (int)offsetToMainBlock, blockData, 0, blockSize);

            return blockData;
        }
    }
}

using Ekona.Images;
using Images;
using LibNDSFormats.NSBMD;
using Microsoft.WindowsAPICodePack.Dialogs;
using NarcAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE {
    public static class DSUtils {

        public const int ERR_OVERLAY_NOTFOUND = -1;
        public const int ERR_OVERLAY_ALREADY_UNCOMPRESSED = -2;

        public const string backupSuffix = ".backup";

        public static readonly string NDSRomFilter = "NDS File (*.nds)|*.nds";
        public class EasyReader : BinaryReader {
            public EasyReader(string path, long pos = 0) : base(File.OpenRead(path)) {
                this.BaseStream.Position = pos;
            }
        }
        public class EasyWriter : BinaryWriter {
            public EasyWriter(string path, long pos = 0, FileMode fmode = FileMode.OpenOrCreate) : base(new FileStream(path, fmode, FileAccess.Write, FileShare.None)) {
                this.BaseStream.Position = pos;
            }
            public void EditSize(int increment) {
                this.BaseStream.SetLength(this.BaseStream.Length + increment);
            }
        }

        public static void WriteToFile(string filepath, byte[] toOutput, uint writeAt = 0, int indexFirstByteToWrite = 0, int? indexLastByteToWrite = null, FileMode fmode = FileMode.OpenOrCreate) {
            using (EasyWriter writer = new EasyWriter(filepath, writeAt, fmode)) {
                writer.Write(toOutput, indexFirstByteToWrite, indexLastByteToWrite is null ? toOutput.Length - indexFirstByteToWrite : (int)indexLastByteToWrite);
            }
        }
        public static byte[] ReadFromFile(string filepath, long startOffset = 0, long numberOfBytes = 0) {
            byte[] buffer = null;

            using (EasyReader reader = new EasyReader(filepath, startOffset)) {
                try {
                    buffer = reader.ReadBytes(numberOfBytes == 0 ? (int)(reader.BaseStream.Length - reader.BaseStream.Position) : (int)numberOfBytes);
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
        public static Process CreateDecompressProcess(string path) {
            Process decompress = new Process();
            decompress.StartInfo.FileName = @"Tools\blz.exe";
            decompress.StartInfo.Arguments = @" -d " + '"' + path + '"';
            decompress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            decompress.StartInfo.CreateNoWindow = true;
            return decompress;

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

        public static void TryUnpackNarcs(List<DirNames> IDs) {
            if (gameDirs == null || gameDirs.Count == 0) {
                return;
            }    
            Parallel.ForEach(IDs, id => {
                if (gameDirs.TryGetValue(id, out (string packedPath, string unpackedPath) paths)) {
                    DirectoryInfo di = new DirectoryInfo(paths.unpackedPath);

                    if (!di.Exists || di.GetFiles().Length == 0) {
                        Narc opened = Narc.Open(paths.packedPath) ?? throw new NullReferenceException();
                        opened.ExtractToFolder(paths.unpackedPath);
                    }
                }
            });
        }
        public static void ForceUnpackNarcs(List<DirNames> IDs) {
            Parallel.ForEach(IDs, id => {
                if (gameDirs.TryGetValue(id, out (string packedPath, string unpackedPath) paths)) {
                    Narc opened = Narc.Open(paths.packedPath);

                    if (opened is null) {
                        throw new NullReferenceException();
                    }

                    opened.ExtractToFolder(paths.unpackedPath);
                }
            });
        }

        public static Image GetPokePic(int species, int w, int h) {
            PaletteBase paletteBase;
            bool fiveDigits = false; // some extreme future proofing
            string filename = "0000";

            try {
                paletteBase = new NCLR(gameDirs[DirNames.monIcons].unpackedDir + "\\" + filename, 0, filename);
            } catch (FileNotFoundException) {
                filename += '0';
                paletteBase = new NCLR(gameDirs[DirNames.monIcons].unpackedDir + "\\" + filename, 0, filename);
                fiveDigits = true;
            }

            // read arm9 table to grab pal ID
            int paletteId = 0;
            string iconTablePath;

            int iconPalTableOffsetFromFileStart;
            string ov129path = OverlayUtils.GetPath(129);
            if (File.Exists(ov129path)) {
                // if overlay 129 exists, read it from there
                iconPalTableOffsetFromFileStart = (int)(RomInfo.monIconPalTableAddress - OverlayUtils.OverlayTable.GetRAMAddress(129));
                iconTablePath = ov129path;
            } else if ((int)(RomInfo.monIconPalTableAddress - RomInfo.synthOverlayLoadAddress) >= 0) {
                // if there is a synthetic overlay, read it from there
                iconPalTableOffsetFromFileStart = (int)(RomInfo.monIconPalTableAddress - RomInfo.synthOverlayLoadAddress);
                iconTablePath = gameDirs[DirNames.synthOverlay].unpackedDir + "\\" + PatchToolboxDialog.expandedARMfileID.ToString("D4");
            } else {
                // default handling
                iconPalTableOffsetFromFileStart = (int)(RomInfo.monIconPalTableAddress - ARM9.address);
                iconTablePath = RomInfo.arm9Path;
            }

            using (DSUtils.EasyReader idReader = new DSUtils.EasyReader(iconTablePath, iconPalTableOffsetFromFileStart + species)) {
                paletteId = idReader.ReadByte();
            }

            if (paletteId != 0) {
                paletteBase.Palette[0] = paletteBase.Palette[paletteId]; // update pal 0 to be the new pal
            }

            // grab tiles
            int spriteFileID = species + 7;
            string spriteFilename = spriteFileID.ToString("D" + (fiveDigits ? "5" : "4"));
            ImageBase imageBase = new NCGR(gameDirs[DirNames.monIcons].unpackedDir + "\\" + spriteFilename, spriteFileID, spriteFilename);

            // grab sprite
            int ncerFileId = 2;
            string ncerFileName = ncerFileId.ToString("D" + (fiveDigits ? "5" : "4"));
            SpriteBase spriteBase = new NCER(gameDirs[DirNames.monIcons].unpackedDir + "\\" + ncerFileName, 2, ncerFileName);

            // copy this from the trainer
            int bank0OAMcount = spriteBase.Banks[0].oams.Length;
            int[] OAMenabled = new int[bank0OAMcount];
            for (int i = 0; i < OAMenabled.Length; i++) {
                OAMenabled[i] = i;
            }

            // finally compose image
            try {
                return spriteBase.Get_Image(imageBase, paletteBase, 0, w, h, false, false, false, true, true, -1, OAMenabled);
            } catch (FormatException) {
                return Properties.Resources.IconPokeball;
            }
        }
    }
}

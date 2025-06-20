using System;
using System.Diagnostics;
using System.IO;
using static DSPRE.RomInfo;

namespace DSPRE {
    public static class ARM9 {
        private const int MAX_SIZE = 0xBC000;
        public static readonly uint address = 0x02000000;
        public class Reader : DSUtils.EasyReader {
            public Reader(long pos = 0) : base(arm9Path, pos) {
                this.BaseStream.Position = pos;
            }
        }
        public class Writer : DSUtils.EasyWriter {
            public Writer(long pos = 0) : base(arm9Path, pos) {
                this.BaseStream.Position = pos;
            }
        }
        public static void EditSize(int increment) {
            using (Writer w = new Writer()) {
                w.EditSize(increment);
            }
        }
        public static bool Decompress(string path) {
            Process decompress = DSUtils.CreateDecompressProcess(path);
            decompress.Start();
            decompress.WaitForExit();

            return new FileInfo(path).Length > MAX_SIZE;
        }

        public static bool Compress(string path) {
            Process compress = new Process();
            compress.StartInfo.FileName = @"Tools\blz.exe";
            compress.StartInfo.Arguments = @" -en9 " + '"' + path + '"';
            compress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            compress.StartInfo.CreateNoWindow = true;
            compress.Start();
            compress.WaitForExit();

            return new FileInfo(path).Length <= MAX_SIZE;
        }
        public static bool CheckCompressionMark() {
            return BitConverter.ToInt32(ReadBytes((uint)(RomInfo.gameFamily == GameFamilies.DP ? 0xB7C : 0xBB4), 4), 0) != 0;
        }

        public static byte[] ReadBytes(uint startOffset, long numberOfBytes = 0) {
            return DSUtils.ReadFromFile(RomInfo.arm9Path, startOffset, numberOfBytes);
        }
        public static void WriteBytes(byte[] bytesToWrite, uint destOffset, int indexFirstByteToWrite = 0, int? indexLastByteToWrite = null) {
            DSUtils.WriteToFile(RomInfo.arm9Path, bytesToWrite, destOffset, indexFirstByteToWrite, indexLastByteToWrite);
        }

        public static ushort ReadWordLE(uint startOffset)
        {
            byte[] bytes = ReadBytes(startOffset, 2);
            ushort word = (ushort)(bytes[0] | (bytes[1] << 8));
            return word;
        }

        public static ushort ReadWordBE(uint startOffset)
        {
            byte[] bytes = ReadBytes(startOffset, 2);
            ushort word = (ushort)((bytes[0] << 8) | bytes[1]);
            return word;
        }

        public static byte ReadByte(uint startOffset) {
            return DSUtils.ReadFromFile(RomInfo.arm9Path, startOffset, 1)[0];
        }
        public static void WriteByte(byte value, uint destOffset) {
            DSUtils.WriteToFile(RomInfo.arm9Path, BitConverter.GetBytes(value), destOffset, 0);
        }
    }
}

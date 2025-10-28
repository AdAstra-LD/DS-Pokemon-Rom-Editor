using System;
using System.IO;
using System.Windows.Forms;
using static DSPRE.DSUtils;
using static DSPRE.RomInfo;

namespace DSPRE
{
    public static class OverlayUtils
    {
        public static class OverlayTable
        {
            private const int ENTRY_LEN = 32;

            public static bool IsDefaultCompressed(int ovNumber)
            {
                using (var f = new EasyReader(RomInfo.overlayTablePath, ovNumber * ENTRY_LEN + 31))
                    return (f.ReadByte() & 1) == 1;
            }

            public static void SetDefaultCompressed(int ovNumber, bool compressStatus)
            {
                DSUtils.WriteToFile(RomInfo.overlayTablePath,
                    new byte[] { compressStatus ? (byte)1 : (byte)0 },
                    (uint)(ovNumber * ENTRY_LEN + 31));
            }

            public static uint GetRAMAddress(int ovNumber)
            {
                using (var f = new EasyReader(RomInfo.overlayTablePath, ovNumber * ENTRY_LEN + 4))
                    return f.ReadUInt32();
            }

            public static uint GetUncompressedSize(int ovNumber)
            {
                using (var f = new EasyReader(RomInfo.overlayTablePath, ovNumber * ENTRY_LEN + 8))
                    return f.ReadUInt32();
            }

            public static void SetUncompressedSize(int ovNumber, uint newSize)
            {
                using (var f = new EasyWriter(RomInfo.overlayTablePath, ovNumber * ENTRY_LEN + 8))
                    f.Write(newSize);
            }

            public static int GetNumberOfOverlays()
            {
                using (var fs = File.OpenRead(RomInfo.overlayTablePath))
                    return (int)(fs.Length / ENTRY_LEN);
            }
        }

        public static string GetPath(int overlayNumber) =>
            $"{workDir}overlay\\overlay_{overlayNumber:D4}.bin";

        public static bool IsCompressed(int ovNumber)
        {
            var info = new FileInfo(GetPath(ovNumber));
            var uncompSize = OverlayTable.GetUncompressedSize(ovNumber);
            return info.Length < uncompSize;
        }

        public static void RestoreFromCompressedBackup(int overlayNumber, bool eventEditorIsReady)
        {
            string path = GetPath(overlayNumber);
            string backup = path + DSUtils.backupSuffix;

            if (File.Exists(backup))
            {
                if (new FileInfo(path).Length <= new FileInfo(backup).Length)
                {
                    AppLogger.Info($"Overlay {overlayNumber} is already compressed.");
                    return;
                }

                File.Delete(path);
                File.Move(backup, path);
            }
            else
            {
                string msg = $"Overlay File {backup} couldn't be found and restored.";
                AppLogger.Debug(msg);
                if (eventEditorIsReady)
                    MessageBox.Show(msg, "Can't restore overlay from backup",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================================================================
        // REAL BLZ (CUE) - NITRO DS OVERLAY COMPRESSION
        // ====================================================================

        private const int BLZ_SHIFT = 1;
        private const int BLZ_MASK = 0x80;
        private const int BLZ_THRESHOLD = 2;
        private const int BLZ_N = 0x1002;
        private const int BLZ_F = 0x12;

        private static void FatalError(string msg) => throw new InvalidOperationException(msg);

        private static void BLZ_Invert(byte[] buffer, int offset, int length)
        {
            int end = offset + length - 1;
            for (int i = offset; i < end; i++, end--)
            {
                byte t = buffer[i];
                buffer[i] = buffer[end];
                buffer[end] = t;
            }
        }

        public static byte[] BLZ_Compress(byte[] raw, bool best = false)
        {
            int rawLen = raw.Length;
            if (rawLen == 0) return new byte[4];

            // Make working copy and reverse (BLZ works backwards)
            byte[] work = new byte[rawLen];
            Buffer.BlockCopy(raw, 0, work, 0, rawLen);
            BLZ_Invert(work, 0, rawLen);

            int pakCap = rawLen + ((rawLen + 7) / 8) + 11;
            byte[] pak = new byte[pakCap];
            int pakPos = 0;
            int rawPos = 0;
            byte mask = 0;
            int flgPos = -1;

            while (rawPos < rawLen)
            {
                if ((mask >>= BLZ_SHIFT) == 0)
                {
                    flgPos = pakPos++;
                    pak[flgPos] = 0;
                    mask = BLZ_MASK;
                }

                // === GREEDY SEARCH: longest match, farthest first ===
                int bestLen = BLZ_THRESHOLD;
                int bestPos = 0;
                int maxBack = Math.Min(BLZ_N, rawPos);

                for (int pos = maxBack; pos >= 3; pos--) // FARTHEST FIRST
                {
                    int len = 0;
                    while (len < BLZ_F &&
                           rawPos + len < rawLen &&
                           work[rawPos + len] == work[rawPos + len - pos])
                        len++;

                    if (len > bestLen || (len == bestLen && pos > bestPos))
                    {
                        bestLen = len;
                        bestPos = pos;
                        if (len == BLZ_F) break;
                    }
                }

                // === LZ-CUE best mode (optional) ===
                if (best && bestLen > BLZ_THRESHOLD && rawPos + bestLen < rawLen)
                {
                    int temp = rawPos + bestLen;
                    int nextLen = 0, nextPos = 0;
                    int max = Math.Min(BLZ_N, temp);
                    for (int pos = max; pos >= 3; pos--)
                    {
                        int len = 0;
                        while (len < BLZ_F && temp + len < rawLen && work[temp + len] == work[temp + len - pos])
                            len++;
                        if (len > nextLen || (len == nextLen && pos > nextPos))
                        {
                            nextLen = len;
                            nextPos = pos;
                            if (len == BLZ_F) break;
                        }
                    }
                    if (nextLen <= BLZ_THRESHOLD) nextLen = 1;

                    temp = rawPos + bestLen - 1;
                    int postLen = 0, postPos = 0;
                    max = Math.Min(BLZ_N, temp);
                    for (int pos = max; pos >= 3; pos--)
                    {
                        int len = 0;
                        while (len < BLZ_F && temp + len < rawLen && work[temp + len] == work[temp + len - pos])
                            len++;
                        if (len > postLen || (len == postLen && pos > postPos))
                        {
                            postLen = len;
                            postPos = pos;
                            if (len == BLZ_F) break;
                        }
                    }
                    if (postLen <= BLZ_THRESHOLD) postLen = 1;

                    if (bestLen + nextLen <= 1 + postLen)
                        bestLen = 1;
                }

                // === Encode ===
                pak[flgPos] = (byte)(pak[flgPos] << 1);
                if (bestLen > BLZ_THRESHOLD)
                {
                    rawPos += bestLen;
                    pak[flgPos] |= 1;
                    pak[pakPos++] = (byte)(((bestLen - 3) << 4) | ((bestPos - 3) >> 8));
                    pak[pakPos++] = (byte)(bestPos - 3);
                }
                else
                {
                    pak[pakPos++] = work[rawPos++];
                }
            }

            // Finalize flag byte
            while (mask > 1)
            {
                mask >>= 1;
                pak[flgPos] = (byte)(pak[flgPos] << 1);
            }

            int encLen = pakPos;

            // === Reverse back ===
            BLZ_Invert(work, 0, rawLen);
            BLZ_Invert(pak, 0, encLen);

            // === Fallback to raw if not smaller ===
            int compressedSize = (encLen + 3) & ~3;
            if (rawLen + 4 < compressedSize + 8)
            {
                byte[] result = new byte[rawLen + 4];
                Buffer.BlockCopy(work, 0, result, 0, rawLen);
                return result; // inc_len = 0
            }

            // === Build final BLZ file ===
            int incLen = rawLen - encLen;
            int hdrLen = 8;
            while ((encLen + hdrLen) % 4 != 0) hdrLen++;

            int total = encLen + hdrLen + 4;
            byte[] final = new byte[total];

            // Copy encoded data
            Buffer.BlockCopy(pak, 0, final, 0, encLen);

            // Pad to align
            int padStart = encLen;
            while ((padStart + hdrLen) % 4 != 0)
                final[padStart++] = 0xFF;

            // Write header (little-endian)
            int encodedTotal = encLen + hdrLen;
            final[padStart++] = (byte)encodedTotal;
            final[padStart++] = (byte)(encodedTotal >> 8);
            final[padStart++] = (byte)(encodedTotal >> 16);
            final[padStart++] = (byte)hdrLen;

            int extra = incLen - hdrLen;
            final[padStart++] = (byte)extra;
            final[padStart++] = (byte)(extra >> 8);
            final[padStart++] = (byte)(extra >> 16);
            final[padStart++] = (byte)(extra >> 24);

            Array.Resize(ref final, padStart);
            return final;
        }

        public static byte[] BLZ_Decompress(byte[] pakData)
        {
            if (pakData.Length < 4) FatalError("BLZ: File too small");

            int incLen = BitConverter.ToInt32(pakData, pakData.Length - 4);
            int rawLen;

            if (incLen == 0)
            {
                rawLen = pakData.Length - 4;
                byte[] rawData = new byte[rawLen]; // Renamed from 'raw' to 'rawData'
                Buffer.BlockCopy(pakData, 0, rawData, 0, rawLen);
                return rawData;
            }

            if (pakData.Length < 8) FatalError("BLZ: Bad header");
            int hdrLen = pakData[pakData.Length - 5];
            if (hdrLen < 8 || hdrLen > 11) FatalError("BLZ: Bad header length");

            int encLen = BitConverter.ToInt32(pakData, pakData.Length - 8) & 0x00FFFFFF;
            int decLen = pakData.Length - encLen;
            rawLen = decLen + encLen + incLen;
            if (rawLen > 0x00FFFFFF) FatalError("BLZ: Bad decoded length");

            byte[] raw = new byte[rawLen];
            Buffer.BlockCopy(pakData, 0, raw, 0, decLen);

            byte[] enc = new byte[encLen - hdrLen];
            Buffer.BlockCopy(pakData, decLen, enc, 0, encLen - hdrLen);
            BLZ_Invert(enc, 0, encLen - hdrLen);

            int rawPos = decLen;
            int encPos = 0;
            byte flags = 0, mask = 0;

            while (rawPos < rawLen)
            {
                if ((mask >>= BLZ_SHIFT) == 0)
                {
                    if (encPos >= enc.Length) break;
                    flags = enc[encPos++];
                    mask = BLZ_MASK;
                }

                if ((flags & mask) == 0)
                {
                    if (encPos >= enc.Length) break;
                    raw[rawPos++] = enc[encPos++];
                }
                else
                {
                    if (encPos + 1 >= enc.Length) break;
                    int pos = enc[encPos++] << 8;
                    pos |= enc[encPos++];
                    int len = (pos >> 12) + BLZ_THRESHOLD + 1;
                    if (rawPos + len > rawLen)
                        len = rawLen - rawPos;
                    pos = (pos & 0xFFF) + 3;

                    for (int i = 0; i < len; i++)
                        raw[rawPos + i] = raw[rawPos + i - pos];
                    rawPos += len;
                }
            }

            BLZ_Invert(raw, decLen, rawLen - decLen);
            Array.Resize(ref raw, rawPos);
            return raw;
        }

        // ====================================================================
        // PUBLIC API
        // ====================================================================

        public static int Compress(int overlayNumber)
        {
            string path = GetPath(overlayNumber);
            if (!File.Exists(path))
            {
                MessageBox.Show($"Overlay to compress #{overlayNumber} doesn't exist",
                                "Overlay not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ERR_OVERLAY_NOTFOUND;
            }

            byte[] data = File.ReadAllBytes(path);
            uint oldSize = OverlayTable.GetUncompressedSize(overlayNumber);
            if (data.Length != oldSize)
                OverlayTable.SetUncompressedSize(overlayNumber, (uint)data.Length);

            try
            {
                byte[] compressed = BLZ_Compress(data, best: false); // normal mode
                File.WriteAllBytes(path, compressed);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"BLZ compression failed: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public static int Decompress(string overlayFilePath, bool makeBackup = true)
        {
            if (!File.Exists(overlayFilePath))
            {
                MessageBox.Show($"File to decompress \"{overlayFilePath}\" doesn't exist",
                                "Overlay not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ERR_OVERLAY_NOTFOUND;
            }

            if (makeBackup)
            {
                string backup = overlayFilePath + backupSuffix;
                if (File.Exists(backup)) File.Delete(backup);
                File.Copy(overlayFilePath, backup);
            }

            byte[] compData = File.ReadAllBytes(overlayFilePath);

            try
            {
                byte[] decompressed = BLZ_Decompress(compData);
                File.WriteAllBytes(overlayFilePath, decompressed);

                string fileName = Path.GetFileName(overlayFilePath);
                if (fileName.StartsWith("overlay_") && fileName.EndsWith(".bin"))
                {
                    if (int.TryParse(fileName.Substring(8, fileName.Length - 12), out int ovNum))
                    {
                        OverlayTable.SetUncompressedSize(ovNum, (uint)decompressed.Length);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"BLZ decompression failed: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public static int Decompress(int overlayNumber, bool makeBackup = true) =>
            Decompress(GetPath(overlayNumber), makeBackup);
    }
}
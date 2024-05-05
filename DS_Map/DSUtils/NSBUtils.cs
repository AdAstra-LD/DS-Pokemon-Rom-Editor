using LibNDSFormats.NSBMD;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DSPRE {
    public static class NSBUtils {
        public const int NSBMD_DOESNTHAVE_TEXTURE = 0;
        public const int NSBMD_HAS_TEXTURE = 1;

        public static string ReadNSBMDname(BinaryReader reader, long? startPos = null) {
            if (startPos != null) {
                reader.BaseStream.Position = (long)startPos;
            }

            if (reader.ReadUInt32() == NSBMD.NDS_TYPE_MDL0) { //MDL0
                reader.BaseStream.Position += 0x1c;
            } else {
                reader.BaseStream.Position += 0x1c + 4;
            }

            return Encoding.UTF8.GetString(reader.ReadBytes(16));
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

        public static byte[] GetModelWithoutTextures(byte[] modelFile) {
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

                byte[] nsbtxHeader = NSBUtils.BuildNSBTXHeader(20 + textureSize);
                byte[] texData = DSUtils.ReadFromByteArray(modelFile, readFrom: texAbsoluteOffset);

                byte[] output = new byte[nsbtxHeader.Length + texData.Length];
                Buffer.BlockCopy(nsbtxHeader, 0, output, 0, nsbtxHeader.Length);
                Buffer.BlockCopy(texData, 0, output, nsbtxHeader.Length, texData.Length);
                return output;
            }
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
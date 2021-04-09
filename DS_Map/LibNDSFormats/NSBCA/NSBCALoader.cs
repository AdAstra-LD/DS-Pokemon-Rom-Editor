using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LibNDSFormats.NSBMD;

namespace LibNDSFormats.NSBCA {
    /// <summary>
	/// Loader for NSBCA files & data.
	/// </summary>
    public static class NSBCALoader {
        #region Methods (2)

        // Public Methods (2) 

        /// <summary>
        /// Load NSBTX from stream.
        /// </summary>
        /// <param name="stream">Stream to use.</param>
        /// <returns>Material definitions.</returns>
        public static IEnumerable<NSBMDAnimation> LoadNsbca(Stream stream) {
            List<NSBMDAnimation> animation = new List<NSBMDAnimation>();
            var reader = new EndianBinaryReader(stream, Endianness.LittleEndian);
            byte[] id = reader.ReadBytes(4);
            if (id == new byte[] { 0x42, 0x43, 0x41, 0x30 }) {
                throw new Exception();
            }

            reader.BaseStream.Position += 2;
            int i = reader.ReadUInt16();
            if (i == NSBMD.NSBMD.NDS_TYPE_UNK1) {
                i = reader.ReadInt32();
                if (i == stream.Length) {
                    int numblock = reader.ReadInt32();
                    numblock >>= 16;
                    int r = reader.ReadInt32();
                    id = reader.ReadBytes(4);
                    if (numblock == 1 && r == 0x14) {
                        animation.AddRange(ReadJnt0(stream, 0x14));
                    }
                }
            }
            reader.Close();
            return animation;
        }

        /// <summary>
        /// Load NSBTX from file.
        /// </summary>
        /// <param name="stream">File to use.</param>
        /// <returns>Material definitions.</returns>
        public static IEnumerable<NSBMDAnimation> LoadNsbca(FileInfo fileInfo) {
            IEnumerable<NSBMDAnimation> result = null;
            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open)) {
                result = LoadNsbca(fileStream);
            }
            return result;
        }

        /// <summary>
        /// Load materials in stream.
        /// </summary>
        /// <param name="stream">Stream to use.</param>
        /// <returns>Material definitions.</returns>
        public static IEnumerable<NSBMDAnimation> ReadJnt0(Stream stream, int blockoffset) {
            EndianBinaryReader reader = new EndianBinaryReader(stream, Endianness.LittleEndian);
            int blocksize, blockptr, blocklimit;
            int num, objnum, i, j, r;
            List<int> dataoffset = new List<int>();
            int sec1offset, sec2offset;
            NSBMDAnimation[] animation = new NSBMDAnimation[1];
            List<int> animlen = new List<int>();

            ////////////////////////////////////////////////
            // joint
            blockptr = blockoffset + 4;			// already read the ID, skip 4 bytes
            blocksize = reader.ReadInt32();		// block size
            blocklimit = blocksize + blockoffset;

            reader.ReadByte();					// skip dummy 0
            num = reader.ReadByte(); //assert(num > 0);	// no of joint must == 1
            Console.WriteLine("No. of Joint = %02x\n", num);

            //dataoffset = (int*)malloc(sizeof(int));
            //if (!dataoffset) return NULL;

            reader.BaseStream.Seek(10 + (num << 2), SeekOrigin.Current);		// skip [char xyz], useless
            blockptr += 10;

            reader.BaseStream.Seek(4, SeekOrigin.Current);				// go straight to joint data offset
            blockptr += 4;
            for (i = 0; i < num; i++)
                dataoffset.Add(getdword(reader.ReadBytes(4)) + blockoffset);

            //fseek( fnsbca, 16 * num, SEEK_CUR );		// skip names
            blockptr += 16 * num;

            for (i = 0; i < num; i++) {
                reader.BaseStream.Seek(dataoffset[i], SeekOrigin.Begin);
                //j = getdword();
                if (reader.ReadBytes(4) == new byte[] { 0x4A, 0x00, 0x41, 0x43 }) return null;
                blockptr += 4;

                animlen.Add(getword(reader.ReadBytes(2)));
                objnum = getword(reader.ReadBytes(2));
                //if (objnum != g_model[0].objnum) return NULL;
                blockptr += 4;

                //animation = (ANIMATION*)calloc(sizeof(ANIMATION), objnum);
                //if (!animation) return NULL;
                animation = new NSBMDAnimation[objnum];

                reader.BaseStream.Seek(4, SeekOrigin.Current);	// skip 4 zeros
                blockptr += 4;

                sec1offset = getdword(reader.ReadBytes(4)) + dataoffset[i];
                sec2offset = getdword(reader.ReadBytes(4)) + dataoffset[i];
                blockptr += 8;

                for (j = 0; j < objnum; j++) {
                    animation[j] = new NSBMDAnimation();
                    animation[j].dataoffset = getword(reader.ReadBytes(2)) + dataoffset[i];
                }

                for (j = 0; j < objnum; j++) {
                    NSBMD.NSBMDAnimation anim = animation[j];
                    r = getdword(reader.ReadBytes(4));
                    anim.flag = r;
                    // if ((r >> 1 & 1) == 0)
                    //{		// any transformation?
                    if ((r >> 1 & 1) == 0) {    // translation
                        if ((r & 4) == 1) { // use Base T
                        } else {
                            if ((r & 8) == 1) { // consTX
                                anim.m_trans[0] = ((float)getdword(reader.ReadBytes(4))) / 4096.0f;
                            } else {
                            }
                            if ((r & 0x10) == 1) {  // consTY
                                anim.m_trans[1] = ((float)getdword(reader.ReadBytes(4))) / 4096.0f;
                            } else {
                            }
                            if ((r & 0x20) == 1) {  // consTZ
                                anim.m_trans[0] = ((float)getdword(reader.ReadBytes(4))) / 4096.0f;
                            } else {
                            }
                        }
                    }
                    if ((r >> 6 & 1) == 0) {    // rotation
                        if ((r & 0x100) == 1) { // constR
                            anim.a = ((float)getword(reader.ReadBytes(2))) / 4096.0f;
                            anim.b = ((float)getword(reader.ReadBytes(2))) / 4096.0f;
                        } else {
                        }
                    }
                    if ((r >> 9 & 1) == 0) {    // scale
                        if ((r & 0x400) == 1) { // use Base S
                        } else {
                            if ((r & 0x800) == 1) { // consSX
                                anim.m_scale[0] = ((float)getdword(reader.ReadBytes(4))) / 4096.0f;
                            } else {
                            }
                            if ((r & 0x1000) == 1) {// consSY
                                anim.m_scale[0] = ((float)getdword(reader.ReadBytes(4))) / 4096.0f;
                            } else {
                            }
                            if ((r & 0x2000) == 1) {// consSZ
                                anim.m_scale[0] = ((float)getdword(reader.ReadBytes(4))) / 4096.0f;
                            } else {
                            }
                        }
                        // }
                    }
                    animation[j] = anim;
                }

            }
            reader.Close();
            //free(dataoffset);
            return animation;
        }
        static Int32 getdword(byte[] b) {
            Int32 v;
            v = b[0];
            v |= b[1] << 8;
            v |= b[2] << 16;
            v |= b[3] << 24;
            return v;
        }
        static Int32 getword(byte[] b) {
            Int32 v;
            v = b[0];
            v |= b[1] << 8;
            return v;
        }
        #endregion Methods
    }
}

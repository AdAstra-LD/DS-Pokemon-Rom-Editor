using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Editor.NSBTA {
    public class NSBTA {
        public struct NSBTA_File {
            public header Header;
            public struct header {
                public string ID;
                public byte[] Magic;
                public Int32 file_size;
                public Int16 header_size;
                public Int16 nSection;
                public Int32[] Section_Offset;
            }
            public srt0 SRT0;
            public struct srt0 //Scale Rotation and Translation
            {
                public string ID;
                public Int32 Size;
                //3D Info Structure
                public byte dummy;
                public byte num_objs;
                public short section_size;
                public UnknownBlock unknownBlock;
                public Info infoBlock;
                public string[] names;

                public struct UnknownBlock {
                    public short header_size;
                    public short section_size;
                    public int constant; // 0x017F

                    public short[] unknown1;
                    public short[] unknown2;
                }
                public struct Info {
                    public short header_size;
                    public short data_size;

                    public info[] Data;

                    public struct info {
                        public Int32 MAToffset;
                    }
                }
            }
            public M_AT MAT;
            public struct M_AT {
                public string ID;
                public Int16 Unknown1;
                public byte Unknown2;
                public byte Unknown3;
                //3D Info Structure
                public byte dummy;
                public byte num_objs;
                public short section_size;
                public UnknownBlock unknownBlock;
                public Info infoBlock;
                public string[] names;

                public struct UnknownBlock {
                    public short header_size;
                    public short section_size;
                    public int constant; // 0x017F

                    public short[] unknown1;
                    public short[] unknown2;
                }
                public struct Info {
                    public short header_size;
                    public short data_size;

                    public info[] Data;

                    public struct info {
                        public Int16[] frame;
                        public Int16[] var1;
                        public Int16[] var2;
                        public Int16[] var3;
                        public int[] frameStep;
                    }
                }
            }
            public srtData[] SRTData;
            public struct srtData {
                public decimal[] scaleS;
                public decimal[] scaleT;
                public decimal[] rotate;
                public decimal[] translateS;
                public decimal[] translateT;
            }
        }

        public static NSBTA_File Read(string Filename) {
            EndianBinaryReader er = new EndianBinaryReader(File.OpenRead(Filename), Endianness.LittleEndian);
            NSBTA_File ns = new NSBTA_File();
            ns.Header.ID = er.ReadString(Encoding.ASCII, 4);
            if (ns.Header.ID == "BTA0") {
                ns.Header.Magic = er.ReadBytes(4);
                ns.Header.file_size = er.ReadInt32();
                ns.Header.header_size = er.ReadInt16();
                ns.Header.nSection = er.ReadInt16();
                ns.Header.Section_Offset = new Int32[ns.Header.nSection];
                for (int i = 0; i < ns.Header.nSection; i++) {
                    ns.Header.Section_Offset[i] = er.ReadInt32();
                }
                ns.SRT0.ID = er.ReadString(Encoding.ASCII, 4);
                if (ns.SRT0.ID == "SRT0") {
                    ns.SRT0.Size = er.ReadInt32();
                    //3D Info Structure
                    ns.SRT0.dummy = er.ReadByte();
                    ns.SRT0.num_objs = er.ReadByte();
                    ns.SRT0.section_size = er.ReadInt16();
                    ns.SRT0.unknownBlock.header_size = er.ReadInt16();
                    ns.SRT0.unknownBlock.section_size = er.ReadInt16();
                    ns.SRT0.unknownBlock.constant = er.ReadInt32();
                    ns.SRT0.unknownBlock.unknown1 = new short[ns.SRT0.num_objs];
                    ns.SRT0.unknownBlock.unknown2 = new short[ns.SRT0.num_objs];
                    for (int i = 0; i < ns.SRT0.num_objs; i++) {
                        ns.SRT0.unknownBlock.unknown1[i] = er.ReadInt16();
                        ns.SRT0.unknownBlock.unknown2[i] = er.ReadInt16();
                    }

                    ns.SRT0.infoBlock.header_size = er.ReadInt16();
                    ns.SRT0.infoBlock.data_size = er.ReadInt16();
                    ns.SRT0.infoBlock.Data = new NSBTA_File.srt0.Info.info[ns.SRT0.num_objs];
                    for (int i = 0; i < ns.SRT0.num_objs; i++) {
                        ns.SRT0.infoBlock.Data[i].MAToffset = er.ReadInt32();
                    }
                    ns.SRT0.names = new string[ns.SRT0.num_objs];
                    for (int i = 0; i < ns.SRT0.num_objs; i++) {
                        ns.SRT0.names[i] = er.ReadString(Encoding.ASCII, 16).Replace("\0", "");
                    }

                    ns.MAT.ID = er.ReadString(Encoding.ASCII, 4);
                    if (ns.MAT.ID == "M" + (char)0x00 + "AT") {
                        ns.MAT.Unknown1 = er.ReadInt16();
                        ns.MAT.Unknown2 = er.ReadByte();
                        ns.MAT.Unknown3 = er.ReadByte();
                        //3D Info Structure
                        ns.MAT.dummy = er.ReadByte();
                        ns.MAT.num_objs = er.ReadByte();
                        ns.MAT.section_size = er.ReadInt16();
                        ns.MAT.unknownBlock.header_size = er.ReadInt16();
                        ns.MAT.unknownBlock.section_size = er.ReadInt16();
                        ns.MAT.unknownBlock.constant = er.ReadInt32();
                        ns.MAT.unknownBlock.unknown1 = new short[ns.MAT.num_objs];
                        ns.MAT.unknownBlock.unknown2 = new short[ns.MAT.num_objs];
                        for (int i = 0; i < ns.MAT.num_objs; i++) {
                            ns.MAT.unknownBlock.unknown1[i] = er.ReadInt16();
                            ns.MAT.unknownBlock.unknown2[i] = er.ReadInt16();
                        }

                        ns.MAT.infoBlock.header_size = er.ReadInt16();
                        ns.MAT.infoBlock.data_size = er.ReadInt16();
                        ns.MAT.infoBlock.Data = new NSBTA_File.M_AT.Info.info[ns.MAT.num_objs];
                        ns.SRTData = new NSBTA_File.srtData[ns.MAT.num_objs];
                        for (int i = 0; i < ns.MAT.num_objs; i++) {
                            ns.MAT.infoBlock.Data[i].frame = new short[5];
                            ns.MAT.infoBlock.Data[i].var1 = new short[5];
                            ns.MAT.infoBlock.Data[i].var2 = new short[5];
                            ns.MAT.infoBlock.Data[i].var3 = new short[5];
                            ns.MAT.infoBlock.Data[i].frameStep = new int[5];
                            for (int j = 0; j < 5; j++) {
                                ns.MAT.infoBlock.Data[i].frame[j] = er.ReadInt16();
                                ns.MAT.infoBlock.Data[i].var1[j] = (short)(er.ReadInt16() / 4096);
                                ns.MAT.infoBlock.Data[i].var2[j] = er.ReadInt16();
                                ns.MAT.infoBlock.Data[i].var3[j] = (short)(er.ReadInt16() / 4096);
                            }
                            if (ns.MAT.infoBlock.Data[i].var1[0] > 1) {
                                ns.SRTData[i].scaleS = new decimal[1];
                                ns.SRTData[i].scaleS[0] = Math.Abs(ns.MAT.infoBlock.Data[i].var2[0] / 4096);
                            } else {
                                ns.SRTData[i].scaleS = new decimal[ns.MAT.infoBlock.Data[i].frame[0]];
                            }
                            if (ns.MAT.infoBlock.Data[i].var1[1] > 1) {
                                ns.SRTData[i].scaleT = new decimal[1];
                                ns.SRTData[i].scaleT[0] = Math.Abs(ns.MAT.infoBlock.Data[i].var2[1] / 4096);
                            } else {
                                ns.SRTData[i].scaleT = new decimal[ns.MAT.infoBlock.Data[i].frame[1]];
                            }
                            if (ns.MAT.infoBlock.Data[i].var1[2] > 1) {
                                ns.SRTData[i].rotate = new decimal[2];
                                ns.SRTData[i].rotate[0] = ns.MAT.infoBlock.Data[i].var2[2];
                                ns.SRTData[i].rotate[1] = ns.MAT.infoBlock.Data[i].var3[2];
                            } else {
                                ns.SRTData[i].rotate = new decimal[ns.MAT.infoBlock.Data[i].frame[2] * 2];
                            }
                            if (ns.MAT.infoBlock.Data[i].var1[3] > 1) {
                                ns.SRTData[i].translateS = new decimal[1];
                                ns.SRTData[i].translateS[0] = Math.Abs(ns.MAT.infoBlock.Data[i].var2[3] / 4096);
                            } else {
                                ns.SRTData[i].translateS = new decimal[ns.MAT.infoBlock.Data[i].frame[3]];
                                ns.MAT.infoBlock.Data[i].frameStep[3] = Math.Abs(ns.MAT.infoBlock.Data[i].var1[3] >> 1);

                            }
                            if (ns.MAT.infoBlock.Data[i].var1[4] > 1) {
                                ns.SRTData[i].translateT = new decimal[1];
                                ns.SRTData[i].translateT[0] = Math.Abs(ns.MAT.infoBlock.Data[i].var2[4] / 4096);
                            } else {
                                ns.SRTData[i].translateT = new decimal[ns.MAT.infoBlock.Data[i].frame[4]];
                                ns.MAT.infoBlock.Data[i].frameStep[4] = Math.Abs(ns.MAT.infoBlock.Data[i].var1[4] >> 1);
                            }
                            //ns.SRTData[i].scaleS = new decimal[ns.MAT.infoBlock.Data[i].var1[0] == 3 ? 1 : ns.MAT.infoBlock.Data[i].frame[0]];
                            //ns.SRTData[i].scaleT = new decimal[ns.MAT.infoBlock.Data[i].var1[1] == 3 ? 1 : ns.MAT.infoBlock.Data[i].frame[1]];
                            //ns.SRTData[i].rotate = new decimal[ns.MAT.infoBlock.Data[i].var1[2] == 2 ? ns.MAT.infoBlock.Data[i].var1[2] == 3 ? 2 : 1 : ns.MAT.infoBlock.Data[i].frame[2]];
                            //ns.SRTData[i].translateS = new decimal[ns.MAT.infoBlock.Data[i].var1[3] == 3 ? 1 : ns.MAT.infoBlock.Data[i].frame[3]];
                            //ns.SRTData[i].translateT = new decimal[ns.MAT.infoBlock.Data[i].var1[4] == 3 ? 1 : ns.MAT.infoBlock.Data[i].frame[4]];
                        }
                        ns.MAT.names = new string[ns.MAT.num_objs];
                        for (int i = 0; i < ns.MAT.num_objs; i++) {
                            ns.MAT.names[i] = er.ReadString(Encoding.ASCII, 16).Replace("\0", "");
                        }
                        for (int i = 0; i < ns.MAT.num_objs; i++) {
                            if (ns.SRTData[i].scaleS.Length != 1) {
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.SRT0.section_size + ns.MAT.infoBlock.Data[i].var2[0] + 8;
                                for (int j = 0; j < ns.SRTData[i].scaleS.Length; j++) {
                                    ns.SRTData[i].scaleS[j] = (decimal)(er.ReadInt16() / 4096d);
                                }
                            }
                            if (ns.SRTData[i].scaleT.Length != 1) {
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.SRT0.section_size + ns.MAT.infoBlock.Data[i].var2[1] + 8;
                                for (int j = 0; j < ns.SRTData[i].scaleT.Length; j++) {
                                    ns.SRTData[i].scaleT[j] = (decimal)(er.ReadInt16() / 4096d);
                                }
                            }
                            if (ns.SRTData[i].rotate.Length != 2) {
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.SRT0.section_size + ns.MAT.infoBlock.Data[i].var2[2] + 8;
                                for (int j = 0; j < ns.SRTData[i].rotate.Length; j++) {
                                    ns.SRTData[i].rotate[j] = (decimal)(er.ReadInt16() / 4096d);
                                }
                            }
                            if (ns.SRTData[i].translateS.Length != 1) {
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.SRT0.section_size + ns.MAT.infoBlock.Data[i].var2[3] + 8;

                                for (int j = 0; j < ns.SRTData[i].translateS.Length; j += (ns.MAT.infoBlock.Data[i].frameStep[3] == 0 ? 1 : ns.MAT.infoBlock.Data[i].frameStep[3])) {
                                    decimal value = (decimal)(er.ReadInt16() / 4096d);
                                    for (int k = 0; k < (ns.MAT.infoBlock.Data[i].frameStep[3] == 0 ? 1 : ns.MAT.infoBlock.Data[i].frameStep[3]); k++) {
                                        ns.SRTData[i].translateS[j + k] = value;
                                    }
                                }
                            }
                            if (ns.SRTData[i].translateT.Length != 1) {
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.SRT0.section_size + ns.MAT.infoBlock.Data[i].var2[4] + 8;
                                for (int j = 0; j < ns.SRTData[i].translateT.Length; j += (ns.MAT.infoBlock.Data[i].frameStep[4] == 0 ? 1 : ns.MAT.infoBlock.Data[i].frameStep[4])) {
                                    decimal value = (decimal)(er.ReadInt16() / 4096d);
                                    for (int k = 0; k < (ns.MAT.infoBlock.Data[i].frameStep[4] == 0 ? 1 : ns.MAT.infoBlock.Data[i].frameStep[4]); k++) {
                                        ns.SRTData[i].translateT[j + k] = value;
                                    }
                                }
                            }
                        }
                    } else {
                        MessageBox.Show("Error");
                        er.Close();
                        return ns;
                    }
                } else {
                    MessageBox.Show("Error");
                    er.Close();
                    return ns;
                }
            } else {
                MessageBox.Show("Error");
                er.Close();
                return ns;
            }
            er.Close();
            return ns;
        }
    }
}

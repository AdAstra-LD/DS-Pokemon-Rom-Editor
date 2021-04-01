using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MKDS_Course_Editor.NSBTP {
    public class NSBTP {
        public struct NSBTP_File {
            public header Header;
            public struct header {
                public string ID;
                public byte[] Magic;
                public Int32 file_size;
                public Int16 header_size;
                public Int16 nSection;
                public Int32[] Section_Offset;
            }
            public pat0 PAT0;
            public struct pat0 //Scale Rotation and Translation
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
                        public Int32 MPToffset;
                    }
                }
            }
            public M_PT MPT;
            public struct M_PT {
                public string ID;
                public Int16 NoFrames;
                public byte NoTex;
                public byte NoPal;
                public Int16 Texoffset;
                public Int16 Paloffset;
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
                        public Int32 KeyFrames;
                        public short Unknown1;
                        public short Offset;
                    }
                }
            }
            public animData[] AnimData;
            public struct animData {
                public keyFrame[] KeyFrames;
                public struct keyFrame {
                    public short Start;
                    public byte texId;
                    public byte palId;
                    public string texName;
                    public string palName;
                }
            }
        }
        public static NSBTP_File Read(string Filename) {
            EndianBinaryReader er = new EndianBinaryReader(File.OpenRead(Filename), Endianness.LittleEndian);
            NSBTP_File ns = new NSBTP_File();
            ns.Header.ID = er.ReadString(Encoding.ASCII, 4);

            if (ns.Header.ID == "BTP0") {
                ns.Header.Magic = er.ReadBytes(4);
                ns.Header.file_size = er.ReadInt32();
                ns.Header.header_size = er.ReadInt16();
                ns.Header.nSection = er.ReadInt16();
                ns.Header.Section_Offset = new Int32[ns.Header.nSection];

                for (int i = 0; i < ns.Header.nSection; i++) {
                    ns.Header.Section_Offset[i] = er.ReadInt32();
                }
                ns.PAT0.ID = er.ReadString(Encoding.ASCII, 4);

                if (ns.PAT0.ID == "PAT0") {
                    ns.PAT0.Size = er.ReadInt32();
                    //3D Info Structure
                    ns.PAT0.dummy = er.ReadByte();
                    ns.PAT0.num_objs = er.ReadByte();
                    ns.PAT0.section_size = er.ReadInt16();
                    ns.PAT0.unknownBlock.header_size = er.ReadInt16();
                    ns.PAT0.unknownBlock.section_size = er.ReadInt16();
                    ns.PAT0.unknownBlock.constant = er.ReadInt32();
                    ns.PAT0.unknownBlock.unknown1 = new short[ns.PAT0.num_objs];
                    ns.PAT0.unknownBlock.unknown2 = new short[ns.PAT0.num_objs];
                    for (int i = 0; i < ns.PAT0.num_objs; i++) {
                        ns.PAT0.unknownBlock.unknown1[i] = er.ReadInt16();
                        ns.PAT0.unknownBlock.unknown2[i] = er.ReadInt16();
                    }

                    ns.PAT0.infoBlock.header_size = er.ReadInt16();
                    ns.PAT0.infoBlock.data_size = er.ReadInt16();
                    ns.PAT0.infoBlock.Data = new NSBTP_File.pat0.Info.info[ns.PAT0.num_objs];

                    for (int i = 0; i < ns.PAT0.num_objs; i++) {
                        ns.PAT0.infoBlock.Data[i].MPToffset = er.ReadInt32();
                    }

                    ns.PAT0.names = new string[ns.PAT0.num_objs];
                    for (int i = 0; i < ns.PAT0.num_objs; i++) {
                        ns.PAT0.names[i] = er.ReadString(Encoding.ASCII, 16).Replace("\0", "");
                    }

                    ns.MPT.ID = er.ReadString(Encoding.ASCII, 4);

                    if (ns.MPT.ID == "M" + (char)0x00 + "PT") {
                        ns.MPT.NoFrames = er.ReadInt16();
                        ns.MPT.NoTex = er.ReadByte();
                        ns.MPT.NoPal = er.ReadByte();
                        ns.MPT.Texoffset = er.ReadInt16();
                        ns.MPT.Paloffset = er.ReadInt16();
                        //3D Info Structure
                        ns.MPT.dummy = er.ReadByte();
                        ns.MPT.num_objs = er.ReadByte();
                        ns.MPT.section_size = er.ReadInt16();
                        ns.MPT.unknownBlock.header_size = er.ReadInt16();
                        ns.MPT.unknownBlock.section_size = er.ReadInt16();
                        ns.MPT.unknownBlock.constant = er.ReadInt32();
                        ns.MPT.unknownBlock.unknown1 = new short[ns.MPT.num_objs];
                        ns.MPT.unknownBlock.unknown2 = new short[ns.MPT.num_objs];

                        for (int i = 0; i < ns.MPT.num_objs; i++) {
                            ns.MPT.unknownBlock.unknown1[i] = er.ReadInt16();
                            ns.MPT.unknownBlock.unknown2[i] = er.ReadInt16();
                        }

                        ns.MPT.infoBlock.header_size = er.ReadInt16();
                        ns.MPT.infoBlock.data_size = er.ReadInt16();
                        ns.MPT.infoBlock.Data = new NSBTP_File.M_PT.Info.info[ns.MPT.num_objs];
                        ns.AnimData = new NSBTP_File.animData[ns.MPT.num_objs];

                        for (int i = 0; i < ns.MPT.num_objs; i++) {
                            ns.MPT.infoBlock.Data[i].KeyFrames = er.ReadInt32();
                            ns.MPT.infoBlock.Data[i].Unknown1 = er.ReadInt16();
                            ns.MPT.infoBlock.Data[i].Offset = er.ReadInt16();
                            ns.AnimData[i].KeyFrames = new NSBTP_File.animData.keyFrame[ns.MPT.infoBlock.Data[i].KeyFrames];
                           
                            for (int j = 0; j < ns.MPT.infoBlock.Data[i].KeyFrames; j++) {
                                long curpos = er.BaseStream.Position;
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.PAT0.section_size + ns.MPT.infoBlock.Data[i].Offset + j * 4 + 8;
                                ns.AnimData[i].KeyFrames[j].Start = er.ReadInt16();
                                ns.AnimData[i].KeyFrames[j].texId = er.ReadByte();
                                ns.AnimData[i].KeyFrames[j].palId = er.ReadByte();
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.PAT0.section_size + ns.MPT.Texoffset + ns.AnimData[i].KeyFrames[j].texId * 16 + 8;
                                ns.AnimData[i].KeyFrames[j].texName = LibNDSFormats.Utils.ReadNSBMDString(er);
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.PAT0.section_size + ns.MPT.Paloffset + ns.AnimData[i].KeyFrames[j].palId * 16 + 8;
                                ns.AnimData[i].KeyFrames[j].palName = LibNDSFormats.Utils.ReadNSBMDString(er);
                                er.BaseStream.Position = curpos;
                            }
                        }
                        ns.MPT.names = new string[ns.MPT.num_objs];
                        
                        for (int i = 0; i < ns.MPT.num_objs; i++) {
                            ns.MPT.names[i] = LibNDSFormats.Utils.ReadNSBMDString(er);
                        }
                    } else {
                        MessageBox.Show("NSBTP Error");
                        er.Close();
                        return ns;
                    }
                } else {
                    MessageBox.Show("NSBTP Error");
                    er.Close();
                    return ns;
                }
            } else {
                MessageBox.Show("NSBTP Error");
                er.Close();
                return ns;
            }
            er.Close();
            return ns;
        }
    }
}

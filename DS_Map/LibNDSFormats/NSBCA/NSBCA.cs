using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MKDS_Course_Editor.NSBCA {
    public class NSBCA {
        public struct NSBCA_File {
            public header Header;
            public struct header {
                public string ID;
                public byte[] Magic;
                public Int32 file_size;
                public Int16 header_size;
                public Int16 nSection;
                public Int32[] Section_Offset;
            }
            public jnt0 JNT0;
            public struct jnt0 //Scale Rotation and Translation
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
                        public Int32 Objectoffset;
                    }
                }
            }
            public J_AC[] JAC;
            public struct J_AC {
                public string ID;
                public Int16 NrFrames;
                public Int16 NrObjects;
                public Int32 Unknown1;
                public Int32 Offset1;
                public Int32 Offset2;

                public byte[] JointData;
                public byte[] RotationData;

                public Int32[] ObjInfoOffset;

                public objInfo[] ObjInfo;
                public struct objInfo {
                    public Int16 Flag;
                    public byte Unknown1;
                    public byte ID;

                    public int tStart;
                    public int tEnd;
                    public int rStart;
                    public int rEnd;
                    public int sStart;
                    public int sEnd;
                    public List<float>[] translate;
                    public List<float>[] translate_keyframes;
                    public List<float> rotate;
                    public List<float>[] rotate_keyframes;
                    public List<float>[][] scale;
                    public List<float>[][] scale_keyframes;
                }
            }
        }
        /*
         * I've been studying the NSBCA format heavily over the last few days trying to figure it out.
         * I've determined that the first offset in a joint animation contains Pivoting data, and the second section contains Rotation data.
         * Rotation keyframes are called when the second byte equals 0 and Pivot keyframes are called when it equals 128, however the data itself is definitely stored a little differently to how it is stored in NSBMD files.
         * Scaling keyframes hold two scaling values for whatever reason, with Translation keyframes being just straight values that can be a signed word/dword.
         * Each animation has a frame length, however each object in an animation seems to have a start and end position that typically don't match up with the total number of frames.
         * Now I haven't had much experience with model animations, but I'm guessing this might have like a decay effect on the animation if anyone has ever played around with animations in Maya?
         * The trickiest part is calculating the right number of keyframes stored in each object as there isn't an actual value written down anywhere.
         * From what I can tell it's calculated based on a bunch of things including the difference in object frame length over animation frame length, the rate in which the object (and possibly the animation) plays back as well as rounding to the upper whole number.
         * This looks a bit funky and overcomplicated but so far all the animations I've been working with calculate the correct number of keyframes, so I'm assuming I'm on the right track.
         * Am yet to get to the point of loading animations into a model, but hopefully I won't get some spastic result :S

          
        
         
        Object Flag: --zyx-Sr-RZYX-T-
        > found in the header of  each object of an animation
        ===========================
        T - has Translation keyframes (0 Yes| 1 No)
        XYZ - flags for Translation attributes
        R - has Rotation/Pivot keyframes (0 Yes| 1 No)
        r - flag for Rotation/Pivot attribute
        S - has Scale keyframes (0 Yes| 1 No)
        xyz - flags for Scale attributes
        ===========================
        if T-XYZ = 1
        > Fixed Translation value (signed dword)
        if R-r = 1
        > Fixed Rotation/Pivot value (dword/2*word?)
        if S-xyz = 1
        > Fixed Scale value (2*dword)

        Note: The below is only done when the bit flag equals 0 for that attribute (TX, TY, TZ, R/P, SX, SY, SZ)

        a|b = datasize = playback speed
        > a & b are flags stored in the object header for each attribute

        Translate
        -----------------------------------------------------------
        2|0 = word = 1/1
        2|1 = word = 1/2
        2|2 = word = 1/3
        0|1 = dword = 1/2

        Rotate
        -----------------------------------------------------------
        0|0 = 2*byte = 1/1
        0|1 = 2*byte = 1/2
        0|2 = 2*byte = 1/3
        > byte0 = index
        > byte1 = 0 Rotation | 128 Pivot
        > not completely sure on rotation yet

        Scale
        -----------------------------------------------------------
        2|1 = 2*word = 1/1
        2|1 = 2*word = 1/2
        -----------------------------------------------------------

        Attribute    [animation flag|start|end|a|b] bytes/keyframes - animation length
        > bytes - the actual size of the data stored
        > keyframes - bytes/datasize(see above)

        Basabasa - 0 Pivot - 33 Rotation
        ===========================================================
        Translate	 [3|0|34|2|1] 38/19 - 36 Frames
        Rotate	 [3|0|34|0|1] 38/19 - 36 Frames
        Scale		 [3|0|34|2|1] 76/19 - 36 Frames
        ===========================================================

        Basabasa2 - 1 Pivot - 20 Rotation
        ===========================================================
        Rotate	 [3|0|78|0|1] 82/41 - 80 Frames
        ===========================================================

        Bilikyu - 31 Pivot
        ===========================================================
        Translate	 [3|0|58|2|1] 62/31 - 60 Frames
        Rotate	 [3|0|58|0|1] 62/31 - 60 Frames
        ===========================================================

        Donketu - 12 Pivot / 20 Rotation
        ===========================================================
        Translate	 [3|0|10|0|1] 24/06 - 11 Frames (dword)
        Translate	 [3|0|10|2|1] 12/06 - 11 Frames
        Translate	 [1|0|02|2|0] 04/02 - 02 Frames
        Rotate	 [3|0|10|0|1] 14/07 - 11 Frames
        ===========================================================

        Gesso - 39 Pivot
        ===========================================================
        Translate	 [0|0|16|2|2] 14/07 - 19 Frames
        Translate	 [0|0|16|2|2] 12/06 - 18 Frames
        Translate	 [0|0|12|2|2] 10/05 - 14 Frames
        Scale		 [0|0|16|2|2] 28/07 - 19 Frames
        Scale		 [0|0|16|2|2] 24/06 - 18 Frames
        Rotate	 [0|0|16|0|2] 16/08 - 19 Frames
        Rotate	 [0|0|16|0|2] 16/08 - 18 Frames
        Rotate	 [0|0|12|0|2] 14/07 - 14 Frames
        ===========================================================

        TTL Bird
        ===========================================================
        Translate	 [1|0|05|2|0] 10/05 - 05 Frames
        Rotate	 [1|0|05|0|0] 10/05 - 05 Frames
        ===========================================================

        Gamaguchi - 11 Pivot - 52 Rotation
        ===========================================================
        Translate	 [3|0|08|2|0] 16/08 - 08 Frames
        Translate	 [3|0|07|2|0] 14/07 - 07 Frames
        Translate	 [3|0|16|2|0] 32/16 - 16 Frames
        Rotate	 [3|0|08|0|0] 16/08 - 08 Frames
        Rotate	 [1|0|07|0|0] 14/07 - 07 Frames
        Rotate	 [3|0|16|0|0] 32/16 - 16 Frames
        Scale		 [3|0|08|2|0] 32/08 - 08 Frames
        Scale		 [1|0|07|2|0] 28/07 - 07 Frames
        ===========================================================
         */
        public static NSBCA_File Read(string Filename) {
            byte[] file_ = File.ReadAllBytes(Filename);
            if (file_[0] == 76 && file_[1] == 90 && file_[2] == 55 && file_[3] == 55) {
            }
            EndianBinaryReader er = new EndianBinaryReader(new MemoryStream(file_), Endianness.LittleEndian);
            NSBCA_File ns = new NSBCA_File();
            ns.Header.ID = er.ReadString(Encoding.ASCII, 4);
            if (ns.Header.ID == "BCA0") {
                ns.Header.Magic = er.ReadBytes(4);
                ns.Header.file_size = er.ReadInt32();
                ns.Header.header_size = er.ReadInt16();
                ns.Header.nSection = er.ReadInt16();
                ns.Header.Section_Offset = new Int32[ns.Header.nSection];
                for (int i = 0; i < ns.Header.nSection; i++) {
                    ns.Header.Section_Offset[i] = er.ReadInt32();
                }

                ns.JNT0.ID = er.ReadString(Encoding.ASCII, 4);
                if (ns.JNT0.ID == "JNT0") {
                    ns.JNT0.Size = er.ReadInt32();
                    //3D Info Structure
                    ns.JNT0.dummy = er.ReadByte();
                    ns.JNT0.num_objs = er.ReadByte();
                    ns.JNT0.section_size = er.ReadInt16();
                    ns.JNT0.unknownBlock.header_size = er.ReadInt16();
                    ns.JNT0.unknownBlock.section_size = er.ReadInt16();
                    ns.JNT0.unknownBlock.constant = er.ReadInt32();
                    ns.JNT0.unknownBlock.unknown1 = new short[ns.JNT0.num_objs];
                    ns.JNT0.unknownBlock.unknown2 = new short[ns.JNT0.num_objs];
                    for (int i = 0; i < ns.JNT0.num_objs; i++) {
                        ns.JNT0.unknownBlock.unknown1[i] = er.ReadInt16();
                        ns.JNT0.unknownBlock.unknown2[i] = er.ReadInt16();
                    }

                    ns.JNT0.infoBlock.header_size = er.ReadInt16();
                    ns.JNT0.infoBlock.data_size = er.ReadInt16();
                    ns.JNT0.infoBlock.Data = new NSBCA_File.jnt0.Info.info[ns.JNT0.num_objs];
                    for (int i = 0; i < ns.JNT0.num_objs; i++) {
                        ns.JNT0.infoBlock.Data[i].Objectoffset = er.ReadInt32();
                    }
                    ns.JNT0.names = new string[ns.JNT0.num_objs];
                    for (int i = 0; i < ns.JNT0.num_objs; i++) {
                        ns.JNT0.names[i] = er.ReadString(Encoding.ASCII, 16).Replace("\0", "");
                    }
                    ns.JAC = new NSBCA_File.J_AC[ns.JNT0.num_objs];
                    for (int i = 0; i < ns.JNT0.num_objs; i++) {
                        er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.JNT0.infoBlock.Data[i].Objectoffset;
                        ns.JAC[i].ID = er.ReadString(Encoding.ASCII, 4);
                        if (ns.JAC[i].ID == "J" + (char)0x00 + "AC") {
                            ns.JAC[i].NrFrames = er.ReadInt16();
                            ns.JAC[i].NrObjects = er.ReadInt16();
                            ns.JAC[i].Unknown1 = er.ReadInt32();
                            ns.JAC[i].Offset1 = er.ReadInt32();
                            ns.JAC[i].Offset2 = er.ReadInt32();
                            long curposs = er.BaseStream.Position;
                            if (ns.JAC[i].Offset2 != ns.JAC[i].Offset1) {
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.JNT0.infoBlock.Data[i].Objectoffset + ns.JAC[i].Offset1;
                                ns.JAC[i].JointData = er.ReadBytes(ns.JAC[i].Offset2 - ns.JAC[i].Offset1);
                                er.BaseStream.Position = curposs;
                            }

                            long dataoffset = 0;


                            ns.JAC[i].ObjInfoOffset = new Int32[ns.JAC[i].NrObjects];
                            for (int j = 0; j < ns.JAC[i].NrObjects; j++) {
                                ns.JAC[i].ObjInfoOffset[j] = er.ReadInt16();
                            }

                            ns.JAC[i].ObjInfo = new NSBCA_File.J_AC.objInfo[ns.JAC[i].NrObjects];

                            for (int j = 0; j < ns.JAC[i].NrObjects; j++) {
                                er.BaseStream.Position = ns.Header.Section_Offset[0] +/* ns.JNT0.section_size*/ns.JNT0.infoBlock.Data[i].Objectoffset + ns.JAC[i].ObjInfoOffset[j];// + 8;
                                ns.JAC[i].ObjInfo[j].Flag = er.ReadInt16();
                                ns.JAC[i].ObjInfo[j].Unknown1 = er.ReadByte();
                                ns.JAC[i].ObjInfo[j].ID = er.ReadByte();
                                ns.JAC[i].ObjInfo[j].translate = new List<float>[3];
                                ns.JAC[i].ObjInfo[j].translate[0] = new List<float>();
                                ns.JAC[i].ObjInfo[j].translate[1] = new List<float>();
                                ns.JAC[i].ObjInfo[j].translate[2] = new List<float>();
                                ns.JAC[i].ObjInfo[j].translate_keyframes = new List<float>[3];
                                ns.JAC[i].ObjInfo[j].translate_keyframes[0] = new List<float>();
                                ns.JAC[i].ObjInfo[j].translate_keyframes[1] = new List<float>();
                                ns.JAC[i].ObjInfo[j].translate_keyframes[2] = new List<float>();
                                ns.JAC[i].ObjInfo[j].rotate = new List<float>();
                                ns.JAC[i].ObjInfo[j].rotate_keyframes = new List<float>[2];
                                ns.JAC[i].ObjInfo[j].rotate_keyframes[0] = new List<float>();
                                ns.JAC[i].ObjInfo[j].rotate_keyframes[1] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale = new List<float>[3][];
                                ns.JAC[i].ObjInfo[j].scale[0] = new List<float>[2];
                                ns.JAC[i].ObjInfo[j].scale[1] = new List<float>[2];
                                ns.JAC[i].ObjInfo[j].scale[2] = new List<float>[2];
                                ns.JAC[i].ObjInfo[j].scale[0][0] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale[0][1] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale[1][0] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale[1][1] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale[2][0] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale[2][1] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale_keyframes = new List<float>[3][];
                                ns.JAC[i].ObjInfo[j].scale_keyframes[0] = new List<float>[2];
                                ns.JAC[i].ObjInfo[j].scale_keyframes[1] = new List<float>[2];
                                ns.JAC[i].ObjInfo[j].scale_keyframes[2] = new List<float>[2];
                                ns.JAC[i].ObjInfo[j].scale_keyframes[0][0] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale_keyframes[0][1] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale_keyframes[1][0] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale_keyframes[1][1] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale_keyframes[2][0] = new List<float>();
                                ns.JAC[i].ObjInfo[j].scale_keyframes[2][1] = new List<float>();
                                double[] speed = {
                1.0D, 0.5D, 0.33333333333333331D
            };
                                if (((ns.JAC[i].ObjInfo[j].Flag >> 1) & 1) == 0) {
                                    //struct.DModelAnimation.MTransformAni trans[] = new struct.DModelAnimation.MTransformAni[3];
                                    //string msg = new StringBuilder().Append(msg).Append("\n   -> Translate: ").ToString();
                                    //string[] type = { "X", "Y", "Z" };
                                    for (int k = 0; k < 3; k++) {
                                        //trans[k] = new struct.DModelAnimation.MTransformAni(this);
                                        int tflag = ns.JAC[i].ObjInfo[j].Flag >> 3 + k & 1;
                                        //msg = new StringBuilder().Append(msg).Append("\n    -> T").Append(type[k]).Append(tflag).Append("[").ToString();
                                        if (tflag == 1) {
                                            int tvar = er.ReadInt32();
                                            //trans[k].setFrame((float)tvar / divide);
                                            ns.JAC[i].ObjInfo[j].translate[k].Add((float)tvar / 4096f);
                                            //msg = (new StringBuilder()).Append(msg).Append(tvar).ToString();
                                            continue;
                                        } else {
                                            int param2 = er.ReadInt32();
                                            int startFrame = param2 & 0xffff;
                                            ns.JAC[i].ObjInfo[j].tStart = startFrame;
                                            int endFrame = param2 >> 16 & 0xfff;
                                            ns.JAC[i].ObjInfo[j].tEnd = endFrame;
                                            int var2 = param2 >> 28 & 3;
                                            int speedId = param2 >> 30 & 3;
                                            int toffset = er.ReadInt32();
                                            int width = var2 != 0 ? 2 : 4;
                                            int extra = (ns.JAC[i].Unknown1 != 3 ? 0 : ns.JAC[i].NrFrames - endFrame);
                                            int length = (int)Math.Ceiling((double)(ns.JAC[i].NrFrames + extra) * speed[speedId]);
                                            long curpos = er.BaseStream.Position;
                                            for (int t = 0; t < length; t++) {
                                                er.BaseStream.Position = ns.Header.Section_Offset[0] +/* ns.JNT0.section_size*/ns.JNT0.infoBlock.Data[i].Objectoffset + toffset + (t * width);
                                                if (dataoffset == 0) {
                                                    dataoffset = toffset;
                                                }
                                                float keyFrame = (width != 2 ? (float)er.ReadInt32() : (float)er.ReadInt16());
                                                ns.JAC[i].ObjInfo[j].translate_keyframes[k].Add((float)LibNDSFormats.NSBMD.NSBMDGlRenderer.Sign((int)keyFrame, (width != 2 ? 32 : 16)) / 4096f);
                                                //m = (new StringBuilder()).append(m).append("\n     -> #").append(t).append(": ").append(keyFrame).toString();
                                            }
                                            er.BaseStream.Position = curpos;
                                        }
                                    }
                                }
                                if (((ns.JAC[i].ObjInfo[j].Flag >> 6) & 1) == 0) {
                                    int rflag = ns.JAC[i].ObjInfo[j].Flag >> 8 & 1;
                                    if (rflag == 1) {
                                        //dataParser _tmp14 = pa;
                                        int rvar = er.ReadInt32(); //dataParser.getInt(data, jump, 4);
                                        ns.JAC[i].ObjInfo[j].rotate.Add((float)rvar);
                                        //msg = (new StringBuilder()).append(msg).append(rvar).toString();
                                        //jump += 4;
                                    } else {
                                        int param2 = er.ReadInt32();
                                        int startFrame = param2 & 0xffff;
                                        ns.JAC[i].ObjInfo[j].rStart = startFrame;
                                        int endFrame = param2 >> 16 & 0xfff;
                                        ns.JAC[i].ObjInfo[j].rEnd = endFrame;
                                        int var2 = param2 >> 28 & 3;
                                        int speedId = param2 >> 30 & 3;
                                        int roffset = er.ReadInt32();
                                        int width = 2;//var2 != 0 ? 2 : 4;
                                        int length = (int)Math.Ceiling((double)(ns.JAC[i].NrFrames) * speed[speedId]);
                                        long curpos = er.BaseStream.Position;
                                        for (int r = 0; r < length; r++) {
                                            er.BaseStream.Position = ns.Header.Section_Offset[0] +/* ns.JNT0.section_size*/ns.JNT0.infoBlock.Data[i].Objectoffset + roffset + (r * width);
                                            if (dataoffset == 0) {
                                                dataoffset = roffset;
                                            }
                                            int rvar6 = er.ReadInt16();
                                            int rindex = rvar6 & 0x7fff;
                                            int mode = rvar6 >> 15 & 1;
                                            ns.JAC[i].ObjInfo[j].rotate_keyframes[0].Add(rindex);
                                            ns.JAC[i].ObjInfo[j].rotate_keyframes[1].Add(mode);
                                        }
                                        er.BaseStream.Position = curpos;

                                    }
                                }
                                if ((ns.JAC[i].ObjInfo[j].Flag >> 9 & 1) == 0) {
                                    //struct.DModelAnimation.MScaleAni scale[] = new struct.DModelAnimation.MScaleAni[3];
                                    //msg = (new StringBuilder()).append(msg).append("\n   -> Scale: ").toString();
                                    for (int k = 0; k < 3; k++) {
                                        //scale[k] = new struct.DModelAnimation.MScaleAni(this);
                                        int sflag = ns.JAC[i].ObjInfo[j].Flag >> 11 + k & 1;
                                        //msg = (new StringBuilder()).append(msg).append("\n    -> S").append(type[k]).append(sflag).append("[").toString();
                                        if (sflag == 1) {
                                            //dataParser _tmp19 = pa;
                                            int svar1 = er.ReadInt32();//dataParser.getInt(data, jump, 4);
                                            ns.JAC[i].ObjInfo[j].scale[k][0].Add((float)svar1 / 4096f);
                                            //dataParser _tmp20 = pa;
                                            int svar2 = er.ReadInt32();//dataParser.getSign(data, jump + 4, 4);
                                            ns.JAC[i].ObjInfo[j].scale[k][1].Add((float)svar2 / 4096f);

                                            //int svar3 = er.ReadInt32();//dataParser.getSign(data, jump + 4, 4);
                                            //int svar4 = er.ReadInt32();//dataParser.getSign(data, jump + 4, 4);
                                            //ns.JAC[i].ObjInfo[j].scale[k][1].Add((float)svar2 / 4096f);
                                            //scale[k].setFrame(new float[] {
                                            //    (float)svar1 / divide, (float)svar2 / divide
                                            //});
                                            //msg = (new StringBuilder()).append(msg).append(svar1).append("|").append(svar2).toString();
                                            //jump += 8;
                                            continue;
                                        } else {
                                            int param2 = er.ReadInt32();
                                            int startFrame = param2 & 0xffff;
                                            ns.JAC[i].ObjInfo[j].sStart = startFrame;
                                            int endFrame = param2 >> 16 & 0xfff;
                                            ns.JAC[i].ObjInfo[j].sEnd = endFrame;
                                            int var2 = param2 >> 28 & 3;
                                            int speedId = param2 >> 30 & 3;
                                            int soffset = er.ReadInt32();
                                            int width = var2 != 0 ? 2 : 4;
                                            int length = (int)Math.Ceiling((double)(ns.JAC[i].NrFrames) * speed[speedId]);
                                            long curpos = er.BaseStream.Position;
                                            for (int s = 0; s < length; s++) {
                                                er.BaseStream.Position = ns.Header.Section_Offset[0] +/* ns.JNT0.section_size*/ns.JNT0.infoBlock.Data[i].Objectoffset + soffset + (s * width * 2);
                                                if (dataoffset == 0) {
                                                    dataoffset = soffset;
                                                }
                                                ns.JAC[i].ObjInfo[j].scale_keyframes[k][0].Add((float)(width != 2 ? (float)er.ReadInt32() : (float)er.ReadInt16()) / 4096f);
                                                ns.JAC[i].ObjInfo[j].scale_keyframes[k][1].Add((float)(width != 2 ? (float)er.ReadInt32() : (float)er.ReadInt16()) / 4096f);
                                            }
                                            er.BaseStream.Position = curpos;
                                        }
                                    }
                                }
                            }
                            if (dataoffset != 0) {
                                curposs = er.BaseStream.Position;
                                er.BaseStream.Position = ns.Header.Section_Offset[0] + ns.JNT0.infoBlock.Data[i].Objectoffset + ns.JAC[i].Offset2;
                                ns.JAC[i].RotationData = er.ReadBytes((int)dataoffset - ns.JAC[i].Offset2);
                                er.BaseStream.Position = curposs;
                            }
                        } else {
                            //MessageBox.Show("Error");
                            er.Close();
                            return ns;
                        }
                    }
                } else {
                    //MessageBox.Show("Error");
                    er.Close();
                    return ns;
                }
            } else {
                //MessageBox.Show("Error");
                er.Close();
                return ns;
            }
            er.Close();
            return ns;
        }
    }
}

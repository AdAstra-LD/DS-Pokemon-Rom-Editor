// NSBMD data definition.
// Code adapted from kiwi.ds' NSBMD Model Viewer.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibNDSFormats.NSBTX;
using System.Drawing;

namespace LibNDSFormats.NSBMD {
    // Class for storing NSBMD data.
    // Adapted from kiwi.ds NSBMD viewer.
    public class NSBMD {

        #region Constants

        public const uint NDS_TYPE_MDL0 = 0x304c444d;
        public const uint NDS_TYPE_TEX0 = 0x30584554;
        public const uint NDS_TYPE_BMD0 = 0x30444d42;
        public const ushort NDS_TYPE_BYTEORDER = 0xfeff;
        public const ushort NDS_TYPE_UNK2 = 0x0002;
        public const ushort NDS_TYPE_UNK1 = 0x0001;
        public const uint NDS_TYPE_BTX0 = 0x30585442;
        public const ushort HEADERSIZE = 16;

        #endregion Constants

        /// <summary>
        /// Models in NSBMD.
        /// </summary>
        public NSBMDModel[] models;

        /// <summary>
        /// Materials in NSBMD. 
        /// </summary>
        public IEnumerable<NSBMDMaterial> materials;
        /// <summary>
        /// NSBMD materials.
        /// </summary>
        public List<NSBMDTexture> Textures = new List<NSBMDTexture>();
        /// <summary>
        /// NSBMD materials.
        /// </summary>
        public List<NSBMDPalette> Palettes = new List<NSBMDPalette>();
        /// <summary>
        /// Match up model / NSBMD textures.
        /// </summary>
        public void MatchTextures() {
            foreach (NSBMDModel m in models) {
                for (int j = 0; j < m.Polygons.Count - 1; j++) {
                    for (int t = 0; t < m.Textures.Count; t++) {
                        if (m.Textures[t].texmatid.Contains((uint)m.Polygons[j].MatId)) {
                            int texid = t;
                            for (int l = 0; l < Textures.Count; l++) {
                                if (Textures[l].texname == m.Textures[t].texname) {
                                    texid = l;
                                    break;
                                }
                            }

                            NSBMDMaterial mat = m.Materials[m.Polygons[j].MatId];
                            NSBMDTexture tex = Textures[texid];
                            mat.spdata = tex.spdata; //RITORNA QUI
                            mat.texdata = tex.texdata;
                            mat.texname = tex.texname;
                            mat.texoffset = tex.texoffset;
                            mat.texsize = tex.texsize;
                            mat.width = tex.width;
                            mat.height = tex.height;
                            mat.format = tex.format;
                            mat.color0 = tex.color0;
                            break;
                        }
                    }
                    if (m.Materials[m.Polygons[j].MatId].format != 7) {
                        for (int k = 0; k < m.Palettes.Count; k++) {
                            if (m.Palettes[k].palmatid.Contains((uint)m.Polygons[j].MatId)) {
                                int palid = k;
                                for (int l = 0; l < Palettes.Count; l++) {
                                    if (Palettes[l].palname == m.Palettes[k].palname) {
                                        palid = l;
                                        break;
                                    }
                                }

                                NSBMDMaterial mat = m.Materials[m.Polygons[j].MatId];
                                NSBMDPalette pal = Palettes[palid];

                                mat.paldata = pal.paldata;
                                mat.palname = pal.palname;
                                mat.paloffset = pal.paloffset;
                                mat.palsize = pal.palsize;
                                break;
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Match up model / NSBMD textures.
        /// </summary>
        public void MatchTextures_org() {
            for (var i = 0; i < models.Length; i++) {
                for (var j = 0; j < models[i].Materials.Count; j++) {
                    /*bool gottex = false;
                    bool gotpal = false;
                    foreach (var mat1 in materials)
                    {
                        if (j >= models[i].Materials.Count)
                            continue;
                        var mat2 = models[i].Materials[j];


                        // match texture
                        if (!gottex && mat1.texname.Equals(mat2.texname))
                        {
                            //Console.WriteLine("tex '{0}' matched.", mat2.texname);
                            mat1.CopyTo(mat2); 

                            gottex = true;
                        }
                        // match palette
                        if (mat2.format != 7 // NB. direct texture has no palette
                            && !gotpal
                            && (mat1.palname).Equals(mat2.palname))
                        {
                            //Console.WriteLine("pal '{0}' matched.", mat1.palname);
                            mat2.palname = mat1.palname;
                            mat2.palsize = mat1.palsize;
                            mat2.paldata = mat1.paldata;
                            //mat1->palsize = 0;
                            gotpal = true;
                        }
                    }*/
                    try
                    {
                        if (materials.ToArray()[models[i].tex_mat.IndexOf(j)].format != 0)
                        {
                            //Console.WriteLine("Texture {0}:", j.ToString());
                            //Console.WriteLine("Texture Name: '{0}'", models[i].Materials[j].texname);
                            //Console.WriteLine("Palette Name: '{0}'", models[i].Materials[j].palname);
                            string matname = models[i].Materials[j].MaterialName;
                            models[i].Materials[j] = materials.ToArray()[models[i].tex_mat.IndexOf(j)].CopyTo(models[i].Materials[j]);
                            models[i].Materials[j].MaterialName = matname;
                            //if (models[i].Materials[j].format != 7)
                            {
                                models[i].Materials[j].paldata = materials.ToArray()[models[i].pal_mat.IndexOf(j)].paldata;
                                models[i].Materials[j].palname = materials.ToArray()[models[i].pal_mat.IndexOf(j)].palname;
                                models[i].Materials[j].palsize = materials.ToArray()[models[i].pal_mat.IndexOf(j)].palsize;
                                //Console.WriteLine("Texture Name: '{0}'", models[i].Materials[j].texname);
                                //Console.WriteLine("Palette Name: '{0}'", models[i].Materials[j].palname);
                            }
                        }
                    } catch (Exception e) {

                    }
                }
            }

            /*for (var i = 0; i < models.Length; i++)
            {
                for (var j = 0; j < models[i].Materials.Count; j++)
                {
                    bool gottex = false;
                    bool gotpal = false;
                    foreach (var mat1 in materials)
                    {
                        if (j >= models[i].Materials.Count)
                            continue;
                        var mat2 = models[i].Materials[j];

                        // match texture
                        if (!gottex && mat1.texname.Equals(mat2.texname))
                        {
                            //Console.WriteLine("tex '{0}' matched.", mat2.texname);
                            mat1.CopyTo(mat2);

                            gottex = true;
                        }
                        for (int q = 0; q < materials.ToList().Count; q++)
                        {

                            if (mat2.format != 7 // NB. direct texture has no palette
                                && !gotpal
                                && materials.ToList()[q].palname.Contains(mat2.texname)
                                && (mat1.palname).Equals(mat2.palname))
                            {
                                //Console.WriteLine("pal '{0}' matched.", materials.ToArray()[q].palname);
                                mat2.palname = materials.ToArray()[q].palname;
                                mat2.palsize = materials.ToArray()[q].palsize;
                                mat2.paldata = materials.ToArray()[q].paldata;
                                //mat1->palsize = 0;
                                gotpal = true;
                            }
                        }*/
            /*// match palette
            if (mat2.format != 7 // NB. direct texture has no palette
                && !gotpal
                && (mat1.palname).Equals(mat2.palname))
            {
                //Console.WriteLine("pal '{0}' matched.", mat1.palname);
                mat2.palname = mat1.palname;
                mat2.palsize = mat1.palsize;
                mat2.paldata = mat1.paldata;
                //mat1->palsize = 0;
                gotpal = true;
            }*/
            // models[i].Materials[j] = mat2;
        }

        public void ClearTextures() {
            foreach (NSBMDModel m in models) {
                m.Materials.Clear();
            }
        }


        /// <summary>
        /// Decode objects.
        /// </summary>
        public static bool DecodeCode(Stream stream, uint codeoffset, uint codelimit, NSBMDModel mod, int maxstack) {
            var reader = new BinaryReader(stream);
            //Console.WriteLine("DecodeCode");
            UInt32 codeptr = codeoffset;
            bool begin = false; // whether there is a 0x0b begin code
            int count = 0;

            int stackID = -1;
            int polyStack = -1;
            int polystack2 = -1;
            int curjoint = -1;
            int matid = -1;
            int emptystack = maxstack - 1;
            stream.Seek(codeoffset, SeekOrigin.Begin);
            while (codeptr < codelimit) {
                int c = reader.ReadByte();
                //Console.WriteLine(BitConverter.ToString(new byte[] { (byte)c }, 0, 1));
                int d, e, f, g, h, i, j, k;
                switch (c) {
                    ////////////////////////////////////////////
                    // bone-definition related byte
                    case 0x06: //NodeDesc[000]
                        d = reader.ReadByte();
                        e = reader.ReadByte();
                        f = reader.ReadByte(); // dummy '0'
                        //			printf("DEBUG: %08x: 06: %02x --> %02x\n", codeptr, d, e);
                        codeptr += 4;
                        //curjoint = d;
                        mod.Objects[d].ParentID = e;
                        mod.Objects[d].StackID = stackID = polystack2 = emptystack = emptystack + 1;//stackID + 1;//-1;
                        mod.Objects[d].RestoreID = -1;
                        break;
                    case 0x26: //NodeDesc[001]
                        d = reader.ReadByte();
                        e = reader.ReadByte();
                        f = reader.ReadByte(); // dummy '0'
                        g = reader.ReadByte(); // store stackID
                        //			printf("DEBUG: %08x: %02x: %02x --> %02x\n", codeptr, c, d, e);
                        codeptr += 5;
                        //curjoint = d;
                        mod.Objects[d].ParentID = e;
                        mod.Objects[d].StackID = stackID = polystack2 = g;
                        mod.Objects[d].RestoreID = -1;
                        break;
                    case 0x46: // 4 bytes follow
                        d = reader.ReadByte();
                        e = reader.ReadByte();
                        f = reader.ReadByte(); // dummy '0'
                        g = reader.ReadByte(); // restore stackID
                        //			printf("DEBUG: %08x: %02x: %02x --> %02x\n", codeptr, c, d, e);
                        codeptr += 5;
                        //curjoint = d;
                        mod.Objects[d].ParentID = e;
                        mod.Objects[d].StackID = stackID = polystack2 = emptystack = emptystack + 1; //stackID + 1;
                        mod.Objects[d].RestoreID = stackID = g;
                        break;
                    case 0x66: //NodeDesc[011]
                        d = reader.ReadByte();
                        e = reader.ReadByte();
                        f = reader.ReadByte(); // dummy '0'
                        g = reader.ReadByte(); // store stackID
                        h = reader.ReadByte(); // restore stackID
                        //			printf("DEBUG: %08x: 66: %02x --> %02x\n", codeptr, d, e);
                        codeptr += 6;
                        //curjoint = d;
                        mod.Objects[d].ParentID = e;
                        mod.Objects[d].StackID = stackID = polystack2 = g;
                        mod.Objects[d].RestoreID = h;
                        break;
                    ////////////////////////////////////////////
                    // node's visibility
                    case 0x02: //Node
                        d = reader.ReadByte(); // node ID
                        e = reader.ReadByte(); // 1 = visible, 0 = hide
                        curjoint = d;
                        //polystack2 = mod.Objects[d].StackID;
                        mod.Objects[d].visible = e == 1;
                        //			printf( "DEBUG: %08x: %02x\n", codeptr, c );
                        codeptr += 3;
                        break;
                    ////////////////////////////////////////////
                    // stackID for polygon
                    case 0x03: //Mtx
                        polyStack = reader.ReadByte();
                        codeptr += 2;
                        break;
                    ////////////////////////////////////////////
                    // unknown
                    case 0x07://NodeDesc_BB[000]
                        d = reader.ReadByte();
                        mod.Objects[d].isBillboard = true;
                        //			printf( "DEBUG: %08x: %02x\n", codeptr, c );
                        codeptr += 2;
                        break;
                    case 0x08:
                        d = reader.ReadByte();
                        mod.Objects[d].isBillboard = true;
                        mod.Objects[d].isYBillboard = true;
                        //			printf( "DEBUG: %08x: %02x\n", codeptr, c );
                        codeptr += 2;
                        break;
                    case 0x09://NodeMix[000] Weight
                        d = reader.ReadByte();
                        polyStack = d;
                        e = reader.ReadByte();
                        codeptr += 2;

                        for (int l = 0; l < e; l++) {
                            int var0 = reader.ReadByte();
                            int var1 = reader.ReadByte();
                            int var2 = reader.ReadByte() & 0xff;
                            codeptr += 3;
                        }
                        codeptr += 1;
                        break;
                    ////////////////////////////////////////////
                    // look like BEGIN and END pair
                    case 0x0b: // 0 byte follows
                        if (begin) {
                            //printf("DEBUG: %08x: previous 0x0b not ended.", codeptr);
                        }
                        begin = true;
                        //			printf( "DEBUG: %08x: %02x\n", codeptr, c );
                        codeptr++;
                        break;
                    case 0x2b: // 0 byte follows
                        if (!begin) {
                            //printf( "DEBUG: %08x: previous 0x0b already ended.", codeptr );
                        }
                        begin = false;
                        //			printf( "DEBUG: %08x: %02x\n", codeptr, c );
                        codeptr++;
                        break;
                    ////////////////////////////////////////////
                    case 0x04: //Mat[000]
                    case 0x24:
                    case 0x44:
                        matid = reader.ReadByte();
                        codeptr += 2;
                        count++;
                        break;
                    case 0x05://Shp
                        d = reader.ReadByte();
                        mod.Polygons[d].MatId = matid;
                        if (polyStack != -1) {
                            mod.Polygons[d].StackID = polyStack;
                        } else {
                            mod.Polygons[d].StackID = polystack2;
                        }
                        mod.Polygons[d].JointID = curjoint;
                        mod.Objects[curjoint].childs.Add(d);
                        matid = -1;
                        codeptr += 2;
                        break;
                    case 0x0C://EnvMap
                        d = reader.ReadByte();
                        mod.Materials[d].isEnvironmentMap = true;
                        codeptr += 2;
                        break;
                    ////////////////////////////////////////////
                    case 0x01: //Ret
                        //			printf( "DEBUG: %08x: %02x\n", codeptr, c );
                        codeptr++;
                        return true;
                    case 0x00://padding
                        //codeptr++;
                        break;
                    default:
                        // TODO
                        //printf( "DEBUG: %08x: decodecode: unknown code %02x.\n", codeptr, c );
                        //getchar();
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// ReadMld0.
        /// </summary>
        private static NSBMDModel[] ReadMdl0(Stream stream, int blockoffset) {
            var reader = new EndianBinaryReader(stream, Endianness.LittleEndian);

            int blocksize;
            int blockptr;
            int blocklimit;
            byte num;
            uint r;
            List<NSBMDModel> model = new List<NSBMDModel>();

            ////////////////////////////////////////////////
            // model
            blockptr = blockoffset + 4; // already read the ID, skip 4 bytes
            blocksize = reader.ReadInt32(); // block size
            blocklimit = blocksize + blockoffset;

            stream.Skip(1); // skip dummy '0'
            num = reader.ReadByte(); // no of model
            if (num <= 0) {
                throw new Exception();
            }

            for (var i = 0; i < num; ++i) {
                model.Add(new NSBMDModel());
            }

            var modelOffset = new UInt32[num];

            stream.Skip(10 + 4 + (num * 4)); // skip [char xyz], useless, go straight to model data offset

            ////////////////////////////////////////////////
            // copy model dataoffset
            for (var i = 0; i < num; i++) {
                modelOffset[i] = (uint)(reader.ReadUInt32() + blockoffset);
            }

            ////////////////////////////////////////////////
            // copy model names
            for (var i = 0; i < num; i++) {
                model[i].Name = Utils.ReadNSBMDString(reader);
            }

            ////////////////////////////////////////////////
            // parse model data

            uint totalsize_base = reader.ReadUInt32();
            uint codeoffset_base = reader.ReadUInt32();
            uint texpaloffset_base = reader.ReadUInt32();
            uint polyoffset_base = reader.ReadUInt32();
            uint polyend_base = reader.ReadUInt32();
            stream.Skip(4);
            uint matnum = reader.ReadByte(); // no. of material
            uint polynum = reader.ReadByte(); // no. of polygon
            byte laststack = reader.ReadByte();
            byte unknown1m = reader.ReadByte();
            float modelscale = (float)reader.ReadInt32() / 4096f;
            float boundscale = (float)reader.ReadInt32();// / 4096f;
            int vertexcount = reader.ReadInt16();
            int surfacecount = reader.ReadInt16();
            int trianglecount = reader.ReadInt16();
            int quadcount = reader.ReadInt16();

            model[0].laststackid = laststack;
            model[0].modelScale = modelscale;
            model[0].boundScale = boundscale;
            model[0].boundXmin = (float)NSBMDGlRenderer.Sign(reader.ReadInt16(), 16) / 4096f;
            model[0].boundYmin = (float)NSBMDGlRenderer.Sign(reader.ReadInt16(), 16) / 4096f;
            model[0].boundZmin = (float)NSBMDGlRenderer.Sign(reader.ReadInt16(), 16) / 4096f;
            model[0].boundXmax = (float)NSBMDGlRenderer.Sign(reader.ReadInt16(), 16) / 4096f;
            model[0].boundYmax = (float)NSBMDGlRenderer.Sign(reader.ReadInt16(), 16) / 4096f;
            model[0].boundZmax = (float)NSBMDGlRenderer.Sign(reader.ReadInt16(), 16) / 4096f;

            var polyOffsets = new UInt32[polynum];
            var polyDataSize = new UInt32[polynum];

            for (int i = 0; i < 1; i++) {
                var mod = model[i];

                stream.Seek(modelOffset[i], SeekOrigin.Begin);
                uint codeoffset;
                UInt32 texpaloffset;
                UInt32 polyoffset;
                long texoffset;
                long paloffset;

                uint modoffset = modelOffset[i];
                // the following variables are all offset values
                long totalsize = totalsize_base + modoffset;
                codeoffset = codeoffset_base + modoffset;
                // additional model data, bone definition etc., just follow NsbmdObject section
                texpaloffset = texpaloffset_base + modoffset;
                polyoffset = polyoffset_base + modoffset;
                long polyend = polyend_base + modoffset;

                stream.Skip(5 * 4 + 4 + 2 + 38); // go straight to NsbmdObject

                ////////////////////////////////////////////////
                // NsbmdObject section
                uint objnum;
                int objdatabase;
                uint[] objdataoffset;
                uint[] objdatasize;
                objdatabase = (int)stream.Position;
                stream.Skip(1); // skip dummy '0'
                objnum = reader.ReadByte(); // no of NsbmdObject

                stream.Skip(14 + (objnum * 4)); // skip bytes, go striaght to NsbmdObject data offset

                for (i = 0; i < objnum; ++i) {
                    mod.Objects.Add(new NSBMDObject());
                }

                objdataoffset = new uint[objnum];
                objdatasize = new uint[objnum];


                for (var j = 0; j < objnum; j++) {
                    objdataoffset[j] = (uint)(reader.ReadUInt32() + objdatabase);
                }

                for (var j = 0; j < objnum - 1; j++) {
                    objdatasize[j] = objdataoffset[j + 1] - objdataoffset[j];
                }

                objdatasize[objnum - 1] = (uint)(codeoffset - objdataoffset[objnum - 1]);


                ////////////////////////////////////////////////
                // copy NsbmdObject names
                for (var j = 0; j < objnum; j++) {
                    mod.Objects[j].Name = Utils.ReadNSBMDString(reader);
                    // TO DEBUG
                    Console.WriteLine(mod.Objects[j].Name);
                }

                ////////////////////////////////////////////////
                // parse NsbmdObject information
                for (var j = 0; j < objnum; j++) {
                    if (objdatasize[j] <= 4) {
                        continue;
                    }

                    stream.Seek(objdataoffset[j], SeekOrigin.Begin);
                    ParseNsbmdObject(reader, mod.Objects[j], modelscale);
                }

                ////////////////////////////////////////////////
                // material section
                stream.Seek(texpaloffset, SeekOrigin.Begin); // now get the texture and palette offset
                texoffset = reader.ReadUInt16() + texpaloffset;
                paloffset = reader.ReadUInt16() + texpaloffset;

                // allocate memory for material
                for (int j = 0; j <= matnum; j++) {//i <= matnum; ++i 
                    mod.Materials.Add(new NSBMDMaterial());
                }

                ////////////////////////////////////////////////
                // parse material definition
                // defines RotA material by pairing texture and palette
                stream.Seek(16 + (matnum * 4), SeekOrigin.Current); // go straight to material data offset
                for (var j = 0; j < matnum; j++) // TODO: BAD!
                {
                    mod.Materials[j] = new NSBMDMaterial();
                    blockptr = (int)stream.Position;
                    r = reader.ReadUInt32() + texpaloffset/* + 4 + 12*/;// skip 18 bytes (+ 2 bytes for texoffset, 2 bytes for paloffset)
                    stream.Seek(r, SeekOrigin.Begin);
                    //mod.Materials[j].repeat = reader.ReadByte();
                    //reader.BaseStream.Position -= 1;
                    int dummy = reader.ReadInt16();
                    int sectionSize = reader.ReadInt16();
                    int unknown1 = reader.ReadInt32();//DifAmbColors
                    int unknown2 = reader.ReadInt32();//SpeEmiColors
                    int unknown3 = reader.ReadInt32();//PolyAttrib
                    int constant2 = reader.ReadInt32();//PolyAttrib Mask
                    int texVramOffset = reader.ReadInt16();
                    int texImageParam = reader.ReadInt16();
                    int constant3 = reader.ReadInt32();//texImageParam Mask
                    int constant4 = reader.ReadInt32();
                    int matWidth = reader.ReadInt16();
                    int matHeight = reader.ReadInt16();
                    int unknown4 = reader.ReadInt16();
                    int unknown5 = reader.ReadInt16();
                    int unknown6 = reader.ReadInt32();
                    //int unknown7 = reader.ReadInt32();//if srt S Scale
                    //int unknown8 = reader.ReadInt32();//if srt T Scale
                    //int unknown9 = reader.ReadInt16();//if srt & 60 Rot
                    //int unknownA = reader.ReadInt16();//if srt & 60 S Trans
                    //int unknownB = reader.ReadInt16();//if srt & 60 T Trans

                    //uint polyParam = reader.ReadUInt32();
                    //reader.ReadInt16();
                    //ushort texImageParam = reader.ReadUInt16();
                    mod.Materials[j].repeatS = texImageParam & 1;
                    mod.Materials[j].repeatT = texImageParam >> 1 & 1;
                    mod.Materials[j].flipS = texImageParam >> 2 & 1;
                    mod.Materials[j].flipT = texImageParam >> 3 & 1;
                    /*if ((texImageParam >> 14 & 0x03) == 1)
                    {
                        mod.Materials[j].scaleS = /*1 << /(texImageParam >> 12 & 2) + 1;
                        mod.Materials[j].scaleT = /*1 << /(texImageParam >> 14 & 2) + 1;
                    }
                    else
                    {
                        mod.Materials[j].scaleS = 1;
                        mod.Materials[j].scaleT = 1;
                    }*/
                    switch (texImageParam >> 14 & 0x03) {
                        case 0:
                            mod.Materials[j].scaleS = 1;
                            mod.Materials[j].scaleT = 1;
                            mod.Materials[j].transS = 0;
                            mod.Materials[j].transT = 0;
                            break;

                        case 1:
                            {
                                int sscale = (int)reader.ReadInt32();// >> 0 & 0xFFFFFFFF;
                                sscale = NSBMDGlRenderer.Sign(sscale, 32);
                                int tscale = (int)reader.ReadInt32();// >> 0 & 0xFFFFFFFF;
                                tscale = NSBMDGlRenderer.Sign(tscale, 32);
                                //int strans = (int)unknown2 >> 0 & 0xFFFF;
                                //int ttrans = (int)unknown2 >> 16 & 0xFFFF;

                                mod.Materials[j].scaleS = (float)sscale / 4096f;
                                mod.Materials[j].scaleT = (float)tscale / 4096f;
                                if (sectionSize >= 60) {
                                    mod.Materials[j].rot = (float)reader.ReadInt16() / 4096f;
                                    mod.Materials[j].transS = (float)reader.ReadInt16() / 4096f;
                                    mod.Materials[j].transT = (float)reader.ReadInt16() / 4096f;
                                } else {

                                }
                                break;
                            }
                        case 2:
                        case 3:
                            mod.Materials[j].mtx = new float[16];
                            for (int k = 0; k < 16; k++) {
                                mod.Materials[j].mtx[k] = reader.ReadInt32();
                            }
                            break;

                        default:
                            break;
                            // throw new Exception(String.Format("BMD: unsupported texture coord transform mode {0}", matgroup.m_TexParams >> 30));
                    }
                    mod.Materials[j].width = matWidth;
                    mod.Materials[j].height = matHeight;
                    int width = 8 << (texImageParam >> 4 & 7);
                    int height = 8 << (texImageParam >> 7 & 7);
                    //int m_DifAmbColors = reader.ReadInt32();
                    //int m_SpeEmiColors = reader.ReadInt32();
                    mod.Materials[j].DiffuseColor = SM64DSe.Helper.BGR15ToColor((ushort)(unknown1 & 0x7FFF));
                    mod.Materials[j].AmbientColor = SM64DSe.Helper.BGR15ToColor((ushort)(unknown1 >> 16 & 0x7FFF));
                    mod.Materials[j].SpecularColor = SM64DSe.Helper.BGR15ToColor((ushort)(unknown2 & 0x7FFF));
                    mod.Materials[j].EmissionColor = SM64DSe.Helper.BGR15ToColor((ushort)(unknown2 >> 16 & 0x7FFF));
                    int a = (int)((unknown3 >> 16) & 31);
                    mod.Materials[j].Alpha = a;//a * 2 + 1;//a * 2 + (a + 31) / 32;
                    mod.Materials[j].PolyAttrib = (uint)unknown3;
                    mod.Materials[j].diffuseColor = (unknown1 >> 15 & 1) == 1;
                    mod.Materials[j].shine = (unknown2 >> 15 & 1) == 1;
                    stream.Seek(blockptr + 4, SeekOrigin.Begin);
                }
                for (var j = 0; j < matnum; j++) {
                    mod.Materials[j].MaterialName = Utils.ReadNSBMDString(reader);
                }

                ////////////////////////////////////////////////
                // now go to read the texture definition
                stream.Seek(texoffset, SeekOrigin.Begin);
                stream.Skip(1); // skip dummy '0'
                int texnum = reader.ReadByte();
                Debug.Assert(texnum <= matnum);
                Console.WriteLine(String.Format("texnum: {0}", texnum));

                if (texnum > 0) {
                    stream.Seek(14 + (texnum * 4), SeekOrigin.Current); // go straight to data offsets
                    for (var j = 0; j < texnum; j++) {
                        Int32 flags = reader.ReadInt32();
                        int numPairs = flags >> 16 & 0xf;
                        int dummy = flags >> 24 & 0xf;
                        blockptr = (int)stream.Position;
                        stream.Seek((flags & 0xffff) + texpaloffset, SeekOrigin.Begin);
                        NSBMDTexture t = new NSBMDTexture();
                        for (int k = 0; k < numPairs; k++) {
                            uint texmatid = reader.ReadByte();
                            mod.tex_mat.Add((int)texmatid);
                            mod.Materials[j].texmatid.Add(texmatid);
                            t.texmatid.Add(texmatid);
                        }
                        mod.Textures.Add(t);
                        stream.Seek(blockptr, SeekOrigin.Begin);
                    }

                    for (var j = 0; j < texnum; j++) // copy texture names
                    {
                        NSBMDMaterial mat = mod.Materials[j];

                        mat.texname = Utils.ReadNSBMDString(reader);
                        reader.BaseStream.Position -= 16;
                        mod.Textures[j].texname = Utils.ReadNSBMDString(reader);

                        Console.WriteLine("tex (matid={0}): {1}", mat.texmatid, mat.texname);
                    }
                }

                ////////////////////////////////////////////////
                // now go to read the palette definition
                stream.Seek(paloffset, SeekOrigin.Begin);
                stream.Skip(1); // skip dummy '0'
                int palnum = reader.ReadByte(); // no of palette definition
                Debug.Assert(palnum <= matnum); // may not always hold?
                Console.WriteLine("DEBUG: palnum = {0}", palnum);

                if (palnum > 0) {
                    stream.Seek(14 + (palnum * 4), SeekOrigin.Current); // go straight to data offsets
                    for (var j = 0; j < palnum; j++) // matching palette with material
                    {
                        Int32 flags = reader.ReadInt32();
                        int numPairs = flags >> 16 & 0xf;
                        int dummy = flags >> 24 & 0xf;
                        blockptr = (int)stream.Position;
                        stream.Seek((flags & 0xffff) + texpaloffset, SeekOrigin.Begin);
                        NSBMDPalette t = new NSBMDPalette();
                        for (int k = 0; k < numPairs; k++) {
                            uint texmatid = reader.ReadByte();
                            mod.tex_mat.Add((int)texmatid);
                            mod.Materials[j].texmatid.Add(texmatid);
                            t.palmatid.Add(texmatid);
                        }
                        mod.Palettes.Add(t);
                        stream.Seek(blockptr, SeekOrigin.Begin);
                    }
                    for (var j = 0; j < palnum; j++) // copy palette names
                    {
                        int palmatid = (int)mod.Materials[j].palmatid;
                        mod.Materials[palmatid].palname = Utils.ReadNSBMDString(reader);
                        reader.BaseStream.Position -= 16;
                        mod.Palettes[j].palname = Utils.ReadNSBMDString(reader);
                        // TO DEBUG
                        //Console.WriteLine("pal (matid={0}): {1}", palmatid, mod.Materials[palmatid].palname);
                    }
                }
                ////////////////////////////////////////////////
                // Polygon
                stream.Seek(polyoffset, SeekOrigin.Begin);
                stream.Skip(1); // skip dummy '0'
                r = reader.ReadByte(); // no of polygon
                Console.WriteLine("DEBUG: polynum = {0}", polynum);

                for (var j = 0; j <= polynum; j++) {
                    mod.Polygons.Add(new NSBMDPolygon());
                }


                stream.Skip(14 + (polynum * 4)); // skip bytes, go straight to data offset


                for (var j = 0; j < polynum; j++)
                    polyOffsets[j] = reader.ReadUInt32() + polyoffset;
                try {
                    for (var j = 0; j < polynum; j++) // copy polygon names
                    {
                        mod.Polygons[j].Name = Utils.ReadNSBMDString(reader);
                        //Console.WriteLine(mod.Polygons[j].Name);
                    }
                } catch { }
                ////////////////////////////////////////////////
                // now go to the polygon data, there is RotA 16-byte-header before geometry commands
                for (var j = 0; j < polynum; j++) {
                    var poly = mod.Polygons[j];
                    //////////////////////////////////////////////////////////
                    poly.MatId = -1; // DEFAULT: indicate no associated material
                    //////////////////////////////////////////////////////////
                    //stream.Seek(polyOffsets[j] + 8, SeekOrigin.Begin); // skip 8 unknown bytes
                    short dummy = reader.ReadInt16();
                    short headerSize = reader.ReadInt16();
                    int unknown2 = reader.ReadInt32();
                    polyOffsets[j] += reader.ReadUInt32();
                    polyDataSize[j] = reader.ReadUInt32();
                    //printf( "poly %2d '%-16s': dataoffset: %08x datasize %08x\n", j, poly->polyname, poly->dataoffset, poly->datasize );
                }
                //}

                ////////////////////////////////////////////////
                // read the polygon data into memory
                for (var j = 0; j < polynum; j++) {
                    var poly = mod.Polygons[j];
                    stream.Seek(polyOffsets[j], SeekOrigin.Begin);
                    poly.PolyData = reader.ReadBytes((int)polyDataSize[j]);
                }
                //}

                ////////////////////////////////////////////////
                // decode the additional model data
                DecodeCode(stream, codeoffset, texpaloffset, mod, laststack);
            }

            //modelnum = num;
            return model.ToArray();
        }
        public static float getFixed(int value, int sign, int var, int frac) {
            float fixe = value;
            if (sign == 1) {
                fixe = NSBMDGlRenderer.Sign(value, GetSizeOfObject(value));
            }
            float divide = 1 << frac;
            fixe /= divide;
            return fixe;
        }
        public static int GetSizeOfObject(object obj) {
            if (obj is Int32) {
                return 32;
            }
            if (obj is Int16) {
                return 16;
            }
            if (obj is byte) {
                return 8;
            }
            return -1;
        }

        /// <summary>
        /// Parse single NSBMD object.
        /// </summary>
        private static void ParseNsbmdObject(EndianBinaryReader reader, NSBMDObject nsbmdObject, float modelscale) {
            ushort v = reader.ReadUInt16();
            Int16 divide = reader.ReadInt16();
            divide = (short)NSBMDGlRenderer.Sign(divide, 16);
            int unknown = v >> 12 & 0xf;
            nsbmdObject.StackID = unknown;
            //nsbmdObject.isBillboard = ((v >> 12 & 0xf) == 1?true:false);
            float[] s = NSBMDGlRenderer.loadIdentity();
            float[] r = NSBMDGlRenderer.loadIdentity();
            float[] t = NSBMDGlRenderer.loadIdentity();
            if ((v & 1) == 0) {
                nsbmdObject.Trans = true;

                nsbmdObject.X = (float)reader.ReadInt32() / 4096f / modelscale;//(float)getdword(reader.ReadBytes(4)) / 4096f; //(float)(reader.ReadDouble() / 4096d);//.ReadUInt32() / 4096;
                nsbmdObject.Y = (float)reader.ReadInt32() / 4096f / modelscale;//(float)getdword(reader.ReadBytes(4)) / 4096f;//(float)(reader.ReadDouble() / 4096d);
                nsbmdObject.Z = (float)reader.ReadInt32() / 4096f / modelscale;//(float)getdword(reader.ReadBytes(4)) / 4096f;//(float)(reader.ReadDouble() / 4096d);
                t = NSBMDGlRenderer.Translate(t, nsbmdObject.X, nsbmdObject.Y, nsbmdObject.Z);
            }
            if ((v >> 3 & 0x1) == 0x1) {
                nsbmdObject.IsRotated = true;
                float a = reader.ReadInt16();
                a = NSBMDGlRenderer.Sign((int)a, 16) / 4096f;
                float b = reader.ReadInt16();
                b = NSBMDGlRenderer.Sign((int)b, 16) / 4096f;
                nsbmdObject.Pivot = (int)(v >> 4) & 0x0f;
                nsbmdObject.Neg = (int)(v >> 8) & 0x0f;
                nsbmdObject.RotA = a;
                nsbmdObject.RotB = b;
                nsbmdObject.rotate_mtx = mtxPivot(new float[] { nsbmdObject.RotA, nsbmdObject.RotB }, nsbmdObject.Pivot, nsbmdObject.Neg);
                r = NSBMDGlRenderer.multMatrix(r, nsbmdObject.rotate_mtx);
            }
            if ((v >> 1 & 1) == 0 && (v >> 3 & 1) == 0) {
                float[] a = new float[16];
                a[0] = 1.0F;
                a[5] = 1.0F;
                a[10] = 1.0F;
                a[15] = 1.0F;
                float[] rotate = new float[8];
                //msg = (new StringBuilder()).append(msg).append(" | R: ").toString();
                for (int j = 0; j < rotate.Length; j++) {
                    //dataParser _tmp4 = pa;
                    int value = NSBMDGlRenderer.Sign(reader.ReadInt16(), 16); //dataParser.getSign(data, offset + 4 + j * 2 + jump, 2);
                    rotate[j] = (float)value / 4096f;
                    //msg = (new StringBuilder()).append(msg).append(pad(Integer.valueOf(value), 4)).toString();
                    //if(j + 1 < rotate.length)
                    //   msg = (new StringBuilder()).append(msg).append(", ").toString();
                }

                a[0] = (float)divide / 4096f;
                a[1] = rotate[0];
                a[2] = rotate[1];
                a[4] = rotate[2];
                a[5] = rotate[3];
                a[6] = rotate[4];
                a[8] = rotate[5];
                a[9] = rotate[6];
                a[10] = rotate[7];
                nsbmdObject.rotate_mtx = a;
                nsbmdObject.IsRotated = true;
                r = NSBMDGlRenderer.multMatrix(r, nsbmdObject.rotate_mtx);
            }
            if ((v >> 2 & 1) == 0) {
                float[] scale = new float[3];
                for (int j = 0; j < scale.Length; j++) {
                    int value = reader.ReadInt32();
                    scale[j] = (float)value / 4096f;
                }
                nsbmdObject.scale = scale;
                nsbmdObject.IsScaled = true;
                s = NSBMDGlRenderer.scale(s, scale[0], scale[1], scale[2]);
            }
            nsbmdObject.materix = NSBMDGlRenderer.loadIdentity();
            nsbmdObject.materix = NSBMDGlRenderer.multMatrix(nsbmdObject.materix, t);
            nsbmdObject.materix = NSBMDGlRenderer.multMatrix(nsbmdObject.materix, r);
            nsbmdObject.materix = NSBMDGlRenderer.multMatrix(nsbmdObject.materix, s);
        }
        public static float[] mtxPivot(float[] ab, int pv, int neg) {
            float[] data = new float[16];
            data[15] = 1.0F;
            float one = 1.0F;
            float a = ab[0];
            float b = ab[1];
            float a2 = a;
            float b2 = b;
            switch (neg) {
                case 1: // '\001'
                case 3: // '\003'
                case 5: // '\005'
                case 7: // '\007'
                case 9: // '\t'
                case 11: // '\013'
                case 13: // '\r'
                case 15: // '\017'
                    one = -1F;
                    // fall through
                    goto case 2;
                case 2: // '\002'
                case 4: // '\004'
                case 6: // '\006'
                case 8: // '\b'
                case 10: // '\n'
                case 12: // '\f'
                case 14: // '\016'
                default:
                    switch (neg) {
                        case 2: // '\002'
                        case 3: // '\003'
                        case 6: // '\006'
                        case 7: // '\007'
                        case 10: // '\n'
                        case 11: // '\013'
                        case 14: // '\016'
                        case 15: // '\017'
                            b2 = -b2;
                            // fall through
                            goto case 4;
                        case 4: // '\004'
                        case 5: // '\005'
                        case 8: // '\b'
                        case 9: // '\t'
                        case 12: // '\f'
                        case 13: // '\r'
                        default:
                            switch (neg) {
                                case 4: // '\004'
                                case 5: // '\005'
                                case 6: // '\006'
                                case 7: // '\007'
                                case 12: // '\f'
                                case 13: // '\r'
                                case 14: // '\016'
                                case 15: // '\017'
                                    a2 = -a2;
                                    // fall through
                                    goto case 8;
                                case 8: // '\b'
                                case 9: // '\t'
                                case 10: // '\n'
                                case 11: // '\013'
                                default:
                                    switch (pv) {
                                        case 0: // '\0'
                                            data[0] = one;
                                            data[5] = a;
                                            data[6] = b;
                                            data[9] = b2;
                                            data[10] = a2;
                                            break;

                                        case 1: // '\001'
                                            data[1] = one;
                                            data[4] = a;
                                            data[6] = b;
                                            data[8] = b2;
                                            data[10] = a2;
                                            break;

                                        case 2: // '\002'
                                            data[2] = one;
                                            data[4] = a;
                                            data[5] = b;
                                            data[8] = b2;
                                            data[9] = a2;
                                            break;

                                        case 3: // '\003'
                                            data[4] = one;
                                            data[1] = a;
                                            data[2] = b;
                                            data[9] = b2;
                                            data[10] = a2;
                                            break;

                                        case 4: // '\004'
                                            data[5] = one;
                                            data[0] = a;
                                            data[2] = b;
                                            data[8] = b2;
                                            data[10] = a2;
                                            break;

                                        case 5: // '\005'
                                            data[6] = one;
                                            data[0] = a;
                                            data[1] = b;
                                            data[8] = b2;
                                            data[9] = a2;
                                            break;

                                        case 6: // '\006'
                                            data[8] = one;
                                            data[1] = a;
                                            data[2] = b;
                                            data[5] = b2;
                                            data[6] = a2;
                                            break;

                                        case 7: // '\007'
                                            data[9] = one;
                                            data[0] = a;
                                            data[2] = b;
                                            data[4] = b2;
                                            data[6] = a2;
                                            break;

                                        case 8: // '\b'
                                            data[10] = one;
                                            data[0] = a;
                                            data[1] = b;
                                            data[4] = b2;
                                            data[5] = a2;
                                            break;

                                        case 9: // '\t'
                                            data[0] = -a;
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
            return data;
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
        /// <summary>
        /// Generate NSBMD from stream.
        /// </summary>
        internal static NSBMD FromStream(Stream stream) {
            var result = new NSBMD();

            var reader = new BinaryReader(stream);

            int tmp;
            tmp = reader.ReadInt32();
            if (tmp != NDS_TYPE_BMD0)
                throw new Exception();

            tmp = reader.ReadUInt16();
            if (tmp != NDS_TYPE_BYTEORDER)
                throw new Exception();

            tmp = reader.ReadUInt16();
            if (tmp != NDS_TYPE_UNK2)
                throw new Exception();

            int filesize = reader.ReadInt32();
            if (filesize > stream.Length)
                throw new Exception();

            int numblock = reader.ReadInt32();
            numblock >>= 16;
            if (numblock == 0) {
                throw new Exception("DEBUG: no of block zero.\n");
            }
            ///////////////////////////////////////////////////////
            // allocate memory for storing blockoffset
            int[] blockoffset = new int[numblock];
            for (int i = 0; i < numblock; i++) {
                tmp = reader.ReadInt32();
                blockoffset[i] = tmp;
            }

            ///////////////////////////////////////////////////////
            // now go to read the blocks
            for (int i = 0; i < numblock; i++) {
                stream.Position = blockoffset[i];
                uint id = reader.ReadUInt32();

                switch (id) {
                    case NDS_TYPE_MDL0:
                        result.models = ReadMdl0(stream, blockoffset[i]);

                        break;
                    case NDS_TYPE_TEX0:
                        int palnum;
                        int texnum;
                        result.materials = NSBTXLoader.ReadTex0(stream, blockoffset[i], out texnum, out palnum, out result.Textures, out result.Palettes);
                        break;
                    default:
                        throw new Exception("Unknown ID");
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Type for storing RGBA data in.
    /// </summary>
    public struct RGBA {
        #region Data Members (6)

        public byte A;
        public byte R;
        public byte G;
        public byte B;

        /// <summary>
        /// Transparent color.
        /// </summary>
        public static RGBA Transparent = new RGBA { R = 0xFF, A = 0x0 };

        public static RGBA fromColor(System.Drawing.Color c) {
            RGBA a = new RGBA();
            a.R = c.R;
            a.G = c.G;
            a.B = c.B;
            a.A = c.A;
            return a;
        }

        /// <summary>
        /// Index accessor.
        /// </summary>
        public byte this[int i] {
            get {
                switch (i) {
                    case 0:
                        return R;
                    case 1:
                        return G;
                    case 2:
                        return B;
                    case 3:
                        return A;
                    default:
                        throw new Exception();
                }
            }
            set {
                switch (i) {
                    case 0:
                        R = value;
                        break;
                    case 1:
                        G = value;
                        break;
                    case 2:
                        B = value;
                        break;
                    case 3:
                        A = value;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
        public Color ToColor() {
            return Color.FromArgb(A, R, G, B);
        }
        #endregion Data Members
    }
}
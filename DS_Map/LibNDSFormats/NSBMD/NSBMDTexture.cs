using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNDSFormats.NSBMD
{
    public class NSBMDTexture
    {
        public int format;
        public int height;
        public byte repeat;
        public byte[] spdata;
        public byte[] texdata;
        public List<uint> texmatid = new List<uint>();
        public string texname = String.Empty;
        public UInt32 texoffset;
        public UInt32 texsize;
        public int width;
        public int color0;
    }
}

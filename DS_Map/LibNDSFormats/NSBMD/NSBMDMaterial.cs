// Material type for NSBMD models.
// Code adapted from kiwi.ds' NSBMD Model Viewer.

using System;
using System.Drawing;
using System.Collections.Generic;

namespace LibNDSFormats.NSBMD
{
    public class NSBMDMaterial
    {
        #region Fields (16) 

        public int color0;
        public int format;
        public int height;
        public RGBA[] paldata;
        public uint palmatid;
        public string palname = String.Empty;
        public UInt32 paloffset;
        public UInt32 palsize;
        public byte repeat;
        public byte[] spdata;
        public byte[] texdata;
        public List<uint> texmatid = new List<uint>();
        public bool isEnvironmentMap = false;
        public string texname = String.Empty;
        public UInt32 texoffset;
        public UInt32 texsize;
        public int width;
        public string MaterialName;
        public int repeatS;
        public int repeatT;
        public int flipS;
        public int flipT;
        public float rot;
        public float scaleT;
        public float scaleS;
        public float transT;
        public float transS;
        public Color DiffuseColor;
        public Color AmbientColor;
        public Color SpecularColor;
        public Color EmissionColor;
        public uint PolyAttrib;
        public int Alpha;
        public bool diffuseColor;
		public bool shine;
        public float[] mtx = null;

        #endregion Fields 

        #region Methods (1) 

        // Public Methods (1) 

        /// <summary>
        /// Copy data to other NSBMD material
        /// </summary>
        /// <param name="other">Other NSBMD material.</param>
        public NSBMDMaterial CopyTo(NSBMDMaterial other1)
        {
            NSBMDMaterial other = other1;
            other.texname = texname;
            other.texoffset = texoffset;
            other.texsize = texsize;
            other.format = format;
            other.color0 = color0;
            other.width = width;
            other.height = height;
            other.texdata = texdata;
            other.spdata = spdata;
            other.MaterialName = MaterialName;
            return other;
        }

        #endregion Methods 
    }
}
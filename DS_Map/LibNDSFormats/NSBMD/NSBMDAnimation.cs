using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNDSFormats.NSBMD
{
    /// <summary>
    /// Type for NSBCA objects.
    /// </summary>
    public class NSBMDAnimation
    {
	    public int dataoffset = 0;
        public int flag = 0;
        public float[] m_trans = new float[3];
        public float a, b = 0;
        public float[] m_scale = new float[3];
        public int frame = 0;
        public int framelen = 0;
        public List<Int16> animdata = new List<short>();
    }
}

// Polygon definition for NSBMD models.
// Code adapted from kiwi.ds' NSBMD Model Viewer.

using System;

namespace LibNDSFormats.NSBMD
{
	/// <summary>
	/// Type for NSBMd polygons.
	/// </summary>
    public class NSBMDPolygon
    {
        #region Properties (4) 

        /// <summary>
        /// Used material ID.
        /// </summary>
        public int MatId { get; set; }

        /// <summary>
        /// Name of polygon.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Polygon data.
        /// </summary>
        public byte[] PolyData { get; set; }

        //public UInt32 DataOffset{ get; set; }
        //public int DataSize{ get; set; }
        /// <summary>
        /// StackID of polygon.
        /// </summary>
        public int StackID { get; set; }

        public int JointID { get; set; }

        #endregion Properties 
    }
}
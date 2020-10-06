// Loader for NSBMD files.
// Code adapted from kiwi.ds' NSBMD Model Viewer.

using System.IO;

namespace LibNDSFormats.NSBMD
{
	/// <summary>
	/// Loader for NSBMD data.
	/// </summary>
    public static class NSBMDLoader
    {
    	/// <summary>
    	/// Load NSBMD from stream.
    	/// </summary>
    	/// <param name="stream">Stream with NSBMD data.</param>
    	/// <returns>NSBMD object.</returns>
        public static NSBMD LoadNSBMD(Stream stream)
        {
            return NSBMD.FromStream(stream);   
        }
    }
}

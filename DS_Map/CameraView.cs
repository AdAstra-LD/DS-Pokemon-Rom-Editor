using LibNDSFormats.NSBMD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;

namespace DSPRE
{
    public partial class CameraView : Form
    {
        #region Variables
        string folder;
        bool disableHandlers = new bool();
        RomInfo info;
        NSBMD currentNSBMD;
        NSBMDGlRenderer renderer = new NSBMDGlRenderer();

        public static float ang = 0.0f;
        public static float dist = 12.8f;
        public static float elev = 50.0f;
        public static float tempAng = 0.0f;
        public static float tempDist = 0.0f;
        public static float tempElev = 0.0f;
        public float perspective = 45f;
        #endregion

        public CameraView(RomInfo romInfo)
        {
            InitializeComponent();
            info = romInfo;

            cameraOpenGLControl.InitializeContexts();
            cameraOpenGLControl.MakeCurrent();
            Gl.glEnable(Gl.GL_TEXTURE_2D);

        }
    }
}

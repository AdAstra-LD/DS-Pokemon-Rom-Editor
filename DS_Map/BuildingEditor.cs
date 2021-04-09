using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LibNDSFormats.NSBMD;
using LibNDSFormats.NSBTX;
using Tao.OpenGl;
using static DSPRE.RomInfo;

namespace DSPRE {
    public partial class BuildingEditor : Form {
        #region Variables
        string folder;
        bool disableHandlers = new bool();
        RomInfo rom;
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

        public BuildingEditor(RomInfo romInfo) {
            InitializeComponent();
            rom = romInfo;

            buildingOpenGLControl.InitializeContexts();
            buildingOpenGLControl.MakeCurrent();
            buildingOpenGLControl.MouseWheel += new MouseEventHandler(buildingOpenGLControl_MouseWheel);
            Gl.glEnable(Gl.GL_TEXTURE_2D);

            if (RomInfo.gameFamily == "HGSS")
                interiorCheckBox.Enabled = true;

            disableHandlers = true;
            FillListBox(false);
            FillTexturesBox();
            textureComboBox.SelectedIndex = 0;
            disableHandlers = false;
            buildingEditorBldListBox.SelectedIndex = 0;
        }

        #region Subroutines
        private void CreateEmbeddedTexturesFile(int modelID, bool interior) {
            string readingPath = folder + rom.GetBuildingModelsDirPath(interior) + "\\" + modelID.ToString("D4");
            string writingPath = Path.GetTempPath() + "BLDtexture.nsbtx";

            byte[] txFile = File.ReadAllBytes(readingPath);
            DSUtils.WriteToFile(writingPath, DSUtils.GetTexturesFromTexturedNSBMD(txFile), fromScratch: true);
        }
        private void FillListBox(bool interior) {
            int modelCount = Directory.GetFiles(folder + rom.GetBuildingModelsDirPath(interior)).Length;
            for (int currentIndex = 0; currentIndex < modelCount; currentIndex++) {
                string filePath = folder + rom.GetBuildingModelsDirPath(interior) + "\\" + currentIndex.ToString("D4");

                using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath))) {
                    string nsbmdName = ReadNSBMDname(reader);
                    buildingEditorBldListBox.Items.Add("[" + currentIndex.ToString("D3") + "] " + nsbmdName);
                }
            }
        }
        private void FillTexturesBox() {
            int texturesCount = Directory.GetFiles(folder + RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir).Length;
            textureComboBox.Items.Add("Embedded textures");

            for (int i = 0; i < texturesCount; i++)
                textureComboBox.Items.Add("Texture " + i);
        }
        private void LoadBuildingModel(int modelID, bool interior) {
            string path = folder + rom.GetBuildingModelsDirPath(interior) + "\\" + modelID.ToString("D4");
            using (Stream fs = new FileStream(path, FileMode.Open))
                currentNSBMD = NSBMDLoader.LoadNSBMD(fs);
        }
        private void LoadModelTextures(int fileID) {
            string path;
            if (fileID > -1)
                path = folder + RomInfo.gameDirs[RomInfo.DirNames.buildingTextures].unpackedDir + "\\" + fileID.ToString("D4");
            else
                path = Path.GetTempPath() + "BLDtexture.nsbtx"; // Load Embedded textures if the argument passed to this function is -1
            try {
                currentNSBMD.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(path)), out currentNSBMD.Textures, out currentNSBMD.Palettes);
                currentNSBMD.MatchTextures();
            } catch { }
        }
        private void RenderModel() {
            MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File bta = new MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File();
            MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File btp = new MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File();
            MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File bca = new MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File();
            int[] aniframeS = new int[0];

            buildingOpenGLControl.Invalidate(); // Invalidate drawing surface
            SetupRenderer(ang, dist, elev, perspective); // Adjust rendering settings

            /* Render the building model */
            renderer.Model = currentNSBMD.models[0];
            Gl.glScalef(currentNSBMD.models[0].modelScale / 32, currentNSBMD.models[0].modelScale / 32, currentNSBMD.models[0].modelScale / 32);
            renderer.RenderModel("", bta, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, bca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, btp, currentNSBMD);
        }
        private void SetupRenderer(float ang, float dist, float elev, float perspective) {
            Gl.glEnable(Gl.GL_RESCALE_NORMAL);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glFrontFace(Gl.GL_CCW);
            Gl.glClearDepth(1);
            Gl.glEnable(Gl.GL_ALPHA_TEST);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glAlphaFunc(Gl.GL_GREATER, 0f);
            Gl.glClearColor(51f / 255f, 51f / 255f, 51f / 255f, 1f);
            float aspect;
            Gl.glViewport(0, 0, buildingOpenGLControl.Width, buildingOpenGLControl.Height);
            aspect = buildingOpenGLControl.Width / buildingOpenGLControl.Height;//(vp[2] - vp[0]) / (vp[3] - vp[1]);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(perspective, aspect, 0.02f, 1000000.0f);//0.02f, 32.0f);
            Gl.glTranslatef(0, 0, -dist);
            Gl.glRotatef(elev, 1, 0, 0);
            Gl.glRotatef(ang, 0, 1, 0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glTranslatef(0, 0, -dist);
            Gl.glRotatef(elev, 1, 0, 0);
            Gl.glRotatef(-ang, 0, 1, 0);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLoadIdentity();
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            Gl.glColor3f(1.0f, 1.0f, 1.0f);
            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
        }
        #endregion

        private void buildingOpenGLControl_MouseWheel(object sender, MouseEventArgs e) // Zoom In/Out
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                dist += (float)e.Delta / 200;
            else
                dist -= (float)e.Delta / 200;
            RenderModel();
        }
        private void buildingEditorListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || buildingEditorBldListBox.SelectedIndex < 0)
                return;

            LoadBuildingModel(buildingEditorBldListBox.SelectedIndex, interiorCheckBox.Checked);
            CreateEmbeddedTexturesFile(buildingEditorBldListBox.SelectedIndex, interiorCheckBox.Checked);
            LoadModelTextures(textureComboBox.SelectedIndex - 1);
            RenderModel();
        }
        private void exportButton_Click(object sender, EventArgs e) {
            SaveFileDialog em = new SaveFileDialog();
            em.Filter = "NSBMD model (*.nsbmd)|*.nsbmd";
            em.FileName = buildingEditorBldListBox.SelectedItem.ToString();
            if (em.ShowDialog(this) != DialogResult.OK)
                return;

            File.Copy(folder + rom.GetBuildingModelsDirPath(interiorCheckBox.Checked) + "\\" + buildingEditorBldListBox.SelectedIndex.ToString("D4"), em.FileName, true);
        }
        private void importButton_Click(object sender, EventArgs e) {
            OpenFileDialog im = new OpenFileDialog();
            im.Filter = "NSBMD model (*.nsbmd)|*.nsbmd";
            if (im.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryReader reader = new BinaryReader(new FileStream(im.FileName, FileMode.Open))) {
                if (reader.ReadUInt32() != NSBMD.NDS_TYPE_BMD0) {
                    MessageBox.Show("Please select an NSBMD file.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } else {
                    int currentIndex = buildingEditorBldListBox.SelectedIndex;

                    File.Copy(im.FileName, folder + rom.GetBuildingModelsDirPath(interiorCheckBox.Checked) + "\\" + currentIndex.ToString("D4"), true);
                    string nsbmdName = ReadNSBMDname(reader);
                    buildingEditorBldListBox.Items[currentIndex] = "[" + currentIndex.ToString("D3") + "] " + nsbmdName;
                    buildingEditorListBox_SelectedIndexChanged(null, null);
                }
            }
        }
        private void interiorCheckBox_CheckedChanged(object sender, EventArgs e) {
            disableHandlers = true;

            buildingEditorBldListBox.Items.Clear();
            FillListBox(interiorCheckBox.Checked);

            disableHandlers = false;

            buildingEditorBldListBox.SelectedIndex = 0;
        }
        private void textureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            LoadModelTextures(textureComboBox.SelectedIndex - 1);
            RenderModel();
        }
        private void buildingOpenGLControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Right:
                    ang += 1;
                    break;
                case Keys.Left:
                    ang -= 1;
                    break;
                case Keys.Down:
                    elev += 1;
                    break;
                case Keys.Up:
                    elev -= 1;
                    break;
            }
            RenderModel();
        }

        #region Utils
        private string ReadNSBMDname(BinaryReader reader) {
            reader.BaseStream.Position = 0x14;

            if (reader.ReadUInt32() == NSBMD.NDS_TYPE_MDL0) { //MDL0
                reader.BaseStream.Position = 0x34;
            } else {
                reader.BaseStream.Position = 0x38;
            }

            return Encoding.UTF8.GetString(reader.ReadBytes(16));
        }
        #endregion
    }
}

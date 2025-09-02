using DSPRE.Resources;
using DSPRE.ROMFiles;
using LibNDSFormats.NSBMD;
using LibNDSFormats.NSBTX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using static DSPRE.RomInfo;
using static Tao.Platform.Windows.Winmm;

namespace DSPRE.Editors
{
    public partial class MapEditor : UserControl
    {
        MainProgram _parent;
        public bool mapEditorIsReady { get; set; } = false;
        public MapEditor()
        {
            InitializeComponent();
        }

        #region Map Editor

        #region Variables & Constants 
        public const int mapEditorSquareSize = 19;

        /* Map Rotation vars */
        public bool lRot;
        public bool rRot;
        public bool uRot;
        public bool dRot;

        /* Screenshot Interpolation mode */
        public InterpolationMode intMode;

        /*  Camera settings */
        public bool hideBuildings = new bool();
        public bool mapTexturesOn = true;
        public bool bldTexturesOn = true;
        public static float ang = 0.0f;
        public static float dist = 12.8f;
        public static float elev = 50.0f;
        public float perspective = 45f;

        private byte bldDecimalPositions = 1;

        /* Renderers */
        public static NSBMDGlRenderer mapRenderer = new NSBMDGlRenderer();
        public static NSBMDGlRenderer buildingsRenderer = new NSBMDGlRenderer();

        /* Map file */
        MapFile currentMapFile;

        /* Permission painters */
        public Pen paintPen;
        public int Transparency = 128;
        public SolidBrush paintBrush;
        public SolidBrush textBrush;
        public byte paintByte;
        StringFormat sf;
        public Rectangle mainCell;
        public Rectangle smallCell;
        public Rectangle painterBox = new Rectangle(0, 0, 100, 100);
        public Font textFont;
        #endregion

        #region Subroutines
        private void FillBuildingsBox()
        {
            buildingsListBox.Items.Clear();

            uint id = 0;

            for (int i = 0; i < currentMapFile.buildings.Count; i++)
            {
                id = currentMapFile.buildings[i].modelID;
                string baseName = (i + 1).ToString("D2") + MapHeader.nameSeparator;
                try
                {
                    buildingsListBox.Items.Add(baseName + buildIndexComboBox.Items[(int)id]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    DialogResult d = MessageBox.Show("Building #" + id + " couldn't be found in the Building List.\n" +
                        "Do you want to load Building 0 in its place?\n" +
                        "(Choosing \"Cancel\" will discard this building altogether.)", "Building not found", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                    if (d == DialogResult.Yes)
                    {
                        buildingsListBox.Items.Add(baseName + buildIndexComboBox.Items[0]);
                    }
                    else if (d == DialogResult.No)
                    {
                        buildingsListBox.Items.Add(baseName + "MISSING " + (int)id + '!');
                    } // else do nothing
                }
            }

        }
        private void MW_LoadModelTextures(NSBMD model, string textureFolder, int fileID)
        {
            if (fileID < 0)
            {
                return;
            }
            string texturePath = textureFolder + "\\" + fileID.ToString("D4");
            model.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out model.Textures, out model.Palettes);
            try
            {
                model.MatchTextures();
            }
            catch { }
        }
        
        private void ScaleTranslateRotateBuilding(Building building)
        {
            float fullXcoord = building.xPosition + building.xFraction / 65536f;
            float fullYcoord = building.yPosition + building.yFraction / 65536f;
            float fullZcoord = building.zPosition + building.zFraction / 65536f;

            float scaleFactor = building.NSBMDFile.models[0].modelScale / 1024;
            float translateFactor = 256 / building.NSBMDFile.models[0].modelScale;

            Gl.glScalef(scaleFactor * building.width, scaleFactor * building.height, scaleFactor * building.length);
            Gl.glTranslatef(fullXcoord * translateFactor / building.width, fullYcoord * translateFactor / building.height, fullZcoord * translateFactor / building.length);
            Gl.glRotatef(Building.U16ToDeg(building.xRotation), 1, 0, 0);
            Gl.glRotatef(Building.U16ToDeg(building.yRotation), 0, 1, 0);
            Gl.glRotatef(Building.U16ToDeg(building.zRotation), 0, 0, 1);
        }
        private void SetupRenderer(float ang, float dist, float elev, float perspective, int width, int height)
        {
            //TODO: improve this
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
            Gl.glViewport(0, 0, width, height);
            aspect = mapOpenGlControl.Width / mapOpenGlControl.Height;//(vp[2] - vp[0]) / (vp[3] - vp[1]);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(perspective, aspect, 0.2f, 500.0f);//0.02f, 32.0f);
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
        public void SetupMapEditor(MainProgram parent, bool force=false)
        {
            mapOpenGlControl.InitializeContexts();
            mapOpenGlControl.MakeCurrent();
            mapOpenGlControl.MouseWheel += new MouseEventHandler(mapOpenGlControl_MouseWheel);

            if (mapEditorIsReady && !force) { return; }
            mapEditorIsReady = true;
            this._parent = parent;

            if (selectMapComboBox.SelectedIndex > -1)
                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);



            /* Extract essential NARCs sub-archives*/
            _parent.toolStripProgressBar.Visible = true;
            _parent.toolStripProgressBar.Maximum = 9;
            _parent.toolStripProgressBar.Value = 0;
            Helpers.statusLabelMessage("Attempting to unpack Map Editor NARCs... Please wait.");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.maps,
                DirNames.exteriorBuildingModels,
                DirNames.buildingConfigFiles,
                DirNames.buildingTextures,
                DirNames.mapTextures,
                DirNames.areaData,
            });

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
            }

            Helpers.DisableHandlers();

            collisionPainterPictureBox.Image = new Bitmap(100, 100);
            typePainterPictureBox.Image = new Bitmap(100, 100);
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    mapPartsTabControl.TabPages.Remove(bgsTabPage);
                    break;
                default:
                    interiorbldRadioButton.Enabled = true;
                    exteriorbldRadioButton.Enabled = true;
                    break;
            }
            ;


            /* Add map names to box */
            selectMapComboBox.Items.Clear();
            int mapCount = _parent.romInfo.GetMapCount();

            for (int i = 0; i < mapCount; i++)
            {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(RomInfo.gameDirs[DirNames.maps].unpackedDir + "\\" + i.ToString("D4")))
                {
                    switch (RomInfo.gameFamily)
                    {
                        case GameFamilies.DP:
                        case GameFamilies.Plat:
                            reader.BaseStream.Position = 0x10 + reader.ReadUInt32() + reader.ReadUInt32();
                            break;
                        default:
                            reader.BaseStream.Position = 0x12;
                            short bgsSize = reader.ReadInt16();
                            long backupPos = reader.BaseStream.Position;

                            reader.BaseStream.Position = 0;
                            reader.BaseStream.Position = backupPos + bgsSize + reader.ReadUInt32() + reader.ReadUInt32();
                            break;
                    }
                    ;

                    reader.BaseStream.Position += 0x14;
                    selectMapComboBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + NSBUtils.ReadNSBMDname(reader));
                }

            }
            _parent.toolStripProgressBar.Value++;

            /* Fill building models list */
            updateBuildingListComboBox(false);

            /*  Fill map textures list */
            mapTextureComboBox.Items.Clear();
            mapTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < _parent.romInfo.GetMapTexturesCount(); i++)
            {
                mapTextureComboBox.Items.Add("Map Texture Pack [" + i.ToString("D2") + "]");
            }
            _parent.toolStripProgressBar.Value++;

            /*  Fill building textures list */
            buildTextureComboBox.Items.Clear();
            buildTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < _parent.romInfo.GetBuildingTexturesCount(); i++)
            {
                buildTextureComboBox.Items.Add("Building Texture Pack [" + i.ToString("D2") + "]");
            }

            _parent.toolStripProgressBar.Value++;

            collisionPainterComboBox.Items.Clear();
            foreach (string s in PokeDatabase.System.MapCollisionPainters.Values)
            {
                collisionPainterComboBox.Items.Add(s);
            }

            collisionTypePainterComboBox.Items.Clear();
            foreach (string s in PokeDatabase.System.MapCollisionTypePainters.Values)
            {
                collisionTypePainterComboBox.Items.Add(s);
            }

            _parent.toolStripProgressBar.Value++;

            /* Set controls' initial values */
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            collisionTypePainterComboBox.SelectedIndex = 0;
            collisionPainterComboBox.SelectedIndex = 1;

            _parent.toolStripProgressBar.Value = 0;
            _parent.toolStripProgressBar.Visible = false;
            Helpers.EnableHandlers();

            //Default selections
            selectMapComboBox.SelectedIndex = 0;
            exteriorbldRadioButton.Checked = true;
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    mapTextureComboBox.SelectedIndex = 7;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                case GameFamilies.HGSS:
                    mapTextureComboBox.SelectedIndex = 3;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                default:
                    mapTextureComboBox.SelectedIndex = 2;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
            }
            ;

            Helpers.statusLabelMessage();
        }
        private void addMapFileButton_Click(object sender, EventArgs e)
        {
            /* Add new map file to map folder */
            new MapFile(0, RomInfo.gameFamily, discardMoveperms: true).SaveToFileDefaultDir(selectMapComboBox.Items.Count);

            /* Update ComboBox and select new file */
            selectMapComboBox.Items.Add(selectMapComboBox.Items.Count.ToString("D3") + MapHeader.nameSeparator + "newmap");
            selectMapComboBox.SelectedIndex = selectMapComboBox.Items.Count - 1;
        }
        private void replaceMapBinButton_Click(object sender, EventArgs e)
        {
            /* Prompt user to select .bin file */
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = "Map BIN File (*.bin)|*.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            MapFile temp = new MapFile(of.FileName, RomInfo.gameFamily, false);

            if (temp.correctnessFlag)
            {
                UpdateMapBinAndRefresh(temp, "Map BIN imported successfully!");
                return;
            }
            else
            {
                if (RomInfo.gameFamily == GameFamilies.HGSS)
                {
                    //If HGSS didn't work try reading as Platinum Map
                    temp = new MapFile(of.FileName, GameFamilies.Plat, false);
                }
                else
                {
                    //If Plat didn't work try reading as HGSS Map
                    temp = new MapFile(of.FileName, GameFamilies.HGSS, false);
                }

                if (temp.correctnessFlag)
                {
                    UpdateMapBinAndRefresh(temp, "Map BIN imported and adapted successfully!");
                    return;
                }
            }

            MessageBox.Show("The BIN file you imported is corrupted!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateMapBinAndRefresh(MapFile newerVersion, string message)
        {
            currentMapFile = newerVersion;

            /* Update map BIN file */
            currentMapFile.SaveToFileDefaultDir(selectMapComboBox.SelectedIndex, showSuccessMessage: false);

            /* Refresh controls */
            selectMapComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buildTextureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int btIndex = buildTextureComboBox.SelectedIndex;

            if (Helpers.HandlersDisabled || btIndex < 0)
            {
                return;
            }

            if (btIndex == 0)
            {
                bldTexturesOn = false;
            }
            else
            {
                string texturePath = RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + (btIndex - 1).ToString("D4");
                byte[] textureFile = File.ReadAllBytes(texturePath);

                Stream str = new MemoryStream(textureFile);
                foreach (Building building in currentMapFile.buildings)
                {
                    str.Position = 0;
                    NSBMD file = building.NSBMDFile;

                    if (file != null)
                    {
                        file.materials = NSBTXLoader.LoadNsbtx(str, out file.Textures, out file.Palettes);

                        try
                        {
                            file.MatchTextures();
                            bldTexturesOn = true;
                        }
                        catch
                        {
                            string itemAtIndex = buildTextureComboBox.Items[btIndex].ToString();
                            if (!itemAtIndex.StartsWith("Error!"))
                            {
                                Helpers.DisableHandlers();
                                buildTextureComboBox.Items[btIndex] = itemAtIndex.Insert(0, "Error! - ");
                                Helpers.EnableHandlers();
                            }
                            bldTexturesOn = false;
                        }
                    }
                }
                //buildTextureComboBox.Items[buildTextureComboBox.SelectedIndex] = "Error - Building Texture Pack too small [" + (buildTextureComboBox.SelectedIndex - 1).ToString("D2") + "]";
            }

            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapTextureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            if (mapTextureComboBox.SelectedIndex == 0)
                mapTexturesOn = false;
            else
            {
                mapTexturesOn = true;

                string texturePath = RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
                currentMapFile.mapModel.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out currentMapFile.mapModel.Textures, out currentMapFile.mapModel.Palettes);
                try
                {
                    currentMapFile.mapModel.MatchTextures();
                }
                catch { }
            }
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        public void mapOpenGlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (mapPartsTabControl.SelectedTab == buildingsTabPage && bldPlaceWithMouseCheckbox.Checked)
            {
                return;
            }
            dist -= (float)e.Delta / 200;
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapOpenGlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            byte multiplier = 2;
            if (e.Modifiers == Keys.Shift)
            {
                multiplier = 1;
            }
            else if (e.Modifiers == Keys.Control)
            {
                multiplier = 4;
            }

            switch (e.KeyCode)
            {
                case Keys.Right:
                    rRot = true;
                    lRot = false;
                    break;
                case Keys.Left:
                    rRot = false;
                    lRot = true;
                    break;
                case Keys.Up:
                    dRot = false;
                    uRot = true;
                    break;
                case Keys.Down:
                    dRot = true;
                    uRot = false;
                    break;
            }

            if (rRot ^ lRot)
            {
                if (rRot)
                {
                    ang += 1 * multiplier;
                }
                else if (lRot)
                {
                    ang -= 1 * multiplier;
                }
            }

            if (uRot ^ dRot)
            {
                if (uRot)
                {
                    elev -= 1 * multiplier;
                }
                else if (dRot)
                {
                    elev += 1 * multiplier;
                }
            }

            mapOpenGlControl.Invalidate();
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapOpenGlControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    rRot = false;
                    break;
                case Keys.Left:
                    lRot = false;
                    break;
                case Keys.Up:
                    uRot = false;
                    break;
                case Keys.Down:
                    dRot = false;
                    break;
            }
        }
        private void mapOpenGlControl_Click(object sender, EventArgs e)
        {
            if (radio2D.Checked && bldPlaceWithMouseCheckbox.Checked)
            {
                PointF coordinates = mapRenderPanel.PointToClient(Cursor.Position);
                PointF mouseTilePos = new PointF(coordinates.X / mapEditorSquareSize, coordinates.Y / mapEditorSquareSize);

                if (buildingsListBox.SelectedIndex > -1)
                {
                    if (!bldPlaceLockXcheckbox.Checked)
                        xBuildUpDown.Value = (decimal)(Math.Round(mouseTilePos.X, bldDecimalPositions) - 16);
                    if (!bldPlaceLockZcheckbox.Checked)
                        zBuildUpDown.Value = (decimal)(Math.Round(mouseTilePos.Y, bldDecimalPositions) - 16);
                }
            }
        }
        private void bldRoundWhole_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 0;
        }
        private void bldRoundDec_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 1;
        }
        private void bldRoundCent_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 2;
        }
        private void bldRoundMil_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 3;
        }
        private void bldRoundDecmil_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 4;
        }
        private void bldRoundCentMil_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 5;
        }
        private void bldPlaceWithMouseCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            bool status = bldPlaceWithMouseCheckbox.Checked && radio2D.Checked;
            bldPlaceLockXcheckbox.Enabled = status;
            bldPlaceLockZcheckbox.Enabled = status;
            bldRoundGroupbox.Enabled = status;
            lockXZgroupbox.Enabled = status;

            if (status)
            {
                SetCam2D();
            }
        }
        private void bldPlaceLockXcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ExclusiveCBInvert(bldPlaceLockZcheckbox);
        }

        private void bldPlaceLockZcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ExclusiveCBInvert(bldPlaceLockXcheckbox);
        }
        private void mapPartsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mapPartsTabControl.SelectedTab == buildingsTabPage)
            {
                radio2D.Checked = false;

                hideBuildings = false;
                radio3D.Enabled = true;
                radio2D.Enabled = true;
                wireframeCheckBox.Enabled = true;

                mapOpenGlControl.BringToFront();

                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
            else if (mapPartsTabControl.SelectedTab == permissionsTabPage)
            {
                radio2D.Checked = true;

                hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                SetCam2D();
                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

                movPictureBox.BackgroundImage = GrabMapScreenshot(movPictureBox.Width, movPictureBox.Height);
                movPictureBox.BringToFront();
            }
            else if (mapPartsTabControl.SelectedTab == modelTabPage)
            {
                radio2D.Checked = false;

                hideBuildings = true;
                radio3D.Enabled = true;
                radio2D.Enabled = true;
                wireframeCheckBox.Enabled = true;

                mapOpenGlControl.BringToFront();

                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
            else
            { // Terrain and BGS
                radio2D.Checked = true;

                hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                mapOpenGlControl.BringToFront();

                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }
        private void radio2D_CheckedChanged(object sender, EventArgs e)
        {
            bool _2dmodeSelected = radio2D.Checked;

            if (_2dmodeSelected)
            {
                SetCam2D();
            }
            else
            {
                SetCam3D();
            }

            bldPlaceWithMouseCheckbox.Enabled = _2dmodeSelected;
            radio3D.Checked = !_2dmodeSelected;

            bldPlaceWithMouseCheckbox_CheckedChanged(null, null);
        }
        private void SetCam2D()
        {
            perspective = 4f;
            ang = 0f;
            dist = 115.2f;
            elev = 90f;

            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void SetCam3D()
        {
            perspective = 45f;
            ang = 0f;
            dist = 12.8f;
            elev = 50.0f;

            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapScreenshotButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Choose where to save the map screenshot.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog imageSFD = new SaveFileDialog
            {
                Filter = "PNG File(*.png)|*.png",
            };
            if (imageSFD.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
            ang, dist, elev, perspective,
            mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            int newW = 512, newH = 512;
            Bitmap newImage = new Bitmap(newW, newH);
            using (var graphCtr = Graphics.FromImage(newImage))
            {
                graphCtr.SmoothingMode = SmoothingMode.HighQuality;
                graphCtr.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphCtr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphCtr.DrawImage(GrabMapScreenshot(mapOpenGlControl.Width, mapOpenGlControl.Height), 0, 0, newW, newH);
            }
            newImage.Save(imageSFD.FileName);
            MessageBox.Show("Screenshot saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void removeLastMapFileButton_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Map BIN File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes))
            {
                /* Delete last map file */
                File.Delete(RomInfo.gameDirs[DirNames.maps].unpackedDir + "\\" + (selectMapComboBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectMapComboBox.Items.Count - 1;
                if (selectMapComboBox.SelectedIndex == lastIndex)
                    selectMapComboBox.SelectedIndex--;

                /* Remove item from ComboBox */
                selectMapComboBox.Items.RemoveAt(lastIndex);
            }
        }
        private void saveMapButton_Click(object sender, EventArgs e)
        {
            currentMapFile.SaveToFileDefaultDir(selectMapComboBox.SelectedIndex);
        }
        private void exportCurrentMapBinButton_Click(object sender, EventArgs e)
        {
            currentMapFile.SaveToFileExplorePath(selectMapComboBox.SelectedItem.ToString());
        }
        public void selectMapComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            /* Load map data into MapFile class instance */
            currentMapFile = new MapFile(selectMapComboBox.SelectedIndex, RomInfo.gameFamily);

            /* Load map textures for renderer */
            if (mapTextureComboBox.SelectedIndex > 0)
            {
                MW_LoadModelTextures(currentMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, mapTextureComboBox.SelectedIndex - 1);
            }

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++)
            {
                currentMapFile.buildings[i].LoadModelData(_parent.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                if (buildTextureComboBox.SelectedIndex > 0)
                {
                    MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
                }
            }

            /* Render the map */
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            /* Draw permissions in the small selection boxes */
            DrawSmallCollision();
            DrawSmallTypeCollision();

            /* Draw selected permissions category */
            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                DrawCollisionGrid();
            }
            else
            {
                DrawTypeGrid();
            }
            /* Set map screenshot as background picture in permissions editor PictureBox */
            movPictureBox.BackgroundImage = GrabMapScreenshot(movPictureBox.Width, movPictureBox.Height);

            RestorePainter();

            /* Fill buildings ListBox, and if not empty select first item */
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0)
            {
                buildingsListBox.SelectedIndex = 0;
            }

            modelSizeLBL.Text = currentMapFile.mapModelData.Length.ToString() + " B";
            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            }
        }
        private void wireframeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (wireframeCheckBox.Checked)
            {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
            }
            else
            {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            }

            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void transparencyBar_Scroll(object sender, EventArgs e)
        {
            Transparency = transparencyBar.Value;
            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                DrawCollisionGrid();
            }
            else
            {
                DrawTypeGrid();
            }

        }

        #region Building Editor
        private void addBuildingButton_Click(object sender, EventArgs e)
        {
            AddBuildingToMap(new Building());
        }

        private void duplicateBuildingButton_Click(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex > -1)
            {
                AddBuildingToMap(new Building(currentMapFile.buildings[buildingsListBox.SelectedIndex]));
            }
        }

        private void AddBuildingToMap(Building b)
        {
            currentMapFile.buildings.Add(b);

            /* Load new building's model and textures for the renderer */
            b.LoadModelData(_parent.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked));
            MW_LoadModelTextures(b.NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1);
            currentMapFile.buildings[currentMapFile.buildings.Count - 1] = b;

            /* Add new entry to buildings ListBox */
            buildingsListBox.Items.Add((buildingsListBox.Items.Count + 1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.Items[(int)b.modelID]);
            buildingsListBox.SelectedIndex = buildingsListBox.Items.Count - 1;

            /* Redraw scene with new building */
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void buildIndexComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || buildingsListBox.SelectedIndex < 0)
            {
                return;
            }

            Helpers.DisableHandlers();
            buildingsListBox.Items[buildingsListBox.SelectedIndex] = (buildingsListBox.SelectedIndex + 1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.SelectedItem;
            Helpers.EnableHandlers();

            currentMapFile.buildings[buildingsListBox.SelectedIndex].modelID = (uint)buildIndexComboBox.SelectedIndex;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].LoadModelData(_parent.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked));
            MW_LoadModelTextures(currentMapFile.buildings[buildingsListBox.SelectedIndex].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1);

            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void buildingsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int buildingNumber = buildingsListBox.SelectedIndex;
            if (Helpers.HandlersDisabled || buildingNumber < 0)
            {
                return;
            }
            Helpers.BackUpDisableHandler();
            Helpers.DisableHandlers();

            Building selected = currentMapFile.buildings[buildingNumber];
            if (selected.NSBMDFile != null)
            {
                buildIndexComboBox.SelectedIndex = (int)selected.modelID;

                xBuildUpDown.Value = selected.xPosition + (decimal)selected.xFraction / 65535;
                yBuildUpDown.Value = selected.yPosition + (decimal)selected.yFraction / 65535;
                zBuildUpDown.Value = selected.zPosition + (decimal)selected.zFraction / 65535;

                xRotBuildUpDown.Value = selected.xRotation;
                yRotBuildUpDown.Value = selected.yRotation;
                zRotBuildUpDown.Value = selected.zRotation;

                xRotDegBldUpDown.Value = (decimal)Building.U16ToDeg((ushort)xRotBuildUpDown.Value);
                yRotDegBldUpDown.Value = (decimal)Building.U16ToDeg((ushort)yRotBuildUpDown.Value);
                zRotDegBldUpDown.Value = (decimal)Building.U16ToDeg((ushort)zRotBuildUpDown.Value);

                buildingWidthUpDown.Value = selected.width;
                buildingHeightUpDown.Value = selected.height;
                buildingLengthUpDown.Value = selected.length;
            }

            Helpers.RestoreDisableHandler();
        }

        private void xRotBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            int selection = buildingsListBox.SelectedIndex;

            if (selection <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();
            currentMapFile.buildings[selection].xRotation = (ushort)((int)xRotBuildUpDown.Value & ushort.MaxValue);
            xRotDegBldUpDown.Value = (decimal)Building.U16ToDeg(currentMapFile.buildings[selection].xRotation);
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void yRotBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            int selection = buildingsListBox.SelectedIndex;

            if (selection <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            yRotDegBldUpDown.Value = (decimal)Building.U16ToDeg(currentMapFile.buildings[selection].yRotation = (ushort)((int)yRotBuildUpDown.Value & ushort.MaxValue));
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void zRotBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            int selection = buildingsListBox.SelectedIndex;

            if (selection <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            zRotDegBldUpDown.Value = (decimal)Building.U16ToDeg(currentMapFile.buildings[selection].zRotation = (ushort)((int)zRotBuildUpDown.Value & ushort.MaxValue));
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void xRotDegBldUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xRotation = (ushort)(xRotBuildUpDown.Value =
                Building.DegToU16((float)xRotDegBldUpDown.Value));
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void yRotDegBldUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yRotation = (ushort)(yRotBuildUpDown.Value =
                Building.DegToU16((float)yRotDegBldUpDown.Value));
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void zRotDegBldUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zRotation = (ushort)(zRotBuildUpDown.Value =
                Building.DegToU16((float)zRotDegBldUpDown.Value));
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void buildingHeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex > -1)
            {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].height = (uint)buildingHeightUpDown.Value;
                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }

        private void buildingLengthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex > -1)
            {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].length = (uint)buildingLengthUpDown.Value;
                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }

        private void buildingWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex > -1)
            {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].width = (uint)buildingWidthUpDown.Value;
                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }

        private void exportBuildingsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = MapFile.BuildingsFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.BuildingsToByteArray());

            MessageBox.Show("Buildings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void importBuildingsButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ib = new OpenFileDialog
            {
                Filter = MapFile.BuildingsFilter
            };
            if (ib.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.ImportBuildings(File.ReadAllBytes(ib.FileName));
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0) { buildingsListBox.SelectedIndex = 0; }

            for (int i = 0; i < currentMapFile.buildings.Count; i++)
            {
                currentMapFile.buildings[i].LoadModelData(_parent.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            MessageBox.Show("Buildings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void interiorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.DisableHandlers();
            int index = buildIndexComboBox.SelectedIndex;
            buildIndexComboBox.Items.Clear();

            /* Fill building models list */
            updateBuildingListComboBox(interiorbldRadioButton.Checked);
            FillBuildingsBox();

            try
            {
                buildIndexComboBox.SelectedIndex = index;
            }
            catch (ArgumentOutOfRangeException)
            {
                buildIndexComboBox.SelectedIndex = 0;
                currentMapFile.buildings[buildIndexComboBox.SelectedIndex].modelID = 0;
            }

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++)
            {
                currentMapFile.buildings[i].LoadModelData(_parent.romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            /* Render the map */
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            Helpers.EnableHandlers();
        }

        private void removeBuildingButton_Click(object sender, EventArgs e)
        {
            int toRemoveListBoxID = buildingsListBox.SelectedIndex;
            if (toRemoveListBoxID > -1)
            {
                Helpers.DisableHandlers();

                /* Remove building object from list and the corresponding entry in the ListBox */

                currentMapFile.buildings.RemoveAt(toRemoveListBoxID);
                buildingsListBox.Items.RemoveAt(toRemoveListBoxID);

                FillBuildingsBox(); // Update ListBox
                Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

                Helpers.EnableHandlers();

                if (buildingsListBox.Items.Count > 0)
                {
                    if (toRemoveListBoxID > 0)
                    {
                        buildingsListBox.SelectedIndex = toRemoveListBoxID - 1;
                    }
                    else
                    {
                        buildingsListBox.SelectedIndex = 0;
                    }
                }
            }
        }

        private void xBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || buildingsListBox.SelectedIndex < 0)
            {
                return;
            }

            var wholePart = Math.Truncate(xBuildUpDown.Value);
            var decPart = xBuildUpDown.Value - wholePart;

            if (decPart < 0)
            {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].xFraction = (ushort)(decPart * 65535);
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void zBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(zBuildUpDown.Value);
            var decPart = zBuildUpDown.Value - wholePart;

            if (decPart < 0)
            {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].zFraction = (ushort)(decPart * 65535);
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void yBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(yBuildUpDown.Value);
            var decPart = yBuildUpDown.Value - wholePart;

            if (decPart < 0)
            {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].yFraction = (ushort)(decPart * 65535);
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        #endregion

        #region Movement Permissions Editor

        #region Subroutines
        private Bitmap GrabMapScreenshot(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Gl.glReadPixels(0, 0, width, height, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }
        private void DrawCollisionGrid()
        {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm))
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        PrepareCollisionPainterGraphics(currentMapFile.collisions[i, j]);

                        /* Draw collision on the main grid */
                        mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                        gMain.DrawRectangle(paintPen, mainCell);
                        gMain.FillRectangle(paintBrush, mainCell);
                    }
                }
            }
            movPictureBox.Image = mainBm;
            movPictureBox.Invalidate();
        }
        private void DrawSmallCollision()
        {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm))
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        PrepareCollisionPainterGraphics(currentMapFile.collisions[i, j]);

                        /* Draw collision on the small image */
                        smallCell = new Rectangle(3 * j, 3 * i, 3, 3);
                        gSmall.DrawRectangle(paintPen, smallCell);
                        gSmall.FillRectangle(paintBrush, smallCell);
                    }
                }
            }
            collisionPictureBox.Image = smallBm;
            collisionPictureBox.Invalidate();
        }
        private void DrawTypeGrid()
        {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm))
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        PrepareTypePainterGraphics(currentMapFile.types[i, j]);

                        /* Draw cell with color */
                        mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                        gMain.DrawRectangle(paintPen, mainCell);
                        gMain.FillRectangle(paintBrush, mainCell);

                        /* Draw byte on cell */
                        StringFormat sf = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
                        gMain.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        gMain.DrawString(currentMapFile.types[i, j].ToString("X2"), textFont, textBrush, mainCell, sf);
                    }
                }
            }
            movPictureBox.Image = mainBm;
            movPictureBox.Invalidate();
        }
        private void DrawSmallTypeCollision()
        {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm))
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        PrepareTypePainterGraphics(currentMapFile.types[i, j]);

                        /* Draw collision on the small image */
                        smallCell = new Rectangle(3 * j, 3 * i, 3, 3);
                        gSmall.DrawRectangle(paintPen, smallCell);
                        gSmall.FillRectangle(paintBrush, smallCell);
                    }
                }
            }
            typePictureBox.Image = smallBm;
            typePictureBox.Invalidate();
        }
        private void scanUsedCollisionTypesButton_Click(object sender, EventArgs e)
        {
            SortedSet<byte> allUsed = FindUsedCollisions();

            List<byte> lst = allUsed.ToList();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < allUsed.Count; i++)
            {
                sb.Append("0x");
                sb.Append(lst[i].ToString("X2"));

                if (i != allUsed.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            string report = sb.ToString();

            MessageBox.Show($"This report has been copied to the clipboard as well, for your convenience.\n\nUsed types (in all Map BINs): \n{report}", "Used collision types report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Clipboard.SetText(report);
        }

        private SortedSet<byte> FindUsedCollisions()
        {
            int mapCount = _parent.romInfo.GetMapCount();

            SortedSet<byte> allUsedTypes = new SortedSet<byte>();

            for (int i = 0; i < mapCount; i++)
            {
                allUsedTypes.UnionWith(new MapFile(i, gameFamily, false, false).GetUsedTypes());
            }

            return allUsedTypes;
        }
        private SortedSet<byte> FindUnusedCollisions()
        {
            int mapCount = _parent.romInfo.GetMapCount();

            SortedSet<byte> allUnusedTypes = new SortedSet<byte>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                allUnusedTypes.Add((byte)i);
            }
            allUnusedTypes.ExceptWith(FindUsedCollisions());

            return allUnusedTypes;
        }
        private void EditCell(int xPosition, int yPosition)
        {
            try
            {
                mainCell = new Rectangle(xPosition * mapEditorSquareSize, yPosition * mapEditorSquareSize, mapEditorSquareSize, mapEditorSquareSize);
                smallCell = new Rectangle(xPosition * 3, yPosition * 3, 3, 3);

                using (Graphics mainG = Graphics.FromImage(movPictureBox.Image))
                {
                    /*  Draw new cell on main grid */
                    mainG.SetClip(mainCell);
                    mainG.Clear(Color.Transparent);
                    mainG.DrawRectangle(paintPen, mainCell);
                    mainG.FillRectangle(paintBrush, mainCell);
                    if (selectTypePanel.BackColor == Color.MidnightBlue)
                    {
                        sf = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
                        mainG.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        mainG.DrawString(paintByte.ToString("X2"), textFont, textBrush, mainCell, sf);
                    }
                }

                if (selectCollisionPanel.BackColor == Color.MidnightBlue)
                {
                    using (Graphics smallG = Graphics.FromImage(collisionPictureBox.Image))
                    {
                        /* Draw new cell on small grid */
                        smallG.SetClip(smallCell);
                        smallG.Clear(Color.Transparent);
                        smallG.DrawRectangle(paintPen, smallCell);
                        smallG.FillRectangle(paintBrush, smallCell);
                    }
                    currentMapFile.collisions[yPosition, xPosition] = paintByte;
                    collisionPictureBox.Invalidate();
                }
                else
                {
                    using (Graphics smallG = Graphics.FromImage(typePictureBox.Image))
                    {
                        /* Draw new cell on small grid */
                        smallG.SetClip(smallCell);
                        smallG.Clear(Color.Transparent);
                        smallG.DrawRectangle(paintPen, smallCell);
                        smallG.FillRectangle(paintBrush, smallCell);
                    }
                    currentMapFile.types[yPosition, xPosition] = paintByte;
                    typePictureBox.Invalidate();
                }
                movPictureBox.Invalidate();
            }
            catch { return; }
        }
        private void FloodFillUtil(byte[,] screen, int x, int y, byte prevC, byte newC, int sizeX, int sizeY)
        {
            // Base cases 
            if (x < 0 || x >= sizeX || y < 0 || y >= sizeY)
            {
                return;
            }

            if (screen[y, x] != prevC)
            {
                return;
            }

            // Replace the color at (x, y) 
            screen[y, x] = newC;

            // Recur for north, east, south and west 
            FloodFillUtil(screen, x + 1, y, prevC, newC, sizeX, sizeY);
            FloodFillUtil(screen, x - 1, y, prevC, newC, sizeX, sizeY);
            FloodFillUtil(screen, x, y + 1, prevC, newC, sizeX, sizeY);
            FloodFillUtil(screen, x, y - 1, prevC, newC, sizeX, sizeY);
        }
        private void FloodFillCell(int x, int y)
        {
            byte toPaint = paintByte;
            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                if (currentMapFile.collisions[y, x] != paintByte)
                {
                    FloodFillUtil(currentMapFile.collisions, x, y, currentMapFile.collisions[y, x], paintByte, 32, 32);
                    DrawCollisionGrid();
                    DrawSmallCollision();
                    PrepareCollisionPainterGraphics(paintByte);
                }
            }
            else
            {
                if (currentMapFile.types[y, x] != paintByte)
                {
                    FloodFillUtil(currentMapFile.types, x, y, currentMapFile.types[y, x], paintByte, 32, 32);
                    DrawTypeGrid();
                    DrawSmallTypeCollision();
                    PrepareTypePainterGraphics(paintByte);
                }
            }

            /* Draw permissions in the small selection boxes */


        }
        private void RestorePainter()
        {
            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                collisionPainterComboBox_SelectedIndexChange(null, null);
            }
            else if (collisionTypePainterComboBox.Enabled)
            {
                typePainterComboBox_SelectedIndexChanged(null, null);
            }
            else
            {
                typePainterUpDown_ValueChanged(null, null);
            }
        }
        private void PrepareCollisionPainterGraphics(byte collisionValue)
        {
            switch (collisionValue)
            {
                case 0x01: // Snow
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Lavender));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Lavender));
                    break;
                case 0x02: // Leaves
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.ForestGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.ForestGreen));
                    break;
                case 0x04: // Grass
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.LimeGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.LimeGreen));
                    break;
                case 0x06: // Stairs and ice
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.PowderBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.PowderBlue));
                    break;
                case 0x07: // Metal
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Silver));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Silver));
                    break;
                case 0x0A: // Stone
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.DimGray));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.DimGray));
                    break;
                case 0x0D: // Wood
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.SaddleBrown));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.SaddleBrown));
                    break;
                case 0x80:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Red));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Red));
                    break;
                default: // 0x00 - Walkeable               
                    paintPen = new Pen(Color.FromArgb(32, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(32, Color.White));
                    break;
            }
        }

        private void PrepareTypePainterGraphics(byte typeValue)
        {
            switch (typeValue)
            {
                case 0x0:
                    paintPen = new Pen(Color.FromArgb(32, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(32, Color.White));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x2:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.LimeGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.LimeGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x3:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Green));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Green));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x6:
                    paintPen = new Pen(Color.FromArgb(128, Color.YellowGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.YellowGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x7:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x8:
                    paintPen = new Pen(Color.FromArgb(128, Color.BurlyWood));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.BurlyWood));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x9:
                    paintPen = new Pen(Color.FromArgb(128, Color.SlateBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.SlateBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0xA:
                    paintPen = new Pen(Color.FromArgb(128, Color.Tomato));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Tomato));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0xC:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.BurlyWood));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.BurlyWood));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x10:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.SkyBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.SkyBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x13:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.SteelBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.SteelBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x15:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.RoyalBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.RoyalBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x16:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.LightSlateGray));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.LightSlateGray));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x20:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Cyan));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Cyan));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x21:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.PeachPuff));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.PeachPuff));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Red));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Red));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x3C:
                case 0x3D:
                case 0x3E:
                    paintPen = new Pen(Color.FromArgb(0x7F654321));
                    paintBrush = new SolidBrush(Color.FromArgb(0x7F654321));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x38:
                case 0x39:
                case 0x3A:
                case 0x3B:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Maroon));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Maroon));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x40:
                case 0x41:
                case 0x42:
                case 0x43:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Gold));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Gold));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x4B:
                case 0x4C:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Sienna));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Sienna));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x5E:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x5F:
                case 0x62:
                case 0x63:
                case 0x64:
                case 0x65:
                case 0x69:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x6C:
                case 0x6D:
                case 0x6E:
                case 0x6F:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA1:
                case 0xA2:
                case 0xA3:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Honeydew));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Honeydew));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA4:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Peru));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Peru));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA6:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.SeaGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.SeaGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                default:
                    paintPen = new Pen(Color.FromArgb(32, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(32, Color.White));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 8.65f);
                    break;
            }
        }
        #endregion

        private void clearCurrentButton_Click(object sender, EventArgs e)
        {
            PictureBox smallBox = selectCollisionPanel.BackColor == Color.MidnightBlue ? collisionPictureBox : typePictureBox;

            using (Graphics smallG = Graphics.FromImage(smallBox.Image))
            {
                using (Graphics mainG = Graphics.FromImage(movPictureBox.Image))
                {
                    smallG.Clear(Color.Transparent);
                    mainG.Clear(Color.Transparent);
                    PrepareCollisionPainterGraphics(0x0);

                    for (int i = 0; i < 32; i++)
                    {
                        for (int j = 0; j < 32; j++)
                        {
                            mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                            mainG.DrawRectangle(paintPen, mainCell);
                            mainG.FillRectangle(paintBrush, mainCell);
                        }
                    }
                }
            }

            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                currentMapFile.collisions = new byte[32, 32]; // Set all collision bytes to clear (0x0)               
            }
            else
            {
                currentMapFile.types = new byte[32, 32]; // Set all type bytes to clear (0x0)
            }

            movPictureBox.Invalidate(); // Refresh main image
            smallBox.Invalidate();
            RestorePainter();
        }

        public string[] GetBuildingsList(bool interior)
        {
            List<string> names = new List<string>();
            string path = _parent.romInfo.GetBuildingModelsDirPath(interior);
            int buildModelsCount = Directory.GetFiles(path).Length;

            for (int i = 0; i < buildModelsCount; i++)
            {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(path + "\\" + i.ToString("D4"), 0x38))
                {
                    string nsbmdName = Encoding.UTF8.GetString(reader.ReadBytes(16)).TrimEnd();
                    names.Add(nsbmdName);
                }
            }
            return names.ToArray();
        }


        public void updateBuildingListComboBox(bool interior)
        {
            string[] bldList = GetBuildingsList(interior);

            buildIndexComboBox.Items.Clear();
            for (int i = 0; i < bldList.Length; i++)
            {
                buildIndexComboBox.Items.Add("[" + i + "] " + bldList[i]);
            }
            _parent.toolStripProgressBar.Value++;
        }


        private void locateCurrentMapBin_Click(object sender, EventArgs e)
        {
            Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.maps].unpackedDir, selectMapComboBox.SelectedIndex.ToString("D4")));
        }

        private void collisionPictureBox_Click(object sender, EventArgs e)
        {
            selectTypePanel.BackColor = Color.Transparent;
            typeGroupBox.Enabled = false;
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            collisionGroupBox.Enabled = true;

            DrawCollisionGrid();
            RestorePainter();
        }
        private void exportMovButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog em = new SaveFileDialog
            {
                Filter = MapFile.MovepermsFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (em.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(em.FileName, currentMapFile.CollisionsToByteArray());

            MessageBox.Show("Permissions exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importMovButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ip = new OpenFileDialog
            {
                Filter = MapFile.MovepermsFilter
            };
            if (ip.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.ImportPermissions(File.ReadAllBytes(ip.FileName));

            DrawSmallCollision();
            DrawSmallTypeCollision();

            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                DrawCollisionGrid();
            }
            else
            {
                DrawTypeGrid();
            }
            RestorePainter();

            MessageBox.Show("Permissions imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void movPictureBox_Click(object sender, EventArgs e)
        {
            MouseEventArgs mea = (MouseEventArgs)e;

            int xCoord = movPictureBox.PointToClient(MousePosition).X / mapEditorSquareSize;
            int yCoord = movPictureBox.PointToClient(MousePosition).Y / mapEditorSquareSize;

            if (mea.Button == MouseButtons.Middle)
            {
                FloodFillCell(xCoord, yCoord);
            }
            else if (mea.Button == MouseButtons.Left)
            {
                EditCell(xCoord, yCoord);
            }
            else
            {
                if (selectCollisionPanel.BackColor == Color.MidnightBlue)
                {
                    byte newValue = currentMapFile.collisions[yCoord, xCoord];
                    updateCollisions(newValue);
                }
                else
                {
                    byte newValue = currentMapFile.types[yCoord, xCoord];
                    typePainterUpDown.Value = newValue;
                    updateTypeCollisions(newValue);
                }
                ;
            }
        }
        private void movPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                EditCell(e.Location.X / mapEditorSquareSize, e.Location.Y / mapEditorSquareSize);
            }
        }
        private void collisionPainterComboBox_SelectedIndexChange(object sender, EventArgs e)
        {
            byte? collisionByte = StringToCollisionByte((string)collisionPainterComboBox.SelectedItem);

            if (collisionByte != null)
            {
                updateCollisions((byte)collisionByte);
            }
        }
        private void typePainterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte? collisionByte = StringToCollisionByte((string)collisionTypePainterComboBox.SelectedItem);

            if (collisionByte != null)
            {
                updateTypeCollisions((byte)collisionByte);
            }
        }

        private byte? StringToCollisionByte(string selectedItem)
        {
            byte? result;
            try
            {
                result = Convert.ToByte(selectedItem.Substring(1, 2), 16);
            }
            catch (FormatException)
            {
                AppLogger.Error("Format incompatible");
                result = null;
            }
            return result;
        }
        private void typePainterUpDown_ValueChanged(object sender, EventArgs e)
        {
            updateTypeCollisions((byte)typePainterUpDown.Value);
        }
        private void updateCollisions(byte typeValue)
        {
            PrepareCollisionPainterGraphics(typeValue);
            paintByte = (byte)typeValue;

            sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using (Graphics g = Graphics.FromImage(collisionPainterPictureBox.Image))
            {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }

            if (PokeDatabase.System.MapCollisionPainters.TryGetValue(typeValue, out string dictResult))
            {
                collisionPainterComboBox.SelectedItem = dictResult;
            }
            collisionPainterPictureBox.Invalidate();
        }
        private void updateTypeCollisions(byte typeValue)
        {
            PrepareTypePainterGraphics(typeValue);
            paintByte = typeValue;

            sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using (Graphics g = Graphics.FromImage(typePainterPictureBox.Image))
            {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }

            if (PokeDatabase.System.MapCollisionTypePainters.TryGetValue(typeValue, out string dictResult))
            {
                collisionTypePainterComboBox.SelectedItem = dictResult;
            }
            else
            {
                valueTypeRadioButton.Checked = true;
                typePainterUpDown.Value = typeValue;
            }
            typePainterPictureBox.Invalidate();
        }
        private void typePictureBox_Click(object sender, EventArgs e)
        {
            selectCollisionPanel.BackColor = Color.Transparent;
            collisionGroupBox.Enabled = false;
            selectTypePanel.BackColor = Color.MidnightBlue;
            typeGroupBox.Enabled = true;

            DrawTypeGrid();
            RestorePainter();
        }
        private void typesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (knownTypesRadioButton.Checked)
            {
                typePainterUpDown.Enabled = false;
                collisionTypePainterComboBox.Enabled = true;
                typePainterComboBox_SelectedIndexChanged(null, null);
            }
        }
        private void valueTypeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (valueTypeRadioButton.Checked)
            {
                collisionTypePainterComboBox.Enabled = false;
                typePainterUpDown.Enabled = true;
                typePainterUpDown_ValueChanged(null, null);
            }
        }
        #endregion

        #region 3D Model Editor
        public const ushort MAPMODEL_CRITICALSIZE = 61000;

        private void importMapButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog im = new OpenFileDialog
            {
                Filter = MapFile.NSBMDFilter,
                InitialDirectory = SettingsManager.Settings.mapImportStarterPoint
            };
            if (im.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.LoadMapModel(DSUtils.ReadFromFile(im.FileName));

            if (mapTextureComboBox.SelectedIndex > 0)
            {
                MW_LoadModelTextures(currentMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, mapTextureComboBox.SelectedIndex - 1);
            }
            Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            modelSizeLBL.Text = currentMapFile.mapModelData.Length.ToString() + " B";

            string message;
            string title;
            if (currentMapFile.mapModelData.Length > MAPMODEL_CRITICALSIZE)
            {
                message = "You imported a map model that exceeds " + MAPMODEL_CRITICALSIZE + " bytes." + Environment.NewLine
                    + "This may lead to unexpected behavior in game.";
                title = "Imported correctly, but...";
            }
            else
            {
                message = "Map model imported successfully!";
                title = "Success!";
            }
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exportMapButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog em = new SaveFileDialog
            {
                FileName = selectMapComboBox.SelectedItem.ToString()
            };

            byte[] modelToWrite;

            if (embedTexturesInMapModelCheckBox.Checked)
            { /* Textured NSBMD file */
                em.Filter = MapFile.TexturedNSBMDFilter;
                if (em.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                string texturePath = RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
                byte[] texturesToEmbed = File.ReadAllBytes(texturePath);
                modelToWrite = NSBUtils.BuildNSBMDwithTextures(currentMapFile.mapModelData, texturesToEmbed);
            }
            else
            { /* Untextured NSBMD file */
                em.Filter = MapFile.UntexturedNSBMDFilter;
                if (em.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                modelToWrite = currentMapFile.mapModelData;
            }

            File.WriteAllBytes(em.FileName, modelToWrite);
            MessageBox.Show("Map model exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void daeExportButton_Click(object sender, EventArgs e)
        {
            ModelUtils.ModelToDAE(
                modelName: selectMapComboBox.SelectedItem.ToString().TrimEnd('\0'),
                modelData: currentMapFile.mapModelData,
                textureData: mapTextureComboBox.SelectedIndex < 0 ? null : File.ReadAllBytes(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4"))
            );
        }

        private void glbExportButton_Click(object sender, EventArgs e)
        {
            ModelUtils.ModelToGLB(
                modelName: selectMapComboBox.SelectedItem.ToString().TrimEnd('\0'),
                modelData: currentMapFile.mapModelData,
                textureData: mapTextureComboBox.SelectedIndex < 0 ? null : File.ReadAllBytes(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4"))
            );
        }
        #endregion


        #region BDHC I/O
        private void bdhcImportButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog it = new OpenFileDialog()
            {
                Filter = RomInfo.gameFamily == GameFamilies.DP ? MapFile.BDHCFilter : MapFile.BDHCamFilter
            };

            if (it.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.ImportTerrain(File.ReadAllBytes(it.FileName));
            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";
            MessageBox.Show("Terrain settings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void bdhcExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                FileName = selectMapComboBox.SelectedItem.ToString(),
                Filter = RomInfo.gameFamily == GameFamilies.DP ? MapFile.BDHCFilter : MapFile.BDHCamFilter
            };

            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.bdhc);

            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";
            MessageBox.Show("Terrain settings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Soundplates I/O
        private void soundPlatesImportButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog it = new OpenFileDialog
            {
                Filter = MapFile.BGSFilter
            };

            if (it.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.ImportSoundPlates(File.ReadAllBytes(it.FileName));
            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = MapFile.BGSFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.bgs);

            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesBlankButton_Click(object sender, EventArgs e)
        {
            currentMapFile.bgs = MapFile.blankBGS;
            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data successfull blanked.\nRemember to save the current map file.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #endregion

        private void mapOpenGlControl_Load(object sender, EventArgs e)
        {
            mapOpenGlControl.InitializeContexts();
        }
    }
}

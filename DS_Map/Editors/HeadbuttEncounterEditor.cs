using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors {
    public partial class HeadbuttEncounterEditor : UserControl {
        public bool headbuttEncounterEditorIsReady { get; set; } = false;

        private ListBox2 listBoxTrees;
        private HeadbuttTree headbuttTree;

        private HeaderHGSS mapHeader;
        private HeadbuttEncounterFile headbuttEncounterFile;
        private HeadbuttEncounterMap headbuttEncounterMap;
        private GameMatrix gameMatrix;
        private AreaData areaData;
        private MapFile mapFile;
        private string locationName;

        private int width;
        private int height;
        static SimpleOpenGlControl2 openGlControl;

        private Pen selectedPen;
        private Pen normalPen;
        private SolidBrush normalBrush;
        private Pen specialPen;
        private SolidBrush specialBrush;

        private static float perspective;
        private static float ang;
        private static float dist;
        private static float elev;

        public HeadbuttEncounterEditor() {
            InitializeComponent();
        }

        //TODO: refresh headers list if a header is added
        public void SetupHeadbuttEncounterEditor(bool force = false) {
            if (headbuttEncounterEditorIsReady && !force) {
                return;
            }

            headbuttEncounterEditorIsReady = true;

            DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames>() {
                RomInfo.DirNames.dynamicHeaders,
                RomInfo.DirNames.matrices,
                RomInfo.DirNames.textArchives,
                RomInfo.DirNames.areaData,
                RomInfo.DirNames.headbutt,
                RomInfo.DirNames.maps,
                RomInfo.DirNames.mapTextures,
                RomInfo.DirNames.exteriorBuildingModels,
                RomInfo.DirNames.interiorBuildingModels, //No trees in interior maps, but fixes exceptions. @AdAstra - 11.06.2024
                RomInfo.DirNames.buildingTextures,
            });

            width = openGlPictureBox.Width;
            height = openGlPictureBox.Height;

            openGlControl = new SimpleOpenGlControl2();
            openGlControl.InitializeContexts();
            openGlControl.Width = width;
            openGlControl.Height = height;
            openGlControl.Invalidate();
            openGlControl.MakeCurrent();

            List<string> headerListBoxNames = Helpers.getHeaderListBoxNames();

            Color selectedColor = Color.FromArgb(255, Color.White);
            selectedPen = new Pen(selectedColor);

            Color normalColor = Color.FromArgb(128, Color.DarkBlue);
            normalPen = new Pen(normalColor);
            normalBrush = new SolidBrush(normalColor);

            Color specialColor = Color.FromArgb(128, Color.DarkRed);
            specialPen = new Pen(specialColor);
            specialBrush = new SolidBrush(specialColor);

            Helpers.DisableHandlers();

            for (int i = 0; i < Filesystem.GetHeadbuttCount(); i++) {
                if (i < headerListBoxNames.Count) {
                    headbuttFileComboBox.Items.Add(headerListBoxNames[i]);
                } else {
                    i.ToString("D4");
                }
            }

            string[] pokemonNames = RomInfo.GetPokemonNames();
            headbuttEncounterEditorTabNormal.comboBoxPokemon.Items.AddRange(pokemonNames);
            headbuttEncounterEditorTabNormal.comboBoxPokemon.SelectedIndex = 0;
            headbuttEncounterEditorTabNormal.listBoxTrees.SelectedIndexChanged += ListBoxTrees_SelectedIndexChanged;

            headbuttEncounterEditorTabSpecial.comboBoxPokemon.Items.AddRange(pokemonNames);
            headbuttEncounterEditorTabSpecial.comboBoxPokemon.SelectedIndex = 0;
            headbuttEncounterEditorTabSpecial.listBoxTrees.SelectedIndexChanged += ListBoxTrees_SelectedIndexChanged;

            openGlPictureBox.BringToFront();
            SetCam2DValues();

            Helpers.EnableHandlers();

            if (headbuttFileComboBox.Items.Count > 0) {
                headbuttFileComboBox.SelectedIndex = 0;
            }
        }

        public void makeCurrent() {
            openGlControl.MakeCurrent();
        }

        private void comboBoxMapHeader_SelectedIndexChanged(object sender, EventArgs e) {
            ushort headbuttID = (ushort)headbuttFileComboBox.SelectedIndex;
            this.headbuttEncounterFile = new HeadbuttEncounterFile(headbuttID);
            setCurrentMap(headbuttEncounterFile);
        }

        public void setCurrentMap(HeadbuttEncounterFile headbuttEncounterFile) {
            this.mapFile = null;
            this.headbuttEncounterMap = null;

            comboBoxMapFile.Items.Clear();
            labelLocationName.Text = "";

            listBoxTrees = null;
            if (headbuttTree != null) { headbuttTree.picked = false; }
            headbuttTree = null;

            headbuttEncounterEditorTabNormal.Reset();
            headbuttEncounterEditorTabSpecial.Reset();

            numericUpDownTreeGlobalX.Value = 0;
            numericUpDownTreeGlobalY.Value = 0;
            numericUpDownTreeMatrixX.Value = 0;
            numericUpDownTreeMatrixY.Value = 0;
            numericUpDownTreeMapX.Value = 0;
            numericUpDownTreeMapY.Value = 0;

            RenderBackground();

            try {
                if (headbuttEncounterFile.ID == GameMatrix.EMPTY) { return; }
                this.mapHeader = (HeaderHGSS)MapHeader.GetMapHeader(headbuttEncounterFile.ID);
            } catch (Exception ex) {
                //most likely more headbutt files than map headers
                //there should be the same amount
                Console.WriteLine(ex);
                return;
            }

            this.gameMatrix = new GameMatrix(mapHeader.matrixID);
            this.areaData = new AreaData(mapHeader.areaDataID);

            TextArchive currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);
            this.locationName = currentTextArchive.messages[mapHeader.locationName];
            labelLocationName.Text = locationName;

            headbuttEncounterEditorTabNormal.SetHeadbuttEncounter(headbuttEncounterFile.normalEncounters, headbuttEncounterFile.normalTreeGroups);
            headbuttEncounterEditorTabSpecial.SetHeadbuttEncounter(headbuttEncounterFile.specialEncounters, headbuttEncounterFile.specialTreeGroups);

            List<HeadbuttEncounterMap> mapHeaderMapsIDsList = new List<HeadbuttEncounterMap>();

            if (gameMatrix.hasHeadersSection) {
                for (int y = 0; y < gameMatrix.height; y++) {
                    for (int x = 0; x < gameMatrix.width; x++) {
                        if (gameMatrix.headers[y, x] == mapHeader.ID) {
                            int mapID = gameMatrix.maps[y, x];
                            
                            if (mapID != GameMatrix.EMPTY) {
                                HeadbuttEncounterMap map = new HeadbuttEncounterMap(mapID, x, y);
                                if (!mapHeaderMapsIDsList.Contains(map)) {
                                    mapHeaderMapsIDsList.Add(map);
                                }
                            }
                        }
                    }
                }
            } else {
                for (int y = 0; y < gameMatrix.height; y++) {
                    for (int x = 0; x < gameMatrix.width; x++) {
                        int mapID = gameMatrix.maps[y, x];
                        
                        if (mapID != GameMatrix.EMPTY) {
                            HeadbuttEncounterMap map = new HeadbuttEncounterMap(mapID, x, y);
                            if (!mapHeaderMapsIDsList.Contains(map)) {
                                mapHeaderMapsIDsList.Add(map);
                            }
                        }
                    }
                }
            }

            foreach (HeadbuttTreeGroup treeGroup in headbuttEncounterFile.normalTreeGroups) {
                foreach (HeadbuttTree tree in treeGroup.trees) {
                    if (!tree.IsUnused && tree.matrixX < gameMatrix.width && tree.matrixY < gameMatrix.height) {
                        int mapID = gameMatrix.maps[tree.matrixY, tree.matrixX];
                        if (mapID != GameMatrix.EMPTY) {
                            HeadbuttEncounterMap map = new HeadbuttEncounterMap(mapID, tree.matrixX, tree.matrixY);
                            if (!mapHeaderMapsIDsList.Contains(map)) {
                                mapHeaderMapsIDsList.Add(map);
                            }
                        }
                    }
                }
            }

            foreach (HeadbuttTreeGroup treeGroup in headbuttEncounterFile.specialTreeGroups) {
                foreach (HeadbuttTree tree in treeGroup.trees) {
                    if (!tree.IsUnused && tree.matrixX < gameMatrix.width && tree.matrixY < gameMatrix.height) {
                        int mapID = gameMatrix.maps[tree.matrixY, tree.matrixX];
                        if (mapID != GameMatrix.EMPTY) {
                            HeadbuttEncounterMap map = new HeadbuttEncounterMap(mapID, tree.matrixX, tree.matrixY);
                            if (!mapHeaderMapsIDsList.Contains(map)) {
                                mapHeaderMapsIDsList.Add(map);
                            }
                        }
                    }
                }
            }

            mapHeaderMapsIDsList.Sort((first, second) => {
                int ret = first.mapID.CompareTo(second.mapID);
                return ret == 0 ? first.x.CompareTo(second.x) : ret;
            });
            foreach (HeadbuttEncounterMap map in mapHeaderMapsIDsList) {
                comboBoxMapFile.Items.Add(map);
            }

            if (comboBoxMapFile.Items.Count > 0) {
                comboBoxMapFile.SelectedIndex = 0;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e) {
            if (headbuttEncounterFile == null) { return; }
            headbuttEncounterFile.SaveToFile();
        }

        private void buttonSaveAs_Click(object sender, EventArgs e) {
            if (headbuttEncounterFile == null) { return; }

            SaveFileDialog sfd = new SaveFileDialog();
            try {
                sfd.InitialDirectory = Path.GetDirectoryName(sfd.FileName);
                sfd.FileName = Path.GetFileName(sfd.FileName);
            } catch (Exception ex) {
                sfd.InitialDirectory = Path.GetDirectoryName(Environment.SpecialFolder.UserProfile.ToString());
                sfd.FileName = Path.GetFileName(sfd.FileName);
            }

            if (sfd.ShowDialog() != DialogResult.OK) { return; }

            headbuttEncounterFile.SaveToFile(sfd.FileName);
        }

        private void buttonImport_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            try {
                ofd.InitialDirectory = Path.GetDirectoryName(ofd.FileName);
                ofd.FileName = Path.GetFileName(ofd.FileName);
            } catch (Exception) {
                ofd.InitialDirectory = Path.GetDirectoryName(Environment.SpecialFolder.UserProfile.ToString());
                ofd.FileName = Path.GetFileName(ofd.FileName);
            }

            if (ofd.ShowDialog() != DialogResult.OK) { 
                return; 
            }

            this.headbuttEncounterFile = new HeadbuttEncounterFile(ofd.FileName);
            headbuttEncounterFile.ID = (ushort)headbuttFileComboBox.SelectedIndex;
            setCurrentMap(headbuttEncounterFile);
        }

        private void comboBoxMapFile_SelectedIndexChanged(object sender, EventArgs e) {
            HeadbuttEncounterMap map = comboBoxMapFile.SelectedItem as HeadbuttEncounterMap;
            int mapID = gameMatrix.maps[map.y, map.x];
            this.mapFile = new MapFile(mapID, RomInfo.gameFamily, discardMoveperms: true);
            this.headbuttEncounterMap = map;
            RenderBackground();
        }

        private Bitmap GetMapBitmap() {
            Bitmap bm = RenderMap();
            openGlControl.Invalidate();
            return bm;
        }

        private void RenderBackground() {
            Bitmap bm = GetMapBitmap();

            if (headbuttEncounterFile != null) {
                using (Graphics g = Graphics.FromImage(bm)) {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    MarkTrees(g, headbuttEncounterFile.normalTreeGroups, HeadbuttTree.Types.Normal);
                    MarkTrees(g, headbuttEncounterFile.specialTreeGroups, HeadbuttTree.Types.Special);
                }
            }

            openGlPictureBox.BackgroundImage = bm;
        }

        private Bitmap RenderMap() {
            MapFile currentMapFile = this.mapFile;

            if (currentMapFile == null) {
                Bitmap blank = new Bitmap(openGlPictureBox.Width, openGlPictureBox.Height);
                using (Graphics g = Graphics.FromImage(blank)) {
                    g.Clear(Color.Black);
                }

                return blank;
            }

            Helpers.MW_LoadModelTextures(currentMapFile, areaData.mapTileset);

            bool isInteriorMap = false;
            if (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS && areaData.areaType == AreaData.TYPE_INDOOR) {
                isInteriorMap = true;
            }

            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                Building building = currentMapFile.buildings[i];
                building.LoadModelData(isInteriorMap); // Load building nsbmd
                Helpers.MW_LoadModelTextures(building, areaData.buildingsTileset); // Load building textures                
            }

            Helpers.RenderMap(ref currentMapFile, openGlControl.Width, openGlControl.Height, ang, dist, elev, perspective);
            return Helpers.GrabMapScreenshot(width, height);
        }

        private void MarkTrees(Graphics g, BindingList<HeadbuttTreeGroup> treeGroups, HeadbuttTree.Types treeType) {
            HeadbuttEncounterMap map = comboBoxMapFile.SelectedItem as HeadbuttEncounterMap;
            if (map != null) {
                foreach (HeadbuttTreeGroup treeGroup in treeGroups) {
                    foreach (HeadbuttTree tree in treeGroup.trees) {
                        if (!tree.IsUnused && tree.matrixX == map.x && tree.matrixY == map.y) {
                            MarkTree(g, tree, treeType);
                        }
                    }
                }
            }
        }

        private void MarkTree(Graphics g, HeadbuttTree tree, HeadbuttTree.Types treeType) {
            Pen paintPen;
            SolidBrush paintBrush;
            if (treeType == HeadbuttTree.Types.Normal) {
                paintPen = normalPen;
                paintBrush = normalBrush;
            } else {
                paintPen = specialPen;
                paintBrush = specialBrush;
            }

            if (tree.picked) {
                paintPen = selectedPen;
            }

            int tileWidth = openGlControl.Width / MapFile.mapSize;
            int tileHeight = openGlControl.Height / MapFile.mapSize;
            int tileX = tree.mapX * tileWidth;
            int tileY = tree.mapY * tileHeight;

            int padding = 1;
            Rectangle rectangle = new Rectangle(tileX + padding, tileY + padding, tileWidth - padding, tileHeight - padding);
            g.FillRectangle(paintBrush, rectangle);
            g.DrawRectangle(paintPen, rectangle);
        }

        private void ListBoxTrees_SelectedIndexChanged(object sender, EventArgs e) {
            listBoxTrees = sender as ListBox2;
            headbuttTree = listBoxTrees.SelectedItem as HeadbuttTree;
            if (headbuttTree == null) { 
                return; 
            }
            numericUpDownTreeGlobalX.Value = headbuttTree.globalX;
            numericUpDownTreeGlobalY.Value = headbuttTree.globalY;
            numericUpDownTreeMatrixX.Value = headbuttTree.matrixX;
            numericUpDownTreeMatrixY.Value = headbuttTree.matrixY;
            numericUpDownTreeMapX.Value = headbuttTree.mapX;
            numericUpDownTreeMapY.Value = headbuttTree.mapY;
        }

        private void openGlPictureBox_Click(object sender, EventArgs e) {
            MouseEventArgs mea = (MouseEventArgs)e;

            int tileWidth = openGlControl.Width / MapFile.mapSize;
            int tileHeight = openGlControl.Height / MapFile.mapSize;
            int mouseX = openGlPictureBox.PointToClient(MousePosition).X / tileWidth;
            int mouseY = openGlPictureBox.PointToClient(MousePosition).Y / tileHeight;


            if (mea.Button == MouseButtons.Left) {
                if (this.headbuttEncounterMap != null) {
                    numericUpDownTreeMatrixX.Value = headbuttEncounterMap.x;
                    numericUpDownTreeMatrixY.Value = headbuttEncounterMap.y;
                    numericUpDownTreeMapX.Value = mouseX;
                    numericUpDownTreeMapY.Value = mouseY;
                }
            } else if (mea.Button == MouseButtons.Middle) {
                //warp
            } else if (mea.Button == MouseButtons.Right) {
                if (headbuttTree != null) { headbuttTree.picked = false; }

                if (FindTreeFromMap(headbuttEncounterEditorTabNormal.listBoxTreeGroups, headbuttEncounterEditorTabNormal.listBoxTrees, mouseX, mouseY)) {
                    tabControl.SelectedTab = tabPageNormal;
                } else if (FindTreeFromMap(headbuttEncounterEditorTabSpecial.listBoxTreeGroups, headbuttEncounterEditorTabSpecial.listBoxTrees, mouseX, mouseY)) {
                    tabControl.SelectedTab = tabPageSpecial;
                } else {
                    headbuttEncounterEditorTabNormal.listBoxTreeGroups.SelectedItem = null;
                    headbuttEncounterEditorTabNormal.listBoxTrees.SelectedItem = null;
                    headbuttEncounterEditorTabSpecial.listBoxTreeGroups.SelectedItem = null;
                    headbuttEncounterEditorTabSpecial.listBoxTrees.SelectedItem = null;
                }
            }

            RenderBackground();
        }

        private bool FindTreeFromMap(ListBox2 listBoxTreeGroups, ListBox2 listBoxTrees, int x, int y) {
            foreach (HeadbuttTreeGroup headbuttTreeGroup in listBoxTreeGroups.Items) {
                foreach (HeadbuttTree tree in headbuttTreeGroup.trees) {
                    if (tree.mapX == x && tree.mapY == y) {
                        listBoxTreeGroups.SelectedItem = headbuttTreeGroup;
                        listBoxTrees.SelectedItem = tree;
                        tree.picked = true;
                        return true;
                    }
                }
            }

            return false;
        }

        private void numericUpDownTreeGlobalX_ValueChanged(object sender, EventArgs e) {
            if (headbuttTree == null) { return; }
            headbuttTree.globalX = (ushort)((NumericUpDown)sender).Value;
            listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
        }

        private void numericUpDownTreeGlobalY_ValueChanged(object sender, EventArgs e) {
            if (headbuttTree == null) { return; }
            headbuttTree.globalY = (ushort)((NumericUpDown)sender).Value;
            listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
        }

        private void numericUpDownTreeMatrixX_ValueChanged(object sender, EventArgs e) {
            if (headbuttTree == null) { return; }
            headbuttTree.matrixX = (ushort)((NumericUpDown)sender).Value;
            listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
        }

        private void numericUpDownTreeMatrixY_ValueChanged(object sender, EventArgs e) {
            if (headbuttTree == null) { return; }
            headbuttTree.matrixY = (ushort)((NumericUpDown)sender).Value;
            listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
        }

        private void numericUpDownTreeMapX_ValueChanged(object sender, EventArgs e) {
            if (headbuttTree == null) { return; }
            headbuttTree.mapX = (ushort)((NumericUpDown)sender).Value;
            listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
        }

        private void numericUpDownTreeMapY_ValueChanged(object sender, EventArgs e) {
            if (headbuttTree == null) { return; }
            headbuttTree.mapY = (ushort)((NumericUpDown)sender).Value;
            listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
        }

        private void mapScreenshotButton_Click(object sender, EventArgs e) {
            SaveFileDialog imageSFD = new SaveFileDialog { 
                Filter = "PNG File(*.png)|*.png"
            };
            if (imageSFD.ShowDialog() != DialogResult.OK) { 
                return; 
            }
            openGlPictureBox.BackgroundImage.Save(imageSFD.FileName);
            MessageBox.Show("Screenshot saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SetCam2DValues() {
            perspective = 4f;
            ang = 0f;
            dist = 115.2f;
            elev = 90f;
        }

    }
}

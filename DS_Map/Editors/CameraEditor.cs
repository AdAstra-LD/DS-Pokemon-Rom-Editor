using System;
using System.IO;
using System.Windows.Forms;

namespace DSPRE.Editors
{
    public partial class CameraEditor : UserControl
    {
        MainProgram _parent;
        public bool cameraEditorIsReady { get; set; } = false;
        
        public CameraEditor()
        {
            InitializeComponent();
        }
        
         GameCamera[] currentCameraTable;
        uint overlayCameraTblOffset;

        public void SetupCameraEditor(MainProgram parent, bool force = false) 
        {
            if (cameraEditorIsReady && !force) { return; }
            
            cameraEditorIsReady = true;

            RomInfo.PrepareCameraData();
            cameraEditorDataGridView.Rows.Clear();

            if (OverlayUtils.OverlayTable.IsDefaultCompressed(RomInfo.cameraTblOverlayNumber)) {
                DialogResult d1 = MessageBox.Show("It is STRONGLY recommended to configure Overlay1 as uncompressed before proceeding.\n\n" +
                        "More details in the following dialog.\n\n" + "Do you want to know more?",
                        "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                bool userConfirmed = (d1 == DialogResult.Yes && PatchToolboxDialog.ConfigureOverlay1Uncompressed());


                if (!userConfirmed) {
                    MessageBox.Show("You chose not to apply the patch. Use this editor responsibly.\n\n" +
                            "If you change your mind, you can apply it later by accessing the Patch Toolbox.",
                            "Caution", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (OverlayUtils.IsCompressed(RomInfo.cameraTblOverlayNumber)) {
                        OverlayUtils.Decompress(RomInfo.cameraTblOverlayNumber);
                    }
                }
            }


            uint[] RAMaddresses = new uint[RomInfo.cameraTblOffsetsToRAMaddress.Length];
            string camOverlayPath = OverlayUtils.GetPath(RomInfo.cameraTblOverlayNumber);
            using (DSUtils.EasyReader br = new DSUtils.EasyReader(camOverlayPath)) {
                for (int i = 0; i < RomInfo.cameraTblOffsetsToRAMaddress.Length; i++) {
                    br.BaseStream.Position = RomInfo.cameraTblOffsetsToRAMaddress[i];
                    RAMaddresses[i] = br.ReadUInt32();
                }
            }

            uint referenceAddress = RAMaddresses[0];
            for (int i = 1; i < RAMaddresses.Length; i++) {
                uint ramAddress = RAMaddresses[i];
                if (ramAddress != referenceAddress) {
                    MessageBox.Show("Value of RAM Pointer to the overlay table is different between Offset #1 and Offset #" + (i + 1) + Environment.NewLine +
                        "The camera values might be wrong.", "Possible errors ahead", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            overlayCameraTblOffset = RAMaddresses[0] - OverlayUtils.OverlayTable.GetRAMAddress(RomInfo.cameraTblOverlayNumber);
            using (DSUtils.EasyReader br = new DSUtils.EasyReader(camOverlayPath, overlayCameraTblOffset)) {
                if (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS) {
                    currentCameraTable = new GameCamera[17];
                    for (int i = 0; i < currentCameraTable.Length; i++) {
                        currentCameraTable[i] = new GameCamera(br.ReadUInt32(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(),
                                                br.ReadInt16(), br.ReadByte(), br.ReadByte(),
                                                br.ReadUInt16(), br.ReadUInt32(), br.ReadUInt32(),
                                                br.ReadInt32(), br.ReadInt32(), br.ReadInt32());

                    }
                } else {
                    currentCameraTable = new GameCamera[16];
                    for (int i = 0; i < 3; i++) {
                        cameraEditorDataGridView.Columns.RemoveAt(cameraEditorDataGridView.Columns.Count - 3);
                    }
                    for (int i = 0; i < currentCameraTable.Length; i++) {
                        currentCameraTable[i] = new GameCamera(br.ReadUInt32(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(),
                                                br.ReadInt16(), br.ReadByte(), br.ReadByte(),
                                                br.ReadUInt16(), br.ReadUInt32(), br.ReadUInt32());
                    }
                }

                cameraEditorDataGridView.RowTemplate.Height = 32 * 16 / currentCameraTable.Length;
                for (int i = 0; i < currentCameraTable.Length; i++) {
                    currentCameraTable[i].ShowInGridView(cameraEditorDataGridView, i);
                }
            }
        }
        private void saveCameraTableButton_Click(object sender, EventArgs e) {
            SaveCameraTable(OverlayUtils.GetPath(RomInfo.cameraTblOverlayNumber), overlayCameraTblOffset);
        }
        private void cameraEditorDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {
            currentCameraTable[e.RowIndex][e.ColumnIndex] = cameraEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            cameraEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = currentCameraTable[e.RowIndex][e.ColumnIndex];
        }
        private void exportCameraTableButton_Click(object sender, EventArgs e) {
            SaveFileDialog of = new SaveFileDialog {
                Filter = "Camera Table File (*.bin)|*.bin",
                FileName = Path.GetFileNameWithoutExtension(RomInfo.projectName) + " - CameraTable.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.Delete(of.FileName);
            SaveCameraTable(of.FileName, 0);
        }
        private void SaveCameraTable(string path, uint destFileOffset) {
            for (int i = 0; i < currentCameraTable.Length; i++) {
                DSUtils.WriteToFile(path, currentCameraTable[i].ToByteArray(), (uint)(destFileOffset + i * RomInfo.cameraSize));
            }
            MessageBox.Show("Camera table correctly saved.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void cameraEditorDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            var senderTable = (DataGridView)sender;

            if (senderTable.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0) {
                string type = "Camera File";
                if (e.ColumnIndex == cameraEditorDataGridView.Columns.Count - 2) { //Export
                    SaveFileDialog sf = new SaveFileDialog {
                        Filter = type + " (*.bin)|*.bin",
                        FileName = Path.GetFileNameWithoutExtension(RomInfo.projectName) + " - Camera " + e.RowIndex + ".bin"
                    };

                    if (sf.ShowDialog(this) != DialogResult.OK) {
                        return;
                    }

                    DSUtils.WriteToFile(sf.FileName, currentCameraTable[e.RowIndex].ToByteArray(), fmode: FileMode.Create);
                    MessageBox.Show("Camera correctly saved.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else if (e.ColumnIndex == cameraEditorDataGridView.Columns.Count - 1) { //Import
                    OpenFileDialog of = new OpenFileDialog {
                        Filter = type + " (*.bin)|*.bin",
                    };

                    if (of.ShowDialog(this) != DialogResult.OK) {
                        return;
                    }

                    currentCameraTable[e.RowIndex] = new GameCamera(File.ReadAllBytes(of.FileName));
                    currentCameraTable[e.RowIndex].ShowInGridView(senderTable, e.RowIndex);
                    MessageBox.Show("Camera correctly imported.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void importCameraTableButton_Click(object sender, EventArgs e) {
            string fileType = "Camera Table File";
            OpenFileDialog of = new OpenFileDialog {
                Filter = fileType + " (*.bin)|*.bin",
            };

            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            long l = new FileInfo(of.FileName).Length;
            if (l % RomInfo.cameraSize != 0) {
                MessageBox.Show("This is not a " + RomInfo.gameFamily + ' ' + fileType +
                    "\nMake sure the file length is a multiple of " + RomInfo.cameraSize + " and try again.", "Wrong file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte nCameras = (byte)(l / RomInfo.cameraSize);
            for (byte b = 0; b < nCameras; b++) {
                currentCameraTable[b] = new GameCamera(DSUtils.ReadFromFile(of.FileName, b * RomInfo.cameraSize, RomInfo.cameraSize));
                currentCameraTable[b].ShowInGridView(cameraEditorDataGridView, b);
            }
            MessageBox.Show("Camera Table imported correctly.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
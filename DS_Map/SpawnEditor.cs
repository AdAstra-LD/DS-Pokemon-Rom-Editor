using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DSPRE {
    public partial class SpawnEditor : Form {
        private TextArchive locations;
        private List<string> names;
        public SpawnEditor(List<string> results, List<string> allNames, ushort headerNumber = 0, int matrixX = 0, int matrixY = 0) {
            InitializeComponent();
            this.names = allNames;

            if (results == null || results.Count <= 1) {
                SetupFields(allNames);
                spawnHeaderComboBox.SelectedIndex = headerNumber;
            } else {
                SetupFields(results);
                spawnHeaderComboBox.SelectedIndex = 0;
            }

            playerDirCombobox.SelectedIndex = 0;
            matrixxUpDown.Value = matrixX;
            matrixyUpDown.Value = matrixY;
        }
        public SpawnEditor(List<string> allNames) {
            InitializeComponent();
            this.names = allNames;
            SetupFields(allNames);
            readDefaultSpawnPosButton_Click(null, null);
        }              
        private void SetupFields(List<string> headersList) {
            SetupDirections();
            SetupHeadersList(headersList);

            locations = new TextArchive(RomInfo.locationNamesTextNumber);
            ReadDefaultMoney();
        }

        private void SetupHeadersList(List<string> headersList) {
            spawnHeaderComboBox.Items.Clear();
            spawnHeaderComboBox.Items.AddRange(headersList.ToArray());
        }
        private void SetupDirections () {
            playerDirCombobox.Items.Clear();
            playerDirCombobox.Items.AddRange(new string[4] { "Up", "Down", "Left", "Right" });
        }
        private void saveSpawnEditorButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("This operation will overwrite: " + Environment.NewLine
                + "- 10 bytes of data at ARM9 offset 0x" + RomInfo.arm9spawnOffset.ToString("X") + Environment.NewLine
                + "- 4 bytes of data at Overlay" + RomInfo.initialMoneyOverlayNumber + " offset 0x" + RomInfo.initialMoneyOffset.ToString("X") + 
                Environment.NewLine + "\nProceed?", "Confirmation required",  MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                string moneyOverlayPath = DSUtils.GetOverlayPath(RomInfo.initialMoneyOverlayNumber);
                ushort headerNumber = ushort.Parse(spawnHeaderComboBox.SelectedItem.ToString().Split()[0]);

                DSUtils.WriteToArm9(RomInfo.arm9spawnOffset, BitConverter.GetBytes(headerNumber));
                DSUtils.WriteToArm9(RomInfo.arm9spawnOffset + 8, BitConverter.GetBytes((short)(matrixxUpDown.Value * 32 + localmapxUpDown.Value)));
                DSUtils.WriteToArm9(RomInfo.arm9spawnOffset + 12, BitConverter.GetBytes((short)(matrixyUpDown.Value * 32 + localmapyUpDown.Value)));
                DSUtils.WriteToArm9(RomInfo.arm9spawnOffset + 16, BitConverter.GetBytes((short)playerDirCombobox.SelectedIndex));

                DSUtils.WriteToFile(moneyOverlayPath, RomInfo.initialMoneyOffset, BitConverter.GetBytes((int)initialMoneyUpDown.Value));
                MessageBox.Show("Your spawn settings have been changed.", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void readDefaultSpawnPosButton_Click(object sender, EventArgs e) {;
            SetupFields(names);

            ushort headerNumber = BitConverter.ToUInt16(DSUtils.ReadFromArm9(RomInfo.arm9spawnOffset, 2), 0);
            ushort globalX = BitConverter.ToUInt16(DSUtils.ReadFromArm9(RomInfo.arm9spawnOffset + 8, 2), 0);
            ushort globalY = BitConverter.ToUInt16(DSUtils.ReadFromArm9(RomInfo.arm9spawnOffset + 12, 2), 0);

            spawnHeaderComboBox.SelectedIndex = headerNumber;
            localmapxUpDown.Value = (short)(globalX % 32);
            localmapyUpDown.Value = (short)(globalY % 32);
            matrixxUpDown.Value = (ushort)(globalX / 32);
            matrixyUpDown.Value = (ushort)(globalY / 32);

            ReadDefaultMoney();
            playerDirCombobox.SelectedIndex = BitConverter.ToUInt16(DSUtils.ReadFromArm9(RomInfo.arm9spawnOffset + 16, 2), 0);
        }
        private void ReadDefaultMoney() {
            if (DSUtils.CheckOverlayHasCompressionFlag(RomInfo.initialMoneyOverlayNumber))
                if (DSUtils.OverlayIsCompressed(RomInfo.initialMoneyOverlayNumber))
                    DSUtils.DecompressOverlay(RomInfo.initialMoneyOverlayNumber, makeBackup: true);

            string pathToMoneyOverlay = DSUtils.GetOverlayPath(RomInfo.initialMoneyOverlayNumber);
            initialMoneyUpDown.Value = BitConverter.ToUInt32(DSUtils.ReadFromFile(pathToMoneyOverlay, RomInfo.initialMoneyOffset, 4), 0);
        }

        private void spawnHeaderComboBox_IndexChanged(object sender, EventArgs e) {
            ushort headerNumber = ushort.Parse(spawnHeaderComboBox.SelectedItem.ToString().Split()[0]);
            MapHeader currentHeader = MapHeader.LoadFromARM9(headerNumber);
            Matrix headerMatrix = new Matrix(currentHeader.matrixID);
            matrixxUpDown.Maximum = headerMatrix.maps.GetLength(1) - 1;
            matrixyUpDown.Maximum = headerMatrix.maps.GetLength(0) - 1;

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    locationNameLBL.Text = locations.messages[((HeaderDP)currentHeader).locationName];
                    break;
                case "Plat":
                    locationNameLBL.Text = locations.messages[((HeaderPt)currentHeader).locationName];
                    break;
                case "HG":
                case "SS":
                    locationNameLBL.Text = locations.messages[((HeaderHGSS)currentHeader).locationName];
                    break;
            }
        }

        private void resetFilterButton_Click(object sender, EventArgs e) {
            if (spawnHeaderComboBox.Items.Count < names.Count) {
                SetupHeadersList(names);
                spawnHeaderComboBox.SelectedIndex = 0;
            }
        }
    }
}
 
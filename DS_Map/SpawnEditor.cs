using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE {
    public partial class SpawnEditor : Form {
        private List<string> locations = RomInfo.GetLocationNames();
        private List<string> names;
        public SpawnEditor(HashSet<string> results, List<string> allNames, ushort headerNumber = 0, int matrixX = 0, int matrixY = 0) {
            InitializeComponent();
            this.names = allNames;

            if (results is null || results.Count <= 1) {
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
        private void SetupFields(IEnumerable<string> headersList) {
            SetupDirections();
            SetupHeadersList(headersList);
            ReadDefaultMoney();
        }

        private void SetupHeadersList(IEnumerable<string> headersList) {
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
                + "- 4 bytes of data at Overlay" + RomInfo.initialMoneyOverlayNumber + " offset 0x" + RomInfo.initialMoneyOverlayOffset.ToString("X") + 
                Environment.NewLine + "\nProceed?", "Confirmation required",  MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                string moneyOverlayPath = OverlayUtils.GetPath(RomInfo.initialMoneyOverlayNumber);
                ushort headerNumber = ushort.Parse(spawnHeaderComboBox.SelectedItem.ToString().Split()[0]);

                ARM9.WriteBytes(BitConverter.GetBytes(headerNumber), RomInfo.arm9spawnOffset);
                ARM9.WriteBytes(BitConverter.GetBytes((short)(matrixxUpDown.Value * 32 + localmapxUpDown.Value)), RomInfo.arm9spawnOffset + 8);
                ARM9.WriteBytes(BitConverter.GetBytes((short)(matrixyUpDown.Value * 32 + localmapyUpDown.Value)), RomInfo.arm9spawnOffset + 12);
                ARM9.WriteBytes(BitConverter.GetBytes((short)playerDirCombobox.SelectedIndex), RomInfo.arm9spawnOffset + 16);

                DSUtils.WriteToFile(moneyOverlayPath, BitConverter.GetBytes((int)initialMoneyUpDown.Value), RomInfo.initialMoneyOverlayOffset);
                MessageBox.Show("Your spawn settings have been changed.", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void readDefaultSpawnPosButton_Click(object sender, EventArgs e) {;
            SetupFields(names);

            ushort headerNumber = BitConverter.ToUInt16(ARM9.ReadBytes(RomInfo.arm9spawnOffset, 2), 0);
            ushort globalX = BitConverter.ToUInt16(ARM9.ReadBytes(RomInfo.arm9spawnOffset + 8, 2), 0);
            ushort globalY = BitConverter.ToUInt16(ARM9.ReadBytes(RomInfo.arm9spawnOffset + 12, 2), 0);

            spawnHeaderComboBox.SelectedIndex = headerNumber;
            
            localmapxUpDown.Value = (short)(globalX % 32);
            localmapyUpDown.Value = (short)(globalY % 32);


            try {
                matrixxUpDown.Value = (ushort)Math.Min(globalX / 32, matrixxUpDown.Maximum);
            } catch (ArgumentOutOfRangeException) {
                matrixxUpDown.Value = matrixxUpDown.Maximum;
            }

            try {
                matrixyUpDown.Value = (ushort)Math.Min(globalY / 32, matrixyUpDown.Maximum);
            } catch (ArgumentOutOfRangeException) {
                matrixyUpDown.Value = matrixyUpDown.Maximum;
            }
            
            ReadDefaultMoney();
            playerDirCombobox.SelectedIndex = BitConverter.ToUInt16(ARM9.ReadBytes(RomInfo.arm9spawnOffset + 16, 2), 0);
        }
        private void ReadDefaultMoney() {
            if (OverlayUtils.OverlayTable.IsDefaultCompressed(RomInfo.initialMoneyOverlayNumber)) {
                if (OverlayUtils.IsCompressed(RomInfo.initialMoneyOverlayNumber)) {
                    OverlayUtils.Decompress(RomInfo.initialMoneyOverlayNumber);
                }
            }

            string pathToMoneyOverlay = OverlayUtils.GetPath(RomInfo.initialMoneyOverlayNumber);
            initialMoneyUpDown.Value = BitConverter.ToUInt32(DSUtils.ReadFromFile(pathToMoneyOverlay, RomInfo.initialMoneyOverlayOffset, 4), 0);
        }

        private void spawnHeaderComboBox_IndexChanged(object sender, EventArgs e) {
            ushort headerNumber = ushort.Parse(spawnHeaderComboBox.SelectedItem.ToString().Split()[0]);

            MapHeader currentHeader;
            if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                currentHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerNumber.ToString("D4"), headerNumber, 0);
            } else {
                currentHeader = MapHeader.LoadFromARM9(headerNumber);
            }

            GameMatrix headerMatrix = new GameMatrix(currentHeader.matrixID);
            matrixxUpDown.Maximum = headerMatrix.maps.GetLength(1) - 1;
            matrixyUpDown.Maximum = headerMatrix.maps.GetLength(0) - 1;

            switch (RomInfo.gameFamily) {
                case GameFamilies.DP:
                    locationNameLBL.Text = locations[((HeaderDP)currentHeader).locationName];
                    break;
                case GameFamilies.Plat:
                    locationNameLBL.Text = locations[((HeaderPt)currentHeader).locationName];
                    break;
                case GameFamilies.HGSS:
                    locationNameLBL.Text = locations[((HeaderHGSS)currentHeader).locationName];
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
 
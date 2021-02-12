using DSPRE.ROMFiles;
using System;
using System.IO;
using System.Windows.Forms;

namespace DSPRE {
    public partial class SpawnEditor : Form {
        private TextArchive locations;
        public SpawnEditor(ListBox.ObjectCollection items, ushort headerNumber = 0, int matrixX = 0, int matrixY = 0) {
            InitializeComponent();
            SetupFields(items);

            playerDirCombobox.SelectedIndex = 0;
            spawnHeaderComboBox.SelectedIndex = headerNumber;

            matrixxUpDown.Value = matrixX;
            matrixyUpDown.Value = matrixY;
        }
        public SpawnEditor(ListBox.ObjectCollection items) {
            InitializeComponent();
            SetupFields(items);
            readDefaultSpawnPosButton_Click(null, null);
        }              
        private void SetupFields(ListBox.ObjectCollection items) {
            playerDirCombobox.Items.AddRange(new string[4] { "Up", "Down", "Left", "Right" });
            foreach (string s in items) {
                spawnHeaderComboBox.Items.Add(s);
            }
            locations = new TextArchive(RomInfo.locationNamesTextNumber);
            ReadDefaultMoney();
        }
        private void saveSpawnEditorButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("This operation will overwrite: " + Environment.NewLine
                + "- 10 bytes of data at ARM9 offset 0x" + RomInfo.arm9spawnOffset.ToString("X") + Environment.NewLine
                + "- 4 bytes of data at Overlay" + RomInfo.initialMoneyOverlayNumber + " offset 0x" + RomInfo.initialMoneyOffset.ToString("X") + 
                Environment.NewLine + "\nProceed?", "Confirmation required",  MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                string moneyOverlayPath = DSUtils.GetOverlayPath(RomInfo.initialMoneyOverlayNumber);

                DSUtils.WriteToArm9(RomInfo.arm9spawnOffset, BitConverter.GetBytes(spawnHeaderComboBox.SelectedIndex));
                DSUtils.WriteToArm9(RomInfo.arm9spawnOffset + 8, BitConverter.GetBytes((short)(matrixxUpDown.Value * 32 + localmapxUpDown.Value)));
                DSUtils.WriteToArm9(RomInfo.arm9spawnOffset + 12, BitConverter.GetBytes((short)(matrixyUpDown.Value * 32 + localmapyUpDown.Value)));
                DSUtils.WriteToArm9(RomInfo.arm9spawnOffset + 16, BitConverter.GetBytes((short)playerDirCombobox.SelectedIndex));

                DSUtils.WriteToFile(moneyOverlayPath, RomInfo.initialMoneyOffset, BitConverter.GetBytes((int)initialMoneyUpDown.Value));
                MessageBox.Show("Your spawn settings have been changed.", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void readDefaultSpawnPosButton_Click(object sender, EventArgs e) {
            ushort headerNumber = BitConverter.ToUInt16(DSUtils.ReadFromArm9(RomInfo.arm9spawnOffset, 2), 0);
            spawnHeaderComboBox.SelectedIndex = headerNumber;

            ushort globalX = BitConverter.ToUInt16(DSUtils.ReadFromArm9(RomInfo.arm9spawnOffset + 8, 2), 0);
            ushort globalY = BitConverter.ToUInt16(DSUtils.ReadFromArm9(RomInfo.arm9spawnOffset + 12, 2), 0);

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
            MapHeader currentHeader = MapHeader.LoadFromARM9((ushort)spawnHeaderComboBox.SelectedIndex);      
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
        private void SpawnEditor_FormClosed(object sender, FormClosedEventArgs e) {
            if (DSUtils.CheckOverlayHasCompressionFlag(RomInfo.initialMoneyOverlayNumber))
                if (!DSUtils.OverlayIsCompressed(RomInfo.initialMoneyOverlayNumber))                
                    DSUtils.CompressOverlay(RomInfo.initialMoneyOverlayNumber);
        }
    }
}
 
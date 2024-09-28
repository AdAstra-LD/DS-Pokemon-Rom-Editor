using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ScintillaNET.Style;

namespace DSPRE { 
    public partial class OverlayEditor : Form {
        
        private List<Overlay> overlays;
        private bool currentValComp = true;
        private bool currentValMark = true;

        public OverlayEditor() {
            InitializeComponent();
            overlays = new List<Overlay>();
            int numOverlays = OverlayUtils.OverlayTable.GetNumberOfOverlays();
            for (int i = 0; i < numOverlays; i++) {
                Overlay ovl = new Overlay();
                ovl.number = i;
                ovl.isCompressed = OverlayUtils.IsCompressed(i);
                ovl.isMarkedCompressed = OverlayUtils.OverlayTable.IsDefaultCompressed(i);
                ovl.RAMAddress = OverlayUtils.OverlayTable.GetRAMAddress(i);
                ovl.uncompressedSize = OverlayUtils.OverlayTable.GetUncompressedSize(i);
                overlays.Add(ovl);
            }
            overlayDataGrid.DataSource = overlays;
            overlayDataGrid.Columns[0].HeaderText = "Overlay ID";
            overlayDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            overlayDataGrid.AllowUserToResizeColumns = false;
            overlayDataGrid.Columns[1].HeaderText = "Compressed";
            overlayDataGrid.Columns[2].HeaderText = "Marked Compressed";
            overlayDataGrid.Columns[3].HeaderText = "RAM Address";
            overlayDataGrid.Columns[4].HeaderText = "Uncompressed Size";
            overlayDataGrid.Columns[0].ReadOnly = true;
            overlayDataGrid.Columns[3].ReadOnly = true;
            overlayDataGrid.Columns[4].ReadOnly = true;
        }

        private void isMarkedCompressedButton_Click(object sender, EventArgs e) {
            foreach (DataGridViewRow r in overlayDataGrid.Rows) {
                r.Cells[2].Value = currentValMark;                
            }
            currentValMark = !currentValMark;
        }

        private void isCompressedButton_Click(object sender, EventArgs e) {
            foreach (DataGridViewRow r in overlayDataGrid.Rows) {
                r.Cells[1].Value = currentValComp;                
            }
            currentValComp = !currentValComp;
        }

        private void revertChangesButton_Click(object sender, EventArgs e) {
            overlays = new List<Overlay>();
            int numOverlays = OverlayUtils.OverlayTable.GetNumberOfOverlays();
            for (int i = 0; i < numOverlays; i++) {
                Overlay ovl = new Overlay();
                ovl.number = i;
                ovl.isCompressed = OverlayUtils.IsCompressed(i);
                ovl.isMarkedCompressed = OverlayUtils.OverlayTable.IsDefaultCompressed(i);
                ovl.RAMAddress = OverlayUtils.OverlayTable.GetRAMAddress(i);
                ovl.uncompressedSize = OverlayUtils.OverlayTable.GetUncompressedSize(i);
                overlays.Add(ovl);
            }
            overlayDataGrid.DataSource = overlays;
        }

        private void overlayDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.ColumnIndex == 3 && e.Value != null) {
                long value = 0;
                if (long.TryParse(e.Value.ToString(), out value)) {
                    e.Value = "0x" + value.ToString("X");
                    e.FormattingApplied = true;
                }
            }            
        }

        private void saveChangesButton_Click(object sender, EventArgs e) {
            // This whole function needs proper optimizing, im just making dumb lists
            List<Overlay> originalOverlays = new List<Overlay>();
            int numOverlays = OverlayUtils.OverlayTable.GetNumberOfOverlays();
            for (int i = 0; i < numOverlays; i++) {
                Overlay ovl = new Overlay();
                ovl.number = i;
                ovl.isCompressed = OverlayUtils.IsCompressed(i);
                ovl.isMarkedCompressed = OverlayUtils.OverlayTable.IsDefaultCompressed(i);
                ovl.RAMAddress = OverlayUtils.OverlayTable.GetRAMAddress(i);
                ovl.uncompressedSize = OverlayUtils.OverlayTable.GetUncompressedSize(i);
                originalOverlays.Add(ovl);
            }
            List<string> modifiedNumbers = new List<string>();
            List<Overlay> modifiedOverlays = new List<Overlay>();
            for (int i = 0; i < originalOverlays.Count; i++) {
                Overlay originalOverlay = originalOverlays[i];
                Overlay newOverlay = overlays[i];

                // Compare properties
                if (originalOverlay.isCompressed != newOverlay.isCompressed || originalOverlay.isMarkedCompressed != newOverlay.isMarkedCompressed) {
                    modifiedOverlays.Add(newOverlay);
                    modifiedNumbers.Add(newOverlay.number.ToString());
                }
            }

            if(FindMismatches(false)) {
                MessageBox.Show("There are some overlays in a compression state that does not match the set value for compression in the y9 table.\n"
                    + "This may cause errors or lack of usability on hardware.\n"
                    + "You can find the mismatched cells coloured in RED.\nThis message is purely informational.", "Compression Mark Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            DialogResult d = MessageBox.Show("This operation will modify the following overlays: " + Environment.NewLine
                + String.Join(", ", modifiedNumbers)
                + "\nProceed?", "Confirmation required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                foreach (Overlay overlay in modifiedOverlays) {
                    OverlayUtils.OverlayTable.SetDefaultCompressed(overlay.number, overlay.isMarkedCompressed);
                    if (overlay.isCompressed && !OverlayUtils.IsCompressed(overlay.number))
                        OverlayUtils.Compress(overlay.number);
                    if (!overlay.isCompressed && OverlayUtils.IsCompressed(overlay.number))
                        OverlayUtils.Decompress(overlay.number);
                }
            }
        }

        private bool FindMismatches(bool paintThem = true) {
            foreach (DataGridViewRow row in overlayDataGrid.Rows) {
                if ((bool)row.Cells[1].Value != (bool)row.Cells[2].Value) {
                    if (paintThem) {
                        row.Cells[1].Style.BackColor = Color.Red;
                        row.Cells[2].Style.BackColor = Color.Red;
                    } else {
                        return true;
                    }                    
                } else {
                    if (paintThem) {
                        row.Cells[1].Style.BackColor = Color.White;
                        row.Cells[2].Style.BackColor = Color.White;
                    }                    
                }
            }
            return false;
        }

        private void overlayDataGrid_SelectionChanged(object sender, EventArgs e) {
            overlayDataGrid.ClearSelection();
        }

        private void overlayDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            FindMismatches();
        }
    }

    public class Overlay {
        public int number { get; set; }
        public bool isCompressed { get; set; }
        public bool isMarkedCompressed { get; set; }
        public uint RAMAddress { get; set; }
        public uint uncompressedSize { get; set; }
    }
}

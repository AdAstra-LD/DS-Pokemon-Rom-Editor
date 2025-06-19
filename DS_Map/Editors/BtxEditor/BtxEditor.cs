using DSPRE.LibNDSFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.Editors.BtxEditor
{
    public partial class BtxEditor : Form
    {
        private Bitmap bm;
        private byte[] BTXFile;

        // Track modified BTX files
        private Dictionary<ushort, byte[]> modifiedBTXFiles = new Dictionary<ushort, byte[]>();

        public BtxEditor()
        {
            RomInfo.SetOWtable();
            RomInfo.Set3DOverworldsDict();
            RomInfo.ReadOWTable();
            InitializeComponent();
            overworldList.Items.Clear();

            foreach (ushort key in RomInfo.OverworldTable.Keys)
            {
                overworldList.Items.Add("OW Entry " + key);
            }

            this.FormClosing += BtxEditor_FormClosing;
        }

        private void overworldList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = overworldList.SelectedIndex;
            if (selection < 0)
            {
                return;
            }

            showBtxFileButton.Enabled = true;
            exportImagePng.Enabled = true;
            importImagePng.Enabled = true;

            ushort overlayTableEntryID = (ushort)RomInfo.OverworldTable.Keys.ElementAt(selection);
            uint spriteID = RomInfo.OverworldTable[overlayTableEntryID].spriteID;
            string path = RomInfo.gameDirs[DirNames.OWSprites].unpackedDir + "\\" + spriteID.ToString("D4");

            if (modifiedBTXFiles.TryGetValue(overlayTableEntryID, out byte[] modifiedData))
            {
                BTXFile = modifiedData;
            }
            else if (File.Exists(path))
            {
                BTXFile = File.ReadAllBytes(path);
            }
            else
            {
                overworldPictureBox.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("overworldUnreadable");
                return;
            }

            bm = BTX0.Read(BTXFile);
            if (bm != null)
            {
                shinyCheckbox.Enabled = (BTX0.PaletteSize == 64 && BTX0.PaletteCount == 2);

                overworldPictureBox.Width = bm.Width;
                overworldPictureBox.Height = bm.Height;
                overworldPictureBox.Image = bm;
            }
            else
            {
                MessageBox.Show("This file is not supported.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void shinyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (shinyCheckbox.Enabled)
            {
                BTX0.PaletteIndex = shinyCheckbox.Checked ? 1u : 0u;
                bm = BTX0.Read(BTXFile);
                overworldPictureBox.Image = bm;
            }
        }

        private void exportImagePng_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save As";
            saveFileDialog.Filter = "PNG (*.png)|*.png";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                bm.Save(saveFileDialog.FileName);
            }
        }

        private void importImagePng_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open";
            openFileDialog.Filter = "PNG (*.png)|*.png";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Bitmap bitmap = new Bitmap(openFileDialog.FileName);
            if (bm.Width == bitmap.Width && bm.Height == bitmap.Height)
            {
                if (GetColorCount(bitmap) <= BTX0.ColorCount)
                {
                    BTXFile = BTX0.Write(BTXFile, bitmap);
                    bm = BTX0.Read(BTXFile);
                    overworldPictureBox.Image = bm;

                    ushort overlayTableEntryID = (ushort)RomInfo.OverworldTable.Keys.ElementAt(overworldList.SelectedIndex);
                    modifiedBTXFiles[overlayTableEntryID] = BTXFile;

                    MessageBox.Show($"OW Entry {overlayTableEntryID} marked as modified. Use Save Selected or Save All to write it.", "Pending Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Too many colors!\nBTX: " + BTX0.ColorCount + "\nPNG: " + GetColorCount(bitmap), "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            else
            {
                MessageBox.Show("Not the same size!\nBTX: " + bm.Width + "x" + bm.Height + "\nPNG: " + bitmap.Width + "x" + bitmap.Height, "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private uint GetColorCount(Bitmap temp)
        {
            HashSet<Color> hashSet = new HashSet<Color>();
            for (int y = 0; y < temp.Height; y++)
            {
                for (int x = 0; x < temp.Width; x++)
                {
                    hashSet.Add(temp.GetPixel(x, y));
                }
            }
            return (uint)hashSet.Count;
        }

        private void showBtxFileButton_Click(object sender, EventArgs e)
        {
            ushort overlayTableEntryID = (ushort)RomInfo.OverworldTable.Keys.ElementAt(overworldList.SelectedIndex);
            uint spriteID = RomInfo.OverworldTable[overlayTableEntryID].spriteID;
            string path = RomInfo.gameDirs[DirNames.OWSprites].unpackedDir + "\\" + spriteID.ToString("D4");
            Helpers.ExplorerSelect(path);
        }

        private void SaveAll_Button_Click(object sender, EventArgs e)
        {
            int savedCount = 0;
            foreach (var kvp in modifiedBTXFiles.ToList())
            {
                ushort entryID = kvp.Key;
                byte[] data = kvp.Value;
                uint spriteID = RomInfo.OverworldTable[entryID].spriteID;
                string path = Path.Combine(RomInfo.gameDirs[DirNames.OWSprites].unpackedDir, spriteID.ToString("D4"));

                try
                {
                    File.WriteAllBytes(path, data);
                    modifiedBTXFiles.Remove(entryID);
                    savedCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save OW Entry {entryID}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            MessageBox.Show($"Saved {savedCount} modified BTX file(s).", "Batch Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveSelected_Button_Click(object sender, EventArgs e)
        {
            if (overworldList.SelectedIndex < 0) return;

            ushort overlayTableEntryID = (ushort)RomInfo.OverworldTable.Keys.ElementAt(overworldList.SelectedIndex);

            if (!modifiedBTXFiles.TryGetValue(overlayTableEntryID, out byte[] btxData))
            {
                MessageBox.Show("No modification to save for the selected OW Entry.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            uint spriteID = RomInfo.OverworldTable[overlayTableEntryID].spriteID;
            string path = Path.Combine(RomInfo.gameDirs[DirNames.OWSprites].unpackedDir, spriteID.ToString("D4"));

            try
            {
                File.WriteAllBytes(path, btxData);
                modifiedBTXFiles.Remove(overlayTableEntryID);
                MessageBox.Show($"Saved OW Entry {overlayTableEntryID} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save OW Entry {overlayTableEntryID}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool isConfirmingExit = false;

        private void BtxEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (modifiedBTXFiles.Count == 0 || isConfirmingExit || e.Cancel)
                return;

            BtxExitConfirmation dialog = new BtxExitConfirmation(modifiedBTXFiles, RomInfo.OverworldTable);
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            } else
            {
                isConfirmingExit = true;
            }

        }

    }
}

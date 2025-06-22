using DSPRE.LibNDSFormats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static DSPRE.RomInfo;
using Panel = System.Windows.Forms.Panel;

namespace DSPRE.Editors.BtxEditor
{
    public partial class BtxExitConfirmation : Form
    {
        private SortedDictionary<uint, (uint spriteID, ushort properties)> _overworldList;
        private Dictionary<ushort, byte[]> _modifiedBtx;

        public BtxExitConfirmation(Dictionary<ushort, byte[]> modifiedBtx, SortedDictionary<uint, (uint spriteID, ushort properties)> overworldList)
        {
            InitializeComponent();
            unsavedFileCountLabel.Text = $"{modifiedBtx.Count} BTX file(s) unsaved";
            _overworldList = overworldList;
            _modifiedBtx = modifiedBtx;

            foreach (var entryID in modifiedBtx.Keys)
            {
                uint spriteID = RomInfo.OverworldTable[entryID].spriteID;
                modifiedOverworldsDetails.Items.Add($"OW Entry {entryID} → Sprite {spriteID:D4}");
            }

            panel1.MouseWheel += Panel_MouseWheel;
            panel2.MouseWheel += Panel_MouseWheel;
        }

        private void modifiedOverworldsDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selection = modifiedOverworldsDetails.SelectedIndex;

            if (selection == -1) return;
            var selectedObj = _modifiedBtx.ElementAt(selection);
            Bitmap newSprite = BTX0.Read(selectedObj.Value);
            afterSpriteBox.Image = newSprite;
            afterSpriteBox.Width = newSprite.Width;
            afterSpriteBox.Height = newSprite.Height;

            uint spriteID = _overworldList[selectedObj.Key].spriteID;
            string path = RomInfo.gameDirs[DirNames.OWSprites].unpackedDir + "\\" + spriteID.ToString("D4");

            if (File.Exists(path))
            {
                Bitmap oldSprite = BTX0.Read(File.ReadAllBytes(path));
                beforeSpriteBox.Image = oldSprite;
                beforeSpriteBox.Width = oldSprite.Width;
                beforeSpriteBox.Height = oldSprite.Height;
            }

        }

        private bool syncing = false;

        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            if (syncing) return;
            syncing = true;
            panel2.AutoScrollPosition = new Point(
                panel2.AutoScrollPosition.X,
                panel1.VerticalScroll.Value
            );
            syncing = false;
        }

        private void panel2_Scroll(object sender, ScrollEventArgs e)
        {
            if (syncing) return;
            syncing = true;
            panel1.AutoScrollPosition = new Point(
                panel1.AutoScrollPosition.X,
                panel2.VerticalScroll.Value
            );
            syncing = false;
        }
        

        private void Panel_MouseWheel(object sender, MouseEventArgs e)
        {
            var source = sender as Panel;
            var target = (source == panel1) ? panel2 : panel1;

            int scrollDelta = e.Delta; // Usually 120 or -120
            int newValue = source.VerticalScroll.Value - scrollDelta;

            // Clamp the new scroll value within limits
            newValue = Math.Max(source.VerticalScroll.Minimum,
                       Math.Min(source.VerticalScroll.Maximum - source.Height, newValue));

            source.AutoScrollPosition = new Point(0, newValue);
            target.AutoScrollPosition = new Point(0, newValue);
        }



    }
}

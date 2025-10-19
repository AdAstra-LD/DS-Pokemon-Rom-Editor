using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE.Editors
{
    public partial class MonReorderForm : Form
    {
        public TrainerFile trainerFile;
        private bool dirty = false;

        public MonReorderForm(TrainerFile trainerFile)
        {
            this.trainerFile = trainerFile;
            InitializeComponent();

            PopulateMonList();

        }

        private void PopulateMonList()
        {
            // Add the Pokémon in the trainer's party to the list box
            for (int i = 0; i < trainerFile.trp.partyCount; i++)
            {
                string monName = RomInfo.GetPokemonNames()[(int)trainerFile.party[i].pokeID];
                monListBox.Items.Add($"[{i}] {monName} Lv. {trainerFile.party[i].level}");
            }
        }

        private void SaveChanges()
        {
            // Create a new party array to hold the reordered Pokémon
            List<PartyPokemon> newParty = new List<PartyPokemon>();

            for (int i = 0; i < monListBox.Items.Count; i++)
            {
                // Extract the original index from the list box item string
                string item = (string)monListBox.Items[i];
                int originalIndex = int.Parse(item.Split(']')[0].TrimStart('['));
                // Add the corresponding Pokémon to the new party list
                newParty.Add(trainerFile.party[originalIndex]);
            }

            // Update the trainer's party with the new order
            for (int i = 0; i < newParty.Count; i++)
            {
                trainerFile.party[i] = newParty[i];
            }

            SetDirty(false);
        }

        private void SetDirty(bool newStatus)
        {
            if (newStatus)
            {
                dirty = true;
                this.Text = this.Text.TrimEnd('*') + "*";
            }
            else
            {
                dirty = false;
                this.Text = this.Text.TrimEnd('*');
            }
        }

        private bool CheckUnsavedChanges()
        {
            if (dirty)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save them?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    SaveChanges();
                    return true; // Proceed with closing
                }
                else if (result == DialogResult.No)
                {
                    return true; // Proceed with closing without saving
                }
                else
                {
                    return false; // Cancel closing
                }
            }
            return true; // No unsaved changes, proceed with closing
        }
        private void MonReorderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckUnsavedChanges())
            {
                e.Cancel = true; // Cancel the form closing
            }
        }

        private void MoveItem(bool up)
        {
            int selectedIndex = monListBox.SelectedIndex;
            if (selectedIndex == -1) return;

            int newIndex = up ? selectedIndex - 1 : selectedIndex + 1;

            // Check bounds
            if (newIndex < 0 || newIndex >= monListBox.Items.Count) return;

            // Swap items
            var item = monListBox.Items[selectedIndex];
            monListBox.Items.RemoveAt(selectedIndex);
            monListBox.Items.Insert(newIndex, item);
            monListBox.SelectedIndex = newIndex; // Reselect the moved item

            SetDirty(true);
        }

        private void UpdateButtonStates()
        {
            int selectedIndex = monListBox.SelectedIndex;
            moveUpButton.Enabled = selectedIndex > 0;
            moveDownButton.Enabled = selectedIndex != -1 && selectedIndex < monListBox.Items.Count - 1;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void monListBox_DragDrop(object sender, DragEventArgs e)
        {
            // Make sure the object being dragged is part of the list box
            if (!monListBox.Items.Contains(e.Data.GetData(typeof(string))))
                return;

            string draggedItem = (string)e.Data.GetData(typeof(string));

            // Get the index of the item being dragged
            int draggedIndex = monListBox.Items.IndexOf(draggedItem);

            // Get the index of the item being dropped onto
            Point point = monListBox.PointToClient(new Point(e.X, e.Y));
            int dropIndex = monListBox.IndexFromPoint(point);

            if (dropIndex == -1) dropIndex = monListBox.Items.Count - 1; // If dropped below all items, set to last index

            // Remove the dragged item from its original position and insert it at the new position
            monListBox.Items.RemoveAt(draggedIndex);
            monListBox.Items.Insert(dropIndex, draggedItem);
            monListBox.SelectedIndex = dropIndex; // Select the moved item
            SetDirty(true);
        }

        private void monListBox_MouseDown(object sender, MouseEventArgs e)
        {
            // Start the drag-and-drop operation when an item is clicked
            if (monListBox.SelectedItem == null) return;
            monListBox.DoDragDrop(monListBox.SelectedItem, DragDropEffects.Move);
        }

        private void monListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            MoveItem(true);
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            MoveItem(false);
        }

        private void monListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }
    }
}

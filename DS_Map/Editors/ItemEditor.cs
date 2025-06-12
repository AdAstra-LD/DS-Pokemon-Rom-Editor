using DSPRE.Resources;
using DSPRE.ROMFiles;
using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace DSPRE.Editors
{
    public partial class ItemEditor : Form
    {

        private readonly string[] itemFileNames;
        //private readonly string[] itemDescriptions;

        private int currentLoadedId = 0;
        private ItemData currentLoadedFile = null;

        private static bool dirty = false;
        private static readonly string formName = "Item Data Editor";

        public ItemEditor(string[] itemFileNames) //, string[] itemDescriptions)
        {
            this.itemFileNames = itemFileNames.ToArray();
            //this.itemDescriptions = itemDescriptions;

            InitializeComponent();

            Helpers.DisableHandlers();

            itemNumberNumericUpDown.Maximum = itemFileNames.Length - 1;
            itemNameInputComboBox.Items.AddRange(this.itemFileNames);

            holdEffectComboBox.Items.AddRange(Enum.GetNames(typeof(HoldEffect)));
            fieldPocketComboBox.Items.AddRange(Enum.GetNames(typeof(FieldPocket)));
            battlePocketComboBox.Items.AddRange(Enum.GetNames(typeof(BattlePocket)));

            Helpers.EnableHandlers();

            itemNameInputComboBox.SelectedIndex = 1;
        }

        private void setDirty(bool status)
        {
            if (status)
            {
                dirty = true;
                this.Text = formName + "*";
            }
            else
            {
                dirty = false;
                this.Text = formName;
            }
        }

        private bool CheckDiscardChanges()
        {
            if (!dirty)
            {
                return true;
            }

            DialogResult res = MessageBox.Show(this, "There are unsaved changes to the current Item data.\nDiscard and proceed?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                return true;
            }

            itemNumberNumericUpDown.Value = currentLoadedId;
            itemNameInputComboBox.SelectedIndex = currentLoadedId;
            return false;
        }

        private void ChangeLoadedFile(int toLoad)
        {
            currentLoadedId = toLoad;
            currentLoadedFile = new ItemData(toLoad);
            PopulateAllFromCurrentFile();
            setDirty(false);
        }

        private void PopulateAllFromCurrentFile()
        {
            holdEffectComboBox.SelectedIndex = (int)currentLoadedFile.holdEffect;
            fieldPocketComboBox.SelectedIndex = (int)currentLoadedFile.fieldPocket;
            battlePocketComboBox.SelectedIndex = (int)currentLoadedFile.battlePocket;
            priceNumericUpDown.Value = currentLoadedFile.price;

            //descriptionTextBox.Text = itemDescriptions[currentLoadedId];
        }

        private void saveDataButton_Click(object sender, EventArgs e)
        {
            currentLoadedFile.SaveToFileDefaultDir(currentLoadedId, true);
            setDirty(false);
        }

        private void itemNameInputComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Helpers.DisableHandlers();

            if (CheckDiscardChanges())
            {
                int newId = itemNameInputComboBox.SelectedIndex;
                itemNumberNumericUpDown.Value = newId;
                ChangeLoadedFile(newId);
            }

            Helpers.EnableHandlers();
        }

        private void itemNumberNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Helpers.DisableHandlers();

            if (CheckDiscardChanges())
            {
                int newId = (int)itemNumberNumericUpDown.Value;
                itemNameInputComboBox.SelectedIndex = newId;
                ChangeLoadedFile(newId);
            }

            Helpers.EnableHandlers();
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string suggestedFilename = this.itemFileNames[currentLoadedId];
            currentLoadedFile.SaveToFileExplorePath(suggestedFilename, true);
        }

        private void holdEffectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.holdEffect = (HoldEffect)holdEffectComboBox.SelectedIndex;
            setDirty(true);
        }

        private void fieldPocketComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.fieldPocket = (FieldPocket)fieldPocketComboBox.SelectedIndex;
            setDirty(true);
        }

        private void battlePocketComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.battlePocket = (BattlePocket)battlePocketComboBox.SelectedIndex;
            setDirty(true);
        }

        private void priceNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.price = (ushort)priceNumericUpDown.Value;
            setDirty(true);
        }
    }
}

using DSPRE.Resources;
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

        private string[] itemFileNames;
        //private readonly string[] itemDescriptions;

        private int currentLoadedId = 0;
        private ItemData currentLoadedFile = null;

        private static bool dirty = false;
        private static readonly string formName = "Item Data Editor";

        public ItemEditor(string[] itemFileNames) //, string[] itemDescriptions)
        {
            int killCount = 0;
            List<string> cleanNames = itemFileNames.ToList();
            for (int i = 0; i < itemFileNames.Length; i++)
            {
                if (itemFileNames[i] == null || itemFileNames[i] == "???")
                {
                    cleanNames.RemoveAt(i-killCount);
                    killCount++;
                }
            }
            this.itemFileNames = cleanNames.ToArray();
            //this.itemDescriptions = itemDescriptions;

            InitializeComponent();

            Helpers.DisableHandlers();

            // Set up max and min for numerics
            priceNumericUpDown.Minimum = 0;
            priceNumericUpDown.Maximum = 65535;
            itemNumberNumericUpDown.Minimum = 1;
            itemNumberNumericUpDown.Maximum = this.itemFileNames.Length - 1;
            holdEffectParameterNumericUpDown.Minimum = 0;
            holdEffectParameterNumericUpDown.Maximum = 255;
            pluckEffectNumericUpDown.Minimum = 0;
            pluckEffectNumericUpDown.Maximum = 255;
            flingEffectNumericUpDown.Minimum = 0;
            flingEffectNumericUpDown.Maximum = 255;
            flingPowerNumericUpDown.Minimum = 0;
            flingPowerNumericUpDown.Maximum = 255;
            naturalGiftPowerNumericUpDown.Minimum = 0;
            naturalGiftPowerNumericUpDown.Maximum = 255;

            // Set up combobox ranges
            itemNameInputComboBox.Items.AddRange(this.itemFileNames);
            holdEffectComboBox.Items.AddRange(Enum.GetNames(typeof(HoldEffect)));
            fieldPocketComboBox.Items.AddRange(Enum.GetNames(typeof(FieldPocket)));
            battlePocketComboBox.Items.AddRange(Enum.GetNames(typeof(BattlePocket)));
            naturalGiftTypeComboBox.Items.AddRange(Enum.GetNames(typeof(NaturalGiftType)));

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
            Console.WriteLine("ItemEditor: ChangeLoadedFile: toLoad = " + toLoad);
            currentLoadedId = toLoad;
            currentLoadedFile = new ItemData(toLoad);
            PopulateAllFromCurrentFile();
            setDirty(false);
        }

        private void PopulateAllFromCurrentFile()
        {
            // Hold effects
            holdEffectComboBox.SelectedIndex = (int)currentLoadedFile.holdEffect;
            holdEffectParameterNumericUpDown.Value = currentLoadedFile.HoldEffectParam;

            // Pockets
            fieldPocketComboBox.SelectedIndex = (int)currentLoadedFile.fieldPocket;
            // Set the selected value for non sequential enums
            BattlePocket battlePocket = (BattlePocket)currentLoadedFile.battlePocket;
            string battlePocketEnum = Enum.GetName(typeof(BattlePocket), battlePocket);
            battlePocketComboBox.SelectedItem = battlePocketEnum;

            // Move Related
            // Set the selected value for non sequential enums
            NaturalGiftType naturalGiftType = (NaturalGiftType)currentLoadedFile.naturalGiftType;
            string naturalGiftTypeEnum = Enum.GetName(typeof(NaturalGiftType), naturalGiftType);
            naturalGiftTypeComboBox.SelectedItem = naturalGiftTypeEnum;
            naturalGiftPowerNumericUpDown.Value = currentLoadedFile.NaturalGiftPower;
            flingEffectNumericUpDown.Value = currentLoadedFile.FlingEffect;
            flingPowerNumericUpDown.Value = currentLoadedFile.FlingPower;
            pluckEffectNumericUpDown.Value = currentLoadedFile.PluckEffect;

            // Checks
            preventTossCheckBox.Checked = currentLoadedFile.PreventToss;
            canSelectCheckBox.Checked = currentLoadedFile.Selectable;

            // Price
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
                Console.WriteLine("ItemEditor: itemNameInputComboBox_SelectedIndexChanged: newId = " + newId);
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
                Console.WriteLine("ItemEditor: itemNumberNumericUpDown_ValueChanged: newId = " + newId);
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

            currentLoadedFile.battlePocket = (BattlePocket)battlePocketComboBox.SelectedValue;
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

        private void holdEffectParameterNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.HoldEffectParam = (byte)holdEffectParameterNumericUpDown.Value;
            setDirty(true);
        }

        private void naturalGiftTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.naturalGiftType = (NaturalGiftType)naturalGiftTypeComboBox.SelectedIndex;
            setDirty(true);
        }

        private void naturalGiftPowerNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.NaturalGiftPower = (byte)naturalGiftPowerNumericUpDown.Value;
            setDirty(true);
        }

        private void pluckEffectNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.PluckEffect = (byte)pluckEffectNumericUpDown.Value;
            setDirty(true);
        }

        private void flingEffectNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.FlingEffect = (byte)flingEffectNumericUpDown.Value;
            setDirty(true);
        }

        private void flingPowerNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.FlingPower = (byte)flingPowerNumericUpDown.Value;
            setDirty(true);
        }

        private void preventTossCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.PreventToss = preventTossCheckBox.Checked;
            setDirty(true);
        }

        private void canSelectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.Selectable = canSelectCheckBox.Checked;
            setDirty(true);
        }
    }
}

using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE.Editors
{
    public partial class TradeEditor : Form
    {

        private TradeData curTradeData;
        private TextArchive tradeArchive;

        // Track if any changes have been made
        private bool tradeDirty = false; 
        private bool textDirty = false;

        #region Enums
        public enum OriginLang
        {
            NONE = 0,
            JAPANESE = 1,
            ENGLISH = 2,
            FRENCH = 3,
            ITALIAN = 4,
            GERMAN = 5,
            UNUSED = 6,
            SPANISH = 7,
            KOREAN = 8
        }
        #endregion

        public TradeEditor()
        {
            Helpers.DisableHandlers();

            AppLogger.Debug("TradeEditor: Initializing Trade Editor.");

            InitializeComponent();
            InitLimits();
            InitDataRanges();
            SetToolTips();
            RegisterMarkDirtyHandlers();

            if (TradeData.GetTradeCount() == 0)
            {
                MessageBox.Show("No trades could be found! The narc may have failed to unpack.", "Loading Trades Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppLogger.Error("TradeEditor: Loading trades failed.");
                return;
            }

            if (RomInfo.gameFamily != RomInfo.GameFamilies.HGSS)
            {
                unknownNumericUpDown.Enabled = false;
            }

            // PLAT: Overlay 6, 0x8050-0x8053: dc f5 f0 fb -> 00 00 00 00 to dummy out ASSERT

            // Disable buttons until trade expansion is implemented
            addFileButton.Enabled = false;
            removeLastButton.Enabled = false;

            tradeArchive = new TextArchive(GetTextBankIndex());
            LoadFromFile(0);
        }

        private void InitLimits()
        {
            // IVs: 0-31
            hpIVNumericUpDown.Minimum = 0;
            hpIVNumericUpDown.Maximum = 31;
            atkIVNumericUpDown.Minimum = 0;
            atkIVNumericUpDown.Maximum = 31;
            defIVNumericUpDown.Minimum = 0;
            defIVNumericUpDown.Maximum = 31;
            speIVNumericUpDown.Minimum = 0;
            speIVNumericUpDown.Maximum = 31;
            spaIVNumericUpDown.Minimum = 0;
            spaIVNumericUpDown.Maximum = 31;
            spdIVNumericUpDown.Minimum = 0;
            spdIVNumericUpDown.Maximum = 31;

            // Contest stats: 0-255
            coolNumericUpDown.Minimum = 0;
            coolNumericUpDown.Maximum = 255;
            beautyNumericUpDown.Minimum = 0;
            beautyNumericUpDown.Maximum = 255;
            cuteNumericUpDown.Minimum = 0;
            cuteNumericUpDown.Maximum = 255;
            smartNumericUpDown.Minimum = 0;
            smartNumericUpDown.Maximum = 255;
            toughNumericUpDown.Minimum = 0;
            toughNumericUpDown.Maximum = 255;

            // PID: (32-bit unsigned)
            pidNumericUpDown.Minimum = 0;
            pidNumericUpDown.Maximum = int.MaxValue;

            // Sheen: 0-255
            sheenNumericUpDown.Minimum = 0;
            sheenNumericUpDown.Maximum = 255;

            // Unknown: (32-bit unsigned)
            unknownNumericUpDown.Minimum = 0;
            unknownNumericUpDown.Maximum = int.MaxValue;

            // Trade ID: (dynamic based on loaded data)
            tradeIDNumericUpDown.Minimum = 0;
            tradeIDNumericUpDown.Maximum = TradeData.GetTradeCount() - 1;

            // OT ID: (32-bit unsigned)
            otIDNumericUpDown.Minimum = 0;
            otIDNumericUpDown.Maximum = int.MaxValue;

            // Text fields
            otNameTextBox.MaxLength = 7;
            nicknameTextBox.MaxLength = 10;
        }

        private void InitDataRanges()
        {
            // Species Names
            speciesComboBox.DataSource = RomInfo.GetPokemonNames();
            speciesComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            requestedComboBox.DataSource = RomInfo.GetPokemonNames();
            requestedComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

            // Abilities
            abilityComboBox.DataSource = RomInfo.GetAbilityNames();
            abilityComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

            // Held Items
            heldItemComboBox.DataSource = RomInfo.GetItemNames();
            heldItemComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

            // OT Gender
            otGenderComboBox.DataSource = new string[] { "Male", "Female" };

            // Languages
            langComboBox.DataSource = Enum.GetNames(typeof(OriginLang));
            langComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void SetToolTips()
        {
            SetToolTipsForControls(new Control[] { tradeIDNumericUpDown, tradeIDLabel }, "The Trade ID to edit.");
            SetToolTipsForControls(new Control[] { speciesComboBox, speciesLabel }, "The species of the Pokémon being traded.");
            SetToolTipsForControls(new Control[] { IVGroupBox }, "Individual Values (IVs)");
            SetToolTipsForControls(new Control[] { abilityComboBox, abilityLabel }, "The ability of the Pokémon.\nThis value is unused and will have no effect!");
            SetToolTipsForControls(new Control[] { otIDNumericUpDown, otIDLabel }, "The Original Trainer ID of the Pokémon.");
            SetToolTipsForControls(new Control[] { contestGroupBox }, "Contest stats for the Pokémon.");
            SetToolTipsForControls(new Control[] { pidNumericUpDown, pidLabel }, "The PID of the Pokémon.\nDetermines ability and nature.");
            SetToolTipsForControls(new Control[] { heldItemComboBox, heldItemLabel }, "The held item of the Pokémon.");
            SetToolTipsForControls(new Control[] { otGenderComboBox, otGenderLabel }, "The gender of the Original Trainer (OT).\nThe OT name is determined by the text bank.");
            SetToolTipsForControls(new Control[] { langComboBox, langLabel }, "The language of origin of the traded Pokémon.");
            SetToolTipsForControls(new Control[] { requestedComboBox, requestedLabel }, "The species of the requested Pokémon.\nWhether the Pokémon selected by the player matches this species or not is checked via scripting.");
            SetToolTipsForControls(new Control[] { unknownNumericUpDown, unknownLabel }, "An unknown value, present in HGSS only.\nAppears to be unused.");
            SetToolTipsForControls(new Control[] { otNameTextBox, otNameLabel }, 
                $"The Original Trainer's name.\nStored in text bank {GetTextBankIndex()}.\nAt most {otNameTextBox.MaxLength} characters.");
            SetToolTipsForControls(new Control[] { nicknameTextBox, nicknameLabel }, 
                $"The nickname of the Pokémon.\nStored in text bank {GetTextBankIndex()}.\nAt most {nicknameTextBox.MaxLength} characters.");

            SetToolTipsForControls(new Control[] { saveTradeButton }, "Save the current trade data to file.");
            SetToolTipsForControls(new Control[] { saveTextDataButton }, "Save the current text data to file.");
            SetToolTipsForControls(new Control[] { saveAllButton }, "Save both trade data and text data to file.");

            // ToDo: Remove these tooltips when the features are implemented
            SetToolTipsForControls(new Control[] { addFileButton, removeLastButton }, "Coming soon!\nThis feature is not yet implemented.\nIt will allow you to add or remove trades from the trade data file.");
        }

        private void SetToolTipsForControls(IEnumerable<Control> controls, string text)
        {
            foreach (var control in controls)
            {
                toolTip.SetToolTip(control, text);
            }
        }

        public void LoadFromFile(int tradeID)
        {
            Helpers.DisableHandlers();
            curTradeData = new TradeData(tradeID);
            tradeArchive = new TextArchive(GetTextBankIndex());
            tradeIDNumericUpDown.Value = tradeID;
            speciesComboBox.SelectedIndex = curTradeData.species;
            hpIVNumericUpDown.Value = curTradeData.hpIV;
            atkIVNumericUpDown.Value = curTradeData.atkIV;
            defIVNumericUpDown.Value = curTradeData.defIV;
            speIVNumericUpDown.Value = curTradeData.speedIV;
            spaIVNumericUpDown.Value = curTradeData.spAtkIV;
            spdIVNumericUpDown.Value = curTradeData.spDefIV;
            abilityComboBox.SelectedIndex = curTradeData.ability;
            otIDNumericUpDown.Value = curTradeData.otID;
            coolNumericUpDown.Value = curTradeData.cool;
            beautyNumericUpDown.Value = curTradeData.beauty;
            cuteNumericUpDown.Value = curTradeData.cute;
            smartNumericUpDown.Value = curTradeData.smart;
            toughNumericUpDown.Value = curTradeData.tough;
            pidNumericUpDown.Value = curTradeData.pid;
            heldItemComboBox.SelectedIndex = curTradeData.heldItem;
            otGenderComboBox.SelectedIndex = curTradeData.otGender;
            sheenNumericUpDown.Value = curTradeData.sheen;
            langComboBox.SelectedIndex = curTradeData.language;
            requestedComboBox.SelectedIndex = curTradeData.requestedSpecies;
            unknownNumericUpDown.Value = curTradeData.unknown;

            otNameTextBox.Text = GetOTName(tradeID);
            nicknameTextBox.Text = GetMonNickname(tradeID);
            tradeDataGroupBox.Text = "Trade Data";
            textDataGroupBox.Text = "Text Data";
            this.Text = "Trade Editor";

            tradeDirty = false;
            textDirty = false;

            Helpers.EnableHandlers();
        }

        private void SaveTradeToFile()
        {
            Helpers.DisableHandlers();
            curTradeData.species = speciesComboBox.SelectedIndex;
            curTradeData.hpIV = (int)hpIVNumericUpDown.Value;
            curTradeData.atkIV = (int)atkIVNumericUpDown.Value;
            curTradeData.defIV = (int)defIVNumericUpDown.Value;
            curTradeData.speedIV = (int)speIVNumericUpDown.Value;
            curTradeData.spAtkIV = (int)spaIVNumericUpDown.Value;
            curTradeData.spDefIV = (int)spdIVNumericUpDown.Value;
            curTradeData.ability = abilityComboBox.SelectedIndex;
            curTradeData.otID = (int)otIDNumericUpDown.Value;
            curTradeData.cool = (int)coolNumericUpDown.Value;
            curTradeData.beauty = (int)beautyNumericUpDown.Value;
            curTradeData.cute = (int)cuteNumericUpDown.Value;
            curTradeData.smart = (int)smartNumericUpDown.Value;
            curTradeData.tough = (int)toughNumericUpDown.Value;
            curTradeData.pid = (int)pidNumericUpDown.Value;
            curTradeData.heldItem = heldItemComboBox.SelectedIndex;
            curTradeData.otGender = otGenderComboBox.SelectedIndex;
            curTradeData.sheen = (int)sheenNumericUpDown.Value;
            curTradeData.language = langComboBox.SelectedIndex;
            curTradeData.requestedSpecies = requestedComboBox.SelectedIndex;

            if (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS)
            {
                curTradeData.unknown = (int)unknownNumericUpDown.Value;
            }
            else
            {
                curTradeData.unknown = 0;
            }

            curTradeData.SaveToFileDefaultDir((int)tradeIDNumericUpDown.Value, false);
            tradeDataGroupBox.Text = "Trade Data";
            tradeDirty = false;
            if (!textDirty)
            {
                this.Text = "Trade Editor";
            }
            AppLogger.Debug($"TradeEditor: Saved trade data for ID {curTradeData.id}.");
            Helpers.EnableHandlers();
        }

        private void SaveTextDataToFile()
        {
            Helpers.DisableHandlers();
            int tradeID = (int)tradeIDNumericUpDown.Value;

            if (tradeID < 0 || tradeID + TradeData.GetTradeCount() > tradeArchive.messages.Count)
            {
                MessageBox.Show("Can't save to text bank. Index is out of range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppLogger.Error("TradeEditor: Can't save to text bank. Index is out of range.");
                Helpers.EnableHandlers();
                return;
            }

            // Nickname
            tradeArchive.messages[tradeID] = nicknameTextBox.Text;

            // OT Name
            tradeArchive.messages[tradeID + TradeData.GetTradeCount()] = otNameTextBox.Text;

            tradeArchive.SaveToFileDefaultDir(GetTextBankIndex(), false);

            // Reset the text data group box and dirty flags
            textDataGroupBox.Text = "Text Data";
            textDirty = false;
            if (!tradeDirty)
            {
                this.Text = "Trade Editor";
            }
            AppLogger.Debug($"TradeEditor: Saved trade text data to message bank {GetTextBankIndex()}");
            Helpers.EnableHandlers();
        }

        private int GetTextBankIndex()
        {
            switch (RomInfo.gameFamily)
            {
                case RomInfo.GameFamilies.DP:
                    switch (RomInfo.gameLanguage)
                    {
                        case RomInfo.GameLanguages.Japanese:
                            return 324;
                        default:
                            return 326;
                    }
                case RomInfo.GameFamilies.Plat:
                    switch (RomInfo.gameLanguage)
                    {
                        case RomInfo.GameLanguages.Japanese:
                            return 369;
                        default:
                            return 370;
                    }
                case RomInfo.GameFamilies.HGSS:
                    switch (RomInfo.gameLanguage)
                    {
                        case RomInfo.GameLanguages.Japanese:
                            return 198;
                        default:
                            return 200;
                    }
                default:
                    AppLogger.Error("TradeEditor: Invalid game family for text bank index retrieval.");
                    MessageBox.Show("Invalid game family for text bank index retrieval.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
            }

        }
        private string GetMonNickname(int tradeID)
        {
            string msg = tradeArchive.messages[tradeID];
            if (msg.Length > nicknameTextBox.MaxLength)
            {
                msg = msg.Substring(0, nicknameTextBox.MaxLength);
            }
            return msg;
        }

        private string GetOTName(int tradeID)
        {
            int index = tradeID + TradeData.GetTradeCount();
            string msg = tradeArchive.messages[index];
            if (msg.Length > otNameTextBox.MaxLength)
            {
                msg = msg.Substring(0, otNameTextBox.MaxLength);
            }
            return msg;
        }

        private void RegisterMarkDirtyHandlers()
        {
            // ComboBoxes
            speciesComboBox.SelectedIndexChanged += MarkDirty;
            abilityComboBox.SelectedIndexChanged += MarkDirty;
            heldItemComboBox.SelectedIndexChanged += MarkDirty;
            otGenderComboBox.SelectedIndexChanged += MarkDirty;
            langComboBox.SelectedIndexChanged += MarkDirty;
            requestedComboBox.SelectedIndexChanged += MarkDirty;

            // NumericUpDowns
            hpIVNumericUpDown.ValueChanged += MarkDirty;
            atkIVNumericUpDown.ValueChanged += MarkDirty;
            defIVNumericUpDown.ValueChanged += MarkDirty;
            speIVNumericUpDown.ValueChanged += MarkDirty;
            spaIVNumericUpDown.ValueChanged += MarkDirty;
            spdIVNumericUpDown.ValueChanged += MarkDirty;
            otIDNumericUpDown.ValueChanged += MarkDirty;
            coolNumericUpDown.ValueChanged += MarkDirty;
            beautyNumericUpDown.ValueChanged += MarkDirty;
            cuteNumericUpDown.ValueChanged += MarkDirty;
            smartNumericUpDown.ValueChanged += MarkDirty;
            toughNumericUpDown.ValueChanged += MarkDirty;
            pidNumericUpDown.ValueChanged += MarkDirty;
            sheenNumericUpDown.ValueChanged += MarkDirty;
            unknownNumericUpDown.ValueChanged += MarkDirty;

            // TextBoxes
            otNameTextBox.TextChanged += MarkDirty;
            nicknameTextBox.TextChanged += MarkDirty;
        }

        #region Handlers

        private void MarkDirty(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Helpers.DisableHandlers();

            if (sender.Equals(otNameTextBox) || sender.Equals(nicknameTextBox))
            {
                textDataGroupBox.Text = "Text Data*";
                this.Text = "Trade Editor*";
                textDirty = true;
            }
            else
            {
                tradeDataGroupBox.Text = "Trade Data*";
                this.Text = "Trade Editor*";
                tradeDirty = true;
            }            

            Helpers.EnableHandlers();
        }

        private void tradeIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Helpers.DisableHandlers();

            if (tradeDirty || textDirty)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save before changing the Trade ID?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    SaveTradeToFile();
                    SaveTextDataToFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    // Revert the change
                    tradeIDNumericUpDown.Value = curTradeData.id;
                    Helpers.EnableHandlers();
                    return;
                }
            }

            int tradeID = (int)tradeIDNumericUpDown.Value;
            if (tradeID < 0 || tradeID >= TradeData.GetTradeCount())
            {
                MessageBox.Show("Invalid Trade ID selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadFromFile(tradeID);

            Helpers.EnableHandlers();

        }

        private void saveTradeButton_Click(object sender, EventArgs e)
        {
            SaveTradeToFile();
        }

        private void saveTextDataButton_Click(object sender, EventArgs e)
        {
            SaveTextDataToFile();
        }

        private void saveAllButton_Click(object sender, EventArgs e)
        {
            SaveTradeToFile();
            SaveTextDataToFile();
        }

        private void TradeEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tradeDirty || textDirty)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    SaveTradeToFile();
                    SaveTextDataToFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // Cancel the closing
                }
            }
        }

        private void addFileButton_Click(object sender, EventArgs e)
        {
            TradeData newTrade = new TradeData(TradeData.GetTradeCount());
            newTrade.SaveToFileDefaultDir(newTrade.id, false);
            tradeIDNumericUpDown.Maximum = TradeData.GetTradeCount() - 1; // Update the maximum value

            // Adjust text archive to accommodate the new trade
            int lastNicknameIndex = TradeData.GetTradeCount() - 1;
            tradeArchive.messages.Insert(lastNicknameIndex, "Nickname");
            tradeArchive.messages.Add("OT Name");
            tradeArchive.SaveToFileDefaultDir(GetTextBankIndex(), false);

            MessageBox.Show($"Added new trade with id {newTrade.id}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void removeLastButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to remove the last trade? This action cannot be undone.", "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
            {
                return; // User cancelled the removal
            }

            if (TradeData.GetTradeCount() <= 1)
            {
                MessageBox.Show("Cannot remove the last trade.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int lastTradeID = TradeData.GetTradeCount() - 1;
            
            try
            {
                File.Delete(RomInfo.gameDirs[RomInfo.DirNames.tradeData].unpackedDir + "\\" + lastTradeID.ToString("D4"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete trade file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppLogger.Error($"TradeEditor: Failed to delete trade file for ID {lastTradeID}: {ex.Message}");
                return;
            }

            tradeIDNumericUpDown.Maximum = lastTradeID - 1; // Update the maximum value
            // Adjust text archive to remove the last trade's text
            
            if (tradeArchive.messages.Count > lastTradeID)
            {
                tradeArchive.messages.RemoveAt(lastTradeID); // Remove Nickname
                tradeArchive.messages.RemoveAt(tradeArchive.messages.Count - 1); // Remove OT Name
                tradeArchive.SaveToFileDefaultDir(GetTextBankIndex(), false);
            }

        }

        #endregion

    }
}

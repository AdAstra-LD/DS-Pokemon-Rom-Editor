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
    public partial class TradeEditor : Form
    {

        private TradeData curTradeData;
        private TextArchive tradeArchive;

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

            InitializeComponent();
            InitLimits();
            InitDataRanges();
            SetToolTips();

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

            tradeArchive = new TextArchive(GetTextBankIndex());
            LoadFromFile(0);
        }

        public void LoadFromFile(int tradeID)
        {
            Helpers.DisableHandlers();
            curTradeData = new TradeData(tradeID);
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

            Helpers.EnableHandlers();
        }

        private void SaveToFile()
        {
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

            curTradeData.SaveToFileDefaultDir((int)tradeIDNumericUpDown.Value, true);
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
            tradeIDNumericUpDown.Maximum = TradeData.GetTradeCount()-1;

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
            requestedComboBox.DataSource = RomInfo.GetPokemonNames();

            // Abilities
            abilityComboBox.DataSource = RomInfo.GetAbilityNames();

            // Held Items
            heldItemComboBox.DataSource = RomInfo.GetItemNames();

            // OT Gender
            otGenderComboBox.DataSource = new string[] {"Male", "Female"};

            // Languages
            langComboBox.DataSource = Enum.GetNames(typeof(OriginLang));
        }

        private void SetToolTips()
        {
            toolTip.SetToolTip(tradeIDNumericUpDown, "The Trade ID to edit.");
            toolTip.SetToolTip(speciesComboBox, "The species of the Pokémon being traded.");
            toolTip.SetToolTip(IVGroupBox, "Individual Values (IVs)");
            toolTip.SetToolTip(abilityComboBox, "The ability of the Pokémon.\nThis value is unused!");
            toolTip.SetToolTip(otIDNumericUpDown, "The Original Trainer ID of the Pokémon.");
            toolTip.SetToolTip(contestGroupBox, "Contest stats for the Pokémon.");
            toolTip.SetToolTip(pidNumericUpDown, "The PID of the Pokémon.");
            toolTip.SetToolTip(heldItemComboBox, "The held item of the Pokémon.");
            toolTip.SetToolTip(otGenderComboBox, "The gender of the Original Trainer (OT).\nThe OT name is determined by the text bank.");
            toolTip.SetToolTip(langComboBox, "The language of origin of the traded Pokémon.");
            toolTip.SetToolTip(requestedComboBox, "The species of the requested Pokémon.\nWhether the Pokémon selected by the player matches this species or not is checked via scripting.");
            toolTip.SetToolTip(unknownNumericUpDown, "An unknown value, present in HGSS only.\nAppears to be unused.");

        }

        private void tradeIDNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            int tradeID = (int)tradeIDNumericUpDown.Value;
            if (tradeID < 0 || tradeID >= TradeData.GetTradeCount())
            {
                MessageBox.Show("Invalid Trade ID selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadFromFile(tradeID);

        }

        private void saveTradeButton_Click(object sender, EventArgs e)
        {
            SaveToFile();
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

        private void saveTextDataButton_Click(object sender, EventArgs e)
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
            Helpers.EnableHandlers();
        }
    }
}

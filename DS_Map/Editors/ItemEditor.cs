using DSPRE.Resources;
using DSPRE.ROMFiles;
using Ekona.Images;
using Images;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using static DSPRE.ROMFiles.ItemData;
using static DSPRE.RomInfo;
using static Images.NCOB.sNCOB;
using MessageBox = System.Windows.Forms.MessageBox;

namespace DSPRE.Editors
{
    struct ItemNarcTableEntry
    {
        public uint itemData;
        public uint itemIcon;
        public uint itemPalette;
        public uint itemAGB;
    };

    public partial class ItemEditor : Form
    {

        private string[] itemFileNames;
        //private readonly string[] itemDescriptions;

        private int currentLoadedId = 0;
        private ItemData currentLoadedFile = null;

        private static bool dirty = false;
        private static readonly string formName = "Item Data Editor";

        private ItemNarcTableEntry[] itemNarcTable;
        private uint itemNarcTableOffset;

        private HashSet<uint> iconIdSet = new HashSet<uint>();
        private HashSet<uint> paletteIdSet = new HashSet<uint>();

        public ItemEditor(string[] itemFileNames) //, string[] itemDescriptions)
         {
            itemNarcTableOffset = (uint)(RomInfo.gameFamily == RomInfo.GameFamilies.HGSS ? 0x100194 : RomInfo.gameFamily == RomInfo.GameFamilies.Plat ? 0xF0CC4 : 0xF85B4);
            itemNarcTable = new ItemNarcTableEntry[itemFileNames.Length];
            List<string> cleanNames = itemFileNames.ToList();
            for (int i = 0; i < itemFileNames.Length; i++)
            {
                ItemNarcTableEntry itemNarcTableEntry = new ItemNarcTableEntry();
                itemNarcTableEntry.itemData = ARM9.ReadWordLE((uint)(itemNarcTableOffset + i*8));
                itemNarcTableEntry.itemIcon = ARM9.ReadWordLE((uint)(itemNarcTableOffset + i * 8 + 2));
                itemNarcTableEntry.itemPalette = ARM9.ReadWordLE((uint)(itemNarcTableOffset + i * 8 + 4));
                itemNarcTableEntry.itemAGB = ARM9.ReadWordLE((uint)(itemNarcTableOffset + i * 8 + 6));
                itemNarcTable[i] = itemNarcTableEntry;
                iconIdSet.Add(itemNarcTableEntry.itemIcon);
                paletteIdSet.Add(itemNarcTableEntry.itemPalette);
            }
            this.itemFileNames = cleanNames.ToArray();
            //this.itemDescriptions = itemDescriptions;

            InitializeComponent();

            Helpers.DisableHandlers();

            // Set up max and min for numerics
            priceNumericUpDown.Minimum = 0;
            priceNumericUpDown.Maximum = 65535;
            itemNumberNumericUpDown.Minimum = 0;
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
            naturalGiftTypeComboBox.Items.AddRange(Enum.GetNames(typeof(NaturalGiftType)));
            fieldFunctionComboBox.Items.AddRange(Enum.GetNames(typeof(FieldUseFunc)));
            battleFunctionComboBox.Items.AddRange(Enum.GetNames(typeof(BattleUseFunc)));

            if (RomInfo.gameFamily == GameFamilies.DP) 
            {
                holdEffectComboBox.Items.RemoveAt((int)HoldEffect.GiratinaBoost); // Effect doesn't exist in DP
            }

            // ItemParameters
            BindItemParamsEvents();
            SetItemParamToolTips();
            SetItemParamRanges();
            PopulateIconPaletteDropdowns();

            Helpers.EnableHandlers();

            itemNameInputComboBox.SelectedIndex = 1;
        }

        private void PopulateIconPaletteDropdowns()
        {
            imageComboBox.BeginUpdate();
            paletteComboBox.BeginUpdate();
            imageComboBox.Items.Clear();
            paletteComboBox.Items.Clear();

            foreach (var icon in iconIdSet.OrderBy(i => i))
                imageComboBox.Items.Add(icon.ToString("D4"));

            foreach (var palette in paletteIdSet.OrderBy(p => p))
                paletteComboBox.Items.Add(palette.ToString("D4"));

            imageComboBox.EndUpdate();
            paletteComboBox.EndUpdate();
        }

        public void UpdateBattlePocketCheckBoxes()
        {
            BattlePocket battlePocket = currentLoadedFile.battlePocket;

            pokeBallsBattlePocketCheck.Checked = (battlePocket & BattlePocket.PokeBalls) != 0;
            battleItemsBattlePocketCheck.Checked = (battlePocket & BattlePocket.BattleItems) != 0;
            hpRestoreBattlePocketCheck.Checked = (battlePocket & BattlePocket.HpRestore) != 0;
            statusHealersBattlePocketCheck.Checked = (battlePocket & BattlePocket.StatusHealers) != 0;
            ppRestoreBattlePocketCheck.Checked = (battlePocket & BattlePocket.PpRestore) != 0;
        }


        private void BindItemParamsEvents()
        {
            foreach (var ctrl in new CheckBox[] {
                slpHealCheckBox, psnHealCheckBox, brnHealCheckBox, frzHealCheckBox,
                przHealCheckBox, cfsHealCheckBox, infHealCheckBox, guardSpecCheckBox,
                reviveCheckBox, reviveAllCheckBox, levelUpCheckBox, evolveCheckBox,
                hpRestoreCheckBox, ppRestoreCheckBox, ppUpsCheckBox, ppMaxCheckBox, ppRestoreAllCheckBox,
                evHpCheckBox, evAtkCheckBox, evDefCheckBox, evSpeedCheckBox, evSpAtkCheckBox, evSpDefCheckBox,
                friendshipLowCheckBox, friendshipMidCheckBox, friendshipHighCheckBox
            }) ctrl.CheckedChanged += PartyParamsControlChanged;

            foreach (var num in new NumericUpDown[] {
                atkStagesNumeric, defStagesNumeric, spAtkStagesNumeric, spDefStagesNumeric,
                speedStagesNumeric, accuracyStagesNumeric, critRateStagesNumeric,
                hpRestoreParamNumeric, ppRestoreParamNumeric,
                evHpValueNumeric, evAtkValueNumeric, evDefValueNumeric,
                evSpeedValueNumeric, evSpAtkValueNumeric, evSpDefValueNumeric,
                friendshipLowValueNumeric, friendshipMidValueNumeric, friendshipHighValueNumeric
            }) num.ValueChanged += PartyParamsControlChanged;
        }

        private void SetItemParamRanges()
        {
            // Stat stage modifiers: range -6 to +6 in gameplay, stored as 0–15 in data (encoded with +6 offset)
            foreach (var n in new NumericUpDown[] {
                atkStagesNumeric, defStagesNumeric, spAtkStagesNumeric,
                spDefStagesNumeric, speedStagesNumeric, accuracyStagesNumeric })
            {
                n.Minimum = -6;
                n.Maximum = 6;
            }

            // HP and PP restore parameters: stored as byte (0–255)
            foreach (var n in new NumericUpDown[] {
                hpRestoreParamNumeric, ppRestoreParamNumeric })
            {
                n.Minimum = 0;
                n.Maximum = 255;
            }

            // EV and friendship modifiers: stored as sbyte (-128 to +127)
            foreach (var n in new NumericUpDown[] {
                evHpValueNumeric, evAtkValueNumeric, evDefValueNumeric,
                evSpeedValueNumeric, evSpAtkValueNumeric, evSpDefValueNumeric,
                friendshipLowValueNumeric, friendshipMidValueNumeric, friendshipHighValueNumeric })
            {
                n.Minimum = -128;
                n.Maximum = 127;
            }

            // Crit rate stages: 2-bit unsigned, range 0–3
            critRateStagesNumeric.Minimum = 0;
            critRateStagesNumeric.Maximum = 3;
        }
        


        private void SetItemParamToolTips()
        {
            toolTip1.SetToolTip(slpHealCheckBox, "Cures sleep status");
            toolTip1.SetToolTip(psnHealCheckBox, "Cures poison status");
            toolTip1.SetToolTip(brnHealCheckBox, "Cures burn status");
            toolTip1.SetToolTip(frzHealCheckBox, "Cures freeze status");
            toolTip1.SetToolTip(przHealCheckBox, "Cures paralysis status");
            toolTip1.SetToolTip(cfsHealCheckBox, "Cures confusion");
            toolTip1.SetToolTip(infHealCheckBox, "Cures infatuation");
            toolTip1.SetToolTip(guardSpecCheckBox, "Applies Mist effect to party (Guard Spec)");

            toolTip1.SetToolTip(reviveCheckBox, "Revives a fainted Pokémon to 50% HP");
            toolTip1.SetToolTip(reviveAllCheckBox, "Revives all fainted Pokémon in party");
            toolTip1.SetToolTip(levelUpCheckBox, "Causes the Pokémon to level up");
            toolTip1.SetToolTip(evolveCheckBox, "Causes the Pokémon to evolve");

            toolTip1.SetToolTip(atkStagesNumeric, "Boost to Attack stat (in stages)");
            toolTip1.SetToolTip(defStagesNumeric, "Boost to Defense stat");
            toolTip1.SetToolTip(spAtkStagesNumeric, "Boost to Special Attack stat");
            toolTip1.SetToolTip(spDefStagesNumeric, "Boost to Special Defense stat");
            toolTip1.SetToolTip(speedStagesNumeric, "Boost to Speed stat");
            toolTip1.SetToolTip(accuracyStagesNumeric, "Boost to Accuracy stat");
            toolTip1.SetToolTip(critRateStagesNumeric, "Boost to Critical Hit rate");

            toolTip1.SetToolTip(hpRestoreCheckBox, "Restores HP by parameter value");
            toolTip1.SetToolTip(hpRestoreParamNumeric, "Amount of HP to restore (0 = use %)");
            toolTip1.SetToolTip(ppRestoreCheckBox, "Restores PP by parameter value");
            toolTip1.SetToolTip(ppRestoreParamNumeric, "Amount of PP to restore");
            toolTip1.SetToolTip(ppUpsCheckBox, "Applies PP Up");
            toolTip1.SetToolTip(ppMaxCheckBox, "Applies PP Max");
            toolTip1.SetToolTip(ppRestoreAllCheckBox, "Restores PP for all moves");

            toolTip1.SetToolTip(evHpCheckBox, "Adds EVs to HP");
            toolTip1.SetToolTip(evAtkCheckBox, "Adds EVs to Attack");
            toolTip1.SetToolTip(evDefCheckBox, "Adds EVs to Defense");
            toolTip1.SetToolTip(evSpeedCheckBox, "Adds EVs to Speed");
            toolTip1.SetToolTip(evSpAtkCheckBox, "Adds EVs to Special Attack");
            toolTip1.SetToolTip(evSpDefCheckBox, "Adds EVs to Special Defense");

            toolTip1.SetToolTip(evHpValueNumeric, "EV value to apply to HP");
            toolTip1.SetToolTip(evAtkValueNumeric, "EV value to apply to Attack");
            toolTip1.SetToolTip(evDefValueNumeric, "EV value to apply to Defense");
            toolTip1.SetToolTip(evSpeedValueNumeric, "EV value to apply to Speed");
            toolTip1.SetToolTip(evSpAtkValueNumeric, "EV value to apply to Special Attack");
            toolTip1.SetToolTip(evSpDefValueNumeric, "EV value to apply to Special Defense");

            toolTip1.SetToolTip(friendshipLowCheckBox, "Affects low-level friendship modification");
            toolTip1.SetToolTip(friendshipMidCheckBox, "Affects mid-level friendship modification");
            toolTip1.SetToolTip(friendshipHighCheckBox, "Affects high-level friendship modification");

            toolTip1.SetToolTip(friendshipLowValueNumeric, "Friendship change for low base friendship");
            toolTip1.SetToolTip(friendshipMidValueNumeric, "Friendship change for mid base friendship");
            toolTip1.SetToolTip(friendshipHighValueNumeric, "Friendship change for high base friendship");
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
            AppLogger.Debug("ItemEditor: ChangeLoadedFile: toLoad = " + toLoad);
            currentLoadedId = toLoad;

            var stream = new FileStream(RomInfo.gameDirs[DirNames.itemData].unpackedDir + "\\" 
                + itemNarcTable[toLoad].itemData.ToString("D4"), FileMode.Open);

            currentLoadedFile = new ItemData(stream, toLoad);
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
            UpdateBattlePocketCheckBoxes();

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
            partyUseCheckBox.Checked = currentLoadedFile.PartyUse == 1;

            // Price
            priceNumericUpDown.Value = currentLoadedFile.price;

            // Usage Functions
            if (Enum.IsDefined(typeof(FieldUseFunc), currentLoadedFile.fieldUseFunc))
            {
                fieldFunctionComboBox.SelectedIndex = (int)currentLoadedFile.fieldUseFunc;
            }
            else
            {
                // Add the unknown value if it's not already in the list
                string unknownValue = $"Unknown ({(int)currentLoadedFile.fieldUseFunc})";
                if (!fieldFunctionComboBox.Items.Contains(unknownValue))
                {
                    fieldFunctionComboBox.Items.Add(unknownValue);
                }
                fieldFunctionComboBox.SelectedItem = unknownValue;
            }

            if (Enum.IsDefined(typeof(BattleUseFunc), currentLoadedFile.battleUseFunc))
            {
                battleFunctionComboBox.SelectedIndex = (int)currentLoadedFile.battleUseFunc;
            }
            else
            {
                string unknownValue = $"Unknown ({(int)currentLoadedFile.battleUseFunc})";
                if (!battleFunctionComboBox.Items.Contains(unknownValue))
                {
                    battleFunctionComboBox.Items.Add(unknownValue);
                }
                battleFunctionComboBox.SelectedItem = unknownValue;
            }

            itemParamsTabControl.Enabled = partyUseCheckBox.Checked;
            PopulateItemPartyParamsUI();

            var entry = itemNarcTable[currentLoadedFile.ID];

            string iconID = entry.itemIcon.ToString("D4");
            string paletteID = entry.itemPalette.ToString("D4");

            if (imageComboBox.Items.Contains(iconID))
            {
                imageComboBox.SelectedItem = iconID;
            }

            if (paletteComboBox.Items.Contains(paletteID))
            {
                paletteComboBox.SelectedItem = paletteID;
            }

            SetUpIcon();

        }

        private void SetUpIcon()
        {
            var itemIconId = itemNarcTable[currentLoadedFile.ID].itemIcon;
            var itemPaletteId = itemNarcTable[currentLoadedFile.ID].itemPalette;

            string paletteFilename = itemPaletteId.ToString("D4");
            var itemPalette = new NCLR(gameDirs[DirNames.itemIcons].unpackedDir + "\\" + paletteFilename, (int)itemPaletteId, paletteFilename);

            string spriteFilename = itemIconId.ToString("D4");
            ImageBase imageBase = new NCGR(gameDirs[DirNames.itemIcons].unpackedDir + "\\" + spriteFilename, (int)itemIconId, spriteFilename);

            int ncerFileId = 1; // there's only one ncer in pt (0001.ncer), probably the case in hg too, but is it the 0001?
            string ncerFileName = ncerFileId.ToString("D4");
            SpriteBase spriteBase = new NCER(gameDirs[DirNames.itemIcons].unpackedDir + "\\" + ncerFileName, 2, ncerFileName);

            try
            {
                itemEditorSelectedPictureBox.Image = spriteBase.Get_Image(imageBase, itemPalette, 0, imageBase.Width, imageBase.Height, false, false, false, true, true, -1);
                itemEditorSelectedPictureBox.Height = imageBase.Height;
                itemEditorSelectedPictureBox.Width = imageBase.Width;
            }
            catch (FormatException)
            {
                itemEditorSelectedPictureBox.Image = Properties.Resources.IconItem;
            }
        }

        private void PopulateItemPartyParamsUI()
        {
            ResetItemPartyParamsUI();
            ItemPartyUseParam param = currentLoadedFile.PartyUseParam;
            slpHealCheckBox.Checked = param.SlpHeal;
            psnHealCheckBox.Checked = param.PsnHeal;
            brnHealCheckBox.Checked = param.BrnHeal;
            frzHealCheckBox.Checked = param.FrzHeal;
            przHealCheckBox.Checked = param.PrzHeal;
            cfsHealCheckBox.Checked = param.CfsHeal;
            infHealCheckBox.Checked = param.InfHeal;
            guardSpecCheckBox.Checked = param.GuardSpec;

            reviveCheckBox.Checked = param.Revive;
            reviveAllCheckBox.Checked = param.ReviveAll;
            levelUpCheckBox.Checked = param.LevelUp;
            evolveCheckBox.Checked = param.Evolve;

            atkStagesNumeric.Value = param.AtkStages;
            defStagesNumeric.Value = param.DefStages;
            spAtkStagesNumeric.Value = param.SpAtkStages;
            spDefStagesNumeric.Value = param.SpDefStages;
            speedStagesNumeric.Value = param.SpeedStages;
            accuracyStagesNumeric.Value = param.AccuracyStages;
            critRateStagesNumeric.Value = param.CritRateStages;
            hpRestoreCheckBox.Checked = param.HPRestore;
            hpRestoreParamNumeric.Value = param.HPRestoreParam;
            ppRestoreCheckBox.Checked = param.PPRestore;
            ppRestoreParamNumeric.Value = param.PPRestoreParam;
            ppUpsCheckBox.Checked = param.PPUps;
            ppMaxCheckBox.Checked = param.PPMax;
            ppRestoreAllCheckBox.Checked = param.PPRestoreAll;

            evHpCheckBox.Checked = param.EVHp;
            evAtkCheckBox.Checked = param.EVAtk;
            evDefCheckBox.Checked = param.EVDef;
            evSpeedCheckBox.Checked = param.EVSpeed;
            evSpAtkCheckBox.Checked = param.EVSpAtk;
            evSpDefCheckBox.Checked = param.EVSpDef;

            evHpValueNumeric.Value = param.EVHpValue;
            evAtkValueNumeric.Value = param.EVAtkValue;
            evDefValueNumeric.Value = param.EVDefValue;
            evSpeedValueNumeric.Value = param.EVSpeedValue;
            evSpAtkValueNumeric.Value = param.EVSpAtkValue;
            evSpDefValueNumeric.Value = param.EVSpDefValue;

            friendshipLowCheckBox.Checked = param.FriendshipLow;
            friendshipMidCheckBox.Checked = param.FriendshipMid;
            friendshipHighCheckBox.Checked = param.FriendshipHigh;

            friendshipLowValueNumeric.Value = param.FriendshipLowValue;
            friendshipMidValueNumeric.Value = param.FriendshipMidValue;
            friendshipHighValueNumeric.Value = param.FriendshipHighValue;
            Update();
        }

        private void ResetItemPartyParamsUI()
        {
            // Uncheck all CheckBoxes
            slpHealCheckBox.Checked = false;
            psnHealCheckBox.Checked = false;
            brnHealCheckBox.Checked = false;
            frzHealCheckBox.Checked = false;
            przHealCheckBox.Checked = false;
            cfsHealCheckBox.Checked = false;
            infHealCheckBox.Checked = false;
            guardSpecCheckBox.Checked = false;

            reviveCheckBox.Checked = false;
            reviveAllCheckBox.Checked = false;
            levelUpCheckBox.Checked = false;
            evolveCheckBox.Checked = false;

            hpRestoreCheckBox.Checked = false;
            ppRestoreCheckBox.Checked = false;
            ppUpsCheckBox.Checked = false;
            ppMaxCheckBox.Checked = false;
            ppRestoreAllCheckBox.Checked = false;

            evHpCheckBox.Checked = false;
            evAtkCheckBox.Checked = false;
            evDefCheckBox.Checked = false;
            evSpeedCheckBox.Checked = false;
            evSpAtkCheckBox.Checked = false;
            evSpDefCheckBox.Checked = false;

            friendshipLowCheckBox.Checked = false;
            friendshipMidCheckBox.Checked = false;
            friendshipHighCheckBox.Checked = false;

            // Reset all NumericUpDowns to 0
            atkStagesNumeric.Value = 0;
            defStagesNumeric.Value = 0;
            spAtkStagesNumeric.Value = 0;
            spDefStagesNumeric.Value = 0;
            speedStagesNumeric.Value = 0;
            accuracyStagesNumeric.Value = 0;
            critRateStagesNumeric.Value = 0;

            hpRestoreParamNumeric.Value = 0;
            ppRestoreParamNumeric.Value = 0;

            evHpValueNumeric.Value = 0;
            evAtkValueNumeric.Value = 0;
            evDefValueNumeric.Value = 0;
            evSpeedValueNumeric.Value = 0;
            evSpAtkValueNumeric.Value = 0;
            evSpDefValueNumeric.Value = 0;

            friendshipLowValueNumeric.Value = 0;
            friendshipMidValueNumeric.Value = 0;
            friendshipHighValueNumeric.Value = 0;

            Update();
        }


        private void saveDataButton_Click(object sender, EventArgs e)
        {
            currentLoadedFile.SaveToFileDefaultDir((int)itemNarcTable[currentLoadedFile.ID].itemData, true);
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
                AppLogger.Debug("ItemEditor: itemNameInputComboBox_SelectedIndexChanged: newId = " + newId);
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
                AppLogger.Debug("ItemEditor: itemNumberNumericUpDown_ValueChanged: newId = " + newId);
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

            currentLoadedFile.naturalGiftType = (NaturalGiftType)Enum.Parse(typeof(NaturalGiftType), (string)naturalGiftTypeComboBox.SelectedItem);
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

        private void fieldFunctionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            string selectedValue = fieldFunctionComboBox.SelectedItem.ToString();
            if (selectedValue.StartsWith("Unknown ("))
            {
                // Parse the numeric value from "Unknown (X)"
                string numericPart = selectedValue.Substring(8, selectedValue.Length - 9);
                if (byte.TryParse(numericPart, out byte value))
                {
                    currentLoadedFile.fieldUseFunc = (FieldUseFunc)value;
                }
            }
            else
            {
                currentLoadedFile.fieldUseFunc = (FieldUseFunc)fieldFunctionComboBox.SelectedIndex;
            }
            setDirty(true);
        }

        private void battleFunctionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            string selectedValue = battleFunctionComboBox.SelectedItem.ToString();
            if (selectedValue.StartsWith("Unknown ("))
            {
                // Parse the numeric value from "Unknown (X)"
                string numericPart = selectedValue.Substring(8, selectedValue.Length - 9);
                if (byte.TryParse(numericPart, out byte value))
                {
                    currentLoadedFile.battleUseFunc = (BattleUseFunc)value;
                }
            }
            else
            {
                currentLoadedFile.battleUseFunc = (BattleUseFunc)battleFunctionComboBox.SelectedIndex;
            }
            setDirty(true);
        }

        private void partyUseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentLoadedFile.PartyUse = (byte)(partyUseCheckBox.Checked ? 1: 0);
            itemParamsTabControl.Enabled = partyUseCheckBox.Checked;
            setDirty(true);
        }

        private void PartyParamsControlChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            if (currentLoadedFile == null) return;

            ItemPartyUseParam param = currentLoadedFile.PartyUseParam;

            // Status heals
            param.SlpHeal = slpHealCheckBox.Checked;
            param.PsnHeal = psnHealCheckBox.Checked;
            param.BrnHeal = brnHealCheckBox.Checked;
            param.FrzHeal = frzHealCheckBox.Checked;
            param.PrzHeal = przHealCheckBox.Checked;
            param.CfsHeal = cfsHealCheckBox.Checked;
            param.InfHeal = infHealCheckBox.Checked;
            param.GuardSpec = guardSpecCheckBox.Checked;

            // Revive, evolve, level
            param.Revive = reviveCheckBox.Checked;
            param.ReviveAll = reviveAllCheckBox.Checked;
            param.LevelUp = levelUpCheckBox.Checked;
            param.Evolve = evolveCheckBox.Checked;

            // Stat stages
            param.AtkStages = (sbyte)atkStagesNumeric.Value;
            param.DefStages = (sbyte)defStagesNumeric.Value;
            param.SpAtkStages = (sbyte)spAtkStagesNumeric.Value;
            param.SpDefStages = (sbyte)spDefStagesNumeric.Value;
            param.SpeedStages = (sbyte)speedStagesNumeric.Value;
            param.AccuracyStages = (sbyte)accuracyStagesNumeric.Value;
            param.CritRateStages = (sbyte)critRateStagesNumeric.Value;

            // Restore
            param.HPRestore = hpRestoreCheckBox.Checked;
            param.HPRestoreParam = (byte)hpRestoreParamNumeric.Value;
            param.PPRestore = ppRestoreCheckBox.Checked;
            param.PPRestoreParam = (byte)ppRestoreParamNumeric.Value;
            param.PPUps = ppUpsCheckBox.Checked;
            param.PPMax = ppMaxCheckBox.Checked;
            param.PPRestoreAll = ppRestoreAllCheckBox.Checked;

            // EVs
            param.EVHp = evHpCheckBox.Checked;
            param.EVAtk = evAtkCheckBox.Checked;
            param.EVDef = evDefCheckBox.Checked;
            param.EVSpeed = evSpeedCheckBox.Checked;
            param.EVSpAtk = evSpAtkCheckBox.Checked;
            param.EVSpDef = evSpDefCheckBox.Checked;

            param.EVHpValue = (sbyte)evHpValueNumeric.Value;
            param.EVAtkValue = (sbyte)evAtkValueNumeric.Value;
            param.EVDefValue = (sbyte)evDefValueNumeric.Value;
            param.EVSpeedValue = (sbyte)evSpeedValueNumeric.Value;
            param.EVSpAtkValue = (sbyte)evSpAtkValueNumeric.Value;
            param.EVSpDefValue = (sbyte)evSpDefValueNumeric.Value;

            // Friendship
            param.FriendshipLow = friendshipLowCheckBox.Checked;
            param.FriendshipMid = friendshipMidCheckBox.Checked;
            param.FriendshipHigh = friendshipHighCheckBox.Checked;

            param.FriendshipLowValue = (sbyte)friendshipLowValueNumeric.Value;
            param.FriendshipMidValue = (sbyte)friendshipMidValueNumeric.Value;
            param.FriendshipHighValue = (sbyte)friendshipHighValueNumeric.Value;

            setDirty(true);
        }


        private void imageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || imageComboBox.SelectedItem == null) return;

            uint newIconID = uint.Parse(imageComboBox.SelectedItem.ToString());
            itemNarcTable[currentLoadedFile.ID].itemIcon = newIconID;

            SetUpIcon();
            setDirty(true);
        }

        private void paletteComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || paletteComboBox.SelectedItem == null) return;

            uint newPaletteID = uint.Parse(paletteComboBox.SelectedItem.ToString());
            itemNarcTable[currentLoadedFile.ID].itemPalette = newPaletteID;

            SetUpIcon();
            setDirty(true);
        }

        private void saveIconButton_Click(object sender, EventArgs e)
        {
            if(Helpers.HandlersDisabled)
            {
                return;
            }

            for (int i = 0; i < itemNarcTable.Length; i++)
            {
                ItemNarcTableEntry itemNarcTableEntry = new ItemNarcTableEntry();
                itemNarcTableEntry.itemData = ARM9.ReadWordLE((uint)(itemNarcTableOffset + i * 8));
                itemNarcTableEntry.itemIcon = ARM9.ReadWordLE((uint)(itemNarcTableOffset + i * 8 + 2));
                if (itemNarcTable[i].itemIcon != itemNarcTableEntry.itemIcon)
                {
                    byte[] bytes = BitConverter.GetBytes((ushort)itemNarcTableEntry.itemIcon);
                    ARM9.WriteBytes(bytes, itemNarcTableOffset + (uint)(currentLoadedFile.ID * 8 + 2));
                }   
                itemNarcTableEntry.itemPalette = ARM9.ReadWordLE((uint)(itemNarcTableOffset + i * 8 + 4));
                if (itemNarcTable[i].itemPalette != itemNarcTableEntry.itemPalette)
                {
                    byte[] bytes = BitConverter.GetBytes((ushort)itemNarcTableEntry.itemPalette);
                    ARM9.WriteBytes(bytes, itemNarcTableOffset + (uint)(currentLoadedFile.ID * 8 + 4));
                }
                itemNarcTableEntry.itemAGB = ARM9.ReadWordLE((uint)(itemNarcTableOffset + i * 8 + 6));
            }
            setDirty(false);
        }

        private void BattlePocketCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            // Build battlePocket from checkbox states
            BattlePocket battlePocket = BattlePocket.None;
            if (pokeBallsBattlePocketCheck.Checked)
                battlePocket |= BattlePocket.PokeBalls;
            if (battleItemsBattlePocketCheck.Checked)
                battlePocket |= BattlePocket.BattleItems;
            if (hpRestoreBattlePocketCheck.Checked)
                battlePocket |= BattlePocket.HpRestore;
            if (statusHealersBattlePocketCheck.Checked)
                battlePocket |= BattlePocket.StatusHealers;
            if (ppRestoreBattlePocketCheck.Checked)
                battlePocket |= BattlePocket.PpRestore;

            currentLoadedFile.battlePocket = battlePocket;
            setDirty(true);
        }
    }
}

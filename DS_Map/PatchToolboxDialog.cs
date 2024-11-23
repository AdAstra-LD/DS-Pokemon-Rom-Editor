using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Linq;
using DSPRE.ROMFiles;
using System.Collections.Generic;
using DSPRE.Resources.ROMToolboxDB;
using DSPRE.Resources;
using static DSPRE.RomInfo;
using System.Threading.Tasks;
using static DSPRE.Resources.ROMToolboxDB.ToolboxDB;
using static NSMBe4.ROM;

namespace DSPRE
{
    public partial class PatchToolboxDialog : Form
    {
        public static uint expandedARMfileID = ToolboxDB.syntheticOverlayFileNumbersDB[RomInfo.gameFamily];

        public static bool flag_standardizedItems { get; private set; } = false;
        public static bool flag_arm9Expanded { get; private set; } = false;
        public static bool flag_BDHCamPatchApplied { get; private set; } = false;
        public static bool flag_DynamicHeadersPatchApplied { get; private set; } = false;
        public static bool flag_MatrixExpansionApplied { get; private set; } = false;

        public static bool flag_MainComboTableRepointed { get; set; } = false;
        public static bool flag_TrainerClassBattleTableRepointed { get; set; } = false;
        public static bool flag_PokemonBattleTableRepointed { get; set; } = false;
        public static bool flag_TrainerNamesExpanded { get; set; } = false;

        public static bool overlay1MustBeRestoredFromBackup { get; private set; } = true;

        public static readonly int expandedTrainerNameLength = 12;

        #region Constructor

        public PatchToolboxDialog()
        {
            InitializeComponent();

            CheckStandardizedItems();

            if (RomInfo.gameLanguage == GameLanguages.English || RomInfo.gameLanguage == GameLanguages.Spanish)
            {
                CheckARM9ExpansionApplied();
            }
            else
            {
                DisableARM9patch("Unsupported\nlanguage");
                DisableBDHCamPatch("Unsupported\nlanguage");
                DisableScrcmdRepointPatch("Unsupported\nlanguage");
            }

            CheckExpandedTrainerNamesPatchApplied();

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    DisableOverlay1patch("Unsupported");
                    DisableDynamicHeadersPatch("Unsupported");
                    DisableMatrixExpansionPatch("Unsupported");
                    DisableScrcmdRepointPatch("Unsupported");
                    DisableKillTextureAnimationsPatch("Unsupported");
                    break;

                case GameFamilies.Plat:
                    DisableOverlay1patch("Unsupported");
                    DisableMatrixExpansionPatch("Unsupported");
                    DisableScrcmdRepointPatch("Unsupported");
                    DisableKillTextureAnimationsPatch("Unsupported");

                    if (RomInfo.gameLanguage == GameLanguages.English || RomInfo.gameLanguage == GameLanguages.Spanish)
                    {
                        CheckBDHCamPatchApplied();
                    }
                    CheckDynamicHeadersPatchApplied();
                    break;

                case GameFamilies.HGSS:
                    if (!OverlayUtils.OverlayTable.IsDefaultCompressed(1))
                    {
                        DisableOverlay1patch("Already applied");
                        overlay1CB.Visible = true;
                    }

                    if (RomInfo.gameLanguage == GameLanguages.English || RomInfo.gameLanguage == GameLanguages.Spanish)
                    {
                        CheckBDHCamPatchApplied();
                        CheckMatrixExpansionApplied();
                        CheckScrcmdRepointPatchApplied();
                    }
                    else
                    {
                        DisableMatrixExpansionPatch("Unsupported\nlanguage");
                        DisableScrcmdRepointPatch("Unsupported\nlanguage");
                    }

                    CheckDynamicHeadersPatchApplied();
                    break;
            }
        }

        #region Patch Disable

        private void DisableOverlay1patch(string reason)
        {
            overlay1uncomprButton.Enabled = false;
            overlay1uncompressedLBL.Enabled = false;
            overlay1patchtextLBL.Enabled = false;
            overlay1uncomprButton.Text = reason;
        }

        private void DisableBDHCamPatch(string reason)
        {
            BDHCamPatchButton.Enabled = false;
            BDHCamPatchLBL.Enabled = false;
            BDHCamPatchTextLBL.Enabled = false;
            BDHCamARM9requiredLBL.Enabled = false;
            BDHCamPatchButton.Text = reason;
        }

        private void DisableARM9patch(string reason)
        {
            applyARM9ExpansionButton.Enabled = false;
            arm9expansionTextLBL.Enabled = false;
            arm9expansionLBL.Enabled = false;
            applyARM9ExpansionButton.Text = reason;
        }

        private void DisableDynamicHeadersPatch(string reason)
        {
            applyDynamicHeadersButton.Enabled = false;
            dynamicHeadersTextLBL.Enabled = false;
            dynamicHeadersLBL.Enabled = false;
            applyDynamicHeadersButton.Text = reason;
        }

        private void DisableMatrixExpansionPatch(string reason)
        {
            expandMatrixButton.Enabled = false;
            matrixExpansionLBL.Enabled = false;
            matrixExpansionTextLBL.Enabled = false;
            expandMatrixButton.Text = reason;
        }

        private void DisableStandardizeItemsPatch(string reason)
        {
            applyItemStandardizeButton.Enabled = false;
            standardizePatchLBL.Enabled = false;
            standardizePatchTextLBL.Enabled = false;
            applyItemStandardizeButton.Text = reason;
        }

        private void DisableScrcmdRepointPatch(string reason)
        {
            repointScrcmdButton.Enabled = false;
            repointScrcmdLBL.Enabled = false;
            repointScrcmdTextLBL.Enabled = false;
            scrcmdARM9requiredLBL.Enabled = false;
            repointScrcmdButton.Text = reason;
        }

        private void DisableKillTextureAnimationsPatch(string reason)
        {
            disableTextureAnimationsButton.Enabled = false;
            disableTextureAnimationsLBL.Enabled = false;
            disableTextureAnimationsTextLBL.Enabled = false;
            disableTextureAnimationsButton.Text = reason;
        }

        private void DisableTrainerNameExpansionPatch(string reason)
        {
            expandTrainerNamesButton.Enabled = false;
            expandTrainerNamesLBL.Enabled = false;
            expandTrainerNamesTextLBL.Enabled = false;
            expandTrainerNamesButton.Text = reason;
        }

        #endregion Patch Disable

        #endregion Constructor

        #region Patch

        private static bool CheckFilesArm9ExpansionApplied()
        {
            ARM9PatchData data = new ARM9PatchData();

            byte[] branchCode = DSUtils.HexStringToByteArray(data.branchString);
            byte[] branchCodeRead = ARM9.ReadBytes(data.branchOffset, data.branchString.Length / 3 + 1); //Read branchCode
            if (branchCodeRead.Length != branchCode.Length || !branchCodeRead.SequenceEqual(branchCode))
                return false;

            byte[] initCode = DSUtils.HexStringToByteArray(data.initString);
            byte[] initCodeRead = ARM9.ReadBytes(data.initOffset, data.initString.Length / 3 + 1); //Read initCode
            if (initCodeRead.Length != initCode.Length || !initCodeRead.SequenceEqual(initCode))
                return false;

            return true;
        }

        public static bool CheckFilesBDHCamPatchApplied()
        {
            BDHCAMPatchData data = new BDHCAMPatchData();

            byte[] branchCode = DSUtils.HexStringToByteArray(data.branchString);
            byte[] branchCodeRead = ARM9.ReadBytes(data.branchOffset, branchCode.Length);

            if (branchCode.Length != branchCodeRead.Length || !branchCode.SequenceEqual(branchCodeRead))
            {
                return false;
            }

            string overlayFilePath = OverlayUtils.GetPath(data.overlayNumber);
            OverlayUtils.Decompress(data.overlayNumber);

            byte[] overlayCode1 = DSUtils.HexStringToByteArray(data.overlayString1);
            byte[] overlayCode1Read = DSUtils.ReadFromFile(overlayFilePath, data.overlayOffset1, overlayCode1.Length);
            if (overlayCode1.Length != overlayCode1Read.Length || !overlayCode1.SequenceEqual(overlayCode1Read))
                return false;

            byte[] overlayCode2 = DSUtils.HexStringToByteArray(data.overlayString2);
            byte[] overlayCode2Read = DSUtils.ReadFromFile(overlayFilePath, data.overlayOffset2, overlayCode2.Length); //Write new overlayCode1
            if (overlayCode2.Length != overlayCode2Read.Length || !overlayCode2.SequenceEqual(overlayCode2Read))
                return false; //0 means BDHCAM patch has not been applied

            String fullFilePath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + PatchToolboxDialog.expandedARMfileID.ToString("D4");
            byte[] subroutineRead = DSUtils.ReadFromFile(fullFilePath, BDHCAMPatchData.BDHCamSubroutineOffset, data.subroutine.Length); //Write new overlayCode1
            if (data.subroutine.Length != subroutineRead.Length || !data.subroutine.SequenceEqual(subroutineRead))
                return false; //0 means BDHCAM patch has not been applied

            return true;
        }

        public static bool CheckFilesMatrixExpansionApplied()
        {
            foreach (KeyValuePair<uint[], string> kv in ToolboxDB.matrixExpansionDB)
            {
                foreach (uint offset in kv.Key)
                {
                    int languageOffset = 0;
                    if (RomInfo.romID == "IPKE" || RomInfo.romID == "IPGE" || RomInfo.romID == "IPGS")
                    {
                        languageOffset = +8;
                    }

                    byte[] read = ARM9.ReadBytes((uint)(offset - ARM9.address + languageOffset), kv.Value.Length / 3 + 1);
                    byte[] code = DSUtils.HexStringToByteArray(kv.Value);
                    if (read.Length != code.Length || !read.SequenceEqual(code))
                        return false;
                }
            }
            return true;
        }

        public static bool CheckScriptsStandardizedItemNumbers()
        {
            ScriptFile itemScript = new ScriptFile(RomInfo.itemScriptFileNumber);
            if (itemScript.allScripts.Count - 1 < new TextArchive(RomInfo.itemNamesTextNumber).messages.Count)
            {
                return false;
            }

            for (ushort i = 0; i < itemScript.allScripts.Count - 1; i++)
            {
                if (BitConverter.ToUInt16(itemScript.allScripts[i].commands[0].cmdParams[1], 0) != i || BitConverter.ToUInt16(itemScript.allScripts[i].commands[1].cmdParams[1], 0) != 1)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckStandardizedItems()
        {
            DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.scripts });

            if (!PatchToolboxDialog.flag_standardizedItems)
            {
                if (!PatchToolboxDialog.CheckScriptsStandardizedItemNumbers())
                {
                    return false;
                }
            }

            itemNumbersCB.Visible = true;
            PatchToolboxDialog.flag_standardizedItems = true;

            DisableStandardizeItemsPatch("Already applied");
            return true;
        }

        public bool CheckMatrixExpansionApplied()
        {
            if (!PatchToolboxDialog.flag_MatrixExpansionApplied)
            {
                if (!PatchToolboxDialog.CheckFilesMatrixExpansionApplied())
                {
                    return false;
                }
            }

            DisableMatrixExpansionPatch("Already applied");
            PatchToolboxDialog.flag_MatrixExpansionApplied = true;
            expandedMatrixCB.Visible = true;
            return true;
        }

        public string backupSuffix = ".backup";

        private bool CheckARM9ExpansionApplied()
        {
            if (!PatchToolboxDialog.flag_arm9Expanded)
            {
                if (!PatchToolboxDialog.CheckFilesArm9ExpansionApplied())
                {
                    return false;
                }
            }

            PatchToolboxDialog.flag_arm9Expanded = true;
            arm9patchCB.Visible = true;
            DisableARM9patch("Already applied");

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.Plat:
                case GameFamilies.HGSS:
                    BDHCamARM9requiredLBL.Visible = false;
                    BDHCamPatchButton.Enabled = true;
                    BDHCamPatchLBL.Enabled = true;
                    BDHCamPatchTextLBL.Enabled = true;
                    break;
            }

            return true;
        }

        public bool CheckDynamicHeadersPatchApplied()
        {
            if (!flag_DynamicHeadersPatchApplied)
            {
                if (!PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
                {
                    return false;
                }
            }

            PatchToolboxDialog.flag_DynamicHeadersPatchApplied = true;
            dynamicHeadersPatchCB.Visible = true;

            DisableDynamicHeadersPatch("Already applied");
            return true;
        }

        public static bool CheckFilesDynamicHeadersPatchApplied()
        {
            DynamicHeadersPatchData data = new DynamicHeadersPatchData();
            ushort initValue = BitConverter.ToUInt16(ARM9.ReadBytes(data.initOffset, 0x2), 0);
            return initValue == 0xB500;
        }

        public bool CheckBDHCamPatchApplied()
        {
            if (!CheckARM9ExpansionApplied())
            {
                BDHCamARM9requiredLBL.Visible = true;
                DisableBDHCamPatch("ARM9 not expanded!");
                return false;
            }

            if (!PatchToolboxDialog.flag_BDHCamPatchApplied)
            {
                if (!PatchToolboxDialog.CheckFilesBDHCamPatchApplied())
                {
                    return false;
                }
            }
            PatchToolboxDialog.flag_BDHCamPatchApplied = true;
            BDHCamCB.Visible = true;

            DisableBDHCamPatch("Already applied");
            return true;
        }

        public void CheckScrcmdRepointPatchApplied()
        {
            //throw new NotImplementedException();
        }

        public void CheckExpandedTrainerNamesPatchApplied()
        {
            if (flag_TrainerNamesExpanded)
            {
                DisableTrainerNameExpansionPatch("Already\nApplied");
            }
            else
            {
                if (RomInfo.trainerNameLenOffset < 0)
                {
                    DisableTrainerNameExpansionPatch("Unsupported");
                }
                else
                {
                    if (RomInfo.trainerNameMaxLen > TrainerFile.defaultNameLen)
                    {
                        DisableTrainerNameExpansionPatch("Already\nApplied");
                        PatchToolboxDialog.flag_TrainerNamesExpanded = true;
                    }
                }
            }
        }

        #endregion Patch

        #region Button Actions

        private void SentenceCasePatchButton_Click(object sender, EventArgs e)
        {
            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Every Pokémon name will be converted to Sentence Case." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes)
            {
                Parallel.ForEach(RomInfo.pokemonNamesTextNumbers, ID =>
                {
                    TextArchive pokeName = new TextArchive(ID);
                    Parallel.For(1, pokeName.messages.Count, i =>
                    {
                        if (pokeName.messages[i].Length <= 1)
                        {
                            i++;
                        }

                        pokeName.messages[i] = pokeName.messages[i].Replace(PokeDatabase.System.pokeNames[(ushort)i].ToUpper(), PokeDatabase.System.pokeNames[(ushort)i]);
                    });
                    pokeName.SaveToFileDefaultDir(ID, showSuccessMessage: false);
                });
                //sentenceCaseCB.Visible = true;
                MessageBox.Show("Pokémon names have been converted to Sentence Case.", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BDHCAMPatchButton_Click(object sender, EventArgs e)
        {
            BDHCAMPatchData data = new BDHCAMPatchData();

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                if (OverlayUtils.OverlayTable.IsDefaultCompressed(data.overlayNumber))
                {
                    DialogResult d1 = MessageBox.Show("It is STRONGLY recommended to configure Overlay1 as uncompressed before proceeding.\n\n" +
                        "More details in the following dialog.\n\n" + "Do you want to know more?",
                        "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (d1 == DialogResult.Yes)
                    {
                        overlay1uncomprButton_Click(null, null);
                    }
                }
            }

            DialogResult d2;
            d2 = MessageBox.Show("This process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin" + backupSuffix + " will be created)." + "\n\n" +
                "- Backup Overlay" + data.overlayNumber + " file (overlay" + data.overlayNumber + ".bin" + backupSuffix + " will be created)." + "\n\n" +
                "- Replace " + (data.branchString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.branchOffset.ToString("X") + " with " + '\n' + data.branchString + "\n\n" +
                "- Replace " + (data.overlayString1.Length / 3 + 1) + " bytes of data at overlay" + data.overlayNumber + " offset 0x" + data.overlayOffset1.ToString("X") + " with " + '\n' + data.overlayString1 + "\n\n" +
                "- Replace " + (data.overlayString2.Length / 3 + 1) + " bytes of data at overlay" + data.overlayNumber + " offset 0x" + data.overlayOffset2.ToString("X") + " with " + '\n' + data.overlayString2 + "\n\n" +
                "- Modify file #" + expandedARMfileID + " inside " + '\n' + RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\n' + "to insert the BDHCAM routine (any data between 0x" + BDHCAMPatchData.BDHCamSubroutineOffset.ToString("X") + " and 0x" + (BDHCAMPatchData.BDHCamSubroutineOffset + data.subroutine.Length).ToString("X") + " will be overwritten)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d2 == DialogResult.Yes)
            {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + backupSuffix, overwrite: true);

                try
                {
                    ARM9.WriteBytes(DSUtils.HexStringToByteArray(data.branchString), data.branchOffset); //Write new branchOffset

                    /* Write to overlayfile */
                    string overlayFilePath = OverlayUtils.GetPath(data.overlayNumber);
                    if (OverlayUtils.IsCompressed(data.overlayNumber))
                    {
                        OverlayUtils.Decompress(data.overlayNumber);
                    }

                    DSUtils.WriteToFile(overlayFilePath, DSUtils.HexStringToByteArray(data.overlayString1), data.overlayOffset1); //Write new overlayCode1
                    DSUtils.WriteToFile(overlayFilePath, DSUtils.HexStringToByteArray(data.overlayString2), data.overlayOffset2); //Write new overlayCode2
                    overlay1MustBeRestoredFromBackup = false;

                    String fullFilePath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + expandedARMfileID.ToString("D4");

                    /*Write Expanded ARM9 File*/
                    DSUtils.WriteToFile(fullFilePath, data.subroutine, BDHCAMPatchData.BDHCamSubroutineOffset);
                }
                catch
                {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 and overlay from their respective backups.", "Something went wrong",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                overlay1MustBeRestoredFromBackup = false;
                DisableBDHCamPatch("Already applied");
                PatchToolboxDialog.flag_BDHCamPatchApplied = true;
                BDHCamCB.Visible = true;

                MessageBox.Show("The BDHCAM patch has been applied.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void overlay1uncomprButton_Click(object sender, EventArgs e)
        {
            if (ConfigureOverlay1Uncompressed())
            {
                DisableOverlay1patch("Already applied");
                overlay1CB.Visible = true;
            }
        }

        public static bool ConfigureOverlay1Uncompressed()
        {
            bool isCompressed = false;
            string stringDecompressOverlay = "";

            if (OverlayUtils.IsCompressed(1))
            {
                isCompressed = true;
                stringDecompressOverlay = "- Overlay 1 will be decompressed.\n\n";
            }

            DialogResult d = MessageBox.Show("This process will apply the following changes:\n\n" +
            stringDecompressOverlay +
            "- Overlay 1 will be configured as \"uncompressed\" in the overlay table.\n\n" +
            "Do you wish to continue?",
            "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes)
            {
                OverlayUtils.OverlayTable.SetDefaultCompressed(1, false);
                if (isCompressed)
                {
                    OverlayUtils.Decompress(1);
                }

                MessageBox.Show("Overlay1 is now configured as uncompressed.", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private void ApplyItemStandardizeButton_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("This process will apply the following changes:\n\n" +
                "- Item scripts will be rearranged to follow the natural, ascending index order.\n\n" +
                "- Any unsaved change to the current Event File will be discarded.\n\n",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes)
            {
                DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.scripts });
                DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.eventFiles });

                if (PatchToolboxDialog.flag_standardizedItems)
                {
                    AlreadyApplied();
                }
                else
                {
                    // Load item script file data
                    ScriptFile itemScriptFile = new ScriptFile(RomInfo.itemScriptFileNumber);

                    // Create map for: script no. -> vanilla item
                    int[] vanillaItemsArray = new int[itemScriptFile.allScripts.Count - 1];

                    for (int i = 0; i < itemScriptFile.allScripts.Count - 1; i++)
                    {
                        vanillaItemsArray[i] = BitConverter.ToInt16(itemScriptFile.allScripts[i].commands[0].cmdParams[1], 0);
                    };

                    // Parse all event files and fix instances of ground items according to the new order
                    int cnt = Filesystem.GetEventFileCount();
                    (int itemScrMin, int itemScrMax) = (7000, 8000);

                    for (int i = 0; i < cnt; i++)
                    {
                        bool dirty = false;

                        EventFile eventFile = new EventFile(i);

                        for (int j = 0; j < eventFile.overworlds.Count; j++)
                        {
                            // If ow is marked as an item, or in the rare case it is not but script no. falls within item script range:
                            bool isItem = eventFile.overworlds[j].type == (ushort)Overworld.OwType.ITEM
                                          || (eventFile.overworlds[j].scriptNumber >= itemScrMin
                                          && eventFile.overworlds[j].scriptNumber <= itemScrMax);

                            if (isItem)
                            {
                                int itemScriptID = eventFile.overworlds[j].scriptNumber - (itemScrMin - 1);
                                eventFile.overworlds[j].scriptNumber = (ushort)(itemScrMin + vanillaItemsArray[itemScriptID - 1]);
                                dirty = true;
                            }
                        }

                        // Save event file
                        if (dirty)
                        {
                            eventFile.SaveToFileDefaultDir(i, showSuccessMessage: false);
                        }
                    };

                    //Distortion world - turnback cave Griseous Orb fix
                    if (gameFamily.Equals(GameFamilies.Plat))
                    {
                        string ow9path = OverlayUtils.GetPath(9);
                        int ow9offs = 0x8E20 + 10;

                        int itemScriptID;

                        using (DSUtils.EasyReader ewr = new DSUtils.EasyReader(ow9path, ow9offs))
                        {
                            itemScriptID = ewr.ReadUInt16() - (itemScrMin - 1);
                        }

                        using (DSUtils.EasyWriter ewr = new DSUtils.EasyWriter(ow9path, ow9offs))
                        {
                            ewr.Write((ushort)(itemScrMin + vanillaItemsArray[itemScriptID - 1]));
                        }
                    }

                    // Sort scripts in the Script File according to item indices
                    int itemCount = new TextArchive(RomInfo.itemNamesTextNumber).messages.Count;
                    ScriptCommandContainer executeGive = new ScriptCommandContainer((uint)itemCount + 1, itemScriptFile.allScripts[itemScriptFile.allScripts.Count - 1]);

                    itemScriptFile.allScripts.Clear();

                    for (ushort i = 0; i < itemCount; i++)
                    {
                        List<ScriptCommand> cmdList = new List<ScriptCommand> {
                            new ScriptCommand("SetVar 0x8008 " + i, itemScriptFile),
                            new ScriptCommand("SetVar 0x8009 0x1", itemScriptFile),
                            new ScriptCommand("Jump Function_#1", itemScriptFile)
                        };

                        itemScriptFile.allScripts.Add(new ScriptCommandContainer((ushort)(i + 1), ScriptFile.ContainerTypes.Script, commandList: cmdList));
                    }

                    itemScriptFile.allScripts.Add(executeGive);
                    itemScriptFile.allFunctions[0].usedScriptID = itemCount + 1;

                    itemScriptFile.SaveToFileDefaultDir(RomInfo.itemScriptFileNumber, showSuccessMessage: false);
                    MessageBox.Show("Operation successful.", "Process completed.", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DisableStandardizeItemsPatch("Already applied");

                    itemNumbersCB.Visible = true;
                    PatchToolboxDialog.flag_standardizedItems = true;
                }
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ApplyARM9ExpansionButton_Click(object sender, EventArgs e)
        {
            ARM9PatchData data = new ARM9PatchData();

            DialogResult d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin" + backupSuffix + " will be created)." + "\n\n" +
                "- Replace " + (data.branchString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.branchOffset.ToString("X") + " with " + '\n' + data.branchString + "\n\n" +
                "- Replace " + (data.initString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.initOffset.ToString("X") + " with " + '\n' + data.initString + "\n\n" +
                "- Modify file #" + expandedARMfileID + " inside " + '\n' + RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\n' + " to accommodate for 88KB of data (no backup)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes)
            {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + backupSuffix, overwrite: true);

                try
                {
                    ARM9.WriteBytes(DSUtils.HexStringToByteArray(data.branchString), data.branchOffset); //Write new branchOffset
                    ARM9.WriteBytes(DSUtils.HexStringToByteArray(data.initString), data.initOffset); //Write new initOffset

                    string fullFilePath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + expandedARMfileID.ToString("D4");
                    File.Delete(fullFilePath);
                    using (BinaryWriter f = new BinaryWriter(File.Create(fullFilePath)))
                    {
                        for (int i = 0; i < 0x16000; i++)
                            f.Write((byte)0x00); // Write Expanded ARM9 File
                    }

                    DisableARM9patch("Already applied");
                    arm9patchCB.Visible = true;
                    PatchToolboxDialog.flag_arm9Expanded = true;

                    switch (RomInfo.gameFamily)
                    {
                        case GameFamilies.Plat:
                        case GameFamilies.HGSS:
                            BDHCamPatchButton.Text = "Apply Patch";
                            BDHCamPatchButton.Enabled = true;
                            BDHCamPatchLBL.Enabled = true;
                            BDHCamPatchTextLBL.Enabled = true;
                            BDHCamARM9requiredLBL.Visible = false;
                            break;
                    }

                    MessageBox.Show("The ARM9's usable memory has been expanded.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 backup (arm9.bin" + backupSuffix + ").", "Something went wrong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void expandMatrixButton_Click(object sender, EventArgs e)
        {
            string listOfChanges = "";
            int languageOffset = 0;

            if (RomInfo.romID == "IPKE" || RomInfo.romID == "IPGE" || RomInfo.romID == "IPGS")
            {
                languageOffset = +8;
            }

            foreach (KeyValuePair<uint[], string> kv in ToolboxDB.matrixExpansionDB)
            {
                listOfChanges += " - Replace " + (kv.Value.Length / 3 + 1) + " bytes of data at arm9 offset";
                if (kv.Key.Length > 1)
                    listOfChanges += "s";

                for (int i = 0; i < kv.Key.Length; i++)
                {
                    listOfChanges += " 0x" + (kv.Key[i] - ARM9.address + languageOffset).ToString("X");

                    if (i < kv.Key.Length - 1)
                        listOfChanges += ",";
                }
                listOfChanges += " with " + '\n' + kv.Value + "\n\n";
            }

            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                listOfChanges +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes)
            {
                try
                {
                    foreach (KeyValuePair<uint[], string> kv in ToolboxDB.matrixExpansionDB)
                    {
                        foreach (uint offset in kv.Key)
                        {
                            ARM9.WriteBytes(DSUtils.HexStringToByteArray(kv.Value), (uint)(offset - ARM9.address + languageOffset));
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 backup (arm9.bin" + backupSuffix + ").", "Something went wrong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                DisableMatrixExpansionPatch("Already applied");
                expandedMatrixCB.Visible = true;
                PatchToolboxDialog.flag_MatrixExpansionApplied = true;
                MessageBox.Show("Matrix 0 can now be freely expanded up to twice its size.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dynamicHeadersButton_Click(object sender, EventArgs e)
        {
            DynamicHeadersPatchData data = new DynamicHeadersPatchData();
            var headersDir = RomInfo.gameDirs[DirNames.dynamicHeaders];

            bool specialCase = RomInfo.gameFamily == GameFamilies.HGSS && RomInfo.gameLanguage != GameLanguages.Japanese && RomInfo.gameLanguage != GameLanguages.Spanish;
            string specialCaseChanges = "";

            if (specialCase)
            {
                specialCaseChanges = "- Replace " + (data.specialCaseData1.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + (data.specialCaseOffset1 + data.pointerDiff).ToString("X") + " with " + '\n' + data.specialCaseData1 + "\n\n" +
                    "- Replace " + (data.specialCaseData2.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + (data.specialCaseOffset2 + data.pointerDiff).ToString("X") + " with " + '\n' + data.specialCaseData2 + "\n\n" +
                    "- Replace " + (data.specialCaseData3.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + (data.specialCaseOffset3 + data.pointerDiff).ToString("X") + " with " + '\n' + data.specialCaseData3 + "\n\n";
            }

            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin" + backupSuffix + " will be created)." + "\n\n" +
                "- NARC file at " + headersDir.packedDir + " will become the new header container." + "\n\n" +
                "- The default ARM9 header table will be split into multiple files (one per header), each one saved into NARC" + headersDir.packedDir + " upon saving the ROM." + "\n\n" +
                "- Replace " + (data.initString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.initOffset.ToString("X") + " with " + '\n' + data.initString + "\n\n" +
                "- Neutralize instances of (HeaderID * 0x18) so the base offset which the data is read from is always 0x0." + "\n\n" +
                "- Change pointers to header fields, from(ARM9_HEADER_TABLE_OFFSET + n) to simply(0 + n)" + "\n\n" +
                specialCaseChanges +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes)
            {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + backupSuffix, overwrite: true);

                try
                {
                    /* Write main routine (HG USA):

                     00 B5		        push (lr)
                     01 1C		        mov r1, r0
                     32 20*		        mov r0, #0x32
                     00 22		        mov r2, #0x0
                     CC F7 58 F9**	    bl 0x02007524	@Load_Memory
                     03 1C		        mov r3, r0
                     DF F7 49 FC**	    bl 0x0201AB0C	@Free_Memory
                     00 BD		        pop, pc

                    *FOR PLATINUM (all languages):
                     94 20		        mov r0, #0x94

                    **BRANCHES FOR OTHER VERSIONS/LANGUAGES:

                     HG ESP (IPKS):
                     CC F7 5C F9	    bl 0x02007524	@Load_Memory
                     DF F7 4D FC	    bl 0x0201AB0C	@Free_Memory

                     HG JAP (IPKJ) and SS JAP (IPGJ):
                     CC F7 08 FB	    bl 0x0200743C	@Load_Memory
                     DF F7 C7 FC	    bl 0x0201A7C0	@Free_Memory

                     Plat USA (CPUE):
                     CC F7 48 FD	    bl 0x02006AC0	@Load_Memory
                     DE F7 C7 F8	    bl 0x020181C4	@Free_Memory

                     Plat ESP (CPUS), ITA (CPUI), FRA (CPUF), GER (CPUD):
                     CC F7 00 FD	    bl 0x02006AD4	@Load_Memory
                     CC F7 74 FC	    bl 0x02018234	@Free_Memory

                     Plat JAP (CPUJ):
                     CC F7 0A FF	    bl 0x02006A00	@Load_Memory
                     DE F7 3D F9	    bl 0x02017E6C	@Free_Memory
                     */

                    ARM9.WriteBytes(DSUtils.HexStringToByteArray(data.initString), data.initOffset);

                    /* - Neutralize instances of (HeaderID * 0x18) so the base offset which the data is read from is always 0x0:

                            Replace this:
                            18 21       mov r1, #0x18
                            41 43       mul r1, r0

                            with this:
                            19 00       lsl r1, r3, 0
                            C0 46       nop

                      - Change pointers to header fields, from (ARM9_HEADER_TABLE_OFFSET + n) to simply (0 + n)

                       * for ESP HG (IPKS): subtract 0x8 from every reference offset
                       * for JAP HG (IPKJ) and SS (IPGJ): subtract 0x448 from every reference offset
                       * for Plat ESP, ITA, FRA, GER, JAP: add 0xA4 to every reference offset
                       * for Plat JAP: subtract 0x444 from every reference offset

                     */

                    foreach (Tuple<uint, uint> reference in DynamicHeadersPatchData.dynamicHeadersPointersDB[RomInfo.gameFamily])
                    {
                        ARM9.WriteBytes(DSUtils.HexStringToByteArray(data.REFERENCE_STRING), (uint)(reference.Item1 + data.pointerDiff));
                        uint pointerValue = BitConverter.ToUInt32(ARM9.ReadBytes((uint)(reference.Item2 + data.pointerDiff), 4), 0) - RomInfo.headerTableOffset - ARM9.address;
                        ARM9.WriteBytes(BitConverter.GetBytes(pointerValue), (uint)(reference.Item2 + data.pointerDiff));
                    }

                    if (specialCase)
                    {
                        /*  Special case: at 0x3B522 (non-JAP and non-Spanish HG offset) there is an instruction
                            between the (mov r1, #0x18) and (mul r1, r0) commands, so we must handle this separately */

                        ARM9.WriteBytes(DSUtils.HexStringToByteArray(data.specialCaseData1), (uint)(data.specialCaseOffset1 + data.pointerDiff));
                        ARM9.WriteBytes(DSUtils.HexStringToByteArray(data.specialCaseData2), (uint)(data.specialCaseOffset2 + data.pointerDiff));
                        ARM9.WriteBytes(DSUtils.HexStringToByteArray(data.specialCaseData3), (uint)(data.specialCaseOffset3 + data.pointerDiff));
                    }

                    // Clear the dynamic headers directory in 'unpacked'
                    Directory.Delete(headersDir.unpackedDir, true);
                    Directory.CreateDirectory(headersDir.unpackedDir);

                    /* Now move the headers data from arm9 to the new directory. Upon saving the ROM,
                       the data will be packed into a NARC and replace a/0/5/0 in HGSS or
                       debug/cb_edit/d_test.narc in Platinum */

                    int headerCount = RomInfo.GetHeaderCount();
                    for (int i = 0; i < headerCount; i++)
                    {
                        byte[] headerData = MapHeader.LoadFromARM9((ushort)i).ToByteArray();
                        DSUtils.WriteToFile(headersDir.unpackedDir + "\\" + i.ToString("D4"), headerData);
                    }

                    DisableDynamicHeadersPatch("Already applied");
                    dynamicHeadersPatchCB.Visible = true;
                    PatchToolboxDialog.flag_DynamicHeadersPatchApplied = true;

                    MessageBox.Show("The headers are now dynamically allocated in memory.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 backup (arm9.bin" + backupSuffix + ").", "Something went wrong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void disableDynamicTexturesButton_Click(object sender, EventArgs e)
        {
            DialogResult d;
            d = MessageBox.Show("Applying this patch will set the Dynamic Textures field of all AreaData files to 0xFFFF.\n\n" +
                "Are you sure you want to proceed?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes)
            {
                DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { DirNames.areaData });

                string[] adFiles = Directory.GetFiles(gameDirs[DirNames.areaData].unpackedDir);
                foreach (string s in adFiles)
                {
                    AreaData a = new AreaData(new FileStream(s, FileMode.Open))
                    {
                        dynamicTextureType = 0xFFFF
                    };
                    a.SaveToFile(s, showSuccessMessage: false);
                }

                DisableKillTextureAnimationsPatch("Already applied");
                disableTextureAnimationsCB.Visible = true;
                MessageBox.Show("Texture Animations have been disabled in every AreaData.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void expandTrainerNamesButton_Click(object sender, EventArgs e)
        {
            // Pearl        USA     ARM9 at 0x6AC32     // TODO: Verify
            // Pearl        Spain   ARM9 at 0x6AC8E     // TODO: Verify
            // Diamond      USA     ARM9 at 0x6AC32
            // Diamond      Spain   ARM9 at 0x6AC8E
            // Platinum     USA     ARM9 at 0x791DE
            // Platinum     Spain   ARM9 at 0x7927E
            // HeartGold    USA     ARM9 at 0x7342E
            // HeartGold    Spain   ARM9 at 0x73426
            // SoulSilver   USA     ARM9 at 0x7342E
            // SoulSilver   Spain   ARM9 at 0x7342E     // TODO: Verify

            DialogResult d = MessageBox.Show($"Applying this patch will set the Trainer Name max length to {PatchToolboxDialog.expandedTrainerNameLength - 1} usable characters.\n" +
                "Are you sure you want to proceed?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes)
            {
                try
                {
                    using (ARM9.Writer wr = new ARM9.Writer(RomInfo.trainerNameLenOffset))
                    {
                        wr.Write((byte)PatchToolboxDialog.expandedTrainerNameLength);
                    }

                    PatchToolboxDialog.flag_TrainerNamesExpanded = true;
                    DisableTrainerNameExpansionPatch("Already applied");
                    expandTrainerNamesCB.Visible = true;
                    MessageBox.Show("Trainer Names have been extended.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException)
                {
                    MessageBox.Show("ARM9 could not be written.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region Mikelan's custom commands

        private void applyCustomCommands(object sender, EventArgs e)
        {
            int expTableOffset = GetCommandTableOffset();

            if (expTableOffset < 0)
            {
                DialogResult d;
                d = MessageBox.Show("Script command table has not been repointed.\n\n" +
                    "Do you wish to repoint it to the expanded ARM9 file?\n\n" +
                    "By default it will be written from 0x200 to 0x1700.\n" +
                    "If you already have something there, you must cancel this window and move these things to a new location, or you can manually repoint the script command table to a different free location in the expanded ARM9 file",
                    "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (d == DialogResult.Yes)
                {
                    RepointCommandTable();
                }
                else
                {
                    return;
                }
            }

            if (ImportCustomCommand())
            {
                MessageBox.Show("Script commands succesfully installed in the ROM", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private int GetCommandTableOffset()
        { // Checks if command table is repointed IN THE EXPANDED ARM9 FILE, returns pointer inside this file
            ResourceManager customcmdDB = new ResourceManager("DSPRE.Resources.ROMToolboxDB.CustomScrCmdDB", Assembly.GetExecutingAssembly());
            int pointerOffset = int.Parse(customcmdDB.GetString("pointerOffset" + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage));
            using (ARM9.Reader r = new ARM9.Reader(pointerOffset))
            {
                uint cmdTable = r.ReadUInt32();
                uint offset = cmdTable - synthOverlayLoadAddress;

                if ((offset >= 0) && (offset <= 0x12B00))
                {
                    return (int)offset; // Table position inside the expanded arm9 file
                }
            }
            return -1; // No table in expanded arm9 file
        }

        private void RepointCommandTable()
        {
            string expandedPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + "\\0000";
            ResourceManager customcmdDB = new ResourceManager("DSPRE.Resources.ROMToolboxDB.CustomScrCmdDB", Assembly.GetExecutingAssembly());

            FileStream arm9FileStream = new FileStream(RomInfo.arm9Path, FileMode.Open); // I make a copy of the stream so the file is free for writing
            MemoryStream arm9Stream = new MemoryStream();
            arm9FileStream.CopyTo(arm9Stream);
            byte[] cmdTbl = arm9Stream.ToArray();

            using (BinaryWriter expArmWriter = new BinaryWriter(new FileStream(expandedPath, FileMode.Open)))
            {
                expArmWriter.BaseStream.Position = 0x200; // Command table default offset
                expArmWriter.Write(cmdTbl, int.Parse(customcmdDB.GetString("originalTableOffset" + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage)), 4 * 0x355);
            }

            arm9FileStream.Close();

            using (ARM9.Writer wr = new ARM9.Writer())
            { // Change both the pointer and the limit
                wr.BaseStream.Position = int.Parse(customcmdDB.GetString("pointerOffset" + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage));
                wr.Write((uint)0x023C8200);

                wr.BaseStream.Position = int.Parse(customcmdDB.GetString("limitOffset" + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage));
                wr.Write((uint)0x053C);
            }
        }

        private bool ImportCustomCommand()
        {
            string expandedPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + "\\0000";
            int appliedPatches = 0;

            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Custom Script Command File (*.scrcmd)|*.scrcmd";
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return false;
            }

            FileStream expandedFileStream = new FileStream(expandedPath, FileMode.Open);
            MemoryStream expandedStream = new MemoryStream();
            expandedFileStream.CopyTo(expandedStream);
            expandedFileStream.Close();

            using (DSUtils.EasyWriter expandedWriter = new DSUtils.EasyWriter(expandedPath, fmode: FileMode.Open))
            {
                using (BinaryReader expandedReader = new BinaryReader(expandedStream))
                {
                    try
                    {
                        System.Xml.Linq.XDocument xmldoc = System.Xml.Linq.XDocument.Load(new FileStream(of.FileName, FileMode.Open));

                        foreach (var node in xmldoc.Root.Elements("scriptcommand"))
                        {
                            ushort commandID = ushort.Parse(node.Attribute("ID").Value, System.Globalization.NumberStyles.HexNumber);
                            string targetROM = node.Element("ROM").Value;
                            string targetLang = node.Element("lang").Value;
                            string commandName = node.Element("name").Value;
                            string paramCount = node.Element("paramcount").Value;
                            string paramCode = node.Element("paramcode").Value;
                            int asmOffset = Int32.Parse(node.Element("asmoffset").Value, System.Globalization.NumberStyles.HexNumber);
                            string asmCode = node.Element("asmcode").Value.Replace("\n", "").Replace("\t", "").Replace(" ", "");

                            if (RomInfo.gameVersion.ToString().Equals(targetROM) && RomInfo.gameLanguage.Equals(targetLang))
                            {
                                expandedReader.BaseStream.Position = 0x200 + commandID * 4;
                                if (expandedReader.ReadUInt32() != 0)
                                {
                                    DialogResult d;
                                    d = MessageBox.Show("Script command " + commandID.ToString("X4") + " is already used.\n\n" +
                                        "Do you really want to overwrite it?",
                                        "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (d == DialogResult.No)
                                    {
                                        continue;
                                    }
                                }

                                expandedWriter.BaseStream.Position = 0x200 + commandID * 4;
                                expandedWriter.Write((int)(synthOverlayLoadAddress + asmOffset + 1));

                                byte[] asmCodeBytes = DSUtils.StringToByteArray(asmCode);
                                expandedWriter.BaseStream.Position = asmOffset;
                                expandedWriter.Write(asmCodeBytes);

                                appliedPatches++;
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Selected command installation file is corrupted.\n\n" +
                        "Please, download it again or contact its creator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return false;
                    }
                }
            }

            if (appliedPatches == 0)
            {
                MessageBox.Show("No command could be installed from this file.\n\n" +
                "Make sure the command installation file supports your current ROM.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        #endregion Mikelan's custom commands

        #endregion Button Actions

        #region Error Messsages

        private void AlreadyApplied()
        {
            MessageBox.Show("This patch has already been applied.", "Can't reapply patch", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion Error Messsages
    }
}
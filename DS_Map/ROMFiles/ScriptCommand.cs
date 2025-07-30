using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    public class ScriptCommand {
        enum ParamTypeEnum {
            INTEGER,
            VARIABLE,
            FLEX,
            OW_ID,
            OW_MOVEMENT_TYPE,
            OW_DIRECTION,
            FUNCTION_ID,
            ACTION_ID,
            CMD_NUMBER,
            POKEMON_NAME,
            ITEM_NAME,
            MOVE_NAME
        };

        public ushort? id;
        public List<byte[]> cmdParams;
        public string name;

        // CHANGE: Update the constructor to use ScriptParameter
        public ScriptCommand(ushort id, List<byte[]> parametersList)
        {
            if (parametersList is null)
            {
                this.id = null;
                return;
            }

            if (!RomInfo.ScriptCommandNamesDict.TryGetValue(id, out name)) {
                name = FormatNumber(id, ParamTypeEnum.CMD_NUMBER);
            }

            switch (id) {
                case 0x0016: // Jump
                case 0x001A: // Call
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.FUNCTION_ID)}";
                    break;
                case 0x0017: // JumpIfObjID
                case 0x0018: // JumpIfEventID
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1])}";
                    break;
                case 0x0019: // JumpIfPlayerDir
                    name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_DIRECTION)} {FormatNumber(parametersList[1], ParamTypeEnum.FUNCTION_ID)}";
                    break;
                case 0x001C: // JumpIf
                case 0x001D: // CallIf
                {
                        string number = FormatNumber(parametersList[1], ParamTypeEnum.FUNCTION_ID);

                        // Access the byte value from the parameter's raw data
                        if (RomInfo.ScriptComparisonOperatorsDict.TryGetValue(parametersList[0][0], out string v)) {
                            name += $" {v} {number}";
                        } else {
                            name += $" {parametersList[0][0]} {number}";
                        }

                        break;
                    }
                case 0x005E: // Movement
                        name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1], ParamTypeEnum.ACTION_ID)}";
                    break;
                case 0x006A: // GetOverworldPosition
                        name += FormatCmd_Overworld_TwoParams(parametersList);
                    break;
                case 0x0062: // Lock
                case 0x0063: // Release
                case 0x0064: // AddOW
                case 0x0065: // RemoveOW
                        name += $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)}";
                    break;
                case 0x006D: // SetOverworldMovement
                        name += FormatCmd_Overworld_Move(parametersList);
                    break;

                case 0x004C: // PlayCry
                        name += FormatCmd_par0Pokemonpar1Generic(parametersList);
                    break;

                case 0x0083: // SetStarter [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += $" {FormatNumber(parametersList[0], ParamTypeEnum.POKEMON_NAME)}";
                    } else {
                        goto default;
                    }
                    break;

                case 0x0089: // GivePokemon [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_GivePokemonHGSS(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x00B0: // Warp [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Warp(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                
                case 0x008A: // GivePokemonEgg [HGSS]
                case 0x00F9: // WildBattle [HGSS]
                case 0x00FA: // WildBattleNoButtons [HGSS]
                case 0x01C4: // ShowPokemonPic [HGSS]
                case 0x0205: // CheckPokemonInParty [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_par0Pokemonpar1Generic(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0081: // CheckItemIsMachine [HGSS]
                case 0x0082: // GetItemPocket [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_par0Itempar1Generic(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x008B: // ReplaceMove [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_ReplaceMove(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x008C: // CheckPokemonHasMove [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_CheckPokemonHasMove(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x008D: // CheckMoveInParty [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_CheckMoveInParty(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x00C2: // TextItem [HGSS]
                case 0x00C4: // TextMachineMove [HGSS]
                case 0x01B0: // CheckFossilPokemon [HGSS]
                case 0x034B: // TextItemLowercase [HGSS]
                case 0x034C: // TextItemPlural [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_par0Genericpar1Item(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x00CA: // TextPokemon [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_TextPokemon(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0185: // CheckBornPokemonInParty [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_par0Genericpar1Pokemon(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0152: // SetOverworldDefaultPosition [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Overworld_TwoParams(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0153: // SetOverworldPosition [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Overworld_3Coords_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0154: // SetOverworldDefaultMovement [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Overworld_Move(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x01B1: // CheckFossil [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_CheckFossil(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x024D: // WildBattleSp [HGSS]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_par0Pokemonpar1Genericpar2Generic(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x007E: // TakeItem [HGSS] or CheckItem [DPPt]
                case 0x007D: // GiveItem [HGSS] or CheckItemSpace [DPPt]
                    name += FormatCmd_TakeItem(parametersList);
                    break;

                case 0x007F: // CheckItemSpace [HGSS] or CheckItemIsMachine [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_TakeItem(parametersList);
                    } else if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_par0Itempar1Generic(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0080: // CheckItem [HGSS] or GetItemPocket [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_TakeItem(parametersList);
                    } else if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_par0Itempar1Generic(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x00D3: // GetSwarmInfo [HGSS] or TextMachineMove [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_par0Genericpar1Pokemon(parametersList);
                    } else if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_par0Genericpar1Item(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x007B: // GiveItem [DPPt]
                case 0x007C: // TakeItem [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_TakeItem(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0155: // SetOverworldDefaultDirection [DPPt]
                case 0x0158: // SetOverworldDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += FormatCmd_Overworld_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0097: // GivePokemonEgg  [DPPt]
                case 0x0124: // WildBattle  [DPPt]
                case 0x0208: // ShowPokemonPic  [DPPt]
                case 0x0262: // CheckPokemonInParty [DPPt]
                case 0x02BD: // WildBattleSp [DPPt]
                case 0x02DD: // GetBornPokemonPartyPos [DPPt]
                case 0x0319: // GiratinaBattle [DPPt]
                case 0x0337: // CheckPokemonIsSeen [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_par0Pokemonpar1Generic(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0096: // GivePokemon [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_GivePokemonDPPt(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0099: // CheckMove [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_CheckMove(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x009A: // CheckMoveInParty [DPPt]
                case 0x00D4: // TextMove [DPPt]
                case 0x0224: // TeachMoveScreen [DPPt]
                case 0x02E7: // LearnMoveScreen [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_par0Genericpar1Move(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x00BE: // Warp [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Warp(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x00DA: // TextPokemon [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_TextPokemon(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x00E3: // GetSwarmInfo [DPPt]
                case 0x01C0: // CheckBornPokemonInParty [DPPt]
                case 0x0217: // GetAmitySquareAccessory [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_par0Genericpar1Pokemon(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x00D1: // TextItem [DPPt]
                case 0x01F4: // CheckFossilPokemon [DPPt]
                case 0x033C: // TextItemLowercase [DPPt]
                case 0x033D: // TextItemPlural [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_par0Genericpar1Item(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x017B: // TextBerry [DPPt]
                case 0x01F5: // CheckFossil [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_CheckFossil(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0182: // SetBerryMulch [DPPt]
                case 0x0183: // SetBerrySpecies [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.HGSS)) {
                        name += $" {FormatNumber(parametersList[0], ParamTypeEnum.ITEM_NAME)}";
                    } else {
                        goto default;
                    }
                    break;

                case 0x0186: // SetOverworldDefaultPosition [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Overworld_TwoParams(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0187: // SetOverworldPosition [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Overworld_3Coords_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0188: // SetOverworldDefaultMovement [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Overworld_Move(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x0189: // SetOverworldDefaultDirection [DPPt]
                case 0x018C: // SetOverworldDirection [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_Overworld_Dir(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x02E9: // ChangePartyPokemonMove [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_ChangePartyPokemonMove(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x02EA: // CheckAffordMove [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += $" {FormatNumber(parametersList[0], ParamTypeEnum.MOVE_NAME)} {FormatNumber(parametersList[1], ParamTypeEnum.MOVE_NAME)}";
                    } else {
                        goto default;
                    }
                    break;

                case 0x02EB: // PayTutorShards [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += $" {FormatNumber(parametersList[0], ParamTypeEnum.MOVE_NAME)}";
                    } else {
                        goto default;
                    }
                    break;

                case 0x02EC: // ShowMovePriceBoard [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += FormatCmd_ShowMovePriceBoard(parametersList);
                    } else {
                        goto default;
                    }
                    break;

                case 0x031A: // RegisterSeenPokemon [DPPt]
                    if (RomInfo.gameFamily.Equals(RomInfo.GameFamilies.DP) || RomInfo.gameFamily.Equals(RomInfo.GameFamilies.Plat)) {
                        name += $" {FormatNumber(parametersList[0], ParamTypeEnum.POKEMON_NAME)}";
                    } else {
                        goto default;
                    }
                    break;

                default:
                    for (int i = 0; i < parametersList.Count; i++) {
                        name += $" {FormatNumber(parametersList[i])}";
                    }
                    break;
            }

            this.id = id;
            this.cmdParams = parametersList;
        }

        private string FormatCmd_Warp(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2])} {FormatNumber(parametersList[3])} {FormatNumber(parametersList[4], ParamTypeEnum.OW_DIRECTION)}";
        }

        private string FormatCmd_Overworld_TwoParams(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2])}";
        }

        private string FormatCmd_Overworld_Move(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1], ParamTypeEnum.OW_MOVEMENT_TYPE)}";
        }

        private string FormatCmd_Overworld_3Coords_Dir(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2])} {FormatNumber(parametersList[3])} {FormatNumber(parametersList[4], ParamTypeEnum.OW_DIRECTION)}";
        }

        private string FormatCmd_Overworld_Dir(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.OW_ID)} {FormatNumber(parametersList[1], ParamTypeEnum.OW_DIRECTION)}";
        }
        // generic formatting command for playcry, showpokemonpic etc, first param is species, second param is a generic integer
        private string FormatCmd_par0Pokemonpar1Generic(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.POKEMON_NAME)} {FormatNumber(parametersList[1])}";
        }
        private string FormatCmd_par0Itempar1Generic(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.ITEM_NAME)} {FormatNumber(parametersList[1])}";
        }
        private string FormatCmd_par0Genericpar1Item(List<byte[]> parametersList)
        {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1], ParamTypeEnum.ITEM_NAME)}";
        }
        private string FormatCmd_par0Genericpar1Pokemon(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1], ParamTypeEnum.POKEMON_NAME)}";
        }
        private string FormatCmd_par0Pokemonpar1Genericpar2Generic(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.POKEMON_NAME)} {FormatNumber(parametersList[1])}  {FormatNumber(parametersList[2])}";
        }
        private string FormatCmd_TakeItem(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.ITEM_NAME)} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2])}";
        }
        private string FormatCmd_ReplaceMove(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2], ParamTypeEnum.MOVE_NAME)}";
        }
        private string FormatCmd_CheckPokemonHasMove(List<byte[]> parametersList)
        {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1], ParamTypeEnum.MOVE_NAME)} {FormatNumber(parametersList[2])}";
        }
        private string FormatCmd_CheckMoveInParty(List<byte[]> parametersList)
        {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1], ParamTypeEnum.MOVE_NAME)}";
        }
        private string FormatCmd_TextPokemon(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1], ParamTypeEnum.POKEMON_NAME)} {FormatNumber(parametersList[2])} {FormatNumber(parametersList[3])}";
        }
        private string FormatCmd_GivePokemonHGSS(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.POKEMON_NAME)} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2], ParamTypeEnum.ITEM_NAME)} {FormatNumber(parametersList[3])} {FormatNumber(parametersList[4])} {FormatNumber(parametersList[5])}";
        }
        private string FormatCmd_GivePokemonDPPt(List<byte[]> parametersList) {
            return $" {FormatNumber(parametersList[0], ParamTypeEnum.POKEMON_NAME)} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2], ParamTypeEnum.ITEM_NAME)} {FormatNumber(parametersList[3])}";
        }
        private string FormatCmd_CheckFossil(List<byte[]> parametersList)
        {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1], ParamTypeEnum.ITEM_NAME)} {FormatNumber(parametersList[2])}";
        }
        private string FormatCmd_CheckMove(List<byte[]> parametersList)
        {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1], ParamTypeEnum.MOVE_NAME)} {FormatNumber(parametersList[2])}";
        }
        private string FormatCmd_par0Genericpar1Move(List<byte[]> parametersList)
        {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1], ParamTypeEnum.MOVE_NAME)}";
        }
        private string FormatCmd_ChangePartyPokemonMove(List<byte[]> parametersList)
        {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2], ParamTypeEnum.MOVE_NAME)}";
        }
        private string FormatCmd_ShowMovePriceBoard(List<byte[]> parametersList)
        {
            return $" {FormatNumber(parametersList[0])} {FormatNumber(parametersList[1])} {FormatNumber(parametersList[2], ParamTypeEnum.MOVE_NAME)} {FormatNumber(parametersList[3])}";
        }

        public ScriptCommand(string wholeLine, int lineNumber = 0) {
            name = wholeLine;
            cmdParams = new List<byte[]>();

            var processedLine = ProcessBracketedItems(wholeLine);
            string[] nameParts = processedLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            if (RomInfo.ScriptCommandNamesReverseDict.TryGetValue(nameParts[0].ToLower(), out ushort cmdID)) {
                id = cmdID;
            } else {
                try {
                    id = ushort.Parse(nameParts[0].PurgeSpecial(ScriptFile.specialChars), nameParts[0].GetNumberStyle());
                } catch {
                    string details;
                    if (wholeLine.Contains(':') && wholeLine.ContainsNumber()) {
                        details = "This probably means you forgot to \"End\" the Script or Function above it.";
                        details += Environment.NewLine + "Please, also note that only Functions can be terminated\nwith \"Return\".";
                    } else {
                        details = "Are you sure it's a proper Script Command?";
                    }

                    MessageBox.Show("This Script file could not be saved." +
                                    $"\nParser failed to interpret line {lineNumber}: \"{wholeLine}\".\n\n{details}", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            /* Read parameters from remainder of the description */
            byte[] parametersSizeArr = RomInfo.ScriptCommandParametersDict[(ushort)id];

            int paramLength = 0;
            int paramsProcessed = 0;

            if (parametersSizeArr.First() == 0xFF) {
                int firstParamValue = int.Parse(nameParts[1].PurgeSpecial(ScriptFile.specialChars), nameParts[1].GetNumberStyle());
                byte firstParamSize = parametersSizeArr[1];

                cmdParams.Add(firstParamValue.ToByteArrayChooseSize(firstParamSize));
                paramsProcessed++;

                int i = 2;
                int optionsCount = 0;

                bool found = false;
                while (i < parametersSizeArr.Length) {
                    paramLength = parametersSizeArr[i + 1];

                    if (parametersSizeArr[i] == firstParamValue) {
                        //Firstly, build subarray of parameter sizes, starting from the chosen option [firstParamValue]
                        //FOR EXAMPLE: CMD 0x235 and firstParamValue = 5

                        // { 0xFF, 2,  
                        // 0, 1,   2,       
                        // 1, 3,   2, 2, 2, 
                        // 2, 0,            
                        // 3, 3,   2, 2, 2, 
                        // 4, 2,   2, 2,    
                        // 5, 3,   (2, 2, 2) => this will be the parameters subarray 
                        // 6, 1,   2
                        // },      
                        byte[] subParametersSize = parametersSizeArr.SubArray(i + 2, paramLength++);

                        //Create a slightly bigger temp array 
                        byte[] temp = new byte[1 + subParametersSize.Length];

                        //Store the size of the firstParamValue there
                        temp[0] = firstParamSize;

                        //Then copy the whole subarray of parameter sizes
                        Array.Copy(subParametersSize, 0, temp, 1, temp.Length - 1);

                        //Replace the original parametersSizeArr with the new array
                        parametersSizeArr = temp;
                        found = true;
                        break;
                    }

                    i += 2 + paramLength;
                    optionsCount++;
                }

                if (!found) {
                    MessageBox.Show($"Command {nameParts[0]} is a special Script Command.\n" +
                                    $"The value of the first parameter must be a number in the range [0 - {optionsCount}].\n\n" +
                                    $"Line {lineNumber}: {wholeLine}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    id = null;
                    return;
                }
            } else if (parametersSizeArr.Length == 1 && parametersSizeArr.First() == 0) {
                paramLength = 0;
            } else {
                paramLength = parametersSizeArr.Length;
            }

            if (nameParts.Length - 1 == paramLength) {
                for (int i = paramsProcessed; i < paramLength; i++) {
                    Console.WriteLine($"Parameter #{i}: {nameParts[i + 1]}");

                    if (RomInfo.ScriptComparisonOperatorsReverseDict.TryGetValue(nameParts[i + 1].ToLower(), out cmdID)) { //Check succeeds when command is like "asdfg LESS" or "asdfg DIFFERENT"
                        cmdParams.Add(new byte[] { (byte)cmdID });
                    } else { //Not a comparison
                        /* Convert strings of parameters to the correct datatypes */
                        NumberStyles numStyle = nameParts[i + 1].GetNumberStyle();
                        nameParts[i + 1] = nameParts[i + 1].PurgeSpecial(ScriptFile.specialChars);

                        int result = 0;

                        try
                        {
                            result = int.Parse(nameParts[i + 1], numStyle);
                        }
                        catch (FormatException)
                        {
                            try
                            {
                                string paramToCheck = RestoreSpaces(nameParts[i + 1]);
                                var first = ScriptDatabase.specialOverworlds.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                if (!string.IsNullOrWhiteSpace(first.Value))
                                {
                                    result = first.Key;
                                }
                                else
                                {
                                    var direction = ScriptDatabase.overworldDirections.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                    if (!string.IsNullOrWhiteSpace(direction.Value))
                                    {
                                        result = direction.Key;
                                    }
                                    else
                                    {
                                        var pokemon = ScriptDatabase.pokemonNames.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                        if (!string.IsNullOrWhiteSpace(pokemon.Value))
                                        {
                                            result = pokemon.Key;
                                        }
                                        else
                                        {
                                            var item = ScriptDatabase.itemNames.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                            if (!string.IsNullOrWhiteSpace(item.Value))
                                            {
                                                result = item.Key;
                                            }
                                            else
                                            {
                                                var move = ScriptDatabase.moveNames.FirstOrDefault(x => x.Value.IgnoreCaseEquals(paramToCheck));
                                                if (!string.IsNullOrWhiteSpace(move.Value))
                                                {
                                                    result = move.Key;
                                                }
                                                else
                                                {
                                                    MessageBox.Show($"Argument {paramToCheck} couldn't be parsed as a valid Condition, Overworld ID, Direction ID, Pokemon ID, Item ID, Move ID, Script, Function or Action number.\n\n" +
                                                                $"Line {lineNumber}: {wholeLine}", "Invalid identifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    id = null;
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (ArgumentException ex)
                            {
                                MessageBox.Show($"{ex.Message}\n\nLine {lineNumber}: {wholeLine}",
                                    "Invalid syntax", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                id = null;
                                return;
                            }
                        }

                        try
                        {
                            cmdParams.Add(result.ToByteArrayChooseSize(parametersSizeArr[i]));
                        }
                        catch (OverflowException)
                        {
                            MessageBox.Show($"Argument {nameParts[i + 1]} at line {lineNumber} is not in the range [0, {Math.Pow(2, 8 * parametersSizeArr[i]) - 1}].", "Argument error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            id = null;
                        }
                    }
                }
            } else {
                MessageBox.Show($"Wrong number of parameters for command {nameParts[0]} at line {lineNumber}.\n" +
                                $"Received: {nameParts.Length - 1}\n" +
                                $"Expected: {paramLength}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                id = null;
            }
        }

        private string ProcessBracketedItems(string line)
        {
            // Early exit if no brackets
            if (!line.Contains('[')) return line;

            StringBuilder result = new StringBuilder(line);
            int currentPos = 0;

            while (true)
            {
                int start = result.ToString().IndexOf('[', currentPos);
                if (start == -1) break;

                int end = result.ToString().IndexOf(']', start);
                if (end == -1) break;

                // Process only the current bracket pair
                for (int i = start + 1; i < end; i++)
                {
                    if (result[i] == ' ')
                    {
                        result[i] = '§';
                    }
                }

                currentPos = end + 1;
            }

            return result.ToString();
        }

        private int LevenshteinDistance(string s1, string s2)
        {
            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(
                        d[i - 1, j] + 1,
                        d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[s1.Length, s2.Length];
        }

        private string FindClosestMatch(string input, IEnumerable<string> possibilities, int threshold = 3)
        {
            // Remove brackets and spaces for comparison
            input = input.Trim('[', ']').Replace(" ", "").ToLower();

            var closest = possibilities
                .Select(x => new {
                    Name = x,
                    Distance = LevenshteinDistance(
                        input,
                        x.Replace(" ", "").ToLower()
                    )
                })
                .Where(x => x.Distance <= threshold)
                .OrderBy(x => x.Distance)
                .FirstOrDefault();

            return closest?.Name;
        }

        private string RestoreSpaces(string parameter)
        {
            if (!parameter.StartsWith("[") || !parameter.EndsWith("]"))
                return parameter;

            string content = parameter.Substring(1, parameter.Length - 2);

            // First restore any § back to spaces for comparison
            string contentWithSpaces = content.Replace("§", " ");

            // Check for Pokemon names first
            var pokemon = ScriptDatabase.pokemonNames.FirstOrDefault(x =>
                x.Value.IgnoreCaseEquals(contentWithSpaces));
            if (!string.IsNullOrWhiteSpace(pokemon.Value))
            {
                return pokemon.Value;
            }

            // Then check for Item names
            var item = ScriptDatabase.itemNames.FirstOrDefault(x =>
                x.Value.IgnoreCaseEquals(contentWithSpaces));
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                return item.Value;
            }

            // Then check for Move names
            var move = ScriptDatabase.moveNames.FirstOrDefault(x =>
                x.Value.IgnoreCaseEquals(contentWithSpaces));
            if (!string.IsNullOrWhiteSpace(move.Value))
            {
                return move.Value;
            }

            // If it's a number in brackets, provide suggestions
            if (int.TryParse(content, out int numericValue))
            {
                string suggestion = "";
                if (ScriptDatabase.itemNames.TryGetValue((ushort)numericValue, out string itemName))
                {
                    suggestion = $"\nDid you mean [{itemName}]?";
                }
                else if (ScriptDatabase.pokemonNames.TryGetValue((ushort)numericValue, out string pokemonName))
                {
                    suggestion = $"\nDid you mean [{pokemonName}]?";
                }
                else if (ScriptDatabase.moveNames.TryGetValue((ushort)numericValue, out string moveName))
                {
                    suggestion = $"\nDid you mean [{moveName}]?";
                }
                throw new ArgumentException($"Invalid syntax: Numbers should not be wrapped in brackets: '{parameter}'{suggestion}");
            }

            string closestItem = FindClosestMatch(contentWithSpaces, ScriptDatabase.itemNames.Values);
            if (!string.IsNullOrWhiteSpace(closestItem))
            {
                throw new ArgumentException($"'{parameter}' is not a valid Item name.\nDid you mean [{closestItem}]?");
            }

            string closestPokemon = FindClosestMatch(contentWithSpaces, ScriptDatabase.pokemonNames.Values);
            if (!string.IsNullOrWhiteSpace(closestPokemon))
            {
                throw new ArgumentException($"'{parameter}' is not a valid Pokemon name.\nDid you mean [{closestPokemon}]?");
            }

            string closestMove = FindClosestMatch(contentWithSpaces, ScriptDatabase.moveNames.Values);
            if (!string.IsNullOrWhiteSpace(closestMove))
            {
                throw new ArgumentException($"'{parameter}' is not a valid Move name.\nDid you mean [{closestMove}]?");
            }

            return contentWithSpaces;
        }
        

        private string FormatNumber(byte[] par, ParamTypeEnum paramType = ParamTypeEnum.INTEGER) {
            //number acquisition
            uint num;
            if (par.Length == 0) {
                return "";
            } else if (par.Length == 1) {
                num = par[0];
            } else if (par.Length == 2) {
                num = BitConverter.ToUInt16(par, 0);
            } else if (par.Length == 4) {
                num = BitConverter.ToUInt32(par, 0);
            } else {
                throw new InvalidOperationException();
            }

            return FormatNumber(num, paramType);
        }

        private string FormatNumber(uint num, ParamTypeEnum paramType = ParamTypeEnum.INTEGER) {
            //differentiate depending on param type
            string formatOverride;
            string prefix;

            if (SettingsManager.Settings.scriptEditorFormatPreference == (int)NumberStyles.HexNumber) {
                formatOverride = "X";
                prefix = "0x";
            } else { //(Properties.Settings.Default.scriptEditorFormatPreference == NumberStyles.Integer)
                formatOverride = "D";
                prefix = "";
            }

            string outp = "";

            switch (paramType) {
                case ParamTypeEnum.CMD_NUMBER:
                    return "CMD_" + prefix + num.ToString(formatOverride + '3');

                case ParamTypeEnum.FUNCTION_ID:
                    return ScriptFile.ContainerTypes.Function.ToString() + "#" + num;

                case ParamTypeEnum.ACTION_ID:
                    return ScriptFile.ContainerTypes.Action.ToString() + "#" + num;

                case ParamTypeEnum.OW_MOVEMENT_TYPE:
                    if (num < 4000) {
                        outp += "Move.";
                    }

                    goto default;

                case ParamTypeEnum.OW_ID: {
                        if (ScriptDatabase.specialOverworlds.TryGetValue((ushort)num, out string output)) {
                            return output;
                        } else {
                            if (num < 4000) {
                                outp += $"{Event.EventType.Overworld}.";
                            }

                            goto default;
                        }
                    }
                case ParamTypeEnum.OW_DIRECTION: {
                        if (ScriptDatabase.overworldDirections.TryGetValue((byte)num, out string output)) {
                            return output;
                        } else {
                            if (num < 4000) {
                                outp += $"Direction.";
                            }

                            goto default;
                        }
                    }
                case ParamTypeEnum.POKEMON_NAME:
                    {
                        if (ScriptDatabase.pokemonNames.TryGetValue((ushort)num, out string output))
                        {
                            return $"[{output}]";  // Return the already-bracketed name directly
                        }
                        else
                        {
                            if (num < 4000)
                            {
                                outp += $"Pokemon.";
                            }
                            goto default;
                        }
                    }
                case ParamTypeEnum.ITEM_NAME:
                    {
                        if (ScriptDatabase.itemNames.TryGetValue((ushort)num, out string output))
                        {
                            return $"[{output}]";
                        }
                        else
                        {
                            if (num < 4000)
                            {
                                outp += $"Item.";
                            }

                            goto default;
                        }
                    }
                case ParamTypeEnum.MOVE_NAME:
                    {
                        if (ScriptDatabase.moveNames.TryGetValue((ushort)num, out string output))
                        {
                            return $"[{output}]";
                        }
                        else
                        {
                            if (num < 4000)
                            {
                                outp += $"Move.";
                            }

                            goto default;
                        }
                    }
                default:
                    if (SettingsManager.Settings.scriptEditorFormatPreference == (int)NumberStyles.None) {
                        if (num >= 4000) {
                            formatOverride = "X";
                            prefix = "0x";
                        }
                    }

                    outp += prefix + num.ToString(formatOverride);
                    break;
            }

            return outp;
        }

        private string FormatParameter(ScriptParameter param, ParamTypeEnum paramType = ParamTypeEnum.INTEGER) {
            // For jump-to-label parameters, return the label name
            if (param.Type == ScriptParameter.ParameterType.RelativeJump && !string.IsNullOrEmpty(param.TargetLabel)) {
                return param.TargetLabel;
            }

            // Otherwise handle the numeric value
            return FormatNumber(param.RawData, paramType);
        }
        public override string ToString() {
            return name + " (" + ((ushort)id).ToString("X") + ")";
        }

        private string FormatLabelReference(string labelName) {
            return labelName;
        }
    }
}

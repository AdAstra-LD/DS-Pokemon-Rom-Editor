using DSPRE.ROMFiles;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static DSPRE.RomInfo;

namespace DSPRE {
    public enum EvolutionMethod : short {
        None = 0,
        Friendship220 = 1,
        Friendship220_Day = 2,
        Friendship220_Night = 3,
        LevelingUp = 4,
        Trade = 5,
        Trade_HeldItem = 6,
        Item = 7,
        Atk_Greater_Def = 8,
        Atk_Equal_Def = 9,
        Def_Greater_Atk = 10,
        Personality1 = 11,
        Personality2 = 12,
        FreeSpaceCheck = 13,
        Shedinja = 14,
        BeautyThreshold = 15,
        ItemMale = 16,
        ItemFemale = 17,
        HeldItem_Day = 18,
        HeldItem_Night = 19,
        KnowsMove = 20,
        PartyPokemonPresence = 21,
        LevelingUp_Male = 22,
        LevelingUp_Female = 23,

        Loc_MtCoronet = LevelingUp_Female + 1,
        Loc_EternaForest = LevelingUp_Female + 2,
        Loc_Route217 = LevelingUp_Female + 3
    }

    public enum EvolutionParamMeaning {
        Ignored,
        FromLevel,
        ItemName,
        MoveName,
        PokemonName,
        BeautyValue,
    }
    
    public struct EvolutionData {
        public EvolutionMethod method;
        public short param;
        public short target;

        public bool isValid() {
            if (method == EvolutionMethod.None) {
                return false;
            }

            if (method == EvolutionMethod.LevelingUp || 
                method == EvolutionMethod.LevelingUp_Male || 
                method == EvolutionMethod.LevelingUp_Female) { 

                return param > 0 && param <= 100;
            }

            if (target <= 0) {
                return false;
            }

            return true;
        }
    }
    public class EvolutionFile : RomFile {
        public const int numEvolutions = 7;

        public EvolutionData[] data;

        public static readonly Dictionary<EvolutionMethod, EvolutionParamMeaning> evoDescriptions = new Dictionary<EvolutionMethod, EvolutionParamMeaning>() {
            [EvolutionMethod.None] =    EvolutionParamMeaning.Ignored,

            [EvolutionMethod.Friendship220] =       EvolutionParamMeaning.Ignored,
            [EvolutionMethod.Friendship220_Day] =   EvolutionParamMeaning.Ignored,
            [EvolutionMethod.Friendship220_Night] = EvolutionParamMeaning.Ignored,
            [EvolutionMethod.LevelingUp] =          EvolutionParamMeaning.FromLevel,
            [EvolutionMethod.Trade] =               EvolutionParamMeaning.Ignored,

            [EvolutionMethod.Trade_HeldItem] =  EvolutionParamMeaning.ItemName,
            [EvolutionMethod.Item] =            EvolutionParamMeaning.ItemName,

            [EvolutionMethod.Atk_Greater_Def] = EvolutionParamMeaning.FromLevel,
            [EvolutionMethod.Atk_Equal_Def] =   EvolutionParamMeaning.FromLevel,
            [EvolutionMethod.Def_Greater_Atk] = EvolutionParamMeaning.FromLevel,
            [EvolutionMethod.Personality1] =    EvolutionParamMeaning.FromLevel,
            [EvolutionMethod.Personality2] =    EvolutionParamMeaning.FromLevel,
            [EvolutionMethod.FreeSpaceCheck] =  EvolutionParamMeaning.FromLevel,
            [EvolutionMethod.Shedinja] =        EvolutionParamMeaning.FromLevel,

            [EvolutionMethod.BeautyThreshold] = EvolutionParamMeaning.BeautyValue,
            
            [EvolutionMethod.ItemMale] =        EvolutionParamMeaning.ItemName,
            [EvolutionMethod.ItemFemale] =      EvolutionParamMeaning.ItemName,
            [EvolutionMethod.HeldItem_Day] =    EvolutionParamMeaning.ItemName,
            [EvolutionMethod.HeldItem_Night] =  EvolutionParamMeaning.ItemName,

            [EvolutionMethod.KnowsMove] =       EvolutionParamMeaning.MoveName,
            
            [EvolutionMethod.PartyPokemonPresence] = EvolutionParamMeaning.PokemonName,

            [EvolutionMethod.LevelingUp_Male] =   EvolutionParamMeaning.FromLevel,
            [EvolutionMethod.LevelingUp_Female] = EvolutionParamMeaning.FromLevel,

            [EvolutionMethod.Loc_EternaForest] =     EvolutionParamMeaning.Ignored,
            [EvolutionMethod.Loc_MtCoronet] =        EvolutionParamMeaning.Ignored,
            [EvolutionMethod.Loc_Route217] =         EvolutionParamMeaning.Ignored,
        };

        public EvolutionFile(Stream stream) {
            data = new EvolutionData[numEvolutions];

            using (BinaryReader reader = new BinaryReader(stream)) {
                for (int i = 0; i < numEvolutions; i++) {
                    data[i].method = (EvolutionMethod)reader.ReadInt16();
                    data[i].param = reader.ReadInt16();
                    data[i].target = reader.ReadInt16();
                }
            }
        }

        public EvolutionFile(int ID) : this(new FileStream(RomInfo.gameDirs[DirNames.evolutions].unpackedDir + "\\" + ID.ToString("D4"), FileMode.Open)) { }

        public EvolutionFile() { }

        public override byte[] ToByteArray() {
            using (MemoryStream memoryStream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(memoryStream)) {
                    foreach (EvolutionData evData in data) {
                        if (evData.isValid()) {
                            writer.Write((short)evData.method);
                            writer.Write(evData.param);
                            writer.Write(evData.target);
                        }
                    }

                    //If the file is smaller than the minimum size, pad it with 00
                    int size = Marshal.SizeOf(typeof(EvolutionData));
                    int minSize = numEvolutions * size + 2; //2B pad
                    if (memoryStream.Length < minSize) {
                        memoryStream.SetLength(minSize);
                    }

                }
                return memoryStream.ToArray();
            }
        }

        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(DirNames.evolutions, IDtoReplace, showSuccessMessage);
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Evolution data", "bin", suggestedFileName, showSuccessMessage);
        }
    }
}
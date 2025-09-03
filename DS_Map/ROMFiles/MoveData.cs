using System.IO;
using DSPRE.ROMFiles;
using System;
using System.Windows.Forms;
using System.Text;
using System.Collections.Generic;
using static DSPRE.RomInfo;

namespace DSPRE {
    public class MoveData : RomFile {
        public enum AttackRange : ushort {
            SELECTION = 0,
            VARIABLE = (1 << 0),
            RANDOM = (1 << 1),
            BOTH = (1 << 2),
            FOES_AND_ALLY = (1 << 3),
            USER = (1 << 4),
            USER_SIDE = (1 << 5),
            ACTIVE_FIELD = (1 << 6),
            OPPONENTS_FIELD = (1 << 7),
            ALLY = (1 << 8),
            ACUPRESSURE = (1 << 9),
            ME_FIRST = (1 << 10)
        };
        public enum MoveSplit : byte {
            PHYSICAL = 0,
            SPECIAL,
            STATUS,
        };
        public enum ContestCondition : byte {
            COOL = 0,
            BEAUTIFUL,
            CUTE,
            SMART,
            TOUGH,
        };

        public enum MoveFlags : byte {
            NONE = 0,
            CONTACT = (1 << 0),
            PROTECT = (1 << 1),
            MAGIC_COAT = (1 << 2),
            SNATCH = (1 << 3),
            MIRROR_MOVE = (1 << 4),
            KINGSROCK = (1 << 5),
            KEEP_HP_BAR = (1 << 6),
            DEL_SHADOW = (1 << 7)
        }


        public ushort battleeffect;
        public MoveSplit split;
        public byte damage;

        public PokemonType movetype;
        public byte accuracy;
        public byte pp;
        public byte sideEffectProbability;

        public ushort target;
        public sbyte priority;
        public byte flagField;

        public byte contestAppeal;
        public ContestCondition contestConditionType;

        public MoveData(Stream stream) {
            using (BinaryReader reader = new BinaryReader(stream)) {
                this.battleeffect = reader.ReadUInt16();
                this.split = (MoveSplit)reader.ReadByte();
                this.damage = reader.ReadByte();

                this.movetype = (PokemonType)reader.ReadByte();
                this.accuracy = reader.ReadByte();
                this.pp = reader.ReadByte();
                this.sideEffectProbability = reader.ReadByte();
                this.target = reader.ReadUInt16(); //bitfield
                this.priority = reader.ReadSByte();
                this.flagField = reader.ReadByte();

                this.contestAppeal = reader.ReadByte();
                this.contestConditionType = (ContestCondition)reader.ReadByte();
            }
        }

        public MoveData(int ID) : this(new FileStream(RomInfo.gameDirs[DirNames.moveData].unpackedDir + "\\" + ID.ToString("D4"), FileMode.Open)) { }

        public override byte[] ToByteArray() {
            using (MemoryStream memoryStream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(memoryStream)) {
                    writer.Write(this.battleeffect);
                    writer.Write((byte)this.split);
                    writer.Write(this.damage);

                    writer.Write((byte)this.movetype);
                    writer.Write(this.accuracy);
                    writer.Write(this.pp);
                    writer.Write(this.sideEffectProbability);
                    writer.Write((ushort)this.target);
                    writer.Write(this.priority);
                    writer.Write(this.flagField);

                    writer.Write(this.contestAppeal);
                    writer.Write((byte)this.contestConditionType);

                    writer.Write((ushort)0); //Filler
                }

                return memoryStream.ToArray();
            }
        }


        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(DirNames.moveData, IDtoReplace, showSuccessMessage);
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Move Data", "bin", suggestedFileName, showSuccessMessage);
        }

        public static void UpdateFromText(MoveData m, string[] split) {
            int target = 1;
            if (split.Length < target + 1) {
                return;
            } else {
                if (!Enum.TryParse(split[target], true, out m.movetype)) {
                    MessageBox.Show($"Malformed entry: \"{string.Join(" ", split)}\".\nMove type is unreadable: \"{split[target]}\"", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                target++;

                if (split.Length < target + 1) {
                    return;
                } else {
                    if (!Enum.TryParse(split[target], true, out m.split)) {
                        MessageBox.Show($"Malformed entry: \"{string.Join(" ", split)}\".\nMove split is unreadable: \"{split[target]}\"", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    target++;

                    if (split.Length < target + 1) {
                        return;
                    } else {
                        if (!byte.TryParse(split[target], out m.damage)) {
                            if (split[target].StartsWith("-") || split[target].StartsWith("—")) {
                                m.damage = 0;
                            } else {
                                MessageBox.Show($"Malformed entry: \"{string.Join(" ", split)}\".\nMove power is unreadable: \"{split[target]}\"", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                        }
                        target++;

                        if (split.Length < target + 1) {
                            return;
                        } else {
                            if (!byte.TryParse(split[target].Replace('%', ' '), out m.accuracy)) {
                                if (split[target].StartsWith("-") || split[target].StartsWith("—")) {
                                    m.accuracy = 0;
                                } else {
                                    MessageBox.Show($"Malformed entry: \"{string.Join(" ", split)}\".\nMove accuracy is unreadable: \"{split[target]}\"", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            target++;

                            if (split.Length < target + 1) {
                                return;
                            } else {
                                if (!byte.TryParse(split[target].Replace('%', ' '), out m.pp)) {
                                    MessageBox.Show($"Malformed entry: \"{string.Join(" ", split)}\".\nMove PP count is unreadable: \"{split[target]}\"", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            return;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(movetype.ToString());
            sb.Append('\t');

            sb.Append(split.ToString());
            sb.Append('\t');

            if (damage == 0) {
                sb.Append("-");
            } else {
                sb.Append(damage);
            }
            sb.Append('\t');

            if (accuracy == 0) {
                sb.Append("-");
            } else {
                sb.Append(accuracy);
                sb.Append('%');
            }
            sb.Append('\t');

            sb.Append(pp);

            return sb.ToString();
        }

        public override bool Equals(object obj) {
            return obj is MoveData data &&
                   battleeffect == data.battleeffect &&
                   split == data.split &&
                   damage == data.damage &&
                   movetype == data.movetype &&
                   accuracy == data.accuracy &&
                   pp == data.pp &&
                   sideEffectProbability == data.sideEffectProbability &&
                   target == data.target &&
                   priority == data.priority &&
                   flagField == data.flagField &&
                   contestAppeal == data.contestAppeal &&
                   contestConditionType == data.contestConditionType;
        }

        public override int GetHashCode() {
            int hashCode = -1756630415;
            hashCode = hashCode * -1521134295 + battleeffect.GetHashCode();
            hashCode = hashCode * -1521134295 + split.GetHashCode();
            hashCode = hashCode * -1521134295 + damage.GetHashCode();
            hashCode = hashCode * -1521134295 + movetype.GetHashCode();
            hashCode = hashCode * -1521134295 + accuracy.GetHashCode();
            hashCode = hashCode * -1521134295 + pp.GetHashCode();
            hashCode = hashCode * -1521134295 + sideEffectProbability.GetHashCode();
            hashCode = hashCode * -1521134295 + target.GetHashCode();
            hashCode = hashCode * -1521134295 + priority.GetHashCode();
            hashCode = hashCode * -1521134295 + flagField.GetHashCode();
            hashCode = hashCode * -1521134295 + contestAppeal.GetHashCode();
            hashCode = hashCode * -1521134295 + contestConditionType.GetHashCode();
            return hashCode;
        }
    }
}
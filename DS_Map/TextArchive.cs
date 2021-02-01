using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;

namespace DSPRE
{
	/// <summary>
	/// Class to store message data from DS Pokémon games
	/// </summary>
	public class TextArchive
	{
        #region Fields (2)
        public List<string> messages = new List<string>();
        public int initialKey;
        #endregion Fields

        #region Constructors (1)
        public TextArchive(FileStream messageStream)
        {
            ResourceManager GetChar = new ResourceManager("DSPRE.Resources.ReadText", Assembly.GetExecutingAssembly());
            BinaryReader readText = new BinaryReader(messageStream);
            int stringCount;
            try {
                stringCount = readText.ReadUInt16();
            } catch (EndOfStreamException) {
                MessageBox.Show("Error loading text file.\n", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                readText.Close();
                return;
            }
            initialKey = readText.ReadUInt16();
            int key1 = (initialKey * 0x2FD) & 0xFFFF;
            int key2 = 0;
            int realKey = 0;
            bool specialCharON = false;
            int[] currentOffset = new int[stringCount];
            int[] currentSize = new int[stringCount];
            string[] currentPokemon = new string[stringCount];
            int car = 0;
            bool compressed = new bool();

            for (int i = 0; i < stringCount; i++) // Reads and stores string offsets and sizes
            {
                key2 = (key1 * (i + 1) & 0xFFFF);
                realKey = key2 | (key2 << 16);
                currentOffset[i] = ((int)readText.ReadUInt32()) ^ realKey;
                currentSize[i] = ((int)readText.ReadUInt32()) ^ realKey;
            }
            for (int i = 0; i < stringCount; i++) // Adds new string
            {
                key1 = (0x91BD3 * (i + 1)) & 0xFFFF;
                readText.BaseStream.Position = currentOffset[i];
                StringBuilder pokemonText = new StringBuilder("");

                for (int j = 0; j < currentSize[i]; j++) // Adds new characters to string
                {
                    car = (readText.ReadUInt16()) ^ key1;

                    switch (car) // Special characters
                    {
                        case 0xE000:
                            pokemonText.Append(@"\n");
                            break;
                        case 0x25BC:
                            pokemonText.Append(@"\r");
                            break;
                        case 0x25BD:
                            pokemonText.Append(@"\f");
                            break;
                        case 0xF100:
                            compressed = true;
                            break;
                        case 0xFFFE:
                            pokemonText.Append(@"\v");
                            specialCharON = true;
                            break;
                        case 0xFFFF:
                            pokemonText.Append("");
                            break;
                        default:
                            if (specialCharON) {
                                pokemonText.Append(car.ToString("X4"));
                                specialCharON = false;
                            } else if (compressed) {
                                #region Compressed String
                                int shift = 0;
                                int trans = 0;
                                string uncomp = "";
                                while (true) {
                                    int tmp = car >> shift;
                                    int tmp1 = tmp;
                                    if (shift >= 0xF) {
                                        shift -= 0xF;
                                        if (shift > 0) {
                                            tmp1 = (trans | ((car << (9 - shift)) & 0x1FF));
                                            if ((tmp1 & 0xFF) == 0xFF) {
                                                break;
                                            }
                                            if (tmp1 != 0x0 && tmp1 != 0x1) {
                                                string character = GetChar.GetString(tmp1.ToString("X4"));
                                                pokemonText.Append(character);
                                                if (character == null) {
                                                    pokemonText.Append(@"\x" + tmp1.ToString("X4"));
                                                }
                                            }
                                        }
                                    } else {
                                        tmp1 = ((car >> shift) & 0x1FF);
                                        if ((tmp1 & 0xFF) == 0xFF) {
                                            break;
                                        }
                                        if (tmp1 != 0x0 && tmp1 != 0x1) {
                                            string character = GetChar.GetString(tmp1.ToString("X4"));
                                            pokemonText.Append(character);
                                            if (character == null) {
                                                pokemonText.Append(@"\x" + tmp1.ToString("X4"));
                                            }
                                        }
                                        shift += 9;
                                        if (shift < 0xF) {
                                            trans = ((car >> shift) & 0x1FF);
                                            shift += 9;
                                        }
                                        key1 += 0x493D;
                                        key1 &= 0xFFFF;
                                        car = Convert.ToUInt16(readText.ReadUInt16() ^ key1);
                                        j++;
                                    }
                                }
                                #endregion
                                pokemonText.Append(uncomp);
                            } else {
                                string character = GetChar.GetString(car.ToString("X4"));
                                pokemonText.Append(character);
                                if (character == null) {
                                    pokemonText.Append(@"\x" + car.ToString("X4"));
                                }
                            }
                            break;
                    }
                    key1 += 0x493D;
                    key1 &= 0xFFFF;
                }
                messages.Add(pokemonText.ToString());
            }

            readText.Dispose();
        }
        public TextArchive(int ID) : this( (new FileStream(RomInfo.textArchivesPath + "\\" + ID.ToString("D4"), FileMode.Open))) {
        }
        #endregion

        #region Methods (2)
        public int[] EncodeString(string currentMessage, int stringIndex, int stringSize) { // Converts string to hex characters 
            ResourceManager GetByte = new ResourceManager("DSPRE.Resources.WriteText", Assembly.GetExecutingAssembly());

            int[] pokemonMessage = new int[stringSize - 1];
            var charArray = currentMessage.ToCharArray();
            int count = 0;
            for (int i = 0; i < currentMessage.Length; i++) {
                if (charArray[i] == '\\') {
                    if (charArray[i + 1] == 'r') {
                        pokemonMessage[count] = 0x25BC;
                        i++;
                    } else {
                        if (charArray[i + 1] == 'n') {
                            pokemonMessage[count] = 0xE000;
                            i++;
                        } else {
                            if (charArray[i + 1] == 'f') {
                                pokemonMessage[count] = 0x25BD;
                                i++;
                            } else {
                                if (charArray[i + 1] == 'v') {
                                    pokemonMessage[count] = 0xFFFE;
                                    count++;
                                    string characterID = ((char)charArray[i + 2]).ToString() + ((char)charArray[i + 3]).ToString() + ((char)charArray[i + 4]).ToString() + ((char)charArray[i + 5]).ToString();
                                    pokemonMessage[count] = (int)Convert.ToUInt32(characterID, 16);
                                    i += 5;
                                } else {
                                    if (charArray[i + 1] == 'x' && charArray[i + 2] == '0' && charArray[i + 3] == '0' && charArray[i + 4] == '0' && charArray[i + 5] == '0') {
                                        pokemonMessage[count] = 0x0000;
                                        i += 5;
                                    } else {
                                        if (charArray[i + 1] == 'x' && charArray[i + 2] == '0' && charArray[i + 3] == '0' && charArray[i + 4] == '0' && charArray[i + 5] == '1') {
                                            pokemonMessage[count] = 0x0001;
                                            i += 5;
                                        } else {
                                            string characterID = ((char)charArray[i + 2]).ToString() + ((char)charArray[i + 3]).ToString() + ((char)charArray[i + 4]).ToString() + ((char)charArray[i + 5]).ToString();
                                            pokemonMessage[count] = (int)Convert.ToUInt32(characterID, 16);
                                            i += 5;
                                        }
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if (charArray[i] == '[') {
                        if (charArray[i + 1] == 'P') {
                            pokemonMessage[count] = 0x01E0;
                            i += 3;
                        }
                        if (charArray[i + 1] == 'M') {
                            pokemonMessage[count] = 0x01E1;
                            i += 3;
                        }
                    } else {
                        pokemonMessage[count] = (int)Convert.ToUInt32(GetByte.GetString(((int)charArray[i]).ToString()), 16);
                    }
                }
                count++;
            }
            return pokemonMessage;
        }
        public int GetStringLength(string currentMessage) { // Calculates string length 
            int count = 0;
            var charArray = currentMessage.ToCharArray();
            for (int i = 0; i < currentMessage.Length; i++) {
                if (charArray[i] == '\\') {
                    if (charArray[i + 1] == 'r') {
                        count++;
                        i++;
                    } else {
                        if (charArray[i + 1] == 'n') {
                            count++;
                            i++;
                        } else {
                            if (charArray[i + 1] == 'f') {
                                count++;
                                i++;
                            } else {
                                if (charArray[i + 1] == 'v') {
                                    count += 2;
                                    i += 5;
                                } else {
                                    if (charArray[i + 1] == 'x' && charArray[i + 2] == '0' && charArray[i + 3] == '0' && charArray[i + 4] == '0' && charArray[i + 5] == '0') {
                                        count++;
                                        i += 5;
                                    } else {
                                        if (charArray[i + 1] == 'x' && charArray[i + 2] == '0' && charArray[i + 3] == '0' && charArray[i + 4] == '0' && charArray[i + 5] == '1') {
                                            count++;
                                            i += 5;
                                        } else {
                                            count++;
                                            i += 5;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if (charArray[i] == '[') {
                        if (charArray[i + 1] == 'P') {
                            count++;
                            i += 3;
                        }
                        if (charArray[i + 1] == 'M') {
                            count++;
                            i += 3;
                        }
                    } else {
                        count++;
                    }
                }
            }
            count++;
            return count;
        }
        public byte[] toByteArray(List<string> msgSource) {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                writer.Write((UInt16)msgSource.Count);
                writer.Write((UInt16)initialKey);
                int key = (initialKey * 0x2FD) & 0xFFFF;
                int key2 = 0;
                int realKey = 0;
                int offset = 0x4 + (msgSource.Count * 8);
                int[] stringSize = new int[msgSource.Count];

                for (int i = 0; i < msgSource.Count; i++) {// Reads and stores string offsets and sizes
                    key2 = (key * (i + 1) & 0xFFFF);
                    realKey = key2 | (key2 << 16);
                    writer.Write(offset ^ realKey);
                    int length = GetStringLength(msgSource[i]);
                    stringSize[i] = length;
                    writer.Write(length ^ realKey);
                    offset += length * 2;
                }
                for (int i = 0; i < msgSource.Count; i++) { // Encodes strings and writes them to file
                    key = (0x91BD3 * (i + 1)) & 0xFFFF;
                    int[] currentString = EncodeString(msgSource[i], i, stringSize[i]);
                    for (int j = 0; j < stringSize[i] - 1; j++) {
                        writer.Write((UInt16)(currentString[j] ^ key));
                        key += 0x493D;
                        key &= 0xFFFF;
                    }
                    writer.Write((UInt16)(0xFFFF ^ key));
                }
            }
            return newData.ToArray();
        }
        public byte[] ToByteArray() {
            return toByteArray(messages);
        }
        public void SaveToFile(string path) {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
                writer.Write(this.ToByteArray());
        }
        public void SaveToFileDefaultDir(int IDtoReplace) {
            string path = RomInfo.textArchivesPath + "\\" + IDtoReplace.ToString("D4");
            this.SaveToFile(path);
        }
        public void SaveToFileExplorePath(string suggestedFileName) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Gen IV Text Archive (*.msg)|*.msg";

            if (suggestedFileName != null && suggestedFileName != "")
                sf.FileName = suggestedFileName;
            if (sf.ShowDialog() != DialogResult.OK)
                return;

            this.SaveToFile(sf.FileName);
        }
        #endregion
    }
}
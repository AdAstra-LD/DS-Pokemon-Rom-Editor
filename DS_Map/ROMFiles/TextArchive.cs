using DSPRE.Editors;
using DSPRE.MessageEnc;
using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles
{
    /// <summary>
    /// Class to store message data from DS Pokémon games
    /// </summary>
    public class TextArchive : RomFile
    {
        #region Fields

        public int ID { get;}
        public List<string> messages;
        private UInt16 key = 0;

        #endregion Fields

        #region Constructors (1)

        public TextArchive(int ID, List<string> msg = null)
        {
            this.ID = ID;

            if (msg != null)
            {
                messages = msg;
                return;
            }

            // First try to read from plain text file if it exists
            if (TryReadPlainTextFile())
            {
                return;
            }

            // If not, extract from the .bin file
            if (!ReadFromBinFile())
            {
                MessageBox.Show($"Failed to read messages from .bin file {ID:D4}. Contents were replaced with empty message!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                messages = new List<string> { "" };
                return;
            }

        }


        #endregion Constructors (1)

        #region Methods (2)

        public static (string binPath, string txtPath) GetFilePaths(int ID)
        {
            string baseDir = gameDirs[DirNames.textArchives].unpackedDir;
            string binPath = Path.Combine(baseDir, $"{ID:D4}");
            string expandedDir = Path.Combine(RomInfo.workDir, "expanded", "textArchives");
            string txtPath = Path.Combine(expandedDir, $"{ID:D4}.txt");
            return (binPath, txtPath);
        }

        public static bool BuildRequiredBins()
        {
            string expandedDir = Path.Combine(RomInfo.workDir, "expanded", "textArchives");

            if (!Directory.Exists(expandedDir))
            {
                AppLogger.Info("Text Archive: No expanded text archive directory found, skipping .bin rebuild.");
                return true;
            }

            var expandedTextFiles = Directory.GetFiles(expandedDir, "*.txt", SearchOption.AllDirectories);
            int newerBinCount = 0;

            for (int i = 0; i < expandedTextFiles.Length; i++)
            {
                string expandedTextFile = expandedTextFiles[i];
                string fileName = Path.GetFileNameWithoutExtension(expandedTextFile);

                int archiveID;

                try
                {
                    archiveID = int.Parse(fileName);
                }
                catch
                {
                    AppLogger.Error($"Skipping invalid text archive file name: {fileName}");
                    continue;
                }

                string binPath = TextArchive.GetFilePaths(archiveID).binPath;

                // Skip if .bin is newer than .txt
                if (File.Exists(binPath) && File.GetLastWriteTimeUtc(binPath) > File.GetLastWriteTimeUtc(expandedTextFile))
                {
                    newerBinCount++;
                    continue;
                }

                var textArchive = new TextArchive(archiveID);
                textArchive.SaveToDefaultDir(archiveID, false);
                // Update .txt last write time to prevent it being overwritten when reopening the ROM
                File.SetLastWriteTimeUtc(expandedTextFile, DateTime.UtcNow);
            }

            AppLogger.Info($"Text Archive: {expandedTextFiles.Length - newerBinCount} .bin files built from .txt, {newerBinCount} .bin files skipped because they were newer than the .txt");

            return true;
        }

        public List<string> GetSimpleTrainerNames()
        {
            List<string> simpleMessages = new List<string>();
            foreach (string msg in messages)
            {
                string simpleMsg = TextConverter.GetSimpleTrainerName(msg);
                simpleMessages.Add(simpleMsg);
            }
            return simpleMessages;
        }

        public bool SetSimpleTrainerName(int messageIndex, string newSimpleName)
        {
            if (messageIndex < 0)
            {
                AppLogger.Error($"Invalid message index {messageIndex} for Text Archive ID {ID:D4}");
                return false;
            }

            if (messageIndex >= messages.Count)
            {
                messages.Add("{TRAINER_NAME:" + newSimpleName + "}");
                return true;
            }

            string currentMessage = messages[messageIndex];
            string updatedMessage = TextConverter.GetProperTrainerName(currentMessage, newSimpleName);
            if (updatedMessage == currentMessage)
            {
                // No change made
                return false;
            }
            messages[messageIndex] = updatedMessage;
            return true;
        }

        private bool TryReadPlainTextFile()
        {
            string txtPath = GetFilePaths(ID).txtPath;
            string binPath = GetFilePaths(ID).binPath;

            if (!File.Exists(txtPath))
            {
                return false;
            }

            // If the .txt file is older than the .bin file, ignore it and re-extract from .bin
            if (File.GetLastWriteTimeUtc(txtPath) < File.GetLastWriteTimeUtc(binPath))
            {
                return false;
            }

            try
            {
                List<string> lines = File.ReadAllLines(txtPath).ToList();
                if (lines.Count == 0)
                {
                    AppLogger.Error($"Text file {txtPath} is empty. Bin file will be reextracted.");
                    return false;
                }

                // First line should be the key
                string firstLine = lines[0];
                if (!firstLine.StartsWith("# Key: "))
                {
                    AppLogger.Error($"Text file {txtPath} is missing the key in the first line. Bin file will be reextracted.");
                    return false;
                }

                string keyHex = firstLine.Substring(7).Trim();
                if (!UInt16.TryParse(keyHex.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out key))
                {
                    AppLogger.Error($"Text file {txtPath} has an invalid key format. Bin file will be reextracted.");
                    return false;
                }

                // Check for newline character in last line and add a blank line if needed
                // Since ReadAllLines() trims the newline, we read the last character of the file directly
                // I hate this - Yako
                using (FileStream fs = new FileStream(txtPath, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length > 0)
                    {
                        fs.Seek(-1, SeekOrigin.End);
                        int lastByte = fs.ReadByte();
                        if (lastByte == '\n' || lastByte == '\r')
                        {
                            lines.Add(string.Empty);
                        }
                    }
                    fs.Close();
                }

                // Remove the first line (the key) from the messages
                lines.RemoveAt(0);

                messages = lines;
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Error reading text file {txtPath}: {ex.Message}. Bin file will be reextracted.");
                return false;
            }
        }

        private bool ReadFromBinFile()
        {
            string binPath = GetFilePaths(ID).binPath;
            if (!File.Exists(binPath))
            {
                MessageBox.Show($"The .bin file for Text Archive ID {ID:D4} does not exist at the expected path: {binPath}", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            try
            {
                using (FileStream fs = new FileStream(binPath, FileMode.Open, FileAccess.Read))
                {
                    messages = TextConverter.ReadMessageFromStream(fs, out key);
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Error reading .bin file {binPath}: {ex.Message}");
                return false;
            }
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, messages);
        }

        public override byte[] ToByteArray()
        {
            Stream stream = new MemoryStream();
            if (!TextConverter.WriteMessagesToStream(ref stream, messages, key))
            {
                AppLogger.Error($"Failed to convert Text Archive ID {ID:D4} to byte array.");
            }

            return ((MemoryStream)stream).ToArray();
        }

        public void SaveToExpandedDir(int IDtoReplace, bool showSuccessMessage = true)
        {
            string baseDir = gameDirs[DirNames.textArchives].unpackedDir;
            string expandedDir = Path.Combine(RomInfo.workDir, "expanded", "textArchives");

            if (!Directory.Exists(expandedDir))
            {
                Directory.CreateDirectory(expandedDir);
            }

            string expandedPath = GetFilePaths(IDtoReplace).txtPath;

            var utf8WithoutBom = new UTF8Encoding(false);

            string firstLine = $"# Key: 0x{key:X4}";
            string textToSave = string.Join(Environment.NewLine, messages);
            textToSave = firstLine + Environment.NewLine + textToSave;

            File.WriteAllText(expandedPath, textToSave, utf8WithoutBom);
        }

        public void SaveToDefaultDir(int IDtoReplace, bool showSuccessMessage = true)
        {
            SaveToFileDefaultDir(DirNames.textArchives, IDtoReplace, showSuccessMessage);
        }

        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true)
        {
            SaveToFileExplorePath("Gen IV Text Archive", "msg", suggestedFileName, showSuccessMessage);
        }

        #endregion Methods (2)
    }
}
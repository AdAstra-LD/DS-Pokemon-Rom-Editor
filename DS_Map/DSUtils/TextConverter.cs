using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DSPRE
{
    internal class TextConverter
    {

        private static Dictionary<ushort, string> decodeMap;
        private static Dictionary<string, ushort> encodeMap;
        private static Dictionary<ushort, string> commandMap;

        private static bool mapsInitialized = false;

        public static Dictionary<ushort, string> GetDecodingMap()
        {
            if (!mapsInitialized)
            {
                InitializeCharMaps();
            }
            return decodeMap;
        }

        public static Dictionary<string, ushort> GetEncodingMap()
        {
            if (!mapsInitialized)
            {
                InitializeCharMaps();
            }
            return encodeMap;
        }

        private static void InitializeCharMaps()
        {
            decodeMap = new Dictionary<ushort, string>();
            encodeMap = new Dictionary<string, ushort>();
            commandMap = new Dictionary<ushort, string>();

            string charmapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "charmap.xml");
            if (!File.Exists(charmapPath))
                throw new FileNotFoundException("Charmap XML file not found.", charmapPath);

            var xml = XDocument.Load(charmapPath, LoadOptions.PreserveWhitespace);

            foreach (var entry in xml.Descendants("entry"))
            {
                var code = entry.Attribute("code")?.Value;
                var kind = entry.Attribute("kind")?.Value;
                var value = entry.Value;

                if (code == null || kind == null || value == null)
                {
                    AppLogger.Error("Found charmap entry with null value.");
                    continue;
                }

                ushort codeValue;
                string codeKind = kind.ToLower();
                string chars = value.ToString();

                if (!ushort.TryParse(code, System.Globalization.NumberStyles.HexNumber, null, out codeValue))
                {
                    AppLogger.Error($"Invalid code value in charmap: {code}");
                    continue;
                }

                if (codeKind == "char" || codeKind == "escape")
                {
                    decodeMap[codeValue] = chars;
                    encodeMap[chars] = codeValue;
                }
                else if (codeKind == "alias")
                {
                    encodeMap[chars] = codeValue;
                }
                else if (codeKind == "command")
                {
                    commandMap[codeValue] = chars;
                }
                else
                {
                    AppLogger.Error($"Unknown kind '{kind}' in charmap entry.");
                }
            }

            mapsInitialized = true;
        }

        public static List<string> ReadMessageFromStream(Stream stream, out UInt16 key)
        {
            List<string> messages = new List<string>();
            key = 0;

            using (BinaryReader reader = new BinaryReader(stream))
            {
                try
                {
                    // Read message count and key
                    UInt16 messageCount = reader.ReadUInt16();
                    key = reader.ReadUInt16();

                    var messageTable = new List<(int offset, int length)>(messageCount);

                    // Read and decrypt message table to obtain offsets and lengths
                    for (int i = 0; i < messageCount; i++)
                    {
                        int offset = reader.ReadInt32();
                        int length = reader.ReadInt32();

                        // Decrypt length and offset
                        int localKey = (765 * (i+1) * key) & 0xFFFF;
                        localKey |= (localKey << 16);
                        offset ^= localKey;
                        length ^= localKey;

                        messageTable.Add((offset, length));
                    }

                    int msgIndex = 1;

                    // Read, decrypt and decode each message
                    foreach (var (offset, length) in messageTable)
                    {
                        if (offset < 0 || length < 0 || offset + length * 2 > stream.Length)
                        {
                            AppLogger.Error($"Invalid message offset/length for message {msgIndex}: offset={offset}, length={length}");
                            msgIndex++;
                            break;
                        }

                        byte[] encryptedBytes = reader.ReadBytes(length * 2);
                        UInt16[] encryptedData = new UInt16[length];
                        for (int j = 0; j < length; j++)
                        {
                            encryptedData[j] = BitConverter.ToUInt16(encryptedBytes, j * 2);
                        }
                        string message = DecryptMessage(encryptedData, msgIndex);
                        messages.Add(message);
                        msgIndex++;
                    }

                    // Get remaining bytes in the stream
                    long remainingBytes = reader.BaseStream.Length - reader.BaseStream.Position;
                    if (remainingBytes > 0)
                    {
                        AppLogger.Warn($"There are {remainingBytes} unread bytes remaining in the message stream. This indicates a possible issue with the message offsets/lengths.");
                        throw new Exception("Unread bytes remaining in message stream.");
                    }

                }
                catch (EndOfStreamException)
                {
                    MessageBox.Show("Unexpected end of file while reading messages.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return messages;

        }

        public static bool WriteMessagesToStream(ref Stream stream, List<string> messages, UInt16 key)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                try
                {
                    UInt16 messageCount = (UInt16)messages.Count;
                    writer.Write(messageCount);
                    writer.Write(key);
                    long tableStartPos = writer.BaseStream.Position;
                    writer.Seek(messageCount * 8, SeekOrigin.Current); // Reserve space for the message table
                    var messageTable = new List<(int offset, int length)>(messageCount);
                    for (int i = 0; i < messageCount; i++)
                    {
                        UInt16[] encryptedMessage = EncryptMessage(messages[i], (i+1));
                        int offset = (int)writer.BaseStream.Position;
                        int length = encryptedMessage.Length;
                        // Write encrypted message
                        foreach (var code in encryptedMessage)
                        {
                            writer.Write(code);
                        }
                        // Encrypt length and offset for the table
                        int localKey = (765 * (i+1) * key) & 0xFFFF;
                        localKey |= (localKey << 16);
                        int encOffset = offset ^ localKey;
                        int encLength = length ^ localKey;
                        messageTable.Add((encOffset, encLength));
                    }
                    long endPos = writer.BaseStream.Position;
                    // Write the message table
                    writer.Seek((int)tableStartPos, SeekOrigin.Begin);
                    foreach (var (offset, length) in messageTable)
                    {
                        writer.Write(offset);
                        writer.Write(length);
                    }

                    writer.Seek((int)endPos, SeekOrigin.Begin); // Move back to the end
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error writing messages: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private static string DecryptMessage(UInt16[] message, int index)
        {
            ushort localKey = (ushort)(index * 596947);

            for (int i = 0; i < message.Length; i++)
            {
                message[i] ^= localKey;
                localKey = (ushort)((localKey + 18749) & 0xFFFF);
            }

            return DecodeMessage(message);

        }

        private static UInt16[] EncryptMessage(string message, int index)
        {
            UInt16[] encodedMessage = EncodeMessage(message);
            ushort localKey = (ushort)(index * 596947);
            for (int i = 0; i < encodedMessage.Length; i++)
            {
                encodedMessage[i] ^= localKey;
                localKey = (ushort)((localKey + 18749) & 0xFFFF);
            }
            return encodedMessage;
        }

        private static string DecodeMessage(UInt16[] message)
        {
            StringBuilder decodedMessage = new StringBuilder();

            int i = 0;

            while (i < message.Length)
            {
                ushort code = message[i];
                // Regular characters and escape sequences
                if (GetDecodingMap().ContainsKey(code))
                {
                    decodedMessage.Append(GetDecodingMap()[code]);
                    i++;
                }
                // Commands
                else if (code == 0xFFFE)
                {
                    var (command, toSkip) = DecodeCommand(message, i);
                    decodedMessage.Append(command);
                    i += toSkip;
                    i++; // Initial 0xFFFE
                }
                // Trainer Name
                else if (code == 0xF100)
                {
                    var (trainerName, toSkip) = DecodeTrainerName(message, i);
                    decodedMessage.Append(trainerName);
                    i += toSkip;
                    i++; // Initial 0xF100
                }
                // String terminator
                else if (code == 0xFFFF)
                {
                    break;
                }
                // Hexadecimal representation for unknown codes
                else
                {
                    decodedMessage.Append(ToHex(code));
                    i++;
                }
            }

            return decodedMessage.ToString();
        }

        private static UInt16[] EncodeMessage(string message)
        {
            List<UInt16> encodedMessage = new List<UInt16>();

            int i = 0;

            while (i < message.Length)
            {
                // Regular characters
                if (GetEncodingMap().ContainsKey(message[i].ToString()))
                {
                    encodedMessage.Add(GetEncodingMap()[message[i].ToString()]);
                    i++;
                }
                // Escape sequences
                else if (message[i] == '\\')
                {
                    // Handle hex escape sequences like \x1234
                    if (i + 5 < message.Length && message[i + 1] == 'x')
                    {
                        string hexSeq = message.Substring(i + 2, 4);
                        if (ushort.TryParse(hexSeq, System.Globalization.NumberStyles.HexNumber, null, out ushort hexValue))
                        {
                            encodedMessage.Add(hexValue);
                            i += 6;
                            continue;
                        }
                    }
                    // Handle single character escape sequences
                    else if (i + 1 < message.Length)
                    {
                        string escapeSeq = message.Substring(i, 2);
                        if (GetEncodingMap().ContainsKey(escapeSeq))
                        {
                            encodedMessage.Add(GetEncodingMap()[escapeSeq]);
                            i += 2;
                            continue;
                        }
                    }
                    // No match add null char
                    else
                    {
                        encodedMessage.Add(0);
                        i++;
                    }
                }
                // Commands
                else if (message[i] == '{')
                {
                    int endIndex = message.IndexOf('}', i);
                    if (endIndex != -1)
                    {
                        string command = message.Substring(i, endIndex - i + 1);

                        // Trainer Name special case
                        if (command.StartsWith("{TRAINER_NAME:") && command.EndsWith("}"))
                        {
                            encodedMessage.AddRange(EncodeTrainerName(command));
                            i = endIndex + 1;
                            continue;
                        }

                        // Regular command
                        encodedMessage.AddRange(EncodeCommand(command));
                        i = endIndex + 1;
                        continue;
                    }
                    // No match add null char
                    encodedMessage.Add(0);
                    i++;
                }
                // Multi character sequences
                else if (message[i] == '[')
                {
                    int endIndex = message.IndexOf(']', i);
                    if (endIndex != -1)
                    {
                        string multiCharSeq = message.Substring(i, endIndex - i + 1);
                        if (GetEncodingMap().ContainsKey(multiCharSeq))
                        {
                            encodedMessage.Add(GetEncodingMap()[multiCharSeq]);
                            i = endIndex + 1;
                            continue;
                        }
                        // No match add null char
                        encodedMessage.Add(0);
                        i++;
                    }
                }
                // No match
                else
                {
                    encodedMessage.Add(0);
                    i++;
                }
            }

            // Add string terminator
            encodedMessage.Add(0xFFFF);

            return encodedMessage.ToArray();

        }

        private static (string command, int toSkip) DecodeCommand(UInt16[] message, int startIndex)
        {
            // We need at least two more codes (command ID and param count)
            if (startIndex + 1 >= message.Length)
            {
                return (ToHex(0), 0);
            }
            else if (startIndex + 2 >= message.Length)
            {
                return (ToHex(message[startIndex + 1]), 1);
            }

            // Number of UInt16 codes to skip not including the initial 0xFFFE
            // We always skip at least the command ID and param count
            int skip = 2;

            ushort commandID = message[startIndex + 1];
            ushort paramCount = message[startIndex + 2];

            // We need to make sure we have enough codes for the parameters
            if (startIndex + 2 + paramCount >= message.Length)
            {
                return (ToHex(commandID, paramCount), skip);
            }

            ushort[] parameters = new ushort[paramCount];
            for (int i = 0; i < paramCount; i++)
            {
                parameters[i] = message[startIndex + 3 + i];
                skip++;
            }

            // Special case for string buffer vars that have 1 byte command ids
            int specialByte = 0;
            
            if (!commandMap.ContainsKey(commandID) && commandMap.ContainsKey((ushort)(commandID & 0xFF00))) 
            {
                specialByte = (ushort)(commandID & 0x00FF);
                commandID = (ushort)(commandID & 0xFF00);
                           
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("{");
            if (commandMap.ContainsKey(commandID))
            {
                sb.Append($"{commandMap[commandID]}");
                sb.Append($", {specialByte}");

                for (int i = 0; i < parameters.Length; i++)
                {
                    sb.Append($", {parameters[i]}");
                }
            }
            else
            {
                // Unknown command, represent as raw number
                sb.Append($"0x{commandID:X4}");
                for (int i = 0; i < parameters.Length; i++)
                {
                    sb.Append($", {parameters[i]}");
                }
            }
            sb.Append("}");
            return (sb.ToString(), skip);
        }

        private static UInt16[] EncodeCommand(string command)
        {
            // Strip the braces
            if (command.StartsWith("{") && command.EndsWith("}"))
            {
                command = command.Substring(1, command.Length - 2);
            }
            else
            {
                AppLogger.Error($"Invalid text command format for command: {command} ");
                return new UInt16[] { 0 };
            }
            
            string[] parts = command.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // parts[0] = command name or hex ID
            // parts[1] = special byte (0 if none)
            // parts[2..] = parameters

            if (parts.Length < 2)
            {
                AppLogger.Error($"Empty text command: {command}. Replaced with null character.");
                return new UInt16[] { 0 };
            }

            string commandName = parts[0].Trim();
            string specialByteStr = parts[1].Trim();
            ushort parameterCount = (ushort)(parts.Length - 2);

            List<UInt16> encodedCommand = new List<UInt16>();

            // Command start
            encodedCommand.Add(0xFFFE);

            // Get ID from name or parse hex
            if (commandMap.ContainsValue(commandName)) 
            {
                encodedCommand.Add(commandMap.Reverse()[commandName]);
            }
            else if (ushort.TryParse(commandName, out ushort commandID))
            {
                encodedCommand.Add(commandID);
            }
            else
            {
                AppLogger.Error($"Unknown text command: {commandName}");
                return new UInt16[] { 0 };
            }

            ushort specialByte;
            if (!ushort.TryParse(specialByteStr, out specialByte))
            {
                AppLogger.Error($"Invalid special byte '{specialByteStr}' in command: {command}. Replaced with value '0'");
                specialByte = 0;
            }

            encodedCommand[1] |= (ushort)(specialByte & 0x00FF);
            encodedCommand.Add(parameterCount);

            for (int i = 2; i < parts.Length; i++)
            {
                string paramStr = parts[i].Trim();
                if (ushort.TryParse(paramStr, out ushort paramValue))
                {
                    encodedCommand.Add(paramValue);
                }
                else
                {
                    AppLogger.Error($"Invalid parameter '{paramStr}' in command: {command}. Replaced with value '0'");
                    encodedCommand.Add(0);
                }
            }

            return encodedCommand.ToArray();

        }

        private static (string trainerName, int toSkip) DecodeTrainerName(UInt16[] message, int startIndex)
        {
            StringBuilder decoded = new StringBuilder();
            int bit = 0;
            int arrayIndex = startIndex + 1;
            int codesConsumed = 1;

            decoded.Append("{TRAINER_NAME:");

            // This code is completely stole from pokeplatinum's msgenc
            ushort curChar;
            while (arrayIndex < message.Length)
            {
                curChar = (ushort)((message[arrayIndex] >> bit) & 0x1FF);
                bit += 9;

                if (bit >= 15)
                {
                    arrayIndex++;
                    codesConsumed++;
                    bit -= 15;
                    if (bit != 0 && arrayIndex < message.Length)
                    {
                        curChar |= (ushort)(((message[arrayIndex] << (9 - bit)) & 0x1FF));
                    }
                }

                if (curChar == 0x1FF)
                    break;

                if (GetDecodingMap().ContainsKey(curChar))
                {
                    decoded.Append(GetDecodingMap()[curChar]);
                }
                else
                {
                    decoded.Append(ToHex(curChar));
                }

            }

            decoded.Append("}");

            return (decoded.ToString(), codesConsumed);
        }

        private static UInt16[] EncodeTrainerName(string trainerName)
        {
            string nameContent = trainerName.Substring(14, trainerName.Length - 15); // Strip {TRAINER_NAME: and }
            List<ushort> encodedChars = new List<ushort>();
            List<ushort> packedCodes = new List<ushort>();

            // Get list of characters to encode
            foreach (char c in nameContent)
            {
                if (GetEncodingMap().ContainsKey(c.ToString()))
                {
                    var code = GetEncodingMap()[c.ToString()];

                    // Ensure code fits in 9 bits
                    if (code >> 9 != 0)
                    {
                        AppLogger.Error($"Character '{c}' in trainer name encodes to value larger than 9 bits. Replaced with null char.");
                        encodedChars.Add(0);
                    }
                    else
                    {
                        encodedChars.Add(code);
                    }
                }
                else
                {
                    AppLogger.Error($"Unknown character '{c}' in trainer name. Replaced with null char.");
                    encodedChars.Add(0);
                }
            }

            // Add trainer name special indicator
            packedCodes.Add(0xF100);

            int bitBuffer = 0;
            int bitOffset = 0;

            // Pack 9-bit codes into 15-bit ushort values. MSB is always 0 except for terminator
            foreach (var code in encodedChars)
            {
                bitBuffer |= (code << bitOffset);
                bitOffset += 9;

                // Check for rollover
                if (bitOffset >= 15)
                {
                    // We have enough bits to write a new ushort
                    packedCodes.Add((ushort)(bitBuffer & 0x7FFF));
                    bitBuffer >>= 15;
                    bitOffset -= 15;
                }
            }

            // Add terminator if required
            // Side Note: All implementations of this that I've seen skip the terminator if there are no more bits to write
            // This means that if the name length is a multiple of 5 characters, there is no terminator
            // The regular message terminator (0xFFFF) is still added after the trainer name which ends up catching this but I am unsure whether this is intentional
            if (bitOffset != 0)
            {
                // Shift the remaining 9‑bit terminator (0x1FF) into the buffer, then emit the last 15‑bit word
                bitBuffer |= (0x1FF << bitOffset);
                packedCodes.Add((ushort)(bitBuffer & 0x7FFF));
            }

            return packedCodes.ToArray();
        }

        public static string GetSimpleTrainerName(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "";

            if (message.StartsWith("{TRAINER_NAME:") && message.EndsWith("}"))
            {
                return message.Substring(14, message.Length - 15);
            }

            return message;
        }

        public static string GetProperTrainerName(string message, string simpleName)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            if (message.StartsWith("{TRAINER_NAME:") && message.EndsWith("}"))
            {
                return "{TRAINER_NAME:" + simpleName + "}";
            }

            return message;
        }

        private static string ToHex(params ushort[] codes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var code in codes)
            {
                sb.Append($"\\x{code:X4}");
            }
            return sb.ToString();
        }

    }
}

using DSPRE;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using System;
using System.Globalization;

public class ScriptParameter {
    public enum ParameterType {
        Integer,
        Variable,
        Flex,
        Overworld,
        OwMovementType,
        OwMovementDirection,
        ComparisonOperator,
        Function,
        Action,
        CMDNumber,
        Pokemon,
        Item,
        Move,
        Sound,
        Trainer
    }

    public static ParameterType ParseTypeString(string typeStr)
    {
        try
        {
            return (ParameterType)Enum.Parse(typeof(ParameterType), typeStr, true); // true = ignore case
        }
        catch (ArgumentException)
        {
            AppLogger.Warn($"Warning: Unknown parameter type '{typeStr}', defaulting to Integer");
            return ParameterType.Integer;
        }
    }

    public ParameterType Type { get; set; } = ParameterType.Integer;
    public byte[] RawData { get; set; }
    public string DisplayValue { get; set; }
    public string TargetLabel { get; set; }  // For RelativeJump type
    public int TargetOffset { get; set; }   // For RelativeJump type

    // Constructor for regular parameters
    public ScriptParameter(byte[] data, ParameterType type)
    {
        RawData = data;
        Type = type;
        DisplayValue = FormatParameter(data, type);
    }

    // Constructor for relative jumps
    public ScriptParameter(int targetOffset, string targetLabel) {
        Type = ParameterType.Function;
        TargetOffset = targetOffset;
        TargetLabel = targetLabel;
        // Store raw bytes only for display purposes
        RawData = BitConverter.GetBytes(targetOffset);
    }

    // Get display representation
    public string GetFormattedValue() {
        if (Type == ParameterType.Function && !string.IsNullOrEmpty(TargetLabel)) {
            return TargetLabel;
        }

        // Default formatting for other types
        if (RawData.Length == 0) return "";
        if (RawData.Length == 1) return RawData[0].ToString("X2");
        if (RawData.Length == 2) return BitConverter.ToUInt16(RawData, 0).ToString("X4");
        if (RawData.Length == 4) return BitConverter.ToUInt32(RawData, 0).ToString("X8");

        return BitConverter.ToString(RawData);
    }
    private string FormatParameter(byte[] data, ParameterType type)
    {
        uint value;
        switch (data.Length)
        {
            case 1:
                value = data[0];
                break;
            case 2:
                value = BitConverter.ToUInt16(data, 0);
                break;
            case 4:
                value = BitConverter.ToUInt32(data, 0);
                break;
            default:
                throw new ArgumentException($"Unexpected parameter data length: {data.Length}");
        }

        switch (type)
        {
            case ParameterType.Pokemon:
                if (ScriptDatabase.pokemonNames.TryGetValue((ushort)value, out string pokeName))
                    return $"{pokeName}";
                break;
            case ParameterType.Item:
                if (ScriptDatabase.itemNames.TryGetValue((ushort)value, out string itemName))
                    return $"{itemName}";
                break;
            case ParameterType.Move:
                if (ScriptDatabase.moveNames.TryGetValue((ushort)value, out string moveName))
                    return $"{moveName}";
                break;
            case ParameterType.Trainer:
                if (ScriptDatabase.trainerNames.TryGetValue((ushort)value, out string trainerName))
                    return $"{trainerName}";
                break;
            case ParameterType.Function:
                return $"Function#{value}";
            //case ParameterType.Variable:
            //    return $"VAR_{FormatHexNumber(value)}";
            case ParameterType.Action:
                return $"Action#{value}";
            case ParameterType.CMDNumber:
                return $"CMD_{value:X3}";
            case ParameterType.Overworld:
                if (ScriptDatabase.specialOverworlds.TryGetValue((ushort)value, out string owName))
                    return owName;
                return value < 4000 ? $"{Event.EventType.Overworld}.{value}" : FormatHexNumber(value);
            case ParameterType.OwMovementType:
                return value < 4000 ? $"Move.{value}" : FormatHexNumber(value);
            case ParameterType.OwMovementDirection:
                if (ScriptDatabase.overworldDirections.TryGetValue((byte)value, out string dirName))
                    return dirName;
                break;
            case ParameterType.ComparisonOperator:
                if (RomInfo.ScriptComparisonOperatorsDict.TryGetValue((byte)value, out string compName))
                    return compName;
                break;
            case ParameterType.Sound:
                if (ScriptDatabase.soundNames.TryGetValue((ushort)value, out string soundName))
                    return soundName;
                break;
        }

        // Default number formatting based on settings
        return FormatHexNumber(value);
    }

    private string FormatHexNumber(uint value)
    {
        if (SettingsManager.Settings.scriptEditorFormatPreference == (int)NumberStyles.HexNumber)
        {
            return $"0x{value:X}";
        }
        else if (SettingsManager.Settings.scriptEditorFormatPreference == (int)NumberStyles.None)
        {
            return value >= 4000 ? $"0x{value:X}" : value.ToString();
        }
        return value.ToString();
    }
}
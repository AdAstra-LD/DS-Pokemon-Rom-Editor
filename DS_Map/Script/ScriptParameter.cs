using System;

public class ScriptParameter {
    public enum ParameterType {
        Integer,
        RelativeJump,
        Byte,
        Variable
    }

    public ParameterType Type { get; set; } = ParameterType.Integer;
    public byte[] RawData { get; set; }
    public string TargetLabel { get; set; }  // For RelativeJump type
    public int TargetOffset { get; set; }   // For RelativeJump type

    // Constructor for regular parameters
    public ScriptParameter(byte[] data) {
        Type = ParameterType.Integer;
        RawData = data;
    }

    // Constructor for relative jumps
    public ScriptParameter(int targetOffset, string targetLabel) {
        Type = ParameterType.RelativeJump;
        TargetOffset = targetOffset;
        TargetLabel = targetLabel;
        // Store raw bytes only for display purposes
        RawData = BitConverter.GetBytes(targetOffset);
    }

    // Get display representation
    public string GetFormattedValue() {
        if (Type == ParameterType.RelativeJump && !string.IsNullOrEmpty(TargetLabel)) {
            return TargetLabel;
        }

        // Default formatting for other types
        if (RawData.Length == 0) return "";
        if (RawData.Length == 1) return RawData[0].ToString("X2");
        if (RawData.Length == 2) return BitConverter.ToUInt16(RawData, 0).ToString("X4");
        if (RawData.Length == 4) return BitConverter.ToUInt32(RawData, 0).ToString("X8");

        return BitConverter.ToString(RawData);
    }
}
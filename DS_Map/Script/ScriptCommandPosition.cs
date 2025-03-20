// ScriptCommandPosition.cs
using System;

namespace DSPRE.ROMFiles {
    public class ScriptCommandPosition {
        public ScriptCommand Command { get; set; }
        public int Offset { get; set; }
        public string Label { get; set; } // null if no label needed
        public bool IsEntryPoint { get; set; }
        public int EntryPointIndex { get; set; } // -1 if not an entry point

        public ScriptCommandPosition(ScriptCommand cmd, int offset, string label = null, bool isEntryPoint = false, int entryPointIndex = -1) {
            Command = cmd;
            Offset = offset;
            Label = label;
            IsEntryPoint = isEntryPoint;
            EntryPointIndex = entryPointIndex;
        }
    }
}
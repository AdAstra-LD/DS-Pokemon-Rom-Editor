// Create this as a new file: ScriptLabeledSection.cs
using System;
using System.Collections.Generic;

namespace DSPRE.ROMFiles {
    public class ScriptLabeledSection {
        public string LabelName { get; set; }
        public List<ScriptCommand> Commands { get; set; }
        public uint OffsetInFile { get; set; }
        public bool IsReferenced { get; set; } = false;

        public ScriptLabeledSection(string labelName, List<ScriptCommand> commands = null) {
            LabelName = labelName;
            Commands = commands ?? new List<ScriptCommand>();
        }

        public override string ToString() {
            return LabelName;
        }
    }
}
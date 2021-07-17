using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    public class ActionContainer {
        public List<ScriptAction> actionCommandsList;
        public uint manualUserID;

        #region Constructors (2)
        public ActionContainer(uint actionNumber, List<ScriptAction> actionCommandsList = null) {
            manualUserID = actionNumber;
            this.actionCommandsList = actionCommandsList;
        }
        #endregion
    }
    public class ScriptAction {

        #region Fields (4)
        public ushort? id;
        public ushort? repetitionCount;
        public string name;
        #endregion

        public ScriptAction(ushort id, ushort? repetitionCount = null) {
            this.id = id;
            this.repetitionCount = repetitionCount;

            if (!RomInfo.ScriptActionNamesDict.TryGetValue(id, out name)) {
                name = id.ToString("X4");
            }

            if (repetitionCount != null && id != 0x00FE) {
                name += " " + "0x" + ((ushort)repetitionCount).ToString("X");
            }
        }
        public ScriptAction(string wholeLine, int lineNumber) {
            name = wholeLine;

            string[] nameParts = wholeLine.Replace("\t", "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            try {
                id = RomInfo.ScriptActionNamesDict.First(x => x.Value.Equals(nameParts[0], StringComparison.InvariantCultureIgnoreCase)).Key;
            } catch (InvalidOperationException) {
                ushort buf;
                if (ushort.TryParse(nameParts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out buf)) {
                    id = buf;
                } else {
                    string details;
                    if (wholeLine.Contains(':') && wholeLine.ContainsNumber()) {
                        details = "This probably means you forgot to \"End\" the Action above it.";
                    } else {
                        details = "Are you sure it's a proper Action Command?";
                    }
                    MessageBox.Show("This Script file could not be saved." +
                        Environment.NewLine + "Parser failed to interpret line " + lineNumber + ": \"" + wholeLine + "\"." +
                        Environment.NewLine + "\n" + details, "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    id = null;
                    return;
                }
            }

            if (id == 0x00FE && nameParts.Length != 1 || id != 0x00FE && nameParts.Length != 2) { //E.g.: End 0x2 0x40    OR     LookUp
                MessageBox.Show("Wrong number of parameters for action " + nameParts[0] + " at line " + lineNumber + "." + Environment.NewLine +
                    "Received: " + (nameParts.Length - 1) + Environment.NewLine + "Expected: 1"
                    + Environment.NewLine + "\nThis Script File can not be saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                id = null;
            } else {
                if (id == 0x00FE) {
                    repetitionCount = 0;
                } else {
                    NumberStyles style;
                    if (nameParts[1].StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)) {
                        style = NumberStyles.HexNumber;
                        nameParts[1] = nameParts[1].Substring(2);
                    } else {
                        style = NumberStyles.Integer;
                    }
                    repetitionCount = ushort.Parse(nameParts[1], style);
                }
            }
        }
    }
}

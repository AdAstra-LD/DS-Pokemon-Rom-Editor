using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
    public class ActionContainer {
        public List<ScriptAction> actions;
        public int manualUserID;

        #region Constructors (2)
        public ActionContainer(int actionNumber, List<ScriptAction> actionList = null) {
            manualUserID = actionNumber;
            actions = actionList;
        }
        #endregion
    }
    public class ScriptAction {

        #region Fields (4)
        public ushort id;
        public ushort repetitionCount = ushort.MaxValue;
        public string name;
        #endregion

        public ScriptAction(ushort id, ushort repetitionCount = ushort.MaxValue) {
            this.id = id;
            this.repetitionCount = repetitionCount;

            try {
                name = PokeDatabase.ScriptEditor.movementsDictIDName[id];
            } catch (KeyNotFoundException) {
                name = id.ToString("X4");
            }

            if (repetitionCount != ushort.MaxValue)
                name += " " + "0x" + repetitionCount.ToString("X");
        }
        public ScriptAction(string wholeLine, int lineNumber) {
            name = wholeLine;

            string[] nameParts = wholeLine.Split(' '); // Separate command code from parameters
            /* Get command id, which is always first in the description */

            try {
                id = PokeDatabase.ScriptEditor.movementsDictIDName.First(x => x.Value == nameParts[0]).Key;
            } catch (InvalidOperationException) {
                try {
                    id = UInt16.Parse(nameParts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                } catch (FormatException) {
                    MessageBox.Show("This Script file could not be saved." +
                        Environment.NewLine + "Parser failed to interpret line " + lineNumber + ": \"" + wholeLine + "\"." +
                        Environment.NewLine + "\nAre you sure it's a proper Action Command?", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    id = UInt16.MaxValue;
                    return;
                }
            }

            /* Read parameters from remainder of the description */
            if (id == 0x00FE) { 
                return;
            }

            if (nameParts.Length > 2) {
                MessageBox.Show("Wrong number of parameters for action " + nameParts[0] + " at line " + lineNumber + "." + Environment.NewLine +
                    "Received: " + (nameParts.Length - 1) + Environment.NewLine + "Action Commands need at most one parameter. "
                    + Environment.NewLine + "\nThis Script File can not be saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                id = ushort.MaxValue; //ERROR VALUE
            } else {
                NumberStyles style;
                if (nameParts[1].StartsWith("0x")) { 
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

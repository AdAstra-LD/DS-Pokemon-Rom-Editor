using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
  public class ActionContainer {
    public List < ScriptAction > actionCommandsList;
    public uint manualUserID;

    #region Constructors(2)
    public ActionContainer(uint actionNumber, List < ScriptAction > actionCommandsList = null) {
      manualUserID = actionNumber;
      this.actionCommandsList = actionCommandsList;
    }
    #endregion
  }
  public class ScriptAction {

    #region Fields(4)
    public ushort ? id;
    public ushort ? repetitionCount;
    public string name;
    #endregion

    public ScriptAction(ushort id, ushort ? repetitionCount = null) {
      this.id = id;
      this.repetitionCount = repetitionCount;

      if (!RomInfo.ScriptActionNamesDict.TryGetValue(id, out name)) {
        name = id.ToString("X4");
      }

      if (repetitionCount != null && id != 0x00FE) {
        name += " " + "0x" + ((ushort) repetitionCount).ToString("X");
      }
    }
    public ScriptAction(string wholeLine, ScriptFile scriptFile, int lineNumber) {
      // handle line comments
      // handle block comments
      name = wholeLine;
      int blockCommentPos1 = -1;
      int blockCommentPos2 = -1;
      int saveTextBeforePos = 0; // block comments will remove text after this point

      if (scriptFile != null && wholeLine.IndexOf("\"\"\"", StringComparison.InvariantCultureIgnoreCase) >= 0) { // if there is a """ on this line
        while ((blockCommentPos1 = wholeLine.IndexOf("\"\"\"", StringComparison.InvariantCultureIgnoreCase)) >= 0) { // while a block comment is on the current line
          if (scriptFile.blockComment) { // if currently in a block comment, remove everything before the upcoming """ after saveTextBeforePos.  end the block comment
            wholeLine = wholeLine.Remove(saveTextBeforePos, blockCommentPos1 + 3 - saveTextBeforePos);
            scriptFile.blockComment = false; // toggle block comment
          } else { // initiate a new block comment
            blockCommentPos2 = wholeLine.Remove(blockCommentPos1, 3).IndexOf("\"\"\"", StringComparison.InvariantCultureIgnoreCase);

            if (blockCommentPos2 >= 0) { // there is another """ in wholeLine, let blockComment == true above handle it but delete comment from string itself so it's not sensed a second time
              wholeLine = wholeLine.Remove(blockCommentPos1, 3);
              saveTextBeforePos = blockCommentPos1;
            } else { // there is no other """ in wholeLine, delete rest of line
              wholeLine = wholeLine.Substring(0, blockCommentPos1);
            }
            scriptFile.blockComment = true; // toggle block comment
          }
        }
      } else if (scriptFile.blockComment) { // if the whole line is contained in a blockComment with no toggle
        id = 0xFFFF;
        return;
      }

      // handle line comments
      int commentPos = wholeLine.IndexOf("##", StringComparison.InvariantCultureIgnoreCase);

      if (commentPos > 0) {
        wholeLine = wholeLine.Substring(0, commentPos);
      } else if (commentPos == 0) {
        id = 0xFFFF;
        return;
      }

      string[] nameParts = wholeLine.Replace("\t", "").Split(new char[] {
        ' '
      }, StringSplitOptions.RemoveEmptyEntries); // Separate command code from parameters
      /* Get command id, which is always first in the description */

      // should be sufficient to determine whether or not the line is empty after removing comments and whitespace
      // 2 is arbitrarily short to still allow for small nothings to pass through
      if (wholeLine.Replace(" ", "").Length < 2) {
        id = 0xFFFF;
        return;
      }

      if (RomInfo.ScriptActionNamesReverseDict.TryGetValue(nameParts[0].ToLower(), out ushort cmdID)) {
        this.id = cmdID;
      } else {
        if (ushort.TryParse(nameParts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort buf)) {
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
          "Received: " + (nameParts.Length - 1) + Environment.NewLine + "Expected: 1" +
          Environment.NewLine + "\nThis Script File can not be saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

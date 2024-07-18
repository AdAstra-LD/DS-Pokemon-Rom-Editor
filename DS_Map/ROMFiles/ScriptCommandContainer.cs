using System.Collections.Generic;

namespace DSPRE.ROMFiles {
    public class ScriptCommandContainer {
        public List<ScriptCommand> commands;
        public uint manualUserID;
        internal static readonly string functionStart;

        public ScriptCommandContainer(uint scriptNumber, List<ScriptCommand> commandList = null) {
            manualUserID = scriptNumber;
            commands = commandList;
        }

        public ScriptCommandContainer(uint newID, ScriptCommandContainer toCopy) {
            manualUserID = newID;
            commands = new List<ScriptCommand>(toCopy.commands); //command parameters need to be copied recursively
        }
    }
}

using System.Collections.Generic;

namespace DSPRE.ROMFiles {
    public class ScriptCommandContainer {
        public List<ScriptCommand> commands;
        public uint manualUserID;
        public int usedScriptID; //useScript ID referenced by this Script/Function
        public ScriptFile.ContainerTypes containerType;
        internal static readonly string functionStart;

        public ScriptCommandContainer(uint scriptNumber, ScriptFile.ContainerTypes containerType, int usedScriptID = -1, List<ScriptCommand> commandList = null) {
            manualUserID = scriptNumber;
            this.usedScriptID = usedScriptID;
            this.containerType = containerType;
            commands = commandList;
        }

        public ScriptCommandContainer(uint newID, ScriptCommandContainer toCopy) {
            manualUserID = newID;
            usedScriptID = toCopy.usedScriptID;
            containerType = toCopy.containerType;
            commands = new List<ScriptCommand>(toCopy.commands); //command parameters need to be copied recursively
        }
    }
}

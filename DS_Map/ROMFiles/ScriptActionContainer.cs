using System.Collections.Generic;

namespace DSPRE.ROMFiles
{
    public class ScriptActionContainer
    {
        public List<ScriptAction> commands;
        public uint manualUserID;

        public ScriptActionContainer(uint actionNumber, List<ScriptAction> commands = null)
        {
            manualUserID = actionNumber;
            this.commands = commands;
        }
    }
}
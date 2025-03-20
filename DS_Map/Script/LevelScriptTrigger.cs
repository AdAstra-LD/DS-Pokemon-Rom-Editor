using System;

namespace DSPRE.ROMFiles {
  public class LevelScriptTrigger {
    public const int VARIABLEVALUE = 1;
    public const int MAPCHANGE = 2;
    public const int SCREENRESET = 3;
    public const int LOADGAME = 4;

    private static int[] _triggerTypes;
    public int triggerType { get; set; }
    public int scriptTriggered { get; set; }

    public LevelScriptTrigger(int triggerType, int scriptTriggered) {
      this.triggerType = triggerType;
      this.scriptTriggered = scriptTriggered;
    }

    public static bool IsValidTriggerType(byte triggerType) {
      if (_triggerTypes == null) {
        _triggerTypes = new[] { VARIABLEVALUE, MAPCHANGE, SCREENRESET, LOADGAME };
      }

      return Array.IndexOf(_triggerTypes, triggerType) != -1;
    }

    public override string ToString() {
      return "Starts Script " + scriptTriggered;
    }
  }
}

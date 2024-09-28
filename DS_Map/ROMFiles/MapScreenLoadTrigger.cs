namespace DSPRE.ROMFiles {
  public class MapScreenLoadTrigger : LevelScriptTrigger {
    public MapScreenLoadTrigger(int type, int scriptTriggered) : base(type, scriptTriggered) { }

    public override string ToString() {
      switch (triggerType) {
        case LevelScriptTrigger.MAPCHANGE:
          return base.ToString() + " upon entering the LS map.";
        case LevelScriptTrigger.SCREENRESET:
          return base.ToString() + " when a fadescreen happens in the LS map.";
        case LevelScriptTrigger.LOADGAME:
          return base.ToString() + " when the game resumes in the LS map.";
        default:
          return base.ToString() + " under unknown circumstances.";
      }
    }

    public override bool Equals(object obj) {
      // If the passed object is null
      if (obj == null) {
        return false;
      }

      if (obj is MapScreenLoadTrigger) {
        return this.ToString() == ((MapScreenLoadTrigger)obj).ToString();
      }

      return false;
    }

    public override int GetHashCode() {
      return this.triggerType.GetHashCode() ^ scriptTriggered.GetHashCode();
    }
  }
}

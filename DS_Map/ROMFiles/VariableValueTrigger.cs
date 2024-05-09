namespace DSPRE.ROMFiles {
  public class VariableValueTrigger : LevelScriptTrigger {
    public int variableToWatch { get; set; }
    public int expectedValue { get; set; }

    public VariableValueTrigger(int scriptIDtoTrigger, int variableToWatch, int expectedValue) : base(VARIABLEVALUE, scriptIDtoTrigger) {
      this.variableToWatch = variableToWatch;
      this.expectedValue = expectedValue;
    }

    public override string ToString() {
      return base.ToString() + " when Var " + variableToWatch + " == " + expectedValue;
    }

    public override bool Equals(object obj) {
      // If the passed object is null
      if (obj == null) {
        return false;
      }

      if (!(obj is VariableValueTrigger)) {
        return false;
      }

      return this.ToString() == ((VariableValueTrigger)obj).ToString();
    }

    public override int GetHashCode() {
      return this.triggerType.GetHashCode() ^ variableToWatch.GetHashCode() ^ expectedValue.GetHashCode();
    }
  }
}

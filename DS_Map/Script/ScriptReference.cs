namespace DSPRE.ROMFiles {
  internal class ScriptReference {
    public ScriptFile.ContainerTypes typeOfCaller { get; private set; }
    public uint callerID { get; private set; }
    public ScriptFile.ContainerTypes typeOfInvoked { get; private set; }
    public uint invokedID { get; private set; }
    public int invokedAt { get; private set; }

    public ScriptReference(ScriptFile.ContainerTypes typeOfCaller, uint callerID, ScriptFile.ContainerTypes invokedType, uint invokedID, int invokedAt) {
      this.typeOfCaller = typeOfCaller;
      this.callerID = callerID;
      this.typeOfInvoked = invokedType;
      this.invokedID = invokedID;

      this.invokedAt = invokedAt;
    }

    public override string ToString() {
      return typeOfCaller + " " + callerID + " invokes " + typeOfInvoked + " " + invokedID + " at " + invokedAt;
    }
  }
}

namespace DSPRE.ROMFiles {
  //This class is in case a MapHeader uses the same MapFile more than once
  //ToString is the matrix x,y and mapID
  class HeadbuttEncounterMap {
    public readonly int mapID;
    public readonly int x;
    public readonly int y;

    public HeadbuttEncounterMap(int mapID, int x, int y) {
      this.mapID = mapID;
      this.x = x;
      this.y = y;
    }

    public override string ToString() {
      return $"{mapID} - {x},{y}";
    }

    public override bool Equals(object obj) {
      // If the passed object is null
      if (obj == null) {
        return false;
      }

      if (obj is HeadbuttEncounterMap) {
        return this.ToString() == ((HeadbuttEncounterMap)obj).ToString();
      }

      return false;
    }

    public override int GetHashCode() {
      return this.x.GetHashCode() ^ y.GetHashCode() ^ mapID.GetHashCode();
    }
  }
}

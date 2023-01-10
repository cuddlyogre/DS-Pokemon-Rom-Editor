using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE.ROMFiles
{
  class HeadbuttEncounterMap {
    public readonly int x;
    public readonly int y;
    public readonly int mapID;

    public HeadbuttEncounterMap(int x, int y, int mapID) {
      this.x = x;
      this.y = y;
      this.mapID = mapID;
    }

    public override string ToString() {
      return $"{x},{y} - {mapID}";
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

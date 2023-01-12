using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE.ROMFiles
{
  class SafariZoneObject
  {
    public static Dictionary<int, string> ObjectTypes = new Dictionary<int, string>() { {0, "No Requirement"}, { 1, "Plains" }, { 2, "Forest" }, { 3, "Peak" }, { 4, "Waterside" } };

    public byte typeID;
    public byte quantity;

    public SafariZoneObject(BinaryReader br) {
      this.typeID = br.ReadByte();
      this.quantity = br.ReadByte();
    }

    public override string ToString() {
      return $"{typeID} {ObjectTypes[typeID]}: {quantity}";
    }
  }
}

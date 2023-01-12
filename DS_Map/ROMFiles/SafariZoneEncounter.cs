using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE.ROMFiles
{
  class SafariZoneEncounter
  {
    public ushort pokemonID;
    public byte level;
    public SafariZoneEncounter() {
      level = 0;
      pokemonID = 0;
    }

    public SafariZoneEncounter(BinaryReader br) {
      this.pokemonID = br.ReadUInt16();
      this.level = br.ReadByte();
    }

    public override string ToString() {
      string[] pokemonNames = RomInfo.GetPokemonNames();
      string pokemon = pokemonNames[pokemonID];
      return $"{pokemonID,4} {pokemon,10}: {level,3}";
    }
  }
}

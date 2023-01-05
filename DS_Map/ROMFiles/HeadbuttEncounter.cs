using System.IO;

namespace DSPRE.ROMFiles {
  public class HeadbuttEncounter {
    public ushort pokemonID;
    public byte minLevel;
    public byte maxLevel;

    public HeadbuttEncounter(BinaryReader br) {
      this.pokemonID = br.ReadUInt16();
      this.minLevel = br.ReadByte();
      this.maxLevel = br.ReadByte();
    }

    public override string ToString() {
      string[] pokemonNames = RomInfo.GetPokemonNames();
      string pokemon = pokemonNames[pokemonID];
      return $"{pokemonID,3} {pokemon,10}: {minLevel} - {maxLevel}";
    }
  }
}

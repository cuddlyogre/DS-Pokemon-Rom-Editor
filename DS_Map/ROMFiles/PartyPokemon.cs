using System.IO;

namespace DSPRE.ROMFiles {
  public class PartyPokemon : RomFile {
    public ushort? pokeID = null;
    public ushort level = 0;
    public ushort unknown1_DATASTART = 0;
    public ushort unknown2_DATAEND = 0;

    public ushort? heldItem = null;
    public ushort[] moves = null;

    public PartyPokemon(bool hasItems = false, bool hasMoves = false) {
      UpdateItemsAndMoves(hasItems, hasMoves);
    }

    public PartyPokemon(ushort Unk1, ushort Level, ushort Pokemon, ushort Unk2, ushort? heldItem = null, ushort[] moves = null) {
      pokeID = Pokemon;
      level = Level;
      unknown1_DATASTART = Unk1;
      unknown2_DATAEND = Unk2;
      this.heldItem = heldItem;
      this.moves = moves;
    }

    public override byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(newData)) {
        writer.Write(unknown1_DATASTART);
        writer.Write(level);
        writer.Write(pokeID ?? 0);

        if (heldItem != null) {
          writer.Write((ushort)heldItem);
        }

        if (moves != null) {
          foreach (ushort move in moves) {
            writer.Write(move);
          }
        }

        writer.Write(unknown2_DATAEND);
      }

      return newData.ToArray();
    }

    public void UpdateItemsAndMoves(bool hasItems = false, bool hasMoves = false) {
      if (hasItems) {
        this.heldItem = 0;
      }

      if (hasMoves) {
        this.moves = new ushort[4];
      }
    }

    public override string ToString() {
      return CheckEmpty() ? "Empty" : this.pokeID + " Lv. " + this.level;
    }

    public bool CheckEmpty() {
      return this is null || pokeID is null || level <= 0;
    }
  }
}

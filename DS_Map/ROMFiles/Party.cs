using System;
using System.IO;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
  public class Party : RomFile {
    private PartyPokemon[] content;
    private TrainerProperties trp;
    public bool exportCondensedData;

    public const int MOVES_PER_POKE = 4;
    public Party(int POKE_IN_PARTY, bool init, TrainerProperties trp) {
      this.trp = trp;
      this.content = new PartyPokemon[POKE_IN_PARTY];

      if (init) {
        for (int i = 0; i < content.Length; i++) {
          this.content[i] = new PartyPokemon();
        }
      }
    }

    public Party(bool readFirstByte, int maxPoke, Stream partyData, TrainerProperties traipr) {
      using (BinaryReader reader = new BinaryReader(partyData)) {
        try {
          this.trp = traipr;
          if (readFirstByte) {
            byte flags = reader.ReadByte();

            trp.hasMoves = (flags & 1) != 0;
            trp.hasItems = (flags & 2) != 0;
            trp.partyCount = (byte)((flags & 28) >> 2);
          }

          int dividend = 8;

          if (trp.hasMoves) {
            dividend += Party.MOVES_PER_POKE * sizeof(ushort);
          }
          if (trp.hasItems) {
            dividend += sizeof(ushort);
          }

          int endval = Math.Min((int)(partyData.Length - 1 / dividend), trp.partyCount);
          this.content = new PartyPokemon[maxPoke];
          for (int i = 0; i < endval; i++) {
            ushort unknown1 = reader.ReadUInt16();
            ushort level = reader.ReadUInt16();

            //NOTE: The following bitwise 'AND' operation fixes TUBER JARED [PLAT] and all trainers who
            //use pokemon with a special form --> ALL PARTY POKEMON WITH A SPECIAL FORM WILL LOSE IT
                        
            //the way special forms are stored is

            //U16 POKEMON
            // 0 1 2 3   4 5 6 7    8 9 A B   C D E F
            //BITS 6 - F --> pokemon ID
            //BITS 0 - 5 --> form ID 
            ushort pokemon = (ushort)(reader.ReadUInt16() & (ushort.MaxValue>>6)); 
                        
            ushort? heldItem = null;
            ushort[] moves = null;

            if (trp.hasItems) {
              heldItem = reader.ReadUInt16();
            }
            if (trp.hasMoves) {
              moves = new ushort[MOVES_PER_POKE];
              for (int m = 0; m < moves.Length; m++) {
                ushort val = reader.ReadUInt16();
                moves[m] = (ushort)(val == ushort.MaxValue ? 0 : val);
              }
            }

            content[i] = new PartyPokemon(unknown1, level, pokemon, reader.ReadUInt16(), heldItem, moves);
          }
          for (int i = endval; i < maxPoke; i++) {
            content[i] = new PartyPokemon();
          }
        } catch (EndOfStreamException) {
          MessageBox.Show("There was a problem reading the party data of this " + this.GetType().Name + ".", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    public PartyPokemon this[int index] {
      get {
        return content[index];
      }
      set {
        content[index] = value;
      }
    }
    public override string ToString() {
      if (this.content == null) {
        return "Empty";
      } else {
        string buffer = "";
        byte nonEmptyCtr = 0;
        foreach(PartyPokemon p in this.content) {
          if (!p.CheckEmpty()) {
            nonEmptyCtr++;
          }
        }
        buffer += nonEmptyCtr + " Poke ";
        if (this.trp.hasMoves) {
          buffer += ", moves ";
        }
        if (this.trp.hasItems) {
          buffer += ", items ";
        }
        return buffer;
      }
    }
    public override byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(newData)) {
        if (this.exportCondensedData && trp != null) {
          byte condensedTrData = (byte)(((trp.hasMoves ? 1 : 0) & 0b_1) + (((trp.hasItems ? 1 : 0) & 0b_1) << 1) + ((trp.partyCount & 0b_1111_11) << 2));
          writer.Write(condensedTrData);
        }

        foreach (PartyPokemon poke in this.content) {
          if (!poke.CheckEmpty()) {
            writer.Write(poke.ToByteArray());
          }
        }
      }
      return newData.ToArray();
    }
    public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
      SaveToFileExplorePath("Gen IV Party Data", "pdat", suggestedFileName, showSuccessMessage);
    }
  }
}

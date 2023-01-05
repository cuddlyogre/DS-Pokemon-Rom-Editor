using System;
using System.Collections;
using System.IO;

namespace DSPRE.ROMFiles {
  public class TrainerProperties : RomFile {
    public const int AI_COUNT = 11;
    public const int TRAINER_ITEMS = 4;

    #region Fields
    public ushort trainerID;
    public byte trDataUnknown;

    public byte trainerClass = 0;
    public byte partyCount = 0;

    public bool doubleBattle = false;
    public bool hasMoves = false;
    public bool hasItems = false;

    public ushort[] trainerItems = new ushort[TRAINER_ITEMS];
    public BitArray AI;
    #endregion

    #region Constructor
    public TrainerProperties(ushort ID, byte partyCount = 0) {
      trainerID = ID;
      trainerItems = new ushort[TRAINER_ITEMS];
      AI = new BitArray( new bool[AI_COUNT] { true, false, false, false, false, false, false, false, false, false, false } );
      trDataUnknown = 0;
    }
    public TrainerProperties(ushort ID, Stream trainerPropertiesStream) {
      trainerID = ID;
      using (BinaryReader reader = new BinaryReader(trainerPropertiesStream)) {
        byte flags = reader.ReadByte();
        hasMoves = (flags & 1) != 0;
        hasItems = (flags & 2) != 0;

        trainerClass = reader.ReadByte();
        trDataUnknown = reader.ReadByte();
        partyCount = reader.ReadByte();

        for (int i = 0; i < trainerItems.Length; i++) {
          trainerItems[i] = reader.ReadUInt16();
        }

        AI = new BitArray( BitConverter.GetBytes(reader.ReadUInt32()) );
        doubleBattle = reader.ReadUInt32() == 2;
      }
    }
    #endregion

    #region Methods
    public override byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(newData)) {
        byte flags = 0;
        flags |= (byte)(hasMoves ? 1 : 0);
        flags |= (byte)(hasItems ? 2 : 0);

        writer.Write(flags);
        writer.Write(trainerClass);
        writer.Write(trDataUnknown);
        writer.Write(partyCount);

        foreach (ushort trItem in trainerItems) {
          writer.Write(trItem);
        }

        uint AIflags = 0;
        for (int i = 0; i < AI.Length; i++) {
          if (AI[i]) {
            AIflags |= (uint)1 << i;
          }
        }

        writer.Write(AIflags);
        writer.Write((uint)(doubleBattle ? 2 : 0));
      }
      return newData.ToArray();
    }

    public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
      SaveToFileExplorePath("Gen IV Trainer Properties", "trp", suggestedFileName, showSuccessMessage);
    }
    #endregion

  }
}

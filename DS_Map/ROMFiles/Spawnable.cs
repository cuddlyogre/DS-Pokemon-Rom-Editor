using System.IO;
using DSPRE.Resources;

namespace DSPRE.ROMFiles {
  public class Spawnable : Event {
    public const int TYPE_MISC = 0;
    public const int TYPE_BOARD = 1;
    public const int TYPE_HIDDENITEM = 2;

    public ushort scriptNumber;
    public ushort type;
    public ushort unknown2;
    public ushort unknown3;
    public ushort unknown4;
    public ushort dir;
    public ushort unknown5;

    public Spawnable(Stream data) {
      evType = EventType.Spawnable;
      using (BinaryReader reader = new BinaryReader(data)) {
        scriptNumber = reader.ReadUInt16();
        type = reader.ReadUInt16();

        /* Decompose x coordinate in matrix and map positions */
        int xPosition = reader.ReadInt16();
        xMapPosition = (short)(xPosition % MapFile.mapSize);
        xMatrixPosition = (ushort)(xPosition / MapFile.mapSize);

        unknown2 = reader.ReadUInt16();

        /* Decompose y coordinate in matrix and map positions */
        int yPosition = reader.ReadInt16();
        yMapPosition = (short)(yPosition % MapFile.mapSize);
        yMatrixPosition = (ushort)(yPosition / MapFile.mapSize);

        unknown3 = reader.ReadUInt16();
        zPosition = reader.ReadInt16();
        unknown4 = reader.ReadUInt16();
        dir = reader.ReadUInt16();
        unknown5 = reader.ReadUInt16();
      }
    }

    public Spawnable(int xMatrixPosition, int yMatrixPosition) {
      evType = EventType.Spawnable;

      scriptNumber = 0;
      type = 0;
      unknown2 = 0;
      unknown3 = 0;
      unknown4 = 0;
      unknown5 = 0;
      dir = 0;

      xMapPosition = 0;
      yMapPosition = 0;
      zPosition = 0;
      this.xMatrixPosition = (ushort)xMatrixPosition;
      this.yMatrixPosition = (ushort)yMatrixPosition;
    }

    public Spawnable(Spawnable toCopy) {
      evType = EventType.Spawnable;

      scriptNumber = toCopy.scriptNumber;
      type = toCopy.type;
      unknown2 = toCopy.unknown2;
      unknown3 = toCopy.unknown3;
      unknown4 = toCopy.unknown4;
      unknown5 = toCopy.unknown5;
      dir = toCopy.dir;

      xMapPosition = toCopy.xMapPosition;
      yMapPosition = toCopy.yMapPosition;
      zPosition = toCopy.zPosition;
      this.xMatrixPosition = toCopy.xMatrixPosition;
      this.yMatrixPosition = toCopy.yMatrixPosition;
    }

    public override byte[] ToByteArray() {
      using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {
        writer.Write(scriptNumber);
        writer.Write(type);
        short xCoordinate = (short)(xMapPosition + MapFile.mapSize * xMatrixPosition);
        writer.Write(xCoordinate);
        writer.Write(unknown2);
        short yCoordinate = (short)(yMapPosition + MapFile.mapSize * yMatrixPosition);
        writer.Write(yCoordinate);
        writer.Write(unknown3);
        writer.Write(zPosition);
        writer.Write(unknown4);
        writer.Write(dir);
        writer.Write(unknown5);

        return ((MemoryStream)writer.BaseStream).ToArray();
      }
    }

    public override string ToString() {
      string msg = "";
      switch (this.type) {
        case TYPE_MISC:
          msg += $"Misc, {PokeDatabase.EventEditor.Spawnables.orientationsArray[dir].ToLower()}";
          break;

        case TYPE_BOARD:
          msg += $"Board, {PokeDatabase.EventEditor.Spawnables.orientationsArray[dir].ToLower()}";

          break;

        case TYPE_HIDDENITEM:
          msg += "Hidden Item";
          break;
      }

      return msg + $", [Scr {scriptNumber}]";
    }
  }
}

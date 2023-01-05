using System.IO;

namespace DSPRE.ROMFiles {
  public class Overworld : Event {
    public static string MovementCodeKW = "Move";

    public enum OwType : ushort {
      NORMAL = 0,
      TRAINER = 1,
      ITEM = 3
    };

    public ushort owID;
    public ushort overlayTableEntry;
    public ushort movement;
    public ushort type;
    public ushort flag;
    public ushort scriptNumber;
    public ushort orientation;
    public ushort sightRange;
    public ushort unknown1;
    public ushort unknown2;
    public ushort xRange;
    public ushort yRange;
    public ushort unknown3;
    public bool is3D = new bool();

    public Overworld(Stream data) {
      evType = EventType.Overworld;
      using (BinaryReader reader = new BinaryReader(data)) {
        owID = reader.ReadUInt16();
        overlayTableEntry = reader.ReadUInt16();
        movement = reader.ReadUInt16();
        type = reader.ReadUInt16();
        flag = reader.ReadUInt16();
        scriptNumber = reader.ReadUInt16();
        orientation = reader.ReadUInt16();
        sightRange = reader.ReadUInt16();
        unknown1 = reader.ReadUInt16();
        unknown2 = reader.ReadUInt16();
        xRange = reader.ReadUInt16();
        yRange = reader.ReadUInt16();

        /* Decompose x-y coordinates in matrix and map positions */
        int xPosition = reader.ReadInt16();
        int yPosition = reader.ReadInt16();
        xMapPosition = (short)(xPosition % MapFile.mapSize);
        yMapPosition = (short)(yPosition % MapFile.mapSize);
        xMatrixPosition = (ushort)(xPosition / MapFile.mapSize);
        yMatrixPosition = (ushort)(yPosition / MapFile.mapSize);

        zPosition = reader.ReadInt16();
        unknown3 = reader.ReadUInt16();
      }
    }

    public Overworld(int owID, int xMatrixPosition, int yMatrixPosition) {
      evType = EventType.Overworld;

      this.owID = (ushort)owID;
      overlayTableEntry = 1;
      movement = 0;
      type = 0;
      flag = 0;
      scriptNumber = 0;
      orientation = 1;
      sightRange = 0;
      unknown1 = 0;
      unknown2 = 0;
      xRange = 0;
      yRange = 0;
      unknown3 = 0;

      xMapPosition = 16;
      yMapPosition = 16;
      zPosition = 0;
      this.xMatrixPosition = (ushort)xMatrixPosition;
      this.yMatrixPosition = (ushort)yMatrixPosition;
    }

    public Overworld(Overworld toCopy) {
      evType = EventType.Overworld;

      this.owID = toCopy.owID;
      overlayTableEntry = toCopy.overlayTableEntry;
      movement = toCopy.movement;
      type = toCopy.type;
      flag = toCopy.flag;
      scriptNumber = toCopy.scriptNumber;
      orientation = toCopy.orientation;
      sightRange = toCopy.sightRange;
      unknown1 = toCopy.unknown1;
      unknown2 = toCopy.unknown2;
      xRange = toCopy.xRange;
      yRange = toCopy.yRange;
      unknown3 = toCopy.unknown3;

      xMapPosition = toCopy.xMapPosition;
      yMapPosition = toCopy.yMapPosition;
      zPosition = toCopy.zPosition;
      this.xMatrixPosition = toCopy.xMatrixPosition;
      this.yMatrixPosition = toCopy.yMatrixPosition;
    }

    public override byte[] ToByteArray() {
      using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {
        writer.Write(owID);
        writer.Write(overlayTableEntry);
        writer.Write(movement);
        writer.Write(type);
        writer.Write(flag);
        writer.Write(scriptNumber);
        writer.Write(orientation);
        writer.Write(sightRange);
        writer.Write(unknown1);
        writer.Write(unknown2);
        writer.Write(xRange);
        writer.Write(yRange);

        short xCoordinate = (short)(xMapPosition + MapFile.mapSize * xMatrixPosition);
        writer.Write(xCoordinate);

        short yCoordinate = (short)(yMapPosition + MapFile.mapSize * yMatrixPosition);
        writer.Write(yCoordinate);

        writer.Write(zPosition);
        writer.Write(unknown3);

        return ((MemoryStream)writer.BaseStream).ToArray();
      }
    }

    public override string ToString() {
      string entityName = ", " + "Entry " + overlayTableEntry;
      return $"{(this.isAlias() ? "AliasOf" : "ID")} {this.owID} {entityName}";
    }

    private bool isAlias() {
      return scriptNumber == 0xFFFF;
    }
  }
}

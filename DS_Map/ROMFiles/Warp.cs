using System.IO;

namespace DSPRE.ROMFiles {
  public class Warp : Event {
    public ushort header;
    public ushort anchor;
    public uint height;

    public Warp(Stream data) {
      evType = EventType.Warp;
      using (BinaryReader reader = new BinaryReader(data)) {
        /* Decompose x-y coordinates in matrix and map positions */
        int xPosition = reader.ReadInt16();
        int yPosition = reader.ReadInt16();
        xMapPosition = (short)(xPosition % MapFile.mapSize);
        yMapPosition = (short)(yPosition % MapFile.mapSize);
        xMatrixPosition = (ushort)(xPosition / MapFile.mapSize);
        yMatrixPosition = (ushort)(yPosition / MapFile.mapSize);

        header = reader.ReadUInt16();
        anchor = reader.ReadUInt16();
        height = reader.ReadUInt32();
      }
    }
    public Warp(int xMatrixPosition, int yMatrixPosition) {
      evType = EventType.Warp;

      header = 0;
      anchor = 0;

      xMapPosition = 0;
      yMapPosition = 0;
      this.xMatrixPosition = (ushort)xMatrixPosition;
      this.yMatrixPosition = (ushort)yMatrixPosition;
    }
    public Warp(Warp toCopy) {
      evType = EventType.Warp;

      header = toCopy.header;
      anchor = toCopy.anchor;

      xMapPosition = toCopy.xMapPosition;
      yMapPosition = toCopy.yMapPosition;
      this.xMatrixPosition = toCopy.xMatrixPosition;
      this.yMatrixPosition = toCopy.yMatrixPosition;
    }

    public override byte[] ToByteArray() {
      using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {
        ushort xCoordinate = (ushort)(xMapPosition + MapFile.mapSize * xMatrixPosition);
        writer.Write(xCoordinate);

        ushort yCoordinate = (ushort)(yMapPosition + MapFile.mapSize * yMatrixPosition);
        writer.Write(yCoordinate);

        writer.Write(header);
        writer.Write(anchor);
        writer.Write(height);

        return ((MemoryStream)writer.BaseStream).ToArray();
      }
    }
    public override string ToString() {
      return "To Header " + header.ToString("D3") + ", " + "Hook " + anchor.ToString("D2");
    }
  }
}

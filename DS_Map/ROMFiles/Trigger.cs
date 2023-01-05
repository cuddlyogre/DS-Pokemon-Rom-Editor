using System.IO;

namespace DSPRE.ROMFiles {
  public class Trigger : Event {
    #region Fields (7)
    public ushort scriptNumber;
    public ushort widthX;
    public ushort heightY;
    new public ushort zPosition;
    public ushort expectedVarValue;
    public ushort variableWatched;
    #endregion Fields

    #region Constructors (2)
    public Trigger(Stream data) {
      evType = EventType.Trigger;
      using (BinaryReader reader = new BinaryReader(data)) {
        scriptNumber = reader.ReadUInt16();

        /* Decompose x-y coordinates in matrix and map positions */
        int xPosition = reader.ReadInt16();
        int yPosition = reader.ReadInt16();
        xMapPosition = (short)(xPosition % MapFile.mapSize);
        yMapPosition = (short)(yPosition % MapFile.mapSize);
        xMatrixPosition = (ushort)(xPosition / MapFile.mapSize);
        yMatrixPosition = (ushort)(yPosition / MapFile.mapSize);

        widthX = reader.ReadUInt16();
        heightY = reader.ReadUInt16();

        zPosition = reader.ReadUInt16();
        expectedVarValue = reader.ReadUInt16();
        variableWatched = reader.ReadUInt16();
      }
    }
    public Trigger(int xMatrixPosition, int yMatrixPosition) {
      evType = EventType.Trigger;

      scriptNumber = 0;
      variableWatched = 0;
      expectedVarValue = 0;
      widthX = 1;
      heightY = 1;

      xMapPosition = 0;
      yMapPosition = 0;
      this.xMatrixPosition = (ushort)xMatrixPosition;
      this.yMatrixPosition = (ushort)yMatrixPosition;
    }
    public Trigger(Trigger toCopy) {
      evType = EventType.Trigger;

      scriptNumber = toCopy.scriptNumber;
      variableWatched = toCopy.variableWatched;
      expectedVarValue = toCopy.expectedVarValue;
      widthX = toCopy.widthX;
      heightY = toCopy.heightY;

      xMapPosition = toCopy.xMapPosition;
      yMapPosition = toCopy.xMapPosition;
      this.xMatrixPosition = toCopy.xMatrixPosition;
      this.yMatrixPosition = toCopy.yMatrixPosition;
    }
    #endregion

    #region Methods (1)
    public override byte[] ToByteArray() {
      using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {
        writer.Write(scriptNumber);
        ushort xCoordinate = (ushort)(xMapPosition + MapFile.mapSize * xMatrixPosition);
        writer.Write(xCoordinate);
        ushort yCoordinate = (ushort)(yMapPosition + MapFile.mapSize * yMatrixPosition);
        writer.Write(yCoordinate);
        writer.Write(widthX);
        writer.Write(heightY);
        writer.Write(zPosition);
        writer.Write(expectedVarValue);
        writer.Write(variableWatched);

        return ((MemoryStream)writer.BaseStream).ToArray();
      }
    }
    public override string ToString() {
      string msg = "Run script " + scriptNumber;
      if (variableWatched != 0) {
        msg += $" when Var {variableWatched} is {expectedVarValue}";
      }
      return msg;
    }
    #endregion
  }
}

namespace DSPRE.ROMFiles {
  public abstract class Event {
    public enum EventType : byte {
      Spawnable,
      Overworld,
      Warp,
      Trigger
    }

    public EventType evType;

    public short xMapPosition;
    public short yMapPosition;
    public short zPosition;
    public ushort xMatrixPosition;
    public ushort yMatrixPosition;

    public abstract byte[] ToByteArray();
  }
}

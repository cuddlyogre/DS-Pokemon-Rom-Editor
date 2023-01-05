namespace DSPRE.ROMFiles {
  public abstract class Event {
    public enum EventType : byte {
      Spawnable,
      Overworld,
      Warp,
      Trigger
    }
    #region Fields (6)
    public EventType evType;

    public short xMapPosition;
    public short yMapPosition;
    public short zPosition;
    public ushort xMatrixPosition;
    public ushort yMatrixPosition;
    #endregion

    #region Methods (1)
    public abstract byte[] ToByteArray();
    #endregion
  }
}

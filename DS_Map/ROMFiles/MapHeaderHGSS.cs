using System.IO;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
  /// <summary>
  /// Class to store map header data from Pokémon HG and SS
  /// </summary>
  public class MapHeaderHGSS : MapHeader {
    #region Fields (7)
    public byte areaIcon { get; set; }
    public byte followMode { get; set; }
    public byte locationName { get; set; }
    public byte locationType { get; set; }  //4 bits only
    public byte unknown0 { get; set; } //4 bits only
    public byte unknown1 { get; set; } //4 bits only
    public byte worldmapX { get; set; } //6 bits only
    public byte worldmapY { get; set; } //6 bits only
    public bool kantoFlag { get; set; }
    #endregion

    #region Constructors (1)
    public MapHeaderHGSS(ushort headerNumber, Stream data) {
      this.ID = headerNumber;
      using (BinaryReader reader = new BinaryReader(data)) {
        try {
          wildPokemon = reader.ReadByte();
          areaDataID = reader.ReadByte();

          ushort coords = reader.ReadUInt16();
          unknown0 = (byte)(coords & 0b_1111); //get 4 bits
          worldmapX = (byte)((coords >> 4) & 0b_1111_11); //get 6 bits after the first 4
          worldmapY = (byte)((coords >> 10) & 0b_1111_11); //get 6 bits after the first 10

          matrixID = reader.ReadUInt16();
          scriptFileID = reader.ReadUInt16();
          levelScriptID = reader.ReadUInt16();
          textArchiveID = reader.ReadUInt16();
          musicDayID = reader.ReadUInt16();
          musicNightID = reader.ReadUInt16();
          eventFileID = reader.ReadUInt16();
          locationName = reader.ReadByte();
                    
          byte areaProperties = reader.ReadByte();
          areaIcon = (byte)(areaProperties & 0b_1111); //get 4 bits
          unknown1 = (byte)((areaProperties >> 4) & 0b_1111); //get 4 bits after the first 4

          uint last32 = reader.ReadUInt32();
          kantoFlag = (last32 & 0b_1) == 1; //get 1 bit
          weatherID = (byte)((last32 >> 1) & 0b_1111_111); //get 7 bits after the first one

          locationType = (byte)((last32 >> 8) & 0b_1111); //get 4 bits after the first 8
          cameraAngleID = (byte)((last32 >> 12) & 0b_1111_11); //get 6 bits after the first 12
          followMode = (byte)((last32 >> 18) & 0b_11); //get 2 bits after the first 17
          battleBackground = (byte)((last32 >> 20) & 0b_1111_1); //get 5 bits after the first 19
          flags = (byte)(last32 >> 25 & 0b_1111_111); //get 7 bits after the first 24

        } catch (EndOfStreamException) {
          MessageBox.Show("Error loading header " + ID + '.', "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
          ID = ushort.MaxValue;
        }
      }
    }
    #endregion Constructors

    #region Methods(1)
    public override byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(newData)) {
        writer.Write((byte)wildPokemon);
        writer.Write(areaDataID);

        ushort worldMapCoordinates = (ushort)((unknown0 & 0b_1111) + ((worldmapX & 0b_1111_11) << 4) + ((worldmapY & 0b_1111_11) << 10));
        writer.Write(worldMapCoordinates);

        writer.Write(matrixID);
        writer.Write(scriptFileID);
        writer.Write(levelScriptID);
        writer.Write(textArchiveID);
        writer.Write(musicDayID);
        writer.Write(musicNightID);
        writer.Write(eventFileID);
        writer.Write(locationName);

        byte areaProperties = (byte)((areaIcon & 0b_1111) + ((unknown1 & 0b_1111) << 4));
        writer.Write(areaProperties);

        uint last32 = (uint)(((weatherID & 0b_1111_111) << 1) +
                             ((locationType & 0b_1111) << 8) +
                             ((cameraAngleID & 0b_1111_1) << 12) +
                             ((followMode & 0b_11) << 18) +
                             ((battleBackground & 0b_1111_1) << 20) +
                             ((flags & 0b_1111_111) << 25));

        if (kantoFlag) {
          last32++;
        }

        writer.Write(last32);
      }
      return newData.ToArray();
    }
    #endregion
  }
}

using System.IO;
using System.Windows.Forms;

namespace DSPRE.ROMFiles {
  /// <summary>
  /// Class to store map header data from Pokémon Plat
  /// </summary>
  public class MapHeaderPt : MapHeader {
    public byte areaIcon { get; set; }
    public byte locationName { get; set; }
    public byte unknown1 { get; set; }

    public MapHeaderPt(ushort headerNumber, Stream data) {
      this.ID = headerNumber;
      using (BinaryReader reader = new BinaryReader(data)) {
        try {
          areaDataID = reader.ReadByte();
          unknown1 = reader.ReadByte();
          matrixID = reader.ReadUInt16();
          scriptFileID = reader.ReadUInt16();
          levelScriptID = reader.ReadUInt16();
          textArchiveID = reader.ReadUInt16();
          musicDayID = reader.ReadUInt16();
          musicNightID = reader.ReadUInt16();
          wildPokemon = reader.ReadUInt16();
          eventFileID = reader.ReadUInt16();
          locationName = reader.ReadByte();
          areaIcon = reader.ReadByte();
          weatherID = reader.ReadByte();
          cameraAngleID = reader.ReadByte();

          ushort mapSettings = reader.ReadUInt16();
          locationSpecifier = (byte)(mapSettings & 0b_1111_111);
          battleBackground = (byte)(mapSettings >> 7 & 0b_1111_1);
          flags = (byte)(mapSettings >> 12 & 0b_1111);
        }
        catch (EndOfStreamException) {
          MessageBox.Show("Error loading header " + ID + '.', "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    public override byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(newData)) {
        writer.Write(areaDataID);
        writer.Write(unknown1);
        writer.Write(matrixID);
        writer.Write(scriptFileID);
        writer.Write(levelScriptID);
        writer.Write(textArchiveID);
        writer.Write(musicDayID);
        writer.Write(musicNightID);
        writer.Write(wildPokemon);
        writer.Write(eventFileID);
        writer.Write(locationName);
        writer.Write(areaIcon);
        writer.Write(weatherID);
        writer.Write(cameraAngleID);

        ushort mapSettings = (ushort)((locationSpecifier & 0b_1111_111) + ((battleBackground & 0b_1111_1) << 7) + ((flags & 0b_1111) << 12));
        writer.Write(mapSettings);
      }

      return newData.ToArray();
    }
  }
}

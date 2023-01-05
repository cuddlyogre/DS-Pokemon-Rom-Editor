using System.IO;

namespace DSPRE.ROMFiles {
  /// <summary>
  /// Class to store map header data from Pokémon D and P
  /// </summary>
  public class MapHeaderDP : MapHeader {
    public byte unknown1 { get; set; }
    public ushort locationName { get; set; }

    public MapHeaderDP(ushort headerNumber, Stream data) {
      this.ID = headerNumber;
      using (BinaryReader reader = new BinaryReader(data)) {
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
        locationName = reader.ReadUInt16();
        weatherID = reader.ReadByte();
        cameraAngleID = reader.ReadByte();
        locationSpecifier = reader.ReadByte();

        byte mapSettings = reader.ReadByte();
        battleBackground = (byte)(mapSettings & 0b_1111);
        flags = (byte)(mapSettings >> 4 & 0b_1111);
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
        writer.Write(weatherID);
        writer.Write(cameraAngleID);
        writer.Write(locationSpecifier);

        byte mapSettings = (byte)((battleBackground & 0b_1111) + ((flags & 0b_1111) << 4));
        writer.Write(mapSettings);
      }

      return newData.ToArray();
    }
  }
}

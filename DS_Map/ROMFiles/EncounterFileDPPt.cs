using System.Collections.Generic;
using System.IO;

namespace DSPRE.ROMFiles {
  /// <summary>
  /// Class to store wild Pokemon data from Pokemon Diamond, Pearl and Platinum
  /// </summary>
  public class EncounterFileDPPt : EncounterFile {
    /* Field encounters */
    public uint[] radarPokemon = new uint[4];
    public uint[] walkingPokemon = new uint[12];

    /* Time-specific encounters */
    public uint[] morningPokemon = new uint[2];
    public uint[] nightPokemon = new uint[2];

    /* Dual slot exclusives */
    public uint[] rubyPokemon = new uint[2];
    public uint[] sapphirePokemon = new uint[2];
    public uint[] emeraldPokemon = new uint[2];
    public uint[] fireRedPokemon = new uint[2];
    public uint[] leafGreenPokemon = new uint[2];

    public EncounterFileDPPt() {
      swarmPokemon = new ushort[2];
    }

    public EncounterFileDPPt(int ID) {
      string path = Filesystem.GetEncounterPath(ID);
      Stream data = new FileStream(path, FileMode.Open);
      LoadFile(data);
    }

    public EncounterFileDPPt(Stream data) {
      LoadFile(data);
    }

    void LoadFile(Stream data) {
      using (BinaryReader reader = new BinaryReader(data)) {
        List<string> fieldsWithErrors = new List<string>();

        /* Walking encounters */
        try {
          walkingRate = (byte)reader.ReadInt32();
          for (int i = 0; i < 12; i++) {
            walkingLevels[i] = (byte)reader.ReadUInt32();
            walkingPokemon[i] = reader.ReadUInt32();
          }
        }
        catch {
          fieldsWithErrors.Add("Regular encounters");
        }

        /* Swarms */
        swarmPokemon = new ushort[2];
        for (int i = 0; i < 2; i++) {
          try {
            swarmPokemon[i] = (ushort)reader.ReadUInt32();
          }
          catch (EndOfStreamException) {
            swarmPokemon[i] = 0x00;
            fieldsWithErrors.Add("Swarms" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        /* Time-specific encounters */
        for (int i = 0; i < 2; i++) {
          try {
            morningPokemon[i] = reader.ReadUInt32();
          }
          catch {
            morningPokemon[i] = 0x00;
            fieldsWithErrors.Add("Morning encounters" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        for (int i = 0; i < 2; i++) {
          try {
            nightPokemon[i] = reader.ReadUInt32();
          }
          catch {
            nightPokemon[i] = 0x00;
            fieldsWithErrors.Add("Night encounters" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        /* Poké-Radar encounters */
        for (int i = 0; i < 4; i++) {
          try {
            radarPokemon[i] = reader.ReadUInt32();
          }
          catch {
            radarPokemon[i] = 0x00;
            fieldsWithErrors.Add("PokéRadar" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        reader.BaseStream.Position = 0xA4;

        /* Dual-slot encounters */
        for (int i = 0; i < 2; i++) {
          try {
            rubyPokemon[i] = reader.ReadUInt32();
          }
          catch {
            rubyPokemon[i] = 0x00;
            fieldsWithErrors.Add("Dual-Slot Ruby" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        for (int i = 0; i < 2; i++) {
          try {
            sapphirePokemon[i] = reader.ReadUInt32();
          }
          catch {
            sapphirePokemon[i] = 0x00;
            fieldsWithErrors.Add("Dual-Slot Sapphire" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        for (int i = 0; i < 2; i++) {
          try {
            emeraldPokemon[i] = reader.ReadUInt32();
          }
          catch {
            emeraldPokemon[i] = 0x00;
            fieldsWithErrors.Add("Dual-Slot Emerald" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        for (int i = 0; i < 2; i++) {
          try {
            fireRedPokemon[i] = reader.ReadUInt32();
          }
          catch {
            fireRedPokemon[i] = 0x00;
            fieldsWithErrors.Add("Dual-Slot FireRed" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        for (int i = 0; i < 2; i++) {
          try {
            leafGreenPokemon[i] = reader.ReadUInt32();
          }
          catch {
            leafGreenPokemon[i] = 0x00;
            fieldsWithErrors.Add("Dual-Slot LeafGreen" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        /* Surf encounters */
        try {
          surfRate = (byte)reader.ReadInt32();
          for (int i = 0; i < 5; i++) {
            surfMaxLevels[i] = reader.ReadByte();
            surfMinLevels[i] = reader.ReadByte();
            reader.BaseStream.Position += 0x2;
            surfPokemon[i] = (ushort)reader.ReadUInt32();
          }
        }
        catch {
          fieldsWithErrors.Add("Surf");
        }

        reader.BaseStream.Position = 0x124;

        /* Old Rod encounters */
        try {
          oldRodRate = (byte)reader.ReadInt32();
          for (int i = 0; i < 5; i++) {
            oldRodMaxLevels[i] = reader.ReadByte();
            oldRodMinLevels[i] = reader.ReadByte();

            reader.BaseStream.Position += 0x2;
            oldRodPokemon[i] = (ushort)reader.ReadUInt32();
          }
        }
        catch {
          fieldsWithErrors.Add("Old Rod");
        }

        /* Good Rod encounters */
        try {
          goodRodRate = (byte)reader.ReadInt32();
          for (int i = 0; i < 5; i++) {
            goodRodMaxLevels[i] = reader.ReadByte();
            goodRodMinLevels[i] = reader.ReadByte();

            reader.BaseStream.Position += 0x2;
            goodRodPokemon[i] = (ushort)reader.ReadUInt32();
          }
        }
        catch {
          fieldsWithErrors.Add("Good Rod");
        }

        /* Super Rod encounters */
        try {
          superRodRate = (byte)reader.ReadInt32();
          for (int i = 0; i < 5; i++) {
            superRodMaxLevels[i] = reader.ReadByte();
            superRodMinLevels[i] = reader.ReadByte();

            reader.BaseStream.Position += 0x2;
            superRodPokemon[i] = (ushort)reader.ReadUInt32();
          }
        }
        catch {
          fieldsWithErrors.Add("Super Rod");
        }

        if (fieldsWithErrors.Count > 0) {
          ReportErrors(fieldsWithErrors);
        }
      }
    }

    public override byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(newData)) {
        writer.Write((uint)walkingRate);

        /* Walking encounters */
        for (int i = 0; i < 12; i++) {
          writer.Write((uint)walkingLevels[i]);
          writer.Write(walkingPokemon[i]);
        }

        /* Swarms */
        for (int i = 0; i < 2; i++) {
          writer.Write((uint)swarmPokemon[i]);
        }

        /* Time-specific encounters */
        for (int i = 0; i < 2; i++) {
          writer.Write(morningPokemon[i]);
        }

        for (int i = 0; i < 2; i++) {
          writer.Write(nightPokemon[i]);
        }

        /* Poké-Radar encounters */
        for (int i = 0; i < 4; i++) {
          writer.Write(radarPokemon[i]);
        }

        writer.BaseStream.Position = 0xA4;

        /* Dual-slot encounters */
        for (int i = 0; i < 2; i++) {
          writer.Write(rubyPokemon[i]);
        }

        for (int i = 0; i < 2; i++) {
          writer.Write(sapphirePokemon[i]);
        }

        for (int i = 0; i < 2; i++) {
          writer.Write(emeraldPokemon[i]);
        }

        for (int i = 0; i < 2; i++) {
          writer.Write(fireRedPokemon[i]);
        }

        for (int i = 0; i < 2; i++) {
          writer.Write(leafGreenPokemon[i]);
        }

        /* Surf encounters */
        writer.Write((uint)surfRate);
        for (int i = 0; i < 5; i++) {
          writer.Write(surfMaxLevels[i]);
          writer.Write(surfMinLevels[i]);
          writer.BaseStream.Position += 0x2;
          writer.Write((uint)surfPokemon[i]);
        }

        writer.BaseStream.Position = 0x124;

        /* Old Rod encounters */
        writer.Write((uint)oldRodRate);
        for (int i = 0; i < 5; i++) {
          writer.Write(oldRodMaxLevels[i]);
          writer.Write(oldRodMinLevels[i]);
          writer.BaseStream.Position += 0x2;
          writer.Write((uint)oldRodPokemon[i]);
        }

        /* Good Rod encounters */
        writer.Write((uint)goodRodRate);
        for (int i = 0; i < 5; i++) {
          writer.Write(goodRodMaxLevels[i]);
          writer.Write(goodRodMinLevels[i]);
          writer.BaseStream.Position += 0x2;
          writer.Write((uint)goodRodPokemon[i]);
        }

        /* Super Rod encounters */
        writer.Write((uint)superRodRate);
        for (int i = 0; i < 5; i++) {
          writer.Write(superRodMaxLevels[i]);
          writer.Write(superRodMinLevels[i]);
          writer.BaseStream.Position += 0x2;
          writer.Write((uint)superRodPokemon[i]);
        }
      }

      return newData.ToArray();
    }

    public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
      SaveToFileExplorePath("DPPt Encounter File", EncounterFile.extension, suggestedFileName, showSuccessMessage);
    }
  }
}

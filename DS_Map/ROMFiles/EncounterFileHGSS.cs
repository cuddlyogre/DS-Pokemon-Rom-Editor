using System.Collections.Generic;
using System.IO;

namespace DSPRE.ROMFiles {
  /// <summary>
  /// Class to store wild Pokemon data from Pokemon HeartGold and SoulSilver
  /// </summary>
  public class EncounterFileHGSS : EncounterFile {
    #region Fields (9)
    public byte rockSmashRate;
    public ushort[] morningPokemon = new ushort[12];
    public ushort[] dayPokemon = new ushort[12];
    public ushort[] nightPokemon = new ushort[12];
    public ushort[] hoennMusicPokemon = new ushort[2];
    public ushort[] sinnohMusicPokemon = new ushort[2];
    public ushort[] rockSmashPokemon = new ushort[2];
    public byte[] rockSmashMinLevels = new byte[2];
    public byte[] rockSmashMaxLevels = new byte[2];
    #endregion

    #region Constructors
    public EncounterFileHGSS() {
      swarmPokemon = new ushort[4];
    }

    public EncounterFileHGSS(int ID) {
      string path = Filesystem.GetEncounterPath(ID);
      Stream data = new FileStream(path, FileMode.Open);
      LoadFile(data);
    }

    public EncounterFileHGSS(Stream data) {
      LoadFile(data);
    }

    void LoadFile(Stream data) {
      using (BinaryReader reader = new BinaryReader(data)) {
        List<string> fieldsWithErrors = new List<string>();

        /* Encounter rates */
        try {
          walkingRate = reader.ReadByte();
        } catch {
          walkingRate = 0x00;
          fieldsWithErrors.Add("Regular Encounters rate" + msgFixed);
        }

        try {
          surfRate = reader.ReadByte();
        } catch {
          surfRate = 0x00;
          fieldsWithErrors.Add("Surf rate" + msgFixed);
        }

        try { 
          rockSmashRate = reader.ReadByte();
        } catch {
          rockSmashRate = 0x00;
          fieldsWithErrors.Add("Rock Smash rate" + msgFixed);
        }

        try { 
          oldRodRate = reader.ReadByte();
        } catch {
          oldRodRate = 0x00;
          fieldsWithErrors.Add("Old Rod rate" + msgFixed);
        }

        try { 
          goodRodRate = reader.ReadByte();
        } catch {
          goodRodRate = 0x00;
          fieldsWithErrors.Add("Good Rod rate" + msgFixed);
        }

        try { 
          superRodRate = reader.ReadByte();
        } catch {
          superRodRate = 0x00;
          fieldsWithErrors.Add("Super Rod rate" + msgFixed);
        }

        reader.BaseStream.Position += 0x2;

        /* Walking encounters levels */
        for (int i = 0; i < 12; i++) {
          try { 
            walkingLevels[i] = reader.ReadByte();
          } catch {
            walkingLevels[i] = 0x00;
            fieldsWithErrors.Add("Regular Encounters" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        /* Morning walking encounters */
        for (int i = 0; i < 12; i++) {
          try {
            morningPokemon[i] = reader.ReadUInt16();
          } catch {
            morningPokemon[i] = 0x00;
            fieldsWithErrors.Add("Morning Encounters" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        /* Day walking encounters */
        for (int i = 0; i < 12; i++) {
          try { 
            dayPokemon[i] = reader.ReadUInt16();
          } catch {
            dayPokemon[i] = 0x00;
            fieldsWithErrors.Add("Day Encounters" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        /* Night walking encounters */
        for (int i = 0; i < 12; i++) {
          try { 
            nightPokemon[i] = reader.ReadUInt16();
          } catch {
            nightPokemon[i] = 0x00;
            fieldsWithErrors.Add("Night Encounters" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        /* PokéGear music encounters */
        for (int i = 0; i < 2; i++) {
          try {
            hoennMusicPokemon[i] = reader.ReadUInt16();
          } catch {
            hoennMusicPokemon[i] = 0x00;
            fieldsWithErrors.Add("Hoenn Music Encounters" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        for (int i = 0; i < 2; i++) {
          try {  
            sinnohMusicPokemon[i] = reader.ReadUInt16();
          } catch {
            sinnohMusicPokemon[i] = 0x00;
            fieldsWithErrors.Add("Sinnoh Music Encounters" + ' ' + '[' + i + ']' + msgFixed);
          }
        }

        /* Surf encounters */
        for (int i = 0; i < 5; i++) {
          try {
            surfMinLevels[i] = reader.ReadByte();
          } catch {
            surfMinLevels[i] = 0x01;
            fieldsWithErrors.Add("Surf Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
          }

          try {
            surfMaxLevels[i] = reader.ReadByte();
          } catch {
            surfMaxLevels[i] = 0x01;
            fieldsWithErrors.Add("Surf Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
          }

          try {
            surfPokemon[i] = reader.ReadUInt16();
          } catch {
            surfMinLevels[i] = 0x00;
            fieldsWithErrors.Add("Surf Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
          }
        }

        /* Rock Smash encounters */
        for (int i = 0; i < 2; i++) {
          try {
            rockSmashMinLevels[i] = reader.ReadByte();
          } catch {
            rockSmashMinLevels[i] = 0x01;
            fieldsWithErrors.Add("Rock Smash Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
          }

          try {
            rockSmashMaxLevels[i] = reader.ReadByte();
          } catch {
            rockSmashMaxLevels[i] = 0x01;
            fieldsWithErrors.Add("Rock Smash Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
          }

          try {
            rockSmashPokemon[i] = reader.ReadUInt16();
          } catch {
            rockSmashPokemon[i] = 0x00;
            fieldsWithErrors.Add("Rock Smash Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
          }
        }

        /* Old Rod encounters */
        for (int i = 0; i < 5; i++) {
          try {
            oldRodMinLevels[i] = reader.ReadByte();
          } catch {
            oldRodMinLevels[i] = 0x01;
            fieldsWithErrors.Add("Old Rod Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
          }

          try {
            oldRodMaxLevels[i] = reader.ReadByte();
          } catch {
            oldRodMaxLevels[i] = 0x01;
            fieldsWithErrors.Add("Old Rod Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
          }

          try {
            oldRodPokemon[i] = reader.ReadUInt16();
          } catch {
            oldRodPokemon[i] = 0x00;
            fieldsWithErrors.Add("Old Rod Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
          }
        }

        /* Good Rod encounters */
        for (int i = 0; i < 5; i++) {
          try {
            goodRodMinLevels[i] = reader.ReadByte();
          } catch {
            goodRodMinLevels[i] = 0x01;
            fieldsWithErrors.Add("Good Rod Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
          }

          try {
            goodRodMaxLevels[i] = reader.ReadByte();
          } catch {
            goodRodMaxLevels[i] = 0x01;
            fieldsWithErrors.Add("Good Rod Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
          }

          try {
            goodRodPokemon[i] = reader.ReadUInt16();
          } catch {
            goodRodPokemon[i] = 0x00;
            fieldsWithErrors.Add("Good Rod Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
          }
        }

        /* Super Rod encounters */
        for (int i = 0; i < 5; i++) {
          try {
            superRodMinLevels[i] = reader.ReadByte();
          } catch {
            superRodMinLevels[i] = 0x01;
            fieldsWithErrors.Add("Super Rod Encounters" + ' ' + '[' + i + ']' + " min. level" + msgFixed);
          }

          try {
            superRodMaxLevels[i] = reader.ReadByte();
          } catch {
            superRodMaxLevels[i] = 0x01;
            fieldsWithErrors.Add("Super Rod Encounters" + ' ' + '[' + i + ']' + " max. level" + msgFixed);
          }

          try {
            superRodPokemon[i] = reader.ReadUInt16();
          } catch {
            superRodPokemon[i] = 0x00;
            fieldsWithErrors.Add("Super Rod Encounters" + ' ' + '[' + i + ']' + " Pokémon" + msgFixed);
          }
        }

        /* Swarm encounters */
        swarmPokemon = new ushort[4];
        for (int i = 0; i < 4; i++) {
          try {
            swarmPokemon[i] = reader.ReadUInt16();
          } catch (EndOfStreamException) {
            swarmPokemon[i] = 0x00;
            fieldsWithErrors.Add("Swarms" + '[' + i + ']' + msgFixed);
          }
        }

        if (fieldsWithErrors.Count > 0) {
          ReportErrors(fieldsWithErrors);
        }
      }
    }
    #endregion

    #region Methods (1)
    public override byte[] ToByteArray() {
      MemoryStream newData = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(newData)) {
        /* Encounter rates */
        writer.Write(walkingRate);
        writer.Write(surfRate);
        writer.Write(rockSmashRate);
        writer.Write(oldRodRate);
        writer.Write(goodRodRate);
        writer.Write(superRodRate);

        writer.BaseStream.Position += 0x2;

        /* Walking encounters levels */
        for (int i = 0; i < 12; i++) {
          writer.Write(walkingLevels[i]);
        }

        /* Morning walking encounters */
        for (int i = 0; i < 12; i++) { 
          writer.Write(morningPokemon[i]);
        }

        /* Day walking encounters */
        for (int i = 0; i < 12; i++) {
          writer.Write(dayPokemon[i]);
        }

        /* Night walking encounters */
        for (int i = 0; i < 12; i++) {
          writer.Write(nightPokemon[i]);
        }

        /* PokéGear music encounters */
        for (int i = 0; i < 2; i++) {
          writer.Write(hoennMusicPokemon[i]);
        }

        for (int i = 0; i < 2; i++) {
          writer.Write(sinnohMusicPokemon[i]);
        }

        /* Surf encounters */
        for (int i = 0; i < 5; i++) {
          writer.Write(surfMinLevels[i]);
          writer.Write(surfMaxLevels[i]);
          writer.Write(surfPokemon[i]);
        }

        /* Rock Smash encounters */
        for (int i = 0; i < 2; i++) {
          writer.Write(rockSmashMinLevels[i]);
          writer.Write(rockSmashMaxLevels[i]);
          writer.Write(rockSmashPokemon[i]);
        }

        /* Old Rod encounters */
        for (int i = 0; i < 5; i++) {
          writer.Write(oldRodMinLevels[i]);
          writer.Write(oldRodMaxLevels[i]);
          writer.Write(oldRodPokemon[i]);
        }

        /* Good Rod encounters */
        for (int i = 0; i < 5; i++) {
          writer.Write(goodRodMinLevels[i]);
          writer.Write(goodRodMaxLevels[i]);
          writer.Write(goodRodPokemon[i]);
        }

        /* Super Rod encounters */
        for (int i = 0; i < 5; i++) {
          writer.Write(superRodMinLevels[i]);
          writer.Write(superRodMaxLevels[i]);
          writer.Write(superRodPokemon[i]);
        }

        /* Swarm encounters */
        for (int i = 0; i < 4; i++) {
          writer.Write(swarmPokemon[i]);
        }
      }
      return newData.ToArray();
    }

    public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
      SaveToFileExplorePath("HGSS Encounter File", EncounterFile.extension, suggestedFileName, showSuccessMessage);
    }
    #endregion
  }
}

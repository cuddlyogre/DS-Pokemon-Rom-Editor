using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using DSPRE.ROMFiles;

namespace DSPRE.HGSSReports {
  public class SafariZoneEncounterReport {
    private SafariZoneEncounterFile safariZoneEncounterFile;
    public HashSet<string> allEncounters = new HashSet<string>();

    public SafariZoneEncounterReport(ushort id) {
      DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames>() {
        RomInfo.DirNames.safariZone,
        RomInfo.DirNames.textArchives,
      });

      safariZoneEncounterFile = new SafariZoneEncounterFile(id);
    }

    public void WriteFile(string dir) {
      string[] pokemonNames = RomInfo.GetPokemonNames();

      //safariZoneEncounterFile.surfEncounterGroup;
      //safariZoneEncounterFile.oldRodEncounterGroup;
      //safariZoneEncounterFile.goodRodEncounterGroup;
      //safariZoneEncounterFile.superRodEncounterGroup;

      // List<SafariZoneEncounter> morningGrassEncounters = new List<SafariZoneEncounter>();
      // List<SafariZoneEncounter> dayGrassEncounters = new List<SafariZoneEncounter>();
      // List<SafariZoneEncounter> nightGrassEncounters = new List<SafariZoneEncounter>();

      BindingList<SafariZoneEncounter> encounters;
      BindingList<SafariZoneEncounter> objectEncounters;
      BindingList<SafariZoneObjectRequirement> objectRequirements;
      BindingList<SafariZoneObjectRequirement> optionalObjectRequirements;

      StringBuilder sb = new StringBuilder();
      sb.Append($"Safari Zone Encounter File {safariZoneEncounterFile.ID}\n");
      sb.Append($"Location Name: {SafariZoneEncounterFile.Names[safariZoneEncounterFile.ID]}\n");
      sb.Append("\n");

      objectRequirements = safariZoneEncounterFile.grassEncounterGroup.ObjectRequirements;
      optionalObjectRequirements = safariZoneEncounterFile.grassEncounterGroup.OptionalObjectRequirements;

      sb.Append($"Grass\n");
      sb.Append($"   Morning\n");
      sb.Append($"     Level                  Requirements\n");
      encounters = safariZoneEncounterFile.grassEncounterGroup.MorningEncounters;
      objectEncounters = safariZoneEncounterFile.grassEncounterGroup.MorningEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Day\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.grassEncounterGroup.DayEncounters;
      objectEncounters = safariZoneEncounterFile.grassEncounterGroup.DayEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Night\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.grassEncounterGroup.NightEncounters;
      objectEncounters = safariZoneEncounterFile.grassEncounterGroup.NightEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      objectRequirements = safariZoneEncounterFile.surfEncounterGroup.ObjectRequirements;
      optionalObjectRequirements = safariZoneEncounterFile.surfEncounterGroup.OptionalObjectRequirements;
      
      sb.Append($"Surfing\n");
      sb.Append($"   Morning\n");
      sb.Append($"     Level                  Requirements\n");
      encounters = safariZoneEncounterFile.surfEncounterGroup.MorningEncounters;
      objectEncounters = safariZoneEncounterFile.surfEncounterGroup.MorningEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Day\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.surfEncounterGroup.DayEncounters;
      objectEncounters = safariZoneEncounterFile.surfEncounterGroup.DayEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Night\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.surfEncounterGroup.NightEncounters;
      objectEncounters = safariZoneEncounterFile.surfEncounterGroup.NightEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      objectRequirements = safariZoneEncounterFile.oldRodEncounterGroup.ObjectRequirements;
      optionalObjectRequirements = safariZoneEncounterFile.oldRodEncounterGroup.OptionalObjectRequirements;
      
      sb.Append($"Old Rod%\n");
      sb.Append($"   Morning\n");
      sb.Append($"     Level                  Requirements\n");
      encounters = safariZoneEncounterFile.oldRodEncounterGroup.MorningEncounters;
      objectEncounters = safariZoneEncounterFile.oldRodEncounterGroup.MorningEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Day\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.oldRodEncounterGroup.DayEncounters;
      objectEncounters = safariZoneEncounterFile.oldRodEncounterGroup.DayEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Night\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.oldRodEncounterGroup.NightEncounters;
      objectEncounters = safariZoneEncounterFile.oldRodEncounterGroup.NightEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      objectRequirements = safariZoneEncounterFile.goodRodEncounterGroup.ObjectRequirements;
      optionalObjectRequirements = safariZoneEncounterFile.goodRodEncounterGroup.OptionalObjectRequirements;
      
      sb.Append($"Good Rod\n");
      sb.Append($"   Morning\n");
      sb.Append($"     Level                  Requirements\n");
      encounters = safariZoneEncounterFile.goodRodEncounterGroup.MorningEncounters;
      objectEncounters = safariZoneEncounterFile.goodRodEncounterGroup.MorningEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Day\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.goodRodEncounterGroup.DayEncounters;
      objectEncounters = safariZoneEncounterFile.goodRodEncounterGroup.DayEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Night\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.goodRodEncounterGroup.NightEncounters;
      objectEncounters = safariZoneEncounterFile.goodRodEncounterGroup.NightEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      objectRequirements = safariZoneEncounterFile.superRodEncounterGroup.ObjectRequirements;
      optionalObjectRequirements = safariZoneEncounterFile.superRodEncounterGroup.OptionalObjectRequirements;
      
      sb.Append($"Super Rod\n");
      sb.Append($"   Morning\n");
      sb.Append($"     Level                  Requirements\n");
      encounters = safariZoneEncounterFile.superRodEncounterGroup.MorningEncounters;
      objectEncounters = safariZoneEncounterFile.superRodEncounterGroup.MorningEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Day\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.superRodEncounterGroup.DayEncounters;
      objectEncounters = safariZoneEncounterFile.superRodEncounterGroup.DayEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      sb.Append($"   Night\n");
      sb.Append($"     Level\n");
      encounters = safariZoneEncounterFile.superRodEncounterGroup.NightEncounters;
      objectEncounters = safariZoneEncounterFile.superRodEncounterGroup.NightEncountersObject;
      writeEncounters(sb, pokemonNames, encounters, objectEncounters, objectRequirements, optionalObjectRequirements);
      sb.Append($"\n");

      write_text_report(dir, sb.ToString());
    }

    private void writeEncounters(StringBuilder sb, 
                                        string[] pokemonNames, 
                                        BindingList<SafariZoneEncounter> encounters, 
                                        BindingList<SafariZoneEncounter> objectEncounters, 
                                        BindingList<SafariZoneObjectRequirement> objectRequirements, 
                                        BindingList<SafariZoneObjectRequirement> optionalObjectRequirements) {
      for (int i = 0; i < encounters.Count; i++) {
        SafariZoneEncounter encounter = encounters[i];
        string pokemon = pokemonNames[encounter.pokemonID];
        sb.Append($"{encounter.level.ToString(),10} {pokemon,15}\n");
        allEncounters.Add(pokemon);
      }

      for (int i = 0; i < objectEncounters.Count; i++) {
        SafariZoneEncounter encounter = objectEncounters[i];
        string pokemon = pokemonNames[encounter.pokemonID];
        sb.Append($"{encounter.level.ToString(),10} {pokemon,15}");
        allEncounters.Add(pokemon);

        int qty = objectRequirements[i].quantity;
        string requirement = SafariZoneObjectRequirement.ObjectTypes[objectRequirements[i].typeID];
        sb.Append($"{qty,4} {requirement,-11}");

        qty = optionalObjectRequirements[i].quantity;
        requirement = SafariZoneObjectRequirement.ObjectTypes[optionalObjectRequirements[i].typeID];
        if (qty > 0) {
          sb.Append($"{qty} {requirement}");
        }

        sb.Append("\n");
      }
    }

    private void write_text_report(string dir, string reportText) {
      string report_dir = Path.Combine(dir, "safari_zone_encounters");
      if (!Directory.Exists(report_dir)) {
        Directory.CreateDirectory(report_dir);
      }

      string path = Path.Combine(report_dir, $"{safariZoneEncounterFile.ID.ToString("D4")}.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        writer.Write(reportText);
      }
    }
  }
}

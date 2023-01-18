using System;
using System.Collections.Generic;
using System.IO;
using DSPRE.ROMFiles;

namespace DSPRE.HGSSReports
{
  public static class FullReport {
    public static void WriteReports(string dir) {
      Tuple<List<string>, List<string>> headerNames = Helpers.BuildHeaderNames();
      List<string> headerListBoxNames = headerNames.Item1;
      string[] pokemonNames = RomInfo.GetPokemonNames();

      Dictionary<int, List<MapHeaderHGSS>> headerEncounters = new Dictionary<int, List<MapHeaderHGSS>>();

      //collect headers that use the same encounter file
      for (ushort i = 0; i < headerListBoxNames.Count; i++) {
        MapHeaderHGSS mapHeader = (MapHeaderHGSS)MapHeader.GetMapHeader(i);

        ushort encounterID = mapHeader.wildPokemon;
        if (!headerEncounters.ContainsKey(encounterID)) {
          headerEncounters[encounterID] = new List<MapHeaderHGSS>();
        }

        headerEncounters[encounterID].Add(mapHeader);
      }

      List<string> allEncounters = new List<string>();
      List<string> allGrassEncounters = new List<string>();
      List<string> allRadioHeonnEncounters = new List<string>();
      List<string> allRadioSinnohEncounters = new List<string>();
      List<string> allSwarmGrassEncounters = new List<string>();
      List<string> allSwarmSurfEncounters = new List<string>();
      List<string> allSwarmGoodRodEncounters = new List<string>();
      List<string> allSwarmSuperRodEncounters = new List<string>();
      List<string> allRockSmashEncounters = new List<string>();
      List<string> allSurfEncounters = new List<string>();
      List<string> allOldRodEncounters = new List<string>();
      List<string> allGoodRodEncounters = new List<string>();
      List<string> allSuperRodEncounters = new List<string>();

      List<string> allHeadbuttNormalEncounters = new List<string>();
      List<string> allHeadbuttSpecialEncounters = new List<string>();

      int headerCount = MapHeader.GetHeaderCount();
      for (ushort i = 0; i < headerCount; i++) {
        HeadbuttEncounterReport encounterReport = new HeadbuttEncounterReport(i);
        encounterReport.WriteFile(dir);

        allEncounters.AddRange(encounterReport.normalEncounters);
        allEncounters.AddRange(encounterReport.specialEncounters);

        allHeadbuttNormalEncounters.AddRange(encounterReport.normalEncounters);
        allHeadbuttSpecialEncounters.AddRange(encounterReport.specialEncounters);
      }

      foreach (KeyValuePair<int, List<MapHeaderHGSS>> kv in headerEncounters) {
        int wildPokemon = kv.Key;
        List<MapHeaderHGSS> headers = kv.Value;

        WildEditorHGSSReport report = new WildEditorHGSSReport(headers, wildPokemon);
        string report_path = Path.Combine(dir, "encounters", $"{wildPokemon.ToString("D4")}.txt");
        report.WriteFile(report_path);

        allEncounters.AddRange(report.grassEncounters);
        allEncounters.AddRange(report.radioHoennEncounters);
        allEncounters.AddRange(report.radioSinnohEncounters);
        allEncounters.AddRange(report.swarmGrassEncounters);
        allEncounters.AddRange(report.swarmSurfEncounters);
        allEncounters.AddRange(report.swarmGoodRodEncounters);
        allEncounters.AddRange(report.swarmSuperRodEncounters);
        allEncounters.AddRange(report.rockSmashEncounters);
        allEncounters.AddRange(report.surfEncounters);
        allEncounters.AddRange(report.oldRodEncounters);
        allEncounters.AddRange(report.goodRodEncounters);
        allEncounters.AddRange(report.superRodEncounters);

        allGrassEncounters.AddRange(report.grassEncounters);
        allRadioHeonnEncounters.AddRange(report.radioHoennEncounters);
        allRadioSinnohEncounters.AddRange(report.radioSinnohEncounters);
        allSwarmGrassEncounters.AddRange(report.swarmGrassEncounters);
        allSwarmSurfEncounters.AddRange(report.swarmSurfEncounters);
        allSwarmGoodRodEncounters.AddRange(report.swarmGoodRodEncounters);
        allSwarmSuperRodEncounters.AddRange(report.swarmSuperRodEncounters);
        allRockSmashEncounters.AddRange(report.rockSmashEncounters);
        allSurfEncounters.AddRange(report.surfEncounters);
        allOldRodEncounters.AddRange(report.oldRodEncounters);
        allGoodRodEncounters.AddRange(report.goodRodEncounters);
        allSuperRodEncounters.AddRange(report.superRodEncounters);
      }

      int ii = 0;

      string path = Path.Combine(dir, $"_{ii.ToString("D2")}_names.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        writer.Write(String.Join("\n", pokemonNames));
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_all_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_missing_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (!allEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_grass_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allGrassEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_radio_hoenn_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allRadioHeonnEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_radio_sinnoh_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allRadioSinnohEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_swarm_grass_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allSwarmGrassEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_swarm_surf_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allSwarmSurfEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_swarm_good_rod_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allSwarmGoodRodEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_swarm_super_rod_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allSwarmSuperRodEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_rock_smash_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allRockSmashEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_surf_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allSurfEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_old_rod_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allOldRodEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_good_rod_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allGoodRodEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_super_rod_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allSuperRodEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_normal_headbutt_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allHeadbuttNormalEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;

      path = Path.Combine(dir, $"_{ii.ToString("D2")}_special_headbutt_encounters.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        foreach (string name in pokemonNames) {
          if (allHeadbuttSpecialEncounters.Contains(name)) {
            writer.WriteLine(name);
          }
        }
      }

      ii += 1;
    }
  }
}

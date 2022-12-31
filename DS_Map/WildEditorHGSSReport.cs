using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DSPRE.ROMFiles;

namespace DSPRE {
  public class WildEditorHGSSReport {
    int id;
    string locationName;

    byte walkingRate;
    byte surfRate;
    byte rockSmashRate;
    byte oldRodRate;
    byte goodRodRate;
    byte superRodRate;

    byte twentyFirstLevel;
    byte twentySecondLevel;
    byte tenFirstLevel;
    byte tenSecondLevel;
    byte tenThirdLevel;
    byte tenFourthLevel;
    byte fiveFirstLevel;
    byte fiveSecondLevel;
    byte fourFirstLevel;
    byte fourSecondLevel;
    byte oneFirstLevel;
    byte oneSecondLevel;

    string morningTwentyFirstPokemon = "";
    string morningTwentySecondPokemon = "";
    string morningTenFirstPokemon = "";
    string morningTenSecondPokemon = "";
    string morningTenThirdPokemon = "";
    string morningTenFourthPokemon = "";
    string morningFiveFirstPokemon = "";
    string morningFiveSecondPokemon = "";
    string morningFourFirstPokemon = "";
    string morningFourSecondPokemon = "";
    string morningOneFirstPokemon = "";
    string morningOneSecondPokemon = "";

    string dayTwentyFirstPokemon = "";
    string dayTwentySecondPokemon = "";
    string dayTenFirstPokemon = "";
    string dayTenSecondPokemon = "";
    string dayTenThirdPokemon = "";
    string dayTenFourthPokemon = "";
    string dayFiveFirstPokemon = "";
    string dayFiveSecondPokemon = "";
    string dayFourFirstPokemon = "";
    string dayFourSecondPokemon = "";
    string dayOneFirstPokemon = "";
    string dayOneSecondPokemon = "";

    string nightTwentyFirstPokemon = "";
    string nightTwentySecondPokemon = "";
    string nightTenFirstPokemon = "";
    string nightTenSecondPokemon = "";
    string nightTenThirdPokemon = "";
    string nightTenFourthPokemon = "";
    string nightFiveFirstPokemon = "";
    string nightFiveSecondPokemon = "";
    string nightFourFirstPokemon = "";
    string nightFourSecondPokemon = "";
    string nightOneFirstPokemon = "";
    string nightOneSecondPokemon = "";

    string hoennFirstPokemon = "";
    string hoennSecondPokemon = "";
    string sinnohFirstPokemon = "";
    string sinnohSecondPokemon = "";

    string rockSmashNinetyPokemon = "";
    string rockSmashTenPokemon = "";
    byte rockSmashNinetyMinLevel;
    byte rockSmashTenMinLevel;
    byte rockSmashNinetyMaxLevel;
    byte rockSmashTenMaxLevel;

    string grassSwarmPokemon = "";
    string surfSwarmPokemon = "";
    string goodRodSwarmPokemon = "";
    string superRodSwarmPokemon = "";

    string surfSixtyPokemon = "";
    byte surfSixtyMinLevel;
    byte surfSixtyMaxLevel;

    string surfThirtyPokemon = "";
    byte surfThirtyMinLevel;
    byte surfThirtyMaxLevel;

    string surfFivePokemon = "";
    byte surfFiveMinLevel;
    byte surfFiveMaxLevel;

    string surfFourPokemon = "";
    byte surfFourMinLevel;
    byte surfFourMaxLevel;

    string surfOnePokemon = "";
    byte surfOneMinLevel;
    byte surfOneMaxLevel;

    string oldRodSixtyPokemon = "";
    byte oldRodSixtyMinLevel;
    byte oldRodSixtyMaxLevel;

    string oldRodThirtyPokemon = "";
    byte oldRodThirtyMinLevel;
    byte oldRodThirtyMaxLevel;

    string oldRodFivePokemon = "";
    byte oldRodFiveMinLevel;
    byte oldRodFiveMaxLevel;

    string oldRodFourPokemon = "";
    byte oldRodFourMinLevel;
    byte oldRodFourMaxLevel;

    string oldRodOnePokemon = "";
    byte oldRodOneMinLevel;
    byte oldRodOneMaxLevel;

    string goodRodFirstFortyPokemon = "";
    byte goodRodFirstFortyMinLevel;
    byte goodRodFirstFortyMaxLevel;

    string goodRodSecondFortyPokemon = "";
    byte goodRodSecondFortyMinLevel;
    byte goodRodSecondFortyMaxLevel;

    string goodRodFifteenPokemon = "";
    byte goodRodFifteenMinLevel;
    byte goodRodFifteenMaxLevel;

    string goodRodFourPokemon = "";
    byte goodRodFourMinLevel;
    byte goodRodFourMaxLevel;

    string goodRodOnePokemon = "";
    byte goodRodOneMinLevel;
    byte goodRodOneMaxLevel;

    string superRodFirstFortyPokemon = "";
    byte superRodFirstFortyMinLevel;
    byte superRodFirstFortyMaxLevel;

    string superRodSecondFortyPokemon = "";
    byte superRodSecondFortyMinLevel;
    byte superRodSecondFortyMaxLevel;

    string superRodFifteenPokemon = "";
    byte superRodFifteenMinLevel;
    byte superRodFifteenMaxLevel;

    string superRodFourPokemon = "";
    byte superRodFourMinLevel;
    byte superRodFourMaxLevel;

    string superRodOnePokemon = "";
    byte superRodOneMinLevel;
    byte superRodOneMaxLevel;

    public WildEditorHGSSReport(string locationName, int wildPokemon) {
      DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames>() { RomInfo.DirNames.encounters, RomInfo.DirNames.monIcons });

      this.id = wildPokemon;
      this.locationName = locationName;

      if (wildPokemon == RomInfo.nullEncounterID) return;
      EncounterFileHGSS currentFile = new EncounterFileHGSS(wildPokemon);

      string[] names = RomInfo.GetPokemonNames();

      // @formatter:off
      walkingRate   = currentFile.walkingRate;
      surfRate      = currentFile.surfRate;
      rockSmashRate = currentFile.rockSmashRate;
      oldRodRate    = currentFile.oldRodRate;
      goodRodRate   = currentFile.goodRodRate;
      superRodRate  = currentFile.superRodRate;

      twentyFirstLevel  = currentFile.walkingLevels[0];
      twentySecondLevel = currentFile.walkingLevels[1];
      tenFirstLevel     = currentFile.walkingLevels[2];
      tenSecondLevel    = currentFile.walkingLevels[3];
      tenThirdLevel     = currentFile.walkingLevels[4];
      tenFourthLevel    = currentFile.walkingLevels[5];
      fiveFirstLevel    = currentFile.walkingLevels[6];
      fiveSecondLevel   = currentFile.walkingLevels[7];
      fourFirstLevel    = currentFile.walkingLevels[8];
      fourSecondLevel   = currentFile.walkingLevels[9];
      oneFirstLevel     = currentFile.walkingLevels[10];
      oneSecondLevel    = currentFile.walkingLevels[11];

      morningTwentyFirstPokemon  = names[currentFile.morningPokemon[0]];
      morningTwentySecondPokemon = names[currentFile.morningPokemon[1]];
      morningTenFirstPokemon     = names[currentFile.morningPokemon[2]];
      morningTenSecondPokemon    = names[currentFile.morningPokemon[3]];
      morningTenThirdPokemon     = names[currentFile.morningPokemon[4]];
      morningTenFourthPokemon    = names[currentFile.morningPokemon[5]];
      morningFiveFirstPokemon    = names[currentFile.morningPokemon[6]];
      morningFiveSecondPokemon   = names[currentFile.morningPokemon[7]];
      morningFourFirstPokemon    = names[currentFile.morningPokemon[8]];
      morningFourSecondPokemon   = names[currentFile.morningPokemon[9]];
      morningOneFirstPokemon     = names[currentFile.morningPokemon[10]];
      morningOneSecondPokemon    = names[currentFile.morningPokemon[11]];

      dayTwentyFirstPokemon  = names[currentFile.dayPokemon[0]];
      dayTwentySecondPokemon = names[currentFile.dayPokemon[1]];
      dayTenFirstPokemon     = names[currentFile.dayPokemon[2]];
      dayTenSecondPokemon    = names[currentFile.dayPokemon[3]];
      dayTenThirdPokemon     = names[currentFile.dayPokemon[4]];
      dayTenFourthPokemon    = names[currentFile.dayPokemon[5]];
      dayFiveFirstPokemon    = names[currentFile.dayPokemon[6]];
      dayFiveSecondPokemon   = names[currentFile.dayPokemon[7]];
      dayFourFirstPokemon    = names[currentFile.dayPokemon[8]];
      dayFourSecondPokemon   = names[currentFile.dayPokemon[9]];
      dayOneFirstPokemon     = names[currentFile.dayPokemon[10]];
      dayOneSecondPokemon    = names[currentFile.dayPokemon[11]];

      nightTwentyFirstPokemon  = names[currentFile.nightPokemon[0]];
      nightTwentySecondPokemon = names[currentFile.nightPokemon[1]];
      nightTenFirstPokemon     = names[currentFile.nightPokemon[2]];
      nightTenSecondPokemon    = names[currentFile.nightPokemon[3]];
      nightTenThirdPokemon     = names[currentFile.nightPokemon[4]];
      nightTenFourthPokemon    = names[currentFile.nightPokemon[5]];
      nightFiveFirstPokemon    = names[currentFile.nightPokemon[6]];
      nightFiveSecondPokemon   = names[currentFile.nightPokemon[7]];
      nightFourFirstPokemon    = names[currentFile.nightPokemon[8]];
      nightFourSecondPokemon   = names[currentFile.nightPokemon[9]];
      nightOneFirstPokemon     = names[currentFile.nightPokemon[10]];
      nightOneSecondPokemon    = names[currentFile.nightPokemon[11]];

      hoennFirstPokemon   = names[currentFile.hoennMusicPokemon[0]];
      hoennSecondPokemon  = names[currentFile.hoennMusicPokemon[1]];
      sinnohFirstPokemon  = names[currentFile.sinnohMusicPokemon[0]];
      sinnohSecondPokemon = names[currentFile.sinnohMusicPokemon[1]];

      rockSmashNinetyPokemon  = names[currentFile.rockSmashPokemon[0]];
      rockSmashTenPokemon     = names[currentFile.rockSmashPokemon[1]];
      rockSmashNinetyMinLevel = currentFile.rockSmashMinLevels[0];
      rockSmashTenMinLevel    = currentFile.rockSmashMinLevels[1];
      rockSmashNinetyMaxLevel = currentFile.rockSmashMaxLevels[0];
      rockSmashTenMaxLevel    = currentFile.rockSmashMaxLevels[1];

      grassSwarmPokemon    = names[currentFile.swarmPokemon[0]];
      surfSwarmPokemon     = names[currentFile.swarmPokemon[1]];
      goodRodSwarmPokemon  = names[currentFile.swarmPokemon[2]];
      superRodSwarmPokemon = names[currentFile.swarmPokemon[3]];

      surfSixtyPokemon  = names[currentFile.surfPokemon[0]];
      surfSixtyMinLevel = currentFile.surfMinLevels[0];
      surfSixtyMaxLevel = currentFile.surfMaxLevels[0];

      surfThirtyPokemon  = names[currentFile.surfPokemon[1]];
      surfThirtyMinLevel = currentFile.surfMinLevels[1];
      surfThirtyMaxLevel = currentFile.surfMaxLevels[1];

      surfFivePokemon  = names[currentFile.surfPokemon[2]];
      surfFiveMinLevel = currentFile.surfMinLevels[2];
      surfFiveMaxLevel = currentFile.surfMaxLevels[2];

      surfFourPokemon  = names[currentFile.surfPokemon[3]];
      surfFourMinLevel = currentFile.surfMinLevels[3];
      surfFourMaxLevel = currentFile.surfMaxLevels[3];

      surfOnePokemon  = names[currentFile.surfPokemon[4]];
      surfOneMinLevel = currentFile.surfMinLevels[4];
      surfOneMaxLevel = currentFile.surfMaxLevels[4];

      oldRodSixtyPokemon  = names[currentFile.oldRodPokemon[0]];
      oldRodSixtyMinLevel = currentFile.oldRodMinLevels[0];
      oldRodSixtyMaxLevel = currentFile.oldRodMaxLevels[0];

      oldRodThirtyPokemon  = names[currentFile.oldRodPokemon[1]];
      oldRodThirtyMinLevel = currentFile.oldRodMinLevels[1];
      oldRodThirtyMaxLevel = currentFile.oldRodMaxLevels[1];

      oldRodFivePokemon  = names[currentFile.oldRodPokemon[2]];
      oldRodFiveMinLevel = currentFile.oldRodMinLevels[2];
      oldRodFiveMaxLevel = currentFile.oldRodMaxLevels[2];

      oldRodFourPokemon  = names[currentFile.oldRodPokemon[3]];
      oldRodFourMinLevel = currentFile.oldRodMinLevels[3];
      oldRodFourMaxLevel = currentFile.oldRodMaxLevels[3];

      oldRodOnePokemon  = names[currentFile.oldRodPokemon[4]];
      oldRodOneMinLevel = currentFile.oldRodMinLevels[4];
      oldRodOneMaxLevel = currentFile.oldRodMaxLevels[4];

      goodRodFirstFortyPokemon  = names[currentFile.goodRodPokemon[0]];
      goodRodFirstFortyMinLevel = currentFile.goodRodMinLevels[0];
      goodRodFirstFortyMaxLevel = currentFile.goodRodMaxLevels[0];

      goodRodSecondFortyPokemon  = names[currentFile.goodRodPokemon[1]];
      goodRodSecondFortyMinLevel = currentFile.goodRodMinLevels[1];
      goodRodSecondFortyMaxLevel = currentFile.goodRodMaxLevels[1];

      goodRodFifteenPokemon  = names[currentFile.goodRodPokemon[2]];
      goodRodFifteenMinLevel = currentFile.goodRodMinLevels[2];
      goodRodFifteenMaxLevel = currentFile.goodRodMaxLevels[2];

      goodRodFourPokemon  = names[currentFile.goodRodPokemon[3]];
      goodRodFourMinLevel = currentFile.goodRodMinLevels[3];
      goodRodFourMaxLevel = currentFile.goodRodMaxLevels[3];

      goodRodOnePokemon  = names[currentFile.goodRodPokemon[4]];
      goodRodOneMinLevel = currentFile.goodRodMinLevels[4];
      goodRodOneMaxLevel = currentFile.goodRodMaxLevels[4];

      superRodFirstFortyPokemon  = names[currentFile.superRodPokemon[0]];
      superRodFirstFortyMinLevel = currentFile.superRodMinLevels[0];
      superRodFirstFortyMaxLevel = currentFile.superRodMaxLevels[0];

      superRodSecondFortyPokemon  = names[currentFile.superRodPokemon[1]];
      superRodSecondFortyMinLevel = currentFile.superRodMinLevels[1];
      superRodSecondFortyMaxLevel = currentFile.superRodMaxLevels[1];

      superRodFifteenPokemon  = names[currentFile.superRodPokemon[2]];
      superRodFifteenMinLevel = currentFile.superRodMinLevels[2];
      superRodFifteenMaxLevel = currentFile.superRodMaxLevels[2];

      superRodFourPokemon  = names[currentFile.superRodPokemon[3]];
      superRodFourMinLevel = currentFile.superRodMinLevels[3];
      superRodFourMaxLevel = currentFile.superRodMaxLevels[3];

      superRodOnePokemon  = names[currentFile.superRodPokemon[4]];
      superRodOneMinLevel = currentFile.superRodMinLevels[4];
      superRodOneMaxLevel = currentFile.superRodMaxLevels[4];
      // @formatter:on
    }

    //20 10 5 4 1
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append($"Encounter File {this.id}\n");
      sb.Append($"  Walking - {walkingRate}%\n");
      sb.Append($"     Rate   Level         Morning             Day           Night\n");
      sb.Append($"      20%   {rateRow(twentyFirstLevel, morningTwentyFirstPokemon, dayTwentyFirstPokemon, nightTwentyFirstPokemon)}\n");
      sb.Append($"      20%   {rateRow(twentySecondLevel, morningTwentySecondPokemon, dayTwentySecondPokemon, nightTwentySecondPokemon)}\n");
      sb.Append($"      10%   {rateRow(tenFirstLevel, morningTenFirstPokemon, dayTenFirstPokemon, nightTenFirstPokemon)}\n");
      sb.Append($"      10%   {rateRow(tenSecondLevel, morningTenSecondPokemon, dayTenSecondPokemon, nightTenSecondPokemon)}\n");
      sb.Append($"      10%   {rateRow(tenThirdLevel, morningTenThirdPokemon, dayTenThirdPokemon, nightTenThirdPokemon)}\n");
      sb.Append($"      10%   {rateRow(tenFourthLevel, morningTenFourthPokemon, dayTenFourthPokemon, nightTenFourthPokemon)}\n");
      sb.Append($"       5%   {rateRow(fiveFirstLevel, morningFiveFirstPokemon, dayFiveFirstPokemon, nightFiveFirstPokemon)}\n");
      sb.Append($"       5%   {rateRow(fiveSecondLevel, morningFiveSecondPokemon, dayFiveSecondPokemon, nightFiveSecondPokemon)}\n");
      sb.Append($"       4%   {rateRow(fourFirstLevel, morningFourFirstPokemon, dayFourFirstPokemon, nightFourFirstPokemon)}\n");
      sb.Append($"       4%   {rateRow(fourSecondLevel, morningFourSecondPokemon, dayFourSecondPokemon, nightFourSecondPokemon)}\n");
      sb.Append($"       1%   {rateRow(oneFirstLevel, morningOneFirstPokemon, dayOneFirstPokemon, nightOneFirstPokemon)}\n");
      sb.Append($"       1%   {rateRow(oneSecondLevel, morningOneSecondPokemon, dayOneSecondPokemon, nightOneSecondPokemon)}\n");
      sb.Append("\n");
      sb.Append($"  Radio\n");
      sb.Append($"       Hoenn   {radioRow(hoennFirstPokemon, hoennSecondPokemon)}\n");
      sb.Append($"      Sinnoh   {radioRow(sinnohFirstPokemon, sinnohSecondPokemon)}\n");
      sb.Append("\n");
      sb.Append($"   Swarm\n");
      sb.Append($"       Grass   {swarmRow(grassSwarmPokemon)}\n");
      sb.Append($"        Surf   {swarmRow(surfSwarmPokemon)}\n");
      sb.Append($"    Good Rod   {swarmRow(goodRodSwarmPokemon)}\n");
      sb.Append($"   Super Rod   {swarmRow(superRodSwarmPokemon)}\n");
      sb.Append("\n");
      sb.Append($"  Rock Smash - {rockSmashRate}%\n");
      sb.Append($"        Rate         Min Level       Max Level\n");
      sb.Append($"         90%   {rockSmashRow(rockSmashNinetyMinLevel, rockSmashNinetyMaxLevel, rockSmashNinetyPokemon)}\n");
      sb.Append($"         10%   {rockSmashRow(rockSmashTenMinLevel, rockSmashTenMaxLevel, rockSmashTenPokemon)}\n");
      sb.Append("\n");
      sb.Append($"  Surfing - {surfRate}%\n");
      sb.Append($"        Rate         Min Level       Max Level\n");
      sb.Append($"         60%   {rockSmashRow(surfSixtyMinLevel, surfSixtyMaxLevel, surfSixtyPokemon)}\n");
      sb.Append($"         30%   {rockSmashRow(surfThirtyMinLevel, surfThirtyMaxLevel, surfThirtyPokemon)}\n");
      sb.Append($"          5%   {rockSmashRow(surfFiveMinLevel, surfFiveMaxLevel, surfFivePokemon)}\n");
      sb.Append($"          4%   {rockSmashRow(surfFourMinLevel, surfFourMaxLevel, surfFourPokemon)}\n");
      sb.Append($"          1%   {rockSmashRow(surfOneMinLevel, surfOneMaxLevel, surfOnePokemon)}\n");
      sb.Append("\n");
      sb.Append($"  Old Rod - {oldRodRate}%\n");
      sb.Append($"        Rate         Min Level       Max Level\n");
      sb.Append($"         60%   {rockSmashRow(oldRodSixtyMinLevel, oldRodSixtyMaxLevel, oldRodSixtyPokemon)}\n");
      sb.Append($"         30%   {rockSmashRow(oldRodThirtyMinLevel, oldRodThirtyMaxLevel, oldRodThirtyPokemon)}\n");
      sb.Append($"          5%   {rockSmashRow(oldRodFiveMinLevel, oldRodFiveMaxLevel, oldRodFivePokemon)}\n");
      sb.Append($"          4%   {rockSmashRow(oldRodFourMinLevel, oldRodFourMaxLevel, oldRodFourPokemon)}\n");
      sb.Append($"          1%   {rockSmashRow(oldRodOneMinLevel, oldRodOneMaxLevel, oldRodOnePokemon)}\n");
      sb.Append("\n");
      sb.Append($"  Good Rod - {goodRodRate}%\n");
      sb.Append($"        Rate         Min Level       Max Level\n");
      sb.Append($"         40%   {rockSmashRow(goodRodFirstFortyMinLevel, goodRodFirstFortyMaxLevel, goodRodFirstFortyPokemon)}\n");
      sb.Append($"         40%   {rockSmashRow(goodRodSecondFortyMinLevel, goodRodSecondFortyMaxLevel, goodRodSecondFortyPokemon)}\n");
      sb.Append($"         15%   {rockSmashRow(goodRodFifteenMinLevel, goodRodFifteenMaxLevel, goodRodFifteenPokemon)}\n");
      sb.Append($"          4%   {rockSmashRow(goodRodFourMinLevel, goodRodFourMaxLevel, goodRodFourPokemon)}\n");
      sb.Append($"          1%   {rockSmashRow(goodRodOneMinLevel, goodRodOneMaxLevel, goodRodOnePokemon)}\n");
      sb.Append("\n");
      sb.Append($"  Super Rod - {superRodRate}%\n");
      sb.Append($"        Rate         Min Level       Max Level\n");
      sb.Append($"         40%   {rockSmashRow(superRodFirstFortyMinLevel, superRodFirstFortyMaxLevel, superRodFirstFortyPokemon)}\n");
      sb.Append($"         40%   {rockSmashRow(superRodSecondFortyMinLevel, superRodSecondFortyMaxLevel, superRodSecondFortyPokemon)}\n");
      sb.Append($"         15%   {rockSmashRow(superRodFifteenMinLevel, superRodFifteenMaxLevel, superRodFifteenPokemon)}\n");
      sb.Append($"          4%   {rockSmashRow(superRodFourMinLevel, superRodFourMaxLevel, superRodFourPokemon)}\n");
      sb.Append($"          1%   {rockSmashRow(superRodOneMinLevel, superRodOneMaxLevel, superRodOnePokemon)}\n");
      sb.Append("\n");
      sb.Append($"{locationName}\n");
      sb.Append("\n");

      return sb.ToString();
    }

    string rateRow(byte a, string b, string c, string d) {
      return $"{a.ToString().PadLeft(5)} {b.PadLeft(15)} {c.PadLeft(15)} {d.PadLeft(15)}";
    }

    string radioRow(string a, string b) {
      return $"{a.PadLeft(15)} {b.PadLeft(15)}";
    }

    string swarmRow(string a) {
      return $"{a.PadLeft(15)}";
    }

    string rockSmashRow(byte a, byte b, string c) {
      return $"{a.ToString().PadLeft(15)} {b.ToString().PadLeft(15)} {c.PadLeft(15)}";
    }

    void WriteFile(string path) {
      using (StreamWriter writer = new StreamWriter(path)) {
        writer.Write(this.ToString());
      }
    }

    public static void WriteReports(IList items, string path) {
      Dictionary<int, List<MapHeader>> map = new Dictionary<int, List<MapHeader>>();
      TextArchive currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);

      for (ushort i = 0; i < items.Count; i++) {
        MapHeader currentHeader;
        ushort headerNumber = i;
        /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
        if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
          currentHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[RomInfo.DirNames.dynamicHeaders].unpackedDir + "\\" + headerNumber.ToString("D4"), headerNumber, 0);
        }
        else {
          currentHeader = MapHeader.LoadFromARM9(headerNumber);
        }

        ushort wildPokemon = currentHeader.wildPokemon;

        if (!map.ContainsKey(wildPokemon)) {
          map[wildPokemon] = new List<MapHeader>();
        }

        map[wildPokemon].Add(currentHeader);
      }

      foreach (KeyValuePair<int, List<MapHeader>> kv in map) {
        switch (RomInfo.gameFamily) {
          case RomInfo.GameFamilies.DP: {
            // HeaderDP h = (HeaderDP)currentHeader;
            break;
          }
          case RomInfo.GameFamilies.Plat: {
            // HeaderPt h = (HeaderPt)currentHeader;
            break;
          }
          default: {
            StringBuilder sb = new StringBuilder();
            foreach (MapHeader headerFile in kv.Value) {
              HeaderHGSS h = (HeaderHGSS)headerFile;
              sb.Append($"{h.ID} - {currentTextArchive.messages[h.locationName]}\n");
            }

            int wildPokemon = kv.Key;
            WildEditorHGSSReport er = new WildEditorHGSSReport(sb.ToString(), wildPokemon);
            er.WriteFile(Path.Combine(path, $"{wildPokemon.ToString("D4")}.txt"));
            break;
          }
        }
      }
    }
  }
}

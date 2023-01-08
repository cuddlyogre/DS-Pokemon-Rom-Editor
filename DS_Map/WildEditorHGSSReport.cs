using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using DSPRE.ROMFiles;
using LibNDSFormats.NSBMD;

namespace DSPRE {
  public class WildHeadbuttReport {
    readonly int id;
    readonly string locationName;

    private MapHeaderHGSS mapHeader;
    private GameMatrix matrix;
    private AreaData areaData;
    public static NSBMDGlRenderer mapRenderer = new NSBMDGlRenderer();
    public static NSBMDGlRenderer buildingsRenderer = new NSBMDGlRenderer();
    private static float perspective;
    private static float ang;
    private static float dist;
    private static float elev;

    private HeadbuttEncounterFile headbuttEncounterFile;

    public HashSet<string> normalEncounters = new HashSet<string>();
    public HashSet<string> specialEncounters = new HashSet<string>();

    int width = 608;
    int height = 608;
    static SimpleOpenGlControl2 openGlControl;

    public WildHeadbuttReport(ushort i) {
      DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames>() {
        RomInfo.DirNames.dynamicHeaders,
        RomInfo.DirNames.matrices,
        RomInfo.DirNames.textArchives,
        RomInfo.DirNames.areaData,
        RomInfo.DirNames.headbutt,
        RomInfo.DirNames.maps,
        RomInfo.DirNames.mapTextures,
        RomInfo.DirNames.exteriorBuildingModels,
        RomInfo.DirNames.buildingTextures,
      });

      Tuple<List<string>, List<string>> headerNames = Helpers.BuildHeaderNames();
      List<string> headerListBoxNames = headerNames.Item1;
      List<string> internalNames = headerNames.Item2;
      string[] pokemonNames = RomInfo.GetPokemonNames();
      TextArchive currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);

      this.mapHeader = (MapHeaderHGSS)MapHeader.GetMapHeader(i);
      this.headbuttEncounterFile = new HeadbuttEncounterFile(i);
      this.matrix = new GameMatrix(mapHeader.matrixID);
      this.areaData = new AreaData(mapHeader.areaDataID);
      this.locationName = currentTextArchive.messages[mapHeader.locationName];

      SetCam2DValues();

      if (openGlControl == null) {
        openGlControl = new SimpleOpenGlControl2();
        openGlControl.InitializeContexts();
        openGlControl.Width = width;
        openGlControl.Height = height;
        openGlControl.Invalidate();
        openGlControl.MakeCurrent();
      }

      foreach (HeadbuttEncounter encounter in headbuttEncounterFile.normalEncounters) {
        string pokemon = pokemonNames[encounter.pokemonID];
        normalEncounters.Add(pokemon);
      }

      foreach (HeadbuttEncounter encounter in headbuttEncounterFile.specialEncounters) {
        string pokemon = pokemonNames[encounter.pokemonID];
        specialEncounters.Add(pokemon);
      }
    }

    private void SetCam2DValues() {
      perspective = 4f;
      ang = 0f;
      dist = 115.2f;
      elev = 90f;
    }

    private Bitmap RenderMap(MapFile currentMapFile) {
      Helpers.MW_LoadModelTextures(currentMapFile, areaData.mapTileset);

      bool isInteriorMap = false;
      if (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS && areaData.areaType == AreaData.TYPE_INDOOR) {
        isInteriorMap = true;
      }

      for (int i = 0; i < currentMapFile.buildings.Count; i++) {
        Building building = currentMapFile.buildings[i];
        building.LoadModelData(isInteriorMap); // Load building nsbmd
        Helpers.MW_LoadModelTextures(building, areaData.buildingsTileset); // Load building textures                
      }

      Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, openGlControl.Width, openGlControl.Height, ang, dist, elev, perspective);
      openGlControl.Invalidate();

      return Helpers.GrabMapScreenshot(width, height);
    }

    public void BuildReport(string dir) {
      string report_dir;
      report_dir = Path.Combine(dir, "headbutt_encounters");
      if (!Directory.Exists(report_dir)) {
        Directory.CreateDirectory(report_dir);
      }

      Tuple<List<string>, List<string>> headerNames = Helpers.BuildHeaderNames();
      List<string> headerListBoxNames = headerNames.Item1;
      List<string> internalNames = headerNames.Item2;
      string[] pokemonNames = RomInfo.GetPokemonNames();
      TextArchive currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);

      StringBuilder sb = new StringBuilder();
      sb.Append($"Headbutt Encounter File {headbuttEncounterFile.ID}\n");
      sb.Append($"Header File {mapHeader.ID}\n");
      sb.Append($"Internal Name: {internalNames[mapHeader.ID].Trim()}\n");
      sb.Append($"Location Name: {locationName}\n");
      sb.Append("\n");

      sb.Append($"Normal Encounters\n");
      WriteEncounters(sb, headbuttEncounterFile.normalEncounters, pokemonNames);
      sb.Append("\n");
      Dictionary<int, List<HeadbuttTree>> normalCoordinates = WriteCoordinates(sb, headbuttEncounterFile.normalTreeGroups);
      sb.Append("\n");

      sb.Append("\n");

      sb.Append($"Special Encounters\n");
      WriteEncounters(sb, headbuttEncounterFile.specialEncounters, pokemonNames);
      sb.Append("\n");
      Dictionary<int, List<HeadbuttTree>> specialCoordinates = WriteCoordinates(sb, headbuttEncounterFile.specialTreeGroups);
      sb.Append("\n");

      write_text_report(dir, sb.ToString());
      write_headbutt_encounter_maps(dir, normalCoordinates, true);
      write_headbutt_encounter_maps(dir, specialCoordinates, false);
    }

    private void write_text_report(string dir, string reportText) {
      string path = Path.Combine(dir, $"{headbuttEncounterFile.ID.ToString("D4")}.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        writer.Write(reportText);
      }
    }

    private void write_headbutt_encounter_maps(string dir, Dictionary<int, List<HeadbuttTree>> coordinates, bool normal) {
      string report_dir = Path.Combine(dir, "headbutt_encounter_maps");
      if (!Directory.Exists(report_dir)) {
        Directory.CreateDirectory(report_dir);
      }

      foreach (KeyValuePair<int, List<HeadbuttTree>> kv in coordinates) {
        if (normal) {
          Pen paintPen = new Pen(Color.FromArgb(128, Color.LimeGreen));
          SolidBrush paintBrush = new SolidBrush(Color.FromArgb(128, Color.LimeGreen));
          writeImage(report_dir, kv.Key, kv.Value, paintPen, paintBrush);
        }
        else {
          Pen paintPen = new Pen(Color.FromArgb(128, Color.Red));
          SolidBrush paintBrush = new SolidBrush(Color.FromArgb(128, Color.Red));
          writeImage(report_dir, kv.Key, kv.Value, paintPen, paintBrush);
        }
      }
    }

    void writeImage(string dir, int mapID, List<HeadbuttTree> trees, Pen paintPen, SolidBrush paintBrush) {
      MapFile currentMapFile = new MapFile(mapID, RomInfo.gameFamily, discardMoveperms: true);
      Bitmap bm = RenderMap(currentMapFile);

      using (Graphics gSmall = Graphics.FromImage(bm)) {
        foreach (HeadbuttTree tree in trees) {
          int tileWidth = openGlControl.Width / MapFile.mapSize;
          int tileHeight = openGlControl.Height / MapFile.mapSize;
          Rectangle smallCell = new Rectangle(tree.mapX * tileWidth, tree.mapY * tileHeight, tileWidth, tileHeight);
          gSmall.DrawRectangle(paintPen, smallCell);
          gSmall.FillRectangle(paintBrush, smallCell);
        }
      }

      string path2 = Path.Combine(dir, $"{headbuttEncounterFile.ID.ToString("D4")}_{mapID.ToString("D4")}.jpg");
      bm.Save(path2, ImageFormat.Jpeg);
      bm.Dispose();
    }

    void WriteEncounters(StringBuilder sb, List<HeadbuttEncounter> encounters, string[] pokemonNames) {
      sb.Append($"   Min   Max\n");
      foreach (HeadbuttEncounter encounter in encounters) {
        string pokemon = pokemonNames[encounter.pokemonID];
        sb.Append($"   {encounter.minLevel,3}   {encounter.maxLevel,3}   {pokemon,10}\n");
      }
    }

    Dictionary<int, List<HeadbuttTree>> WriteCoordinates(StringBuilder sb, BindingList<HeadbuttTreeGroup> treeGroups) {
      Dictionary<int, List<HeadbuttTree>> ids = new Dictionary<int, List<HeadbuttTree>>();

      sb.Append($"   Coordinates\n");
      foreach (HeadbuttTreeGroup treeGroup in treeGroups) {
        foreach (HeadbuttTree tree in treeGroup.trees) {
          if (tree.unused) continue;
          sb.Append($"   {tree.globalX.ToString(),4},{tree.globalY.ToString(),4} {tree.matrixX.ToString(),2},{tree.matrixY.ToString(),2} {tree.mapX.ToString(),2},{tree.mapY.ToString(),2}\n");

          if (mapHeader.ID == GameMatrix.EMPTY) continue;
          if (tree.matrixX >= matrix.width || tree.matrixY >= matrix.height) continue;
          ushort mapIndex = matrix.maps[tree.matrixY, tree.matrixX];
          if (mapIndex == GameMatrix.EMPTY) continue;

          if (!ids.ContainsKey((int)mapIndex)) {
            ids[(int)mapIndex] = new List<HeadbuttTree>();
          }

          ids[(int)mapIndex].Add(tree);
        }
      }

      return ids;
    }

    public void WriteFile(string dir) {
      BuildReport(dir);
    }
  }

  public class WildEditorHGSSReport {
    readonly int id;
    readonly string locationName;

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

    public HashSet<string> grassEncounters = new HashSet<string>();
    public HashSet<string> swarmGrassEncounters = new HashSet<string>();
    public HashSet<string> swarmSurfEncounters = new HashSet<string>();
    public HashSet<string> swarmGoodRodEncounters = new HashSet<string>();
    public HashSet<string> swarmSuperRodEncounters = new HashSet<string>();
    public HashSet<string> radioHoennEncounters = new HashSet<string>();
    public HashSet<string> radioSinnohEncounters = new HashSet<string>();
    public HashSet<string> rockSmashEncounters = new HashSet<string>();
    public HashSet<string> surfEncounters = new HashSet<string>();
    public HashSet<string> oldRodEncounters = new HashSet<string>();
    public HashSet<string> goodRodEncounters = new HashSet<string>();
    public HashSet<string> superRodEncounters = new HashSet<string>();

    public WildEditorHGSSReport(List<MapHeaderHGSS> headers, int wildPokemon) {
      DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames>() { RomInfo.DirNames.encounters });

      Tuple<List<string>, List<string>> headerNames = Helpers.BuildHeaderNames();
      List<string> headerListBoxNames = headerNames.Item1;
      List<string> internalNames = headerNames.Item2;
      string[] pokemonNames = RomInfo.GetPokemonNames();
      TextArchive currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);

      StringBuilder sb = new StringBuilder();
      foreach (MapHeaderHGSS header in headers) {
        string _locationName = currentTextArchive.messages[header.locationName];
        sb.Append($"{header.ID} {MapHeader.nameSeparator.Trim()} {internalNames[header.ID].Trim()} - {_locationName}\n");
      }

      this.id = wildPokemon;
      this.locationName = sb.ToString();

      if (wildPokemon == RomInfo.nullEncounterID) return;

      EncounterFileHGSS currentFile = new EncounterFileHGSS(wildPokemon);
      processEncounters(currentFile, pokemonNames);
    }

    void processEncounters(EncounterFileHGSS currentFile, string[] pokemonNames) {
      walkingRate = currentFile.walkingRate;
      surfRate = currentFile.surfRate;
      rockSmashRate = currentFile.rockSmashRate;
      oldRodRate = currentFile.oldRodRate;
      goodRodRate = currentFile.goodRodRate;
      superRodRate = currentFile.superRodRate;

      twentyFirstLevel = currentFile.walkingLevels[0];
      twentySecondLevel = currentFile.walkingLevels[1];
      tenFirstLevel = currentFile.walkingLevels[2];
      tenSecondLevel = currentFile.walkingLevels[3];
      tenThirdLevel = currentFile.walkingLevels[4];
      tenFourthLevel = currentFile.walkingLevels[5];
      fiveFirstLevel = currentFile.walkingLevels[6];
      fiveSecondLevel = currentFile.walkingLevels[7];
      fourFirstLevel = currentFile.walkingLevels[8];
      fourSecondLevel = currentFile.walkingLevels[9];
      oneFirstLevel = currentFile.walkingLevels[10];
      oneSecondLevel = currentFile.walkingLevels[11];

      morningTwentyFirstPokemon = pokemonNames[currentFile.morningPokemon[0]];
      morningTwentySecondPokemon = pokemonNames[currentFile.morningPokemon[1]];
      morningTenFirstPokemon = pokemonNames[currentFile.morningPokemon[2]];
      morningTenSecondPokemon = pokemonNames[currentFile.morningPokemon[3]];
      morningTenThirdPokemon = pokemonNames[currentFile.morningPokemon[4]];
      morningTenFourthPokemon = pokemonNames[currentFile.morningPokemon[5]];
      morningFiveFirstPokemon = pokemonNames[currentFile.morningPokemon[6]];
      morningFiveSecondPokemon = pokemonNames[currentFile.morningPokemon[7]];
      morningFourFirstPokemon = pokemonNames[currentFile.morningPokemon[8]];
      morningFourSecondPokemon = pokemonNames[currentFile.morningPokemon[9]];
      morningOneFirstPokemon = pokemonNames[currentFile.morningPokemon[10]];
      morningOneSecondPokemon = pokemonNames[currentFile.morningPokemon[11]];
      grassEncounters.Add(morningTwentyFirstPokemon);
      grassEncounters.Add(morningTwentySecondPokemon);
      grassEncounters.Add(morningTenFirstPokemon);
      grassEncounters.Add(morningTenSecondPokemon);
      grassEncounters.Add(morningTenThirdPokemon);
      grassEncounters.Add(morningTenFourthPokemon);
      grassEncounters.Add(morningFiveFirstPokemon);
      grassEncounters.Add(morningFiveSecondPokemon);
      grassEncounters.Add(morningFourFirstPokemon);
      grassEncounters.Add(morningFourSecondPokemon);
      grassEncounters.Add(morningOneFirstPokemon);
      grassEncounters.Add(morningOneSecondPokemon);

      dayTwentyFirstPokemon = pokemonNames[currentFile.dayPokemon[0]];
      dayTwentySecondPokemon = pokemonNames[currentFile.dayPokemon[1]];
      dayTenFirstPokemon = pokemonNames[currentFile.dayPokemon[2]];
      dayTenSecondPokemon = pokemonNames[currentFile.dayPokemon[3]];
      dayTenThirdPokemon = pokemonNames[currentFile.dayPokemon[4]];
      dayTenFourthPokemon = pokemonNames[currentFile.dayPokemon[5]];
      dayFiveFirstPokemon = pokemonNames[currentFile.dayPokemon[6]];
      dayFiveSecondPokemon = pokemonNames[currentFile.dayPokemon[7]];
      dayFourFirstPokemon = pokemonNames[currentFile.dayPokemon[8]];
      dayFourSecondPokemon = pokemonNames[currentFile.dayPokemon[9]];
      dayOneFirstPokemon = pokemonNames[currentFile.dayPokemon[10]];
      dayOneSecondPokemon = pokemonNames[currentFile.dayPokemon[11]];
      grassEncounters.Add(dayTwentyFirstPokemon);
      grassEncounters.Add(dayTwentySecondPokemon);
      grassEncounters.Add(dayTenFirstPokemon);
      grassEncounters.Add(dayTenSecondPokemon);
      grassEncounters.Add(dayTenThirdPokemon);
      grassEncounters.Add(dayTenFourthPokemon);
      grassEncounters.Add(dayFiveFirstPokemon);
      grassEncounters.Add(dayFiveSecondPokemon);
      grassEncounters.Add(dayFourFirstPokemon);
      grassEncounters.Add(dayFourSecondPokemon);
      grassEncounters.Add(dayOneFirstPokemon);
      grassEncounters.Add(dayOneSecondPokemon);

      nightTwentyFirstPokemon = pokemonNames[currentFile.nightPokemon[0]];
      nightTwentySecondPokemon = pokemonNames[currentFile.nightPokemon[1]];
      nightTenFirstPokemon = pokemonNames[currentFile.nightPokemon[2]];
      nightTenSecondPokemon = pokemonNames[currentFile.nightPokemon[3]];
      nightTenThirdPokemon = pokemonNames[currentFile.nightPokemon[4]];
      nightTenFourthPokemon = pokemonNames[currentFile.nightPokemon[5]];
      nightFiveFirstPokemon = pokemonNames[currentFile.nightPokemon[6]];
      nightFiveSecondPokemon = pokemonNames[currentFile.nightPokemon[7]];
      nightFourFirstPokemon = pokemonNames[currentFile.nightPokemon[8]];
      nightFourSecondPokemon = pokemonNames[currentFile.nightPokemon[9]];
      nightOneFirstPokemon = pokemonNames[currentFile.nightPokemon[10]];
      nightOneSecondPokemon = pokemonNames[currentFile.nightPokemon[11]];
      grassEncounters.Add(nightTwentyFirstPokemon);
      grassEncounters.Add(nightTwentySecondPokemon);
      grassEncounters.Add(nightTenFirstPokemon);
      grassEncounters.Add(nightTenSecondPokemon);
      grassEncounters.Add(nightTenThirdPokemon);
      grassEncounters.Add(nightTenFourthPokemon);
      grassEncounters.Add(nightFiveFirstPokemon);
      grassEncounters.Add(nightFiveSecondPokemon);
      grassEncounters.Add(nightFourFirstPokemon);
      grassEncounters.Add(nightFourSecondPokemon);
      grassEncounters.Add(nightOneFirstPokemon);
      grassEncounters.Add(nightOneSecondPokemon);

      hoennFirstPokemon = pokemonNames[currentFile.hoennMusicPokemon[0]];
      hoennSecondPokemon = pokemonNames[currentFile.hoennMusicPokemon[1]];
      radioHoennEncounters.Add(hoennFirstPokemon);
      radioHoennEncounters.Add(hoennSecondPokemon);

      sinnohFirstPokemon = pokemonNames[currentFile.sinnohMusicPokemon[0]];
      sinnohSecondPokemon = pokemonNames[currentFile.sinnohMusicPokemon[1]];
      radioSinnohEncounters.Add(sinnohFirstPokemon);
      radioSinnohEncounters.Add(sinnohSecondPokemon);

      grassSwarmPokemon = pokemonNames[currentFile.swarmPokemon[0]];
      surfSwarmPokemon = pokemonNames[currentFile.swarmPokemon[1]];
      goodRodSwarmPokemon = pokemonNames[currentFile.swarmPokemon[2]];
      superRodSwarmPokemon = pokemonNames[currentFile.swarmPokemon[3]];
      swarmGrassEncounters.Add(grassSwarmPokemon);
      swarmSurfEncounters.Add(surfSwarmPokemon);
      swarmGoodRodEncounters.Add(goodRodSwarmPokemon);
      swarmSuperRodEncounters.Add(superRodSwarmPokemon);

      rockSmashNinetyPokemon = pokemonNames[currentFile.rockSmashPokemon[0]];
      rockSmashTenPokemon = pokemonNames[currentFile.rockSmashPokemon[1]];
      rockSmashNinetyMinLevel = currentFile.rockSmashMinLevels[0];
      rockSmashTenMinLevel = currentFile.rockSmashMinLevels[1];
      rockSmashNinetyMaxLevel = currentFile.rockSmashMaxLevels[0];
      rockSmashTenMaxLevel = currentFile.rockSmashMaxLevels[1];
      rockSmashEncounters.Add(rockSmashNinetyPokemon);
      rockSmashEncounters.Add(rockSmashTenPokemon);

      surfSixtyPokemon = pokemonNames[currentFile.surfPokemon[0]];
      surfSixtyMinLevel = currentFile.surfMinLevels[0];
      surfSixtyMaxLevel = currentFile.surfMaxLevels[0];
      surfThirtyPokemon = pokemonNames[currentFile.surfPokemon[1]];
      surfThirtyMinLevel = currentFile.surfMinLevels[1];
      surfThirtyMaxLevel = currentFile.surfMaxLevels[1];
      surfFivePokemon = pokemonNames[currentFile.surfPokemon[2]];
      surfFiveMinLevel = currentFile.surfMinLevels[2];
      surfFiveMaxLevel = currentFile.surfMaxLevels[2];
      surfFourPokemon = pokemonNames[currentFile.surfPokemon[3]];
      surfFourMinLevel = currentFile.surfMinLevels[3];
      surfFourMaxLevel = currentFile.surfMaxLevels[3];
      surfOnePokemon = pokemonNames[currentFile.surfPokemon[4]];
      surfOneMinLevel = currentFile.surfMinLevels[4];
      surfOneMaxLevel = currentFile.surfMaxLevels[4];
      surfEncounters.Add(surfSixtyPokemon);
      surfEncounters.Add(surfThirtyPokemon);
      surfEncounters.Add(surfFivePokemon);
      surfEncounters.Add(surfFourPokemon);
      surfEncounters.Add(surfOnePokemon);

      oldRodSixtyPokemon = pokemonNames[currentFile.oldRodPokemon[0]];
      oldRodSixtyMinLevel = currentFile.oldRodMinLevels[0];
      oldRodSixtyMaxLevel = currentFile.oldRodMaxLevels[0];
      oldRodThirtyPokemon = pokemonNames[currentFile.oldRodPokemon[1]];
      oldRodThirtyMinLevel = currentFile.oldRodMinLevels[1];
      oldRodThirtyMaxLevel = currentFile.oldRodMaxLevels[1];
      oldRodFivePokemon = pokemonNames[currentFile.oldRodPokemon[2]];
      oldRodFiveMinLevel = currentFile.oldRodMinLevels[2];
      oldRodFiveMaxLevel = currentFile.oldRodMaxLevels[2];
      oldRodFourPokemon = pokemonNames[currentFile.oldRodPokemon[3]];
      oldRodFourMinLevel = currentFile.oldRodMinLevels[3];
      oldRodFourMaxLevel = currentFile.oldRodMaxLevels[3];
      oldRodOnePokemon = pokemonNames[currentFile.oldRodPokemon[4]];
      oldRodOneMinLevel = currentFile.oldRodMinLevels[4];
      oldRodOneMaxLevel = currentFile.oldRodMaxLevels[4];
      oldRodEncounters.Add(oldRodSixtyPokemon);
      oldRodEncounters.Add(oldRodThirtyPokemon);
      oldRodEncounters.Add(oldRodFivePokemon);
      oldRodEncounters.Add(oldRodFourPokemon);
      oldRodEncounters.Add(oldRodOnePokemon);

      goodRodFirstFortyPokemon = pokemonNames[currentFile.goodRodPokemon[0]];
      goodRodFirstFortyMinLevel = currentFile.goodRodMinLevels[0];
      goodRodFirstFortyMaxLevel = currentFile.goodRodMaxLevels[0];
      goodRodSecondFortyPokemon = pokemonNames[currentFile.goodRodPokemon[1]];
      goodRodSecondFortyMinLevel = currentFile.goodRodMinLevels[1];
      goodRodSecondFortyMaxLevel = currentFile.goodRodMaxLevels[1];
      goodRodFifteenPokemon = pokemonNames[currentFile.goodRodPokemon[2]];
      goodRodFifteenMinLevel = currentFile.goodRodMinLevels[2];
      goodRodFifteenMaxLevel = currentFile.goodRodMaxLevels[2];
      goodRodFourPokemon = pokemonNames[currentFile.goodRodPokemon[3]];
      goodRodFourMinLevel = currentFile.goodRodMinLevels[3];
      goodRodFourMaxLevel = currentFile.goodRodMaxLevels[3];
      goodRodOnePokemon = pokemonNames[currentFile.goodRodPokemon[4]];
      goodRodOneMinLevel = currentFile.goodRodMinLevels[4];
      goodRodOneMaxLevel = currentFile.goodRodMaxLevels[4];
      goodRodEncounters.Add(goodRodFirstFortyPokemon);
      goodRodEncounters.Add(goodRodSecondFortyPokemon);
      goodRodEncounters.Add(goodRodFifteenPokemon);
      goodRodEncounters.Add(goodRodFourPokemon);
      goodRodEncounters.Add(goodRodOnePokemon);

      superRodFirstFortyPokemon = pokemonNames[currentFile.superRodPokemon[0]];
      superRodFirstFortyMinLevel = currentFile.superRodMinLevels[0];
      superRodFirstFortyMaxLevel = currentFile.superRodMaxLevels[0];
      superRodSecondFortyPokemon = pokemonNames[currentFile.superRodPokemon[1]];
      superRodSecondFortyMinLevel = currentFile.superRodMinLevels[1];
      superRodSecondFortyMaxLevel = currentFile.superRodMaxLevels[1];
      superRodFifteenPokemon = pokemonNames[currentFile.superRodPokemon[2]];
      superRodFifteenMinLevel = currentFile.superRodMinLevels[2];
      superRodFifteenMaxLevel = currentFile.superRodMaxLevels[2];
      superRodFourPokemon = pokemonNames[currentFile.superRodPokemon[3]];
      superRodFourMinLevel = currentFile.superRodMinLevels[3];
      superRodFourMaxLevel = currentFile.superRodMaxLevels[3];
      superRodOnePokemon = pokemonNames[currentFile.superRodPokemon[4]];
      superRodOneMinLevel = currentFile.superRodMinLevels[4];
      superRodOneMaxLevel = currentFile.superRodMaxLevels[4];
      superRodEncounters.Add(superRodFirstFortyPokemon);
      superRodEncounters.Add(superRodSecondFortyPokemon);
      superRodEncounters.Add(superRodFifteenPokemon);
      superRodEncounters.Add(superRodFourPokemon);
      superRodEncounters.Add(superRodOnePokemon);
      // @formatter:on
    }

    //20 10 5 4 1
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append($"Encounter File {this.id}\n");
      sb.Append($"{locationName}");
      sb.Append("\n");
      sb.Append($"Walking - {walkingRate}%\n");
      sb.Append($"   Rate   Level         Morning             Day           Night\n");
      sb.Append($"    20%   {rateRow(twentyFirstLevel, morningTwentyFirstPokemon, dayTwentyFirstPokemon, nightTwentyFirstPokemon)}\n");
      sb.Append($"    20%   {rateRow(twentySecondLevel, morningTwentySecondPokemon, dayTwentySecondPokemon, nightTwentySecondPokemon)}\n");
      sb.Append($"    10%   {rateRow(tenFirstLevel, morningTenFirstPokemon, dayTenFirstPokemon, nightTenFirstPokemon)}\n");
      sb.Append($"    10%   {rateRow(tenSecondLevel, morningTenSecondPokemon, dayTenSecondPokemon, nightTenSecondPokemon)}\n");
      sb.Append($"    10%   {rateRow(tenThirdLevel, morningTenThirdPokemon, dayTenThirdPokemon, nightTenThirdPokemon)}\n");
      sb.Append($"    10%   {rateRow(tenFourthLevel, morningTenFourthPokemon, dayTenFourthPokemon, nightTenFourthPokemon)}\n");
      sb.Append($"     5%   {rateRow(fiveFirstLevel, morningFiveFirstPokemon, dayFiveFirstPokemon, nightFiveFirstPokemon)}\n");
      sb.Append($"     5%   {rateRow(fiveSecondLevel, morningFiveSecondPokemon, dayFiveSecondPokemon, nightFiveSecondPokemon)}\n");
      sb.Append($"     4%   {rateRow(fourFirstLevel, morningFourFirstPokemon, dayFourFirstPokemon, nightFourFirstPokemon)}\n");
      sb.Append($"     4%   {rateRow(fourSecondLevel, morningFourSecondPokemon, dayFourSecondPokemon, nightFourSecondPokemon)}\n");
      sb.Append($"     1%   {rateRow(oneFirstLevel, morningOneFirstPokemon, dayOneFirstPokemon, nightOneFirstPokemon)}\n");
      sb.Append($"     1%   {rateRow(oneSecondLevel, morningOneSecondPokemon, dayOneSecondPokemon, nightOneSecondPokemon)}\n");
      sb.Append("\n");
      sb.Append($"Radio\n");
      sb.Append($"     Hoenn   {radioRow(hoennFirstPokemon, hoennSecondPokemon)}\n");
      sb.Append($"    Sinnoh   {radioRow(sinnohFirstPokemon, sinnohSecondPokemon)}\n");
      sb.Append("\n");
      sb.Append($"Swarm\n");
      sb.Append($"     Grass   {swarmRow(grassSwarmPokemon)}\n");
      sb.Append($"      Surf   {swarmRow(surfSwarmPokemon)}\n");
      sb.Append($"  Good Rod   {swarmRow(goodRodSwarmPokemon)}\n");
      sb.Append($" Super Rod   {swarmRow(superRodSwarmPokemon)}\n");
      sb.Append("\n");
      sb.Append($"Rock Smash - {rockSmashRate}%\n");
      sb.Append($"      Rate   Min   Max\n");
      sb.Append($"       90%   {rockSmashRow(rockSmashNinetyMinLevel, rockSmashNinetyMaxLevel, rockSmashNinetyPokemon)}\n");
      sb.Append($"       10%   {rockSmashRow(rockSmashTenMinLevel, rockSmashTenMaxLevel, rockSmashTenPokemon)}\n");
      sb.Append("\n");
      sb.Append($"Surfing - {surfRate}%\n");
      sb.Append($"      Rate   Min   Max\n");
      sb.Append($"       60%   {rockSmashRow(surfSixtyMinLevel, surfSixtyMaxLevel, surfSixtyPokemon)}\n");
      sb.Append($"       30%   {rockSmashRow(surfThirtyMinLevel, surfThirtyMaxLevel, surfThirtyPokemon)}\n");
      sb.Append($"        5%   {rockSmashRow(surfFiveMinLevel, surfFiveMaxLevel, surfFivePokemon)}\n");
      sb.Append($"        4%   {rockSmashRow(surfFourMinLevel, surfFourMaxLevel, surfFourPokemon)}\n");
      sb.Append($"        1%   {rockSmashRow(surfOneMinLevel, surfOneMaxLevel, surfOnePokemon)}\n");
      sb.Append("\n");
      sb.Append($"Old Rod - {oldRodRate}%\n");
      sb.Append($"      Rate   Min   Max\n");
      sb.Append($"       60%   {rockSmashRow(oldRodSixtyMinLevel, oldRodSixtyMaxLevel, oldRodSixtyPokemon)}\n");
      sb.Append($"       30%   {rockSmashRow(oldRodThirtyMinLevel, oldRodThirtyMaxLevel, oldRodThirtyPokemon)}\n");
      sb.Append($"        5%   {rockSmashRow(oldRodFiveMinLevel, oldRodFiveMaxLevel, oldRodFivePokemon)}\n");
      sb.Append($"        4%   {rockSmashRow(oldRodFourMinLevel, oldRodFourMaxLevel, oldRodFourPokemon)}\n");
      sb.Append($"        1%   {rockSmashRow(oldRodOneMinLevel, oldRodOneMaxLevel, oldRodOnePokemon)}\n");
      sb.Append("\n");
      sb.Append($"Good Rod - {goodRodRate}%\n");
      sb.Append($"      Rate   Min   Max\n");
      sb.Append($"       40%   {rockSmashRow(goodRodFirstFortyMinLevel, goodRodFirstFortyMaxLevel, goodRodFirstFortyPokemon)}\n");
      sb.Append($"       40%   {rockSmashRow(goodRodSecondFortyMinLevel, goodRodSecondFortyMaxLevel, goodRodSecondFortyPokemon)}\n");
      sb.Append($"       15%   {rockSmashRow(goodRodFifteenMinLevel, goodRodFifteenMaxLevel, goodRodFifteenPokemon)}\n");
      sb.Append($"        4%   {rockSmashRow(goodRodFourMinLevel, goodRodFourMaxLevel, goodRodFourPokemon)}\n");
      sb.Append($"        1%   {rockSmashRow(goodRodOneMinLevel, goodRodOneMaxLevel, goodRodOnePokemon)}\n");
      sb.Append("\n");
      sb.Append($"Super Rod - {superRodRate}%\n");
      sb.Append($"      Rate   Min   Max\n");
      sb.Append($"       40%   {rockSmashRow(superRodFirstFortyMinLevel, superRodFirstFortyMaxLevel, superRodFirstFortyPokemon)}\n");
      sb.Append($"       40%   {rockSmashRow(superRodSecondFortyMinLevel, superRodSecondFortyMaxLevel, superRodSecondFortyPokemon)}\n");
      sb.Append($"       15%   {rockSmashRow(superRodFifteenMinLevel, superRodFifteenMaxLevel, superRodFifteenPokemon)}\n");
      sb.Append($"        4%   {rockSmashRow(superRodFourMinLevel, superRodFourMaxLevel, superRodFourPokemon)}\n");
      sb.Append($"        1%   {rockSmashRow(superRodOneMinLevel, superRodOneMaxLevel, superRodOnePokemon)}\n");
      sb.Append("\n");

      return sb.ToString();
    }

    string rateRow(byte a, string b, string c, string d) {
      return $"{a.ToString().PadLeft(5)} {b.PadLeft(15)} {c.PadLeft(15)} {d.PadLeft(15)}";
    }

    string radioRow(string a, string b) {
      return $"{a.PadLeft(10)} {b.PadLeft(10)}";
    }

    string swarmRow(string a) {
      return $"{a.PadLeft(10)}";
    }

    string rockSmashRow(byte a, byte b, string c) {
      return $"{a.ToString(),3}   {b.ToString(),3}   {c,10}";
    }

    public void WriteFile(string path) {
      string report_dir = Path.GetDirectoryName(path);
      if (!Directory.Exists(report_dir)) {
        Directory.CreateDirectory(report_dir);
      }

      using (StreamWriter writer = new StreamWriter(path)) {
        writer.Write(this.ToString());
      }
    }
  }

  public static class FullEncounterReport {
    public static void WriteReports(string dir) {
      Tuple<List<string>, List<string>> headerNames = Helpers.BuildHeaderNames();
      List<string> headerListBoxNames = headerNames.Item1;
      List<string> internalNames = headerNames.Item2;
      string[] pokemonNames = RomInfo.GetPokemonNames();
      TextArchive currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);

      Dictionary<int, List<MapHeaderHGSS>> headerEncounters = new Dictionary<int, List<MapHeaderHGSS>>();
      Dictionary<MapHeaderHGSS, HeadbuttEncounterFile> headbuttEncounters = new Dictionary<MapHeaderHGSS, HeadbuttEncounterFile>();

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
        WildHeadbuttReport report = new WildHeadbuttReport(i);
        report.WriteFile(dir);

        allEncounters.AddRange(report.normalEncounters);
        allEncounters.AddRange(report.specialEncounters);

        allHeadbuttNormalEncounters.AddRange(report.normalEncounters);
        allHeadbuttSpecialEncounters.AddRange(report.specialEncounters);
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

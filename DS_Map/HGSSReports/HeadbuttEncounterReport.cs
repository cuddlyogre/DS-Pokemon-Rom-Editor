using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using DSPRE.ROMFiles;

namespace DSPRE.HGSSReports {
  public class HeadbuttEncounterReport {
    private MapHeaderHGSS mapHeader;
    private HeadbuttEncounterFile headbuttEncounterFile;
    private GameMatrix gameMatrix;
    private AreaData areaData;
    readonly string locationName;

    private static float perspective;
    private static float ang;
    private static float dist;
    private static float elev;

    public HashSet<string> normalEncounters = new HashSet<string>();
    public HashSet<string> specialEncounters = new HashSet<string>();

    int width = 608;
    int height = 608;
    static SimpleOpenGlControl2 openGlControl;

    private Pen normalPen;
    private SolidBrush normalBrush;
    private Pen specialPen;
    private SolidBrush specialBrush;

    public HeadbuttEncounterReport(ushort i) {
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

      this.mapHeader = (MapHeaderHGSS)MapHeader.GetMapHeader(i);
      this.headbuttEncounterFile = new HeadbuttEncounterFile(this.mapHeader.ID);
      this.gameMatrix = new GameMatrix(mapHeader.matrixID);
      this.areaData = new AreaData(mapHeader.areaDataID);
      TextArchive currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);
      this.locationName = currentTextArchive.messages[mapHeader.locationName];

      Color normalColor = Color.FromArgb(128, Color.DarkBlue);
      normalPen = new Pen(normalColor);
      normalBrush = new SolidBrush(normalColor);

      Color specialColor = Color.FromArgb(128, Color.DarkRed);
      specialPen = new Pen(specialColor);
      specialBrush = new SolidBrush(specialColor);

      SetCam2DValues();

      if (openGlControl == null) {
        openGlControl = new SimpleOpenGlControl2();
        openGlControl.InitializeContexts();
        openGlControl.Width = width;
        openGlControl.Height = height;
        openGlControl.Invalidate();
        openGlControl.MakeCurrent();
      }

      string[] pokemonNames = RomInfo.GetPokemonNames();
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

    public void WriteFile(string dir) {
      Tuple<List<string>, List<string>> headerNames = Helpers.BuildHeaderNames();
      List<string> internalNames = headerNames.Item2;
      string[] pokemonNames = RomInfo.GetPokemonNames();

      StringBuilder sb = new StringBuilder();
      sb.Append($"Headbutt Encounter File {headbuttEncounterFile.ID}\n");
      sb.Append($"Header File {mapHeader.ID}\n");
      sb.Append($"Internal Name: {internalNames[mapHeader.ID].Trim()}\n");
      sb.Append($"Location Name: {locationName}\n");
      sb.Append("\n");

      sb.Append($"Normal Encounters\n");
      WriteEncounters(sb, headbuttEncounterFile.normalEncounters, pokemonNames);
      sb.Append("\n");
      Dictionary<HeadbuttEncounterMap, List<HeadbuttTree>> normalCoordinates = WriteCoordinates(sb, headbuttEncounterFile.normalTreeGroups);
      sb.Append("\n");

      sb.Append("\n");

      sb.Append($"Special Encounters\n");
      WriteEncounters(sb, headbuttEncounterFile.specialEncounters, pokemonNames);
      sb.Append("\n");
      Dictionary<HeadbuttEncounterMap, List<HeadbuttTree>> specialCoordinates = WriteCoordinates(sb, headbuttEncounterFile.specialTreeGroups);
      sb.Append("\n");

      write_text_report(dir, sb.ToString());
      write_headbutt_encounter_maps(dir, normalCoordinates, specialCoordinates);
    }

    void WriteEncounters(StringBuilder sb, List<HeadbuttEncounter> encounters, string[] pokemonNames) {
      sb.Append($"   Min   Max\n");
      foreach (HeadbuttEncounter encounter in encounters) {
        string pokemon = pokemonNames[encounter.pokemonID];
        sb.Append($"   {encounter.minLevel,3}   {encounter.maxLevel,3}   {pokemon,10}\n");
      }
    }

    Dictionary<HeadbuttEncounterMap, List<HeadbuttTree>> WriteCoordinates(StringBuilder sb, BindingList<HeadbuttTreeGroup> treeGroups) {
      Dictionary<HeadbuttEncounterMap, List<HeadbuttTree>> coordinates = new Dictionary<HeadbuttEncounterMap, List<HeadbuttTree>>();

      sb.Append($"   Coordinates\n");
      foreach (HeadbuttTreeGroup treeGroup in treeGroups) {
        foreach (HeadbuttTree tree in treeGroup.trees) {
          if (tree.unused) continue;
          sb.Append($"   {tree.globalX,4},{tree.globalY,4} {tree.matrixX,2},{tree.matrixY,2} {tree.mapX,2},{tree.mapY,2}\n");

          if (mapHeader.ID == GameMatrix.EMPTY) continue;
          if (tree.matrixX >= gameMatrix.width || tree.matrixY >= gameMatrix.height) continue;
          ushort mapID = gameMatrix.maps[tree.matrixY, tree.matrixX];
          if (mapID == GameMatrix.EMPTY) continue;

          HeadbuttEncounterMap map = new HeadbuttEncounterMap(tree.matrixX, tree.matrixY, mapID);

          if (!coordinates.ContainsKey(map)) {
            coordinates[map] = new List<HeadbuttTree>();
          }

          coordinates[map].Add(tree);
        }
      }

      return coordinates;
    }

    private void write_text_report(string dir, string reportText) {
      string report_dir = Path.Combine(dir, "headbutt_encounters");
      if (!Directory.Exists(report_dir)) {
        Directory.CreateDirectory(report_dir);
      }

      string path = Path.Combine(report_dir, $"{headbuttEncounterFile.ID.ToString("D4")}.txt");
      using (StreamWriter writer = new StreamWriter(path)) {
        writer.Write(reportText);
      }
    }

    private void write_headbutt_encounter_maps(string dir, Dictionary<HeadbuttEncounterMap, List<HeadbuttTree>> normalCoordinates, Dictionary<HeadbuttEncounterMap, List<HeadbuttTree>> specialCoordinates) {
      string report_dir = Path.Combine(dir, "headbutt_encounter_maps");
      if (!Directory.Exists(report_dir)) {
        Directory.CreateDirectory(report_dir);
      }

      List<HeadbuttEncounterMap> allMaps = new List<HeadbuttEncounterMap>();
      allMaps.AddRange(normalCoordinates.Keys);
      allMaps.AddRange(specialCoordinates.Keys);
      HashSet<HeadbuttEncounterMap> umaps = new HashSet<HeadbuttEncounterMap>(allMaps);

      foreach (HeadbuttEncounterMap map in umaps) {
        MapFile currentMapFile = new MapFile(map.mapID, RomInfo.gameFamily, discardMoveperms: true);
        Bitmap bm = RenderMap(currentMapFile);

        if (normalCoordinates.ContainsKey(map)) {
          writeImage(bm, normalCoordinates[map], true);
        }

        if (specialCoordinates.ContainsKey(map)) {
          writeImage(bm, specialCoordinates[map], false);
        }

        //TODO: HeadbuttEncounterMap
        string path2 = Path.Combine(report_dir, $"{headbuttEncounterFile.ID:D4}_{map.mapID:D4}_{map.x:D2}_{map.y:D2}.jpg");
        bm.Save(path2, ImageFormat.Jpeg);
        bm.Dispose();
      }
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

      Helpers.RenderMap(ref currentMapFile, openGlControl.Width, openGlControl.Height, ang, dist, elev, perspective);
      openGlControl.Invalidate();

      return Helpers.GrabMapScreenshot(width, height);
    }

    void writeImage(Bitmap bm, List<HeadbuttTree> trees, bool normal) {
      Pen paintPen;
      SolidBrush paintBrush;

      if (normal) {
        paintPen = normalPen;
        paintBrush = normalBrush;
      }
      else {
        paintPen = specialPen;
        paintBrush = specialBrush;
      }

      using (Graphics g = Graphics.FromImage(bm)) {
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        foreach (HeadbuttTree tree in trees) {
          int tileWidth = openGlControl.Width / MapFile.mapSize;
          int tileHeight = openGlControl.Height / MapFile.mapSize;
          Rectangle smallCell = new Rectangle(tree.mapX * tileWidth, tree.mapY * tileHeight, tileWidth, tileHeight);
          g.DrawRectangle(paintPen, smallCell);
          g.FillRectangle(paintBrush, smallCell);
        }
      }
    }
  }
}

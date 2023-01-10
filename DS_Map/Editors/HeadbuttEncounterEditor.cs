using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;
using LibNDSFormats.NSBMD;
using Tao.OpenGl;

namespace DSPRE.Editors {
  public partial class HeadbuttEncounterEditor : UserControl {
    public bool headbuttEncounterEditorIsReady { get; set; } = false;

    private MapHeaderHGSS mapHeader;
    private HeadbuttEncounterFile headbuttEncounterFile;
    private GameMatrix gameMatrix;
    private AreaData areaData;
    private MapFile mapFile;
    private string locationName;

    private int width;
    private int height;

    public static NSBMDGlRenderer mapRenderer = new NSBMDGlRenderer();
    public static NSBMDGlRenderer buildingsRenderer = new NSBMDGlRenderer();
    private static float perspective;
    private static float ang;
    private static float dist;
    private static float elev;

    public HeadbuttEncounterEditor() {
      InitializeComponent();
    }

    public void SetupHeadbuttEncounterEditor(bool force = false) {
      if (headbuttEncounterEditorIsReady && !force) return;
      headbuttEncounterEditorIsReady = true;

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

      Helpers.DisableHandlers();

      headbuttEncounterEditorTabNormal.comboBoxPokemon.Items.AddRange(pokemonNames);
      headbuttEncounterEditorTabNormal.comboBoxPokemon.SelectedIndex = 0;

      headbuttEncounterEditorTabSpecial.comboBoxPokemon.Items.AddRange(pokemonNames);
      headbuttEncounterEditorTabSpecial.comboBoxPokemon.SelectedIndex = 0;

      openGlControl.InitializeContexts();
      comboBoxMapHeader.Items.AddRange(headerListBoxNames.ToArray());

      openGlPictureBox.BringToFront();
      SetCam2DValues();

      Helpers.EnableHandlers();

      if (comboBoxMapHeader.Items.Count > 0) {
        comboBoxMapHeader.SelectedIndex = 0;
      }
    }

    private void comboBoxMapHeader_SelectedIndexChanged(object sender, EventArgs e) {
      setCurrentMap((ushort)comboBoxMapHeader.SelectedIndex);
    }

    public void setCurrentMap(ushort headerID) {
      this.mapFile = null;
      comboBoxMapFile.Items.Clear();
      RenderBackground();

      if (headerID == GameMatrix.EMPTY) return;

      TextArchive currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);

      this.mapHeader = (MapHeaderHGSS)MapHeader.GetMapHeader(headerID);
      this.headbuttEncounterFile = new HeadbuttEncounterFile(this.mapHeader.ID);
      this.gameMatrix = new GameMatrix(mapHeader.matrixID);
      this.areaData = new AreaData(mapHeader.areaDataID);
      this.locationName = currentTextArchive.messages[mapHeader.locationName];

      width = openGlControl.Width;
      height = openGlControl.Height;

      HashSet<int> mapIDs = new HashSet<int>();

      foreach (HeadbuttTreeGroup treeGroup in headbuttEncounterFile.normalTreeGroups) {
        foreach (HeadbuttTree tree in treeGroup.trees) {
          if (tree.unused) continue;
          if (tree.matrixX >= gameMatrix.width || tree.matrixY >= gameMatrix.height) continue;
          int mapID = gameMatrix.maps[tree.matrixY, tree.matrixX];
          if (mapID == GameMatrix.EMPTY) return;
          mapIDs.Add(mapID);
        }
      }

      foreach (HeadbuttTreeGroup treeGroup in headbuttEncounterFile.specialTreeGroups) {
        foreach (HeadbuttTree tree in treeGroup.trees) {
          if (tree.unused) continue;
          if (tree.matrixX >= gameMatrix.width || tree.matrixY >= gameMatrix.height) continue;
          int mapID = gameMatrix.maps[tree.matrixY, tree.matrixX];
          if (mapID == GameMatrix.EMPTY) return;
          mapIDs.Add(mapID);
        }
      }

      List<int> idsList = new List<int>(mapIDs);
      idsList.Sort();
      foreach (int id in idsList) {
        comboBoxMapFile.Items.Add(id);
      }

      if (comboBoxMapFile.Items.Count > 0) {
        comboBoxMapFile.SelectedIndex = 0;
      }
    }

    private void comboBoxMapFile_SelectedIndexChanged(object sender, EventArgs e) {
      int mapID = int.Parse(comboBoxMapFile.SelectedItem.ToString());
      this.mapFile = new MapFile(mapID, RomInfo.gameFamily, discardMoveperms: true);
      RenderBackground();
    }

    private void RenderBackground() {
      Bitmap bm = RenderMap();
      openGlControl.Invalidate();
      openGlPictureBox.BackgroundImage = bm;
    }

    private Bitmap RenderMap() {
      MapFile currentMapFile = this.mapFile;

      if (currentMapFile == null) {
        Bitmap blank = new Bitmap(openGlPictureBox.Width, openGlPictureBox.Height);
        using (Graphics g = Graphics.FromImage(blank)) {
          g.Clear(Color.Black);
        }

        return blank;
      }

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
      return Helpers.GrabMapScreenshot(width, height);
    }

    public void makeCurrent() {
      openGlControl.MakeCurrent();
    }

    public void OpenHeadbuttEncounterEditor(int headerID) {
      SetupHeadbuttEncounterEditor();
      comboBoxMapHeader.SelectedIndex = headerID;
      EditorPanels.mainTabControl.SelectedTab = EditorPanels.headbuttEncounterEditorTabPage;
    }

    void buttonLoad_Click(object sender, EventArgs e) {
      try {
        openFileDialog1.InitialDirectory = Path.GetDirectoryName(openFileDialog1.FileName);
        openFileDialog1.FileName = Path.GetFileName(openFileDialog1.FileName);
      }
      catch (Exception ex) {
        openFileDialog1.InitialDirectory = Path.GetDirectoryName(Environment.SpecialFolder.UserProfile.ToString());
        openFileDialog1.FileName = Path.GetFileName(openFileDialog1.FileName);
      }

      if (openFileDialog1.ShowDialog() == DialogResult.OK) {
        //path = @"unpacked\headbutt\0000"; // normalTreeGroups =  0 specialTreeGroups = 0
        //path = @"unpacked\headbutt\0021"; // normalTreeGroups = 15 specialTreeGroups = 0
        //path = @"unpacked\headbutt\0117"; // normalTreeGroups = 56 specialTreeGroups = 0
        //path = @"unpacked\headbutt\0151"; // normalTreeGroups = 10 specialTreeGroups = 4
        string path = "";
        path = openFileDialog1.FileName;
        LoadFile(path);
      }
    }

    public void LoadFile(string path) {
      headbuttEncounterFile = new HeadbuttEncounterFile(path);

      headbuttEncounterEditorTabNormal.listBoxEncounters.DataSource = headbuttEncounterFile.normalEncounters;
      headbuttEncounterEditorTabNormal.listBoxTreeGroups.DataSource = headbuttEncounterFile.normalTreeGroups;
      headbuttEncounterEditorTabSpecial.listBoxEncounters.DataSource = headbuttEncounterFile.specialEncounters;
      headbuttEncounterEditorTabSpecial.listBoxTreeGroups.DataSource = headbuttEncounterFile.specialTreeGroups;
    }

    private void buttonSave_Click(object sender, EventArgs e) {
      doSave(String.IsNullOrWhiteSpace(saveFileDialog1.FileName));
    }

    private void buttonSaveAs_Click(object sender, EventArgs e) {
      doSave(true);
    }

    void doSave(bool saveAs) {
      try {
        saveFileDialog1.InitialDirectory = Path.GetDirectoryName(saveFileDialog1.FileName);
        saveFileDialog1.FileName = Path.GetFileName(saveFileDialog1.FileName);
      }
      catch (Exception ex) {
        saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Environment.SpecialFolder.UserProfile.ToString());
        saveFileDialog1.FileName = Path.GetFileName(saveFileDialog1.FileName);
      }

      if (saveAs) {
        if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
          headbuttEncounterFile.SaveToFile(saveFileDialog1.FileName);
        }
      }
      else {
        headbuttEncounterFile.SaveToFile(saveFileDialog1.FileName);
      }
    }

    private void mapScreenshotButton_Click(object sender, EventArgs e) {
      // MessageBox.Show("Choose where to save the map screenshot.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
      // SaveFileDialog imageSFD = new SaveFileDialog {
      //   Filter = "PNG File(*.png)|*.png",
      // };
      // if (imageSFD.ShowDialog() != DialogResult.OK) {
      //   return;
      // }
      //
      // Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, openGlControl, ang, dist, elev, perspective, mapTexturesOn, bldTexturesOn);
      //
      // int newW = 512, newH = 512;
      // Bitmap newImage = new Bitmap(newW, newH);
      // using (var graphCtr = Graphics.FromImage(newImage)) {
      //   graphCtr.SmoothingMode = SmoothingMode.HighQuality;
      //   graphCtr.InterpolationMode = InterpolationMode.NearestNeighbor;
      //   graphCtr.PixelOffsetMode = PixelOffsetMode.HighQuality;
      //   graphCtr.DrawImage(Helpers.GrabMapScreenshot(openGlControl.Width, openGlControl.Height), 0, 0, newW, newH);
      // }
      //
      // newImage.Save(imageSFD.FileName);
      // MessageBox.Show("Screenshot saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void radio2D_CheckedChanged(object sender, EventArgs e) {
      // bool _2dmodeSelected = radio2D.Checked;
      //
      // if (_2dmodeSelected) {
      //   SetCam2D();
      // }
      // else {
      //   SetCam3D();
      // }
      //
      // bldPlaceWithMouseCheckbox.Enabled = _2dmodeSelected;
      // radio3D.Checked = !_2dmodeSelected;
      //
      // bldPlaceWithMouseCheckbox_CheckedChanged(null, null);
    }

    private void wireframeCheckBox_CheckedChanged(object sender, EventArgs e) {
      SetCamWireframe();
    }

    private void SetCam2DValues() {
      perspective = 4f;
      ang = 0f;
      dist = 115.2f;
      elev = 90f;
    }

    private void SetCam2D() {
      SetCam2DValues();
      Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref mapFile, openGlControl.Width, openGlControl.Height, ang, dist, elev, perspective);
    }

    private void SetCam3DValues() {
      perspective = 45f;
      ang = 0f;
      dist = 12.8f;
      elev = 50.0f;
    }

    private void SetCam3D() {
      SetCam3DValues();
      Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref mapFile, openGlControl.Width, openGlControl.Height, ang, dist, elev, perspective);
    }

    private void SetCamWireframe() {
      // if (wireframeCheckBox.Checked) {
      //   Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
      // }
      // else {
      //   Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
      // }
      //
      // Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, openGlControl, ang, dist, elev, perspective, mapTexturesOn, bldTexturesOn);
    }
  }
}

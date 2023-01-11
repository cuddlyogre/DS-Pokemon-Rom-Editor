﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    private ListBox2 listBoxTrees;
    private HeadbuttTree headbuttTree;

    private MapHeaderHGSS mapHeader;
    private HeadbuttEncounterFile headbuttEncounterFile;
    private GameMatrix gameMatrix;
    private AreaData areaData;
    private MapFile mapFile;
    private string locationName;

    private int width;
    private int height;

    private Pen normalPen;
    private SolidBrush normalBrush;
    private Pen specialPen;
    private SolidBrush specialBrush;

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

      Color normalColor = Color.FromArgb(128, Color.DarkBlue);
      normalPen = new Pen(normalColor);
      normalBrush = new SolidBrush(normalColor);

      Color specialColor = Color.FromArgb(128, Color.DarkRed);
      specialPen = new Pen(specialColor);
      specialBrush = new SolidBrush(specialColor);

      Helpers.DisableHandlers();

      headbuttEncounterEditorTabNormal.comboBoxPokemon.Items.AddRange(pokemonNames);
      headbuttEncounterEditorTabNormal.comboBoxPokemon.SelectedIndex = 0;
      headbuttEncounterEditorTabNormal.listBoxTrees.SelectedIndexChanged += ListBoxTrees_SelectedIndexChanged;

      headbuttEncounterEditorTabSpecial.comboBoxPokemon.Items.AddRange(pokemonNames);
      headbuttEncounterEditorTabSpecial.comboBoxPokemon.SelectedIndex = 0;
      headbuttEncounterEditorTabSpecial.listBoxTrees.SelectedIndexChanged += ListBoxTrees_SelectedIndexChanged;

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
      labelLocationName.Text = "";

      listBoxTrees = null;
      headbuttTree = null;

      headbuttEncounterEditorTabNormal.Reset();
      headbuttEncounterEditorTabSpecial.Reset();

      numericUpDownTreeGlobalX.Value = 0;
      numericUpDownTreeGlobalY.Value = 0;
      numericUpDownTreeMatrixX.Value = 0;
      numericUpDownTreeMatrixY.Value = 0;
      numericUpDownTreeMapX.Value = 0;
      numericUpDownTreeMapY.Value = 0;

      RenderBackground();

      if (headerID == GameMatrix.EMPTY) return;

      this.mapHeader = (MapHeaderHGSS)MapHeader.GetMapHeader(headerID);
      this.headbuttEncounterFile = new HeadbuttEncounterFile(this.mapHeader.ID);
      this.gameMatrix = new GameMatrix(mapHeader.matrixID);
      this.areaData = new AreaData(mapHeader.areaDataID);

      TextArchive currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);
      this.locationName = currentTextArchive.messages[mapHeader.locationName];
      labelLocationName.Text = locationName;

      headbuttEncounterEditorTabNormal.SetHeadbuttEncounter(headbuttEncounterFile.normalEncounters, headbuttEncounterFile.normalTreeGroups);
      headbuttEncounterEditorTabSpecial.SetHeadbuttEncounter(headbuttEncounterFile.specialEncounters, headbuttEncounterFile.specialTreeGroups);

      List<HeadbuttEncounterMap> mapHeaderMapsIDsList = new List<HeadbuttEncounterMap>();

      if (gameMatrix.hasHeadersSection) {
        for (int y = 0; y < gameMatrix.height; y++) {
          for (int x = 0; x < gameMatrix.width; x++) {
            if (gameMatrix.headers[y, x] == mapHeader.ID) {
              int mapID = gameMatrix.maps[y, x];
              if (mapID == GameMatrix.EMPTY) continue;
              HeadbuttEncounterMap map = new HeadbuttEncounterMap(x, y, mapID);
              if (mapHeaderMapsIDsList.Contains(map)) continue;
              mapHeaderMapsIDsList.Add(map);
            }
          }
        }
      }
      else {
        for (int y = 0; y < gameMatrix.height; y++) {
          for (int x = 0; x < gameMatrix.width; x++) {
            int mapID = gameMatrix.maps[y, x];
            if (mapID == GameMatrix.EMPTY) continue;
            HeadbuttEncounterMap map = new HeadbuttEncounterMap(x, y, mapID);
            if (mapHeaderMapsIDsList.Contains(map)) continue;
            mapHeaderMapsIDsList.Add(map);
          }
        }
      }

      foreach (HeadbuttTreeGroup treeGroup in headbuttEncounterFile.normalTreeGroups) {
        foreach (HeadbuttTree tree in treeGroup.trees) {
          if (tree.unused) continue;
          if (tree.matrixX >= gameMatrix.width || tree.matrixY >= gameMatrix.height) continue;
          int x = tree.matrixX;
          int y = tree.matrixY;
          int mapID = gameMatrix.maps[y, x];
          if (mapID == GameMatrix.EMPTY) continue;
          HeadbuttEncounterMap map = new HeadbuttEncounterMap(x, y, mapID);
          if (mapHeaderMapsIDsList.Contains(map)) continue;
          mapHeaderMapsIDsList.Add(map);
        }
      }

      foreach (HeadbuttTreeGroup treeGroup in headbuttEncounterFile.specialTreeGroups) {
        foreach (HeadbuttTree tree in treeGroup.trees) {
          if (tree.unused) continue;
          if (tree.matrixX >= gameMatrix.width || tree.matrixY >= gameMatrix.height) continue;
          int x = tree.matrixX;
          int y = tree.matrixY;
          int mapID = gameMatrix.maps[y, x];
          if (mapID == GameMatrix.EMPTY) continue;
          HeadbuttEncounterMap map = new HeadbuttEncounterMap(x, y, mapID);
          if (mapHeaderMapsIDsList.Contains(map)) continue;
          mapHeaderMapsIDsList.Add(map);
        }
      }

      foreach (HeadbuttEncounterMap map in mapHeaderMapsIDsList) {
        comboBoxMapFile.Items.Add(map);
      }

      if (comboBoxMapFile.Items.Count > 0) {
        comboBoxMapFile.SelectedIndex = 0;
      }
    }

    private void comboBoxMapFile_SelectedIndexChanged(object sender, EventArgs e) {
      HeadbuttEncounterMap map = comboBoxMapFile.SelectedItem as HeadbuttEncounterMap;
      int mapID = gameMatrix.maps[map.y, map.x];
      this.mapFile = new MapFile(mapID, RomInfo.gameFamily, discardMoveperms: true);
      RenderBackground();
    }

    private void RenderBackground() {
      width = openGlControl.Width;
      height = openGlControl.Height;
      Bitmap bm = RenderMap();
      openGlControl.Invalidate();

      if (headbuttEncounterFile != null) {
        using (Graphics gSmall = Graphics.FromImage(bm)) {
          MarkTrees(gSmall, normalPen, normalBrush, headbuttEncounterFile.normalTreeGroups);
          MarkTrees(gSmall, specialPen, specialBrush, headbuttEncounterFile.specialTreeGroups);
        }
      }

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

    private void MarkTrees(Graphics gSmall, Pen paintPen, SolidBrush paintBrush, BindingList<HeadbuttTreeGroup> treeGroups) {
      HeadbuttEncounterMap map = comboBoxMapFile.SelectedItem as HeadbuttEncounterMap;
      if (map == null) return;

      foreach (HeadbuttTreeGroup treeGroup in treeGroups) {
        foreach (HeadbuttTree tree in treeGroup.trees) {
          if (tree.unused) continue;
          if (tree.matrixX != map.x || tree.matrixY != map.y) continue;
          int tileWidth = openGlControl.Width / MapFile.mapSize;
          int tileHeight = openGlControl.Height / MapFile.mapSize;
          Rectangle smallCell = new Rectangle(tree.mapX * tileWidth, tree.mapY * tileHeight, tileWidth, tileHeight);
          gSmall.DrawRectangle(paintPen, smallCell);
          gSmall.FillRectangle(paintBrush, smallCell);
        }
      }
    }

    public void makeCurrent() {
      openGlControl.MakeCurrent();
    }

    public void OpenHeadbuttEncounterEditor(int headerID) {
      SetupHeadbuttEncounterEditor();
      comboBoxMapHeader.SelectedIndex = headerID;
      EditorPanels.mainTabControl.SelectedTab = EditorPanels.headbuttEncounterEditorTabPage;
    }

    private void buttonSave_Click(object sender, EventArgs e) {
      headbuttEncounterFile.SaveToFile();
    }

    private void buttonSaveAs_Click(object sender, EventArgs e) {
      try {
        saveFileDialog1.InitialDirectory = Path.GetDirectoryName(saveFileDialog1.FileName);
        saveFileDialog1.FileName = Path.GetFileName(saveFileDialog1.FileName);
      }
      catch (Exception ex) {
        saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Environment.SpecialFolder.UserProfile.ToString());
        saveFileDialog1.FileName = Path.GetFileName(saveFileDialog1.FileName);
      }

      if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
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
      if (wireframeCheckBox.Checked) {
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
      }
      else {
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
      }

      Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref mapFile, openGlControl.Width, openGlControl.Height, ang, dist, elev, perspective);
    }

    private void ListBoxTrees_SelectedIndexChanged(object sender, EventArgs e) {
      listBoxTrees = sender as ListBox2;
      headbuttTree = listBoxTrees.SelectedItem as HeadbuttTree;
      if (headbuttTree == null) return;
      numericUpDownTreeGlobalX.Value = headbuttTree.globalX;
      numericUpDownTreeGlobalY.Value = headbuttTree.globalY;
      numericUpDownTreeMatrixX.Value = headbuttTree.matrixX;
      numericUpDownTreeMatrixY.Value = headbuttTree.matrixY;
      numericUpDownTreeMapX.Value = headbuttTree.mapX;
      numericUpDownTreeMapY.Value = headbuttTree.mapY;
    }

    private void numericUpDownTreeGlobalX_ValueChanged(object sender, EventArgs e) {
      if (headbuttTree == null) return;
      headbuttTree.globalX = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeGlobalY_ValueChanged(object sender, EventArgs e) {
      if (headbuttTree == null) return;
      headbuttTree.globalY = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeMatrixX_ValueChanged(object sender, EventArgs e) {
      if (headbuttTree == null) return;
      headbuttTree.matrixX = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeMatrixY_ValueChanged(object sender, EventArgs e) {
      if (headbuttTree == null) return;
      headbuttTree.matrixY = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeMapX_ValueChanged(object sender, EventArgs e) {
      if (headbuttTree == null) return;
      headbuttTree.mapX = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeMapY_ValueChanged(object sender, EventArgs e) {
      if (headbuttTree == null) return;
      headbuttTree.mapY = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }
  }
}

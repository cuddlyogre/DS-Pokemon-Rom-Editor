﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using NSMBe4.NSBMD;

namespace DSPRE.Editors {
  public partial class NSBTXEditor : UserControl {
    public bool nsbtxEditorIsReady { get; set; } = false;
    public NSBTX_File currentNsbtx;
    public AreaData currentAreaData;
    private float nsbtxScaleFactor = 1.0f;

    public NSBTXEditor() {
      InitializeComponent();
    }

    public void SetupNSBTXEditor(bool force = false) {
      if (nsbtxEditorIsReady && !force) return;
      nsbtxEditorIsReady = true;

      Helpers.statusLabelMessage("Attempting to unpack Tileset Editor NARCs... Please wait.");
      Update();

      DSUtils.TryUnpackNarcs(new List<DirNames> {
        DirNames.buildingTextures,
        DirNames.mapTextures,
        DirNames.buildingConfigFiles,
        DirNames.areaData
      });

      /* Fill Tileset ListBox */
      FillTilesetBox();

      /* Fill AreaData ComboBox */
      selectAreaDataListBox.Items.Clear();
      int areaDataCount = Helpers.romInfo.GetAreaDataCount();
      for (int i = 0; i < areaDataCount; i++) {
        selectAreaDataListBox.Items.Add("AreaData File " + i);
      }

      /* Enable gameVersion-specific controls */
      string[] lightTypes;

      switch (RomInfo.gameFamily) {
        case GameFamilies.DP:
        case GameFamilies.Plat:
          lightTypes = new string[3] { "Day/Night Light", "Model's light", "Unknown Light" };
          break;
        default:
          lightTypes = new string[3] { "Model's light", "Day/Night Light", "Unknown Light" };
          areaDataDynamicTexturesNumericUpDown.Enabled = true;
          areaTypeGroupbox.Enabled = true;
          break;
      }

      areaDataLightTypeComboBox.Items.Clear();
      areaDataLightTypeComboBox.Items.AddRange(lightTypes);

      if (selectAreaDataListBox.Items.Count > 0) {
        selectAreaDataListBox.SelectedIndex = 0;
      }

      if (texturePacksListBox.Items.Count > 0) {
        texturePacksListBox.SelectedIndex = 0;
      }

      if (texturesListBox.Items.Count > 0) {
        texturesListBox.SelectedIndex = 0;
      }

      if (palettesListBox.Items.Count > 0) {
        palettesListBox.SelectedIndex = 0;
      }

      Helpers.statusLabelMessage();
    }

    public void OpenNSBTXEditor(int areaDataID) {
      SetupNSBTXEditor();

      selectAreaDataListBox.SelectedIndex = areaDataID;
      if (mapTilesetRadioButton.Checked) {
        texturePacksListBox.SelectedIndex = (int)areaDataMapTilesetUpDown.Value;
      }
      else {
        texturePacksListBox.SelectedIndex = (int)areaDataBuildingTilesetUpDown.Value;
      }

      EditorPanels.mainTabControl.SelectedTab = EditorPanels.nsbtxEditorTabPage;

      if (texturesListBox.Items.Count > 0)
        texturesListBox.SelectedIndex = 0;
      if (palettesListBox.Items.Count > 0)
        palettesListBox.SelectedIndex = 0;
    }

    private void texturePacksListBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      Helpers.DisableHandlers();

      /* Clear ListBoxes */
      texturesListBox.Items.Clear();
      palettesListBox.Items.Clear();

      /* Load tileset file */
      string tilesetPath = mapTilesetRadioButton.Checked
        ? RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4")
        : RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");

      currentNsbtx = new NSBTX_File(new FileStream(tilesetPath, FileMode.Open));
      string currentItemName = texturePacksListBox.Items[texturePacksListBox.SelectedIndex].ToString();

      if (currentNsbtx.texInfo.names is null || currentNsbtx.palInfo.names is null) {
        if (!currentItemName.StartsWith("Error!")) {
          texturePacksListBox.Items[texturePacksListBox.SelectedIndex] = "Error! - " + currentItemName;
        }

        Helpers.EnableHandlers();
        return;
      }

      /* Add textures and palette slot names to ListBoxes */
      texturesListBox.Items.AddRange(currentNsbtx.texInfo.names.ToArray());
      palettesListBox.Items.AddRange(currentNsbtx.palInfo.names.ToArray());

      Helpers.EnableHandlers();

      if (texturesListBox.Items.Count > 0) {
        texturesListBox.SelectedIndex = 0;
      }
    }

    public void FillTilesetBox() {
      texturePacksListBox.Items.Clear();

      int tilesetFileCount;
      if (mapTilesetRadioButton.Checked) {
        tilesetFileCount = Helpers.romInfo.GetMapTexturesCount();
      }
      else {
        tilesetFileCount = Helpers.romInfo.GetBuildingTexturesCount();
      }

      for (int i = 0; i < tilesetFileCount; i++) {
        texturePacksListBox.Items.Add("Texture Pack " + i);
      }
    }

    private void mapTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {
      FillTilesetBox();

      try {
        if (mapTilesetRadioButton.Checked) {
          texturePacksListBox.SelectedIndex = (int)areaDataMapTilesetUpDown.Value;
        }
        else if (buildingsTilesetRadioButton.Checked) {
          texturePacksListBox.SelectedIndex = (int)areaDataBuildingTilesetUpDown.Value;
        }
      }
      catch (ArgumentOutOfRangeException) {
        texturePacksListBox.SelectedIndex = 0;
      }
    }

    private void buildingsTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {
      FillTilesetBox();
      texturePacksListBox.SelectedIndex = (int)areaDataBuildingTilesetUpDown.Value;
      if (texturesListBox.Items.Count > 0) {
        texturesListBox.SelectedIndex = 0;
      }

      if (palettesListBox.Items.Count > 0) {
        palettesListBox.SelectedIndex = 0;
      }
    }

    private void texturesListBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      Helpers.BackUpDisableHandler();
      Helpers.DisableHandlers();

      texturesLabel.Text = $"Textures [{texturesListBox.SelectedIndex + 1}/{texturesListBox.Items.Count}]";

      string findThis = texturesListBox.SelectedItem.ToString();
      string matchingPalette = findAndSelectMatchingPalette(findThis);
      if (matchingPalette == null) {
        Helpers.statusLabelError("Couldn't find a palette to match " + '"' + findThis + '"', severe: false);
      }
      else {
        //palettesListBox.SelectedIndex = 0;
        palettesListBox.SelectedItem = matchingPalette;
        Helpers.statusLabelMessage("Ready");
      }

      Helpers.RestoreDisableHandler();

      int ctrlCode = NSBTXRender(tex: Math.Max(0, texturesListBox.SelectedIndex), pal: Math.Max(0, palettesListBox.SelectedIndex), scale: nsbtxScaleFactor);
      if (matchingPalette != null && ctrlCode > 0) {
        Helpers.statusLabelError($"ERROR! The selected palette doesn't have enough colors for this Palette{ctrlCode} texture.");
      }
    }

    private int NSBTXRender(int tex, int pal, float scale = -1, NSBTX_File file = null) {
      NSBTX_File toload = file;
      if (toload is null) {
        if (currentNsbtx is null) {
          return -1;
        }

        toload = currentNsbtx;
      }

      (Bitmap bmp, int ctrlCode) ret;
      if (tex == -1 && pal == -1) {
        return -1;
      }
      else {
        ret = toload.GetBitmap(tex, pal);
      }

      if (ret.bmp != null) {
        try {
          texturePictureBox.Image = ret.bmp.Resize(scale);
          texturePictureBox.Invalidate();
        }
        catch {
        }
      }

      return ret.ctrlCode;
    }

    private string findAndSelectMatchingPalette(string findThis) {
      Helpers.statusLabelMessage("Searching palette...");

      string copy = findThis;
      while (copy.Length > 0) {
        if (palettesListBox.Items.Contains(copy + "_pl")) {
          return copy + "_pl";
        }

        if (palettesListBox.Items.Contains(copy)) {
          return copy;
        }

        copy = copy.Substring(0, copy.Length - 1);
      }

      foreach (string palette in palettesListBox.Items) {
        if (palette.StartsWith(findThis)) {
          return palette;
        }
      }

      return null;
    }

    private void palettesListBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      palettesLabel.Text = $"Palettes [{palettesListBox.SelectedIndex + 1}/{palettesListBox.Items.Count}]";

      int ctrlCode = NSBTXRender(tex: texturesListBox.SelectedIndex, pal: palettesListBox.SelectedIndex, scale: nsbtxScaleFactor);
      if (ctrlCode > 0) {
        Helpers.statusLabelError($"ERROR! The selected palette doesn't have enough colors for this Palette{ctrlCode} texture.");
      }
      else {
        Helpers.statusLabelMessage();
      }
    }

    private void importNSBTXButton_Click(object sender, EventArgs e) {
      /* Prompt user to select .nsbtx file */
      OpenFileDialog ofd = new OpenFileDialog {
        Filter = "NSBTX File (*.nsbtx)|*.nsbtx"
      };
      if (ofd.ShowDialog(this) != DialogResult.OK) {
        return;
      }

      /* Update nsbtx file */
      string tilesetPath = mapTilesetRadioButton.Checked
        ? RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4")
        : RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
      File.Copy(ofd.FileName, tilesetPath, true);

      /* Update nsbtx object in memory and controls */
      currentNsbtx = new NSMBe4.NSBMD.NSBTX_File(new FileStream(ofd.FileName, FileMode.Open));
      texturePacksListBox_SelectedIndexChanged(null, null);
      MessageBox.Show("NSBTX tileset imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void exportNSBTXButton_Click(object sender, EventArgs e) {
      SaveFileDialog sf = new SaveFileDialog {
        Filter = "NSBTX File (*.nsbtx)|*.nsbtx",
        FileName = "Texture Pack " + texturePacksListBox.SelectedIndex
      };
      if (sf.ShowDialog(this) != DialogResult.OK) {
        return;
      }

      string tilesetPath = mapTilesetRadioButton.Checked
        ? RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4")
        : RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
      File.Copy(tilesetPath, sf.FileName);

      MessageBox.Show("NSBTX tileset exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void addNSBTXButton_Click(object sender, EventArgs e) {
      /* Add new NSBTX file to the correct folder */
      if (mapTilesetRadioButton.Checked) {
        File.Copy(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + 0.ToString("D4"), RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.Items.Count.ToString("D4"));

        if (EditorPanels.mapEditor.mapEditorIsReady) {
          EditorPanels.mapEditor.mapTextureComboBox.Items.Add("Map Texture Pack [" + EditorPanels.mapEditor.mapTextureComboBox.Items.Count.ToString("D2") + "]");
        }
      }
      else {
        File.Copy(RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + 0.ToString("D4"), RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.Items.Count.ToString("D4"));
        File.Copy(RomInfo.gameDirs[DirNames.buildingConfigFiles].unpackedDir + "\\" + 0.ToString("D4"), RomInfo.gameDirs[DirNames.buildingConfigFiles].unpackedDir + "\\" + texturePacksListBox.Items.Count.ToString("D4"));

        if (EditorPanels.mapEditor.mapEditorIsReady) {
          EditorPanels.mapEditor.buildTextureComboBox.Items.Add("Building Texture Pack [" + EditorPanels.mapEditor.buildTextureComboBox.Items.Count.ToString("D2") + "]");
        }
      }

      /* Update ComboBox and select new file */
      texturePacksListBox.Items.Add("Texture Pack " + texturePacksListBox.Items.Count);
      texturePacksListBox.SelectedIndex = texturePacksListBox.Items.Count - 1;
    }

    private void locateCurrentAreaData_Click(object sender, EventArgs e) {
      Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.areaData].unpackedDir, selectAreaDataListBox.SelectedIndex.ToString("D4")));
    }

    private void exportAreaDataButton_Click(object sender, EventArgs e) {
      currentAreaData.SaveToFileExplorePath("Area Data " + selectAreaDataListBox.SelectedIndex);
    }

    private void indoorAreaRadioButton_CheckedChanged(object sender, EventArgs e) {
      currentAreaData.areaType = indoorAreaRadioButton.Checked ? AreaData.TYPE_INDOOR : AreaData.TYPE_OUTDOOR;
    }

    private void importAreaDataButton_Click(object sender, EventArgs e) {
      if (selectAreaDataListBox.SelectedIndex < 0) {
        return;
      }

      OpenFileDialog of = new OpenFileDialog {
        Filter = "AreaData File (*.bin)|*.bin"
      };

      if (of.ShowDialog(this) != DialogResult.OK) {
        return;
      }

      /* Update areadata object in memory */
      string path = RomInfo.gameDirs[DirNames.areaData].unpackedDir + "\\" + selectAreaDataListBox.SelectedIndex.ToString("D4");
      File.Copy(of.FileName, path, true);

      /* Refresh controls */
      selectAreaDataListBox_SelectedIndexChanged(sender, e);

      /* Display success message */
      MessageBox.Show("AreaData File imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void removeAreaDataButton_Click(object sender, EventArgs e) {
      if (selectAreaDataListBox.Items.Count > 1) {
        /* Delete AreaData file */
        File.Delete(RomInfo.gameDirs[DirNames.areaData].unpackedDir + "\\" + (selectAreaDataListBox.Items.Count - 1).ToString("D4"));

        /* Check if currently selected file is the last one, and in that case select the one before it */
        int lastIndex = selectAreaDataListBox.Items.Count - 1;
        if (selectAreaDataListBox.SelectedIndex == lastIndex) {
          selectAreaDataListBox.SelectedIndex--;
        }

        /* Remove item from ComboBox */
        selectAreaDataListBox.Items.RemoveAt(lastIndex);

        if (EditorPanels.eventEditor.eventEditorIsReady) {
          EditorPanels.eventEditor.eventAreaDataUpDown.Maximum--;
        }
      }
      else {
        MessageBox.Show("At least one AreaData file must be kept.", "Can't delete AreaData", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }
    }

    private void addAreaDataButton_Click(object sender, EventArgs e) {
      /* Add new NSBTX file to the correct folder */
      string areaDataDirPath = RomInfo.gameDirs[DirNames.areaData].unpackedDir;
      File.Copy(areaDataDirPath + "\\" + 0.ToString("D4"), areaDataDirPath + "\\" + selectAreaDataListBox.Items.Count.ToString("D4"));

      /* Update ComboBox and select new file */
      selectAreaDataListBox.Items.Add("AreaData File " + selectAreaDataListBox.Items.Count);
      selectAreaDataListBox.SelectedIndex = selectAreaDataListBox.Items.Count - 1;

      if (EditorPanels.eventEditor.eventEditorIsReady) {
        EditorPanels.eventEditor.eventAreaDataUpDown.Maximum++;
      }
    }

    private void saveAreaDataButton_Click(object sender, EventArgs e) {
      currentAreaData.SaveToFileDefaultDir(selectAreaDataListBox.SelectedIndex);
    }

    private void areaDataDynamicTexturesUpDown_ValueChanged(object sender, EventArgs e) {
      if (areaDataDynamicTexturesNumericUpDown.Value == areaDataDynamicTexturesNumericUpDown.Maximum) {
        areaDataDynamicTexturesNumericUpDown.ForeColor = Color.Red;
      }
      else {
        areaDataDynamicTexturesNumericUpDown.ForeColor = Color.Black;
      }

      if (Helpers.HandlersDisabled) {
        return;
      }

      currentAreaData.dynamicTextureType = (ushort)areaDataDynamicTexturesNumericUpDown.Value;
    }

    private void areaDataLightTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      currentAreaData.lightType = (byte)areaDataLightTypeComboBox.SelectedIndex;
    }

    private void areaDataMapTilesetUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      currentAreaData.mapTileset = (ushort)areaDataMapTilesetUpDown.Value;
    }

    private void areaDataBuildingTilesetUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      currentAreaData.buildingsTileset = (ushort)areaDataBuildingTilesetUpDown.Value;
    }

    private void selectAreaDataListBox_SelectedIndexChanged(object sender, EventArgs e) {
      currentAreaData = new AreaData((byte)selectAreaDataListBox.SelectedIndex);

      areaDataBuildingTilesetUpDown.Value = currentAreaData.buildingsTileset;
      areaDataMapTilesetUpDown.Value = currentAreaData.mapTileset;
      areaDataLightTypeComboBox.SelectedIndex = currentAreaData.lightType;

      Helpers.DisableHandlers();
      if (RomInfo.gameFamily == GameFamilies.HGSS) {
        areaDataDynamicTexturesNumericUpDown.Value = currentAreaData.dynamicTextureType;

        bool interior = currentAreaData.areaType == 0;
        indoorAreaRadioButton.Checked = interior;
        outdoorAreaRadioButton.Checked = !interior;
      }

      Helpers.EnableHandlers();
    }

    private void removeNSBTXButton_Click(object sender, EventArgs e) {
      if (texturePacksListBox.Items.Count > 1) {
        /* Delete NSBTX file */
        DialogResult d = MessageBox.Show("Are you sure you want to delete the last Texture Pack?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (d.Equals(DialogResult.Yes)) {
          if (mapTilesetRadioButton.Checked) {
            File.Delete(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));

            if (EditorPanels.mapEditor.mapEditorIsReady) {
              EditorPanels.mapEditor.mapTextureComboBox.Items.RemoveAt(EditorPanels.mapEditor.mapTextureComboBox.Items.Count - 1);
            }
          }
          else {
            File.Delete(RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));
            File.Delete(RomInfo.gameDirs[DirNames.buildingConfigFiles].unpackedDir + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));

            if (EditorPanels.mapEditor.mapEditorIsReady) {
              EditorPanels.mapEditor.buildTextureComboBox.Items.RemoveAt(EditorPanels.mapEditor.buildTextureComboBox.Items.Count - 1);
            }
          }

          /* Check if currently selected file is the last one, and in that case select the one before it */
          int lastIndex = texturePacksListBox.Items.Count - 1;
          if (texturePacksListBox.SelectedIndex == lastIndex) {
            texturePacksListBox.SelectedIndex--;
          }

          /* Remove item from ComboBox */
          texturePacksListBox.Items.RemoveAt(lastIndex);
        }
      }
      else {
        MessageBox.Show("At least one tileset must be kept.", "Can't delete tileset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    private void locateCurrentNsbtx_Click(object sender, EventArgs e) {
      if (mapTilesetRadioButton.Checked) {
        Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.mapTextures].unpackedDir, texturePacksListBox.SelectedIndex.ToString("D4")));
      }
      else {
        Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.buildingTextures].unpackedDir, texturePacksListBox.SelectedIndex.ToString("D4")));
      }
    }

    private void scalingTrackBar_Scroll(object sender, EventArgs e) {
      int val = (sender as TrackBar).Value;
      nsbtxScaleFactor = (float)(val > 0 ? val + 1 : Math.Pow(2, (sender as TrackBar).Value));

      scalingLabel.Text = $"x{nsbtxScaleFactor}";
      NSBTXRender(texturesListBox.SelectedIndex, palettesListBox.SelectedIndex, scale: nsbtxScaleFactor);
    }

    private void repositionImageButton_Click(object sender, EventArgs e) {
      texturePictureBox.RedrawCentered();
    }

    private void invertDragCheckbox_CheckedChanged(object sender, EventArgs e) {
      texturePictureBox.invertDrag = invertDragCheckbox.Checked;
    }
  }
}

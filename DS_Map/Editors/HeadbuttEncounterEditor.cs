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
    HeadbuttEncounterFile currentHeadbuttEncounterFile;

    public HeadbuttEncounterEditor() {
      InitializeComponent();
    }

    public void SetupHeadbuttEncounterEditor(bool force = false) {
      if (headbuttEncounterEditorIsReady && !force) return;
      headbuttEncounterEditorIsReady = true;

      DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.headbutt });

      string[] pokemonNames = RomInfo.GetPokemonNames();
      headbuttEncounterEditorTabNormal.comboBoxPokemon.Items.AddRange(pokemonNames);
      headbuttEncounterEditorTabSpecial.comboBoxPokemon.Items.AddRange(pokemonNames);

      headbuttEncounterEditorTabNormal.comboBoxPokemon.SelectedIndex = 0;
      headbuttEncounterEditorTabSpecial.comboBoxPokemon.SelectedIndex = 0;
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
        textBoxPath.Text = openFileDialog1.FileName;

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
      currentHeadbuttEncounterFile = new HeadbuttEncounterFile(path);

      headbuttEncounterEditorTabNormal.listBoxEncounters.DataSource = currentHeadbuttEncounterFile.normalEncounters;
      headbuttEncounterEditorTabNormal.listBoxTreeGroups.DataSource = currentHeadbuttEncounterFile.normalTreeGroups;
      headbuttEncounterEditorTabSpecial.listBoxEncounters.DataSource = currentHeadbuttEncounterFile.specialEncounters;
      headbuttEncounterEditorTabSpecial.listBoxTreeGroups.DataSource = currentHeadbuttEncounterFile.specialTreeGroups;
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
          currentHeadbuttEncounterFile.SaveToFile(saveFileDialog1.FileName);
        }
      }
      else {
        currentHeadbuttEncounterFile.SaveToFile(saveFileDialog1.FileName);
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

    private void SetCam2D() {
      // perspective = 4f;
      // ang = 0f;
      // dist = 115.2f;
      // elev = 90f;
      //
      // Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, openGlControl, ang, dist, elev, perspective, mapTexturesOn, bldTexturesOn);
    }

    private void SetCam3D() {
      // perspective = 45f;
      // ang = 0f;
      // dist = 12.8f;
      // elev = 50.0f;
      //
      // Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, openGlControl, ang, dist, elev, perspective, mapTexturesOn, bldTexturesOn);
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

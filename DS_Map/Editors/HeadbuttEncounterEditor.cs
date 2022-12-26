using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors {
  public partial class HeadbuttEncounterEditor : UserControl {
    public bool headbuttEncounterEditorIsReady { get; set; } = false;
    HeadbuttEncounterFile currentHeadbuttEncounterFile;

    public HeadbuttEncounterEditor() {
      InitializeComponent();
    }

    public void SetupHeadbuttEncounterEditor(bool force=false) {
      if (headbuttEncounterEditorIsReady && !force) return;
      headbuttEncounterEditorIsReady = true;
      DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.headbutt });
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
      currentHeadbuttEncounterFile = new HeadbuttEncounterFile();
      currentHeadbuttEncounterFile.parse_file(path);

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
  }
}

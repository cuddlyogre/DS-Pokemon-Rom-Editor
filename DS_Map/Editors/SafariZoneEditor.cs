using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors
{
  public partial class SafariZoneEditor : UserControl
  {
    public bool safariZoneEditorIsReady { get; set; } = false;
    private SafariZoneEncounterFile safariZoneEncounterFile;

    public SafariZoneEditor()
    {
      InitializeComponent();
    }

    public void SetupSafariZoneEditor(bool force = false) {
      if (safariZoneEditorIsReady && !force) return;
      safariZoneEditorIsReady = true;

      DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames>() {
        RomInfo.DirNames.safariZone,
        RomInfo.DirNames.textArchives,
      });

      _safariZoneEncounterGroupEditorGrass.SetPokemonNames();
      _safariZoneEncounterGroupEditorSurf.SetPokemonNames();
      _safariZoneEncounterGroupEditorOldRod.SetPokemonNames();
      _safariZoneEncounterGroupEditorGoodRod.SetPokemonNames();
      _safariZoneEncounterGroupEditorSuperRod.SetPokemonNames();

      int safariZoneCount = Filesystem.GetSafariZoneCount();
      comboBoxFileID.Items.Clear();
      for (int i = 0; i < safariZoneCount; i++) {
        comboBoxFileID.Items.Add(SafariZoneEncounterFile.Names[i]);
      }

      if (comboBoxFileID.Items.Count > 0) {
        comboBoxFileID.SelectedIndex = 0;
      }
    }

    private void comboBoxFileID_SelectedIndexChanged(object sender, EventArgs e) {
      if (comboBoxFileID.SelectedIndex == -1) {
        safariZoneEncounterFile = null;
        _safariZoneEncounterGroupEditorGrass.Reset();
        _safariZoneEncounterGroupEditorSurf.Reset();
        _safariZoneEncounterGroupEditorOldRod.Reset();
        _safariZoneEncounterGroupEditorGoodRod.Reset();
        _safariZoneEncounterGroupEditorSuperRod.Reset();
        return;
      };

      safariZoneEncounterFile = new SafariZoneEncounterFile(comboBoxFileID.SelectedIndex);
      _safariZoneEncounterGroupEditorGrass.SetData(safariZoneEncounterFile.grassEncounterGroup);
      _safariZoneEncounterGroupEditorSurf.SetData(safariZoneEncounterFile.surfEncounterGroup);
      _safariZoneEncounterGroupEditorOldRod.SetData(safariZoneEncounterFile.oldRodEncounterGroup);
      _safariZoneEncounterGroupEditorGoodRod.SetData(safariZoneEncounterFile.goodRodEncounterGroup);
      _safariZoneEncounterGroupEditorSuperRod.SetData(safariZoneEncounterFile.superRodEncounterGroup);
    }

    private void buttonSave_Click(object sender, EventArgs e)
    {

    }

    private void buttonSaveAs_Click(object sender, EventArgs e)
    {

    }
  }
}

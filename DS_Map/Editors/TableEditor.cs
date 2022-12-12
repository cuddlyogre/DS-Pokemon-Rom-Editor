﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using Ekona.Images;

namespace DSPRE.Editors {
  public partial class TableEditor : UserControl {
    public bool tableEditorIsReady { get; set; } = false;
    string[] pokeNames;
    string[] trcNames;

    List<(ushort header, ushort flag, ushort music)> conditionalMusicTable;
    uint conditionalMusicTableStartAddress;

    public List<(int trainerClass, int comboID)> vsTrainerEffectsList;
    List<(int pokemonID, int comboID)> vsPokemonEffectsList;
    List<(ushort vsGraph, ushort battleSSEQ)> effectsComboTable;

    uint vsTrainerTableStartAddress;
    uint vsPokemonTableStartAddress;
    uint effectsComboMainTableStartAddress;

    //Show Pokemon Icons
    private readonly PaletteBase tableEditorMonIconPal;
    private readonly ImageBase tableEditorMonIconTile;
    private readonly SpriteBase tableEditorMonIconSprite;

    public TableEditor() {
      InitializeComponent();
    }

    public void SetupTableEditor() {
      switch (RomInfo.gameFamily) {
        case GameFamilies.HGSS:
          RomInfo.SetConditionalMusicTableOffsetToRAMAddress();
          conditionalMusicTable = new List<(ushort, ushort, ushort)>();

          conditionalMusicTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.conditionalMusicTableOffsetToRAMAddress, 4), 0) - 0x02000000;
          byte tableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.conditionalMusicTableOffsetToRAMAddress - 8);

          conditionalMusicTableListBox.Items.Clear();
          using (DSUtils.ARM9.Reader ar = new DSUtils.ARM9.Reader(conditionalMusicTableStartAddress)) {
            for (int i = 0; i < tableEntriesCount; i++) {
              ushort header = ar.ReadUInt16();
              ushort flag = ar.ReadUInt16();
              ushort musicID = ar.ReadUInt16();

              conditionalMusicTable.Add((header, flag, musicID));
              conditionalMusicTableListBox.Items.Add(EditorPanels.headerEditor.headerListBox.Items[header]);
            }
          }

          headerConditionalMusicComboBox.Items.Clear();
          foreach (string location in EditorPanels.headerEditor.headerListBox.Items) {
            headerConditionalMusicComboBox.Items.Add(location);
          }

          if (conditionalMusicTableListBox.Items.Count > 0) {
            conditionalMusicTableListBox.SelectedIndex = 0;
          }

          break;

        case GameFamilies.Plat:
          pbEffectsMonGroupBox.Enabled = false;
          pbEffectsTrainerGroupBox.Enabled = false;
          break;

        default:
          pbEffectsGroupBox.Enabled = false;
          pbEffectsMonGroupBox.Enabled = false;
          pbEffectsTrainerGroupBox.Enabled = false;
          conditionalMusicGroupBox.Enabled = false;
          break;
      }

      SetupBattleEffectsTables();
    }

    public void SetupBattleEffectsTables() {
      if (RomInfo.gameFamily == GameFamilies.HGSS || RomInfo.gameFamily == GameFamilies.Plat) {
        DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.trainerGraphics, DirNames.textArchives });
        RomInfo.SetBattleEffectsData();

        effectsComboTable = new List<(ushort vsGraph, ushort battleSSEQ)>();

        effectsComboMainTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.effectsComboTableOffsetToRAMAddress, 4), 0);
        ROMToolboxDialog.flag_MainComboTableRepointed = (effectsComboMainTableStartAddress >= RomInfo.synthOverlayLoadAddress);
        effectsComboMainTableStartAddress -= ROMToolboxDialog.flag_MainComboTableRepointed ? RomInfo.synthOverlayLoadAddress : DSUtils.ARM9.address;

        byte comboTableEntriesCount;

        if (RomInfo.gameFamily == GameFamilies.HGSS) {
          comboTableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.effectsComboTableOffsetToSizeLimiter);

          vsPokemonEffectsList = new List<(int pokemonID, int comboID)>();
          vsTrainerEffectsList = new List<(int trainerClass, int comboID)>();

          vsPokemonTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.vsPokemonEntryTableOffsetToRAMAddress, 4), 0);
          ROMToolboxDialog.flag_PokemonBattleTableRepointed = (vsPokemonTableStartAddress >= RomInfo.synthOverlayLoadAddress);
          vsPokemonTableStartAddress -= ROMToolboxDialog.flag_PokemonBattleTableRepointed ? RomInfo.synthOverlayLoadAddress : DSUtils.ARM9.address;

          vsTrainerTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.vsTrainerEntryTableOffsetToRAMAddress, 4), 0);
          ROMToolboxDialog.flag_TrainerClassBattleTableRepointed = (vsTrainerTableStartAddress >= RomInfo.synthOverlayLoadAddress);
          vsTrainerTableStartAddress -= ROMToolboxDialog.flag_TrainerClassBattleTableRepointed ? RomInfo.synthOverlayLoadAddress : DSUtils.ARM9.address;

          pbEffectsPokemonCombobox.Items.Clear();
          pokeNames = RomInfo.GetPokemonNames();
          for (int i = 0; i < pokeNames.Length; i++) {
            pbEffectsPokemonCombobox.Items.Add("[" + i + "]" + " " + pokeNames[i]);
          }

          RepopulateTableEditorTrainerClasses();

          pbEffectsVsTrainerListbox.Items.Clear();
          pbEffectsVsPokemonListbox.Items.Clear();
        }
        else {
          comboTableEntriesCount = 35;
        }

        pbEffectsCombosListbox.Items.Clear();

        String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");

        if (RomInfo.gameFamily == GameFamilies.HGSS) {
          using (DSUtils.EasyReader ar = new DSUtils.EasyReader(ROMToolboxDialog.flag_TrainerClassBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsTrainerTableStartAddress)) {
            byte trainerTableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.vsTrainerEntryTableOffsetToSizeLimiter);

            for (int i = 0; i < trainerTableEntriesCount; i++) {
              ushort entry = ar.ReadUInt16();
              int classID = entry&1023;
              int comboID = entry >> 10;
              vsTrainerEffectsList.Add((classID, comboID));
              pbEffectsVsTrainerListbox.Items.Add(pbEffectsTrainerCombobox.Items[classID] + " uses Combo #" + comboID);
            }
          }

          using (DSUtils.EasyReader ar = new DSUtils.EasyReader(ROMToolboxDialog.flag_PokemonBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsPokemonTableStartAddress)) {
            byte pokemonTableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.vsPokemonEntryTableOffsetToSizeLimiter);

            for (int i = 0; i < pokemonTableEntriesCount; i++) {
              ushort entry = ar.ReadUInt16();
              int pokeID = entry&1023;
              int comboID = entry >> 10;
              vsPokemonEffectsList.Add((pokeID, comboID));

              string pokeName;
              try {
                pokeName = pokeNames[pokeID];
              }
              catch (IndexOutOfRangeException) {
                pokeName = "UNKNOWN";
              }

              pbEffectsVsPokemonListbox.Items.Add("[" + pokeID.ToString("D3") + "]" + " " + pokeName + " uses Combo #" + comboID);
            }
          }
        }

        using (DSUtils.EasyReader ar = new DSUtils.EasyReader(ROMToolboxDialog.flag_MainComboTableRepointed ? expArmPath : RomInfo.arm9Path, effectsComboMainTableStartAddress)) {
          for (int i = 0; i < comboTableEntriesCount; i++) {
            ushort battleIntroEffect = ar.ReadUInt16();
            ushort battleMusic = ar.ReadUInt16();
            effectsComboTable.Add((battleIntroEffect, battleMusic));
            pbEffectsCombosListbox.Items.Add("Combo " + i.ToString("D2") + " - " + "Effect #" + battleIntroEffect + ", " + "Music #" + battleMusic);
          }
        }

        if (RomInfo.gameFamily == GameFamilies.HGSS) {
          var items = pbEffectsCombosListbox.Items.Cast<Object>().ToArray();

          pbEffectsPokemonChooseMainCombobox.Items.Clear();
          pbEffectsPokemonChooseMainCombobox.Items.AddRange(items);
          pbEffectsTrainerChooseMainCombobox.Items.Clear();
          pbEffectsTrainerChooseMainCombobox.Items.AddRange(items);

          if (pbEffectsVsTrainerListbox.Items.Count > 0) {
            pbEffectsVsTrainerListbox.SelectedIndex = 0;
          }

          if (pbEffectsVsPokemonListbox.Items.Count > 0) {
            pbEffectsVsPokemonListbox.SelectedIndex = 0;
          }
        }

        if (pbEffectsCombosListbox.Items.Count > 0) {
          pbEffectsCombosListbox.SelectedIndex = 0;
        }
      }
      else {
        pbEffectsGroupBox.Enabled = false;
      }
    }

    private void RepopulateTableEditorTrainerClasses() {
      pbEffectsTrainerCombobox.Items.Clear();
      trcNames = RomInfo.GetTrainerClassNames();
      for (int i = 0; i < trcNames.Length; i++) {
        pbEffectsTrainerCombobox.Items.Add("[" + i.ToString("D3") + "]" + " " + trcNames[i]);
      }
    }

    private void HOWconditionalMusicTableButton_Click(object sender, EventArgs e) {
      MessageBox.Show("For each Location in the list, override Header's music with chosen Music ID, if Flag is set.", "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void saveConditionalMusicTableBTN_Click(object sender, EventArgs e) {
      for (int i = 0; i < conditionalMusicTable.Count; i++) {
        DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(conditionalMusicTable[i].header), (uint)(conditionalMusicTableStartAddress + 6 * i));
        DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(conditionalMusicTable[i].flag), (uint)(conditionalMusicTableStartAddress + 6 * i + 2));
        DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(conditionalMusicTable[i].music), (uint)(conditionalMusicTableStartAddress + 6 * i + 4));
      }
    }

    private void musicIDconditionalMusicUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      (ushort header, ushort flag, ushort music) oldTuple = conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex];
      conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex] = (oldTuple.header, oldTuple.flag, (ushort)musicIDconditionalMusicUpDown.Value);
    }

    private void flagConditionalMusicUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      (ushort header, ushort flag, ushort music) oldTuple = conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex];
      conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex] = (oldTuple.header, (ushort)flagConditionalMusicUpDown.Value, oldTuple.music);
    }

    private void headerConditionalMusicComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      (ushort header, ushort flag, ushort music) oldTuple = conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex];
      (ushort header, ushort flag, ushort music) newTuple = ((ushort)headerConditionalMusicComboBox.SelectedIndex, oldTuple.flag, oldTuple.music);
      conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex] = newTuple;

      MapHeader selected = MapHeader.LoadFromARM9(newTuple.header);
      switch (RomInfo.gameFamily) {
        case GameFamilies.DP:
          locationNameConditionalMusicLBL.Text = RomInfo.GetLocationNames()[(selected as HeaderDP).locationName];
          break;
        case GameFamilies.Plat:
          locationNameConditionalMusicLBL.Text = RomInfo.GetLocationNames()[(selected as HeaderPt).locationName];
          break;
        case GameFamilies.HGSS:
          locationNameConditionalMusicLBL.Text = RomInfo.GetLocationNames()[(selected as HeaderHGSS).locationName];
          break;
      }
    }

    private void conditionalMusicTableListBox_SelectedIndexChanged(object sender, EventArgs e) {
      int selection = conditionalMusicTableListBox.SelectedIndex;
      headerConditionalMusicComboBox.SelectedIndex = conditionalMusicTable[selection].header;

      Helpers.DisableHandlers();

      flagConditionalMusicUpDown.Value = conditionalMusicTable[selection].flag;
      musicIDconditionalMusicUpDown.Value = conditionalMusicTable[selection].music;

      Helpers.EnableHandlers();
    }

    private void HOWpbEffectsTableButton_Click(object sender, EventArgs e) {
      MessageBox.Show("An entry of this table is a combination of VS. Graphics + Battle Theme.\n\n" +
                      (RomInfo.gameFamily.Equals(GameFamilies.HGSS) ? "Each entry can be \"inherited\" by one or more Pokémon or Trainer classes." : ""),
        "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void saveEffectComboBTN_Click(object sender, EventArgs e) {
      int index = pbEffectsCombosListbox.SelectedIndex;
      ushort battleIntroEffect = (ushort)pbEffectsVSAnimationUpDown.Value;
      ushort battleMusic = (ushort)pbEffectsBattleSSEQUpDown.Value;

      effectsComboTable[index] = (battleIntroEffect, battleMusic);

      String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");
      using (DSUtils.EasyWriter wr = new DSUtils.EasyWriter(ROMToolboxDialog.flag_MainComboTableRepointed ? expArmPath : RomInfo.arm9Path, effectsComboMainTableStartAddress + 4 * index)) {
        wr.Write(battleIntroEffect);
        wr.Write(battleMusic);
      }

      Helpers.DisableHandlers();
      pbEffectsCombosListbox.Items[index] = pbEffectsTrainerChooseMainCombobox.Items[index] = pbEffectsPokemonChooseMainCombobox.Items[index] = "Combo " + index.ToString("D2") + " - " + "Effect #" + battleIntroEffect + ", " + "Music #" + battleMusic;
      Helpers.EnableHandlers();
    }

    private void pbEffectsCombosListbox_SelectedIndexChanged(object sender, EventArgs e) {
      int comboSelection = pbEffectsCombosListbox.SelectedIndex;

      if (Helpers.HandlersDisabled || comboSelection < 0) {
        return;
      }

      (ushort vsGraph, ushort battleSSEQ) entry = effectsComboTable[comboSelection];
      pbEffectsBattleSSEQUpDown.Value = entry.battleSSEQ;
      pbEffectsVSAnimationUpDown.Value = entry.vsGraph;
    }

    private void TBLEditortrainerClassPreviewPic_ValueChanged(object sender, EventArgs e) {
      EditorPanels.trainerEditor.UpdateTrainerClassPic(tbEditorTrClassPictureBox, (int)((NumericUpDown)sender).Value);
    }

    private void HOWVsTrainerButton_Click(object sender, EventArgs e) {
      MessageBox.Show("Each entry of this table links a Trainer Class to an Effect Combo from the Combos Table.\n\n" +
                      "Every Trainer Class with a given combo will start the same VS. Sequence and Battle Theme.", "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void saveVSTrainerEntryBTN_Click(object sender, EventArgs e) {
      int index = pbEffectsVsTrainerListbox.SelectedIndex;
      ushort trainerClass = (ushort)pbEffectsTrainerCombobox.SelectedIndex;
      ushort comboID = (ushort)pbEffectsTrainerChooseMainCombobox.SelectedIndex;

      vsTrainerEffectsList[index] = (trainerClass, comboID);
      String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");
      using (DSUtils.EasyWriter wr = new DSUtils.EasyWriter(ROMToolboxDialog.flag_TrainerClassBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsTrainerTableStartAddress + 2 * index)) {
        wr.Write((ushort)((trainerClass&1023) + (comboID << 10)));
      }

      Helpers.DisableHandlers();
      pbEffectsVsTrainerListbox.Items[index] = "[" + trainerClass.ToString("D3") + "]" + " " + trcNames[trainerClass] + " uses Combo #" + comboID;
      Helpers.EnableHandlers();
    }

    private void pbEffectsTrainerCombobox_SelectedIndexChanged(object sender, EventArgs e) {
      int maxFrames = EditorPanels.trainerEditor.LoadTrainerClassPic((sender as ComboBox).SelectedIndex);
      EditorPanels.trainerEditor.UpdateTrainerClassPic(tbEditorTrClassPictureBox);

      tbEditorTrClassFramePreviewUpDown.Maximum = maxFrames;
      tbEditortrainerClassFrameMaxLabel.Text = "/" + maxFrames;
    }

    private void pbEffectsVsTrainerListbox_SelectedIndexChanged(object sender, EventArgs e) {
      int trainerSelection = pbEffectsVsTrainerListbox.SelectedIndex;
      if (Helpers.HandlersDisabled || trainerSelection < 0) {
        return;
      }

      (int trainerClass, int comboID) entry = vsTrainerEffectsList[trainerSelection];
      pbEffectsTrainerCombobox.SelectedIndex = entry.trainerClass;
      pbEffectsCombosListbox.SelectedIndex = pbEffectsTrainerChooseMainCombobox.SelectedIndex = entry.comboID;

      tbEditorTrClassFramePreviewUpDown.Value = 0;
    }

    private void HOWvsPokemonButton_Click(object sender, EventArgs e) {
      MessageBox.Show("Each entry of this table links a \"Wild\" Pokémon to an Effect Combo from the Combos Table.\n\n" +
                      "Whenever that Pokémon is encountered in the tall grass or via script command, its VS. Sequence and Battle Theme will be automatically triggered.",
        "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void saveVSPokemonEntryBTN_Click(object sender, EventArgs e) {
      int index = pbEffectsVsPokemonListbox.SelectedIndex;
      ushort pokemonID = (ushort)pbEffectsPokemonCombobox.SelectedIndex;
      ushort comboID = (ushort)pbEffectsPokemonChooseMainCombobox.SelectedIndex;

      vsPokemonEffectsList[index] = (pokemonID, comboID);

      String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");
      using (DSUtils.EasyWriter wr = new DSUtils.EasyWriter(ROMToolboxDialog.flag_PokemonBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsPokemonTableStartAddress + 2 * index)) {
        wr.Write((ushort)((pokemonID&1023) + (comboID << 10))); //PokemonID
      }

      Helpers.DisableHandlers();
      pbEffectsVsPokemonListbox.Items[index] = "[" + pokemonID.ToString("D3") + "]" + " " + pokeNames[pokemonID] + " uses Combo #" + comboID;
      Helpers.EnableHandlers();
    }

    private void pbEffectsPokemonCombobox_SelectedIndexChanged(object sender, EventArgs e) {
      ComboBox cb = sender as ComboBox;
      tbEditorPokeminiPictureBox.Image = Helpers.GetPokePic(cb.SelectedIndex, tbEditorPokeminiPictureBox.Width, tbEditorPokeminiPictureBox.Height, tableEditorMonIconPal, tableEditorMonIconTile, tableEditorMonIconSprite);
      tbEditorPokeminiPictureBox.Update();
    }

    private void pbEffectsVsPokemonListbox_SelectedIndexChanged(object sender, EventArgs e) {
      int pokemonSelection = pbEffectsVsPokemonListbox.SelectedIndex;

      if (Helpers.HandlersDisabled || pokemonSelection < 0) {
        return;
      }

      (int pokemonID, int comboID) entry = vsPokemonEffectsList[pokemonSelection];

      try {
        pbEffectsPokemonCombobox.SelectedIndex = entry.pokemonID;
      }
      catch (ArgumentOutOfRangeException) {
        pbEffectsPokemonCombobox.SelectedIndex = 0;
      }

      pbEffectsCombosListbox.SelectedIndex = pbEffectsPokemonChooseMainCombobox.SelectedIndex = entry.comboID;
    }
  }
}

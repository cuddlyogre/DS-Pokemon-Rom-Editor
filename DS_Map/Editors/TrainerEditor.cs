using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using Images;
using Ekona.Images;

namespace DSPRE.Editors {
  public partial class TrainerEditor : UserControl {
    public bool trainerEditorIsReady { get; set; } = false;
    private List<ComboBox> partyPokemonComboboxList = new List<ComboBox>();
    private List<ComboBox> partyItemsComboboxList = new List<ComboBox>();
    private List<GroupBox> partyMovesGroupboxList = new List<GroupBox>();
    private List<NumericUpDown> partyLevelUpdownList = new List<NumericUpDown>();
    private List<NumericUpDown> partyIVUpdownList = new List<NumericUpDown>();
    private List<NumericUpDown> partyBallUpdownList = new List<NumericUpDown>();
    private List<GroupBox> partyGroupboxList = new List<GroupBox>();
    private List<PictureBox> partyPokemonPictureBoxList = new List<PictureBox>();
    private List<PictureBox> partyPokemonItemIconList = new List<PictureBox>();
    TrainerFile currentTrainerFile;
    PaletteBase trainerPal;
    ImageBase trainerTile;
    SpriteBase trainerSprite;

    private readonly PaletteBase[] monIconPals = new PaletteBase[6];
    private readonly ImageBase[] monIconTiles = new ImageBase[6];
    private readonly SpriteBase[] monIconSprites = new SpriteBase[6];

    public TrainerEditor() {
      InitializeComponent();
    }

    public void SetupTrainerEditor(bool force = false) {
      if (trainerEditorIsReady && !force) return;
      trainerEditorIsReady = true;

      Helpers.DisableHandlers();
      SetupTrainerClassEncounterMusicTable();
      /* Extract essential NARCs sub-archives*/
      Helpers.statusLabelMessage("Setting up Trainer Editor...");
      Update();

      DSUtils.TryUnpackNarcs(new List<DirNames> {
        DirNames.trainerProperties,
        DirNames.trainerParty,
        DirNames.trainerGraphics,
        DirNames.textArchives,
        DirNames.monIcons
      });

      partyPokemonComboboxList.Clear();
      partyPokemonComboboxList.Add(partyPokemon1ComboBox);
      partyPokemonComboboxList.Add(partyPokemon2ComboBox);
      partyPokemonComboboxList.Add(partyPokemon3ComboBox);
      partyPokemonComboboxList.Add(partyPokemon4ComboBox);
      partyPokemonComboboxList.Add(partyPokemon5ComboBox);
      partyPokemonComboboxList.Add(partyPokemon6ComboBox);

      partyItemsComboboxList.Clear();
      partyItemsComboboxList.Add(partyItem1ComboBox);
      partyItemsComboboxList.Add(partyItem2ComboBox);
      partyItemsComboboxList.Add(partyItem3ComboBox);
      partyItemsComboboxList.Add(partyItem4ComboBox);
      partyItemsComboboxList.Add(partyItem5ComboBox);
      partyItemsComboboxList.Add(partyItem6ComboBox);

      partyLevelUpdownList.Clear();
      partyLevelUpdownList.Add(partyLevel1UpDown);
      partyLevelUpdownList.Add(partyLevel2UpDown);
      partyLevelUpdownList.Add(partyLevel3UpDown);
      partyLevelUpdownList.Add(partyLevel4UpDown);
      partyLevelUpdownList.Add(partyLevel5UpDown);
      partyLevelUpdownList.Add(partyLevel6UpDown);

      partyIVUpdownList.Clear();
      partyIVUpdownList.Add(partyIV1UpDown);
      partyIVUpdownList.Add(partyIV2UpDown);
      partyIVUpdownList.Add(partyIV3UpDown);
      partyIVUpdownList.Add(partyIV4UpDown);
      partyIVUpdownList.Add(partyIV5UpDown);
      partyIVUpdownList.Add(partyIV6UpDown);

      partyBallUpdownList.Clear();
      partyBallUpdownList.Add(partyBall1UpDown);
      partyBallUpdownList.Add(partyBall2UpDown);
      partyBallUpdownList.Add(partyBall3UpDown);
      partyBallUpdownList.Add(partyBall4UpDown);
      partyBallUpdownList.Add(partyBall5UpDown);
      partyBallUpdownList.Add(partyBall6UpDown);

      partyMovesGroupboxList.Clear();
      partyMovesGroupboxList.Add(poke1MovesGroupBox);
      partyMovesGroupboxList.Add(poke2MovesGroupBox);
      partyMovesGroupboxList.Add(poke3MovesGroupBox);
      partyMovesGroupboxList.Add(poke4MovesGroupBox);
      partyMovesGroupboxList.Add(poke5MovesGroupBox);
      partyMovesGroupboxList.Add(poke6MovesGroupBox);

      partyGroupboxList.Clear();
      partyGroupboxList.Add(party1GroupBox);
      partyGroupboxList.Add(party2GroupBox);
      partyGroupboxList.Add(party3GroupBox);
      partyGroupboxList.Add(party4GroupBox);
      partyGroupboxList.Add(party5GroupBox);
      partyGroupboxList.Add(party6GroupBox);

      partyPokemonPictureBoxList.Clear();
      partyPokemonPictureBoxList.Add(partyPokemon1PictureBox);
      partyPokemonPictureBoxList.Add(partyPokemon2PictureBox);
      partyPokemonPictureBoxList.Add(partyPokemon3PictureBox);
      partyPokemonPictureBoxList.Add(partyPokemon4PictureBox);
      partyPokemonPictureBoxList.Add(partyPokemon5PictureBox);
      partyPokemonPictureBoxList.Add(partyPokemon6PictureBox);

      partyPokemonItemIconList.Clear();
      partyPokemonItemIconList.Add(partyPokemonItemPictureBox1);
      partyPokemonItemIconList.Add(partyPokemonItemPictureBox2);
      partyPokemonItemIconList.Add(partyPokemonItemPictureBox3);
      partyPokemonItemIconList.Add(partyPokemonItemPictureBox4);
      partyPokemonItemIconList.Add(partyPokemonItemPictureBox5);
      partyPokemonItemIconList.Add(partyPokemonItemPictureBox6);

      int trainerCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir).Length;
      trainerComboBox.Items.Clear();
      trainerComboBox.Items.AddRange(Helpers.GetTrainerNames());

      string[] classNames = RomInfo.GetTrainerClassNames();
      trainerClassListBox.Items.Clear();
      if (classNames.Length > byte.MaxValue + 1) {
        MessageBox.Show("There can't be more than 256 trainer classes! [Found " + classNames.Length + "].\nAborting.",
          "Too many trainer classes", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      for (int i = 0; i < classNames.Length; i++) {
        trainerClassListBox.Items.Add("[" + i.ToString("D3") + "]" + " " + classNames[i]);
      }

      string[] itemNames = RomInfo.GetItemNames();
      string[] pokeNames = RomInfo.GetPokemonNames();
      string[] moveNames = RomInfo.GetAttackNames();

      foreach (Control c in trainerItemsGroupBox.Controls) {
        if (c is ComboBox) {
          (c as ComboBox).DataSource = new BindingSource(itemNames, string.Empty);
        }
      }

      foreach (ComboBox CB in partyPokemonComboboxList) {
        CB.DataSource = new BindingSource(pokeNames, string.Empty);
      }

      foreach (ComboBox CB in partyItemsComboboxList) {
        CB.DataSource = new BindingSource(itemNames, string.Empty);
      }

      foreach (GroupBox movesGroup in partyMovesGroupboxList) {
        foreach (Control c in movesGroup.Controls) {
          if (c is ComboBox) {
            (c as ComboBox).DataSource = new BindingSource(moveNames, string.Empty);
          }
        }
      }

      trainerComboBox.SelectedIndex = 0;

      Helpers.EnableHandlers();
      trainerComboBox_SelectedIndexChanged(null, null);
      Helpers.statusLabelMessage();
    }

    Dictionary<byte, (uint entryOffset, ushort musicD, ushort? musicN)> trainerClassEncounterMusicDict;

    private void SetupTrainerClassEncounterMusicTable() {
      RomInfo.SetEncounterMusicTableOffsetToRAMAddress();
      trainerClassEncounterMusicDict = new Dictionary<byte, (uint entryOffset, ushort musicD, ushort? musicN)>();

      uint encounterMusicTableTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.encounterMusicTableOffsetToRAMAddress, 4), 0) - 0x02000000;

      uint entrySize = 4;
      uint tableSizeOffset = 10;
      if (gameFamily == GameFamilies.HGSS) {
        entrySize += 2;
        tableSizeOffset += 2;
        encounterSSEQAltUpDown.Enabled = true;
      }

      byte tableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.encounterMusicTableOffsetToRAMAddress - tableSizeOffset);
      using (DSUtils.ARM9.Reader ar = new DSUtils.ARM9.Reader(encounterMusicTableTableStartAddress)) {
        for (int i = 0; i < tableEntriesCount; i++) {
          uint entryOffset = (uint)ar.BaseStream.Position;
          byte tclass = (byte)ar.ReadUInt16();
          ushort musicD = ar.ReadUInt16();
          ushort? musicN = gameFamily == GameFamilies.HGSS ? ar.ReadUInt16() : (ushort?)null;
          trainerClassEncounterMusicDict[tclass] = (entryOffset, musicD, musicN);
        }
      }
    }

    private void trainerSaveCurrentButton_Click(object sender, EventArgs e) {
      currentTrainerFile.trp.partyCount = (byte)partyCountUpDown.Value;
      currentTrainerFile.trp.hasMoves = trainerMovesCheckBox.Checked;
      currentTrainerFile.trp.hasItems = trainerItemsCheckBox.Checked;
      currentTrainerFile.trp.doubleBattle = trainerDoubleCheckBox.Checked;

      IList trainerItems = trainerItemsGroupBox.Controls;
      for (int i = 0; i < trainerItems.Count; i++) {
        currentTrainerFile.trp.trainerItems[i] = (ushort)(trainerItems[i] as ComboBox).SelectedIndex;
      }

      IList trainerAI = TrainerAIGroupBox.Controls;
      for (int i = 0; i < trainerAI.Count; i++) {
        currentTrainerFile.trp.AI[i] = (trainerAI[i] as CheckBox).Checked;
      }

      for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
        currentTrainerFile.party[i].moves = trainerMovesCheckBox.Checked ? new ushort[4] : null;
      }

      for (int i = 0; i < partyCountUpDown.Value; i++) {
        currentTrainerFile.party[i].pokeID = (ushort)partyPokemonComboboxList[i].SelectedIndex;
        currentTrainerFile.party[i].level = (ushort)partyLevelUpdownList[i].Value;

        if (trainerMovesCheckBox.Checked) {
          IList movesList = partyMovesGroupboxList[i].Controls;
          for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
            currentTrainerFile.party[i].moves[j] = (ushort)(movesList[j] as ComboBox).SelectedIndex;
          }
        }

        if (trainerItemsCheckBox.Checked) {
          currentTrainerFile.party[i].heldItem = (ushort)partyItemsComboboxList[i].SelectedIndex;
        }

        currentTrainerFile.party[i].unknown1_DATASTART = (ushort)partyIVUpdownList[i].Value;
        currentTrainerFile.party[i].unknown2_DATAEND = (ushort)partyBallUpdownList[i].Value;
      }

      /*Write to File*/
      string indexStr = "\\" + trainerComboBox.SelectedIndex.ToString("D4");
      File.WriteAllBytes(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + indexStr, currentTrainerFile.trp.ToByteArray());
      File.WriteAllBytes(RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + indexStr, currentTrainerFile.party.ToByteArray());

      UpdateCurrentTrainerName(newName: trainerNameTextBox.Text);
      UpdateCurrentTrainerShownName();

      MessageBox.Show("Trainer saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private string GetTrainerClassNameFromListbox(object selectedItem) {
      string lbname = selectedItem.ToString();
      return lbname.Substring(lbname.IndexOf(" ") + 1);
    }

    private void UpdateCurrentTrainerShownName() {
      string trClass = GetTrainerClassNameFromListbox(trainerClassListBox.SelectedItem);

      string editedTrainer = "[" + currentTrainerFile.trp.trainerID.ToString("D2") + "] " + trClass + " " + currentTrainerFile.name;

      Helpers.DisableHandlers();
      trainerComboBox.Items[trainerComboBox.SelectedIndex] = editedTrainer;
      Helpers.EnableHandlers();

      if (EditorPanels.eventEditor.eventEditorIsReady) {
        EditorPanels.eventEditor.owTrainerComboBox.Items[trainerComboBox.SelectedIndex] = editedTrainer;
      }
    }

    private void UpdateCurrentTrainerName(string newName) {
      currentTrainerFile.name = newName;
      TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);
      if (currentTrainerFile.trp.trainerID < trainerNames.messages.Count) {
        trainerNames.messages[currentTrainerFile.trp.trainerID] = newName;
      }
      else {
        trainerNames.messages.Add(newName);
      }

      trainerNames.SaveToFileDefaultDir(RomInfo.trainerNamesMessageNumber, showSuccessMessage: false);
    }

    private void trainerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      Helpers.DisableHandlers();

      int currentIndex = trainerComboBox.SelectedIndex;
      string suffix = "\\" + currentIndex.ToString("D4");
      string[] trNames = RomInfo.GetSimpleTrainerNames();

      bool error = currentIndex >= trNames.Length;

      currentTrainerFile = new TrainerFile(
        new TrainerProperties(
          (ushort)trainerComboBox.SelectedIndex,
          new FileStream(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + suffix, FileMode.Open)
        ),
        new FileStream(RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + suffix, FileMode.Open),
        error ? TrainerFile.NAME_NOT_FOUND : trNames[currentIndex]
      );

      RefreshTrainerPartyGUI();
      RefreshTrainerPropertiesGUI();

      Helpers.EnableHandlers();

      if (error) {
        MessageBox.Show("This Trainer File doesn't have a corresponding name.\n\n" +
                        "If you edited this ROM's Trainers with another tool before, don't worry.\n" +
                        "DSPRE will attempt to add the missing line to the Trainer Names Text Archive [" + RomInfo.trainerNamesMessageNumber + "] upon resaving.",
          "Trainer name not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    public void RefreshTrainerPartyGUI() {
      for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
        partyPokemonComboboxList[i].SelectedIndex = currentTrainerFile.party[i].pokeID ?? 0;
        partyItemsComboboxList[i].SelectedIndex = currentTrainerFile.party[i].heldItem ?? 0;
        partyLevelUpdownList[i].Value = Math.Max((ushort)1, currentTrainerFile.party[i].level);
        partyIVUpdownList[i].Value = currentTrainerFile.party[i].unknown1_DATASTART;
        partyBallUpdownList[i].Value = currentTrainerFile.party[i].unknown2_DATAEND;

        if (currentTrainerFile.party[i].moves == null) {
          for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
            (partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex = 0;
          }
        }
        else {
          for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
            (partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex = currentTrainerFile.party[i].moves[j];
          }
        }
      }
    }

    public void RefreshTrainerPropertiesGUI() {
      trainerNameTextBox.Text = currentTrainerFile.name;

      trainerClassListBox.SelectedIndex = currentTrainerFile.trp.trainerClass;
      trainerDoubleCheckBox.Checked = currentTrainerFile.trp.doubleBattle;
      trainerMovesCheckBox.Checked = currentTrainerFile.trp.hasMoves;
      trainerItemsCheckBox.Checked = currentTrainerFile.trp.hasItems;
      partyCountUpDown.Value = currentTrainerFile.trp.partyCount;

      IList trainerItems = trainerItemsGroupBox.Controls;
      for (int i = 0; i < trainerItems.Count; i++) {
        (trainerItems[i] as ComboBox).SelectedIndex = currentTrainerFile.trp.trainerItems[i];
      }

      IList trainerAI = TrainerAIGroupBox.Controls;
      for (int i = 0; i < trainerAI.Count; i++) {
        (trainerAI[i] as CheckBox).Checked = currentTrainerFile.trp.AI[i];
      }
    }

    private void trainerItemsCheckBox_CheckedChanged(object sender, EventArgs e) {
      for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
        partyItemsComboboxList[i].Enabled = trainerItemsCheckBox.Checked;
      }
    }

    private void trainerMovesCheckBox_CheckedChanged(object sender, EventArgs e) {
      for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
        for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
          (partyMovesGroupboxList[i].Controls[j] as ComboBox).Enabled = trainerMovesCheckBox.Checked;
        }

        currentTrainerFile.party[i].moves = trainerMovesCheckBox.Checked ? new ushort[Party.MOVES_PER_POKE] : null;
      }
    }

    private void partyCountUpDown_ValueChanged(object sender, EventArgs e) {
      for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
        partyGroupboxList[i].Enabled = (partyCountUpDown.Value > i);
        partyPokemonPictureBoxList[i].Visible = partyGroupboxList[i].Enabled;
      }

      for (int i = Math.Min(currentTrainerFile.trp.partyCount, (int)partyCountUpDown.Value); i < TrainerFile.POKE_IN_PARTY; i++) {
        currentTrainerFile.party[i] = new PartyPokemon(currentTrainerFile.trp.hasItems, currentTrainerFile.trp.hasMoves);
      }

      //if (Helpers.HandlersEnabled) {
      //    RefreshTrainerPartyGUI();
      //    RefreshTrainerPropertiesGUI();
      //}
    }

    public int LoadTrainerClassPic(int trClassID) {
      int paletteFileID = (trClassID * 5 + 1);
      string paletteFilename = paletteFileID.ToString("D4");
      trainerPal = new NCLR(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + paletteFilename, paletteFileID, paletteFilename);

      int tilesFileID = trClassID * 5;
      string tilesFilename = tilesFileID.ToString("D4");
      trainerTile = new NCGR(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + tilesFilename, tilesFileID, tilesFilename);

      if (gameFamily == GameFamilies.DP) {
        return 0;
      }

      int spriteFileID = (trClassID * 5 + 2);
      string spriteFilename = spriteFileID.ToString("D4");
      trainerSprite = new NCER(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + spriteFilename, spriteFileID, spriteFilename);

      return trainerSprite.Banks.Length - 1;
    }

    public void UpdateTrainerClassPic(PictureBox pb, int frameNumber = 0) {
      if (trainerSprite == null) {
        Console.WriteLine("Sprite is null!");
        return;
      }

      int bank0OAMcount = trainerSprite.Banks[0].oams.Length;
      int[] OAMenabled = new int[bank0OAMcount];
      for (int i = 0; i < OAMenabled.Length; i++) {
        OAMenabled[i] = i;
      }

      frameNumber = Math.Min(trainerSprite.Banks.Length, frameNumber);
      Image trSprite = trainerSprite.Get_Image(trainerTile, trainerPal, frameNumber, trainerClassPicBox.Width, trainerClassPicBox.Height, false, false, false, true, true, -1, OAMenabled);
      pb.Image = trSprite;
      pb.Update();
    }

    private void trainerClassListBox_SelectedIndexChanged(object sender, EventArgs e) {
      int selection = trainerClassListBox.SelectedIndex;
      if (selection < 0) {
        return;
      }

      try {
        int maxFrames = LoadTrainerClassPic(selection);
        UpdateTrainerClassPic(trainerClassPicBox);

        trClassFramePreviewUpDown.Maximum = maxFrames;
        trainerClassFrameMaxLabel.Text = "/" + maxFrames;
      }
      catch {
        trClassFramePreviewUpDown.Maximum = 0;
      }

      trainerClassNameTextbox.Text = GetTrainerClassNameFromListbox(trainerClassListBox.SelectedItem);

      if (trainerClassEncounterMusicDict.TryGetValue((byte)selection, out (uint entryOffset, ushort musicD, ushort? musicN) output)) {
        encounterSSEQMainUpDown.Enabled = eyeContactMusicLabel.Enabled = true;
        encounterSSEQMainUpDown.Value = output.musicD;
      }
      else {
        encounterSSEQMainUpDown.Enabled = eyeContactMusicLabel.Enabled = false;
        encounterSSEQMainUpDown.Value = 0;
      }

      eyeContactMusicAltLabel.Enabled = encounterSSEQAltUpDown.Enabled = (encounterSSEQMainUpDown.Enabled && gameFamily == GameFamilies.HGSS);
      encounterSSEQAltUpDown.Value = output.musicN != null ? (ushort)output.musicN : 0;
      currentTrainerFile.trp.trainerClass = (byte)selection;
    }

    private void UpdateCurrentTrainerClassName(string newName) {
      TextArchive trainerClassNames = new TextArchive(RomInfo.trainerClassMessageNumber);
      trainerClassNames.messages[trainerClassListBox.SelectedIndex] = newName;
      trainerClassNames.SaveToFileDefaultDir(RomInfo.trainerClassMessageNumber, showSuccessMessage: false);
    }

    private void trClassFramePreviewUpDown_ValueChanged(object sender, EventArgs e) {
      UpdateTrainerClassPic(trainerClassPicBox, (int)((NumericUpDown)sender).Value);
    }

    private void saveTrainerClassButton_Click(object sender, EventArgs e) {
      Helpers.DisableHandlers();

      int selectedTrClass = trainerClassListBox.SelectedIndex;

      byte b_selectedTrClass = (byte)selectedTrClass;
      ushort eyeMusicID = (ushort)encounterSSEQMainUpDown.Value;

      if (trainerClassEncounterMusicDict.TryGetValue(b_selectedTrClass, out var dictEntry)) {
        DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(eyeMusicID), dictEntry.entryOffset);
        trainerClassEncounterMusicDict[b_selectedTrClass] = (dictEntry.entryOffset, eyeMusicID, dictEntry.musicN);
      }

      string newName = trainerClassNameTextbox.Text;
      UpdateCurrentTrainerClassName(newName);
      trainerClassListBox.Items[selectedTrClass] = "[" + selectedTrClass.ToString("D3") + "]" + " " + newName;

      if (currentTrainerFile.trp.trainerClass == trainerClassListBox.SelectedIndex) {
        UpdateCurrentTrainerShownName();
      }

      Helpers.EnableHandlers();

      //trainerClassListBox_SelectedIndexChanged(null, null);
      if (gameFamily.Equals(GameFamilies.HGSS) && EditorPanels.tableEditor.tableEditorIsReady) {
        EditorPanels.tableEditor.pbEffectsTrainerCombobox.Items[selectedTrClass] = trainerClassListBox.Items[selectedTrClass];
        for (int i = 0; i < EditorPanels.tableEditor.vsTrainerEffectsList.Count; i++) {
          if (EditorPanels.tableEditor.vsTrainerEffectsList[i].trainerClass == selectedTrClass) {
            EditorPanels.tableEditor.pbEffectsVsTrainerListbox.Items[i] = EditorPanels.tableEditor.pbEffectsTrainerCombobox.Items[selectedTrClass] + " uses Combo #" + EditorPanels.tableEditor.vsTrainerEffectsList[i].comboID;
          }
        }
      }

      MessageBox.Show("Trainer Class settings saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void partyMoveComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersEnabled) {
        for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
          ushort[] moves = currentTrainerFile.party[i].moves;

          if (moves != null) {
            for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
              moves[j] = (ushort)(partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex;
            }
          }
        }
      }
    }

    private void showTrainerEditorItemPic(byte partyPos) {
      ComboBox cb = partyItemsComboboxList[partyPos];
      partyPokemonItemIconList[partyPos].Visible = cb.SelectedIndex > 0;
    }

    private void ShowPartyPokemonPic(byte partyPos) {
      ComboBox cb = partyPokemonComboboxList[partyPos];
      int species = cb.SelectedIndex > 0 ? cb.SelectedIndex : 0;

      PictureBox pb = partyPokemonPictureBoxList[partyPos];

      partyPokemonPictureBoxList[partyPos].Image = Helpers.GetPokePic(species, pb.Width, pb.Height, monIconPals[partyPos], monIconTiles[partyPos], monIconSprites[partyPos]);
    }

    private void partyItem6ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      showTrainerEditorItemPic(5);
    }

    private void partyPokemon6ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      ShowPartyPokemonPic(5);
    }

    private void partyItem5ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      showTrainerEditorItemPic(4);
    }

    private void partyPokemon5ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      ShowPartyPokemonPic(4);
    }

    private void partyItem4ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      showTrainerEditorItemPic(3);
    }

    private void partyPokemon4ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      ShowPartyPokemonPic(3);
    }

    private void partyItem3ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      showTrainerEditorItemPic(2);
    }

    private void partyPokemon3ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      ShowPartyPokemonPic(2);
    }

    private void partyItem2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      showTrainerEditorItemPic(1);
    }

    private void partyPokemon2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      ShowPartyPokemonPic(1);
    }

    private void partyItem1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      showTrainerEditorItemPic(0);
    }

    private void partyPokemon1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      ShowPartyPokemonPic(0);
    }

    private void addTrainerButton_Click(object sender, EventArgs e) {
      /* Add new trainer file to 2 folders */
      string suffix = "\\" + trainerComboBox.Items.Count.ToString("D4");

      string trainerPropertiesPath = gameDirs[DirNames.trainerProperties].unpackedDir + suffix;
      string partyFilePath = gameDirs[DirNames.trainerParty].unpackedDir + suffix;

      File.WriteAllBytes(trainerPropertiesPath, new TrainerProperties((ushort)trainerComboBox.Items.Count).ToByteArray());
      File.WriteAllBytes(partyFilePath, new PartyPokemon().ToByteArray());

      TextArchive trainerClasses = new TextArchive(RomInfo.trainerClassMessageNumber);
      TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);

      /* Update ComboBox and select new file */
      trainerComboBox.Items.Add(trainerClasses.messages[0]);
      trainerNames.messages.Add("");
      trainerNames.SaveToFileDefaultDir(RomInfo.trainerNamesMessageNumber, showSuccessMessage: false);

      trainerComboBox.SelectedIndex = trainerComboBox.Items.Count - 1;
      UpdateCurrentTrainerShownName();
    }

    private void DVExplainButton_Click(object sender, EventArgs e) {
      MessageBox.Show("DV, or \"Difficulty Value\", is used by the game engine to calculate how tough an opponent Pokemon should be.\n" +
                      "The DV affects a Pokemon's Nature and IVs - the higher the value, the stronger the Pokemon.\n" +
                      "DVs will go from 1 (0 IVs) to 255 (31 IVs). Natures are chosen semi-randomly." +
                      "\nIVs will be the same value for all Stats at any DV, so Hidden Power will only be Fighting or Dark Type." +
                      "\n\nFor the time being, DSPRE Reloaded is unable to calculate the target DV of a Pokémon for a given Nature and set of IVs.", "Difficulty Value", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void exportTrainerButton_Click(object sender, EventArgs e) {
      currentTrainerFile.SaveToFileExplorePath("G4 Trainer File " + trainerComboBox.SelectedItem);
    }

    private void importTrainerButton_Click(object sender, EventArgs e) {
      OpenFileDialog of = new OpenFileDialog {
        Filter = "Gen IV Trainer File (*.trf)|*.trf"
      };
      if (of.ShowDialog(this) != DialogResult.OK) {
        return;
      }

      /* Update trainer on disk */
      using (DSUtils.EasyReader reader = new DSUtils.EasyReader(of.FileName)) {
        string trName = reader.ReadString();

        byte datSize = reader.ReadByte();
        byte[] trDat = reader.ReadBytes(datSize);

        byte partySize = reader.ReadByte();
        byte[] pDat = reader.ReadBytes(partySize);

        string pathData = RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + "\\" + trainerComboBox.SelectedIndex.ToString("D4");
        string pathParty = RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + "\\" + trainerComboBox.SelectedIndex.ToString("D4");
        File.WriteAllBytes(pathData, trDat);
        File.WriteAllBytes(pathParty, pDat);

        UpdateCurrentTrainerName(trName);
      }

      /* Refresh controls and re-read file */
      trainerComboBox_SelectedIndexChanged(null, null);
      UpdateCurrentTrainerShownName();

      /* Display success message */
      MessageBox.Show("Trainer File imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void exportPropertiesButton_Click(object sender, EventArgs e) {
      currentTrainerFile.trp.SaveToFileExplorePath("G4 Trainer Properties " + trainerComboBox.SelectedItem);
    }

    private void importReplacePropertiesButton_Click(object sender, EventArgs e) {
      OpenFileDialog of = new OpenFileDialog {
        Filter = "Gen IV Trainer Properties (*.trp)|*.trp"
      };
      if (of.ShowDialog(this) != DialogResult.OK) {
        return;
      }

      /* Update trp object in memory */
      currentTrainerFile.trp = new TrainerProperties((ushort)trainerComboBox.SelectedIndex, new FileStream(of.FileName, FileMode.Open));
      RefreshTrainerPropertiesGUI();

      /* Display success message */
      MessageBox.Show("Trainer Properties imported successfully!\nRemember to save the current Trainer File.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void importReplacePartyButton_Click(object sender, EventArgs e) {
      OpenFileDialog of = new OpenFileDialog {
        Filter = "Gen IV Party File (*.pdat)|*.pdat"
      };
      if (of.ShowDialog(this) != DialogResult.OK) {
        return;
      }

      /* Update trp object in memory */
      currentTrainerFile.party = new Party(readFirstByte: true, TrainerFile.POKE_IN_PARTY, new FileStream(of.FileName, FileMode.Open), currentTrainerFile.trp);
      RefreshTrainerPropertiesGUI();
      RefreshTrainerPartyGUI();

      /* Display success message */
      MessageBox.Show("Trainer Party imported successfully!\nRemember to save the current Trainer File.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void exportPartyButton_Click(object sender, EventArgs e) {
      currentTrainerFile.party.exportCondensedData = true;
      currentTrainerFile.party.SaveToFileExplorePath("G4 Party Data " + trainerComboBox.SelectedItem);
      currentTrainerFile.party.exportCondensedData = false;
    }
  }
}

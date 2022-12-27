using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Forms;
using LibNDSFormats.NSBMD;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using NSMBe4.NSBMD;

namespace DSPRE.Editors {
  public partial class EventEditor : UserControl {
    public bool eventEditorIsReady { get; set; } = false;
    private bool itemComboboxIsUpToDate = false;
    public EventFile currentEvFile;
    public GameMatrix eventMatrix;
    public Event selectedEvent;
    public const byte tileSize = 16;
    public static MapFile currentMapFile;
    public static NSBMDGlRenderer mapRenderer = new NSBMDGlRenderer();
    public static NSBMDGlRenderer buildingsRenderer = new NSBMDGlRenderer();

    public EventEditor() {
      InitializeComponent();
      openGlControl.InitializeContexts();
    }

    public void SetupEventEditor(bool force = false) {
      if (eventEditorIsReady && !force) return;
      eventEditorIsReady = true;

      /* Extract essential NARCs sub-archives*/

      Helpers.statusLabelMessage("Attempting to unpack Event Editor NARCs... Please wait. This might take a while");
      Helpers.toolStripProgressBar.Visible = true;
      Helpers.toolStripProgressBar.Maximum = 12;
      Helpers.toolStripProgressBar.Value = 0;
      Update();

      DSUtils.TryUnpackNarcs(new List<DirNames> {
        DirNames.matrices,
        DirNames.maps,
        DirNames.exteriorBuildingModels,
        DirNames.buildingConfigFiles,
        DirNames.buildingTextures,
        DirNames.mapTextures,
        DirNames.areaData,

        DirNames.eventFiles,
        DirNames.trainerProperties,
        DirNames.OWSprites,

        DirNames.scripts,
      });

      RomInfo.SetOWtable();
      RomInfo.Set3DOverworldsDict();

      if (RomInfo.gameFamily == GameFamilies.HGSS) {
        DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
      }

      Helpers.DisableHandlers();
      if (File.Exists(RomInfo.OWtablePath)) {
        switch (RomInfo.gameFamily) {
          case GameFamilies.DP:
          case GameFamilies.Plat:
            break;
          default:
            // HGSS Overlay 1 must be decompressed in order to read the overworld table
            if (DSUtils.CheckOverlayHasCompressionFlag(1)) {
              if (DSUtils.OverlayIsCompressed(1)) {
                if (DSUtils.DecompressOverlay(1) < 0) {
                  MessageBox.Show("Overlay 1 couldn't be decompressed.\nOverworld sprites in the Event Editor will be " +
                                  "displayed incorrectly or not displayed at all.", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
              }
            }

            break;
        }
      }

      /* Add event file numbers to box */
      Helpers.statusLabelMessage("Loading Events... Please wait.");
      Update();

      int eventCount = RomInfo.GetEventFileCount();
      RomInfo.ReadOWTable();

      eventEditorWarpHeaderListBox.Items.Clear();
      eventEditorWarpHeaderListBox.Items.AddRange(EditorPanels.headerEditor.headerListBoxNames.ToArray());
      eventEditorHeaderLocationNameLabel.Text = "";

      string[] trainerNames = Helpers.GetTrainerNames();
      Helpers.toolStripProgressBar.Maximum = (int)(eventCount + RomInfo.OverworldTable.Keys.Max() + trainerNames.Length);
      Helpers.toolStripProgressBar.Value = 0;
      Update();

      /* Add event list to event combobox */
      selectEventComboBox.Items.Clear();
      for (int i = 0; i < eventCount; i++) {
        selectEventComboBox.Items.Add("Event File " + i);
        Helpers.toolStripProgressBar.Value++;
      }

      /* Add sprite list to ow sprite box */
      owSpriteComboBox.Items.Clear();
      foreach (ushort key in RomInfo.OverworldTable.Keys) {
        owSpriteComboBox.Items.Add("OW Entry " + key);
        Helpers.toolStripProgressBar.Value++;
      }

      /* Add trainer list to ow trainer box */
      owTrainerComboBox.Items.Clear();
      owTrainerComboBox.Items.AddRange(trainerNames);

      /* Add item list to ow item box */
      string[] itemNames = RomInfo.GetItemNames();
      if (ROMToolboxDialog.CheckScriptsStandardizedItemNumbers()) {
        UpdateItemComboBox(itemNames);
      }
      else {
        ScriptFile itemScript = new ScriptFile(RomInfo.itemScriptFileNumber);
        owItemComboBox.Items.Clear();
        foreach (CommandContainer cont in itemScript.allScripts) {
          if (cont.commands.Count > 4) {
            continue;
          }

          owItemComboBox.Items.Add(BitConverter.ToUInt16(cont.commands[1].cmdParams[1], 0) + "x " + itemNames[BitConverter.ToUInt16(cont.commands[0].cmdParams[1], 0)]);
        }
      }

      /* Add ow movement list to box */
      owMovementComboBox.Items.Clear();
      spawnableDirComboBox.Items.Clear();
      spawnableTypeComboBox.Items.Clear();
      owMovementComboBox.Items.AddRange(PokeDatabase.EventEditor.Overworlds.movementsArray);
      spawnableDirComboBox.Items.AddRange(PokeDatabase.EventEditor.Spawnables.orientationsArray);
      spawnableTypeComboBox.Items.AddRange(PokeDatabase.EventEditor.Spawnables.typesArray);

      int defaultMatrix = 0;
      eventMatrix = new GameMatrix(defaultMatrix);

      matrixNavigator.CellMarked += (o, args) => {
        setEventMatrixUpDown(args.X, args.Y);
      };

      showSpawnablesCheckBox.Checked = Properties.Settings.Default.renderSpawnables;
      showOwsCheckBox.Checked = Properties.Settings.Default.renderOverworlds;
      showWarpsCheckBox.Checked = Properties.Settings.Default.renderWarps;
      showTriggersCheckBox.Checked = Properties.Settings.Default.renderTriggers;

      if (owOrientationComboBox.SelectedIndex < 0 && overworldsListBox.Items.Count <= 0) {
        owOrientationComboBox.SelectedIndex = 2;
      }

      if (owMovementComboBox.SelectedIndex < 0 && overworldsListBox.Items.Count <= 0) {
        owOrientationComboBox.SelectedIndex = 1;
      }

      eventMatrixUpDown.Maximum = Helpers.romInfo.GetMatrixCount() - 1;
      eventAreaDataUpDown.Maximum = Helpers.romInfo.GetAreaDataCount() - 1;

      Helpers.EnableHandlers();

      /* Draw matrix 0 in matrix navigator */
      selectEventComboBox.SelectedIndex = defaultMatrix;

      owItemComboBox.SelectedIndex = 0;
      owTrainerComboBox.SelectedIndex = 0;

      Helpers.toolStripProgressBar.Value = 0;
      Helpers.toolStripProgressBar.Visible = false;

      Helpers.statusLabelMessage();
    }

    public void makeCurrent() {
      openGlControl.MakeCurrent();
    }

    public void OpenEventEditor(int eventID, int matrixID, int areaDataID) {
      SetupEventEditor();

      selectEventComboBox.SelectedIndex = eventID; // Select event file
      if (matrixID != 0) eventAreaDataUpDown.Value = areaDataID; // Use Area Data for textures if matrix is not 0
      eventMatrixUpDown.Value = matrixID; // Open the right matrix in event editor
      EditorPanels.mainTabControl.SelectedTab = EditorPanels.eventEditorTabPage;

      eventMatrixUpDown_ValueChanged(null, null);
    }

    public void UpdateItemComboBox(string[] itemNames) {
      if (itemComboboxIsUpToDate) {
        return;
      }

      itemsSelectorHelpBtn.Visible = false;
      owItemComboBox.Size = new Size(new Point(owItemComboBox.Size.Width + 30, owItemComboBox.Size.Height));
      owItemComboBox.Items.Clear();
      owItemComboBox.Items.AddRange(itemNames);
      OWTypeChanged(null, null);
      itemComboboxIsUpToDate = true;
    }

    private void MarkActiveCell(int x, int y) {
      matrixNavigator.width = eventMatrix.width;
      matrixNavigator.height = eventMatrix.height;
      matrixNavigator.positions = currentEvFile.positions;
      matrixNavigator.MarkActiveCell(x, y);
      setEventMatrixUpDown(x, y);
    }

    private void setEventMatrixUpDown(int x, int y) {
      eventMatrixXUpDown.Value = x;
      eventMatrixYUpDown.Value = y;
    }

    private void CenterEventViewOnEntities() {
      int destX = 0;
      int destY = 0;
      if (currentEvFile.overworlds.Count > 0) {
        destX = currentEvFile.overworlds[0].xMatrixPosition;
        destY = currentEvFile.overworlds[0].yMatrixPosition;
      }
      else if (currentEvFile.warps.Count > 0) {
        destX = currentEvFile.warps[0].xMatrixPosition;
        destY = currentEvFile.warps[0].yMatrixPosition;
      }
      else if (currentEvFile.spawnables.Count > 0) {
        destX = currentEvFile.spawnables[0].xMatrixPosition;
        destY = currentEvFile.spawnables[0].yMatrixPosition;
      }
      else if (currentEvFile.triggers.Count > 0) {
        destX = currentEvFile.triggers[0].xMatrixPosition;
        destY = currentEvFile.triggers[0].yMatrixPosition;
      }

      if (destX > eventMatrixXUpDown.Maximum || destY > eventMatrixYUpDown.Maximum) {
        //MessageBox.Show("One of the events tried to reference a bigger Matrix.\nMake sure the Header File associated to this Event File is using the correct Matrix.", "Error",
        //        MessageBoxButtons.OK, MessageBoxIcon.Error);
        setEventMatrixUpDown(0, 0);
      }
      else {
        setEventMatrixUpDown(destX, destY);
      }
    }

    private void selectEventComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      /* Load events data into EventFile class instance */
      currentEvFile = new EventFile(selectEventComboBox.SelectedIndex);

      /* Update ListBoxes */
      FillSpawnablesBox();
      FillOverworldsBox();
      FillTriggersBox();
      FillWarpsBox();

      eventEditorFullMapReload();
    }

    private void saveEventsButton_Click(object sender, EventArgs e) {
      currentEvFile.SaveToFileDefaultDir(selectEventComboBox.SelectedIndex);
    }

    private void importEventFileButton_Click(object sender, EventArgs e) {
      /* Prompt user to select .evt file */
      OpenFileDialog of = new OpenFileDialog {
        Filter = EventFile.DefaultFilter
      };
      if (of.ShowDialog(this) != DialogResult.OK) {
        return;
      }

      EventFile toImport = new EventFile(File.OpenRead(of.FileName));
      if (toImport.isEmpty()) {
        DialogResult d = MessageBox.Show("Are you sure you want to import an empty event file?", "Empty File loaded", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (!d.Equals(DialogResult.Yes)) {
          return;
        }

        /* Update event object on disk */
        File.Copy(of.FileName, RomInfo.gameDirs[DirNames.eventFiles].unpackedDir + "\\" + selectEventComboBox.SelectedIndex.ToString("D4"), true);

        /* Refresh controls */
        selectEventComboBox_SelectedIndexChanged(null, null);

        /* Display success message */
        MessageBox.Show("Events imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

        /* Refresh controls */
        selectEventComboBox_SelectedIndexChanged(null, null);
      }
      else {
        EventFileImport efi = new EventFileImport(toImport);
        efi.ShowDialog();

        if (!efi.DialogResult.Equals(DialogResult.OK)) {
          MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
          return;
        }

        int[] currentArray;

        currentArray = efi.userSelected[(int)EventFile.SerializationOrders.Spawnables];
        if (currentArray != null) {
          foreach (int index in currentArray) {
            currentEvFile.spawnables.Add(toImport.spawnables[index]);

            spawnablesListBox.Items.Add("");
            spawnablesListBox.SelectedIndex = spawnablesListBox.Items.Count - 1;
            updateSelectedSpawnableName();
          }
        }

        currentArray = efi.userSelected[(int)EventFile.SerializationOrders.Overworlds];
        if (currentArray != null) {
          foreach (int index in currentArray) {
            currentEvFile.overworlds.Add(toImport.overworlds[index]);

            overworldsListBox.Items.Add("");
            overworldsListBox.SelectedIndex = overworldsListBox.Items.Count - 1;
            updateSelectedOverworldName();
          }
        }

        currentArray = efi.userSelected[(int)EventFile.SerializationOrders.Warps];
        if (currentArray != null) {
          foreach (int index in currentArray) {
            currentEvFile.warps.Add(toImport.warps[index]);

            warpsListBox.Items.Add("");
            warpsListBox.SelectedIndex = warpsListBox.Items.Count - 1;
            updateSelectedWarpName();
          }
        }

        currentArray = efi.userSelected[(int)EventFile.SerializationOrders.Triggers];
        if (currentArray != null) {
          foreach (int index in currentArray) {
            currentEvFile.triggers.Add(toImport.triggers[index]);

            triggersListBox.Items.Add("");
            triggersListBox.SelectedIndex = triggersListBox.Items.Count - 1;
            updateSelectedTriggerName();
          }
        }

        /* Display success message */
        MessageBox.Show("Events imported successfully!\nRemember to save the current Event File.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void updateSelectedWarpName() {
      int index = warpsListBox.SelectedIndex;
      warpsListBox.Items[index] = index.ToString("D" + Math.Max(0, warpsListBox.Items.Count - 1).ToString().Length) + ": " + (selectedEvent as Warp).ToString();
    }

    private void updateSelectedTriggerName() {
      int index = triggersListBox.SelectedIndex;
      triggersListBox.Items[index] = index.ToString("D" + Math.Max(0, triggersListBox.Items.Count - 1).ToString().Length) + ": " + (selectedEvent as Trigger).ToString();
    }

    private void updateSelectedOverworldName() {
      int index = overworldsListBox.SelectedIndex;
      overworldsListBox.Items[index] = index.ToString("D" + Math.Max(0, overworldsListBox.Items.Count - 1).ToString().Length) + ": " + (selectedEvent as Overworld).ToString();
    }

    private void exportEventFileButton_Click(object sender, EventArgs e) {
      currentEvFile.SaveToFileExplorePath("Event File " + selectEventComboBox.SelectedIndex);
    }

    private void eventShiftDownButton_Click(object sender, EventArgs e) {
      if (eventMatrixYUpDown.Value < eventMatrix.height - 1) {
        eventMatrixYUpDown.Value += 1;
      }
    }

    private void eventShiftUpButton_Click(object sender, EventArgs e) {
      if (eventMatrixYUpDown.Value > 0) {
        eventMatrixYUpDown.Value -= 1;
      }
    }

    private void eventShiftLeftButton_Click(object sender, EventArgs e) {
      if (eventMatrixXUpDown.Value > 0) {
        eventMatrixXUpDown.Value -= 1;
      }
    }

    private void eventShiftRightButton_Click(object sender, EventArgs e) {
      if (eventMatrixXUpDown.Value < eventMatrix.width - 1) {
        eventMatrixXUpDown.Value += 1;
      }
    }

    private void addEventFileButton_Click(object sender, EventArgs e) {
      /* Add copy of event 0 to event folder */
      new EventFile().SaveToFileDefaultDir(selectEventComboBox.Items.Count);

      /* Update ComboBox and select new file */
      selectEventComboBox.Items.Add("Event File " + selectEventComboBox.Items.Count);
      selectEventComboBox.SelectedIndex = selectEventComboBox.Items.Count - 1;
    }

    private void removeEventFileButton_Click(object sender, EventArgs e) {
      DialogResult d = MessageBox.Show("Are you sure you want to delete the last Event File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
      if (d.Equals(DialogResult.Yes)) {
        /* Delete event file */
        File.Delete(RomInfo.gameDirs[DirNames.eventFiles].unpackedDir + "\\" + (selectEventComboBox.Items.Count - 1).ToString("D4"));

        /* Check if currently selected file is the last one, and in that case select the one before it */
        int lastIndex = selectEventComboBox.Items.Count - 1;
        if (selectEventComboBox.SelectedIndex == lastIndex) {
          selectEventComboBox.SelectedIndex--;
        }

        /* Remove item from ComboBox */
        selectEventComboBox.Items.RemoveAt(lastIndex);
      }
    }

    private void eventPictureBox_Click(object sender, EventArgs e) {
      Point coordinates = openGlPictureBox.PointToClient(Cursor.Position);
      Point mouseTilePos = new Point(coordinates.X / (tileSize + 1), coordinates.Y / (tileSize + 1));
      MouseEventArgs mea = (MouseEventArgs)e;

      if (mea.Button == MouseButtons.Left) {
        if (selectedEvent != null) {
          switch (selectedEvent.evType) {
            case Event.EventType.Spawnable:
              if (!showSpawnablesCheckBox.Checked) {
                return;
              }

              spawnableMapXUpDown.Value = (short)mouseTilePos.X;
              spawnableMapYUpDown.Value = (short)mouseTilePos.Y;
              spawnableMatrixXUpDown.Value = (short)eventMatrixXUpDown.Value;
              spawnableMatrixYUpDown.Value = (short)eventMatrixYUpDown.Value;

              break;
            case Event.EventType.Overworld:
              if (!showOwsCheckBox.Checked) {
                return;
              }

              owMapXUpDown.Value = (short)mouseTilePos.X;
              owMapYUpDown.Value = (short)mouseTilePos.Y;
              owMatrixXUpDown.Value = (short)eventMatrixXUpDown.Value;
              owMatrixYUpDown.Value = (short)eventMatrixYUpDown.Value;

              break;
            case Event.EventType.Warp:
              if (!showWarpsCheckBox.Checked) {
                return;
              }

              warpMapXUpDown.Value = (short)mouseTilePos.X;
              warpMapYUpDown.Value = (short)mouseTilePos.Y;
              warpMatrixXUpDown.Value = (short)eventMatrixXUpDown.Value;
              warpMatrixYUpDown.Value = (short)eventMatrixYUpDown.Value;

              break;
            case Event.EventType.Trigger:
              if (!showTriggersCheckBox.Checked) {
                return;
              }

              triggerMapXUpDown.Value = (short)mouseTilePos.X;
              triggerMapYUpDown.Value = (short)mouseTilePos.Y;
              triggerMatrixXUpDown.Value = (short)eventMatrixXUpDown.Value;
              triggerMatrixYUpDown.Value = (short)eventMatrixYUpDown.Value;

              break;
          }

          DisplayActiveEvents();
        }
      }
      else if (mea.Button == MouseButtons.Right) {
        if (showWarpsCheckBox.Checked)
          for (int i = 0; i < currentEvFile.warps.Count; i++) {
            Warp ev = currentEvFile.warps[i];

            if (isEventUnderMouse(ev, mouseTilePos)) {
              if (ev == selectedEvent) {
                goToWarpDestination_Click(sender, e);
                return;
              }

              selectedEvent = ev;
              eventsTabControl.SelectedTab = warpsTabPage;
              warpsListBox.SelectedIndex = i;
              DisplayActiveEvents();
              return;
            }
          }

        if (showSpawnablesCheckBox.Checked)
          for (int i = 0; i < currentEvFile.spawnables.Count; i++) {
            Spawnable ev = currentEvFile.spawnables[i];

            if (isEventUnderMouse(ev, mouseTilePos)) {
              selectedEvent = ev;
              eventsTabControl.SelectedTab = signsTabPage;
              spawnablesListBox.SelectedIndex = i;
              DisplayActiveEvents();
              return;
            }
          }

        if (showOwsCheckBox.Checked)
          for (int i = 0; i < currentEvFile.overworlds.Count; i++) {
            Overworld ev = currentEvFile.overworlds[i];

            if (isEventUnderMouse(ev, mouseTilePos)) {
              selectedEvent = ev;
              eventsTabControl.SelectedTab = overworldsTabPage;
              overworldsListBox.SelectedIndex = i;
              DisplayActiveEvents();
              return;
            }
          }

        for (int i = 0; i < currentEvFile.triggers.Count; i++) {
          Trigger ev = currentEvFile.triggers[i];

          if (isEventUnderMouse(ev, mouseTilePos, ev.widthX - 1, ev.heightY - 1)) {
            selectedEvent = ev;
            eventsTabControl.SelectedTab = triggersTabPage;
            triggersListBox.SelectedIndex = i;
            DisplayActiveEvents();
            return;
          }
        }
      }
      else if (mea.Button == MouseButtons.Middle) {
        for (int i = 0; i < currentEvFile.warps.Count; i++) {
          Warp ev = currentEvFile.warps[i];

          if (isEventUnderMouse(ev, mouseTilePos)) {
            if (ev == selectedEvent) {
              goToWarpDestination_Click(sender, e);
              return;
            }
          }
        }
      }
    }

    private bool isEventOnCurrentMatrix(Event ev) {
      if (ev.xMatrixPosition == eventMatrixXUpDown.Value) {
        if (ev.yMatrixPosition == eventMatrixYUpDown.Value) {
          return true;
        }
      }

      return false;
    }

    private bool isEventUnderMouse(Event ev, Point mouseTilePos, int widthX = 0, int heightY = 0) {
      if (isEventOnCurrentMatrix(ev)) {
        Point evLocalCoords = new Point(ev.xMapPosition, ev.yMapPosition);
        Func<int, int, int, bool> checkRange = (mouseCoord, evCoord, extension) => mouseCoord >= evCoord && mouseCoord <= evCoord + extension;

        if (checkRange(mouseTilePos.X, evLocalCoords.X, widthX) && checkRange(mouseTilePos.Y, evLocalCoords.Y, heightY)) {
          return true;
        }
      }

      return false;
    }

    private Bitmap GetOverworldImage(ushort eventEntryID, ushort orientation) {
      /* Find sprite corresponding to ID and load it*/
      if (RomInfo.ow3DSpriteDict.TryGetValue(eventEntryID, out string imageName)) {
        // If overworld is 3D, load image from dictionary
        return (Bitmap)Properties.Resources.ResourceManager.GetObject(imageName);
      }

      if (!RomInfo.OverworldTable.TryGetValue(eventEntryID, out (uint spriteID, ushort properties) result)) {
        // try loading image from dictionary
        return (Bitmap)Properties.Resources.ResourceManager.GetObject("overworld"); //if there's no match, load bounding box
      }

      try {
        FileStream stream = new FileStream(RomInfo.gameDirs[DirNames.OWSprites].unpackedDir + "\\" + result.spriteID.ToString("D4"), FileMode.Open);
        NSBTX_File nsbtx = new NSBTX_File(stream);

        if (nsbtx.texInfo.num_objs <= 1) {
          return nsbtx.GetBitmap(0, 0).bmp; // Read nsbtx slot 0 if ow has only 2 frames
        }

        if (nsbtx.texInfo.num_objs <= 4) {
          switch (orientation) {
            case 0:
              return nsbtx.GetBitmap(0, 0).bmp;
            case 1:
              return nsbtx.GetBitmap(1, 0).bmp;
            case 2:
              return nsbtx.GetBitmap(2, 0).bmp;
            default:
              return nsbtx.GetBitmap(3, 0).bmp;
          }
        }

        if (nsbtx.texInfo.num_objs <= 8) {
          //Read nsbtx slot corresponding to overworld's movement
          switch (orientation) {
            case 0:
              return nsbtx.GetBitmap(0, 0).bmp;
            case 1:
              return nsbtx.GetBitmap(2, 0).bmp;
            case 2:
              return nsbtx.GetBitmap(4, 0).bmp;
            default:
              return nsbtx.GetBitmap(6, 0).bmp;
          }
        }

        if (nsbtx.texInfo.num_objs <= 16) {
          // Read nsbtx slot corresponding to overworld's movement
          switch (orientation) {
            case 0:
              return nsbtx.GetBitmap(0, 0).bmp;
            case 1:
              return nsbtx.GetBitmap(11, 0).bmp;
            case 2:
              return nsbtx.GetBitmap(2, 0).bmp;
            default:
              return nsbtx.GetBitmap(4, 0).bmp;
          }
        }
        else {
          switch (orientation) {
            case 0:
              return nsbtx.GetBitmap(0, 0).bmp;
            case 1:
              return nsbtx.GetBitmap(27, 0).bmp;
            case 2:
              return nsbtx.GetBitmap(2, 0).bmp;
            default:
              return nsbtx.GetBitmap(4, 0).bmp;
          }
        }
      }
      catch {
        // Load bounding box if sprite cannot be found
        return (Bitmap)Properties.Resources.ResourceManager.GetObject("overworldUnreadable");
      }
    }

    private void eventPictureBox_MouseMove(object sender, MouseEventArgs e) {
      Point coordinates = openGlPictureBox.PointToClient(Cursor.Position);
      Point mouseTilePos = new Point(coordinates.X / (tileSize + 1), coordinates.Y / (tileSize + 1));
      Helpers.statusLabelMessage("Local: " + mouseTilePos.X + ", " + mouseTilePos.Y + "   |   " + "Global: " + (eventMatrixXUpDown.Value * MapFile.mapSize + mouseTilePos.X).ToString() + ", " + (eventMatrixYUpDown.Value * MapFile.mapSize + mouseTilePos.Y).ToString());
    }

    private void updateSelectedSpawnableName() {
      int index = spawnablesListBox.SelectedIndex;
      spawnablesListBox.Items[index] = index.ToString("D" + Math.Max(0, spawnablesListBox.Items.Count - 1).ToString().Length) + ": " + (selectedEvent as Spawnable).ToString();
    }

    private void spawnableTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int selectedSpawnable = spawnablesListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selectedSpawnable < 0) {
        return;
      }

      spawnableDirComboBox.Enabled = spawnableTypeComboBox.SelectedIndex != Spawnable.TYPE_HIDDENITEM;

      currentEvFile.spawnables[selectedSpawnable].type = (ushort)spawnableTypeComboBox.SelectedIndex;
      updateSelectedSpawnableName();
    }

    private void duplicateSpawnableButton_Click(object sender, EventArgs e) {
      if (spawnablesListBox.SelectedIndex < 0) {
        return;
      }

      currentEvFile.spawnables.Add(new Spawnable((Spawnable)selectedEvent));

      spawnablesListBox.Items.Add("");
      spawnablesListBox.SelectedIndex = spawnablesListBox.Items.Count - 1;
      updateSelectedSpawnableName();
    }

    private void centerEventViewOnSelectedEvent_Click(object sender, EventArgs e) {
      if (selectedEvent is null) {
        MessageBox.Show("You haven't selected any event.", "Nothing to do here", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      else {
        try {
          setEventMatrixUpDown(selectedEvent.xMatrixPosition, selectedEvent.yMatrixPosition);
        }
        catch (ArgumentOutOfRangeException) {
          DialogResult main = MessageBox.Show("The selected event tried to reference a bigger Matrix than the one which is currently being displayed.\nDo you want to check for another potentially compatible matrix?", "Event is out of range", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

          if (main.Equals(DialogResult.Yes)) {
            ushort[] result = HeaderSearch.AdvancedSearch(0, (ushort)EditorPanels.headerEditor.internalNames.Count, EditorPanels.headerEditor.internalNames, (int)MapHeader.SearchableFields.EventFileID, (int)HeaderSearch.NumOperators.Equal, selectEventComboBox.SelectedIndex.ToString())
              .Select(x => ushort.Parse(x.Split()[0]))
              .ToArray();

            Dictionary<ushort, ushort> dict = new Dictionary<ushort, ushort>();

            if (gameFamily.Equals(GameFamilies.DP)) {
              foreach (ushort headerID in result) {
                HeaderDP hdp = (HeaderDP)MapHeader.LoadFromARM9(headerID);

                if (hdp.matrixID != eventMatrixUpDown.Value && hdp.locationName != 0) {
                  dict.Add(hdp.ID, hdp.matrixID);
                }
              }
            }
            else if (gameFamily.Equals(GameFamilies.Plat)) {
              foreach (ushort headerID in result) {
                HeaderPt hpt;

                if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                  hpt = (HeaderPt)MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerID.ToString("D4"), headerID, 0);
                }
                else {
                  hpt = (HeaderPt)MapHeader.LoadFromARM9(headerID);
                }

                if (hpt.matrixID != eventMatrixUpDown.Value && hpt.locationName != 0) {
                  dict.Add(hpt.ID, hpt.matrixID);
                }
              }
            }
            else {
              foreach (ushort headerID in result) {
                HeaderHGSS hgss;

                if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                  hgss = (HeaderHGSS)MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerID.ToString("D4"), headerID, 0);
                }
                else {
                  hgss = (HeaderHGSS)MapHeader.LoadFromARM9(headerID);
                }

                if (hgss.matrixID != eventMatrixUpDown.Value && hgss.locationName != 0) {
                  dict.Add(hgss.ID, hgss.matrixID);
                }
              }
            }

            if (dict.Count < 1) {
              MessageBox.Show("DSPRE could not find another Header referencing the same Event File and a different Matrix.", "Search failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
              setEventMatrixUpDown(0, 0);
            }
            else {
              if (dict.Count > 1) {
                if (dict.Keys.Contains(EditorPanels.headerEditor.currentHeader.ID)) {
                  DialogResult yn = MessageBox.Show("DSPRE found multiple Headers referencing the same Event File and a different Matrix.\n" +
                                                    $"The last selected Header ({EditorPanels.headerEditor.currentHeader.ID}) is one of those.\n" +
                                                    $"Do you want to load its matrix (#{EditorPanels.headerEditor.currentHeader.matrixID}?)", "Potential solution found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                  if (yn.Equals(DialogResult.Yes)) {
                    eventMatrixUpDown.Value = EditorPanels.headerEditor.currentHeader.matrixID;
                  }
                }
                else {
                  var kvp = dict.First();

                  DialogResult yn = MessageBox.Show($"DSPRE found {dict.Count} Headers referencing the same Event File and a different Matrix.\n" +
                                                    $"Do you want to load Header {kvp.Key}'s matrix (#{kvp.Value})?", "Potential solution found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                  if (yn.Equals(DialogResult.Yes)) {
                    eventMatrixUpDown.Value = kvp.Value;
                  }
                }
              }
              else {
                var kvp = dict.First();

                DialogResult yn = MessageBox.Show($"Header {kvp.Key}'s matrix (#{kvp.Value}) seems to be the only match for this Event File.\n" +
                                                  $"Do you want to load it?", "Potential solution found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (yn.Equals(DialogResult.Yes)) {
                  eventMatrixUpDown.Value = kvp.Value;
                }
              }
            }
          }
        }
        finally {
          Update();
        }
      }
    }

    private void spawnableMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
      int selectedSpawnable = spawnablesListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selectedSpawnable < 0) {
        return;
      }

      currentEvFile.spawnables[selectedSpawnable].xMatrixPosition = (ushort)spawnableMatrixXUpDown.Value;
      DisplayActiveEvents();
    }

    private void spawnableMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
      int selectedSpawnable = spawnablesListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selectedSpawnable < 0) {
        return;
      }

      currentEvFile.spawnables[selectedSpawnable].yMatrixPosition = (ushort)spawnableMatrixYUpDown.Value;
      DisplayActiveEvents();
    }

    private void spawnableMapXUpDown_ValueChanged(object sender, EventArgs e) {
      int selectedSpawnable = spawnablesListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selectedSpawnable < 0) {
        return;
      }

      currentEvFile.spawnables[selectedSpawnable].xMapPosition = (short)spawnableMapXUpDown.Value;
      DisplayActiveEvents();
    }

    private void spawnableMapYUpDown_ValueChanged(object sender, EventArgs e) {
      int selectedSpawnable = spawnablesListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selectedSpawnable < 0) {
        return;
      }

      currentEvFile.spawnables[selectedSpawnable].yMapPosition = (short)spawnableMapYUpDown.Value;
      DisplayActiveEvents();
    }

    private void spawnableMapZUpDown_ValueChanged(object sender, EventArgs e) {
      int selectedSpawnable = spawnablesListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selectedSpawnable < 0) {
        return;
      }

      currentEvFile.spawnables[selectedSpawnable].zMapPosition = (short)spawnableMapZUpDown.Value;
      DisplayActiveEvents();
    }

    private void removeSpawnableButton_Click(object sender, EventArgs e) {
      if (spawnablesListBox.SelectedIndex < 0) {
        return;
      }

      Helpers.DisableHandlers();

      /* Remove trigger object from list and the corresponding entry in the ListBox */
      int spawnableNumber = spawnablesListBox.SelectedIndex;
      currentEvFile.spawnables.RemoveAt(spawnableNumber);
      spawnablesListBox.Items.RemoveAt(spawnableNumber);

      FillSpawnablesBox(); // Update ListBox

      Helpers.EnableHandlers();

      if (spawnableNumber > 0) {
        spawnablesListBox.SelectedIndex = spawnableNumber - 1;
      }
      else {
        DisplayActiveEvents();
      }
    }

    private void FillSpawnablesBox() {
      spawnablesListBox.Items.Clear();
      int count = currentEvFile.spawnables.Count;

      for (int i = 0; i < count; i++) {
        spawnablesListBox.Items.Add(i.ToString("D" + Math.Max(0, count - 1).ToString().Length) + ": " + currentEvFile.spawnables[i].ToString());
      }
    }

    private void addSpawnableButton_Click(object sender, EventArgs e) {
      int spCount = currentEvFile.spawnables.Count;

      currentEvFile.spawnables.Add(new Spawnable((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));

      spawnablesListBox.Items.Add("");
      spawnablesListBox.SelectedIndex = spCount;
      updateSelectedSpawnableName();
    }

    private void spawnableDirComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int selectedSpawnable = spawnablesListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selectedSpawnable < 0) {
        return;
      }

      currentEvFile.spawnables[selectedSpawnable].dir = (ushort)spawnableDirComboBox.SelectedIndex;
      updateSelectedSpawnableName();
    }

    private void spawnableScriptUpDown_ValueChanged(object sender, EventArgs e) {
      int selectedSpawnable = spawnablesListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selectedSpawnable < 0) {
        return;
      }

      currentEvFile.spawnables[selectedSpawnable].scriptNumber = (ushort)spawnableScriptUpDown.Value;
      updateSelectedSpawnableName();
    }

    private void spawnablesListBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled || spawnablesListBox.SelectedIndex < 0)
        return;
      Helpers.DisableHandlers();

      /* Set Event */
      selectedEvent = currentEvFile.spawnables[spawnablesListBox.SelectedIndex];

      /* Update Controls */
      spawnableDirComboBox.SelectedIndex = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].dir;
      spawnableTypeComboBox.SelectedIndex = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].type;

      spawnableScriptUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].scriptNumber;
      spawnableMapXUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].xMapPosition;
      spawnableMapYUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].yMapPosition;
      spawnableMapZUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].zMapPosition;
      spawnableMatrixXUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].xMatrixPosition;
      spawnableMatrixYUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].yMatrixPosition;

      DisplayActiveEvents();
      Helpers.EnableHandlers();
    }

    private void OWTypeChanged(object sender, EventArgs e) {
      if (overworldsListBox.SelectedIndex < 0) {
        return;
      }

      if (normalRadioButton.Checked == true) {
        owScriptNumericUpDown.Enabled = true;
        owSpecialGroupBox.Enabled = false;

        if (Helpers.HandlersDisabled) {
          return;
        }

        currentEvFile.overworlds[overworldsListBox.SelectedIndex].type = 0x0;
        currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(owScriptNumericUpDown.Value = 0);
      }
      else if (isItemRadioButton.Checked == true) {
        owScriptNumericUpDown.Enabled = false;

        owSpecialGroupBox.Enabled = true;
        owTrainerComboBox.Enabled = false;
        owTrainerLabel.Enabled = false;
        owSightRangeUpDown.Enabled = false;
        owSightRangeLabel.Enabled = false;
        owPartnerTrainerCheckBox.Enabled = false;

        if (Helpers.HandlersDisabled) {
          return;
        }

        if (isItemRadioButton.Enabled) {
          owItemComboBox.Enabled = true;
          itemsSelectorHelpBtn.Enabled = true;
          owItemLabel.Enabled = true;

          currentEvFile.overworlds[overworldsListBox.SelectedIndex].type = 0x3;
          currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(owScriptNumericUpDown.Value = 7000 + owItemComboBox.SelectedIndex);
        }
      }
      else {
        //trainer
        owScriptNumericUpDown.Enabled = false;

        owSpecialGroupBox.Enabled = true;
        owTrainerComboBox.Enabled = true;
        owTrainerLabel.Enabled = true;
        owItemLabel.Enabled = false;
        owSightRangeUpDown.Enabled = true;
        owSightRangeLabel.Enabled = true;
        owPartnerTrainerCheckBox.Enabled = true;

        owItemComboBox.Enabled = false;
        itemsSelectorHelpBtn.Enabled = false;

        if (Helpers.HandlersDisabled) {
          return;
        }

        currentEvFile.overworlds[overworldsListBox.SelectedIndex].type = 0x1;
        if (owTrainerComboBox.SelectedIndex >= 0) {
          owTrainerComboBox_SelectedIndexChanged(null, null);
        }
      }
    }

    private void eventEditorFullMapReload() {
      MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
      DisplayActiveEvents();
      DisplayEventMap();
    }

    public void eventMatrixUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      Helpers.DisableHandlers();

      eventMatrix = new GameMatrix((int)eventMatrixUpDown.Value);
      eventMatrixXUpDown.Maximum = eventMatrix.width - 1;
      eventMatrixYUpDown.Maximum = eventMatrix.height - 1;
      eventEditorFullMapReload();

      Helpers.EnableHandlers();

      CenterEventViewOnEntities();
    }

    private void owMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].xMatrixPosition = (ushort)owMatrixXUpDown.Value;

      MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
      DisplayActiveEvents();
    }

    private void owMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].yMatrixPosition = (ushort)owMatrixYUpDown.Value;

      MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
      DisplayActiveEvents();
    }

    private void DisplayActiveEvents() {
      openGlPictureBox.Image = new Bitmap(openGlPictureBox.Width, openGlPictureBox.Height);

      using (Graphics g = Graphics.FromImage(openGlPictureBox.Image)) {
        Bitmap icon;

        /* Draw spawnables */
        if (showSpawnablesCheckBox.Checked) {
          icon = (Bitmap)Properties.Resources.ResourceManager.GetObject("sign");

          for (int i = 0; i < currentEvFile.spawnables.Count; i++) {
            Spawnable spawnable = currentEvFile.spawnables[i];
            if (!isEventOnCurrentMatrix(spawnable)) {
              continue;
            }

            g.DrawImage(icon, spawnable.xMapPosition * (tileSize + 1), spawnable.yMapPosition * (tileSize + 1));

            if (selectedEvent == spawnable) {
              // Draw selection rectangle if event is the selected one
              DrawSelectionRectangle(g, spawnable);
            }
          }
        }

        /* Draw overworlds */
        if (showOwsCheckBox.Checked) {
          for (int i = 0; i < currentEvFile.overworlds.Count; i++) {
            Overworld overworld = currentEvFile.overworlds[i];
            if (!isEventOnCurrentMatrix(overworld)) continue;

            // Draw image only if event is in current map
            Bitmap sprite = GetOverworldImage(overworld.overlayTableEntry, overworld.orientation);
            sprite.MakeTransparent();
            g.DrawImage(sprite, (overworld.xMapPosition) * (tileSize + 1) - 7 + (32 - sprite.Width) / 2, (overworld.yMapPosition - 1) * (tileSize + 1) + (32 - sprite.Height));

            if (selectedEvent == overworld) {
              DrawSelectionRectangleOverworld(g, overworld);
            }
          }
        }

        /* Draw warps */
        if (showWarpsCheckBox.Checked) {
          DrawWarpCollisions(g);

          icon = (Bitmap)Properties.Resources.ResourceManager.GetObject("warp");
          for (int i = 0; i < currentEvFile.warps.Count; i++) {
            Warp warp = currentEvFile.warps[i];
            if (!isEventOnCurrentMatrix(warp)) continue;

            g.DrawImage(icon, warp.xMapPosition * (tileSize + 1), warp.yMapPosition * (tileSize + 1));

            if (selectedEvent == warp) {
              // Draw selection rectangle if event is the selected one
              DrawSelectionRectangle(g, warp);
            }
          }
        }

        /* Draw triggers */
        if (showTriggersCheckBox.Checked) {
          icon = (Bitmap)Properties.Resources.ResourceManager.GetObject("trigger");

          for (int i = 0; i < currentEvFile.triggers.Count; i++) {
            Trigger trigger = currentEvFile.triggers[i];
            if (!isEventOnCurrentMatrix(trigger)) continue;

            for (int y = 0; y < currentEvFile.triggers[i].heightY; y++) {
              for (int x = 0; x < currentEvFile.triggers[i].widthX; x++) {
                g.DrawImage(icon, (trigger.xMapPosition + x) * (tileSize + 1), (trigger.yMapPosition + y) * (tileSize + 1));
              }
            }

            if (selectedEvent == trigger) {
              // Draw selection rectangle if event is the selected one
              DrawSelectionRectangleTrigger(g, trigger);
            }
          }
        }
      }

      openGlPictureBox.Invalidate();
    }

    private void DrawSelectionRectangleOverworld(Graphics g, Overworld ow) {
      Pen pen = Pens.Red;
      g.DrawRectangle(pen, (ow.xMapPosition) * (tileSize + 1) - 8, (ow.yMapPosition - 1) * (tileSize + 1), 34, 34);
      g.DrawRectangle(pen, (ow.xMapPosition) * (tileSize + 1) - 9, (ow.yMapPosition - 1) * (tileSize + 1) - 1, 36, 36);
    }

    private void DrawWarpCollisions(Graphics g) {
      if (currentMapFile == null) return;

      Bitmap icon = (Bitmap)Properties.Resources.ResourceManager.GetObject("warpCollision");
      for (int y = 0; y < MapFile.mapSize; y++) {
        for (int x = 0; x < MapFile.mapSize; x++) {
          byte moveperm = currentMapFile.types[x, y];
          if (!PokeDatabase.System.MapCollisionTypePainters.TryGetValue(moveperm, out string val)) continue;
          if (val.IndexOf("Warp", StringComparison.InvariantCultureIgnoreCase) < 0) continue;
          //Console.WriteLine("Found warp at " + i + ", " + j);
          g.DrawImage(icon, y * (tileSize + 1), x * (tileSize + 1));
        }
      }
    }

    private void DrawSelectionRectangleTrigger(Graphics g, Trigger t) {
      Pen pen = Pens.Red;
      g.DrawRectangle(pen, (t.xMapPosition) * (tileSize + 1) - 1, (t.yMapPosition) * (tileSize + 1) - 1, 17 * t.widthX + 1, (tileSize + 1) * t.heightY + 1);
      g.DrawRectangle(pen, (t.xMapPosition) * (tileSize + 1) - 2, (t.yMapPosition) * (tileSize + 1) - 2, 17 * t.widthX + 3, (tileSize + 1) * t.heightY + 3);
    }

    private void DrawSelectionRectangle(Graphics g, Event ev) {
      Pen pen = Pens.Red;
      g.DrawRectangle(pen, (ev.xMapPosition) * (tileSize + 1) - 1, (ev.yMapPosition) * (tileSize + 1) - 1, 18, 18);
      g.DrawRectangle(pen, (ev.xMapPosition) * (tileSize + 1) - 2, (ev.yMapPosition) * (tileSize + 1) - 2, 20, 20);
    }

    private void owRangeXUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[overworldsListBox.SelectedIndex].xRange = (ushort)owRangeXUpDown.Value;
    }

    private void owRangeYUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[overworldsListBox.SelectedIndex].yRange = (ushort)owRangeYUpDown.Value;
    }

    private void owOrientationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      ushort orientation = (ushort)owOrientationComboBox.SelectedIndex;
      int selection = overworldsListBox.SelectedIndex;
      if (owSpriteComboBox.SelectedIndex < 0 || orientation < 0) {
        return;
      }

      if (selection >= 0) {
        owSpritePictureBox.BackgroundImage = GetOverworldImage(currentEvFile.overworlds[selection].overlayTableEntry, orientation);

        if (Helpers.HandlersEnabled) {
          currentEvFile.overworlds[selection].orientation = orientation;
          DisplayActiveEvents();
        }
      }
      else {
        owSpritePictureBox.BackgroundImage = GetOverworldImage((ushort)owSpriteComboBox.SelectedIndex, orientation);
      }

      owSpritePictureBox.Invalidate();
    }

    private void owMovementComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].movement = (ushort)owMovementComboBox.SelectedIndex;
    }

    private void owXMapUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[overworldsListBox.SelectedIndex].xMapPosition = (short)owMapXUpDown.Value;
      DisplayActiveEvents();
    }

    private void owMapYUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[overworldsListBox.SelectedIndex].yMapPosition = (short)owMapYUpDown.Value;
      DisplayActiveEvents();
    }

    private void owMapZUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].zMapPosition = (short)owMapZUpDown.Value;
    }

    private void itemsSelectorHelpBtn_Click(object sender, EventArgs e) {
      MessageBox.Show("This selector allows you to pick a preset Ground Item script from the game data.\n" +
                      "Unlike in previous DSPRE versions, you can now change the Ground Item to be obtained even if you decided not to apply the Standardize Items patch from the Rom ToolBox.\n\n" +
                      "However, some items are unavailable by default. The aforementioned patch can neutralize this limitation.\n\n",
        "About Ground Items", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void owPartnerTrainerCheckBox_CheckedChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].scriptNumber = (ushort)(currentEvFile.overworlds[selection].scriptNumber + (owPartnerTrainerCheckBox.Checked ? 2000 : -2000));
    }

    private void owItemComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled || overworldsListBox.SelectedIndex < 0) {
        return;
      }

      owScriptNumericUpDown.Value = currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(7000 + owItemComboBox.SelectedIndex);
    }

    private void owTrainerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].scriptNumber = (ushort)(owTrainerComboBox.SelectedIndex + (owPartnerTrainerCheckBox.Checked ? 4999 : 2999));
    }

    private void owSightRangeUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].sightRange = (ushort)owSightRangeUpDown.Value;
    }

    private void owScriptNumericUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].scriptNumber = (ushort)owScriptNumericUpDown.Value;
      updateSelectedOverworldName();
    }

    private void owFlagNumericUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].flag = (ushort)owFlagNumericUpDown.Value;
    }

    private void owSpriteComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;
      if (owSpriteComboBox.SelectedIndex < 0) {
        return;
      }

      ushort overlayTableEntryID = (ushort)RomInfo.OverworldTable.Keys.ElementAt(owSpriteComboBox.SelectedIndex);
      uint spriteID = RomInfo.OverworldTable[overlayTableEntryID].spriteID;

      if (spriteID == 0x3D3D) {
        spriteIDlabel.Text = "3D Overworld";
      }
      else {
        spriteIDlabel.Text = "Sprite ID: " + spriteID;
      }

      if (selection >= 0) {
        owSpritePictureBox.BackgroundImage = GetOverworldImage(overlayTableEntryID, currentEvFile.overworlds[selection].orientation);

        if (Helpers.HandlersEnabled) {
          currentEvFile.overworlds[selection].overlayTableEntry = overlayTableEntryID;
          DisplayActiveEvents();
        }
      }
      else {
        owSpritePictureBox.BackgroundImage = GetOverworldImage(overlayTableEntryID, (ushort)owOrientationComboBox.SelectedIndex);
      }

      owSpritePictureBox.Invalidate();
      updateSelectedOverworldName();
    }

    private void owIDNumericUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.overworlds[selection].owID = (ushort)owIDNumericUpDown.Value;
      updateSelectedOverworldName();
    }

    private void duplicateOverworldsButton_Click(object sender, EventArgs e) {
      if (overworldsListBox.SelectedIndex < 0) {
        return;
      }

      currentEvFile.overworlds.Add(new Overworld(selectedEvent as Overworld));

      overworldsListBox.Items.Add("");
      overworldsListBox.SelectedIndex = currentEvFile.overworlds.Count - 1;
      updateSelectedOverworldName();
    }

    private void removeOverworldButton_Click(object sender, EventArgs e) {
      if (overworldsListBox.SelectedIndex < 0) {
        return;
      }

      Helpers.DisableHandlers();

      /* Remove overworld object from list and the corresponding entry in the ListBox */
      int owNumber = overworldsListBox.SelectedIndex;
      currentEvFile.overworlds.RemoveAt(owNumber);
      overworldsListBox.Items.RemoveAt(owNumber);

      FillOverworldsBox(); // Update ListBox
      Helpers.EnableHandlers();

      if (owNumber > 0) {
        overworldsListBox.SelectedIndex = owNumber - 1;
      }
      else {
        DisplayActiveEvents();
      }
    }

    private void FillOverworldsBox() {
      overworldsListBox.Items.Clear();
      int count = currentEvFile.overworlds.Count;

      for (int i = 0; i < count; i++) {
        overworldsListBox.Items.Add(i.ToString("D" + Math.Max(0, count - 1).ToString().Length) + ": " + currentEvFile.overworlds[i].ToString());
      }
    }

    private void addOverworldButton_Click(object sender, EventArgs e) {
      int owCount = currentEvFile.overworlds.Count;
      currentEvFile.overworlds.Add(new Overworld(owCount + 1, (int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));

      overworldsListBox.Items.Add("");
      overworldsListBox.SelectedIndex = owCount;
      updateSelectedOverworldName();
    }

    private void overworldsListBox_SelectedIndexChanged(object sender, EventArgs e) {
      int index = overworldsListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || index < 0) {
        return;
      }

      Helpers.DisableHandlers();

      selectedEvent = currentEvFile.overworlds[index];
      Overworld selectedOw = (Overworld)selectedEvent;
      try {
        /* Sprite index and image controls */
        owSpriteComboBox.SelectedIndex = Array.IndexOf(RomInfo.overworldTableKeys, selectedOw.overlayTableEntry);
        owSpritePictureBox.BackgroundImage = GetOverworldImage(selectedOw.overlayTableEntry, selectedOw.orientation);
      }
      catch (ArgumentOutOfRangeException) {
        String errorMsg = "This Overworld's sprite ID couldn't be read correctly.";
        MessageBox.Show(errorMsg, "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      try {
        /* Special settings controls */
        if (selectedOw.type == (ushort)Overworld.OwType.TRAINER) {
          isTrainerRadioButton.Checked = true;
          if (selectedOw.scriptNumber >= 4999) {
            owTrainerComboBox.SelectedIndex = Math.Max(selectedOw.scriptNumber - 4999, 0); // Partner of double battle trainer
            owPartnerTrainerCheckBox.Checked = true;
          }
          else {
            owTrainerComboBox.SelectedIndex = Math.Max(selectedOw.scriptNumber - 2999, 0); // Normal trainer
            owPartnerTrainerCheckBox.Checked = false;
          }
        }
        else if (selectedOw.type == (ushort)Overworld.OwType.ITEM || selectedOw.scriptNumber >= 7000 && selectedOw.scriptNumber <= 8000) {
          isItemRadioButton.Checked = true;
          owItemComboBox.SelectedIndex = Math.Max(selectedOw.scriptNumber - 7000, 0);
        }
        else {
          normalRadioButton.Checked = true;
        }

        /* Set coordinates controls */
        owMapXUpDown.Value = selectedOw.xMapPosition;
        owMapYUpDown.Value = selectedOw.yMapPosition;
        owMatrixXUpDown.Value = selectedOw.xMatrixPosition;
        owMatrixYUpDown.Value = selectedOw.yMatrixPosition;
        owMapZUpDown.Value = selectedOw.zMapPosition;

        /*ID, Flag and Script number controls */
        owIDNumericUpDown.Value = selectedOw.owID;
        owFlagNumericUpDown.Value = selectedOw.flag;
        owScriptNumericUpDown.Value = selectedOw.scriptNumber;

        /* Movement settings */
        owMovementComboBox.SelectedIndex = selectedOw.movement;
        owOrientationComboBox.SelectedIndex = selectedOw.orientation;
        owSightRangeUpDown.Value = selectedOw.sightRange;
        owRangeXUpDown.Value = selectedOw.xRange;
        owRangeYUpDown.Value = selectedOw.yRange;

        try {
          uint spriteID = RomInfo.OverworldTable[currentEvFile.overworlds[overworldsListBox.SelectedIndex].overlayTableEntry].spriteID;
          if (spriteID == 0x3D3D) {
            spriteIDlabel.Text = "3D Overworld";
          }
          else {
            spriteIDlabel.Text = "Sprite ID: " + spriteID;
          }
        }
        catch {
        }

        DisplayActiveEvents();
      }
      catch (ArgumentOutOfRangeException) {
        String errorMsg = "There was a problem loading the overworld events of this Event file.";
        MessageBox.Show(errorMsg, "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      Helpers.EnableHandlers();
    }

    private void duplicateWarpsButton_Click(object sender, EventArgs e) {
      if (warpsListBox.SelectedIndex < 0) {
        return;
      }

      Warp n = new Warp(selectedEvent as Warp);
      currentEvFile.warps.Add(n);

      int index = currentEvFile.warps.Count - 1;

      warpsListBox.Items.Add("");
      warpsListBox.SelectedIndex = index;
      updateSelectedWarpName();

      eventEditorWarpHeaderListBox.SelectedIndex = n.header;
    }

    private void eventEditorWarpHeaderListBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (eventEditorWarpHeaderListBox.SelectedIndex < 0) {
        eventEditorHeaderLocationNameLabel.Text = "";
        return;
      }

      ushort destHeaderID = (ushort)eventEditorWarpHeaderListBox.SelectedIndex;

      MapHeader destHeader;
      if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
        destHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + destHeaderID.ToString("D4"), destHeaderID, 0);
      }
      else {
        destHeader = MapHeader.LoadFromARM9(destHeaderID);
      }

      int locNum;
      switch (RomInfo.gameFamily) {
        case GameFamilies.DP: {
          HeaderDP h = (HeaderDP)destHeader;

          locNum = h.locationName;
          break;
        }
        case GameFamilies.Plat: {
          HeaderPt h = (HeaderPt)destHeader;

          locNum = h.locationName;
          break;
        }
        default: {
          HeaderHGSS h = (HeaderHGSS)destHeader;

          locNum = h.locationName;
          break;
        }
      }

      eventEditorHeaderLocationNameLabel.Text = (string)EditorPanels.headerEditor.locationNameComboBox.Items[locNum];

      if (Helpers.HandlersDisabled) {
        return;
      }

      currentEvFile.warps[warpsListBox.SelectedIndex].header = destHeaderID;
      updateSelectedWarpName();
    }

    private void goToWarpDestination_Click(object sender, EventArgs e) {
      if (warpsListBox.SelectedIndex < 0) {
        return;
      }

      int destAnchor = (int)warpAnchorUpDown.Value;
      ushort destHeaderID = (ushort)eventEditorWarpHeaderListBox.SelectedIndex;

      MapHeader destHeader;
      if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
        destHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + destHeaderID.ToString("D4"), destHeaderID, 0);
      }
      else {
        destHeader = MapHeader.LoadFromARM9(destHeaderID);
      }

      if (new EventFile(destHeader.eventFileID).warps.Count < destAnchor + 1) {
        DialogResult d = MessageBox.Show("The selected warp's destination anchor doesn't exist.\n" +
                                         "Do you want to open the destination map anyway?", "Warp is not connected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (d == DialogResult.No)
          return;
        else {
          eventMatrixUpDown.Value = destHeader.matrixID;
          eventAreaDataUpDown.Value = destHeader.areaDataID;
          selectEventComboBox.SelectedIndex = destHeader.eventFileID;
          CenterEventViewOnEntities();
          return;
        }
      }

      eventMatrixUpDown.Value = destHeader.matrixID;
      eventAreaDataUpDown.Value = destHeader.areaDataID;
      selectEventComboBox.SelectedIndex = destHeader.eventFileID;

      warpsListBox.SelectedIndex = destAnchor;
      centerEventViewOnSelectedEvent_Click(sender, e);
    }

    private void warpAnchorUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled || warpsListBox.SelectedIndex < 0) {
        return;
      }

      currentEvFile.warps[warpsListBox.SelectedIndex].anchor = (ushort)warpAnchorUpDown.Value;
      updateSelectedWarpName();
    }

    private void removeWarpButton_Click(object sender, EventArgs e) {
      if (warpsListBox.SelectedIndex < 0) {
        return;
      }

      Helpers.DisableHandlers();

      /* Remove warp object from list and the corresponding entry in the ListBox */
      int warpNumber = warpsListBox.SelectedIndex;
      currentEvFile.warps.RemoveAt(warpNumber);
      warpsListBox.Items.RemoveAt(warpNumber);

      FillWarpsBox(); // Update ListBox

      Helpers.EnableHandlers();

      if (warpNumber > 0) {
        warpsListBox.SelectedIndex = warpNumber - 1;
      }
      else {
        DisplayActiveEvents();
      }
    }

    private void FillWarpsBox() {
      warpsListBox.Items.Clear();
      int count = currentEvFile.warps.Count;

      for (int i = 0; i < count; i++) {
        warpsListBox.Items.Add(i.ToString("D" + Math.Max(0, count - 1).ToString().Length) + ": " + currentEvFile.warps[i].ToString());
      }
    }

    private void addWarpButton_Click(object sender, EventArgs e) {
      Warp n = new Warp((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
      currentEvFile.warps.Add(n);

      int index = currentEvFile.warps.Count - 1;

      warpsListBox.Items.Add("");
      warpsListBox.SelectedIndex = index;
      updateSelectedWarpName();

      eventEditorWarpHeaderListBox.SelectedIndex = n.header;
    }

    private void warpsListBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled || warpsListBox.SelectedIndex < 0) {
        return;
      }

      selectedEvent = currentEvFile.warps[warpsListBox.SelectedIndex];
      eventEditorWarpHeaderListBox.SelectedIndex = currentEvFile.warps[warpsListBox.SelectedIndex].header;

      Helpers.DisableHandlers();

      warpAnchorUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].anchor;
      warpMapXUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].xMapPosition;
      warpMapYUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].yMapPosition;
      warpMapZUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].zMapPosition;
      warpMatrixXUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].xMatrixPosition;
      warpMatrixYUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].yMatrixPosition;

      DisplayActiveEvents(); // Redraw events to show selection box

      Helpers.EnableHandlers();
    }

    private void triggerVariableWatchedUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].variableWatched = (ushort)triggerVariableWatchedUpDown.Value;
      updateSelectedTriggerName();
    }

    private void triggerExpectedValueUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].expectedVarValue = (ushort)triggerExpectedValueUpDown.Value;
      updateSelectedTriggerName();
    }

    private void triggerScriptUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].scriptNumber = (ushort)triggerScriptUpDown.Value;
      updateSelectedTriggerName();
    }

    private void triggerWidthUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].widthX = (ushort)triggerWidthUpDown.Value;
      DisplayActiveEvents();
    }

    private void triggerLengthUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].heightY = (ushort)triggerLengthUpDown.Value;
      DisplayActiveEvents();
    }

    private void triggerMapXUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].xMapPosition = (short)triggerMapXUpDown.Value;
      DisplayActiveEvents();
    }

    private void triggerMapYUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].yMapPosition = (short)triggerMapYUpDown.Value;
      DisplayActiveEvents();
    }

    private void triggerMapZUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].zPosition = (ushort)triggerMapZUpDown.Value;
      DisplayActiveEvents();
    }

    private void duplicateTriggersButton_Click(object sender, EventArgs e) {
      if (triggersListBox.SelectedIndex < 0) {
        return;
      }

      currentEvFile.triggers.Add(new Trigger(selectedEvent as Trigger));

      triggersListBox.Items.Add("");
      triggersListBox.SelectedIndex = currentEvFile.triggers.Count - 1;
      updateSelectedTriggerName();
    }

    private void FillTriggersBox() {
      triggersListBox.Items.Clear();
      int count = currentEvFile.triggers.Count;

      for (int i = 0; i < count; i++) {
        triggersListBox.Items.Add(i.ToString("D" + Math.Max(0, count - 1).ToString().Length) + ": " + currentEvFile.triggers[i].ToString());
      }
    }

    private void removeTriggerButton_Click(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;

      if (selection < 0) {
        return;
      }

      Helpers.DisableHandlers();

      /* Remove trigger object from list and the corresponding entry in the ListBox */
      currentEvFile.triggers.RemoveAt(selection);
      triggersListBox.Items.RemoveAt(selection);

      FillTriggersBox(); // Update ListBox

      Helpers.EnableHandlers();

      if (selection > 0) {
        triggersListBox.SelectedIndex = selection - 1;
      }
      else {
        DisplayActiveEvents();
      }
    }

    private void addTriggerButton_Click(object sender, EventArgs e) {
      currentEvFile.triggers.Add(new Trigger((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));

      triggersListBox.Items.Add("");
      triggersListBox.SelectedIndex = currentEvFile.triggers.Count - 1;
      updateSelectedTriggerName();
    }

    private void triggersListBox_SelectedIndexChanged(object sender, EventArgs e) {
      int selectedIndex = triggersListBox.SelectedIndex;

      if (Helpers.HandlersDisabled || selectedIndex < 0) {
        return;
      }

      Helpers.DisableHandlers();

      Trigger t = (selectedEvent = currentEvFile.triggers[selectedIndex]) as Trigger;

      triggerScriptUpDown.Value = t.scriptNumber;
      triggerVariableWatchedUpDown.Value = t.variableWatched;
      triggerExpectedValueUpDown.Value = t.expectedVarValue;

      triggerWidthUpDown.Value = t.widthX;
      triggerLengthUpDown.Value = t.heightY;

      triggerMapXUpDown.Value = t.xMapPosition;
      triggerMapYUpDown.Value = t.yMapPosition;
      triggerMapZUpDown.Value = t.zPosition;
      triggerMatrixXUpDown.Value = t.xMatrixPosition;
      triggerMatrixYUpDown.Value = t.yMatrixPosition;

      DisplayActiveEvents();

      Helpers.EnableHandlers();
    }

    private void eventsTabControl_SelectedIndexChanged(object sender, EventArgs e) {
      if (eventsTabControl.SelectedTab == signsTabPage) {
        if (spawnablesListBox.Items.Count > 0) {
          spawnablesListBox.SelectedIndex = 0;
        }
      }
      else if (eventsTabControl.SelectedTab == overworldsTabPage) {
        if (overworldsListBox.Items.Count > 0) {
          overworldsListBox.SelectedIndex = 0;
        }
      }
      else if (eventsTabControl.SelectedTab == warpsTabPage) {
        if (warpsListBox.Items.Count > 0) {
          warpsListBox.SelectedIndex = 0;
        }
      }
      else if (eventsTabControl.SelectedTab == triggersTabPage) {
        if (triggersListBox.Items.Count > 0) {
          triggersListBox.SelectedIndex = 0;
        }
      }
    }

    private void DisplayEventMap(bool readGraphicsFromHeader = true) {
      /* Determine map file to open and open it in BinaryReader, unless map is VOID */
      uint mapIndex = GameMatrix.EMPTY;
      if (eventMatrixXUpDown.Value > eventMatrix.width || eventMatrixYUpDown.Value > eventMatrix.height) {
        String errorMsg = "This event file contains elements located on an unreachable map, beyond the current matrix.\n" +
                          "It is strongly advised that you bring every Overworld, Spawnable, Warp and Trigger of this event to a map that belongs to the matrix's range.";
        MessageBox.Show(errorMsg, "Can't load proper map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      else {
        mapIndex = eventMatrix.maps[(int)(eventMatrixYUpDown.Value), (int)(eventMatrixXUpDown.Value)];
      }

      if (mapIndex == GameMatrix.EMPTY) {
        openGlPictureBox.BackgroundImage = new Bitmap(openGlPictureBox.Width, openGlPictureBox.Height);
        using (Graphics g = Graphics.FromImage(openGlPictureBox.BackgroundImage)) {
          g.Clear(Color.Black);
        }

        currentMapFile = null;
      }
      else {
        /* Determine area data */
        byte areaDataID;
        if (eventMatrix.hasHeadersSection && readGraphicsFromHeader) {
          ushort headerID = (ushort)eventMatrix.headers[(short)eventMatrixYUpDown.Value, (short)eventMatrixXUpDown.Value];
          MapHeader h;
          if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
            h = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerID.ToString("D4"), headerID, 0);
          }
          else {
            h = MapHeader.LoadFromARM9(headerID);
          }

          areaDataID = h.areaDataID;

          Helpers.BackUpDisableHandler();
          Helpers.DisableHandlers();
          eventAreaDataUpDown.Value = h.areaDataID;
          Helpers.RestoreDisableHandler();
        }
        else {
          areaDataID = (byte)eventAreaDataUpDown.Value;
        }

        /* get texture file numbers from area data */
        AreaData areaData = new AreaData(areaDataID);

        /* Read map and building models, match them with textures and render them*/
        currentMapFile = new MapFile((int)mapIndex, RomInfo.gameFamily, discardMoveperms: false);
        Helpers.MW_LoadModelTextures(currentMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, areaData.mapTileset);

        bool isInteriorMap = gameFamily == GameFamilies.HGSS && areaData.areaType == 0x0;

        for (int i = 0; i < currentMapFile.buildings.Count; i++) {
          currentMapFile.buildings[i].LoadModelData(Helpers.romInfo.GetBuildingModelsDirPath(isInteriorMap)); // Load building nsbmd
          Helpers.MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, areaData.buildingsTileset); // Load building textures                
        }

        float ang = 0f;
        float dist = 115.0f;
        float elev = 90f;
        float perspective = 4f;
        bool mapTexturesOn = true;
        bool bldTexturesOn = true;
        Helpers.RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, openGlControl, ang, dist, elev, perspective, mapTexturesOn, bldTexturesOn);
        openGlPictureBox.BackgroundImage = Helpers.GrabMapScreenshot(openGlControl.Width, openGlControl.Height);
      }

      openGlPictureBox.Invalidate();
    }

    private void eventMatrixCoordsUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      eventEditorFullMapReload();
    }

    private void showEventsCheckBoxes_CheckedChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) {
        return;
      }

      DisplayActiveEvents();
      Properties.Settings.Default.renderSpawnables = showSpawnablesCheckBox.Checked;
      Properties.Settings.Default.renderOverworlds = showOwsCheckBox.Checked;
      Properties.Settings.Default.renderWarps = showWarpsCheckBox.Checked;
      Properties.Settings.Default.renderTriggers = showTriggersCheckBox.Checked;
    }

    private void eventAreaDataUpDown_ValueChanged(object sender, EventArgs e) {
      DisplayEventMap(readGraphicsFromHeader: false);
    }

    private void locateCurrentEvFile_Click(object sender, EventArgs e) {
      Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.eventFiles].unpackedDir, selectEventComboBox.SelectedIndex.ToString("D4")));
    }

    private void warpMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled || warpsListBox.SelectedIndex < 0) {
        return;
      }

      currentEvFile.warps[warpsListBox.SelectedIndex].xMatrixPosition = (ushort)warpMatrixXUpDown.Value;
      DisplayActiveEvents();
    }

    private void warpMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled || warpsListBox.SelectedIndex < 0) {
        return;
      }

      currentEvFile.warps[warpsListBox.SelectedIndex].yMatrixPosition = (ushort)warpMatrixYUpDown.Value;
      DisplayActiveEvents();
    }

    private void warpMapXUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled || warpsListBox.SelectedIndex < 0) {
        return;
      }

      currentEvFile.warps[warpsListBox.SelectedIndex].xMapPosition = (short)warpMapXUpDown.Value;
      DisplayActiveEvents();
    }

    private void warpMapYUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled || warpsListBox.SelectedIndex < 0) {
        return;
      }

      currentEvFile.warps[warpsListBox.SelectedIndex].yMapPosition = (short)warpMapYUpDown.Value;
      DisplayActiveEvents();
    }

    private void warpMapZUpDown_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled || warpsListBox.SelectedIndex < 0) {
        return;
      }

      currentEvFile.warps[warpsListBox.SelectedIndex].zMapPosition = (short)warpMapZUpDown.Value;
      DisplayActiveEvents();
    }

    private void triggerMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].xMatrixPosition = (ushort)triggerMatrixXUpDown.Value;
      DisplayActiveEvents();
    }

    private void triggerMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
      int selection = triggersListBox.SelectedIndex;
      if (Helpers.HandlersDisabled || selection < 0) {
        return;
      }

      currentEvFile.triggers[selection].yMatrixPosition = (ushort)triggerMatrixYUpDown.Value;
      DisplayActiveEvents();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using ScintillaNET;
using ScintillaNET.Utils;
using System.Globalization;
using static DSPRE.ROMFiles.Event;

namespace DSPRE.Editors {
  public partial class ScriptEditor : UserControl {
    public bool scriptEditorIsReady { get; set; } = false;
    private ScintillaNET.Scintilla ScriptTextArea;
    private ScintillaNET.Scintilla FunctionTextArea;
    private ScintillaNET.Scintilla ActionTextArea;
    private SearchManager scriptSearchManager;
    private SearchManager functionSearchManager;
    private SearchManager actionSearchManager;
    private Scintilla currentScintillaEditor;
    private SearchManager currentSearchManager;
    private bool scriptsDirty = false;
    private bool functionsDirty = false;
    private bool actionsDirty = false;
    private string cmdKeyWords = "";
    private string secondaryKeyWords = "";
    private ScriptFile currentScriptFile;

    /// <summary>
    /// the background color of the text area
    /// </summary>
    private readonly Color BACK_COLOR = Color.FromArgb(0x2A211C);

    /// <summary>
    /// default text color of the text area
    /// </summary>
    private readonly Color FORE_COLOR = Color.FromArgb(0xB7B7B7);

    /// <summary>
    /// change this to whatever margin you want the line numbers to show in
    /// </summary>
    private const int NUMBER_MARGIN = 1;

    /// <summary>
    /// change this to whatever margin you want the bookmarks/breakpoints to show in
    /// </summary>
    private const int BOOKMARK_MARGIN = 2;

    private const int BOOKMARK_MARKER = 2;

    /// <summary>
    /// change this to whatever margin you want the code folding tree (+/-) to show in
    /// </summary>
    private const int FOLDING_MARGIN = 3;

    /// <summary>
    /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
    /// </summary>
    private const bool CODEFOLDING_CIRCULAR = true;

    public ScriptEditor() {
      InitializeComponent();
    }

    public void SetupScriptEditor() {
      /* Extract essential NARCs sub-archives*/
      Program.MainProgram.statusLabelMessage("Setting up Script Editor...");
      Update();

      DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.scripts }); //12 = scripts Narc Dir

      selectScriptFileComboBox.Items.Clear();
      int scriptCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.scripts].unpackedDir).Length;
      for (int i = 0; i < scriptCount; i++) {
        selectScriptFileComboBox.Items.Add("Script File " + i);
      }

      UpdateScriptNumberCheckBox((NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference);
      selectScriptFileComboBox.SelectedIndex = 0;
      Program.MainProgram.statusLabelMessage();
    }

    public void SetupScriptEditorTextAreas() {
      //PREPARE SCRIPT EDITOR KEYWORDS
      cmdKeyWords = String.Join(" ", ScriptCommandNamesDict.Values) +
                    " " + String.Join(" ", ScriptDatabase.movementsDictIDName.Values);
      cmdKeyWords += " " + cmdKeyWords.ToUpper() + " " + cmdKeyWords.ToLower();

      secondaryKeyWords = String.Join(" ", RomInfo.ScriptComparisonOperatorsDict.Values) +
                          " " + String.Join(" ", ScriptDatabase.specialOverworlds.Values) +
                          " " + String.Join(" ", ScriptDatabase.overworldDirections.Values) +
                          " " + ScriptFile.containerTypes.Script.ToString() +
                          " " + ScriptFile.containerTypes.Function.ToString() +
                          " " + ScriptFile.containerTypes.Action.ToString() +
                          " " + EventType.Overworld +
                          " " + Overworld.MovementCodeKW;
      secondaryKeyWords += " " + secondaryKeyWords.ToUpper() + " " + secondaryKeyWords.ToLower();

      // CREATE CONTROLS
      ScriptTextArea = new ScintillaNET.Scintilla();
      scriptSearchManager = new SearchManager(Program.MainProgram, ScriptTextArea, panelSearchScriptTextBox, PanelSearchScripts);
      scintillaScriptsPanel.Controls.Add(ScriptTextArea);

      FunctionTextArea = new ScintillaNET.Scintilla();
      functionSearchManager = new SearchManager(Program.MainProgram, FunctionTextArea, panelSearchFunctionTextBox, PanelSearchFunctions);
      scintillaFunctionsPanel.Controls.Add(FunctionTextArea);

      ActionTextArea = new ScintillaNET.Scintilla();
      actionSearchManager = new SearchManager(Program.MainProgram, ActionTextArea, panelSearchActionTextBox, PanelSearchActions);
      scintillaActionsPanel.Controls.Add(ActionTextArea);

      currentScintillaEditor = ScriptTextArea;
      currentSearchManager = scriptSearchManager;

      // BASIC CONFIG
      ScriptTextArea.TextChanged += (this.OnTextChangedScript);
      FunctionTextArea.TextChanged += (this.OnTextChangedFunction);
      ActionTextArea.TextChanged += (this.OnTextChangedAction);

      // INITIAL VIEW CONFIG
      InitialViewConfig(ScriptTextArea);
      InitialViewConfig(FunctionTextArea);
      InitialViewConfig(ActionTextArea);

      InitSyntaxColoring(ScriptTextArea);
      InitSyntaxColoring(FunctionTextArea);
      InitSyntaxColoring(ActionTextArea);

      // NUMBER MARGIN
      InitNumberMargin(ScriptTextArea, ScriptTextArea_MarginClick);
      InitNumberMargin(FunctionTextArea, FunctionTextArea_MarginClick);
      InitNumberMargin(ActionTextArea, ActionTextArea_MarginClick);

      // BOOKMARK MARGIN
      InitBookmarkMargin(ScriptTextArea);
      InitBookmarkMargin(FunctionTextArea);
      InitBookmarkMargin(ActionTextArea);

      // CODE FOLDING MARGIN
      InitCodeFolding(ScriptTextArea);
      InitCodeFolding(FunctionTextArea);
      InitCodeFolding(ActionTextArea);

      // INIT HOTKEYS
      InitHotkeys(ScriptTextArea, scriptSearchManager);
      InitHotkeys(FunctionTextArea, functionSearchManager);
      InitHotkeys(ActionTextArea, actionSearchManager);

      // INIT TOOLTIPS DWELLING
      /*
      ScriptTextArea.MouseDwellTime = 300;
      ScriptTextArea.DwellEnd += TextArea_DwellEnd;
      ScriptTextArea.DwellStart += TextArea_DwellStart;

      FunctionTextArea.MouseDwellTime = 300;
      FunctionTextArea.DwellEnd += TextArea_DwellEnd;
      FunctionTextArea.DwellStart += TextArea_DwellStart;
      */
    }

    private void InitialViewConfig(Scintilla textArea) {
      textArea.Dock = DockStyle.Fill;
      textArea.WrapMode = ScintillaNET.WrapMode.Word;
      textArea.IndentationGuides = IndentView.LookBoth;
      textArea.CaretPeriod = 500;
      textArea.CaretForeColor = Color.White;
      textArea.SetSelectionBackColor(true, Color.FromArgb(0x114D9C));
      textArea.WrapIndentMode = WrapIndentMode.Same;
    }

    private void InitSyntaxColoring(Scintilla textArea) {
      // Configure the default style
      textArea.StyleResetDefault();
      textArea.Styles[Style.Default].Font = "Consolas";
      textArea.Styles[Style.Default].Size = 12;
      textArea.Styles[Style.Default].BackColor = Color.FromArgb(0x212121);
      textArea.Styles[Style.Default].ForeColor = Color.FromArgb(0xFFFFFF);
      textArea.StyleClearAll();

      // Configure the lexer styles
      textArea.Styles[Style.Python.Identifier].ForeColor = Color.FromArgb(0xD0DAE2);
      textArea.Styles[Style.Python.CommentLine].ForeColor = Color.FromArgb(0x40BF57);
      textArea.Styles[Style.Python.Number].ForeColor = Color.FromArgb(0xFFFF00);
      textArea.Styles[Style.Python.String].ForeColor = Color.FromArgb(0xFF00FF);
      textArea.Styles[Style.Python.Character].ForeColor = Color.FromArgb(0xE95454);
      textArea.Styles[Style.Python.Operator].ForeColor = Color.FromArgb(0xFFFF00);
      textArea.Styles[Style.Python.Word].ForeColor = Color.FromArgb(0x48A8EE);
      textArea.Styles[Style.Python.Word2].ForeColor = Color.FromArgb(0xF98906);

      textArea.Lexer = Lexer.Python;

      textArea.SetKeywords(0, cmdKeyWords);
      textArea.SetKeywords(1, secondaryKeyWords);
    }

    private void InitNumberMargin(Scintilla textArea, EventHandler<MarginClickEventArgs> textArea_MarginClick) {
      textArea.Styles[Style.LineNumber].BackColor = BACK_COLOR;
      textArea.Styles[Style.LineNumber].ForeColor = FORE_COLOR;
      textArea.Styles[Style.IndentGuide].ForeColor = FORE_COLOR;
      textArea.Styles[Style.IndentGuide].BackColor = BACK_COLOR;

      var nums = textArea.Margins[NUMBER_MARGIN];
      nums.Type = MarginType.Number;
      nums.Sensitive = true;
      nums.Mask = 0;

      textArea.MarginClick += textArea_MarginClick;
    }

    private void InitBookmarkMargin(Scintilla textArea) {
      //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

      var margin = textArea.Margins[BOOKMARK_MARGIN];
      margin.Width = 20;
      margin.Sensitive = true;
      margin.Type = MarginType.Symbol;
      margin.Mask = (1 << BOOKMARK_MARKER);
      //margin.Cursor = MarginCursor.Arrow;

      var marker = textArea.Markers[BOOKMARK_MARKER];
      marker.Symbol = MarkerSymbol.Circle;
      marker.SetBackColor(Color.FromArgb(0xFF003B));
      marker.SetForeColor(Color.FromArgb(0x000000));
      marker.SetAlpha(100);
    }

    private void InitCodeFolding(Scintilla textArea) {
      textArea.SetFoldMarginColor(true, BACK_COLOR);
      textArea.SetFoldMarginHighlightColor(true, BACK_COLOR);

      // Enable code folding
      textArea.SetProperty("fold", "1");
      textArea.SetProperty("fold.compact", "1");

      // Configure a margin to display folding symbols
      textArea.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
      textArea.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
      textArea.Margins[FOLDING_MARGIN].Sensitive = true;
      textArea.Margins[FOLDING_MARGIN].Width = 20;

      // Set colors for all folding markers
      for (int i = 25; i <= 31; i++) {
        textArea.Markers[i].SetForeColor(BACK_COLOR); // styles for [+] and [-]
        textArea.Markers[i].SetBackColor(FORE_COLOR); // styles for [+] and [-]
      }

      // Configure folding markers with respective symbols
      textArea.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
      textArea.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
      textArea.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
      textArea.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
      textArea.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
      textArea.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
      textArea.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

      // Enable automatic folding
      textArea.AutomaticFold = (AutomaticFold.Show|AutomaticFold.Click|AutomaticFold.Change);
    }

    private void InitHotkeys(Scintilla scintillaTb, SearchManager sm) {
      // register the hotkeys with the form
      HotKeyManager.AddHotKey(scintillaTb, sm.OpenSearch, Keys.F, true);
      HotKeyManager.AddHotKey(scintillaTb, () => Uppercase(scintillaTb), Keys.U, true);
      HotKeyManager.AddHotKey(scintillaTb, () => Lowercase(scintillaTb), Keys.L, true);
      HotKeyManager.AddHotKey(scintillaTb, () => ZoomIn(scintillaTb), Keys.Oemplus, true);
      HotKeyManager.AddHotKey(scintillaTb, () => ZoomOut(scintillaTb), Keys.OemMinus, true);
      HotKeyManager.AddHotKey(scintillaTb, () => ZoomDefault(scintillaTb), Keys.D0, true);
      HotKeyManager.AddHotKey(scintillaTb, sm.CloseSearch, Keys.Escape);

      // remove conflicting hotkeys from scintilla
      scintillaTb.ClearCmdKey(Keys.Control|Keys.F);
      scintillaTb.ClearCmdKey(Keys.Control|Keys.R);
      scintillaTb.ClearCmdKey(Keys.Control|Keys.H);
      scintillaTb.ClearCmdKey(Keys.Control|Keys.L);
      scintillaTb.ClearCmdKey(Keys.Control|Keys.U);
    }

    private void Uppercase(Scintilla textArea) {
      // save the selection
      int start = textArea.SelectionStart;
      int end = textArea.SelectionEnd;

      // modify the selected text
      textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToUpper());

      // preserve the original selection
      textArea.SetSelection(start, end);
    }

    private void Lowercase(Scintilla textArea) {
      // save the selection
      int start = textArea.SelectionStart;
      int end = textArea.SelectionEnd;

      // modify the selected text
      textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToLower());

      // preserve the original selection
      textArea.SetSelection(start, end);
    }

    private void ZoomIn(Scintilla textArea) {
      textArea.ZoomIn();
    }

    private void ZoomOut(Scintilla textArea) {
      textArea.ZoomOut();
    }

    private void ZoomDefault(Scintilla textArea) {
      textArea.Zoom = 0;
    }

    private void ScriptEditorSetClean() {
      Helpers.disableHandlers = true;

      scriptsTabPage.Text = ScriptFile.containerTypes.Script.ToString() + "s";
      functionsTabPage.Text = ScriptFile.containerTypes.Function.ToString() + "s";
      actionsTabPage.Text = ScriptFile.containerTypes.Action.ToString() + "s";
      scriptsDirty = functionsDirty = actionsDirty = false;

      Helpers.disableHandlers = false;
    }

    private void OnTextChangedScript(object sender, EventArgs e) {
      ScriptTextArea.Margins[NUMBER_MARGIN].Width = ScriptTextArea.Lines.Count.ToString().Length * 13;
      scriptsDirty = true;
      scriptsTabPage.Text = ScriptFile.containerTypes.Script.ToString() + "s" + "*";
    }

    private void MarginClick(Scintilla textArea, MarginClickEventArgs e) {
      if (e.Margin == BOOKMARK_MARGIN) {
        // Do we have a marker for this line?
        const uint mask = (1 << BOOKMARK_MARKER);
        var line = textArea.Lines[textArea.LineFromPosition(e.Position)];
        if ((line.MarkerGet()&mask) > 0) {
          // Remove existing bookmark
          line.MarkerDelete(BOOKMARK_MARKER);
        }
        else {
          // Add bookmark
          line.MarkerAdd(BOOKMARK_MARKER);
        }
      }
    }

    private void ScriptTextArea_MarginClick(object sender, MarginClickEventArgs e) {
      MarginClick(ScriptTextArea, e);
    }

    private void FunctionTextArea_MarginClick(object sender, MarginClickEventArgs e) {
      MarginClick(FunctionTextArea, e);
    }

    private void ActionTextArea_MarginClick(object sender, MarginClickEventArgs e) {
      MarginClick(ActionTextArea, e);
    }

    private void UpdateScriptNumberCheckBox(NumberStyles toSet) {
      Helpers.disableHandlers = true;
      Properties.Settings.Default.scriptEditorFormatPreference = (int)toSet;

      switch ((NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference) {
        case NumberStyles.None:
          scriptEditorNumberFormatNoPreference.Checked = true;
          break;
        case NumberStyles.HexNumber:
          scriptEditorNumberFormatHex.Checked = true;
          break;
        case NumberStyles.Integer:
          scriptEditorNumberFormatDecimal.Checked = true;
          break;
      }

      Console.WriteLine("changed style to " + Properties.Settings.Default.scriptEditorFormatPreference);
      Helpers.disableHandlers = false;
    }

    private void OnTextChangedFunction(object sender, EventArgs e) {
      FunctionTextArea.Margins[NUMBER_MARGIN].Width = FunctionTextArea.Lines.Count.ToString().Length * 13;
      functionsDirty = true;
      functionsTabPage.Text = ScriptFile.containerTypes.Function.ToString() + "s" + "*";
    }

    private void selectScriptFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      ReloadScript();
    }

    private void OnTextChangedAction(object sender, EventArgs e) {
      ActionTextArea.Margins[NUMBER_MARGIN].Width = ActionTextArea.Lines.Count.ToString().Length * 13;
      actionsDirty = true;
      actionsTabPage.Text = ScriptFile.containerTypes.Action.ToString() + "s" + "*";
    }

    private bool ReloadScript() {
      Console.WriteLine("Script Reload has been requested");
      /* clear controls */
      if (Helpers.disableHandlers || selectScriptFileComboBox.SelectedIndex < 0) {
        return false;
      }

      if (scriptsDirty || functionsDirty || actionsDirty) {
        DialogResult d = MessageBox.Show("There are unsaved changes in this Script File.\nDo you wish to discard them?", "Unsaved work", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (!d.Equals(DialogResult.Yes)) {
          Helpers.disableHandlers = true;
          selectScriptFileComboBox.SelectedIndex = (int)currentScriptFile.fileID;
          Helpers.disableHandlers = false;
          return false;
        }
      }

      currentScriptFile = new ScriptFile(selectScriptFileComboBox.SelectedIndex); // Load script file

      ScriptTextArea.ClearAll();
      FunctionTextArea.ClearAll();
      ActionTextArea.ClearAll();

      scriptsNavListbox.Items.Clear();
      functionsNavListbox.Items.Clear();
      actionsNavListbox.Items.Clear();

      if (currentScriptFile.isLevelScript) {
        ScriptTextArea.Text += "LevelScript files are currently not supported.\nYou can use AdAstra's Level Scripts Editor.";
        addScriptFileButton.Visible = false;
        removeScriptFileButton.Visible = false;

        clearCurrentLevelScriptButton.Visible = true;
      }
      else {
        Helpers.disableHandlers = true;
        addScriptFileButton.Visible = true;
        removeScriptFileButton.Visible = true;

        clearCurrentLevelScriptButton.Visible = false;

        string buffer = "";

        /* Add scripts */
        for (int i = 0; i < currentScriptFile.allScripts.Count; i++) {
          CommandContainer currentScript = currentScriptFile.allScripts[i];

          /* Write header */
          string header = ScriptFile.containerTypes.Script.ToString() + " " + (i + 1);
          buffer += header + ':' + Environment.NewLine;
          scriptsNavListbox.Items.Add(header);

          /* If current script is identical to another, print UseScript instead of commands */
          if (currentScript.usedScript < 0) {
            for (int j = 0; j < currentScript.commands.Count; j++) {
              if (!ScriptDatabase.endCodes.Contains(currentScript.commands[j].id)) {
                buffer += '\t';
              }

              buffer += currentScript.commands[j].name + Environment.NewLine;
            }
          }
          else {
            buffer += '\t' + "UseScript_#" + currentScript.usedScript + Environment.NewLine;
          }

          ScriptTextArea.AppendText(buffer + Environment.NewLine);
          buffer = "";
        }

        /* Add functions */
        for (int i = 0; i < currentScriptFile.allFunctions.Count; i++) {
          CommandContainer currentFunction = currentScriptFile.allFunctions[i];

          /* Write Heaader */
          string header = ScriptFile.containerTypes.Function.ToString() + " " + (i + 1);
          buffer += header + ':' + Environment.NewLine;
          functionsNavListbox.Items.Add(header);

          /* If current function is identical to a script, print UseScript instead of commands */
          if (currentFunction.usedScript < 0) {
            for (int j = 0; j < currentFunction.commands.Count; j++) {
              if (!ScriptDatabase.endCodes.Contains(currentFunction.commands[j].id)) {
                buffer += '\t';
              }

              buffer += currentFunction.commands[j].name + Environment.NewLine;
            }
          }
          else {
            buffer += '\t' + "UseScript_#" + currentFunction.usedScript + Environment.NewLine;
          }

          FunctionTextArea.AppendText(buffer + Environment.NewLine);
          buffer = "";
        }

        /* Add movements */
        for (int i = 0; i < currentScriptFile.allActions.Count; i++) {
          ActionContainer currentAction = currentScriptFile.allActions[i];

          string header = ScriptFile.containerTypes.Action.ToString() + " " + (i + 1);
          buffer += header + ':' + Environment.NewLine;
          actionsNavListbox.Items.Add(header);

          for (int j = 0; j < currentAction.actionCommandsList.Count; j++) {
            if (currentAction.actionCommandsList[j].id != 0x00FE) {
              buffer += '\t';
            }

            buffer += currentAction.actionCommandsList[j].name + Environment.NewLine;
          }

          ActionTextArea.AppendText(buffer + Environment.NewLine);
          buffer = "";
        }
      }

      ScriptEditorSetClean();
      Program.MainProgram.statusLabelMessage();
      Helpers.disableHandlers = false;
      return true;
    }

    private void BtnNextSearchScript_Click(object sender, EventArgs e) {
      scriptSearchManager.Find(true, false);
    }

    private void BtnPrevSearchScript_Click(object sender, EventArgs e) {
      scriptSearchManager.Find(false, false);
    }

    private void BtnCloseSearchScript_Click(object sender, EventArgs e) {
      scriptSearchManager.CloseSearch();
    }

    private void panelSearchScriptTextBox_TextChanged(object sender, EventArgs e) {
      scriptSearchManager.Find(true, true);
    }

    private void scriptTxtSearch_KeyDown(object sender, KeyEventArgs e) {
      TxtSearchKeyDown(scriptSearchManager, e);
    }

    private void TxtSearchKeyDown(SearchManager sm, KeyEventArgs e) {
      if (HotKeyManager.IsHotkey(e, Keys.Enter)) {
        sm.Find(true, false);
      }

      if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true)) {
        sm.Find(false, false);
      }
    }

    private void BtnNextSearchFunc_Click(object sender, EventArgs e) {
      functionSearchManager.Find(true, false);
    }

    private void BtnPrevSearchFunc_Click(object sender, EventArgs e) {
      functionSearchManager.Find(false, false);
    }

    private void BtnCloseSearchFunc_Click(object sender, EventArgs e) {
      functionSearchManager.CloseSearch();
    }

    private void panelSearchFunctionTextBox_TextChanged(object sender, EventArgs e) {
      functionSearchManager.Find(true, true);
    }

    private void functionTxtSearch_KeyDown(object sender, KeyEventArgs e) {
      TxtSearchKeyDown(functionSearchManager, e);
    }

    private void BtnNextSearchActions_Click(object sender, EventArgs e) {
      actionSearchManager.Find(true, false);
    }

    private void BtnPrevSearchActions_Click(object sender, EventArgs e) {
      actionSearchManager.Find(false, false);
    }

    private void BtnCloseSearchActions_Click(object sender, EventArgs e) {
      actionSearchManager.CloseSearch();
    }

    private void panelSearchActionTextBox_TextChanged(object sender, EventArgs e) {
      actionSearchManager.Find(true, true);
    }

    private void actiontTxtSearch_KeyDown(object sender, KeyEventArgs e) {
      TxtSearchKeyDown(actionSearchManager, e);
    }

    private void scriptEditorZoomInButton_Click(object sender, EventArgs e) {
      ZoomIn(currentScintillaEditor);
    }

    private void scriptEditorZoomOutButton_Click(object sender, EventArgs e) {
      ZoomOut(currentScintillaEditor);
    }

    private void scriptEditorZoomResetButton_Click(object sender, EventArgs e) {
      ZoomDefault(currentScintillaEditor);
    }

    private void scriptEditorTabControl_TabIndexChanged(object sender, EventArgs e) {
      if (scriptEditorTabControl.SelectedTab == scriptsTabPage) {
        currentSearchManager = scriptSearchManager;
        currentScintillaEditor = ScriptTextArea;
      }
      else if (scriptEditorTabControl.SelectedTab == functionsTabPage) {
        currentSearchManager = functionSearchManager;
        currentScintillaEditor = FunctionTextArea;
      }
      else {
        //Actions
        currentSearchManager = actionSearchManager;
        currentScintillaEditor = ActionTextArea;
      }
    }

    private void addScriptFileButton_Click(object sender, EventArgs e) {
      /* Add new event file to event folder */
      string scriptFilePath = RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + selectScriptFileComboBox.Items.Count.ToString("D4");
      File.WriteAllBytes(scriptFilePath, new ScriptFile(0).ToByteArray());

      /* Update ComboBox and select new file */
      selectScriptFileComboBox.Items.Add("Script File " + selectScriptFileComboBox.Items.Count);
      selectScriptFileComboBox.SelectedIndex = selectScriptFileComboBox.Items.Count - 1;
    }

    private void removeScriptFileButton_Click(object sender, EventArgs e) {
      DialogResult d = MessageBox.Show("Are you sure you want to delete the last Script File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
      if (d.Equals(DialogResult.Yes)) {
        /* Delete script file */
        File.Delete(RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + (selectScriptFileComboBox.Items.Count - 1).ToString("D4"));

        /* Check if currently selected file is the last one, and in that case select the one before it */
        int lastIndex = selectScriptFileComboBox.Items.Count - 1;
        if (selectScriptFileComboBox.SelectedIndex == lastIndex) {
          selectScriptFileComboBox.SelectedIndex--;
        }

        /* Remove item from ComboBox */
        selectScriptFileComboBox.Items.RemoveAt(lastIndex);
      }
    }

    private void saveScriptFileButton_Click(object sender, EventArgs e) {
      /* Create new ScriptFile object */
      int idToAssign = selectScriptFileComboBox.SelectedIndex;

      ScriptFile userEdited = new ScriptFile(
        scriptLines: ScriptTextArea.Lines.ToStringsList(trim: true),
        functionLines: FunctionTextArea.Lines.ToStringsList(trim: true),
        actionLines: ActionTextArea.Lines.ToStringsList(trim: true),
        selectScriptFileComboBox.SelectedIndex
      );

      /* Write new scripts to file */
      if (userEdited.fileID == null) {
        MessageBox.Show("This " + typeof(ScriptFile).Name + " couldn't be saved, due to a processing error.", "Can't save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      else if (userEdited.fileID == int.MaxValue) {
        MessageBox.Show("This " + typeof(ScriptFile).Name + " is couldn't be saved since it's empty.", "Can't save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      else {
        //check if ScriptFile instance was created succesfully
        if (userEdited.SaveToFileDefaultDir(selectScriptFileComboBox.SelectedIndex)) {
          currentScriptFile = userEdited;
          ScriptEditorSetClean();
        }
      }
    }

    private void exportScriptFileButton_Click(object sender, EventArgs e) {
      string suggestion = "Script File ";
      if (currentScriptFile.isLevelScript) {
        suggestion = "Level " + suggestion;
      }

      currentScriptFile.SaveToFileExplorePath(suggestion + selectScriptFileComboBox.SelectedIndex, blindmode: true);
    }

    private void importScriptFileButton_Click(object sender, EventArgs e) {
      /* Prompt user to select .scr or .bin file */
      OpenFileDialog of = new OpenFileDialog {
        Filter = "Script File (*.scr, *.bin)|*.scr;*.bin"
      };
      if (of.ShowDialog(this) != DialogResult.OK) {
        return;
      }

      /* Update scriptFile object in memory */
      string path = RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + selectScriptFileComboBox.SelectedIndex.ToString("D4");
      File.Copy(of.FileName, path, true);

      /* Refresh controls */
      selectScriptFileComboBox_SelectedIndexChanged(null, null);

      /* Display success message */
      MessageBox.Show("Scripts imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void searchInScriptsTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        searchInScriptsButton_Click(null, null);
      }
    }

    private void searchInScriptsButton_Click(object sender, EventArgs e) {
      if (searchInScriptsTextBox.Text == "") {
        return;
      }

      int firstArchive;
      int lastArchive;

      if (searchOnlyCurrentScriptCheckBox.Checked) {
        firstArchive = selectScriptFileComboBox.SelectedIndex;
        lastArchive = firstArchive + 1;
      }
      else {
        firstArchive = 0;
        lastArchive = Helpers.romInfo.GetScriptCount();
      }

      searchInScriptsResultListBox.Items.Clear();
      string searchString = searchInScriptsTextBox.Text;
      searchProgressBar.Maximum = selectScriptFileComboBox.Items.Count;

      List<string> results = new List<string>();

      string scriptKw = ScriptFile.containerTypes.Script.ToString();
      string functionKw = ScriptFile.containerTypes.Function.ToString();

      for (int i = firstArchive; i < lastArchive; i++) {
        try {
          Console.WriteLine("Attempting to load script " + i);
          ScriptFile file = new ScriptFile(i, readActions: false);

          if (scriptSearchCaseSensitiveCheckBox.Checked) {
            results.AddRange(SearchInScripts(i, file.allScripts, scriptKw, (string s) => s.Contains(searchString)));
            results.AddRange(SearchInScripts(i, file.allFunctions, functionKw, (string s) => s.Contains(searchString)));
          }
          else {
            results.AddRange(SearchInScripts(i, file.allScripts, scriptKw, (string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0));
            results.AddRange(SearchInScripts(i, file.allFunctions, functionKw, (string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0));
          }
        }
        catch {
        }

        searchProgressBar.Value = i;
      }

      searchProgressBar.Value = 0;
      searchInScriptsResultListBox.Items.AddRange(results.ToArray());
    }

    private List<string> SearchInScripts(int fileID, List<CommandContainer> cmdList, string entryType, Func<string, bool> criteria) {
      List<string> results = new List<string>();

      for (int j = 0; j < cmdList.Count; j++) {
        if (cmdList[j].commands is null) {
          continue;
        }

        foreach (ScriptCommand cur in cmdList[j].commands) {
          if (criteria(cur.name)) {
            results.Add($"File {fileID} - {entryType} {j + 1}: {cur.name}{Environment.NewLine}");
          }
        }
      }

      return results;
    }

    private void searchInScriptsResultListBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        searchInScripts_GoToEntryResult(null, null);
      }
    }

    private void searchInScripts_GoToEntryResult(object sender, MouseEventArgs e) {
      if (searchInScriptsResultListBox.SelectedIndex < 0) {
        return;
      }

      string[] split = searchInScriptsResultListBox.SelectedItem.ToString().Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      selectScriptFileComboBox.SelectedIndex = int.Parse(split[1]);
      string cmdNameAndParams = String.Join(" ", split.Skip(5).Take(split.Length - 5));

      if (split[3].StartsWith(ScriptFile.containerTypes.Script.ToString())) {
        if (scriptEditorTabControl.SelectedTab != scriptsTabPage) {
          scriptEditorTabControl.SelectedTab = scriptsTabPage;
        }

        scriptSearchManager.Find(true, false, ScriptFile.containerTypes.Script.ToString() + " " + split[4].Replace(":", ""));
        scriptSearchManager.Find(true, false, cmdNameAndParams);
      }
      else if (split[3].StartsWith(ScriptFile.containerTypes.Function.ToString())) {
        if (scriptEditorTabControl.SelectedTab != functionsTabPage) {
          scriptEditorTabControl.SelectedTab = functionsTabPage;
        }

        functionSearchManager.Find(true, false, ScriptFile.containerTypes.Function.ToString() + " " + split[4].Replace(":", ""));
        functionSearchManager.Find(true, false, cmdNameAndParams);
      }
      else if (split[3].StartsWith(ScriptFile.containerTypes.Action.ToString())) {
        if (scriptEditorTabControl.SelectedTab != actionsTabPage) {
          scriptEditorTabControl.SelectedTab = actionsTabPage;
        }

        actionSearchManager.Find(true, false, ScriptFile.containerTypes.Action.ToString() + " " + split[4].Replace(":", ""));
        actionSearchManager.Find(true, false, cmdNameAndParams);
      }
    }

    private void NavigatorGoTo(ListBox currentLB, int indexToSwitchTo, SearchManager entrusted, string keyword) {
      if (currentLB.SelectedIndex < 0) {
        return;
      }

      if (scriptEditorTabControl.SelectedIndex != indexToSwitchTo) {
        scriptEditorTabControl.SelectedIndex = indexToSwitchTo;
      }

      entrusted.Find(true, false, keyword + ' ' + (currentLB.SelectedIndex + 1) + ':');
    }

    private void scriptsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
      NavigatorGoTo((ListBox)sender, 0, scriptSearchManager, ScriptFile.containerTypes.Script.ToString());
    }

    private void functionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
      NavigatorGoTo((ListBox)sender, 1, functionSearchManager, ScriptFile.containerTypes.Function.ToString());
    }

    private void actionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
      NavigatorGoTo((ListBox)sender, 2, actionSearchManager, ScriptFile.containerTypes.Action.ToString());
    }

    private void openSearchScriptEditorButton_Click(object sender, EventArgs e) {
      currentSearchManager.OpenSearch();
    }

    private void ScriptEditorExpandButton_Click(object sender, EventArgs e) {
      currentScintillaEditor.FoldAll(FoldAction.Expand);
    }

    private void ScriptEditorCollapseButton_Click(object sender, EventArgs e) {
      currentScintillaEditor.FoldAll(FoldAction.Contract);
    }

    private void scriptEditorWordWrapCheckbox_CheckedChanged(object sender, EventArgs e) {
      ScriptTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
      FunctionTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
      ActionTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
    }

    private void viewWhiteSpacesButton_Click(object sender, EventArgs e) {
      ScriptTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
      FunctionTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
      ActionTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
    }

    private void UpdateScriptNumberFormatNoPref(object sender, EventArgs e) {
      if (!Helpers.disableHandlers && scriptEditorNumberFormatNoPreference.Checked) {
        NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
        Properties.Settings.Default.scriptEditorFormatPreference = (int)NumberStyles.None;

        if (!ReloadScript()) {
          UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
        }
      }
    }

    private void UpdateScriptNumberFormatDec(object sender, EventArgs e) {
      if (!Helpers.disableHandlers && scriptEditorNumberFormatDecimal.Checked) {
        NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
        Properties.Settings.Default.scriptEditorFormatPreference = (int)NumberStyles.Integer;

        if (!ReloadScript()) {
          UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
        }
      }
    }

    private void UpdateScriptNumberFormatHex(object sender, EventArgs e) {
      if (!Helpers.disableHandlers && scriptEditorNumberFormatHex.Checked) {
        NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
        Properties.Settings.Default.scriptEditorFormatPreference = (int)NumberStyles.HexNumber;

        if (!ReloadScript()) {
          UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
        }
      }
    }

    private void clearCurrentLevelScriptButton_Click(object sender, EventArgs e) {
      string path = RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + selectScriptFileComboBox.SelectedIndex.ToString("D4");
      File.WriteAllBytes(path, new byte[4]);
      MessageBox.Show("Level script correctly cleared.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void locateCurrentScriptFile_Click(object sender, EventArgs e) {
      Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.scripts].unpackedDir, selectScriptFileComboBox.SelectedIndex.ToString("D4")));
    }
  }
}

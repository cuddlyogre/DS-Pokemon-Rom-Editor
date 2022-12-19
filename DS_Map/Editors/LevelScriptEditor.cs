using System;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;

namespace DSPRE.Editors {
  public partial class LevelScriptEditor : UserControl {
    public bool levelScriptEditorIsReady { get; set; } = false;
    LevelScriptFile _levelScriptFile;

    public LevelScriptEditor() {
      InitializeComponent();
    }

    public void SetUpLevelScriptEditor() {
      populate_selectScriptFileComboBox();
    }

    private void populate_selectScriptFileComboBox() {
      selectScriptFileComboBox.Items.Clear();
      int scriptCount = Directory.GetFiles(gameDirs[RomInfo.DirNames.scripts].unpackedDir).Length;
      for (int i = 0; i < scriptCount; i++) {
        // ScriptFile currentScriptFile = new ScriptFile(i, true, true);
        // selectScriptFileComboBox.Items.Add(currentScriptFile);
        selectScriptFileComboBox.Items.Add($"Script File {i}");
      }
    }

    private void selectScriptFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (selectScriptFileComboBox.SelectedIndex == -1) return;
      listBoxTriggers.DataSource = null;
      radioButtonVariableValue.Checked = false;
      radioButtonMapChange.Checked = false;
      radioButtonScreenReset.Checked = false;
      radioButtonLoadGame.Checked = false;
      try {
        _levelScriptFile = new LevelScriptFile(selectScriptFileComboBox.SelectedIndex);
        listBoxTriggers.DataSource = _levelScriptFile.bufferSet;
      }
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
      }
    }

    private void buttonSave_Click(object sender, EventArgs e) {
      doSave(String.IsNullOrWhiteSpace(saveFileDialog1.FileName));
    }

    private void buttonSaveAs_Click(object sender, EventArgs e) {
      doSave(true);
    }

    void doSave(bool saveAs) {
      if (saveAs) {
        try {
          saveFileDialog1.InitialDirectory = Path.GetDirectoryName(saveFileDialog1.FileName);
          saveFileDialog1.FileName = Path.GetFileName(saveFileDialog1.FileName);
        }
        catch (Exception ex) {
          saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Environment.SpecialFolder.UserProfile.ToString());
          saveFileDialog1.FileName = Path.GetFileName(saveFileDialog1.FileName);
        }

        if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
          saveFile(saveFileDialog1.FileName);
        }
      }
      else {
        saveFile(saveFileDialog1.FileName);
      }
    }

    void saveFile(string path) {
      try {
        long bytes_written = _levelScriptFile.write_file(path);
        if (bytes_written <= 4) {
          MessageBox.Show("Empty level script file was correctly saved.", "Success!");
        }
        else {
          MessageBox.Show("File was correctly saved.", "Success!");
        }
      }
      catch (Exception ex) {
        MessageBox.Show(ex.Message, ex.GetType().ToString());
      }
    }

    private void listBoxTriggers_SelectedIndexChanged(object sender, EventArgs e) {
      textBoxScriptID.Clear();
      textBoxVariableName.Clear();
      textBoxVariableValue.Clear();

      if (listBoxTriggers.SelectedItem == null) {
        buttonOpenScript.Enabled = false;
        buttonRemove.Enabled = false;
        return;
      }

      if (listBoxTriggers.SelectedItem is MapScreenLoadTrigger mapScreenLoadTrigger) {
        if (mapScreenLoadTrigger.triggerType == LevelScriptTrigger.LOADGAME) {
          radioButtonLoadGame.Checked = true;
        }
        else if (mapScreenLoadTrigger.triggerType == LevelScriptTrigger.MAPCHANGE) {
          radioButtonMapChange.Checked = true;
        }
        else if (mapScreenLoadTrigger.triggerType == LevelScriptTrigger.SCREENRESET) {
          radioButtonScreenReset.Checked = true;
        }
      }
      else if (listBoxTriggers.SelectedItem is VariableValueTrigger variableValueTrigger) {
        if (variableValueTrigger.triggerType == LevelScriptTrigger.VARIABLEVALUE) {
          radioButtonVariableValue.Checked = true;
        }
      }

      handleAutoFormat();
      handleHexFormat();
      handleDecimalFormat();

      buttonOpenScript.Enabled = true;
      buttonRemove.Enabled = true;
    }

    private void handleAutoFormat() {
      if (!radioButtonAuto.Checked) return;

      textBoxScriptID.Clear();
      textBoxVariableName.Clear();
      textBoxVariableValue.Clear();

      if (listBoxTriggers.SelectedItem is MapScreenLoadTrigger mapScreenLoadTrigger) {
        textBoxScriptID.Text = mapScreenLoadTrigger.scriptTriggered.ToString();
      }
      else if (listBoxTriggers.SelectedItem is VariableValueTrigger variableValueTrigger) {
        textBoxScriptID.Text = variableValueTrigger.scriptTriggered.ToString();
        textBoxVariableName.Text = "" + variableValueTrigger.variableToWatch.ToString("D");
        textBoxVariableValue.Text = "" + variableValueTrigger.expectedValue.ToString("D");
      }
    }

    private void handleHexFormat() {
      if (!radioButtonHex.Checked) return;

      textBoxScriptID.Clear();
      textBoxVariableName.Clear();
      textBoxVariableValue.Clear();

      if (listBoxTriggers.SelectedItem is MapScreenLoadTrigger mapScreenLoadTrigger) {
        textBoxScriptID.Text = mapScreenLoadTrigger.scriptTriggered.ToString();
      }
      else if (listBoxTriggers.SelectedItem is VariableValueTrigger variableValueTrigger) {
        textBoxScriptID.Text = variableValueTrigger.scriptTriggered.ToString();
        textBoxVariableName.Text = "0x" + variableValueTrigger.variableToWatch.ToString("X");
        textBoxVariableValue.Text = "0x" + variableValueTrigger.expectedValue.ToString("X");
      }
    }

    private void handleDecimalFormat() {
      if (!radioButtonDecimal.Checked) return;

      textBoxScriptID.Clear();
      textBoxVariableName.Clear();
      textBoxVariableValue.Clear();

      if (listBoxTriggers.SelectedItem is MapScreenLoadTrigger mapScreenLoadTrigger) {
        textBoxScriptID.Text = mapScreenLoadTrigger.scriptTriggered.ToString();
      }
      else if (listBoxTriggers.SelectedItem is VariableValueTrigger variableValueTrigger) {
        textBoxScriptID.Text = variableValueTrigger.scriptTriggered.ToString();
        textBoxVariableName.Text = "" + variableValueTrigger.variableToWatch.ToString("D");
        textBoxVariableValue.Text = "" + variableValueTrigger.expectedValue.ToString("D");
      }
    }

    private void radioButtonAuto_CheckedChanged(object sender, EventArgs e) {
      handleAutoFormat();
    }

    private void radioButtonHex_CheckedChanged(object sender, EventArgs e) {
      handleHexFormat();
    }

    private void radioButtonDecimal_CheckedChanged(object sender, EventArgs e) {
      handleDecimalFormat();
    }

    private void radioButtonVariableValue_CheckedChanged(object sender, EventArgs e) {
      textBoxVariableName.Enabled = true;
      textBoxVariableValue.Enabled = true;
    }

    private void radioButtonMapChange_CheckedChanged(object sender, EventArgs e) {
      textBoxVariableName.Enabled = false;
      textBoxVariableValue.Enabled = false;
    }

    private void radioButtonScreenReset_CheckedChanged(object sender, EventArgs e) {
      textBoxVariableName.Enabled = false;
      textBoxVariableValue.Enabled = false;
    }

    private void radioButtonLoadGame_CheckedChanged(object sender, EventArgs e) {
      textBoxVariableName.Enabled = false;
      textBoxVariableValue.Enabled = false;
    }

    private void buttonAdd_Click(object sender, EventArgs e) {
      int convertBase = 10; //decimal
      if (radioButtonHex.Checked) {
        convertBase = 16; //hex
      }

      try {
        if (radioButtonVariableValue.Checked) {
          int scriptID = Convert.ToInt16(textBoxScriptID.Text, convertBase);
          int variableName = Convert.ToInt16(textBoxVariableName.Text, convertBase);
          int variableValue = Convert.ToInt16(textBoxVariableValue.Text, convertBase);
          VariableValueTrigger variableValueTrigger = new VariableValueTrigger(scriptID, variableName, variableValue);
          _levelScriptFile.bufferSet.Add(variableValueTrigger);
        }
        else {
          int scriptID = Convert.ToInt16(textBoxScriptID.Text, convertBase);
          if (radioButtonMapChange.Checked) {
            MapScreenLoadTrigger mapScreenLoadTrigger = new MapScreenLoadTrigger(LevelScriptTrigger.MAPCHANGE, scriptID);
            _levelScriptFile.bufferSet.Add(mapScreenLoadTrigger);
          }
          else if (radioButtonScreenReset.Checked) {
            MapScreenLoadTrigger mapScreenLoadTrigger = new MapScreenLoadTrigger(LevelScriptTrigger.SCREENRESET, scriptID);
            _levelScriptFile.bufferSet.Add(mapScreenLoadTrigger);
          }
          else if (radioButtonLoadGame.Checked) {
            MapScreenLoadTrigger mapScreenLoadTrigger = new MapScreenLoadTrigger(LevelScriptTrigger.LOADGAME, scriptID);
            _levelScriptFile.bufferSet.Add(mapScreenLoadTrigger);
          }
        }
      }
      catch (Exception exception) {
        MessageBox.Show(exception.Message);
      }
    }

    private void buttonRemove_Click(object sender, EventArgs e) {
      _levelScriptFile.bufferSet.RemoveAt(listBoxTriggers.SelectedIndex);
    }

    private void buttonOpenScript_Click(object sender, EventArgs e) {
      if (!EditorPanels.scriptEditor.scriptEditorIsReady) {
        EditorPanels.scriptEditor.SetupScriptEditor();
        EditorPanels.scriptEditor.scriptEditorIsReady = true;
      }

      EditorPanels.scriptEditor.scriptEditorTabControl.SelectedIndex = 0;
      EditorPanels.scriptEditor.selectScriptFileComboBox.SelectedIndex = (int)EditorPanels.headerEditor.scriptFileUpDown.Value; //((LevelScriptTrigger)listBoxTriggers.SelectedItem).scriptTriggered;
      EditorPanels.mainTabControl.SelectedTab = EditorPanels.scriptEditorTabPage;
    }
  }
}

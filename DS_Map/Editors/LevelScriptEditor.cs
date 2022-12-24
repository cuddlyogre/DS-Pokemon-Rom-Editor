using System;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE {
  public partial class LevelScriptEditor : UserControl {
    LevelScriptFile _levelScriptFile;

    public LevelScriptEditor() {
      InitializeComponent();
    }

    private void buttonLoad_Click(object sender, System.EventArgs e) {
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

        //path = @"unpacked\scripts\0000"; //not a level script
        //path = @"unpacked\scripts\0266"; //valid
        //path = @"unpacked\scripts\0267"; //does nothing
        //path = @"unpacked\scripts\0472"; //valid
        //path = @"unpacked\scripts\0474"; //valid
        //path = @"unpacked\scripts\0505"; //valid
        string path = "";
        path = openFileDialog1.FileName;

        _levelScriptFile = new LevelScriptFile();
        try {
          _levelScriptFile.parse_file(path);
          listBoxTriggers.DataSource = _levelScriptFile.bufferSet;
          // listBoxTriggers.DisplayMember = "QWER";
          // listBoxTriggers.ValueMember = "this";
        }
        catch (Exception ex) {
          MessageBox.Show(ex.Message, ex.GetType().ToString());
        }
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
          saveFile();
        }
      }
      else {
        saveFile();
      }
    }

    void saveFile() {
      textBoxPath.Text = saveFileDialog1.FileName;

      string path = "";
      path = saveFileDialog1.FileName;

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
  }
}

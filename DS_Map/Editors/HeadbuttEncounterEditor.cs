using System;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors
{
  public partial class HeadbuttEncounterEditor : UserControl {
    HeadbuttEncounterFile currentHeadbuttEncounterFile;

    public HeadbuttEncounterEditor() {
      InitializeComponent();
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

      headbuttEncounterEditorTabNormal.listBoxEncounters.DataSource =  currentHeadbuttEncounterFile.normalEncounters;
      headbuttEncounterEditorTabSpecial.listBoxEncounters.DataSource =  currentHeadbuttEncounterFile.specialEncounters;

      listBoxNormalEncounters.DataSource = currentHeadbuttEncounterFile.normalEncounters;
      listBoxSpecialEncounters.DataSource = currentHeadbuttEncounterFile.specialEncounters;

      listBoxNormalTreeGroups.DataSource = currentHeadbuttEncounterFile.normalTreeGroups;
      listBoxSpecialTreeGroups.DataSource = currentHeadbuttEncounterFile.specialTreeGroups;

      listBoxNormalTrees.DataSource = null;
      listBoxSpecialTrees.DataSource = null;
    }

    private void listBoxNormalEncounters_SelectedIndexChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxNormalEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      textBoxNormalPokemonID.Text = headbuttEncounter.pokemonID.ToString();
      numericUpDownNormalMinLevel.Value = headbuttEncounter.minLevel;
      numericUpDownNormalMaxLevel.Value = headbuttEncounter.maxLevel;
    }

    private void textBoxNormalPokemonID_TextChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxNormalEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      headbuttEncounter.pokemonID = ushort.Parse(textBoxNormalPokemonID.Text);
      listBoxNormalEncounters.RefreshItem(listBoxNormalEncounters.SelectedIndex);
    }

    private void numericUpDownNormalMinLevel_ValueChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxNormalEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      headbuttEncounter.minLevel = (byte)numericUpDownNormalMinLevel.Value;
      listBoxNormalEncounters.RefreshItem(listBoxNormalEncounters.SelectedIndex);
    }

    private void numericUpDownNormalMaxLevel_ValueChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxNormalEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      headbuttEncounter.maxLevel = (byte)numericUpDownNormalMaxLevel.Value;
      listBoxNormalEncounters.RefreshItem(listBoxNormalEncounters.SelectedIndex);
    }

    private void listBoxSpecialEncounters_SelectedIndexChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxSpecialEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      textBoxSpecialPokemonID.Text = headbuttEncounter.pokemonID.ToString();
      numericUpDownSpecialMinLevel.Value = headbuttEncounter.minLevel;
      numericUpDownSpecialMaxLevel.Value = headbuttEncounter.maxLevel;
    }

    private void textBoxSpecialPokemonID_TextChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxSpecialEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      headbuttEncounter.pokemonID = ushort.Parse(textBoxSpecialPokemonID.Text);
      listBoxSpecialEncounters.RefreshItem(listBoxSpecialEncounters.SelectedIndex);
    }

    private void numericUpDownSpecialMinLevel_ValueChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxSpecialEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      headbuttEncounter.minLevel = (byte)numericUpDownSpecialMinLevel.Value;
      listBoxSpecialEncounters.RefreshItem(listBoxSpecialEncounters.SelectedIndex);
    }

    private void numericUpDownSpecialMaxLevel_ValueChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxSpecialEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      headbuttEncounter.maxLevel = (byte)numericUpDownSpecialMaxLevel.Value;
      listBoxSpecialEncounters.RefreshItem(listBoxSpecialEncounters.SelectedIndex);
    }

    private void listBoxNormalTreeGroups_SelectedIndexChanged(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxNormalTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null) return;
      listBoxNormalTrees.DataSource = headbuttTreeGroup.trees;
    }

    private void buttonAddNormalTreeGroup_Click(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxNormalTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null) return;
      currentHeadbuttEncounterFile.AddNormalTreeGroup();
    }

    private void buttonRemoveNormalTreeGroup_Click(object sender, EventArgs e) {
      int selectedIndex = listBoxNormalTreeGroups.SelectedIndex;
      if (selectedIndex == -1) return;
      currentHeadbuttEncounterFile.RemoveNormalTreeGroup(selectedIndex);
    }

    private void buttonDuplicateNormalTreeGroup_Click(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxNormalTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null) return;
      currentHeadbuttEncounterFile.DuplicateNormalTreeGroup(headbuttTreeGroup);
    }

    private void listBoxNormalTrees_SelectedIndexChanged(object sender, EventArgs e) {
      //HeadbuttTree headbuttTree = (HeadbuttTree)((ListBox)sender).SelectedItem;
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxNormalTrees.SelectedItem;
      if (headbuttTree == null) return;
      numericUpDownNormalTreeGlobalX.Value = headbuttTree.globalX;
      numericUpDownNormalTreeGlobalY.Value = headbuttTree.globalY;
      numericUpDownNormalTreeMatrixX.Value = headbuttTree.matrixX;
      numericUpDownNormalTreeMatrixY.Value = headbuttTree.matrixY;
      numericUpDownNormalTreeMapX.Value = headbuttTree.mapX;
      numericUpDownNormalTreeMapY.Value = headbuttTree.mapY;
    }

    private void numericUpDownNormalTreeGlobalX_ValueChanged(object sender, EventArgs e) {
      // ((HeadbuttTree)listBoxTrees.SelectedItem).globalX = (ushort)((NumericUpDown)sender).Value;
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxNormalTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.globalX = (ushort)((NumericUpDown)sender).Value;
      listBoxNormalTrees.RefreshItem(listBoxNormalTrees.SelectedIndex);
      // listBoxTrees.Items[listBoxTrees.SelectedIndex] = listBoxTrees.SelectedItem;
    }

    private void numericUpDownNormalTreeGlobalY_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxNormalTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.globalY = (ushort)((NumericUpDown)sender).Value;
      listBoxNormalTrees.RefreshItem(listBoxNormalTrees.SelectedIndex);
      // listBoxTrees.Items[listBoxTrees.SelectedIndex] = listBoxTrees.SelectedItem;
    }

    private void numericUpDownNormalTreeMatrixX_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxNormalTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.matrixX = (ushort)((NumericUpDown)sender).Value;
      listBoxNormalTrees.RefreshItem(listBoxNormalTrees.SelectedIndex);
    }

    private void numericUpDownNormalTreeMatrixY_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxNormalTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.matrixY = (ushort)((NumericUpDown)sender).Value;
      listBoxNormalTrees.RefreshItem(listBoxNormalTrees.SelectedIndex);
    }

    private void numericUpDownNormalTreeMapX_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxNormalTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.mapX = (ushort)((NumericUpDown)sender).Value;
      listBoxNormalTrees.RefreshItem(listBoxNormalTrees.SelectedIndex);
    }

    private void numericUpDownNormalTreeMapY_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxNormalTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.mapY = (ushort)((NumericUpDown)sender).Value;
      listBoxNormalTrees.RefreshItem(listBoxNormalTrees.SelectedIndex);
    }

    private void listBoxSpecialTreeGroups_SelectedIndexChanged(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxSpecialTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null) return;
      listBoxSpecialTrees.DataSource = headbuttTreeGroup.trees;
    }

    private void buttonAddSpecialTreeGroup_Click(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxSpecialTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null) return;
      currentHeadbuttEncounterFile.AddSpecialTreeGroup();
    }

    private void buttonRemoveSpecialTreeGroup_Click(object sender, EventArgs e) {
      int selectedIndex = listBoxSpecialTreeGroups.SelectedIndex;
      if (selectedIndex == -1) return;
      currentHeadbuttEncounterFile.RemoveSpecialTreeGroup(selectedIndex);
    }

    private void buttonDuplicateSpecialTreeGroup_Click(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxSpecialTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null) return;
      currentHeadbuttEncounterFile.DuplicateSpecialTreeGroup(headbuttTreeGroup);
    }

    private void listBoxSpecialTrees_SelectedIndexChanged(object sender, EventArgs e) {
      //HeadbuttTree headbuttTree = (HeadbuttTree)((ListBox)sender).SelectedItem;
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxSpecialTrees.SelectedItem;
      if (headbuttTree == null) return;
      numericUpDownSpecialTreeGlobalX.Value = headbuttTree.globalX;
      numericUpDownSpecialTreeGlobalY.Value = headbuttTree.globalY;
      numericUpDownSpecialTreeMatrixX.Value = headbuttTree.matrixX;
      numericUpDownSpecialTreeMatrixY.Value = headbuttTree.matrixY;
      numericUpDownSpecialTreeMapX.Value = headbuttTree.mapX;
      numericUpDownSpecialTreeMapY.Value = headbuttTree.mapY;
    }

    private void numericUpDownSpecialTreeGlobalX_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxSpecialTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.globalX = (ushort)((NumericUpDown)sender).Value;
      listBoxSpecialTrees.RefreshItem(listBoxSpecialTrees.SelectedIndex);
    }

    private void numericUpDownSpecialTreeGlobalY_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxSpecialTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.globalY = (ushort)((NumericUpDown)sender).Value;
      listBoxSpecialTrees.RefreshItem(listBoxSpecialTrees.SelectedIndex);
    }

    private void numericUpDownSpecialTreeMatrixX_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxSpecialTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.matrixX = (ushort)((NumericUpDown)sender).Value;
      listBoxSpecialTrees.RefreshItem(listBoxSpecialTrees.SelectedIndex);
    }

    private void numericUpDownSpecialTreeMatrixY_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxSpecialTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.matrixY = (ushort)((NumericUpDown)sender).Value;
      listBoxSpecialTrees.RefreshItem(listBoxSpecialTrees.SelectedIndex);
    }

    private void numericUpDownSpecialTreeMapX_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxSpecialTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.mapX = (ushort)((NumericUpDown)sender).Value;
      listBoxSpecialTrees.RefreshItem(listBoxSpecialTrees.SelectedIndex);
    }

    private void numericUpDownSpecialTreeMapY_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxSpecialTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.mapY = (ushort)((NumericUpDown)sender).Value;
      listBoxSpecialTrees.RefreshItem(listBoxSpecialTrees.SelectedIndex);
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

using System;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors {
  public partial class HeadbuttEncounterEditorTab : UserControl {
    public HeadbuttEncounterEditorTab() {
      InitializeComponent();
    }

    private void listBoxEncounters_SelectedIndexChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      textBoxPokemonID.Text = headbuttEncounter.pokemonID.ToString();
      numericUpDownMinLevel.Value = headbuttEncounter.minLevel;
      numericUpDownMaxLevel.Value = headbuttEncounter.maxLevel;
    }

    private void textBoxPokemonID_TextChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      headbuttEncounter.pokemonID = ushort.Parse(textBoxPokemonID.Text);
      listBoxEncounters.RefreshItem(listBoxEncounters.SelectedIndex);
    }

    private void numericUpDownMinLevel_ValueChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      headbuttEncounter.minLevel = (byte)numericUpDownMinLevel.Value;
      listBoxEncounters.RefreshItem(listBoxEncounters.SelectedIndex);
    }

    private void numericUpDownMaxLevel_ValueChanged(object sender, EventArgs e) {
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxEncounters.SelectedItem;
      if (headbuttEncounter == null) return;
      headbuttEncounter.maxLevel = (byte)numericUpDownMaxLevel.Value;
      listBoxEncounters.RefreshItem(listBoxEncounters.SelectedIndex);
    }

    private void listBoxTreeGroups_SelectedIndexChanged(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null) return;
      listBoxTrees.DataSource = headbuttTreeGroup.trees;
    }

    private void buttonAddTreeGroup_Click(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null) return;
      listBoxTreeGroups.Items.Add(new HeadbuttTreeGroup(HeadbuttTree.Types.Normal));
    }

    private void buttonRemoveTreeGroup_Click(object sender, EventArgs e) {
      int selectedIndex = listBoxTreeGroups.SelectedIndex;
      if (selectedIndex == -1) return;
      listBoxTreeGroups.Items.RemoveAt(selectedIndex);
    }

    private void buttonDuplicateTreeGroup_Click(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null) return;
      listBoxTreeGroups.Items.Add(new HeadbuttTreeGroup(headbuttTreeGroup));
    }

    private void listBoxTrees_SelectedIndexChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxTrees.SelectedItem;
      if (headbuttTree == null) return;
      numericUpDownTreeGlobalX.Value = headbuttTree.globalX;
      numericUpDownTreeGlobalY.Value = headbuttTree.globalY;
      numericUpDownTreeMatrixX.Value = headbuttTree.matrixX;
      numericUpDownTreeMatrixY.Value = headbuttTree.matrixY;
      numericUpDownTreeMapX.Value = headbuttTree.mapX;
      numericUpDownTreeMapY.Value = headbuttTree.mapY;
    }

    private void numericUpDownTreeGlobalX_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.globalX = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeGlobalY_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.globalY = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeMatrixX_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.matrixX = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeMatrixY_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.matrixY = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeMapX_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.mapX = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }

    private void numericUpDownTreeMapY_ValueChanged(object sender, EventArgs e) {
      HeadbuttTree headbuttTree = (HeadbuttTree)listBoxTrees.SelectedItem;
      if (headbuttTree == null) return;
      headbuttTree.mapY = (ushort)((NumericUpDown)sender).Value;
      listBoxTrees.RefreshItem(listBoxTrees.SelectedIndex);
    }
  }
}

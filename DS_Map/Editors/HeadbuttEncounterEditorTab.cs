using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors {
  public partial class HeadbuttEncounterEditorTab : UserControl {
    private List<HeadbuttEncounter> encounters;
    private BindingList<HeadbuttTreeGroup> treeGroups;

    public HeadbuttEncounterEditorTab() {
      InitializeComponent();
    }

    public void Reset() {
      Helpers.DisableHandlers();
      listBoxEncounters.DataSource = null;
      listBoxTreeGroups.DataSource = null;
      listBoxTrees.DataSource = null;
      comboBoxPokemon.SelectedIndex = 0;
      numericUpDownMinLevel.Value = 0;
      numericUpDownMaxLevel.Value = 0;
      Helpers.EnableHandlers();
    }

    public void SetHeadbuttEncounter(List<HeadbuttEncounter> encounters, BindingList<HeadbuttTreeGroup> treeGroups) {
      Helpers.DisableHandlers();
      this.encounters = encounters;
      this.treeGroups = treeGroups;
      listBoxEncounters.DataSource = this.encounters;
      listBoxTreeGroups.DataSource = this.treeGroups;
      listBoxEncounters.SelectedIndex = -1;
      listBoxTreeGroups.SelectedIndex = -1;
      Helpers.EnableHandlers();
    }

    private void listBoxEncounters_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxEncounters.SelectedItem;
      if (headbuttEncounter == null){ return; }
      comboBoxPokemon.SelectedIndex = headbuttEncounter.pokemonID;
      numericUpDownMinLevel.Value = headbuttEncounter.minLevel;
      numericUpDownMaxLevel.Value = headbuttEncounter.maxLevel;
    }
    
    private void comboBoxPokemon_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (Helpers.HandlersDisabled){ return; }
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxEncounters.SelectedItem;
      if (headbuttEncounter == null){ return; }
      headbuttEncounter.pokemonID = (ushort)comboBoxPokemon.SelectedIndex;
      listBoxEncounters.RefreshItem(listBoxEncounters.SelectedIndex);
    }

    private void numericUpDownMinLevel_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxEncounters.SelectedItem;
      if (headbuttEncounter == null){ return; }
      headbuttEncounter.minLevel = (byte)numericUpDownMinLevel.Value;
      listBoxEncounters.RefreshItem(listBoxEncounters.SelectedIndex);
    }

    private void numericUpDownMaxLevel_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      HeadbuttEncounter headbuttEncounter = (HeadbuttEncounter)listBoxEncounters.SelectedItem;
      if (headbuttEncounter == null){ return; }
      headbuttEncounter.maxLevel = (byte)numericUpDownMaxLevel.Value;
      listBoxEncounters.RefreshItem(listBoxEncounters.SelectedIndex);
    }

    private void listBoxTreeGroups_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null){ return; }
      listBoxTrees.DataSource = headbuttTreeGroup.trees;
    }

    private void buttonAddTreeGroup_Click(object sender, EventArgs e) {
      treeGroups.Add(new HeadbuttTreeGroup());
    }

    private void buttonRemoveTreeGroup_Click(object sender, EventArgs e) {
      int selectedIndex = listBoxTreeGroups.SelectedIndex;
      if (selectedIndex == -1){ return; }
      treeGroups.RemoveAt(selectedIndex);
    }

    private void buttonDuplicateTreeGroup_Click(object sender, EventArgs e) {
      HeadbuttTreeGroup headbuttTreeGroup = (HeadbuttTreeGroup)listBoxTreeGroups.SelectedItem;
      if (headbuttTreeGroup == null){ return; }
      treeGroups.Add(new HeadbuttTreeGroup(headbuttTreeGroup));
    }
  }
}

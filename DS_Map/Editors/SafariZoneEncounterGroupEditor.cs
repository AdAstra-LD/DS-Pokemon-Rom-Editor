using System;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors {
  public partial class SafariZoneEncounterGroupEditor : UserControl {
    private SafariZoneEncounterGroup safariZoneEncounterGroup;

    public SafariZoneEncounterGroupEditor() {
      InitializeComponent();
      safariZoneEncounterEditorMorningTab.listBoxEncountersObject.SelectedIndexChanged += ListBoxEncountersObject_SelectedIndexChanged;
      safariZoneEncounterEditorDayTab.listBoxEncountersObject.SelectedIndexChanged += ListBoxEncountersObject_SelectedIndexChanged;
      safariZoneEncounterEditorNightTab.listBoxEncountersObject.SelectedIndexChanged += ListBoxEncountersObject_SelectedIndexChanged;
      listBoxObjectRequirements.SelectedIndexChanged += ListBoxEncountersObject_SelectedIndexChanged;
      listBoxObjectOptionalRequirements.SelectedIndexChanged += ListBoxEncountersObject_SelectedIndexChanged;
    }

    private void ListBoxEncountersObject_SelectedIndexChanged(object sender, EventArgs e) {
      try {
        ListBox2 s = sender as ListBox2;
        safariZoneEncounterEditorMorningTab.listBoxEncountersObject.SelectedIndex = s.SelectedIndex;
        safariZoneEncounterEditorDayTab.listBoxEncountersObject.SelectedIndex = s.SelectedIndex;
        safariZoneEncounterEditorNightTab.listBoxEncountersObject.SelectedIndex = s.SelectedIndex;
        listBoxObjectRequirements.SelectedIndex = s.SelectedIndex;
        listBoxObjectOptionalRequirements.SelectedIndex = s.SelectedIndex;
      }
      catch (Exception ex) {
      }
    }

    public void SetPokemonNames() {
      string[] pokemonNames = RomInfo.GetPokemonNames();
      safariZoneEncounterEditorMorningTab.comboBoxPokemon.Items.AddRange(pokemonNames);
      safariZoneEncounterEditorMorningTab.comboBoxPokemonObject.Items.AddRange(pokemonNames);
      safariZoneEncounterEditorDayTab.comboBoxPokemon.Items.AddRange(pokemonNames);
      safariZoneEncounterEditorDayTab.comboBoxPokemonObject.Items.AddRange(pokemonNames);
      safariZoneEncounterEditorNightTab.comboBoxPokemon.Items.AddRange(pokemonNames);
      safariZoneEncounterEditorNightTab.comboBoxPokemonObject.Items.AddRange(pokemonNames);

      foreach (string type in SafariZoneObjectRequirement.ObjectTypes.Values) {
        comboBoxObjectRequirementType.Items.Add(type);
        comboBoxOptionalObjectRequirementType.Items.Add(type);
      }
    }

    public void Reset() {
      this.safariZoneEncounterGroup = null;
      safariZoneEncounterEditorMorningTab.listBoxEncounters.DataSource = null;
      safariZoneEncounterEditorDayTab.listBoxEncounters.DataSource = null;
      safariZoneEncounterEditorNightTab.listBoxEncounters.DataSource = null;

      safariZoneEncounterEditorMorningTab.listBoxEncountersObject.DataSource = null;
      safariZoneEncounterEditorDayTab.listBoxEncountersObject.DataSource = null;
      safariZoneEncounterEditorNightTab.listBoxEncountersObject.DataSource = null;
      listBoxObjectRequirements.DataSource = null;
      listBoxObjectOptionalRequirements.DataSource = null;
    }

    public void SetData(SafariZoneEncounterGroup safariZoneEncounterGroup) {
      this.safariZoneEncounterGroup = safariZoneEncounterGroup;
      safariZoneEncounterEditorMorningTab.listBoxEncounters.DataSource = this.safariZoneEncounterGroup.MorningEncounters;
      safariZoneEncounterEditorDayTab.listBoxEncounters.DataSource = this.safariZoneEncounterGroup.DayEncounters;
      safariZoneEncounterEditorNightTab.listBoxEncounters.DataSource = this.safariZoneEncounterGroup.NightEncounters;

      safariZoneEncounterEditorMorningTab.listBoxEncountersObject.DataSource = this.safariZoneEncounterGroup.MorningEncountersObject;
      safariZoneEncounterEditorDayTab.listBoxEncountersObject.DataSource = this.safariZoneEncounterGroup.DayEncountersObject;
      safariZoneEncounterEditorNightTab.listBoxEncountersObject.DataSource = this.safariZoneEncounterGroup.NightEncountersObject;
      listBoxObjectRequirements.DataSource = this.safariZoneEncounterGroup.ObjectRequirements;
      listBoxObjectOptionalRequirements.DataSource = this.safariZoneEncounterGroup.OptionalObjectRequirements;
    }

    private void listBoxObjectRequirements_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      SafariZoneObjectRequirement safariZoneObjectRequirement = (SafariZoneObjectRequirement)listBoxObjectRequirements.SelectedItem;
      if (safariZoneObjectRequirement == null){ return; }
      comboBoxObjectRequirementType.SelectedIndex = safariZoneObjectRequirement.typeID;
      numericUpDownObjectRequirementQty.Value = safariZoneObjectRequirement.quantity;
    }

    private void comboBoxObjectRequirementType_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      // if (comboBoxObjectRequirementType.SelectedIndex == 0){ comboBoxObjectRequirementType.SelectedIndex = 1; } //no requirement is not valid
      SafariZoneObjectRequirement safariZoneObjectRequirement = (SafariZoneObjectRequirement)listBoxObjectRequirements.SelectedItem;
      if (safariZoneObjectRequirement == null){ return; }
      safariZoneObjectRequirement.typeID = (byte)comboBoxObjectRequirementType.SelectedIndex;
      listBoxObjectRequirements.RefreshItem(listBoxObjectRequirements.SelectedIndex);
    }

    private void numericUpDownObjectRequirementQty_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      SafariZoneObjectRequirement safariZoneObjectRequirement = (SafariZoneObjectRequirement)listBoxObjectRequirements.SelectedItem;
      if (safariZoneObjectRequirement == null){ return; }
      safariZoneObjectRequirement.quantity = (byte)numericUpDownObjectRequirementQty.Value;
      listBoxObjectRequirements.RefreshItem(listBoxObjectRequirements.SelectedIndex);
    }

    private void listBoxObjectOptionalRequirements_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      SafariZoneObjectRequirement safariZoneObjectRequirement = (SafariZoneObjectRequirement)listBoxObjectOptionalRequirements.SelectedItem;
      if (safariZoneObjectRequirement == null){ return; }
      comboBoxOptionalObjectRequirementType.SelectedIndex = safariZoneObjectRequirement.typeID;
      numericUpDownObjectOptionalRequirementQty.Value = safariZoneObjectRequirement.quantity;
    }

    private void comboBoxOptionalObjectRequirementType_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      SafariZoneObjectRequirement safariZoneObjectRequirement = (SafariZoneObjectRequirement)listBoxObjectOptionalRequirements.SelectedItem;
      if (safariZoneObjectRequirement == null){ return; }
      safariZoneObjectRequirement.typeID = (byte)comboBoxOptionalObjectRequirementType.SelectedIndex;
      listBoxObjectOptionalRequirements.RefreshItem(listBoxObjectOptionalRequirements.SelectedIndex);
    }

    private void numericUpDownObjectOptionalRequirementQty_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled){ return; }
      SafariZoneObjectRequirement safariZoneObjectRequirement = (SafariZoneObjectRequirement)listBoxObjectOptionalRequirements.SelectedItem;
      if (safariZoneObjectRequirement == null){ return; }
      safariZoneObjectRequirement.quantity = (byte)numericUpDownObjectOptionalRequirementQty.Value;
      listBoxObjectOptionalRequirements.RefreshItem(listBoxObjectOptionalRequirements.SelectedIndex);
    }

    private void buttonAddObjectEncounter_Click(object sender, EventArgs e) {
      if (this.safariZoneEncounterGroup == null){ return; }
      if (listBoxObjectOptionalRequirements.SelectedIndex == -1){ return; }
      safariZoneEncounterGroup.MorningEncountersObject.Add(new SafariZoneEncounter());
      safariZoneEncounterGroup.DayEncountersObject.Add(new SafariZoneEncounter());
      safariZoneEncounterGroup.NightEncountersObject.Add(new SafariZoneEncounter());
      safariZoneEncounterGroup.ObjectRequirements.Add(new SafariZoneObjectRequirement(1, 1));
      safariZoneEncounterGroup.OptionalObjectRequirements.Add(new SafariZoneObjectRequirement(0, 0));
      safariZoneEncounterGroup.ObjectSlots = (byte)safariZoneEncounterGroup.ObjectRequirements.Count; //all the list counts should be the same
    }

    private void buttonRemoveObjectEncounter_Click(object sender, EventArgs e) {
      if (this.safariZoneEncounterGroup == null){ return; }
      if (listBoxObjectOptionalRequirements.SelectedIndex == -1){ return; }
      safariZoneEncounterGroup.MorningEncountersObject.RemoveAt(safariZoneEncounterGroup.MorningEncountersObject.Count - 1);
      safariZoneEncounterGroup.DayEncountersObject.RemoveAt(safariZoneEncounterGroup.DayEncountersObject.Count - 1);
      safariZoneEncounterGroup.NightEncountersObject.RemoveAt(safariZoneEncounterGroup.NightEncountersObject.Count - 1);
      safariZoneEncounterGroup.ObjectRequirements.RemoveAt(safariZoneEncounterGroup.ObjectRequirements.Count - 1);
      safariZoneEncounterGroup.OptionalObjectRequirements.RemoveAt(safariZoneEncounterGroup.OptionalObjectRequirements.Count - 1);
      safariZoneEncounterGroup.ObjectSlots = (byte)safariZoneEncounterGroup.ObjectRequirements.Count; //all the list counts should be the same
    }
  }
}

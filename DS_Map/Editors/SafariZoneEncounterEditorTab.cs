using System;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors {
  public partial class SafariZoneEncounterEditorTab : UserControl {
    public SafariZoneEncounterEditorTab() {
      InitializeComponent();
    }
    
    private void listBoxEncounters_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) return;
      SafariZoneEncounter safariZoneEncounter = (SafariZoneEncounter)listBoxEncounters.SelectedItem;
      if (safariZoneEncounter == null) return;
      comboBoxPokemon.SelectedIndex = safariZoneEncounter.pokemonID;
      numericUpDownLevel.Value = safariZoneEncounter.level;
    }

    private void comboBoxPokemon_SelectedIndexChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) return;
      SafariZoneEncounter safariZoneEncounter = (SafariZoneEncounter)listBoxEncounters.SelectedItem;
      if (safariZoneEncounter == null) return;
      safariZoneEncounter.pokemonID = (ushort)comboBoxPokemon.SelectedIndex;
      listBoxEncounters.RefreshItem(listBoxEncounters.SelectedIndex);
    }

    private void numericUpDownLevel_ValueChanged(object sender, EventArgs e) {
      if (Helpers.HandlersDisabled) return;
      SafariZoneEncounter safariZoneEncounter = (SafariZoneEncounter)listBoxEncounters.SelectedItem;
      if (safariZoneEncounter == null) return;
      safariZoneEncounter.level = (byte)numericUpDownLevel.Value;
      listBoxEncounters.RefreshItem(listBoxEncounters.SelectedIndex);
    }

    private void listBoxEncountersObject_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (Helpers.HandlersDisabled) return;
      SafariZoneEncounter safariZoneEncounter = (SafariZoneEncounter)listBoxEncountersObject.SelectedItem;
      if (safariZoneEncounter == null) return;

      comboBoxPokemonObject.SelectedIndex = safariZoneEncounter.pokemonID;
      numericUpDownLevelObject.Value = safariZoneEncounter.level;
    }

    private void comboBoxPokemonObject_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (Helpers.HandlersDisabled) return;
      SafariZoneEncounter safariZoneEncounter = (SafariZoneEncounter)listBoxEncountersObject.SelectedItem;
      if (safariZoneEncounter == null) return;
      safariZoneEncounter.pokemonID = (ushort)comboBoxPokemonObject.SelectedIndex;
      listBoxEncountersObject.RefreshItem(listBoxEncountersObject.SelectedIndex);
    }

    private void numericUpDownLevelObject_ValueChanged(object sender, EventArgs e)
    {
      if (Helpers.HandlersDisabled) return;
      SafariZoneEncounter safariZoneEncounter = (SafariZoneEncounter)listBoxEncountersObject.SelectedItem;
      if (safariZoneEncounter == null) return;
      safariZoneEncounter.level = (byte)numericUpDownLevelObject.Value;
      listBoxEncountersObject.RefreshItem(listBoxEncountersObject.SelectedIndex);
    }
  }
}

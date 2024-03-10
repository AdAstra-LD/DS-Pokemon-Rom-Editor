using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE {
    public partial class PokemonEditor : Form {
        PersonalDataEditor personalEditor;
        LearnsetEditor learnsetEditor;
        EvolutionsEditor evoEditor;

        public PokemonEditor(string[] itemNames, string[] abilityNames, string[] moveNames) {
            InitializeComponent();
            IsMdiContainer = true;

            personalEditor = new PersonalDataEditor(itemNames, abilityNames, personalPage, this);
            personalEditor.TopLevel = false;
            personalEditor.Show();
            personalPage.Controls.Add(personalEditor);

            learnsetEditor = new LearnsetEditor(moveNames, learnsetPage, this);
            learnsetEditor.TopLevel = false;
            learnsetEditor.Show();
            learnsetPage.Controls.Add(learnsetEditor);

            evoEditor = new EvolutionsEditor(evoPage, this);
            evoEditor.TopLevel = false;
            evoEditor.Show();
            evoPage.Controls.Add(evoEditor);
        }

        public void TrySyncIndices(ComboBox sender) {
            if(!syncChangesCheckbox.Checked) {
                return;
            }

            Helpers.BackUpDisableHandler();
            //Helpers.DisableHandlers();
            personalEditor.pokemonNameInputComboBox.SelectedIndex = sender.SelectedIndex;
            learnsetEditor.pokemonNameInputComboBox.SelectedIndex = sender.SelectedIndex;
            evoEditor.pokemonNameInputComboBox.SelectedItem = sender.SelectedItem;

            Helpers.RestoreDisableHandler();
        }
    }
}

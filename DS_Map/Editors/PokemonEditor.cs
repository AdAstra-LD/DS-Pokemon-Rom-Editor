using DSPRE.Editors;
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
        PokemonSpriteEditor spriteEditor;

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

            spriteEditor = new PokemonSpriteEditor(spritePage, this);
            spriteEditor.TopLevel = false;
            spriteEditor.Show();
            spritePage.Controls.Add(spriteEditor);
        }

        public void TrySyncIndices(ComboBox sender) {
            if(!syncChangesCheckbox.Checked) {
                return;
            }

            Helpers.BackUpDisableHandler();
            Helpers.DisableHandlers();
            if (personalEditor.CheckDiscardChanges()) {
                personalEditor.pokemonNameInputComboBox.SelectedIndex = sender.SelectedIndex;
                personalEditor.monNumberNumericUpDown.Value = sender.SelectedIndex;
                personalEditor.ChangeLoadedFile(sender.SelectedIndex);
            }
            if (learnsetEditor.CheckDiscardChanges()) {
                learnsetEditor.pokemonNameInputComboBox.SelectedIndex = sender.SelectedIndex;
                learnsetEditor.monNumberNumericUpDown.Value = sender.SelectedIndex;
                learnsetEditor.ChangeLoadedFile(sender.SelectedIndex);
            }
            if (evoEditor.CheckDiscardChanges()) {
                evoEditor.pokemonNameInputComboBox.SelectedIndex = sender.SelectedIndex;
                evoEditor.monNumberNumericUpDown.Value = sender.SelectedIndex;
                evoEditor.ChangeLoadedFile(sender.SelectedIndex);
            }               
            Helpers.RestoreDisableHandler();
        }

        public void TrySyncIndices(NumericUpDown sender) {
            if (!syncChangesCheckbox.Checked) {
                return;
            }

            Helpers.BackUpDisableHandler();
            Helpers.DisableHandlers();
            if (personalEditor.CheckDiscardChanges()) {
                personalEditor.pokemonNameInputComboBox.SelectedIndex = (int)sender.Value;
                personalEditor.monNumberNumericUpDown.Value = sender.Value;
                personalEditor.ChangeLoadedFile((int)sender.Value);
            }
            if (learnsetEditor.CheckDiscardChanges()) {
                learnsetEditor.pokemonNameInputComboBox.SelectedIndex = (int)sender.Value;
                learnsetEditor.monNumberNumericUpDown.Value = sender.Value;
                learnsetEditor.ChangeLoadedFile((int)sender.Value);
            }
            if (evoEditor.CheckDiscardChanges()) {
                evoEditor.pokemonNameInputComboBox.SelectedIndex = (int)sender.Value;
                evoEditor.monNumberNumericUpDown.Value = sender.Value;
                evoEditor.ChangeLoadedFile((int)sender.Value);
            }
            Helpers.RestoreDisableHandler();
        }
    }
}

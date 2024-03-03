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
        public PokemonEditor(string[] itemNames, string[] abilityNames, string[] moveNames) {
            InitializeComponent();
            IsMdiContainer = true;

            PersonalDataEditor personalEditor = new PersonalDataEditor(itemNames, abilityNames, evoPage);
            personalEditor.TopLevel = false;
            personalEditor.Show();
            personalPage.Controls.Add(personalEditor);

            LearnsetEditor learnsetEditor = new LearnsetEditor(moveNames, evoPage);
            learnsetEditor.TopLevel = false;
            learnsetEditor.Show();
            learnsetPage.Controls.Add(learnsetEditor);

            EvolutionsEditor evoEditor = new EvolutionsEditor(evoPage);
            evoEditor.TopLevel = false;
            evoEditor.Show();
            evoPage.Controls.Add(evoEditor);
        }
    }
}

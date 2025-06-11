using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE
{
    public partial class DVCalc : Form
    {

        private TrainerFile trainerFile;
        private TrainerProperties trainerProp;
        private List<Label> pokeLabels = new List<Label>();
        private List<ComboBox> abilityCombos = new List<ComboBox>();
        private List<ComboBox> genderCombos = new List<ComboBox>();
        private List<NumericUpDown> upDownsDV = new List<NumericUpDown>();
        private List<Label> natureLabels = new List<Label>();
        private List<Button> changeButtons = new List<Button>();
        private List<Button> showAllButtons = new List<Button>();

        private readonly string[] abilitySelection = { "No Flag", "Force Ability 1", "Force Ability 2" };
        private readonly string[] genderSelection = { "No Flag", "Force Male", "Force Female" };
        private bool listsSetup = false;


        public DVCalc(TrainerFile trainerFile)
        {
            Helpers.DisableHandlers();
            InitializeComponent();

            this.trainerFile = trainerFile;
            this.trainerProp = trainerFile.trp;

            labelTrainerName.Text = $"Trainer Name: [{trainerProp.trainerID}] " + RomInfo.GetSimpleTrainerNames()[trainerProp.trainerID];
            labelTrainerClass.Text = $"Trainer Class: [{trainerProp.trainerClass}] " + RomInfo.GetTrainerClassNames()[trainerProp.trainerClass];

            if (DVCalculator.TrainerClassGender.GetTrainerClassGender(trainerProp.trainerClass))
                radioMale.Checked = true;
            else
                radioFemale.Checked = true;

            SetupLists();
            UpdateNatures();
            Helpers.EnableHandlers();

        }

        private void SetupLists()
        {
            pokeLabels.Clear();
            abilityCombos.Clear();
            genderCombos.Clear();
            upDownsDV.Clear();
            natureLabels.Clear();
            changeButtons.Clear();
            showAllButtons.Clear();

            pokeLabels.AddRange(new Label[] { pokeLabel1, pokeLabel2, pokeLabel3, pokeLabel4, pokeLabel5, pokeLabel6});
            abilityCombos.AddRange(new ComboBox[] { comboAbility1, comboAbility2, comboAbility3, comboAbility4, comboAbility5, comboAbility6});
            genderCombos.AddRange(new ComboBox[] { comboGender1, comboGender2, comboGender3, comboGender4, comboGender5, comboGender6 });
            upDownsDV.AddRange(new NumericUpDown[] { numericUpDownDV1, numericUpDownDV2, numericUpDownDV3, numericUpDownDV4, numericUpDownDV5, numericUpDownDV6 });
            natureLabels.AddRange(new Label[] { labelNature1, labelNature2, labelNature3, labelNature4, labelNature5, labelNature6 });
            changeButtons.AddRange(new Button[] { buttonChange1, buttonChange2, buttonChange3, buttonChange4, buttonChange5, buttonChange6 });
            showAllButtons.AddRange(new Button[] { buttonShowAll1, buttonShowAll2, buttonShowAll3, buttonShowAll4, buttonShowAll5, buttonShowAll6 });

            string[] abilityNames = RomInfo.GetAbilityNames();

            for (int i = 0; i < trainerProp.partyCount; i++)
            {
                // Enable only if mon exists
                abilityCombos[i].Enabled = true;
                genderCombos[i].Enabled = true;
                upDownsDV[i].Enabled = true;
                changeButtons[i].Enabled = true;
                showAllButtons[i].Enabled = true;
                abilityCombos[i].DataSource = new BindingSource(abilitySelection, string.Empty);
                genderCombos[i].DataSource = new BindingSource(genderSelection, string.Empty);

                // Pokemon names can be obtained from the ID
                int pokeID = trainerFile.party[i].pokeID ?? (int)trainerFile.party[i].pokeID;
                pokeLabels[i].Text = RomInfo.GetPokemonNames()[pokeID];

                // Upper 4 bits = ability index (0 = no flag, 1 = ability 1, 2 = ability 2)
                abilityCombos[i].SelectedIndex = (((int)trainerFile.party[i].genderAndAbilityFlags & 0xF0) >> 4);

                // Lower 4 bits = gender index (0 = no flag, 1 = force male, 2 = force female)
                genderCombos[i].SelectedIndex = ((int)trainerFile.party[i].genderAndAbilityFlags & 0x0F);

                // DV is stored as a single byte, so we can just cast it directly
                int DV = (int)trainerFile.party[i].difficulty;                
                upDownsDV[i].Value = DV;

            }
            for (int i = trainerProp.partyCount; i < 6; i++)
            {
                abilityCombos[i].Enabled = false;
                genderCombos[i].Enabled = false;
                upDownsDV[i].Enabled = false;
                changeButtons[i].Enabled = false;
                showAllButtons[i].Enabled = false;

                pokeLabels[i].Text = "Empty Slot";
                natureLabels[i].Text = "N/A";
            }

            listsSetup = true;
        }

        private void UpdateNatures()
        {
            if (!listsSetup)
                return;

            DVCalculator.ResetGenderMod(radioMale.Checked);

            for (int i = 0; i < trainerProp.partyCount; i++)
            {
                uint pokeID = trainerFile.party[i].pokeID ?? (uint)trainerFile.party[i].pokeID;
                byte pokeLevel = (byte) trainerFile.party[i].level;
                byte baseGenderRatio = new PokemonPersonalData((int) pokeID).genderVec;

                uint PID = DVCalculator.generatePID(trainerProp.trainerID,trainerProp.trainerClass, 
                    pokeID, pokeLevel, baseGenderRatio, genderCombos[i].SelectedIndex, abilityCombos[i].SelectedIndex,(byte)upDownsDV[i].Value);
                string nature = DVCalculator.Natures[DVCalculator.getNatureFromPID(PID)];

                natureLabels[i].Text = nature;
            }

        }

        private void valueChanged(object sender, EventArgs e)
        {
            Helpers.DisableHandlers();
            UpdateNatures();
            Helpers.EnableHandlers();
        }

        private void buttonShowAll_Click(object sender, EventArgs e)
        {
            Button button = (Button) sender;
            int index = button.Name.Last() - '1'; // Get the index from the button name

            DVCalculator.ResetGenderMod(radioMale.Checked);
            List<DVIVNatureTriplet> triplets = 
                DVCalculator.getAllNatures(
                trainerProp.trainerID,
                trainerProp.trainerClass,
                (uint)trainerFile.party[index].pokeID,
                (byte)trainerFile.party[index].level,
                new PokemonPersonalData((int)trainerFile.party[index].pokeID).genderVec,
                genderCombos[index].SelectedIndex,
                abilityCombos[index].SelectedIndex);
        

            // Need to run this loop at least until we reach the current index :(
            // RIP performance
            for (int i = 1; i < index; i++)
            {
                triplets = DVCalculator.getAllNatures(
                trainerProp.trainerID,
                trainerProp.trainerClass,
                (uint)trainerFile.party[index].pokeID,
                (byte)trainerFile.party[index].level,
                new PokemonPersonalData((int)trainerFile.party[index].pokeID).genderVec,
                genderCombos[index].SelectedIndex,
                abilityCombos[index].SelectedIndex);

            }
            // Show the nature viewer form with the triplets
            DVCalcNatureViewerForm form = new DVCalcNatureViewerForm(triplets);
            form.ShowDialog();

        }

        
    }
}

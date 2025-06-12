using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE
{
    public partial class DVCalc : Form
    {

        public TrainerFile trainerFile;
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
                abilityCombos[i].Enabled = (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS || RomInfo.AIBackportEnabled);
                genderCombos[i].Enabled = (RomInfo.gameFamily == RomInfo.GameFamilies.HGSS || RomInfo.AIBackportEnabled);
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
            if (Helpers.HandlersDisabled || !listsSetup)
                return;

            UpdateNatures();
        }

        private List<DVIVNatureTriplet> generateTriplets(int index)
        {
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

            return triplets;
        }

        private void buttonShowAll_Click(object sender, EventArgs e)
        {
            Button button = (Button) sender;
            int index = button.Name.Last() - '1'; // Get the index from the button name, maybe jank?

            List<DVIVNatureTriplet> triplets = generateTriplets(index);

            // Show the nature viewer form with the triplets
            DVCalcNatureViewerForm form = new DVCalcNatureViewerForm(triplets);
            form.ShowDialog();
            if (form.selectedDV != -1)
                upDownsDV[index].Value = form.selectedDV;

        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DV, or \"Difficulty Value\", is used by the game to calculate how tough an opponent Pokémon should be.\n" +
               "The DV primarily affects a Pokémon's IVs - the higher the value, the higher the Pokémon's IVs.\n" +
               "DVs will go from 0 (0 IVs) to 255 (31 IVs). Natures are chosen semi-randomly." +
               "\nIVs will be the same value for all Stats at any DV, so Hidden Power will only be Fighting or Dark Type." +
               "\nThis calculator allows you to choose a desired Nature and then find the highest possible DV that will yield that Nature." +
               "\nIf you want a specific combination of IVs and Nature instead, please click the \"Show All\" button and find the one you want.\n\n" +
               "For more information click \"More Info\""
               , "Difficulty Value", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonUsage_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Click the \"Change\" or \"Show All\" buttons to open a list of possible nature / IV combinations. " +
                "Double click an entry to select it. You can also try changing the ability and gender flags." +
                "Because of the way that the PIDs of trainer Pokémon are generated, only certain combinations of " +
                "gender, ability, IVs and nature are possible. This is a limitation of the game itself.\n" +
                "In general changes made to gender and ability flags of a Pokémon in any given party slot " +
                "will affect all the slots after it. You should therefore work from top to bottom.\n" +
                "Diamond, Pearl and Platinum do not allow for gender or ability flags to be set."
               , "DVCalc Usage", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonMoreInfo_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to open a link to an external website?",
                "Confirm Open Website", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://gist.github.com/YakosWG/2200732c473656db2e47c37ca72807d7",
                    UseShellExecute = true
                });
            }

        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int index = button.Name.Last() - '1'; // Get the index from the button name, maybe jank?

            List<DVIVNatureTriplet> triplets = generateTriplets(index);
            DVCalculator.filterHighestDV(ref triplets);

            // Show the nature viewer form with the triplets
            DVCalcNatureViewerForm form = new DVCalcNatureViewerForm(triplets);
            form.ShowDialog();
            if (form.selectedDV != -1)
                upDownsDV[index].Value = form.selectedDV;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < trainerProp.partyCount; i++)
            {
                trainerFile.party[i].genderAndAbilityFlags = (PartyPokemon.GenderAndAbilityFlags)(((abilityCombos[i].SelectedIndex & 0x0F) << 4) 
                    | (genderCombos[i].SelectedIndex & 0x0F));
                trainerFile.party[i].difficulty = (byte)upDownsDV[i].Value;

            }
        }
    }
}

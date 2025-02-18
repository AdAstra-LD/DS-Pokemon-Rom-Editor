using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE
{
    partial class DVCalc
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>

        #endregion

        private Label poke_label;
        private Label trainerClassIdx_label;
        private Label trainerIdx_label;
        private Label pokeLVL_label;
        private NumericUpDown pokeLevel;
        private NumericUpDown trainerClassIdx;
        private NumericUpDown trainerIdx;
        private ComboBox natureSelect;
        private Label nature_label;
        private Label DV_label;
        private Button calcButton;
        private CheckBox maleCheck;
        private Label maxDVNature_label;
        private Label IV_label;
        private ComboBox pokemonSelector;
        private Button showAllButton;
        private RadioButton radioButtonMale;
        private RadioButton radioButtonFemale;
        private RadioButton radioButtonAbility1;
        private RadioButton radioButtonAbility2;
        private NumericUpDown numericUpDownGender;
        private GroupBox groupBoxGender;
        private GroupBox groupBoxAbility;
        private Label labelGenderRatio;
        private GroupBox groupBoxHGSS;
        private RadioButton radioButtonIngoreGender;
        private RadioButton radioButtonIgnoreAbility;
        private Button buttonHelp;
        private Button buttonHGSS;
    }
}

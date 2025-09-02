using DSPRE.Editors.Utils;
using System.Drawing;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.Editors
{
    partial class PokemonSpriteEditor
    {
        private System.ComponentModel.IContainer components = null;
        private ComboBox SaveBox;
        private ComboBox BasePalette;
        private ComboBox ShinyPalette;
        private Button OpenPngs;
        private Button LoadSheet;
        private Button SaveSingle;
        private Button MakeShiny;
        private Button OpenOther;
        private Button SaveChanges;
        private Label lblMale;
        private Label lblFemale;
        private Label BackN;
        private Label BackS;
        private Label FrontN;
        private Label FrontS;
        private Label lblNormal;
        private Label lblShiny;

        private void InitializeComponent()
        {
            this.OpenPngs = new System.Windows.Forms.Button();
            this.LoadSheet = new System.Windows.Forms.Button();
            this.SaveSingle = new System.Windows.Forms.Button();
            this.MakeShiny = new System.Windows.Forms.Button();
            this.OpenOther = new System.Windows.Forms.Button();
            this.SaveChanges = new System.Windows.Forms.Button();
            this.lblMale = new System.Windows.Forms.Label();
            this.lblFemale = new System.Windows.Forms.Label();
            this.BackN = new System.Windows.Forms.Label();
            this.BackS = new System.Windows.Forms.Label();
            this.FrontN = new System.Windows.Forms.Label();
            this.FrontS = new System.Windows.Forms.Label();
            this.lblNormal = new System.Windows.Forms.Label();
            this.lblShiny = new System.Windows.Forms.Label();
            this.IndexBox = new System.Windows.Forms.ComboBox();
            this.BasePalette = new System.Windows.Forms.ComboBox();
            this.ShinyPalette = new System.Windows.Forms.ComboBox();
            this.SaveBox = new System.Windows.Forms.ComboBox();
            this.femaleBackNormalPic = new System.Windows.Forms.PictureBox();
            this.maleBackNormalPic = new System.Windows.Forms.PictureBox();
            this.femaleFrontNormalPic = new System.Windows.Forms.PictureBox();
            this.maleFrontNormalPic = new System.Windows.Forms.PictureBox();
            this.maleBackShinyPic = new System.Windows.Forms.PictureBox();
            this.femaleBackShinyPic = new System.Windows.Forms.PictureBox();
            this.maleFrontShinyPic = new System.Windows.Forms.PictureBox();
            this.femaleFrontShinyPic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.femaleBackNormalPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maleBackNormalPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.femaleFrontNormalPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maleFrontNormalPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maleBackShinyPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.femaleBackShinyPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maleFrontShinyPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.femaleFrontShinyPic)).BeginInit();
            this.SuspendLayout();
            // 
            // OpenPngs
            // 
            this.OpenPngs.Enabled = false;
            this.OpenPngs.Location = new System.Drawing.Point(475, 8);
            this.OpenPngs.Name = "OpenPngs";
            this.OpenPngs.Size = new System.Drawing.Size(100, 25);
            this.OpenPngs.TabIndex = 3;
            this.OpenPngs.Text = "Load Sprite Set";
            // 
            // LoadSheet
            // 
            this.LoadSheet.Location = new System.Drawing.Point(369, 8);
            this.LoadSheet.Name = "LoadSheet";
            this.LoadSheet.Size = new System.Drawing.Size(100, 25);
            this.LoadSheet.TabIndex = 9;
            this.LoadSheet.Text = "Load Sprite Sheet";
            this.LoadSheet.Click += new System.EventHandler(this.btnLoadSheet_Click);
            // 
            // SaveSingle
            // 
            this.SaveSingle.Location = new System.Drawing.Point(674, 728);
            this.SaveSingle.Name = "SaveSingle";
            this.SaveSingle.Size = new System.Drawing.Size(70, 21);
            this.SaveSingle.TabIndex = 10;
            this.SaveSingle.Text = "Save PNG";
            this.SaveSingle.Click += new System.EventHandler(this.SaveSingle_Click);
            // 
            // MakeShiny
            // 
            this.MakeShiny.Location = new System.Drawing.Point(12, 728);
            this.MakeShiny.Name = "MakeShiny";
            this.MakeShiny.Size = new System.Drawing.Size(120, 21);
            this.MakeShiny.TabIndex = 11;
            this.MakeShiny.Text = "Create Shiny Palette";
            // 
            // OpenOther
            // 
            this.OpenOther.Location = new System.Drawing.Point(12, 8);
            this.OpenOther.Name = "OpenOther";
            this.OpenOther.Size = new System.Drawing.Size(100, 25);
            this.OpenOther.TabIndex = 12;
            this.OpenOther.Text = "Open Forms";
            this.OpenOther.Visible = false;
            this.OpenOther.Click += new System.EventHandler(this.btnOpenOther_Click);
            // 
            // SaveChanges
            // 
            this.SaveChanges.Location = new System.Drawing.Point(642, 12);
            this.SaveChanges.Name = "SaveChanges";
            this.SaveChanges.Size = new System.Drawing.Size(100, 21);
            this.SaveChanges.TabIndex = 13;
            this.SaveChanges.Text = "Save Changes";
            this.SaveChanges.Click += new System.EventHandler(this.SaveChanges_Click);
            // 
            // lblMale
            // 
            this.lblMale.Location = new System.Drawing.Point(564, 36);
            this.lblMale.Name = "lblMale";
            this.lblMale.Size = new System.Drawing.Size(100, 23);
            this.lblMale.TabIndex = 1;
            this.lblMale.Text = "Male";
            // 
            // lblFemale
            // 
            this.lblFemale.Location = new System.Drawing.Point(232, 36);
            this.lblFemale.Name = "lblFemale";
            this.lblFemale.Size = new System.Drawing.Size(100, 23);
            this.lblFemale.TabIndex = 0;
            this.lblFemale.Text = "Female";
            // 
            // BackN
            // 
            this.BackN.Location = new System.Drawing.Point(52, 126);
            this.BackN.Name = "BackN";
            this.BackN.Size = new System.Drawing.Size(56, 23);
            this.BackN.TabIndex = 2;
            this.BackN.Text = "Back";
            // 
            // BackS
            // 
            this.BackS.Location = new System.Drawing.Point(52, 462);
            this.BackS.Name = "BackS";
            this.BackS.Size = new System.Drawing.Size(56, 23);
            this.BackS.TabIndex = 3;
            this.BackS.Text = "Back";
            // 
            // FrontN
            // 
            this.FrontN.Location = new System.Drawing.Point(52, 294);
            this.FrontN.Name = "FrontN";
            this.FrontN.Size = new System.Drawing.Size(56, 23);
            this.FrontN.TabIndex = 4;
            this.FrontN.Text = "Front";
            // 
            // FrontS
            // 
            this.FrontS.Location = new System.Drawing.Point(52, 630);
            this.FrontS.Name = "FrontS";
            this.FrontS.Size = new System.Drawing.Size(56, 23);
            this.FrontS.TabIndex = 5;
            this.FrontS.Text = "Front";
            // 
            // lblNormal
            // 
            this.lblNormal.Location = new System.Drawing.Point(8, 210);
            this.lblNormal.Name = "lblNormal";
            this.lblNormal.Size = new System.Drawing.Size(100, 23);
            this.lblNormal.TabIndex = 6;
            this.lblNormal.Text = "Normal";
            // 
            // lblShiny
            // 
            this.lblShiny.Location = new System.Drawing.Point(8, 546);
            this.lblShiny.Name = "lblShiny";
            this.lblShiny.Size = new System.Drawing.Size(100, 23);
            this.lblShiny.TabIndex = 7;
            this.lblShiny.Text = "Shiny";
            // 
            // IndexBox
            // 
            this.IndexBox.DropDownWidth = 160;
            this.IndexBox.Location = new System.Drawing.Point(172, 8);
            this.IndexBox.MaxDropDownItems = 16;
            this.IndexBox.Name = "IndexBox";
            this.IndexBox.Size = new System.Drawing.Size(160, 21);
            this.IndexBox.TabIndex = 6;
            this.IndexBox.SelectedIndexChanged += new System.EventHandler(this.IndexBox_SelectedIndexChanged);
            // 
            // BasePalette
            // 
            this.BasePalette.DropDownWidth = 160;
            this.BasePalette.Enabled = false;
            this.BasePalette.Location = new System.Drawing.Point(138, 728);
            this.BasePalette.MaxDropDownItems = 16;
            this.BasePalette.Name = "BasePalette";
            this.BasePalette.Size = new System.Drawing.Size(160, 21);
            this.BasePalette.TabIndex = 6;
            this.BasePalette.Visible = false;
            this.BasePalette.SelectedIndexChanged += new System.EventHandler(this.BasePalette_SelectedIndexChanged);
            // 
            // ShinyPalette
            // 
            this.ShinyPalette.DropDownWidth = 160;
            this.ShinyPalette.Enabled = false;
            this.ShinyPalette.Location = new System.Drawing.Point(309, 729);
            this.ShinyPalette.MaxDropDownItems = 16;
            this.ShinyPalette.Name = "ShinyPalette";
            this.ShinyPalette.Size = new System.Drawing.Size(160, 21);
            this.ShinyPalette.TabIndex = 6;
            this.ShinyPalette.Visible = false;
            this.ShinyPalette.SelectedIndexChanged += new System.EventHandler(this.ShinyPalette_SelectedIndexChanged);
            // 
            // SaveBox
            // 
            this.SaveBox.DropDownWidth = 160;
            this.SaveBox.Items.AddRange(new object[] {
            "Normal Female Backsprite",
            "Normal Male Backsprite",
            "Normal Female Frontsprite",
            "Normal Male Frontprite",
            "Shiny Female Backsprite",
            "Shiny Male Backsprite",
            "Shiny Female Frontsprite",
            "Shiny Male Frontprite"});
            this.SaveBox.Location = new System.Drawing.Point(512, 728);
            this.SaveBox.Name = "SaveBox";
            this.SaveBox.Size = new System.Drawing.Size(160, 21);
            this.SaveBox.TabIndex = 8;
            // 
            // femaleBackNormalPic
            // 
            this.femaleBackNormalPic.Location = new System.Drawing.Point(96, 64);
            this.femaleBackNormalPic.Name = "femaleBackNormalPic";
            this.femaleBackNormalPic.Size = new System.Drawing.Size(320, 160);
            this.femaleBackNormalPic.TabIndex = 14;
            this.femaleBackNormalPic.TabStop = false;
            this.femaleBackNormalPic.Click += new System.EventHandler(this.OpenPngs_Click);
            // 
            // maleBackNormalPic
            // 
            this.maleBackNormalPic.Location = new System.Drawing.Point(422, 64);
            this.maleBackNormalPic.Name = "maleBackNormalPic";
            this.maleBackNormalPic.Size = new System.Drawing.Size(320, 160);
            this.maleBackNormalPic.TabIndex = 15;
            this.maleBackNormalPic.TabStop = false;
            this.maleBackNormalPic.Click += new System.EventHandler(this.OpenPngs_Click);
            // 
            // femaleFrontNormalPic
            // 
            this.femaleFrontNormalPic.Location = new System.Drawing.Point(96, 230);
            this.femaleFrontNormalPic.Name = "femaleFrontNormalPic";
            this.femaleFrontNormalPic.Size = new System.Drawing.Size(320, 160);
            this.femaleFrontNormalPic.TabIndex = 16;
            this.femaleFrontNormalPic.TabStop = false;
            this.femaleFrontNormalPic.Click += new System.EventHandler(this.OpenPngs_Click);
            // 
            // maleFrontNormalPic
            // 
            this.maleFrontNormalPic.Location = new System.Drawing.Point(422, 230);
            this.maleFrontNormalPic.Name = "maleFrontNormalPic";
            this.maleFrontNormalPic.Size = new System.Drawing.Size(320, 160);
            this.maleFrontNormalPic.TabIndex = 17;
            this.maleFrontNormalPic.TabStop = false;
            this.maleFrontNormalPic.Click += new System.EventHandler(this.OpenPngs_Click);
            // 
            // maleBackShinyPic
            // 
            this.maleBackShinyPic.Location = new System.Drawing.Point(422, 396);
            this.maleBackShinyPic.Name = "maleBackShinyPic";
            this.maleBackShinyPic.Size = new System.Drawing.Size(320, 160);
            this.maleBackShinyPic.TabIndex = 18;
            this.maleBackShinyPic.TabStop = false;
            this.maleBackShinyPic.Click += new System.EventHandler(this.OpenPngs_Click);
            // 
            // femaleBackShinyPic
            // 
            this.femaleBackShinyPic.Location = new System.Drawing.Point(96, 396);
            this.femaleBackShinyPic.Name = "femaleBackShinyPic";
            this.femaleBackShinyPic.Size = new System.Drawing.Size(320, 160);
            this.femaleBackShinyPic.TabIndex = 19;
            this.femaleBackShinyPic.TabStop = false;
            this.femaleBackShinyPic.Click += new System.EventHandler(this.OpenPngs_Click);
            // 
            // maleFrontShinyPic
            // 
            this.maleFrontShinyPic.Location = new System.Drawing.Point(422, 562);
            this.maleFrontShinyPic.Name = "maleFrontShinyPic";
            this.maleFrontShinyPic.Size = new System.Drawing.Size(320, 160);
            this.maleFrontShinyPic.TabIndex = 20;
            this.maleFrontShinyPic.TabStop = false;
            this.maleFrontShinyPic.Click += new System.EventHandler(this.OpenPngs_Click);
            // 
            // femaleFrontShinyPic
            // 
            this.femaleFrontShinyPic.Location = new System.Drawing.Point(96, 562);
            this.femaleFrontShinyPic.Name = "femaleFrontShinyPic";
            this.femaleFrontShinyPic.Size = new System.Drawing.Size(320, 160);
            this.femaleFrontShinyPic.TabIndex = 21;
            this.femaleFrontShinyPic.TabStop = false;
            this.femaleFrontShinyPic.Click += new System.EventHandler(this.OpenPngs_Click);
            // 
            // PokemonSpriteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 761);
            this.Controls.Add(this.femaleFrontShinyPic);
            this.Controls.Add(this.maleFrontShinyPic);
            this.Controls.Add(this.femaleBackShinyPic);
            this.Controls.Add(this.maleBackShinyPic);
            this.Controls.Add(this.maleFrontNormalPic);
            this.Controls.Add(this.femaleFrontNormalPic);
            this.Controls.Add(this.maleBackNormalPic);
            this.Controls.Add(this.femaleBackNormalPic);
            this.Controls.Add(this.lblFemale);
            this.Controls.Add(this.lblMale);
            this.Controls.Add(this.BackN);
            this.Controls.Add(this.BackS);
            this.Controls.Add(this.FrontN);
            this.Controls.Add(this.FrontS);
            this.Controls.Add(this.lblNormal);
            this.Controls.Add(this.lblShiny);
            this.Controls.Add(this.IndexBox);
            this.Controls.Add(this.BasePalette);
            this.Controls.Add(this.ShinyPalette);
            this.Controls.Add(this.SaveBox);
            this.Controls.Add(this.OpenPngs);
            this.Controls.Add(this.LoadSheet);
            this.Controls.Add(this.SaveSingle);
            this.Controls.Add(this.MakeShiny);
            this.Controls.Add(this.OpenOther);
            this.Controls.Add(this.SaveChanges);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PokemonSpriteEditor";
            ((System.ComponentModel.ISupportInitialize)(this.femaleBackNormalPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maleBackNormalPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.femaleFrontNormalPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maleFrontNormalPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maleBackShinyPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.femaleBackShinyPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maleFrontShinyPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.femaleFrontShinyPic)).EndInit();
            this.ResumeLayout(false);

        }

        private PictureBox femaleBackNormalPic;
        private PictureBox maleBackNormalPic;
        private PictureBox femaleFrontNormalPic;
        private PictureBox maleFrontNormalPic;
        private PictureBox maleBackShinyPic;
        private PictureBox femaleBackShinyPic;
        private PictureBox maleFrontShinyPic;
        private PictureBox femaleFrontShinyPic;
        public ComboBox IndexBox;
    }
}

using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System;
using static DSPRE.RomInfo;

namespace DSPRE.Editors
{
    public struct FileEntry
    {
        public int Ofs;
        public int Size;
    }

    public class NarcReader
    {
        public int Entrys;

        public FileEntry[] fe;

        public FileStream fs;

        string m_sFileName;

        public long size;

        public NarcReader(string strFileName)
        {
            m_sFileName = strFileName;
            fs = new FileStream(strFileName, FileMode.Open, FileAccess.ReadWrite);
            BinaryReader binaryReader = new BinaryReader(fs);
            byte[] array = new byte[16];
            binaryReader.Read(array, 0, 16);
            size = BitConverter.ToUInt32(array, 8);
            int num = BitConverter.ToInt16(array, 12);
            fs.Seek(num, SeekOrigin.Begin);
            array = new byte[12];
            binaryReader.Read(array, 0, 12);
            int num2 = BitConverter.ToInt32(array, 4);
            Entrys = BitConverter.ToInt32(array, 8);
            fe = new FileEntry[Entrys];
            for (int i = 0; i < Entrys; i++)
            {
                fe[i].Ofs = binaryReader.ReadInt32();
                fe[i].Size = binaryReader.ReadInt32() - fe[i].Ofs;
            }
            fs.Seek(num + num2, SeekOrigin.Begin);
            array = new byte[16];
            binaryReader.Read(array, 0, 16);
            int num3 = BitConverter.ToInt32(array, 4);
            num3 = num + num3 + num2 + 8;
            for (int j = 0; j < Entrys; j++)
            {
                fe[j].Ofs += num3;
            }
            fs.Close();
        }

        public void Close()
        {
            fs.Close();
        }

        public int OpenEntry(int id)
        {
            fs = new FileStream(m_sFileName, FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(fe[id].Ofs, SeekOrigin.Begin);
            return 0;
        }
    }
    public class SpriteSet
    {
        public Bitmap[] Sprites;
        public ColorPalette Normal;
        public ColorPalette Shiny;

        public SpriteSet()
        {
            Sprites = new Bitmap[4];
            for (int i = 0; i < 4; i++)
            {
                Sprites[i] = null;
            }
            Normal = null;
            Shiny = null;
        }
    }

    public partial class PokemonSpriteEditor : Form
    {
        System.ComponentModel.IContainer components = null;

        ComboBox IndexBox;
        ComboBox SaveBox;
        ComboBox BasePalette;
        ComboBox ShinyPalette;
        Button OpenPngs;
        Button LoadSheet;
        Button SaveSingle;
        Button MakeShiny;
        Button OpenOther;
        Button SaveChanges;

        NarcReader nr;
        PictureBox[,] Display;

        Label lblMale;
        Label lblFemale;
        Label BackN;
        Label BackS;
        Label FrontN;
        Label FrontS;
        Label lblNormal;
        Label lblShiny;

        bool[] used;
        Rectangle rect;
        IndexedBitmapHandler Handler;

        SpriteSet CurrentSprites;
        private readonly string[] pokenames;

        private readonly int[] formPalettes = new int[]
        {
            // Deoxys forms (Base, Attack, Defence, Speed)
            158, 158, 158, 158, 

            // Unown forms (A-Z, !, ?)
            160, 160, 160, 160, 160, 160, 160, 160, 160, 160,
            160, 160, 160, 160, 160, 160, 160, 160, 160, 160,
            160, 160, 160, 160, 160, 160, 160, 160, 160, 160,

            // Castform (Base, Sunny, Rainy, Snowy)
            162, 162, 162, 162, 

            // Burmy (Plant, Sandy, Trash)
            164, 164, 164, 

            // Wormadam (Plant, Sandy, Trash)
            166, 166, 166, 

            // Shellos (West, East)
            168, 168, 168, 168,

            // Gastrodon (West, East)
            170, 170, 170, 170,

            // Cherrim (Overcast, Sunny)
            172, 172, 172, 172,

            // Arceus (various forms)
            174, 174, 174, 174, 174, 174, 174, 174,
            174, 174, 174, 174, 174, 174, 174, 174,
            174, 174, 174, 174, 174, 174, 174, 174,
            174, 174, 174, 174, 174, 174, 174, 174,
            174, 174, 174, 174, 174, 174, 174, 174,

            // Egg (Global, Manaphy)
            176, 176,

            // Shaymin (Land, Sky)
            178, 178, 178, 178,

            // Rotom (Normal, Heat, Wash, Frost, Fan, Mow)
            180, 180, 180, 180, 180, 180, 180, 180,
            180, 180, 180, 180, 180, 180, 180, 180,

            // Giratina (Altered, Origin)
            182, 182, 182, 182,

            // Substitute (Sprites)
            184, 184, 184,

            // Shadows
            186, 186
        };

        private readonly int[] shinyPalettes = new int[]
        {
            // Deoxys forms (Base, Attack, Defence, Speed)
            159, 159, 159, 159,

            // Unown forms (A-Z, !, ?)
            161, 161, 161, 161, 161, 161, 161, 161, 161, 161,
            161, 161, 161, 161, 161, 161, 161, 161, 161, 161,
            161, 161, 161, 161, 161, 161, 161, 161, 161, 161,

            // Castform (Base, Sunny, Rainy, Snowy)
            163, 163, 163, 163, 

            // Burmy (Plant, Sandy, Trash)
            165, 165, 165, 

            // Wormadam (Plant, Sandy, Trash)
            167, 167, 167, 

            // Shellos (West, East)
            169, 169, 169, 169,

            // Gastrodon (West, East)
            171, 171, 171, 171,

            // Cherrim (Overcast, Sunny)
            173, 173, 173, 173,

            // Arceus (various forms)
            175, 175, 175, 175, 175, 175, 175, 175,
            175, 175, 175, 175, 175, 175, 175, 175,
            175, 175, 175, 175, 175, 175, 175, 175,
            175, 175, 175, 175, 175, 175, 175, 175,
            175, 175, 175, 175, 175, 175, 175, 175,

            // Egg (Global, Manaphy)
            177, 177,

            // Shaymin (Land, Sky)
            179, 179, 179, 179,

            // Rotom (Normal, Heat, Wash, Frost, Fan, Mow)
            181, 181, 181, 181, 181, 181, 181, 181,
            181, 181, 181, 181, 181, 181, 181, 181,

            // Giratina (Altered, Origin)
            183, 183, 183, 183,

            // Substitute (Sprites)
            185, 185, 185,

            // Shadows
            187, 187
        };

        private readonly int[] validPalettesHGSS = new int[] 
        {
            158, 159, 160, 161, 162, 163, 164, 165, 166, 167,
            168, 169, 170, 171, 172, 173, 174, 175, 176, 177,
            178, 179, 180, 181, 182, 183, 184, 185, 186, 187,
            188, 189, 190, 191, 192, 193, 194, 195, 196, 197,
            198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
            208, 209, 210, 211, 212, 213, 214, 215, 216, 217,
            218, 219, 220, 221, 222, 223, 224, 225, 226, 227,
            228, 229, 230, 231, 232, 233, 234, 235, 236, 237,
            238, 239, 240, 241, 242, 243, 244, 245, 246, 247,
            248, 249, 250, 251, 252, 253, 254, 255, 258, 260
        };

        private readonly int[] validPalettesPt = new int[]
        {
            154, 155, 156, 157, 158, 159, 160, 161, 162, 163,
            164, 165, 166, 167, 168, 169, 170, 171, 172, 173,
            174, 175, 176, 177, 178, 179, 180, 181, 182, 183,
            184, 185, 186, 187, 188, 189, 190, 191, 192, 193,
            194, 195, 196, 197, 198, 199, 200, 201, 202, 203,
            204, 205, 206, 207, 208, 209, 210, 211, 212, 213,
            214, 215, 216, 217, 218, 219, 220, 221, 222, 223,
            224, 225, 226, 227, 228, 229, 230, 231, 232, 233,
            234, 235, 236, 237, 238, 239, 240, 241, 242, 243,
            244, 245, 246, 247, 250, 252
        };

        private readonly int[] validPalettesDP = new int[]
        {
            134, 135, 136, 137, 138, 139, 140, 141, 142, 145,
            146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 
            156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 
            166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 
            176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 
            186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 
            196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 
            206, 207, 210, 212, 
        };

        private readonly string[] otherPokenames = new string[]
        {
            "Deoxys - Base", "Deoxys - Attack", "Deoxys - Defence", "Deoxys - Speed",
            "Unown - A", "Unown - B", "Unown - C", "Unown - D", "Unown - E", "Unown - F",
            "Unown - G", "Unown - H", "Unown - I", "Unown - J", "Unown - K", "Unown - L",
            "Unown - M", "Unown - N", "Unown - O", "Unown - P", "Unown - Q", "Unown - R",
            "Unown - S", "Unown - T", "Unown - U", "Unown - V", "Unown - W", "Unown - X",
            "Unown - Y", "Unown - Z", "Unown - !", "Unown - ?",
            "Castform - Base", "Castform - Sunny", "Castform - Rainy", "Castform - Snow", 
            "Burmy - Plant", "Burmy - Sandy", "Burmy - Trash", 
            "Wormadam - Plant", "Wormadam - Sandy", "Wormadam - Trash", 
            "Shellos", "Shellos", 
            "Gastrodon", "Gastrodon", 
            "Cherrim", "Cherrim", 
            "Arceus - Type 1", "Arceus - Type 2", "Arceus - Type 3", "Arceus - Type 4",
            "Arceus - Type 5", "Arceus - Type 6", "Arceus - Type 7", "Arceus - Type 8",
            "Arceus - Type 9", "Arceus - Type 10", "Arceus - Type 11", "Arceus - Type 12",
            "Arceus - Type 13", "Arceus - Type 14", "Arceus - Type 15", "Arceus - Type 16",
            "Egg", "Egg",
            "Shaymin", "Shaymin", "Shaymin", "Shaymin",
            "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom",
            "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom",
            "Giratina", "Giratina", "Giratina", "Giratina",
            "Deoxys", "Deoxys",
            "Unown", "Unown",
            "Castform", "Castform", "Castform", "Castform", "Castform", "Castform", "Castform", "Castform",
            "Burmy", "Burmy", "Burmy", "Burmy", "Burmy", "Burmy",
            "Wormadam", "Wormadam", "Wormadam", "Wormadam", "Wormadam", "Wormadam",
            "Shellos", "Shellos", "Shellos", "Shellos",
            "Gastrodon", "Gastrodon", "Gastrodon", "Gastrodon",
            "Cherrim", "Cherrim", "Cherrim", "Cherrim",
            "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus",
            "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus",
            "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus",
            "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus",
            "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus", "Arceus",
            "Egg", "Egg",
            "Shaymin", "Shaymin", "Shaymin", "Shaymin",
            "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom",
            "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom", "Rotom",
            "Giratina", "Giratina", "Giratina", "Giratina",
            "Substitute", "Substitute", "Substitute",
            "Shadows", "Shadows"
        };

        static string[] names = { "Female backsprite", "Male backsprite", "Female frontsprite", "Male frontsprite", "Shiny" };

        private bool loadingOther = false;
        private PokemonEditor _parent;

        private static bool dirty = false;
        private static readonly string formName = "Sprite Editor";
        public PokemonSpriteEditor(Control parent, PokemonEditor pokeEditor)
        {
            this._parent = pokeEditor;
            InitializeComponent();            
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            this.pokenames = RomInfo.GetPokemonNames();
            Helpers.DisableHandlers();
            LoadSprites();           
            Helpers.EnableHandlers();
        }

        void InitializeComponent()
        {
            used = null;
            Handler = new IndexedBitmapHandler();
            CurrentSprites = new SpriteSet();
            BuildPictureBoxes(96, 56);
            
            OpenPngs = new Button();
            LoadSheet = new Button();
            SaveSingle = new Button();
            MakeShiny = new Button();
            OpenOther = new Button();
            SaveChanges = new Button();

            lblMale = new Label();
            lblFemale = new Label();
            BackN = new Label();
            BackS = new Label();
            FrontN = new Label();
            FrontS = new Label();
            lblNormal = new Label();
            lblShiny = new Label();
            SuspendLayout();           

            IndexBox = new ComboBox();
            IndexBox.DropDownWidth = 160;
            IndexBox.Location = new Point(340, 8);
            IndexBox.MaxDropDownItems = 16;
            IndexBox.Name = "IndexBox";
            IndexBox.Size = new Size(160, 21);
            IndexBox.TabIndex = 6;
            IndexBox.SelectedIndexChanged += (IndexBox_SelectedIndexChanged);

            int[] source = RomInfo.gameFamily == GameFamilies.Plat ? validPalettesPt : RomInfo.gameFamily == GameFamilies.DP ? validPalettesDP : validPalettesHGSS;

            BasePalette = new ComboBox();
            BasePalette.DropDownWidth = 160;
            BasePalette.Location = new Point(220, 728);
            BasePalette.MaxDropDownItems = 16;
            BasePalette.Name = "BasePalette";
            BasePalette.Size = new Size(160, 21);
            BasePalette.TabIndex = 6;
            BasePalette.SelectedIndexChanged += (BasePalette_SelectedIndexChanged);
            
            ShinyPalette = new ComboBox();  
            ShinyPalette.DropDownWidth = 160;
            ShinyPalette.Location = new Point(400, 728);
            ShinyPalette.MaxDropDownItems = 16;
            ShinyPalette.Name = "ShinyPalette";
            ShinyPalette.Size = new Size(160, 21);
            ShinyPalette.TabIndex = 6;
            ShinyPalette.SelectedIndexChanged += (ShinyPalette_SelectedIndexChanged);

            foreach ( var item in source )
            {
                BasePalette.Items.Add( item );
                ShinyPalette.Items.Add( item );
            }

            BasePalette.Enabled = false;
            BasePalette.Visible = false;
            ShinyPalette.Enabled = false;
            ShinyPalette.Visible = false;

            OpenPngs.Location = new Point(646, 8);
            OpenPngs.Name = "OpenPng";
            OpenPngs.Size = new Size(100, 25);
            OpenPngs.TabIndex = 3;
            OpenPngs.Text = "Load Sprite Set";
            OpenPngs.Click += (OpenPng_Click);

            LoadSheet.Location = new Point(538, 8);
            LoadSheet.Name = "LoadSheet";
            LoadSheet.Size = new Size(100, 25);
            LoadSheet.Text = "Load Sprite Sheet";
            LoadSheet.Click += (btnLoadSheet_Click);

            OpenOther.Location = new Point(130, 8);
            OpenOther.Name = "OpenForms";
            OpenOther.Size = new Size(100, 25);
            OpenOther.Text = "Open Forms";
            OpenOther.Click += (btnOpenOther_Click);

            SaveBox = new ComboBox();
            SaveBox.DropDownWidth = 160;
            SaveBox.Location = new Point(512, 728);
            SaveBox.MaxDropDownItems = 8;
            SaveBox.Name = "SaveBox";
            SaveBox.Size = new Size(160, 21);

            SaveBox.Items.Add("Normal Female Backsprite");
            SaveBox.Items.Add("Normal Male Backsprite");
            SaveBox.Items.Add("Normal Female Frontsprite");
            SaveBox.Items.Add("Normal Male Frontprite");
            SaveBox.Items.Add("Shiny Female Backsprite");
            SaveBox.Items.Add("Shiny Male Backsprite");
            SaveBox.Items.Add("Shiny Female Frontsprite");
            SaveBox.Items.Add("Shiny Male Frontprite");
            SaveBox.SelectedIndex = 0;

            SaveSingle.Location = new Point(674, 728);
            SaveSingle.Name = "SaveSingle";
            SaveSingle.Size = new Size(70, 21);
            SaveSingle.Text = "Save PNG";
            SaveSingle.Click += (SaveSingle_Click);

            MakeShiny.Location = new Point(96, 728);
            MakeShiny.Name = "MakeShiny";
            MakeShiny.Size = new Size(120, 21);
            MakeShiny.Text = "Create Shiny Palette";
            MakeShiny.Click += (MakeShiny_Click);

            SaveChanges.Location = new Point(754, 8);
            SaveChanges.Name = "SaveChanges";
            SaveChanges.Size = new Size(70, 21);
            SaveChanges.Text = "Save Changes";
            SaveChanges.Click += (SaveChanges_Click);

            lblFemale.Text = "Female";
            lblFemale.Location = new Point(232, 36);
            Controls.Add(lblFemale);
            lblMale.Text = "Male";
            lblMale.Location = new Point(564, 36);
            Controls.Add(lblMale);
            BackN.Text = "Back";
            BackN.Location = new Point(52, 126);
            Controls.Add(BackN);
            BackS.Text = "Back";
            BackS.Location = new Point(52, 462);
            Controls.Add(BackS);
            FrontN.Text = "Front";
            FrontN.Location = new Point(52, 294);
            Controls.Add(FrontN);
            FrontS.Text = "Front";
            FrontS.Location = new Point(52, 630);
            Controls.Add(FrontS);
            lblNormal.Text = "Normal";
            lblNormal.Location = new Point(8, 210);
            Controls.Add(lblNormal);
            lblShiny.Text = "Shiny";
            lblShiny.Location = new Point(8, 546);
            Controls.Add(lblShiny);

            ResumeLayout(false);

            Controls.Add(IndexBox);
            Controls.Add(BasePalette);
            Controls.Add(ShinyPalette);
            Controls.Add(SaveBox);
            Controls.Add(OpenPngs);
            Controls.Add(LoadSheet);
            Controls.Add(SaveSingle);
            Controls.Add(MakeShiny);
            Controls.Add(OpenOther);
            Controls.Add(SaveChanges);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Width = 760;
            Height = 808;
            AutoScaleBaseSize = new Size(5, 13);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            OpenPngs.Enabled = false;
            OpenOther.Visible = false;
        }

        void IndexBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentSprites = new SpriteSet();
            if ( !this.loadingOther )
            {
                int num = (IndexBox.Items.IndexOf(IndexBox.Text) * 6);
                for (int i = 0; i < 4; i++)
                {
                    if (nr.fe[num + i].Size == 6448)
                    {
                        nr.OpenEntry(num + i);
                        CurrentSprites.Sprites[i] = MakeImage(nr.fs);
                        nr.Close();
                    }
                }
                if (nr.fe[num + 4].Size == 72)
                {
                    nr.OpenEntry(num + 4);
                    CurrentSprites.Normal = SetPal(nr.fs);
                    nr.Close();
                }
                if (nr.fe[num + 5].Size == 72)
                {
                    nr.OpenEntry(num + 5);
                    CurrentSprites.Shiny = SetPal(nr.fs);
                    nr.Close();
                }
            } else
            {
                int num = (IndexBox.Items.IndexOf(IndexBox.Text) * 2);
                for (int i = 0; i < 2; i++)
                {
                    if (nr.fe[num + i].Size == 6448)
                    {
                        nr.OpenEntry(num + i);
                        CurrentSprites.Sprites[i*2 + 1] = MakeImage(nr.fs);
                        nr.Close();
                    }
                }
                if (nr.fe[(int)BasePalette.SelectedItem].Size == 72)
                {
                    nr.OpenEntry((int)BasePalette.SelectedItem);
                    CurrentSprites.Normal = SetPal(nr.fs);
                    nr.Close();
                }
                if (nr.fe[(int)ShinyPalette.SelectedItem].Size == 72)
                {
                    nr.OpenEntry((int)ShinyPalette.SelectedItem);
                    CurrentSprites.Shiny = SetPal(nr.fs);
                    nr.Close();
                }
            }
            
            LoadImages();
            OpenPngs.Enabled = true;
        }

        void BasePalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled) return;
            if (nr.fe[(int)BasePalette.SelectedItem].Size == 72)
            {
                nr.OpenEntry((int)BasePalette.SelectedItem);
                CurrentSprites.Normal = SetPal(nr.fs);
                nr.Close();
            }         

            LoadImages();
        }

        void ShinyPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled) return;
            if (nr.fe[(int)ShinyPalette.SelectedItem].Size == 72)
            {
                nr.OpenEntry((int)ShinyPalette.SelectedItem);
                CurrentSprites.Shiny = SetPal(nr.fs);
                nr.Close();
            }

            LoadImages();
        }

        void BuildPictureBoxes(int x_start, int y_start)
        {
            Display = new PictureBox[2, 4];
            for (int i = 0; i < Display.GetLength(0); i++)
            {
                for (int j = 0; j < Display.GetLength(1); j++)
                {
                    Display[i, j] = new PictureBox();
                    Display[i, j].Size = new Size(320, 160);
                    Display[i, j].Location = new Point((x_start + 328 * i), (y_start + 168 * j));
                    Display[i, j].Name = "" + (2 * j + i);
                    Display[i, j].Click += (Picturebox_Click);
                    Controls.Add(Display[i, j]);
                }
            }
        }

        void LoadImages()
        {
            for (int i = 0; i < Display.GetLength(0); i++)
            {
                for (int j = 0; j < Display.GetLength(1); j++)
                {
                    Display[i, j].Image = null;
                }
            }
            if (CurrentSprites.Normal == null)
                return;
            if (CurrentSprites.Shiny == null)
                CurrentSprites.Shiny = CurrentSprites.Normal;
            Bitmap image = new Bitmap(160, 80, PixelFormat.Format8bppIndexed);
            for (int i = 0; i < 4; i++)
            {
                if (CurrentSprites.Sprites[i] != null)
                {
                    CurrentSprites.Sprites[i].Palette = CurrentSprites.Shiny;
                    Display[(i % 2), ((i / 2) + 2)].Image = new Bitmap(CurrentSprites.Sprites[i], 320, 160);
                    CurrentSprites.Sprites[i].Palette = CurrentSprites.Normal;
                    Display[(i % 2), (i / 2)].Image = new Bitmap(CurrentSprites.Sprites[i], 320, 160);
                }
            }
        }

        Bitmap CheckSize(Bitmap image, string filename, string name, int spritenumber = 2)
        {
            DialogResult yesno;
            IndexedBitmapHandler Handler = new IndexedBitmapHandler();
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                yesno = MessageBox.Show(filename + " is not 8bpp Indexed!  Attempt conversion?", "Incompatible image format", MessageBoxButtons.YesNo);
                if (yesno != DialogResult.Yes)
                    return null;
                image = Handler.Convert(image, PixelFormat.Format8bppIndexed);
                if (image == null)
                    return null;
                if ((image.PixelFormat != PixelFormat.Format8bppIndexed) || (image.Palette == null))
                {
                    MessageBox.Show("Conversion failed.", "Failed");
                    return null;
                }
            }
            if (((image.Height != 64) && (image.Height != 80)) || ((image.Width != 64) && (image.Width != 80) && (image.Width != 160)))
            {
                int imagescale = 0;

                if ((image.Width / 64 == image.Height / 64) && (image.Width % 64 == 0) && (image.Height % 64 == 0))
                    imagescale = image.Width / 64;
                if ((image.Width / 80 == image.Height / 80) && (image.Width % 80 == 0) && (image.Height % 80 == 0))
                    imagescale = image.Width / 80;
                if ((image.Width / 160 == image.Height / 80) && (image.Width % 160 == 0) && (image.Height % 80 == 0))
                    imagescale = image.Width / 160;
                if (imagescale > 1)
                {
                    yesno = MessageBox.Show(filename + " is too large.  Attempt to shrink?", "Too large", MessageBoxButtons.YesNo);
                    if (yesno == DialogResult.Yes)
                        image = Handler.ShrinkImage(image, imagescale, imagescale);
                    else
                        imagescale = 0;
                }
                
                if (imagescale == 0)
                {
                    yesno = MessageBox.Show(filename + " size not recognized. Use Canvas Splitter?", "Unrecognized size", MessageBoxButtons.YesNo);
                    if (yesno != DialogResult.Yes)
                        return null;
                    SizeChooser Chooser = new SizeChooser();
                    DialogResult success = Chooser.ShowDialog();
                    int sizeChoice = Chooser.choice;
                    Chooser.Dispose();
                    if (success == DialogResult.Cancel)
                        return null;
                    int a = 80;
                    int b = 80;
                    if (sizeChoice == 0)
                    {
                        a = 64;
                        b = 64;
                    }
                    if (sizeChoice == 2)
                        a = 160;
                    if ((image.Width < a) || (image.Height < b))
                    {
                        MessageBox.Show("Image is too small");
                        return null;
                    }
                    Bitmap[] tiles = Handler.Split(image, a, b);
                    SpriteCropper Cropper = new SpriteCropper(tiles, name);
                    success = Cropper.ShowDialog();
                    if (success == DialogResult.Cancel)
                        return null;
                    image = Cropper.Chosen;
                    Cropper.Dispose();
                }
            }
            
            //image.Palette = StandardizeColors(image);
            byte check = Handler.PaletteSize(image);
            if (check > 16)
            {
                yesno = MessageBox.Show("Image's palette contains more than sixteen colors.  Attempt to shrink?", "Improper palette size", MessageBoxButtons.YesNo);
                if (yesno == DialogResult.Yes)
                {
                    image = Handler.ShrinkPalette(image);
                    check = Handler.PaletteSize(image);
                    if (check > 16)
                        MessageBox.Show("Palette still too large.  Image will not save correctly.", "Failed");
                }
            }
            if (image.Height == 64 && image.Width == 64)
                image = Handler.Resize(image, 8, 8, 8, 8);
            if (image.Height == 80 && image.Width == 80)
            {
                if ((spritenumber < 2) && (RomInfo.gameFamily == RomInfo.GameFamilies.DP))
                    image = Handler.Resize(image, 0, 0, 0, 80);
                else
                    image = Handler.Concat(image, image);
            }
            if (image.Height == 80 && image.Width == 160)
                return image;
            return null;
        }

        Bitmap MakeImage(FileStream fs)
        {
            fs.Seek(48L, SeekOrigin.Current);
            BinaryReader binaryReader = new BinaryReader(fs);
            ushort[] array = new ushort[3200];
            for (int i = 0; i < 3200; i++)
            {
                array[i] = binaryReader.ReadUInt16();
            }
            uint num = array[0];
            if (RomInfo.gameFamily != RomInfo.GameFamilies.DP)
            {
                for (int j = 0; j < 3200; j++)
                {
                    unchecked
                    {
                        ushort[] array2;
                        IntPtr value;
                        (array2 = array)[(int)(value = (IntPtr)j)] = (ushort)(array2[(int)value] ^ (ushort)(num & 0xFFFF));
                        num *= 1103515245;
                        num += 24691;
                    }
                }
            }
            else
            {
                num = array[3199];
                for (int num2 = 3199; num2 >= 0; num2--)
                {
                    unchecked
                    {
                        ushort[] array2;
                        IntPtr value;
                        (array2 = array)[(int)(value = (IntPtr)num2)] = (ushort)(array2[(int)value] ^ (ushort)(num & 0xFFFF));
                        num *= 1103515245;
                        num += 24691;
                    }
                }
            }

            Bitmap r_bitmap = new Bitmap(160, 80, PixelFormat.Format8bppIndexed);
            rect = new Rectangle(0, 0, 160, 80);
            byte[] array3 = new byte[12800];
            for (int k = 0; k < 3200; k++)
            {
                array3[k * 4] = (byte)(array[k] & 0xF);
                array3[k * 4 + 1] = (byte)((array[k] >> 4) & 0xF);
                array3[k * 4 + 2] = (byte)((array[k] >> 8) & 0xF);
                array3[k * 4 + 3] = (byte)((array[k] >> 12) & 0xF);
            }
            BitmapData bitmapData = r_bitmap.LockBits(rect, ImageLockMode.WriteOnly, r_bitmap.PixelFormat);
            IntPtr scan = bitmapData.Scan0;
            Marshal.Copy(array3, 0, scan, 12800);
            r_bitmap.UnlockBits(bitmapData);
            Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format4bppIndexed);
            ColorPalette palette = bitmap.Palette;
            for (int l = 0; l < 16; l++)
            {
                palette.Entries[l] = Color.FromArgb(l << 4, l << 4, l << 4);
            }
            r_bitmap.Palette = palette;

            if (r_bitmap == null)
            {
                MessageBox.Show("MakeImage Failed");
                return null;
            }
            return r_bitmap;
        }

        ColorPalette SetPal(FileStream fs)
        {
            fs.Seek(40L, SeekOrigin.Current);
            ushort[] array = new ushort[16];
            BinaryReader binaryReader = new BinaryReader(fs);
            for (int i = 0; i < 16; i++)
            {
                array[i] = binaryReader.ReadUInt16();
            }
            Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format4bppIndexed);
            ColorPalette palette = bitmap.Palette;
            for (int j = 0; j < 16; j++)
            {
                palette.Entries[j] = Color.FromArgb((array[j] & 0x1F) << 3, ((array[j] >> 5) & 0x1F) << 3, ((array[j] >> 10) & 0x1F) << 3);
            }
            return palette;
        }

        void LoadSprites()
        {
            OpenPngs.Enabled = false;
            IndexBox.Items.Clear();
            int tot6448 = 0, tot72 = 0;
            if (!this.loadingOther)
            {
                nr = new NarcReader(RomInfo.gameDirs[DirNames.pokemonBattleSprites].packedDir);
                for (int i = 0; i < nr.Entrys; i += 6)
                {
                    IndexBox.Items.Add(this.pokenames[i / 6] + " (" + nr.fe[i].Size + ")");
                }
                IndexBox.SelectedIndex = 1;
            } else
            {
                nr = new NarcReader(RomInfo.gameDirs[DirNames.otherPokemonBattleSprites].packedDir);
                for (int i = 0; i < nr.Entrys; i += 2)
                {
                    IndexBox.Items.Add(this.otherPokenames[i/2] + " (" + nr.fe[i].Size + ")");
                }

                IndexBox.SelectedIndex = 0;
            }           

            
            
        }

        void Picturebox_Click(object sender, EventArgs e)
        {
            if (OpenPngs.Enabled == false)
                return;
            OpenPngs.Enabled = false;
            PictureBox source = sender as PictureBox;
            int index = Convert.ToInt32(source.Name);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose an image";
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Supported fomats: *.bmp, *.gif, *.png | *.bmp; *.gif; *.png";
            openFileDialog.ShowHelp = true;
            Bitmap image;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                OpenPngs.Enabled = true;
                return;
            }
            image = new Bitmap(openFileDialog.FileName);
            IndexedBitmapHandler Handler = new IndexedBitmapHandler();
            if (index > 3)
            {
                image = CheckSize(image, openFileDialog.FileName, "Shiny");
                if (image == null)
                {
                    OpenPngs.Enabled = true;
                    return;
                }
                ColorPalette temp = Handler.AlternatePalette(CurrentSprites.Sprites[index % 4], image);
                if (temp != null)
                    CurrentSprites.Shiny = temp;
                else
                    CurrentSprites.Shiny = image.Palette;
            }
            else
            {
                image = CheckSize(image, openFileDialog.FileName, names[index], index);
                if (image == null)
                {
                    OpenPngs.Enabled = true;
                    return;
                }
                bool match = Handler.PaletteEquals(CurrentSprites.Normal, image);
                if (!match)
                {
                    DialogResult yesno = MessageBox.Show("Image's palette does not match the current palette.  Use PaletteMatch?", "Palette mismatch", MessageBoxButtons.YesNo);
                    if (yesno == DialogResult.Yes)
                    {
                        image = Handler.PaletteMatch(CurrentSprites.Normal, image, used);
                        used = Handler.IsUsed(image, used);
                    }
                    else
                        used = Handler.IsUsed(image);
                    CurrentSprites.Normal = image.Palette;
                }
                CurrentSprites.Sprites[index] = image;
            }
            OpenPngs.Enabled = true;
            LoadImages();
        }

        void OpenPng_Click(object sender, EventArgs e)
        {
            if (OpenPngs.Enabled == false)
                return;
            OpenPngs.Enabled = false;
            LoadingForm Open = new LoadingForm();
            var result = Open.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                OpenPngs.Enabled = true;
                return;
            }
            string[] filenames = Open.files;
            bool Autofill = Open.result;
            int shinymatch = Open.shinymatch;
            bool paletteMatch = Open.paletteMatch;
            Open.Dispose();
            SpriteSet temp = new SpriteSet();
            Bitmap image;
            for (int i = 0; i < 4; i++)
            {
                if (filenames[i] == "")
                    continue;
                image = new Bitmap(filenames[i]);
                temp.Sprites[i] = CheckSize(image, filenames[i], names[i], i);
            }
            bool[] tempUsed = null;
            if (paletteMatch)
            {
                temp.Normal = CurrentSprites.Normal;
                tempUsed = used;
            }
            for (int i = 0; i < 4; i++)
            {
                if (temp.Sprites[i] == null)
                    continue;
                if (temp.Normal == null)
                {
                    temp.Normal = temp.Sprites[i].Palette;
                    tempUsed = Handler.IsUsed(temp.Sprites[i]);
                }
                else
                {
                    bool match = Handler.PaletteEquals(temp.Normal, temp.Sprites[i]);
                    if (!match)
                    {
                        temp.Sprites[i] = Handler.PaletteMatch(temp.Normal, temp.Sprites[i], tempUsed);
                        temp.Normal = temp.Sprites[i].Palette;
                    }
                    tempUsed = Handler.IsUsed(temp.Sprites[i], tempUsed);
                }
            }
            used = tempUsed;
            if (filenames[4] != "")
            {
                image = new Bitmap(filenames[4]);
                image = CheckSize(image, filenames[4], names[4], 4);
                if ((shinymatch < 4) && (temp.Sprites[shinymatch] != null))
                    temp.Shiny = Handler.AlternatePalette(temp.Sprites[shinymatch], image);
                else
                    temp.Shiny = image.Palette;
            }

            if (Autofill)
            {
                if (temp.Sprites[0] == null)
                    temp.Sprites[0] = temp.Sprites[1];
                if (temp.Sprites[1] == null)
                    temp.Sprites[1] = temp.Sprites[0];
                if (temp.Sprites[2] == null)
                    temp.Sprites[2] = temp.Sprites[3];
                if (temp.Sprites[3] == null)
                    temp.Sprites[3] = temp.Sprites[2];
                if (filenames[4] == "")
                    temp.Shiny = temp.Normal;
            }

            for (int i = 0; i < 4; i++)
            {
                if (temp.Sprites[i] != null)
                    CurrentSprites.Sprites[i] = temp.Sprites[i];
            }
            if (temp.Normal != null)
                CurrentSprites.Normal = temp.Normal;
            if (temp.Shiny != null)
                CurrentSprites.Shiny = temp.Shiny;

            LoadImages();
            OpenPngs.Enabled = true;
        }

        void SaveChanges_Click(object sender, EventArgs e)
        {
            if (OpenPngs.Enabled == false)
                return;
            int num = (IndexBox.Items.IndexOf(IndexBox.Text) * 6);
            for (int i = 0; i < 4; i++)
            {
                if (nr.fe[num + i].Size == 6448)
                {
                    nr.OpenEntry(num + i);
                    SaveBin(nr.fs, CurrentSprites.Sprites[i]);
                    nr.Close();
                }
            }
            if (nr.fe[num + 4].Size == 72)
            {
                nr.OpenEntry(num + 4);
                SavePal(nr.fs, CurrentSprites.Normal);
                nr.Close();
            }
            if (nr.fe[num + 5].Size == 72)
            {
                nr.OpenEntry(num + 5);
                SavePal(nr.fs, CurrentSprites.Shiny);
                nr.Close();
            }
        }

        void menuItem13_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Credit to loadingNOW and SCV for the original PokeDsPic and PokeDsPicPlatinum, without which this would never have happened.", "Credits");
        }

        protected void btnSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Image Set";
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.Filter = "*.png|*.png";
            saveFileDialog.ShowHelp = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                fileName = fileName.Replace(".png", "");
                bool ShinySaved = false;
                if (CurrentSprites.Sprites[2] != null)
                {
                    if (CurrentSprites.Shiny != null)
                    {
                        CurrentSprites.Sprites[2].Palette = CurrentSprites.Shiny;
                        SavePNG(CurrentSprites.Sprites[2], (fileName + "Shiny.png"));
                        ShinySaved = true;
                    }
                    CurrentSprites.Sprites[2].Palette = CurrentSprites.Normal;
                    SavePNG(CurrentSprites.Sprites[2], (fileName + "FFront.png"));
                }
                if (CurrentSprites.Sprites[3] != null)
                {
                    if ((CurrentSprites.Shiny != null) && (!ShinySaved))
                    {
                        CurrentSprites.Sprites[3].Palette = CurrentSprites.Shiny;
                        SavePNG(CurrentSprites.Sprites[3], (fileName + "Shiny.png"));
                        ShinySaved = true;
                    }
                    CurrentSprites.Sprites[3].Palette = CurrentSprites.Normal;
                    SavePNG(CurrentSprites.Sprites[3], (fileName + "MFront.png"));
                }
                if (CurrentSprites.Sprites[0] != null)
                {
                    if ((CurrentSprites.Shiny != null) && (!ShinySaved))
                    {
                        CurrentSprites.Sprites[0].Palette = CurrentSprites.Shiny;
                        SavePNG(CurrentSprites.Sprites[0], (fileName + "Shiny.png"));
                        ShinySaved = true;
                    }
                    CurrentSprites.Sprites[0].Palette = CurrentSprites.Normal;
                    SavePNG(CurrentSprites.Sprites[0], (fileName + "FBack.png"));
                }
                if (CurrentSprites.Sprites[1] != null)
                {
                    if ((CurrentSprites.Shiny != null) && (!ShinySaved))
                    {
                        CurrentSprites.Sprites[1].Palette = CurrentSprites.Shiny;
                        SavePNG(CurrentSprites.Sprites[1], (fileName + "Shiny.png"));
                    }
                    CurrentSprites.Sprites[1].Palette = CurrentSprites.Normal;
                    SavePNG(CurrentSprites.Sprites[1], (fileName + "MBack.png"));
                }
            }
        }

        void SaveSingle_Click(object sender, EventArgs e)
        {
            int index = SaveBox.SelectedIndex;
            if (CurrentSprites.Sprites[index % 4] == null)
            {
                MessageBox.Show("Image is empty.");
                return;
            }
            string selected = SaveBox.Text;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save As PNG";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.Filter = "*.png|*.png";
            saveFileDialog.ShowHelp = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                Bitmap image = CurrentSprites.Sprites[index % 4];
                if (index > 3)
                    image.Palette = CurrentSprites.Shiny;
                else
                    image.Palette = CurrentSprites.Normal;
                SavePNG(image, fileName);
            }
        }

        void btnOpenOther_Click(object sender, EventArgs e)
        {
            Helpers.DisableHandlers();
            this.loadingOther = true;
            BasePalette.Enabled = true;
            ShinyPalette.Enabled = true;
            BasePalette.Visible = true;
            ShinyPalette.Visible = true;
            BasePalette.SelectedIndex = 0;
            ShinyPalette.SelectedIndex = 0;
            LoadSprites();
            Helpers.EnableHandlers();
        }

        void btnLoadSheet_Click(object sender, EventArgs e)
        {
            if (OpenPngs.Enabled == false)
                return;
            OpenPngs.Enabled = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a sprite sheet";
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Supported fomats: *.bmp, *.gif, *.png | *.bmp; *.gif; *.png";
            openFileDialog.ShowHelp = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                OpenPngs.Enabled = true;
                return;
            }
            Bitmap image = new Bitmap(openFileDialog.FileName);
            if ((image.Width != 256) || (image.Height != 64))
            {
                MessageBox.Show("The sprite sheet should be 256x64.");
                return;
            }
            IndexedBitmapHandler Handler = new IndexedBitmapHandler();
            image = Handler.Convert(image, PixelFormat.Format8bppIndexed);
            image.Palette = StandardizeColors(image);
            Bitmap[] tiles = Handler.Split(image, 64, 64);
            SpriteSet sprites = new SpriteSet();
            bool[] used = Handler.IsUsed(tiles[0]);
            used = Handler.IsUsed(tiles[2], used);
            Bitmap temp = Handler.ShrinkPalette(tiles[0], used);
            sprites.Normal = temp.Palette;
            temp = Handler.Resize(temp, 8, 8, 8, 8);
            temp = Handler.Concat(temp, temp);
            sprites.Sprites[2] = temp;
            sprites.Sprites[3] = temp;
            temp = Handler.ShrinkPalette(tiles[2], used);
            temp = Handler.Resize(temp, 8, 8, 8, 8);
            if (RomInfo.gameFamily == RomInfo.GameFamilies.DP)
                temp = Handler.Resize(temp, 0, 0, 0, 80);
            else
                temp = Handler.Concat(temp, temp);
            sprites.Sprites[0] = temp;
            sprites.Sprites[1] = temp;
            temp = Handler.ShrinkPalette(tiles[1], used);
            temp = Handler.Resize(temp, 8, 8, 8, 8);
            temp = Handler.Concat(temp, temp);
            sprites.Shiny = Handler.AlternatePalette(sprites.Sprites[2], temp);
            CurrentSprites = sprites;
            OpenPngs.Enabled = true;
            LoadImages();
        }

        void MakeShiny_Click(object sender, EventArgs e)
        {
            if (OpenPngs.Enabled == false)
                return;
            OpenPngs.Enabled = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose the base image";
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Supported fomats: *.bmp, *.gif, *.png | *.bmp; *.gif; *.png";
            openFileDialog.ShowHelp = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                OpenPngs.Enabled = true;
                return;
            }
            string filename = openFileDialog.FileName;
            openFileDialog.Title = "Choose the shiny image";
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Supported fomats: *.bmp, *.gif, *.png | *.bmp; *.gif; *.png";
            openFileDialog.ShowHelp = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                OpenPngs.Enabled = true;
                return;
            }
            Bitmap parent = new Bitmap(filename);
            Bitmap child = new Bitmap(openFileDialog.FileName);
            IndexedBitmapHandler Handler = new IndexedBitmapHandler();
            ColorPalette temp = Handler.AlternatePalette(parent, child);
            if (temp != null)
                CurrentSprites.Shiny = temp;
            else
                MessageBox.Show("Failed!", "Failed");
            OpenPngs.Enabled = true;
            LoadImages();
        }

        void menuCheck_Click(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            item.Checked = !item.Checked;
        }

        ColorPalette StandardizeColors(Bitmap image)
        {
            ColorPalette pal = image.Palette;
            bool OffColor = false;
            for (int i = 0; i < pal.Entries.Length; i++)
            {
                if ((pal.Entries[i].R % 8 != 0) || (pal.Entries[i].G % 8 != 0) || (pal.Entries[i].B % 8 != 0))
                    OffColor = true;
            }
            if (OffColor)
            {
                //				yesno = MessageBox.Show("Colors are not appropriately formatted for storage.  Fix?", "Incompatible colors", MessageBoxButtons.YesNo);
                //				if(yesno != DialogResult.Yes)
                //					MessageBox.Show("Colors will not store correctly.  Image may look different in-game.", "Failed");
                for (int i = 0; i < pal.Entries.Length; i++)
                {
                    byte r = (byte)(pal.Entries[i].R - (pal.Entries[i].R % 8));
                    byte g = (byte)(pal.Entries[i].G - (pal.Entries[i].G % 8));
                    byte b = (byte)(pal.Entries[i].B - (pal.Entries[i].B % 8));
                    pal.Entries[i] = Color.FromArgb(r, g, b);
                }
            }
            return pal;
        }

        void SavePNG(Bitmap image, string filename)
        {
            IndexedBitmapHandler Handler = new IndexedBitmapHandler();
            byte[] array = Handler.GetArray(image);
            Bitmap temp = Handler.MakeImage(image.Width, image.Height, array, image.PixelFormat);
            ColorPalette cleaned = Handler.CleanPalette(image);
            temp.Palette = cleaned;
            temp.Save(filename, ImageFormat.Png);
        }

        void SaveBin(FileStream fs, Bitmap source)
        {
            BinaryWriter binaryWriter = new BinaryWriter(fs);
            rect = new Rectangle(0, 0, 160, 80);
            BitmapData bitmapData = source.LockBits(rect, ImageLockMode.ReadOnly, source.PixelFormat);
            IntPtr scan = bitmapData.Scan0;
            byte[] array = new byte[12800];
            Marshal.Copy(scan, array, 0, 12800);
            source.UnlockBits(bitmapData);
            ushort[] array2 = new ushort[3200];
            for (int i = 0; i < 3200; i++)
            {
                array2[i] = (ushort)((array[i * 4] & 0xF) | ((array[i * 4 + 1] & 0xF) << 4) | ((array[i * 4 + 2] & 0xF) << 8) | ((array[i * 4 + 3] & 0xF) << 12));
            }
            uint num = 0u;
            if (RomInfo.gameFamily != RomInfo.GameFamilies.DP)
            {
                for (int j = 0; j < 3200; j++)
                {
                    unchecked
                    {
                        ushort[] array3;
                        IntPtr value;
                        (array3 = array2)[(int)(value = (IntPtr)j)] = (ushort)(array3[(int)value] ^ (ushort)(num & 0xFFFF));
                        num *= 1103515245;
                        num += 24691;
                    }
                }
            }
            else
            {
                num = 31315u;
                for (int num2 = 3199; num2 >= 0; num2--)
                {
                    num += array2[num2];
                }
                for (int num3 = 3199; num3 >= 0; num3--)
                {
                    unchecked
                    {
                        ushort[] array3;
                        IntPtr value;
                        (array3 = array2)[(int)(value = (IntPtr)num3)] = (ushort)(array3[(int)value] ^ (ushort)(num & 0xFFFF));
                        num *= 1103515245;
                        num += 24691;
                    }
                }
            }
            byte[] array4 = new byte[48]
            {82, 71, 67, 78, 255, 254, 0, 1, 48, 25, 0, 0, 16, 0, 1, 0, 82, 65, 72, 67, 32, 25, 0, 0, 10, 0, 20, 0, 3, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 25, 0, 0, 24, 0, 0, 0};
            for (int k = 0; k < 48; k++)
            {
                binaryWriter.Write(array4[k]);
            }
            for (int l = 0; l < 3200; l++)
            {
                binaryWriter.Write(array2[l]);
            }
        }

        void SavePal(FileStream fs, ColorPalette palette)
        {
            byte[] buffer = new byte[40]
            {82, 76, 67, 78, 255, 254, 0, 1, 72, 0, 0, 0, 16, 0, 1, 0, 84, 84, 76, 80, 56, 0, 0, 0, 4, 0, 10, 0, 0, 0, 0, 0, 32, 0, 0, 0, 16, 0, 0, 0};
            BinaryWriter binaryWriter = new BinaryWriter(fs);
            binaryWriter.Write(buffer, 0, 40);
            ushort[] array = new ushort[16];
            for (int i = 0; i < 16; i++)
            {
                array[i] = (ushort)(((palette.Entries[i].R >> 3) & 0x1F) | (((palette.Entries[i].G >> 3) & 0x1F) << 5) | (((palette.Entries[i].B >> 3) & 0x1F) << 10));
            }
            for (int j = 0; j < 16; j++)
            {
                binaryWriter.Write(array[j]);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }

    public class IndexedBitmapHandler
    {
        public IndexedBitmapHandler()
        {
        }

        public Bitmap Convert(Bitmap source, PixelFormat target)
        {
            if ((source.PixelFormat == target) || (target == PixelFormat.DontCare))
                return source;
            Bitmap image = new Bitmap(source.Width, source.Height, target);
            if ((source.PixelFormat == PixelFormat.Format4bppIndexed) && (target == PixelFormat.Format8bppIndexed))
            {
                byte[] array = GetArray(source);
                byte[] array2 = new byte[(array.Length * 2)];
                byte current;
                for (int i = 0; i < array.Length; i++)
                {
                    current = array[i];
                    array2[2 * i] = (byte)((current & 0xF0) >> 4);
                    array2[2 * i + 1] = (byte)((current & 0x0F));
                }
                image = MakeImage(source.Width, source.Height, array2, target);
                image.Palette = source.Palette;
            }
            if (((source.PixelFormat == PixelFormat.Format24bppRgb) || (source.PixelFormat == PixelFormat.Format32bppArgb)) && (target == PixelFormat.Format8bppIndexed))
            {
                int skip = 3;
                if (source.PixelFormat == PixelFormat.Format32bppArgb)
                    skip = 4;
                Color[] newPalette = new Color[256];
                byte[] array = new byte[source.Width * source.Height];
                byte[] sourceArray = GetArray(source);
                Color Pixel = Color.FromArgb(sourceArray[2], sourceArray[1], sourceArray[0]);
                newPalette[0] = Pixel;
                int index = 1;
                array[0] = 0;
                byte r = 0;
                byte g = 0;
                byte b = 0;
                bool match = false;
                for (int i = 1; i < (source.Width * source.Height); i++)
                {
                    b = sourceArray[(i * skip)];
                    g = sourceArray[(i * skip + 1)];
                    r = sourceArray[(i * skip + 2)];
                    Pixel = Color.FromArgb(r, g, b);
                    for (int j = 0; j < index; j++)
                    {
                        if (Pixel == newPalette[j])
                        {
                            array[i] = (byte)j;
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                    {
                        if (index >= 256)
                        {
                            MessageBox.Show("Conversion failed");
                            return null;
                        }
                        newPalette[index] = Pixel;
                        array[i] = (byte)index;
                        index++;
                    }
                    match = false;
                }
                image = MakeImage(source.Width, source.Height, array, target);
                ColorPalette oldPalette = image.Palette;
                for (int i = 0; i < index; i++)
                {
                    oldPalette.Entries[i] = newPalette[i];
                }
                image.Palette = oldPalette;
            }
            return image;
        }

        public Bitmap Resize(Bitmap source, int top, int bottom, int left, int right)
        {
            int newWidth = source.Width + left + right;
            int newHeight = source.Height + top + bottom;
            int arraysize = newWidth * newHeight;
            ColorPalette palette = source.Palette;

            byte[] source_array = GetArray(source);

            byte[] byte_array = new byte[arraysize];
            byte background = source_array[0];

            int source_index = 0;
            int index = 0;
            for (int j = 0; j < newHeight; j++)
            {
                for (int i = 0; i < newWidth; i++)
                {
                    if (i < left || i >= (left + source.Width) || j < top || j >= (top + source.Height))
                        byte_array[index] = background;
                    else
                    {
                        source_index = ((j - top) * source.Width) + i - left;
                        byte_array[index] = source_array[source_index];
                    }
                    index++;
                }
            }
            Bitmap newImage = MakeImage(newWidth, newHeight, byte_array, PixelFormat.Format8bppIndexed);
            newImage.Palette = palette;
            return newImage;
        }

        public Bitmap Concat(Bitmap first, Bitmap second)
        {
            int newWidth = first.Width + second.Width;
            int newHeight = first.Height;
            if (first.Height < second.Height)
                newHeight = second.Height;
            int arraysize = newWidth * newHeight;
            ColorPalette palette = first.Palette;

            byte[] first_array = GetArray(first);
            byte[] second_array = GetArray(second);

            byte[] byte_array = new byte[arraysize];
            byte background = first_array[0];

            int source_index = 0;
            int index = 0;
            for (int j = 0; j < newHeight; j++)
            {
                for (int i = 0; i < first.Width; i++)
                {
                    if (j > first.Height)
                        byte_array[index] = background;
                    else
                    {
                        source_index = ((j * first.Width) + i);
                        byte_array[index] = first_array[source_index];
                    }
                    index++;
                }
                for (int i = 0; i < second.Width; i++)
                {
                    if (j > second.Height)
                        byte_array[index] = background;
                    else
                    {
                        source_index = ((j * second.Width) + i);
                        byte_array[index] = second_array[source_index];
                    }
                    index++;
                }
            }
            Bitmap newImage = MakeImage(newWidth, newHeight, byte_array, PixelFormat.Format8bppIndexed);
            newImage.Palette = palette;
            return newImage;
        }

        public Bitmap[] Split(Bitmap source, int tilewidth, int tileheight)
        {
            int maxTiles = (source.Width / tilewidth) * (source.Height / tileheight);
            Bitmap[] tiles = new Bitmap[maxTiles];
            int index = 0;
            int x_index = 0;
            int y_index = 0;
            while (y_index + tileheight <= source.Height)
            {
                if (x_index + tilewidth <= source.Width)
                {
                    tiles[index] = Resize(source, (y_index * -1), ((source.Height - y_index - tileheight) * -1), (x_index * -1), ((source.Width - x_index - tilewidth) * -1));
                    index++;
                }
                x_index += tilewidth;
                if (x_index + tilewidth > source.Width)
                {
                    y_index += tileheight;
                    x_index = 0;
                }
            }
            return tiles;
        }

        public Bitmap ShrinkImage(Bitmap image, int x_scale, int y_scale)
        {
            byte[] array = GetArray(image);
            ColorPalette palette = image.Palette;
            int new_width = image.Width / x_scale;
            int new_height = image.Height / y_scale;
            byte[] array2 = new byte[new_width * new_height];
            for (int j = 0; j < new_height; j++)
            {
                for (int i = 0; i < new_width; i++)
                {
                    array2[j * new_width + i] = array[y_scale * j * image.Width + i * x_scale];
                }
            }
            Bitmap temp = MakeImage(new_width, new_height, array2, image.PixelFormat);
            temp.Palette = palette;
            return temp;
        }

        public ColorPalette AlternatePalette(Bitmap parent, Bitmap child)
        {
            Bitmap temp = new Bitmap(1, 1, parent.PixelFormat);
            ColorPalette newPalette = temp.Palette;
            ColorPalette ChildPalette = child.Palette;
            byte[] ParentArray = GetArray(parent);
            byte[] ChildArray = GetArray(child);
            if (ParentArray.Length != ChildArray.Length)
                return null;
            for (int i = 0; i < ChildPalette.Entries.Length; i++)
            {
                for (int j = 0; j < ParentArray.Length; j++)
                {
                    if (ParentArray[j] == i)
                    {
                        newPalette.Entries[i] = ChildPalette.Entries[ChildArray[j]];
                        break;
                    }
                }
            }
            return newPalette;
        }

        public Bitmap PaletteMatch(ColorPalette parent, Bitmap child, bool[] used = null)
        {
            if (parent.Entries == child.Palette.Entries)
                return child;
            if (used == null)
            {
                used = new bool[parent.Entries.Length];
                for (int i = 0; i < parent.Entries.Length; i++)
                    used[i] = true;
            }
            Bitmap image = ShrinkPalette(child);
            Bitmap temp = new Bitmap(1, 1, child.PixelFormat);
            ColorPalette childPalette = image.Palette;
            ColorPalette newPalette = temp.Palette;
            int size = PaletteSize(image);
            byte[] array = GetArray(image);
            byte[] indexof = new byte[size];
            bool[] NotFound = new bool[size];
            for (int i = 0; i < size; i++)
                NotFound[i] = true;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < parent.Entries.Length; j++)
                {
                    if (parent.Entries[j] == childPalette.Entries[i])
                    {
                        indexof[i] = (byte)j;
                        NotFound[i] = false;
                        break;
                    }
                }
            }
            int maxsize = 0;
            for (int i = 0; i < parent.Entries.Length; i++)
            {
                if (used[i])
                {
                    newPalette.Entries[i] = parent.Entries[i];
                    maxsize++;
                }
            }
            for (int i = 0; i < size; i++)
            {
                if (NotFound[i])
                    maxsize++;
            }
            for (int i = 0; i < maxsize; i++)
            {
                if ((i < used.Length) && (used[i]))
                    continue;
                for (int j = 0; j < size; j++)
                {
                    if (NotFound[j])
                    {
                        indexof[j] = (byte)i;
                        newPalette.Entries[i] = childPalette.Entries[j];
                        NotFound[j] = false;
                        break;
                    }
                }
            }
            byte[] newArray = new byte[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = indexof[array[i]];
            }
            image = MakeImage(child.Width, child.Height, newArray, child.PixelFormat);
            image.Palette = newPalette;
            return image;
        }

        public Bitmap ShrinkPalette(Bitmap image, bool[] used = null)
        {
            used = IsUsed(image, used);
            byte[] array = GetArray(image);
            ColorPalette oldPalette = image.Palette;
            Bitmap temp = new Bitmap(image.Width, image.Height, image.PixelFormat);
            ColorPalette newPalette = temp.Palette;
            int size = oldPalette.Entries.Length;
            int index = 0;
            int unused = 0;
            byte[] indexof = new byte[size];
            for (int i = 0; i < size; i++)
            {
                if (used[i])
                {
                    newPalette.Entries[index] = oldPalette.Entries[i];
                    indexof[i] = (byte)index;
                    index++;
                }
                else
                    unused++;
            }
            if (unused == 0 || unused == size)
                return image;
            byte[] newArray = new byte[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = indexof[array[i]];
            }
            image = MakeImage(image.Width, image.Height, newArray, image.PixelFormat);
            image.Palette = newPalette;
            return image;
        }

        public bool[] IsUsed(Bitmap image, bool[] used = null)
        {
            byte[] array = GetArray(image);
            int size = image.Palette.Entries.Length;
            if (used == null)
            {
                used = new bool[size];
                for (int i = 0; i < size; i++)
                {
                    used[i] = false;
                }
            }
            if (size > used.Length)
            {
                bool[] temp = new bool[size];
                for (int i = 0; i < size; i++)
                {
                    if (i < used.Length)
                        temp[i] = used[i];
                    else
                        temp[i] = false;
                }
                used = temp;
            }
            for (int i = 0; i < array.Length; i++)
            {
                used[array[i]] = true;
            }
            return used;
        }

        public byte PaletteSize(Bitmap image)
        {
            byte[] array = GetArray(image);
            byte max = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > max)
                    max = array[i];
            }
            return (byte)(max + 1);
        }

        public bool PaletteEquals(ColorPalette parent, Bitmap child)
        {
            bool Match = true;
            int max = PaletteSize(child);
            for (int i = 0; i < max; i++)
            {
                if (parent.Entries[i] != child.Palette.Entries[i])
                    Match = false;
            }
            return Match;
        }

        public ColorPalette CleanPalette(Bitmap image)
        {
            int used = PaletteSize(image);
            ColorPalette temp = image.Palette;
            for (int i = used; i < image.Palette.Entries.Length; i++)
                temp.Entries[i] = Color.FromArgb(0, 0, 0);
            return temp;
        }

        public byte[] GetArray(Bitmap target)
        {
            int Width = target.Width;
            if (target.PixelFormat == PixelFormat.Format1bppIndexed)
                Width = Width / 8;
            if (target.PixelFormat == PixelFormat.Format4bppIndexed)
                Width = Width / 2;
            if ((target.PixelFormat == PixelFormat.Format16bppArgb1555) || (target.PixelFormat == PixelFormat.Format16bppGrayScale) || (target.PixelFormat == PixelFormat.Format16bppRgb555) || (target.PixelFormat == PixelFormat.Format16bppRgb565))
                Width = Width * 2;
            if (target.PixelFormat == PixelFormat.Format24bppRgb)
                Width = Width * 3;
            if ((target.PixelFormat == PixelFormat.Format32bppRgb) || (target.PixelFormat == PixelFormat.Format32bppArgb) || (target.PixelFormat == PixelFormat.Format32bppPArgb))
                Width = Width * 4;
            byte[] array = new byte[Width * target.Height];
            Rectangle rect = new Rectangle(0, 0, target.Width, target.Height);
            BitmapData sourceData = target.LockBits(rect, ImageLockMode.ReadOnly, target.PixelFormat);
            IntPtr scan = sourceData.Scan0;
            Marshal.Copy(scan, array, 0, Width * target.Height);
            target.UnlockBits(sourceData);
            return array;
        }

        public Bitmap MakeImage(int width, int height, byte[] array, PixelFormat format)
        {
            Bitmap image = new Bitmap(width, height, format);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData imageData = image.LockBits(rect, ImageLockMode.WriteOnly, image.PixelFormat);
            IntPtr scan = imageData.Scan0;
            Marshal.Copy(array, 0, scan, (width * height));
            image.UnlockBits(imageData);
            return image;
        }
    }
}

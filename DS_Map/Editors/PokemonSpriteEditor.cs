using DSPRE.Editors.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.Editors
{
    public struct FileEntry
    {
        public int Ofs;
        public int Size;
    }

    public partial class PokemonSpriteEditor : Form
    {
        private readonly string[] pokenames;
        private readonly int[] formPalettes = new int[]
        {
            158, 158, 158, 158,
            160, 160, 160, 160, 160, 160, 160, 160, 160, 160,
            160, 160, 160, 160, 160, 160, 160, 160, 160, 160,
            160, 160, 160, 160, 160, 160, 160, 160, 160, 160,
            162, 162, 162, 162,
            164, 164, 164,
            166, 166, 166,
            168, 168, 168, 168,
            170, 170, 170, 170,
            172, 172, 172, 172,
            174, 174, 174, 174, 174, 174, 174, 174,
            174, 174, 174, 174, 174, 174, 174, 174,
            174, 174, 174, 174, 174, 174, 174, 174,
            174, 174, 174, 174, 174, 174, 174, 174,
            174, 174, 174, 174, 174, 174, 174, 174,
            176, 176,
            178, 178, 178, 178,
            180, 180, 180, 180, 180, 180, 180, 180,
            180, 180, 180, 180, 180, 180, 180, 180,
            182, 182, 182, 182,
            184, 184, 184,
            186, 186
        };

        private readonly int[] shinyPalettes = new int[]
        {
            159, 159, 159, 159,
            161, 161, 161, 161, 161, 161, 161, 161, 161, 161,
            161, 161, 161, 161, 161, 161, 161, 161, 161, 161,
            161, 161, 161, 161, 161, 161, 161, 161, 161, 161,
            163, 163, 163, 163,
            165, 165, 165,
            167, 167, 167,
            169, 169, 169, 169,
            171, 171, 171, 171,
            173, 173, 173, 173,
            175, 175, 175, 175, 175, 175, 175, 175,
            175, 175, 175, 175, 175, 175, 175, 175,
            175, 175, 175, 175, 175, 175, 175, 175,
            175, 175, 175, 175, 175, 175, 175, 175,
            175, 175, 175, 175, 175, 175, 175, 175,
            177, 177,
            179, 179, 179, 179,
            181, 181, 181, 181, 181, 181, 181, 181,
            181, 181, 181, 181, 181, 181, 181, 181,
            183, 183, 183, 183,
            185, 185, 185,
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
            206, 207, 210, 212
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

        private static string[] names = { "Female backsprite", "Male backsprite", "Female frontsprite", "Male frontsprite", "Shiny" };
        private bool loadingOther = false;
        private PokemonEditor _parent;
        public bool dirty = false;
        private static readonly string formName = "Sprite Editor";
        private NarcReader nr;
        private PictureBox[,] Display;
        private bool[] used;
        private Rectangle rect;
        private IndexedBitmapHandler Handler;
        private SpriteSet CurrentSprites;
        private int currentLoadedId;

        public PokemonSpriteEditor(Control parent, PokemonEditor pokeEditor)
        {
            this._parent = pokeEditor;
            InitializeComponent();
            this.Text = formName;
            SetupPictureBoxes();
            int[] source = RomInfo.gameFamily == GameFamilies.Plat ? validPalettesPt : RomInfo.gameFamily == GameFamilies.DP ? validPalettesDP : validPalettesHGSS;
            foreach (var item in source)
            {
                BasePalette.Items.Add(item);
                ShinyPalette.Items.Add(item);
            }

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            this.pokenames = RomInfo.GetPokemonNames();
            SaveBox.SelectedIndex = 0;
            Helpers.DisableHandlers();
            LoadSprites();
            Helpers.EnableHandlers();
            IndexBox.SelectedIndex = 1;
        }

        public bool CheckDiscardChanges()
        {
            if (!dirty)
            {
                return true;
            }

            DialogResult res = MessageBox.Show("Sprite Editor\nThere are unsaved changes to the current Sprite data.\nDiscard and proceed?", "Sprite Editor - Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res.Equals(DialogResult.Yes))
            {
                return true;
            }

            IndexBox.SelectedIndex = currentLoadedId;

            return false;
        }

        private void setDirty(bool status)
        {
            if (status)
            {
                dirty = true;
                this.Text = formName + "*";
            }
            else
            {
                dirty = false;
                this.Text = formName;
            }
            _parent.UpdateTabPageNames();
        }

        private void IndexBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Update();
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            if (!loadingOther)
            {
                this._parent.TrySyncIndices((ComboBox)sender);
            }
            Helpers.DisableHandlers();
            if (CheckDiscardChanges())
            {
                ChangeLoadedFile(((ComboBox)sender).SelectedIndex);
            }
            Helpers.EnableHandlers();
        }

        public void ChangeLoadedFile(int toLoad)
        {
            currentLoadedId = toLoad;
            Helpers.DisableHandlers();
            IndexBox.SelectedIndex = toLoad;
            Helpers.EnableHandlers();
            CurrentSprites = new SpriteSet();
            int selectedIndex = toLoad;
            if (!this.loadingOther)
            {
                int num = selectedIndex * 6;
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
            }
            else
            {
                Helpers.DisableHandlers();
                BasePalette.SelectedItem = formPalettes[selectedIndex];
                ShinyPalette.SelectedItem = shinyPalettes[selectedIndex];
                Helpers.EnableHandlers();
                int num = selectedIndex * 2;
                for (int i = 0; i < 2; i++)
                {
                    if (nr.fe[num + i].Size == 6448)
                    {
                        nr.OpenEntry(num + i);
                        CurrentSprites.Sprites[i * 2 + 1] = MakeImage(nr.fs);
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
            setDirty(false);
        }

        private void BasePalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled) return;
            if (nr.fe[(int)BasePalette.SelectedItem].Size == 72)
            {
                nr.OpenEntry((int)BasePalette.SelectedItem);
                CurrentSprites.Normal = SetPal(nr.fs);
                nr.Close();
            }
            LoadImages();
            setDirty(true);
        }

        private void ShinyPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled) return;
            if (nr.fe[(int)ShinyPalette.SelectedItem].Size == 72)
            {
                nr.OpenEntry((int)ShinyPalette.SelectedItem);
                CurrentSprites.Shiny = SetPal(nr.fs);
                nr.Close();
            }
            LoadImages();
            setDirty(true);
        }

        private void SetupPictureBoxes()
        {
            Display = new PictureBox[2, 4];

            femaleBackNormalPic.Name = "0";
            Display[0, 0] = femaleBackNormalPic;

            maleBackNormalPic.Name = "1";
            Display[1, 0] = maleBackNormalPic;

            femaleFrontNormalPic.Name = "2";
            Display[0, 1] = femaleFrontNormalPic;

            maleFrontNormalPic.Name = "3";
            Display[1, 1] = maleFrontNormalPic;

            femaleBackShinyPic.Name = "4";
            Display[0, 2] = femaleBackShinyPic;

            maleBackShinyPic.Name = "5";
            Display[1, 2] = maleBackShinyPic;

            femaleFrontShinyPic.Name = "6";
            Display[0, 3] = femaleFrontShinyPic;

            maleFrontShinyPic.Name = "7";
            Display[1, 3] = maleFrontShinyPic;
        }

        private void LoadImages()
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

        private Bitmap CheckSize(Bitmap image, string filename, string name, int spritenumber = 2)
        {
            DialogResult yesno;
            IndexedBitmapHandler Handler = new IndexedBitmapHandler();
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                yesno = MessageBox.Show(filename + " is not 8bpp Indexed! Attempt conversion?", "Incompatible image format", MessageBoxButtons.YesNo);
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
                    yesno = MessageBox.Show(filename + " is too large. Attempt to shrink?", "Image too large", MessageBoxButtons.YesNo);
                    if (yesno != DialogResult.Yes)
                        return null;
                    image = Handler.Resize(image, 0, 0, imagescale, imagescale);
                }
                else
                {
                    MessageBox.Show(filename + " is wrong size. Must be 64x64, 80x80 or 160x80.", "Wrong size");
                    return null;
                }
            }
            if (image.Width == 64)
                image = Handler.Resize(image, 48, 8, 0, 0);
            if (image.Height == 64)
                image = Handler.Resize(image, 0, 0, 0, 16);
            if (image.Width == 80)
                image = Handler.Resize(image, 40, 0, 0, 0);
            if (image.Palette.Entries.Length > 16)
            {
                MessageBox.Show(filename + " has too many colors. Must have 16 or less.", "Too many colors");
                return null;
            }
            return image;
        }

        private void OpenPngs_Click(object sender, EventArgs e)
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
                    DialogResult yesno = MessageBox.Show("Image's palette does not match the current palette. Use PaletteMatch?", "Palette mismatch", MessageBoxButtons.YesNo);
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
            setDirty(true);
        }

        private void SaveChanges_Click(object sender, EventArgs e)
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
            setDirty(false);
        }

        // Credit to loadingNOW and SCV for the original PokeDsPic and PokeDsPicPlatinum, without which this would never have happened.
        // In addition to G4SpriteEditor

        private void btnSaveAs_Click(object sender, EventArgs e)
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

        private void SaveSingle_Click(object sender, EventArgs e)
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

        private void btnOpenOther_Click(object sender, EventArgs e)
        {
            if (!CheckDiscardChanges())
            {
                return;
            }
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

        private void btnLoadSheet_Click(object sender, EventArgs e)
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
            setDirty(true);
        }

        private void MakeShiny_Click(object sender, EventArgs e)
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
            setDirty(true);
        }

        private ColorPalette StandardizeColors(Bitmap image)
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

        private void SavePNG(Bitmap image, string filename)
        {
            IndexedBitmapHandler Handler = new IndexedBitmapHandler();
            byte[] array = Handler.GetArray(image);
            Bitmap temp = Handler.MakeImage(image.Width, image.Height, array, image.PixelFormat);
            ColorPalette cleaned = Handler.CleanPalette(image);
            temp.Palette = cleaned;
            temp.Save(filename, ImageFormat.Png);
        }

        private Bitmap MakeImage(FileStream fs)
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

        private ColorPalette SetPal(FileStream fs)
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

        private void LoadSprites()
        {
            nr = new NarcReader(RomInfo.gameDirs[DirNames.pokemonBattleSprites].packedDir);
            used = new bool[nr.fe.Length];
            for (int i = 0; i < nr.fe.Length; i++)
            {
                used[i] = (nr.fe[i].Size > 0);
            }
            if (!loadingOther)
            {
                IndexBox.Items.Clear();
                for (int i = 0; i < pokenames.Length; i++)
                {
                    IndexBox.Items.Add(i.ToString("D3") + " " + pokenames[i]);
                }
                IndexBox.SelectedIndex = 1;
            }
            else
            {
                IndexBox.Items.Clear();
                for (int i = 0; i < otherPokenames.Length; i++)
                {
                    IndexBox.Items.Add(otherPokenames[i]);
                }
                IndexBox.SelectedIndex = 0;
            }
        }

        private void SaveBin(FileStream fs, Bitmap source)
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
            {
                82, 71, 67, 78, 255, 254, 0, 1, 48, 25, 0, 0, 16, 0, 1, 0,
                82, 65, 72, 67, 32, 25, 0, 0, 10, 0, 20, 0, 3, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0, 0, 25, 0, 0, 24, 0, 0, 0
            };
            for (int k = 0; k < 48; k++)
            {
                binaryWriter.Write(array4[k]);
            }
            for (int l = 0; l < 3200; l++)
            {
                binaryWriter.Write(array2[l]);
            }
        }

        private void SavePal(FileStream fs, ColorPalette palette)
        {
            byte[] buffer = new byte[40]
            {
                82, 76, 67, 78, 255, 254, 0, 1, 72, 0, 0, 0, 16, 0, 1, 0,
                84, 84, 76, 80, 56, 0, 0, 0, 4, 0, 10, 0, 0, 0, 0, 0,
                32, 0, 0, 0, 16, 0, 0, 0
            };
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
    }

    public class SpriteSet
    {
        public Bitmap[] Sprites = new Bitmap[4];
        public ColorPalette Normal;
        public ColorPalette Shiny;
    }
}
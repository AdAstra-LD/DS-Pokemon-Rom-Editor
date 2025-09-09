using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE.Editors.Utils
{
    public class LoadingForm : Form
    {
        private ProgressBar progressBar;
        private Label factLabel;
        private readonly string[] pokemonFacts = new[]
        {
            "Did you know? Pikachu is the mascot of the Pokémon franchise!",
            "Bulbasaur is the first Pokémon in the National Pokédex.",
            "Mewtwo was created by scientists in a lab on Cinnabar Island.",
            "Eevee has eight different evolutions, known as 'Eeveelutions'!",
            "Charizard's tail flame burns brighter when it's healthy.",
            "Gengar is said to steal the shadows of its victims.",
            "The Pokémon world has eight regions, from Kanto to Paldea!",
            "Snorlax can sleep for days and only wakes up to eat.",
            "Magikarp is weak but evolves into the powerful Gyarados.",
            "Jigglypuff loves to sing, but its song puts others to sleep!"
        };
        private Random random = new Random();
        private Timer factTimer;

        public LoadingForm(int totalAmount, string textDisplay)
        {
            Text = textDisplay;
            Width = 400;
            Height = 200;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            progressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(20, 20),
                Width = 340,
                Height = 30,
                Minimum = 0,
                Maximum = totalAmount,
                Value = 0
            };
            Controls.Add(progressBar);

            factLabel = new Label
            {
                Location = new System.Drawing.Point(20, 60),
                Width = 340,
                Height = 80,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Text = "Loading Pokémon facts..."
            };
            Controls.Add(factLabel);

            try
            {
                string factFilePath = Path.Combine(Application.StartupPath, "Tools", "pokefatcs.txt");
                pokemonFacts = File.ReadAllLines(factFilePath)
                                  .Where(line => !string.IsNullOrWhiteSpace(line))
                                  .ToArray();
                if (pokemonFacts.Length == 0)
                {
                    pokemonFacts = new[] { "No Pokémon facts found, but we're still loading!" };
                }
            }
            catch (Exception ex)
            {
                pokemonFacts = new[] { $"Failed to load Pokémon facts: {ex.Message}" };
            }
            factLabel.Text = GetRandomFact();

            factTimer = new Timer
            {
                Interval = 5000 
            };
            factTimer.Tick += (s, e) => factLabel.Text = GetRandomFact();
            factTimer.Start();
        }

        public void UpdateProgress(int current)
        {
            if (current <= progressBar.Maximum)
            {
                progressBar.Value = current;
            }
        }

        private string GetRandomFact()
        {
            return pokemonFacts[random.Next(pokemonFacts.Length)];
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            factTimer.Stop();
            base.OnFormClosing(e);
        }
    }
}

using DSPRE.Resources;
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
using static DSPRE.RomInfo;

namespace DSPRE.Editors
{
    public partial class WeatherEditor : Form
    {
        public WeatherEditor()
        {
            InitializeComponent();
        }

        private String baseScript = "";
        private String baseFunction = "";

        private void WeatherEditor_Load(object sender, EventArgs e)
        {
            weatherSelector.Items.Clear();
            weatherSelector.Items.AddRange(PokeDatabase.Weather.PtWeatherDict.Values.ToArray());
            weatherSelector.SelectedIndex = 0;

            weatherUpOrDown.Maximum = PokeDatabase.Weather.PtWeatherDict.Values.ToArray().Count() - 1;
            updateImage();

            totalPercentage.Value = 100;

            totalPercentageFill.Maximum = (int)totalPercentage.Value;
            totalPercentageFill.Minimum = 0;

            randomVarNum.Value = 16416;
            weatherVarNum.Value = 16417;
            currWeatherVarNum.Value = 16418;

            writeBaseScript();
        }



        private void weatherSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            weatherUpOrDown.Value = PokeDatabase.Weather.PtWeatherDict.Keys.ElementAt(weatherSelector.SelectedIndex);

        }
        private void weatherUpOrDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                weatherSelector.SelectedItem = PokeDatabase.Weather.PtWeatherDict[(int)weatherUpOrDown.Value];
            }
            catch
            {
                weatherSelector.SelectedItem = 0;
            }

            updateImage();
        }

        private void updateImage()
        {
            try
            {
                Dictionary<byte[], string> dict;
                dict = PokeDatabase.System.WeatherPics.ptWeatherImageDict;

                bool found = false;
                foreach (KeyValuePair<byte[], string> dictEntry in dict)
                {
                    if (Array.IndexOf(dictEntry.Key, (byte)weatherUpOrDown.Value) >= 0)
                    {
                        weatherPicture.Image = (Image)Properties.Resources.ResourceManager.GetObject(dictEntry.Value);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    throw new KeyNotFoundException();
            }
            catch (KeyNotFoundException)
            {
                //  weatherPicture.Image = null;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void totalPercentage_ValueChanged(object sender, EventArgs e)
        {
            totalPercentageFill.Maximum = (int)totalPercentage.Value;
            CalculatePercentageLeft();
        }

        private void addWeatherButton_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add(weatherUpOrDown.Value, weatherSelector.SelectedItem, (int)weatherApparitionPercentage.Value);
            CalculatePercentageLeft();
        }

        private void CalculatePercentageLeft()
        {
            int currentPercentage = 0;
            try
            {

                for (int rows = 0; rows < dataGridView1.Rows.Count; rows++)
                {
                    int currentRow = (int)dataGridView1.Rows[rows].Cells[2].Value;
                    currentPercentage += currentRow;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("try again" + ex);
            }

            try
            {
                totalPercentageFill.Value = currentPercentage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("The total percentage exceeds the maximum percentage.",
                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RemoveLastRow();
                CalculatePercentageLeft();

            }

            if (currentPercentage >= totalPercentage.Value)
            {
                addWeatherButton.Enabled = false;
            }
            else
            {
                addWeatherButton.Enabled = true;
            }

            writeScript();
        }

        private void removeFirstRow_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.RemoveAt(0);
            CalculatePercentageLeft();
        }

        private void removeLastRow_Click(object sender, EventArgs e)
        {
            RemoveLastRow();
            CalculatePercentageLeft();
        }

        private void RemoveLastRow()
        {
            int lastIndex = dataGridView1.Rows.Count - 1;
            dataGridView1.Rows.RemoveAt(lastIndex);
        }

        private void currWeatherVarNum_ValueChanged(object sender, EventArgs e)
        {
            writeScript();
        }

        private void weatherVarNum_ValueChanged(object sender, EventArgs e)
        {
            writeScript();
        }

        private void randomVarNum_ValueChanged(object sender, EventArgs e)
        {
            writeScript();
        }

        private void writeBaseScript()
        {
            baseScript =
                "Script X:\n" +
                "   GetCurrentWeather " + weatherVarNum.Value + "\n" +
                "   GetRandom " + randomVarNum.Value + " " + totalPercentage.Value + "\n";

            scriptCodeView.Text = baseScript;

            baseFunction = "";
            functionCodeView.Text = baseFunction;
        }

        private void writeScript()
        {
            SortDataGridViewByWeatherId();
            writeBaseScript();
            int i = 0;


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {

                    var weatherId = row.Cells["WeatherId"].Value;
                    var percentage = row.Cells["Percentage"].Value;

                    // Creating Script side 
                    scriptCodeView.AppendText("   CompareVarValue " + randomVarNum.Value + " " + ((int)totalPercentage.Value - (int)percentage) + "\n");
                    scriptCodeView.AppendText("   JumpIf GREATER/EQUAL Function#" + (i + 1) + "\n \n");

                    // Create Function side
                    functionCodeView.AppendText("Function " + (i + 1) + ":\n");
                    functionCodeView.AppendText("   SetVar " + weatherVarNum.Value + " " + weatherId + "\n");
                    functionCodeView.AppendText("Jump Function#" + (dataGridView1.Rows.Count + 1) + "\n\n");

                    i++;
                }
            }

            functionCodeView.AppendText("Function " + (dataGridView1.Rows.Count + 1) + ":\n");
            functionCodeView.AppendText("   SetWeather " + weatherVarNum.Value + "\n");
            functionCodeView.AppendText("   GetCurrentWeather " + currWeatherVarNum.Value + "\n");
            functionCodeView.AppendText("End");

            scriptCodeView.AppendText("Jump Function#" + (dataGridView1.Rows.Count + 1) + "\n");
        }

        private void SortDataGridViewByWeatherId()
        {
            if (dataGridView1.Columns["Percentage"] != null)
            {
                dataGridView1.Sort(dataGridView1.Columns["Percentage"], System.ComponentModel.ListSortDirection.Ascending);
            }
            else
            {
                MessageBox.Show("Percentage column does not exist.");
            }
        }
    }
}
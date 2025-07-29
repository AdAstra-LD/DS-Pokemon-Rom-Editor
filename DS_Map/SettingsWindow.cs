using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private String getFolderPath()
        {
            CommonOpenFileDialog selectedFolder = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };
            if (selectedFolder.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return null;
            }

            return selectedFolder.FileName;

        }

        private String oldExportPath;
        private String oldMapImportPath;
        private String oldOpenDefaultPath;

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            currentVersionLabel.Text = $"DSPRE Version {Helpers.GetDSPREVersion()}";
            romExportPathTextBox.Text = SettingsManager.Settings.exportPath;
            oldExportPath = SettingsManager.Settings.exportPath;
            mapImportPathTextBox.Text = SettingsManager.Settings.mapImportStarterPoint;
            oldMapImportPath = SettingsManager.Settings.mapImportStarterPoint;
            openDefaultRomTextBox.Text = SettingsManager.Settings.openDefaultRom;
            oldOpenDefaultPath = SettingsManager.Settings.openDefaultRom;
            dontAskOpenCheckbox.Checked = SettingsManager.Settings.neverAskForOpening;

        }

        private void changePathButton1_Click(object sender, EventArgs e)
        {
            String tempPath = getFolderPath();
            if (tempPath != null)
            {
                romExportPathTextBox.Text = tempPath;
            }
        }

        private void changePathButton2_Click(object sender, EventArgs e)
        {
            mapImportPathTextBox.Text = getFolderPath();
        }

        private void changeOpenDefaultPathButton_Click(object sender, EventArgs e)
        {
            var defaultRomPath = getFolderPath();
            if(defaultRomPath != null && !defaultRomPath.EndsWith("DSPRE_contents"))
            {
                if (MessageBox.Show("The folder you selected does not appear to be a DSPRE folder (DSPRE_contents), are you sure you want to proceed?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }

            openDefaultRomTextBox.Text = defaultRomPath;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SettingsManager.Settings.exportPath = romExportPathTextBox.Text;
            SettingsManager.Settings.mapImportStarterPoint = mapImportPathTextBox.Text;
            SettingsManager.Settings.openDefaultRom = openDefaultRomTextBox.Text;
            oldExportPath = SettingsManager.Settings.exportPath;
            oldMapImportPath = SettingsManager.Settings.mapImportStarterPoint;
            oldOpenDefaultPath = SettingsManager.Settings.openDefaultRom;

            SettingsManager.Save();
        }


        private void SettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (oldMapImportPath != mapImportPathTextBox.Text || oldExportPath != romExportPathTextBox.Text || oldOpenDefaultPath != openDefaultRomTextBox.Text)
            {
                if (MessageBox.Show("You still have unsaved modifications, are you sure you want to quit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }

        }

        private void clearButtonExport_Click(object sender, EventArgs e)
        {
            romExportPathTextBox.Text = "";
        }

        private void clearButtonMap_Click(object sender, EventArgs e)
        {
            mapImportPathTextBox.Text = "";
        }

        private void mapImportBasePathLabel_Click(object sender, EventArgs e)
        {

        }
        private void clearButtonOpenDefault_Click(object sender, EventArgs e)
        {
            openDefaultRomTextBox.Text = "";
        }

        private void dontAskOpenCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            SettingsManager.Settings.neverAskForOpening = dontAskOpenCheckbox.Checked;
        }

        private void checkForUpdatesButton_Click(object sender, EventArgs e)
        {
            Helpers.CheckForUpdates(false);
        }
    }
}
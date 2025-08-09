using DSPRE.Editors.BtxEditor;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using MKDS_Course_Editor.Export3DTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;
using static DSPRE.RomInfo;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace DSPRE.Editors
{
    public partial class HeaderEditor : UserControl
    {
        bool isHeaderEditorReady = false;
        MainProgram _parent;

        public HeaderEditor()
        {
            InitializeComponent();
        }

        #region Header Editor

        #region Variables
        public MapHeader currentHeader;
        public List<string> internalNames;
        public List<string> headerListBoxNames;
        #endregion
        public void SetupHeaderEditor(MainProgram parent, bool force=false)
        {

            if (isHeaderEditorReady && !force) return;
            isHeaderEditorReady = true;
            this._parent = parent;
            /* Extract essential NARCs sub-archives*/

            Helpers.statusLabelMessage("Attempting to unpack Header Editor NARCs... Please wait.");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.synthOverlay, DirNames.textArchives, DirNames.dynamicHeaders });

            Helpers.statusLabelMessage("Reading internal names... Please wait.");
            Update();

            internalNames = new List<string>();
            headerListBoxNames = new List<string>();
            int headerCount;
            if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
            {
                addHeaderBTN.Enabled = true;
                removeLastHeaderBTN.Enabled = true;
                headerCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir).Length;
            }
            else
            {
                headerCount = RomInfo.GetHeaderCount();
            }

            /* Read Header internal names */
            try
            {
                headerListBoxNames = Helpers.getHeaderListBoxNames();
                internalNames = Helpers.getInternalNames();

                headerListBox.Items.Clear();
                headerListBox.Items.AddRange(headerListBoxNames.ToArray());
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(RomInfo.internalNamesPath + " doesn't exist.", "Couldn't read internal names", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            /*Add list of options to each control */
            EditorPanels.textEditor.currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);
            EditorPanels.textEditor.ReloadHeaderEditorLocationsList(EditorPanels.textEditor.currentTextArchive.messages, _parent);

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    areaIconComboBox.Enabled = false;
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject("dpareaicon");
                    areaSettingsLabel.Text = "Show nametag:";
                    cameraComboBox.Items.Clear();
                    musicDayComboBox.Items.Clear();
                    musicNightComboBox.Items.Clear();
                    areaSettingsComboBox.Items.Clear();
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.DPPtCameraDict.Values.ToArray());
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.DPMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.DPMusicDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.ShowName.DPShowNameValues);
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.DPWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 65535;

                    battleBackgroundLabel.Location = new Point(battleBackgroundLabel.Location.X - 25, battleBackgroundLabel.Location.Y - 8);
                    battleBackgroundUpDown.Location = new Point(battleBackgroundUpDown.Location.X - 25, battleBackgroundUpDown.Location.Y - 8);
                    break;
                case GameFamilies.Plat:
                    areaSettingsLabel.Text = "Show nametag:";
                    areaIconComboBox.Items.Clear();
                    cameraComboBox.Items.Clear();
                    musicDayComboBox.Items.Clear();
                    musicNightComboBox.Items.Clear();
                    areaSettingsComboBox.Items.Clear();
                    weatherComboBox.Items.Clear();
                    areaIconComboBox.Items.AddRange(PokeDatabase.Area.PtAreaIconValues);
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.DPPtCameraDict.Values.ToArray());
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.PtMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.PtMusicDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.ShowName.PtShowNameValues);
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.PtWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 65535;

                    battleBackgroundLabel.Location = new Point(battleBackgroundLabel.Location.X - 25, battleBackgroundLabel.Location.Y - 8);
                    battleBackgroundUpDown.Location = new Point(battleBackgroundUpDown.Location.X - 25, battleBackgroundUpDown.Location.Y - 8);
                    break;
                default:
                    areaSettingsLabel.Text = "Area Settings:";
                    areaIconComboBox.Items.Clear();
                    cameraComboBox.Items.Clear();
                    areaSettingsComboBox.Items.Clear();
                    musicDayComboBox.Items.Clear();
                    musicNightComboBox.Items.Clear();
                    weatherComboBox.Items.Clear();
                    areaIconComboBox.Items.AddRange(PokeDatabase.Area.HGSSAreaIconsDict.Values.ToArray());
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.HGSSCameraDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.Area.HGSSAreaProperties);
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.HGSSMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.HGSSMusicDict.Values.ToArray());
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.HGSSWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 255;

                    followModeComboBox.Visible = true;
                    followModeLabel.Visible = true;
                    johtoRadioButton.Visible = true;
                    kantoRadioButton.Visible = true;

                    flag6CheckBox.Visible = true;
                    flag5CheckBox.Visible = true;
                    flag4CheckBox.Visible = true;
                    flag6CheckBox.Text = "Flag ?";
                    flag5CheckBox.Text = "Flag ?";
                    flag4CheckBox.Text = "Flag ?";

                    worldmapCoordsGroupBox.Enabled = true;
                    break;
            }
            if (headerListBox.Items.Count > 0)
            {
                headerListBox.SelectedIndex = 0;
            }
            Helpers.statusLabelMessage();
        }

        private void openWildEditorWithIdButtonClick(object sender, EventArgs e)
        {
            _parent.openWildEditor(loadCurrent: true);
        }

        private void addHeaderBTN_Click(object sender, EventArgs e)
        {
            // Add new file in the dynamic headers directory
            string sourcePath = RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + "0000";
            string destPath = RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + RomInfo.GetHeaderCount().ToString("D4");
            File.Copy(sourcePath, destPath);

            // Add row to internal names table
            const string newmap = "NEWMAP";
            DSUtils.WriteToFile(RomInfo.internalNamesPath, StringToInternalName(newmap), (uint)RomInfo.GetHeaderCount() * RomInfo.internalNameLength);

            // Update headers ListBox and internal names list
            headerListBox.Items.Add(headerListBox.Items.Count + MapHeader.nameSeparator + " " + newmap);
            headerListBoxNames.Add(headerListBox.Items.Count + MapHeader.nameSeparator + " " + newmap);
            internalNames.Add(newmap);

            // Select new header
            headerListBox.SelectedIndex = headerListBox.Items.Count - 1;
        }
        private void removeLastHeaderBTN_Click(object sender, EventArgs e)
        {
            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = headerListBox.Items.Count - 1;

            if (lastIndex > 0)
            { //there are at least 2 elements
                if (headerListBox.SelectedIndex == lastIndex)
                {
                    headerListBox.SelectedIndex--;
                }

                /* Physically delete last header file */
                File.Delete(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + lastIndex.ToString("D4"));
                using (DSUtils.EasyWriter ew = new DSUtils.EasyWriter(RomInfo.internalNamesPath))
                {
                    ew.EditSize(-internalNameLength); //Delete internalNameLength amount of bytes from file end
                }

                /* Remove item from collections */
                headerListBox.Items.RemoveAt(lastIndex);
                internalNames.RemoveAt(lastIndex);
                headerListBoxNames.RemoveAt(lastIndex);
            }
            else
            {
                MessageBox.Show("You must have at least one header!", "Can't delete last header", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void areaDataUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.areaDataID = (byte)areaDataUpDown.Value;
        }
        private void internalNameBox_TextChanged(object sender, EventArgs e)
        {
            if (internalNameBox.Text.Length > 13)
            {
                internalNameLenLabel.ForeColor = Color.FromArgb(255, 0, 0);
            }
            else if (internalNameBox.Text.Length > 7)
            {
                internalNameLenLabel.ForeColor = Color.FromArgb(190, 190, 0);
            }
            else
            {
                internalNameLenLabel.ForeColor = Color.FromArgb(0, 180, 0);
            }

            internalNameLenLabel.Text = "[ " + (internalNameBox.Text.Length) + " ]";
        }
        private void areaIconComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            string imageName;
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    break;
                case GameFamilies.Plat:
                    ((HeaderPt)currentHeader).areaIcon = (byte)areaIconComboBox.SelectedIndex;
                    imageName = "areaicon0" + areaIconComboBox.SelectedIndex.ToString();
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
                default:
                    ((HeaderHGSS)currentHeader).areaIcon = (byte)areaIconComboBox.SelectedIndex;
                    imageName = PokeDatabase.System.AreaPics.hgssAreaPicDict[areaIconComboBox.SelectedIndex];
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
            }
        }
        private void eventFileUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.eventFileID = (ushort)eventFileUpDown.Value;
        }
        private void battleBackgroundUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentHeader.battleBackground = (byte)battleBackgroundUpDown.Value;
        }
        private void followModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                HeaderHGSS currentHeaderHGSS = (HeaderHGSS)currentHeader;
                currentHeaderHGSS.followMode = (byte)followModeComboBox.SelectedIndex;
            }
        }

        private void kantoRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                HeaderHGSS currentHeaderHGSS = (HeaderHGSS)currentHeader;
                currentHeaderHGSS.kantoFlag = kantoRadioButton.Checked;
            }
        }
        private void headerFlagsCheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            byte flagVal = 0;
            if (flag0CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 0);

            if (flag1CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 1);

            if (flag2CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 2);

            if (flag3CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 3);

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                if (flag4CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 4);
                if (flag5CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 5);
                if (flag6CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 6);
            }
            currentHeader.flags = flagVal;
        }
        private void headerListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || headerListBox.SelectedIndex < 0)
            {
                return;
            }

            /* Obtain current header ID from listbox*/
            if (!ushort.TryParse(headerListBox.SelectedItem.ToString().Substring(0, 3), out ushort headerNumber))
            {
                headerListBox.SelectedIndex = -1;
                return;
            }

            /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
            if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
            {
                currentHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerNumber.ToString("D4"), headerNumber, 0);
            }
            else
            {
                currentHeader = MapHeader.LoadFromARM9(headerNumber);
            }
            RefreshHeaderEditorFields();
        }

        private void RefreshHeaderEditorFields()
        {
            /* Setup controls for common fields across headers */
            if (currentHeader == null)
            {
                return;
            }

            internalNameBox.Text = internalNames[currentHeader.ID];
            matrixUpDown.Value = currentHeader.matrixID;
            areaDataUpDown.Value = currentHeader.areaDataID;
            scriptFileUpDown.Value = currentHeader.scriptFileID;
            levelScriptUpDown.Value = currentHeader.levelScriptID;
            eventFileUpDown.Value = currentHeader.eventFileID;
            textFileUpDown.Value = currentHeader.textArchiveID;
            wildPokeUpDown.Value = currentHeader.wildPokemon;
            weatherUpDown.Value = currentHeader.weatherID;
            cameraUpDown.Value = currentHeader.cameraAngleID;
            battleBackgroundUpDown.Value = currentHeader.battleBackground;

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                areaSettingsComboBox.SelectedIndex = ((HeaderHGSS)currentHeader).locationType;
            }

            openWildEditorWithIdButton.Enabled = currentHeader.wildPokemon != RomInfo.nullEncounterID;

            /* Setup controls for fields with version-specific differences */
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        {
                            HeaderDP h = (HeaderDP)currentHeader;

                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.locationSpecifier:D3}");
                            break;
                        }
                    case GameFamilies.Plat:
                        {
                            HeaderPt h = (HeaderPt)currentHeader;

                            areaIconComboBox.SelectedIndex = h.areaIcon;
                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.locationSpecifier:D3}");
                            break;
                        }
                    default:
                        {
                            HeaderHGSS h = (HeaderHGSS)currentHeader;

                            areaIconComboBox.SelectedIndex = h.areaIcon;
                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            worldmapXCoordUpDown.Value = h.worldmapX;
                            worldmapYCoordUpDown.Value = h.worldmapY;
                            followModeComboBox.SelectedIndex = h.followMode;
                            kantoRadioButton.Checked = h.kantoFlag;
                            johtoRadioButton.Checked = !h.kantoFlag;
                            break;
                        }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("This header contains an irregular/unsupported field.", "Error loading header file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefreshFlags();
            updateWeatherPicAndComboBox();
            updateCameraPicAndComboBox();
        }
        private void RefreshFlags()
        {
            BitArray ba = new BitArray(new byte[] { currentHeader.flags });

            flag0CheckBox.Checked = ba[0];
            flag1CheckBox.Checked = ba[1];
            flag2CheckBox.Checked = ba[2];
            flag3CheckBox.Checked = ba[3];

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                flag4CheckBox.Checked = ba[4];
                flag5CheckBox.Checked = ba[5];
                flag6CheckBox.Checked = ba[6];
            }
        }

        private void headerListBox_Leave(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            headerListBox.Refresh();
        }
        private void levelScriptUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.levelScriptID = (ushort)levelScriptUpDown.Value;
        }
        private void mapNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    ((HeaderDP)currentHeader).locationName = (ushort)locationNameComboBox.SelectedIndex;
                    break;
                case GameFamilies.Plat:
                    ((HeaderPt)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
                default:
                    ((HeaderHGSS)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
            }
        }
        private void matrixUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.matrixID = (ushort)matrixUpDown.Value;
        }

        private void openScriptButton_Click(object sender, EventArgs e)
        {
            EditorPanels.scriptEditor.OpenScriptEditor(_parent, (int)scriptFileUpDown.Value);
        }

        private void openLevelScriptButton_Click(object sender, EventArgs e)
        {
            EditorPanels.levelScriptEditor.OpenLevelScriptEditor(_parent, (int)levelScriptUpDown.Value);
        }

        private void musicDayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                case GameFamilies.Plat:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicNightComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                case GameFamilies.Plat:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicDayUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Helpers.DisableHandlers();
            ushort updValue = (ushort)((NumericUpDown)sender).Value;
            currentHeader.musicDayID = updValue;
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case GameFamilies.Plat:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.PtMusicDict[updValue];
                        break;
                    default:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.HGSSMusicDict[updValue];
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                musicDayComboBox.SelectedItem = null;
            }
            Helpers.EnableHandlers();
        }
        private void musicNightUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Helpers.DisableHandlers();
            ushort updValue = (ushort)((NumericUpDown)sender).Value;
            currentHeader.musicNightID = updValue;
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case GameFamilies.Plat:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.PtMusicDict[updValue];
                        break;
                    default:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.HGSSMusicDict[updValue];
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                musicNightComboBox.SelectedItem = null;
            }
            Helpers.EnableHandlers();
        }
        private void worldmapXCoordUpDown_ValueChanged(object sender, EventArgs e)
        {
            ((HeaderHGSS)currentHeader).worldmapX = (byte)worldmapXCoordUpDown.Value;
        }
        private void worldmapYCoordUpDown_ValueChanged(object sender, EventArgs e)
        {
            ((HeaderHGSS)currentHeader).worldmapY = (byte)worldmapYCoordUpDown.Value;
        }
        private void updateWeatherPicAndComboBox()
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            /* Update Weather Combobox*/
            Helpers.DisableHandlers();
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.DPWeatherDict[currentHeader.weatherID];
                        break;
                    case GameFamilies.Plat:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.PtWeatherDict[currentHeader.weatherID];
                        break;
                    default:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.HGSSWeatherDict[currentHeader.weatherID];
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                weatherComboBox.SelectedItem = null;
            }
            Helpers.EnableHandlers();

            /* Update Weather Picture */
            try
            {
                Dictionary<byte[], string> dict;
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        dict = PokeDatabase.System.WeatherPics.dpWeatherImageDict;
                        break;
                    case GameFamilies.Plat:
                        dict = PokeDatabase.System.WeatherPics.ptWeatherImageDict;
                        break;
                    default:
                        dict = PokeDatabase.System.WeatherPics.hgssweatherImageDict;
                        break;
                }

                bool found = false;
                foreach (KeyValuePair<byte[], string> dictEntry in dict)
                {
                    if (Array.IndexOf(dictEntry.Key, (byte)weatherUpDown.Value) >= 0)
                    {
                        weatherPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(dictEntry.Value);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    throw new KeyNotFoundException();
            }
            catch (KeyNotFoundException)
            {
                weatherPictureBox.Image = null;
            }
        }
        private void updateCameraPicAndComboBox()
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            /* Update Camera Combobox*/
            Helpers.DisableHandlers();
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.DPPtCameraDict[currentHeader.cameraAngleID];
                        break;
                    case GameFamilies.Plat:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.DPPtCameraDict[currentHeader.cameraAngleID];
                        break;
                    default:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.HGSSCameraDict[currentHeader.cameraAngleID];
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                cameraComboBox.SelectedItem = null;
            }
            Helpers.EnableHandlers();

            /* Update Camera Picture */
            string imageName;
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "dpcamera" + cameraUpDown.Value.ToString();
                        break;
                    case GameFamilies.Plat:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "ptcamera" + cameraUpDown.Value.ToString();
                        break;
                    default:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "hgsscamera" + cameraUpDown.Value.ToString();
                        break;
                }
                cameraPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("The current header uses an unrecognized camera.\n", "Unknown camera settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void weatherComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || weatherComboBox.SelectedIndex < 0)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    weatherUpDown.Value = PokeDatabase.Weather.DPWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                case GameFamilies.Plat:
                    weatherUpDown.Value = PokeDatabase.Weather.PtWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                default:
                    weatherUpDown.Value = PokeDatabase.Weather.HGSSWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
            }
            currentHeader.weatherID = (byte)weatherUpDown.Value;
        }
        private void weatherUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentHeader.weatherID = (byte)weatherUpDown.Value;
            updateWeatherPicAndComboBox();
        }
        private void cameraComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || cameraComboBox.SelectedIndex < 0)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.DPPtCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
                case GameFamilies.Plat:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.DPPtCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
                default:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.HGSSCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
            }
            currentHeader.cameraAngleID = (byte)cameraUpDown.Value;
        }
        private void cameraUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentHeader.cameraAngleID = (byte)cameraUpDown.Value;
            updateCameraPicAndComboBox();
        }
        private void openAreaDataButton_Click(object sender, EventArgs e)
        {
            EditorPanels.nsbtxEditor.SetupNSBTXEditor(_parent);

            EditorPanels.nsbtxEditor.selectAreaDataListBox.SelectedIndex = (int)areaDataUpDown.Value;
            EditorPanels.nsbtxEditor.texturePacksListBox.SelectedIndex = (EditorPanels.nsbtxEditor.mapTilesetRadioButton.Checked ? (int)EditorPanels.nsbtxEditor.areaDataMapTilesetUpDown.Value : (int)EditorPanels.nsbtxEditor.areaDataBuildingTilesetUpDown.Value);
            _parent.mainTabControl.SelectedTab = EditorPanels.nsbtxEditorTabPage;

            if (EditorPanels.nsbtxEditor.texturesListBox.Items.Count > 0)
                EditorPanels.nsbtxEditor.texturesListBox.SelectedIndex = 0;
            if (EditorPanels.nsbtxEditor.palettesListBox.Items.Count > 0)
                EditorPanels.nsbtxEditor.palettesListBox.SelectedIndex = 0;
        }
        private void openEventsButton_Click(object sender, EventArgs e)
        {
            EditorPanels.eventEditor.SetupEventEditor(_parent);


            if (matrixUpDown.Value != 0)
            {
                EditorPanels.eventEditor.eventAreaDataUpDown.Value = areaDataUpDown.Value; // Use Area Data for textures if matrix is not 0
            }

            EditorPanels.eventEditor.eventMatrixUpDown.Value = matrixUpDown.Value; // Open the right matrix in event editor
            EditorPanels.eventEditor.selectEventComboBox.SelectedIndex = (int)eventFileUpDown.Value; // Select event file
            _parent.mainTabControl.SelectedTab = EditorPanels.eventEditorTabPage;

            EditorPanels.eventEditor.eventMatrixUpDown_ValueChanged(null, null);
        }
        private void openMatrixButton_Click(object sender, EventArgs e)
        {

            EditorPanels.matrixEditor.SetupMatrixEditor(_parent);
            _parent.mainTabControl.SelectedTab = EditorPanels.matrixEditorTabPage;
            int matrixNumber = (int)matrixUpDown.Value;
            EditorPanels.matrixEditor.selectMatrixComboBox.SelectedIndex = matrixNumber;

            if (EditorPanels.matrixEditor.currentMatrix.hasHeadersSection)
            {
                EditorPanels.matrixEditor.matrixTabControl.SelectedTab = EditorPanels.matrixEditor.headersTabPage;

                //Autoselect cell containing current header, if such cell exists [and if current matrix has headers sections]
                for (int i = 0; i < EditorPanels.matrixEditor.headersGridView.RowCount; i++)
                {
                    for (int j = 0; j < EditorPanels.matrixEditor.headersGridView.ColumnCount; j++)
                    {
                        if (currentHeader.ID.ToString() == EditorPanels.matrixEditor.headersGridView.Rows[i].Cells[j].Value.ToString())
                        {
                            EditorPanels.matrixEditor.headersGridView.CurrentCell = EditorPanels.matrixEditor.headersGridView.Rows[i].Cells[j];
                            return;
                        }
                    }
                }
            }
        }
        private void openTextArchiveButton_Click(object sender, EventArgs e)
        {
            EditorPanels.textEditor.OpenTextEditor(_parent, (int)textFileUpDown.Value, locationNameComboBox);
        }
        private void saveHeaderButton_Click(object sender, EventArgs e)
        {
            /* Check if dynamic headers patch has been applied, and save header to arm9 or a/0/5/0 accordingly */
            if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
            {
                DSUtils.WriteToFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + currentHeader.ID.ToString("D4"), currentHeader.ToByteArray(), 0, 0, fmode: FileMode.Create);
            }
            else
            {
                uint headerOffset = (uint)(RomInfo.headerTableOffset + MapHeader.length * currentHeader.ID);
                ARM9.WriteBytes(currentHeader.ToByteArray(), headerOffset);
            }
            Helpers.DisableHandlers();

            updateCurrentInternalName();
            updateHeaderNameShown(headerListBox.SelectedIndex);
            headerListBox.Focus();
            Helpers.EnableHandlers();
        }
        private byte[] StringToInternalName(string text)
        {
            if (text.Length > internalNameLength)
            {
                MessageBox.Show("Internal names can't be longer than " + internalNameLength + " characters!", "Length error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Encoding.ASCII.GetBytes(text.Substring(0, Math.Min(text.Length, internalNameLength)).PadRight(internalNameLength, '\0'));
        }
        private void updateCurrentInternalName()
        {
            /* Update internal name according to internalNameBox text*/
            ushort headerID = currentHeader.ID;

            using (DSUtils.EasyWriter writer = new DSUtils.EasyWriter(RomInfo.internalNamesPath, headerID * RomInfo.internalNameLength))
            {
                writer.Write(StringToInternalName(internalNameBox.Text));
            }

            internalNames[headerID] = internalNameBox.Text;
            string elem = headerID.ToString("D3") + MapHeader.nameSeparator + internalNames[headerID];
            headerListBoxNames[headerID] = elem;

            if (EditorPanels.eventEditor.eventEditorIsReady)
            {
                EditorPanels.eventEditor.eventEditorWarpHeaderListBox.Items[headerID] = elem;
            }
        }
        private void updateHeaderNameShown(int thisIndex)
        {
            Helpers.DisableHandlers();
            string val = (string)(headerListBox.Items[thisIndex] = headerListBoxNames[currentHeader.ID]);
            if (EditorPanels.eventEditor.eventEditorIsReady)
            {
                EditorPanels.eventEditor.eventEditorWarpHeaderListBox.Items[thisIndex] = val;
            }
            Helpers.EnableHandlers();
        }
        private void resetButton_Click(object sender, EventArgs e)
        {
            resetHeaderSearch();
        }

        public void resetHeaderSearch()
        {
            searchLocationTextBox.Clear();
            HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
            Helpers.statusLabelMessage();
        }

        private void searchHeaderTextBox_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                startSearchGameLocation();
            }
        }
        private void searchHeaderButton_Click(object sender, EventArgs e)
        {
            startSearchGameLocation();
        }
        private void startSearchGameLocation()
        {
            if (searchLocationTextBox.Text.Length != 0)
            {
                headerListBox.Items.Clear();
                bool noResult = true;

                /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
                for (ushort i = 0; i < internalNames.Count; i++)
                {
                    MapHeader h;
                    if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
                    {
                        h = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + i.ToString("D4"), i, 0);
                    }
                    else
                    {
                        h = MapHeader.LoadFromARM9(i);
                    }

                    string locationName = "";
                    switch (RomInfo.gameFamily)
                    {
                        case GameFamilies.DP:
                            locationName = locationNameComboBox.Items[((HeaderDP)h).locationName].ToString();
                            break;
                        case GameFamilies.Plat:
                            locationName = locationNameComboBox.Items[((HeaderPt)h).locationName].ToString();
                            break;
                        case GameFamilies.HGSS:
                            locationName = locationNameComboBox.Items[((HeaderHGSS)h).locationName].ToString();
                            break;
                    }

                    if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        headerListBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + internalNames[i]);
                        noResult = false;
                    }
                }


                if (noResult)
                {
                    headerListBox.Items.Add("No result for " + '"' + searchLocationTextBox.Text + '"');
                    headerListBox.Enabled = false;
                }
                else
                {
                    headerListBox.SelectedIndex = 0;
                    headerListBox.Enabled = true;
                }
            }
            else if (headerListBox.Items.Count < internalNames.Count)
            {
                HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
            }
        }
        private void PrintMapHeadersSummary()
        {
            List<string> output = new List<string>();
            int sameInARow = 0;

            MapHeader[] hBuff = new MapHeader[2] {
                null,
                MapHeader.LoadFromARM9(0),
            };


            string[] locBuff = new string[2];
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    locBuff[1] = locationNameComboBox.Items[((HeaderDP)hBuff[1]).locationName].ToString();
                    break;
                case GameFamilies.Plat:
                    locBuff[1] = locationNameComboBox.Items[((HeaderPt)hBuff[1]).locationName].ToString();
                    break;
                case GameFamilies.HGSS:
                    locBuff[1] = locationNameComboBox.Items[((HeaderHGSS)hBuff[1]).locationName].ToString();
                    break;
            }

            for (ushort i = 0; i < internalNames.Count; i++)
            {
                hBuff[0] = hBuff[1];
                hBuff[1] = MapHeader.LoadFromARM9((ushort)(i + 1));

                string lastName = locBuff[0]; //Kind of a locBuff[-1]
                locBuff[0] = locBuff[1];
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        locBuff[1] = locationNameComboBox.Items[((HeaderDP)hBuff[1]).locationName].ToString();
                        break;
                    case GameFamilies.Plat:
                        locBuff[1] = locationNameComboBox.Items[((HeaderPt)hBuff[1]).locationName].ToString();
                        break;
                    case GameFamilies.HGSS:
                        locBuff[1] = locationNameComboBox.Items[((HeaderHGSS)hBuff[1]).locationName].ToString();
                        break;
                }


                string newStr = i.ToString("D3") + " - " + internalNames[i] + " - " + locBuff[0];

                if (output.Count > 0)
                {
                    if (lastName.Equals(locBuff[0]))
                    {
                        output.Add(newStr);
                        sameInARow++;
                    }
                    else
                    {
                        if (sameInARow > 0 || (sameInARow == 0 && locBuff[0].Equals(locBuff[1])))
                        {
                            output.Add("");
                        }
                        output.Add(newStr);
                        sameInARow = 0;
                    }
                }
                else
                {
                    output.Add(newStr);
                }
            }

            //File.WriteAllLines("dummy.txt", output);
        }
        private void scriptFileUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.scriptFileID = (ushort)scriptFileUpDown.Value;
        }
        private void areaSettingsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || areaSettingsComboBox.SelectedItem is null)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    currentHeader.locationSpecifier = Byte.Parse(areaSettingsComboBox.SelectedItem.ToString().Substring(1, 3));
                    break;
                case GameFamilies.HGSS:
                    HeaderHGSS ch = (HeaderHGSS)currentHeader;
                    ch.locationType = (byte)areaSettingsComboBox.SelectedIndex;
                    break;
            }
        }
        private void textFileUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.textArchiveID = (ushort)textFileUpDown.Value;
        }

        private void wildPokeUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentHeader.wildPokemon = (ushort)wildPokeUpDown.Value;
            if (wildPokeUpDown.Value == RomInfo.nullEncounterID)
            {
                wildPokeUpDown.ForeColor = Color.Red;
            }
            else
            {
                wildPokeUpDown.ForeColor = Color.Black;
            }

            if (currentHeader.wildPokemon == RomInfo.nullEncounterID)
                openWildEditorWithIdButton.Enabled = false;
            else
                openWildEditorWithIdButton.Enabled = true;
        }
        private void importHeaderFromFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = MapHeader.DefaultFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            MapHeader h = null;
            try
            {
                if (new FileInfo(of.FileName).Length > 48)
                    throw new FileFormatException();

                h = MapHeader.LoadFromFile(of.FileName, currentHeader.ID, 0);
                if (h == null)
                    throw new FileFormatException();

            }
            catch (FileFormatException)
            {
                MessageBox.Show("The file you tried to import is either malformed or not a Header file.\nNo changes have been made.",
                        "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentHeader = h;
            /* Check if dynamic headers patch has been applied, and save header to arm9 or a/0/5/0 accordingly */
            if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
            {
                DSUtils.WriteToFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + currentHeader.ID.ToString("D4"), currentHeader.ToByteArray(), 0, 0, fmode: FileMode.Create);
            }
            else
            {
                uint headerOffset = (uint)(RomInfo.headerTableOffset + MapHeader.length * currentHeader.ID);
                ARM9.WriteBytes(currentHeader.ToByteArray(), headerOffset);
            }

            try
            {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(of.FileName, MapHeader.length + 8))
                {
                    internalNameBox.Text = Encoding.UTF8.GetString(reader.ReadBytes(RomInfo.internalNameLength));
                }
                updateCurrentInternalName();
                updateHeaderNameShown(headerListBox.SelectedIndex);
            }
            catch (EndOfStreamException) { }

            RefreshHeaderEditorFields();
        }

        private void exportHeaderToFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = MapHeader.DefaultFilter,
                FileName = "Header " + currentHeader.ID + " - " + internalNames[currentHeader.ID] + " (" + locationNameComboBox.SelectedItem.ToString() + ")"
            };

            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            using (DSUtils.EasyWriter writer = new DSUtils.EasyWriter(sf.FileName))
            {
                writer.Write(currentHeader.ToByteArray()); //Write full header
                writer.Write((byte)0x00); //Padding
                writer.Write(Encoding.UTF8.GetBytes("INTNAME")); //Signature
                writer.Write(Encoding.UTF8.GetBytes(internalNames[currentHeader.ID])); //Save Internal name
            }
        }

        #region CopyPaste Buttons
        /*Copy Paste Functions*/
        #region Variables
        int locationNameCopy;
        string internalNameCopy;
        decimal encountersIDCopy;
        int shownameCopy;
        int areaIconCopy;

        int musicdayCopy;
        int musicnightCopy;
        int weatherCopy;
        int camAngleCopy;
        int areaSettingsCopy;

        decimal scriptsCopy;
        decimal levelScriptsCopy;
        decimal eventsCopy;
        decimal textsCopy;

        decimal matrixCopy;
        decimal areadataCopy;
        decimal worldmapXCoordCopy;
        decimal worldmapYCoordCopy;
        decimal battleBGCopy;

        byte flagsCopy;
        int followingPokeCopy;
        bool kantoFlagCopy;

        #endregion
        private void copyHeaderButton_Click(object sender, EventArgs e)
        {
            locationNameCopy = locationNameComboBox.SelectedIndex;
            internalNameCopy = internalNameBox.Text;
            shownameCopy = areaSettingsComboBox.SelectedIndex;
            areaIconCopy = areaIconComboBox.SelectedIndex;
            areaSettingsCopy = areaSettingsComboBox.SelectedIndex;
            encountersIDCopy = wildPokeUpDown.Value;

            musicdayCopy = musicDayComboBox.SelectedIndex;
            musicnightCopy = musicNightComboBox.SelectedIndex;
            weatherCopy = weatherComboBox.SelectedIndex;
            camAngleCopy = cameraComboBox.SelectedIndex;

            scriptsCopy = scriptFileUpDown.Value;
            levelScriptsCopy = levelScriptUpDown.Value;
            eventsCopy = eventFileUpDown.Value;
            textsCopy = textFileUpDown.Value;

            matrixCopy = matrixUpDown.Value;
            areadataCopy = areaDataUpDown.Value;
            worldmapXCoordCopy = worldmapXCoordUpDown.Value;
            worldmapYCoordCopy = worldmapYCoordUpDown.Value;

            battleBGCopy = battleBackgroundUpDown.Value;
            flagsCopy = currentHeader.flags;
            followingPokeCopy = followModeComboBox.SelectedIndex;
            kantoFlagCopy = kantoRadioButton.Checked;

            /*Enable paste buttons*/
            pasteHeaderButton.Enabled = true;

            pasteLocationNameButton.Enabled = true;
            pasteInternalNameButton.Enabled = true;
            pasteAreaSettingsButton.Enabled = true;
            pasteAreaIconButton.Enabled = true;
            pasteWildEncountersButton.Enabled = true;

            pasteMusicDayButton.Enabled = true;
            pasteMusicNightButton.Enabled = true;
            pasteWeatherButton.Enabled = true;
            pasteCameraAngleButton.Enabled = true;

            pasteScriptsButton.Enabled = true;
            pasteLevelScriptsButton.Enabled = true;
            pasteEventsButton.Enabled = true;
            pasteTextsButton.Enabled = true;

            pasteMatrixButton.Enabled = true;
            pasteAreaDataButton.Enabled = true;

            worldmapCoordsCopyButton.Enabled = true;

            pasteMapSettingsButton.Enabled = true;

            headerListBox.Focus();
        }
        private void copyInternalNameButton_Click(object sender, EventArgs e)
        {
            internalNameCopy = internalNameBox.Text;
            Clipboard.SetData(DataFormats.Text, internalNameCopy);
            pasteInternalNameButton.Enabled = true;
        }
        private void copyLocationNameButton_Click(object sender, EventArgs e)
        {
            locationNameCopy = locationNameComboBox.SelectedIndex;
            pasteLocationNameButton.Enabled = true;
        }
        private void copyAreaSettingsButton_Click(object sender, EventArgs e)
        {
            areaSettingsCopy = areaSettingsComboBox.SelectedIndex;
            pasteAreaSettingsButton.Enabled = true;
        }
        private void copyAreaIconButton_Click(object sender, EventArgs e)
        {
            areaIconCopy = areaIconComboBox.SelectedIndex;
            pasteAreaIconButton.Enabled = true;
        }
        private void copyWildEncountersButton_Click(object sender, EventArgs e)
        {
            encountersIDCopy = wildPokeUpDown.Value;
            Clipboard.SetData(DataFormats.Text, encountersIDCopy);
            pasteWildEncountersButton.Enabled = true;
        }
        private void copyMusicDayButton_Click(object sender, EventArgs e)
        {
            musicdayCopy = musicDayComboBox.SelectedIndex;
            pasteMusicDayButton.Enabled = true;
        }
        private void copyWeatherButton_Click(object sender, EventArgs e)
        {
            weatherCopy = weatherComboBox.SelectedIndex;
            pasteWeatherButton.Enabled = true;
        }
        private void copyMusicNightButton_Click(object sender, EventArgs e)
        {
            musicnightCopy = musicNightComboBox.SelectedIndex;
            pasteMusicNightButton.Enabled = true;
        }
        private void copyCameraAngleButton_Click(object sender, EventArgs e)
        {
            camAngleCopy = cameraComboBox.SelectedIndex;
            pasteCameraAngleButton.Enabled = true;
        }
        private void copyScriptsButton_Click(object sender, EventArgs e)
        {
            scriptsCopy = scriptFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, scriptsCopy);
            pasteScriptsButton.Enabled = true;
        }
        private void copyLevelScriptsButton_Click(object sender, EventArgs e)
        {
            levelScriptsCopy = levelScriptUpDown.Value;
            Clipboard.SetData(DataFormats.Text, levelScriptsCopy);
            pasteLevelScriptsButton.Enabled = true;
        }
        private void copyEventsButton_Click(object sender, EventArgs e)
        {
            eventsCopy = eventFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, eventsCopy);
            pasteEventsButton.Enabled = true;
        }
        private void copyTextsButton_Click(object sender, EventArgs e)
        {
            textsCopy = textFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, textsCopy);
            pasteTextsButton.Enabled = true;
        }
        private void copyMatrixButton_Click(object sender, EventArgs e)
        {
            matrixCopy = matrixUpDown.Value;
            Clipboard.SetData(DataFormats.Text, matrixCopy);
            pasteMatrixButton.Enabled = true;
        }
        private void copyAreaDataButton_Click(object sender, EventArgs e)
        {
            areadataCopy = areaDataUpDown.Value;
            Clipboard.SetData(DataFormats.Text, areadataCopy);
            pasteAreaDataButton.Enabled = true;
        }
        private void worldmapCoordsCopyButton_Click(object sender, EventArgs e)
        {
            worldmapXCoordCopy = worldmapXCoordUpDown.Value;
            worldmapYCoordCopy = worldmapYCoordUpDown.Value;
            worldmapCoordsPasteButton.Enabled = true;
        }
        private void copyMapSettingsButton_Click(object sender, EventArgs e)
        {
            flagsCopy = currentHeader.flags;
            battleBGCopy = currentHeader.battleBackground;
            followingPokeCopy = followModeComboBox.SelectedIndex;
            kantoFlagCopy = kantoRadioButton.Checked;
            pasteMapSettingsButton.Enabled = true;
        }

        /* Paste Buttons */
        private void pasteHeaderButton_Click(object sender, EventArgs e)
        {
            locationNameComboBox.SelectedIndex = locationNameCopy;
            internalNameBox.Text = internalNameCopy;
            wildPokeUpDown.Value = encountersIDCopy;

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    areaSettingsComboBox.SelectedIndex = shownameCopy;
                    break;
                case GameFamilies.HGSS:
                    areaSettingsComboBox.SelectedIndex = areaSettingsCopy;
                    break;
            }
            areaIconComboBox.SelectedIndex = areaIconCopy;

            musicDayComboBox.SelectedIndex = musicdayCopy;
            musicNightComboBox.SelectedIndex = musicnightCopy;
            weatherComboBox.SelectedIndex = weatherCopy;
            cameraComboBox.SelectedIndex = camAngleCopy;

            scriptFileUpDown.Value = scriptsCopy;
            levelScriptUpDown.Value = levelScriptsCopy;
            eventFileUpDown.Value = eventsCopy;
            textFileUpDown.Value = textsCopy;

            matrixUpDown.Value = matrixCopy;
            areaDataUpDown.Value = areadataCopy;

            currentHeader.flags = flagsCopy;
            worldmapXCoordUpDown.Value = worldmapXCoordCopy;
            worldmapYCoordUpDown.Value = worldmapYCoordCopy;
            battleBackgroundUpDown.Value = battleBGCopy;
            RefreshFlags();
        }
        private void pasteInternalNameButton_Click(object sender, EventArgs e)
        {
            internalNameBox.Text = internalNameCopy;
        }
        private void pasteLocationNameButton_Click(object sender, EventArgs e)
        {
            locationNameComboBox.SelectedIndex = locationNameCopy;
        }
        private void pasteAreaSettingsButton_Click(object sender, EventArgs e)
        {
            areaSettingsComboBox.SelectedIndex = shownameCopy;
        }
        private void pasteAreaIconButton_Click(object sender, EventArgs e)
        {
            if (areaIconComboBox.Enabled)
            {
                areaIconComboBox.SelectedIndex = areaIconCopy;
            }
        }
        private void pasteWildEncountersButton_Click(object sender, EventArgs e)
        {
            wildPokeUpDown.Value = encountersIDCopy;
        }
        private void pasteMusicDayButton_Click(object sender, EventArgs e)
        {
            musicDayComboBox.SelectedIndex = musicdayCopy;
        }
        private void pasteScriptsButton_Click(object sender, EventArgs e)
        {
            scriptFileUpDown.Value = scriptsCopy;
        }
        private void pasteLevelScriptsButton_Click(object sender, EventArgs e)
        {
            levelScriptUpDown.Value = levelScriptsCopy;
        }
        private void pasteEventsButton_Click(object sender, EventArgs e)
        {
            eventFileUpDown.Value = eventsCopy;
        }
        private void pasteTextsButton_Click(object sender, EventArgs e)
        {
            textFileUpDown.Value = textsCopy;
        }
        private void pasteMatrixButton_Click(object sender, EventArgs e)
        {
            matrixUpDown.Value = matrixCopy;
        }
        private void pasteAreaDataButton_Click(object sender, EventArgs e)
        {
            areaDataUpDown.Value = areadataCopy;
        }
        private void pasteWeatherButton_Click(object sender, EventArgs e)
        {
            weatherComboBox.SelectedIndex = weatherCopy;
        }
        private void pasteMusicNightButton_Click(object sender, EventArgs e)
        {
            musicNightComboBox.SelectedIndex = musicnightCopy;
        }
        private void pasteCameraAngleButton_Click(object sender, EventArgs e)
        {
            cameraComboBox.SelectedIndex = camAngleCopy;
        }
        private void worldmapCoordsPasteButton_Click(object sender, EventArgs e)
        {
            worldmapXCoordUpDown.Value = worldmapXCoordCopy;
            worldmapYCoordUpDown.Value = worldmapYCoordCopy;
        }
        private void pasteMapSettingsButton_Click(object sender, EventArgs e)
        {
            currentHeader.flags = flagsCopy;
            battleBackgroundUpDown.Value = battleBGCopy;

            followModeComboBox.SelectedIndex = followingPokeCopy;
            kantoRadioButton.Checked = kantoFlagCopy;
            RefreshFlags();
        }
        #endregion

        #endregion
    }
}

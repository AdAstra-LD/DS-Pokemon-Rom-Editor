using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors {
  public partial class SafariZoneEditor : UserControl {
    public bool safariZoneEditorIsReady { get; set; } = false;
    private SafariZoneEncounterFile safariZoneEncounterFile;

    public SafariZoneEditor() {
      InitializeComponent();
    }

    public void SetupSafariZoneEditor(bool force = false) {
      if (safariZoneEditorIsReady && !force){ return; }
      safariZoneEditorIsReady = true;

      DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames>() {
        RomInfo.DirNames.safariZone,
        RomInfo.DirNames.textArchives,
      });

      safariZoneEncounterGroupEditorGrass.SetPokemonNames();
      safariZoneEncounterGroupEditorSurf.SetPokemonNames();
      safariZoneEncounterGroupEditorOldRod.SetPokemonNames();
      safariZoneEncounterGroupEditorGoodRod.SetPokemonNames();
      safariZoneEncounterGroupEditorSuperRod.SetPokemonNames();

      int safariZoneCount = Filesystem.GetSafariZoneCount();
      comboBoxFileID.Items.Clear();
      for (int i = 0; i < safariZoneCount; i++) {
        comboBoxFileID.Items.Add(SafariZoneEncounterFile.Names[i]);
      }

      if (comboBoxFileID.Items.Count > 0) {
        comboBoxFileID.SelectedIndex = 0;
      }
    }

    private void comboBoxFileID_SelectedIndexChanged(object sender, EventArgs e) {
      if (comboBoxFileID.SelectedIndex == -1) {
        safariZoneEncounterFile = null;
        safariZoneEncounterGroupEditorGrass.Reset();
        safariZoneEncounterGroupEditorSurf.Reset();
        safariZoneEncounterGroupEditorOldRod.Reset();
        safariZoneEncounterGroupEditorGoodRod.Reset();
        safariZoneEncounterGroupEditorSuperRod.Reset();
        return;
      }

      safariZoneEncounterFile = new SafariZoneEncounterFile(comboBoxFileID.SelectedIndex);
      safariZoneEncounterGroupEditorGrass.SetData(safariZoneEncounterFile.grassEncounterGroup);
      safariZoneEncounterGroupEditorSurf.SetData(safariZoneEncounterFile.surfEncounterGroup);
      safariZoneEncounterGroupEditorOldRod.SetData(safariZoneEncounterFile.oldRodEncounterGroup);
      safariZoneEncounterGroupEditorGoodRod.SetData(safariZoneEncounterFile.goodRodEncounterGroup);
      safariZoneEncounterGroupEditorSuperRod.SetData(safariZoneEncounterFile.superRodEncounterGroup);
    }

    private void buttonSave_Click(object sender, EventArgs e) {
      if (safariZoneEncounterFile == null){ return; }
      safariZoneEncounterFile.SaveToFile();
    }

    private void buttonSaveAs_Click(object sender, EventArgs e) {
      if (safariZoneEncounterFile == null){ return; }

      SaveFileDialog sfd = new SaveFileDialog();
      try {
        sfd.InitialDirectory = Path.GetDirectoryName(sfd.FileName);
        sfd.FileName = Path.GetFileName(sfd.FileName);
      }
      catch (Exception ex) {
        sfd.InitialDirectory = Path.GetDirectoryName(Environment.SpecialFolder.UserProfile.ToString());
        sfd.FileName = Path.GetFileName(sfd.FileName);
      }
      if (sfd.ShowDialog() != DialogResult.OK){ return; }

      safariZoneEncounterFile.SaveToFile(sfd.FileName);
    }

    private void buttonImport_Click(object sender, EventArgs e) {
      if (safariZoneEncounterFile == null){ return; }

      OpenFileDialog ofd = new OpenFileDialog();
      try {
        ofd.InitialDirectory = Path.GetDirectoryName(ofd.FileName);
        ofd.FileName = Path.GetFileName(ofd.FileName);
      }
      catch (Exception ex) {
        ofd.InitialDirectory = Path.GetDirectoryName(Environment.SpecialFolder.UserProfile.ToString());
        ofd.FileName = Path.GetFileName(ofd.FileName);
      }
      if (ofd.ShowDialog() != DialogResult.OK){ return; }

      safariZoneEncounterFile = new SafariZoneEncounterFile(ofd.FileName);
      safariZoneEncounterGroupEditorGrass.SetData(safariZoneEncounterFile.grassEncounterGroup);
      safariZoneEncounterGroupEditorSurf.SetData(safariZoneEncounterFile.surfEncounterGroup);
      safariZoneEncounterGroupEditorOldRod.SetData(safariZoneEncounterFile.oldRodEncounterGroup);
      safariZoneEncounterGroupEditorGoodRod.SetData(safariZoneEncounterFile.goodRodEncounterGroup);
      safariZoneEncounterGroupEditorSuperRod.SetData(safariZoneEncounterFile.superRodEncounterGroup);
    }
  }
}

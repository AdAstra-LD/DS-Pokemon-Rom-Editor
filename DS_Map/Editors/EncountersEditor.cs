using MKDS_Course_Editor.Export3DTools;
using System.Windows.Forms;

namespace DSPRE.Editors
{
  public partial class EncountersEditor : UserControl
  {
        public bool encounterEditorIsReady { get; set; } = false;
    public EncountersEditor()
    {
      InitializeComponent();
    }

    public void SetupEncountersEditor() {
            encounterEditorIsReady = true;
            tabPageHeadbuttEditor_Enter(null, null);
    }

    private void tabPageHeadbuttEditor_Enter(object sender, System.EventArgs e)
    {
      headbuttEncounterEditor.SetupHeadbuttEncounterEditor();
      headbuttEncounterEditor.makeCurrent();
    }

    private void tabPageSafariZoneEditor_Enter(object sender, System.EventArgs e)
    {
      safariZoneEditor.SetupSafariZoneEditor();
    }
  }
}

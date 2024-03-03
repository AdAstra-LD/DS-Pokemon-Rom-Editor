using System.Windows.Forms;

namespace DSPRE.Editors
{
  public partial class EncountersEditor : UserControl
  {
    public EncountersEditor()
    {
      InitializeComponent();
    }

    public void SetupEncountersEditor() {
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

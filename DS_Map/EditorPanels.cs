using System.Windows.Forms;
using DSPRE.Editors;

namespace DSPRE {
  public static class EditorPanels {
    public static MainProgram MainProgram;

    public static void Initialize(MainProgram mainProgram) {
      MainProgram = mainProgram;
    }
        public static TabControl mainTabControl { get { return MainProgram.mainTabControl; } }

        public static ScriptEditor scriptEditor { get { return MainProgram.scriptEditor; } }
        public static LevelScriptEditor levelScriptEditor { get { return MainProgram.levelScriptEditor; } }
        
        public static EncountersEditor encountersEditor { get { return MainProgram.encountersEditor; } }

        public static TabPage scriptEditorTabPage { get { return MainProgram.tabPageScriptEditor; } }
        public static TabPage levelScriptEditorTabPage { get { return MainProgram.tabPageLevelScriptEditor; } }
        public static TabPage tabPageEncountersEditor { get { return MainProgram.tabPageEncountersEditor; } }


    }
}

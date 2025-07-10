using System.Windows.Forms;
using DSPRE.Editors;

namespace DSPRE {
  public static class EditorPanels {
    public static MainProgram MainProgram;

    public static void Initialize(MainProgram mainProgram) {
      MainProgram = mainProgram;
    }

        #region Editors
        public static TabControl mainTabControl { get { return MainProgram.mainTabControl; } }

        public static ScriptEditor scriptEditor { get { return MainProgram.scriptEditor; } }
        public static LevelScriptEditor levelScriptEditor { get { return MainProgram.levelScriptEditor; } }

        public static TextEditor textEditor { get { return MainProgram.textEditor; } }
        
        public static EncountersEditor encountersEditor { get { return MainProgram.encountersEditor; } }

        public static TableEditor tableEditor { get { return MainProgram.tableEditor; } }

        public static TrainerEditor trainerEditor { get { return MainProgram.trainerEditor; } }

        public static CameraEditor cameraEditor { get { return MainProgram.cameraEditor;  } }

        public static NsbtxEditor nsbtxEditor { get { return MainProgram.nsbtxEditor; } }

        #endregion

        #region Tabs
        public static TabPage scriptEditorTabPage { get { return MainProgram.tabPageScriptEditor; } }

        public static TabPage cameraEditorTabPage { get { return MainProgram.cameraEditorTabPage; } }
        public static TabPage levelScriptEditorTabPage { get { return MainProgram.tabPageLevelScriptEditor; } }

        public static TabPage textEditorTabPage { get { return MainProgram.textEditorTabPage; } }
        public static TabPage tabPageEncountersEditor { get { return MainProgram.tabPageEncountersEditor; } }
        public static TabPage tabPageTableEditor { get { return MainProgram.tableEditorTabPage; } }

        public static TabPage trainerEditorTabPage { get { return MainProgram.trainerEditorTabPage; } }

        public static TabPage nsbtxEditorTabPage { get { return MainProgram.nsbtxEditorTabPage; } }

        #endregion

    }
}

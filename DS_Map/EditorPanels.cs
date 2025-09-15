using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using DSPRE.Editors;

namespace DSPRE {
  public static class EditorPanels {
    public static MainProgram MainProgram;

    public static void Initialize(MainProgram mainProgram) {
      MainProgram = mainProgram;
    }
    

        public sealed class EditorPopoutConfig
        {
            public Control Control { get; }
            public Label PlaceholderLabel { get; }
            public Button PopoutButton { get; }

            public EditorPopoutConfig(Control control, Label placeholderLabel, Button popoutButton)
            {
                Control = control;
                PlaceholderLabel = placeholderLabel;
                PopoutButton = popoutButton;
            }

        }
        
        public static class PopoutRegistry
        {
            private static readonly Dictionary<Control, Form> _hosts = new Dictionary<Control, Form>();

            public static bool IsPoppedOut(Control control) => _hosts.ContainsKey(control);

            public static bool TryGetHost(Control control, out Form host) => _hosts.TryGetValue(control, out host);

            public static IReadOnlyDictionary<Control, Form> Snapshot
                => new ReadOnlyDictionary<Control, Form>(new Dictionary<Control, Form>(_hosts));

            internal static void Add(Control control, Form host)
            {
                _hosts[control] = host;
                host.FormClosed += (_, __) => _hosts.Remove(control);
            }

            public static void CloseAll()
            {
                foreach (var host in _hosts.Values.ToList())
                    host.Close();
            }
        }
        



        #region Editors
        public static HeaderEditor headerEditor { get { return MainProgram.headerEditor; } }
        public static MapEditor mapEditor { get { return MainProgram.mapEditor; } }

        public static MatrixEditor matrixEditor { get { return MainProgram.matrixEditor; } }

        public static EventEditor eventEditor { get { return MainProgram.eventEditor; } }

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
        public static TabControl mainTabControl { get { return MainProgram.mainTabControl; } }
        public static TabPage scriptEditorTabPage { get { return MainProgram.tabPageScriptEditor; } }

        public static TabPage headerEditorTabPage { get { return MainProgram.headerEditorTabPage; } }
        
        public static TabPage matrixEditorTabPage { get { return MainProgram.matrixEditorTabPage; } }
        public static TabPage eventEditorTabPage { get { return MainProgram.eventEditorTabPage; } }
        public static TabPage cameraEditorTabPage { get { return MainProgram.cameraEditorTabPage; } }
        public static TabPage levelScriptEditorTabPage { get { return MainProgram.tabPageLevelScriptEditor; } }

        public static TabPage textEditorTabPage { get { return MainProgram.textEditorTabPage; } }
        public static TabPage tabPageEncountersEditor { get { return MainProgram.tabPageEncountersEditor; } }
        public static TabPage tabPageTableEditor { get { return MainProgram.tableEditorTabPage; } }

        public static TabPage trainerEditorTabPage { get { return MainProgram.trainerEditorTabPage; } }

        public static TabPage nsbtxEditorTabPage { get { return MainProgram.nsbtxEditorTabPage; } }

        public static TabPage mapEditorTabPage { get { return MainProgram.mapEditorTabPage; } }

        #endregion

    }
}

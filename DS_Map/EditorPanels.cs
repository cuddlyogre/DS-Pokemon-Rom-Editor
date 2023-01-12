using System.Windows.Forms;
using DSPRE.Editors;

namespace DSPRE {
  public static class EditorPanels {
    public static MainProgram MainProgram;

    public static void Initialize(MainProgram mainProgram) {
      MainProgram = mainProgram;
    }

    public static MatrixEditor matrixEditor { get { return MainProgram.matrixEditor; } }
    public static MapEditor mapEditor { get { return MainProgram.mapEditor; } }
    public static NSBTXEditor nsbtxEditor { get { return MainProgram.nsbtxEditor; } }
    public static HeaderEditor headerEditor { get { return MainProgram.headerEditor; } }
    public static EventEditor eventEditor { get { return MainProgram.eventEditor; } }
    public static ScriptEditor scriptEditor { get { return MainProgram.scriptEditor; } }
    public static LevelScriptEditor levelScriptEditor { get { return MainProgram.levelScriptEditor; } }
    public static HeadbuttEncounterEditor headbuttEncounterEditor { get { return MainProgram.headbuttEncounterEditor; } }
    public static SafariZoneEditor safariZoneEditor { get { return MainProgram.safariZoneEditor; } }
    public static TextEditor textEditor { get { return MainProgram.textEditor; } }
    public static CameraEditor cameraEditor { get { return MainProgram.cameraEditor; } }
    public static TrainerEditor trainerEditor { get { return MainProgram.trainerEditor; } }
    public static TableEditor tableEditor { get { return MainProgram.tableEditor; } }
    public static TabControl mainTabControl { get { return MainProgram.mainTabControl; } }
    public static TabPage headerEditorTabPage { get { return MainProgram.tabPageHeaderEditor; } }
    public static TabPage matrixEditorTabPage { get { return MainProgram.tabPageMatrixEditor; } }
    public static TabPage mapEditorTabPage { get { return MainProgram.tabPageMapEditor; } }
    public static TabPage nsbtxEditorTabPage { get { return MainProgram.tabPageNSBTXEditor; } }
    public static TabPage eventEditorTabPage { get { return MainProgram.tabPageEventEditor; } }
    public static TabPage scriptEditorTabPage { get { return MainProgram.tabPageScriptEditor; } }
    public static TabPage levelScriptEditorTabPage { get { return MainProgram.tabPageLevelScriptEditor; } }
    public static TabPage headbuttEncounterEditorTabPage { get { return MainProgram.tabPageHeabuttEncounterEditor; } }
    public static TabPage safariZoneEditorTabPage { get { return MainProgram.tabPageSafariZoneEditor; } }
    public static TabPage textEditorTabPage { get { return MainProgram.tabPageTextEditor; } }
    public static TabPage cameraEditorTabPage { get { return MainProgram.tabPageCameraEditor; } }
    public static TabPage trainerEditorTabPage { get { return MainProgram.tabPageTrainerEditor; } }
    public static TabPage tableEditorTabPage { get { return MainProgram.tabPageTableEditor; } }
  }
}

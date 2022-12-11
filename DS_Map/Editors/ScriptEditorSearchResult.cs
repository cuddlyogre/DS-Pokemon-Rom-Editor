using DSPRE.ROMFiles;

namespace DSPRE.Editors {
  public class ScriptEditorSearchResult {
    public readonly ScriptFile scriptFile;
    public readonly ScriptFile.containerTypes containerType;
    public readonly int commandNumber;
    public readonly ScriptCommand scriptCommand;

    public ScriptEditorSearchResult(ScriptFile scriptFile, ScriptFile.containerTypes containerType, int commandNumber, ScriptCommand scriptCommand) {
      this.scriptFile = scriptFile;
      this.containerType = containerType;
      this.commandNumber = commandNumber;
      this.scriptCommand = scriptCommand;
    }

    public string CommandBlockOpen { get { return $"{containerType} {commandNumber}:"; } }

    public override string ToString() {
      return $"File {scriptFile.fileID} - {CommandBlockOpen} {scriptCommand.name}";
    }
  }
}

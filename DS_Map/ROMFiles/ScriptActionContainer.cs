using System.Collections.Generic;

namespace DSPRE.ROMFiles {
  public class ScriptActionContainer {
    public List<ScriptAction> commands;
    public uint manualUserID;

    #region Constructors (2)
    public ScriptActionContainer(uint actionNumber, List<ScriptAction> commands = null) {
      manualUserID = actionNumber;
      this.commands = commands;
    }
    #endregion
  }
}

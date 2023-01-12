using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors
{
  public partial class SafariZoneEditor : UserControl
  {
    public bool safariZoneEditorIsReady { get; set; } = false;
    private SafariZoneEncounterFile safariZoneEncounterFile;

    public SafariZoneEditor()
    {
      InitializeComponent();
    }

    public void SetupSafariZoneEditor(bool force = false) {
      if (safariZoneEditorIsReady && !force) return;
      safariZoneEditorIsReady = true;

      DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames>() {
        RomInfo.DirNames.safariZone,
      });

      int safariZoneCount = Filesystem.GetSafariZoneCount();

      comboBoxFileID.Items.Clear();
      for (int i = 0; i < safariZoneCount; i++) {
        comboBoxFileID.Items.Add(i.ToString("D3"));
      }
    }

    private void comboBoxFileID_SelectedIndexChanged(object sender, EventArgs e) {
      if (comboBoxFileID.SelectedIndex == -1) return;
      safariZoneEncounterFile = new SafariZoneEncounterFile(comboBoxFileID.SelectedIndex);
    }
  }
}

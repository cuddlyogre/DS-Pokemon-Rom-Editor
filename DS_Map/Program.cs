using System;
using System.Windows.Forms;

namespace DSPRE {
  static class Program {
    public static MainProgram MainProgram;

    /// <summary>
    /// Punto di ingresso principale dell'applicazione.
    /// </summary>
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      MainProgram = new MainProgram();
      Application.Run(MainProgram);
    }
  }
}

using System;
using System.Drawing;
using System.Windows.Forms;

using Liven.Forms;


namespace Liven {
  public class EnvironmentVariableEditor {

    [STAThread]
    public static void Main(string[] args) {
      Application.EnableVisualStyles();
      Application.Run(new MainForm());
    }

  }
}

using System;
using System.Windows.Forms;
using Rca7.UpdateTray.WinForms.Views;

namespace Rca7.UpdateTray.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}

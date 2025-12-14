using System;
using System.Windows.Forms;
using Rca7.UpdateTray.WinForms.Views;

namespace Rca7.UpdateTray.WinForms;

/// <summary>
/// 托盘程序入口类
/// </summary>
internal static class Program
{
    /// <summary>
    /// 应用程序主入口点
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // 初始化应用程序配置
        ApplicationConfiguration.Initialize();
        
        // 运行主窗体
        Application.Run(new MainForm());
    }
}

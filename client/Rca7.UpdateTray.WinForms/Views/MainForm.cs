using System.Drawing;
using System.Windows.Forms;
using Rca7.UpdateClient.Shared.Config;

namespace Rca7.UpdateTray.WinForms.Views;

public class MainForm : Form
{
    private readonly TabControl _tabs;
    private readonly TabPage _wizardPage;
    private readonly TabPage _configurationPage;
    private readonly Label _wizardPlaceholder;
    private readonly Label _defaultDirectoryLabel;
    private readonly Label _backupDirectoryLabel;
    private readonly ListBox _sqlOrderList;

    public MainForm()
    {
        Text = "Update Tray";
        MinimumSize = new Size(640, 400);

        _tabs = new TabControl { Dock = DockStyle.Fill };
        _wizardPage = new TabPage("向导") { Padding = new Padding(10) };
        _configurationPage = new TabPage("配置") { Padding = new Padding(10) };

        _wizardPlaceholder = new Label
        {
            Dock = DockStyle.Fill,
            Text = "此处预留向导页，用于分步引导用户检查并启动更新。",
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.DimGray
        };

        _defaultDirectoryLabel = new Label { Dock = DockStyle.Top, AutoSize = true };
        _backupDirectoryLabel = new Label { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 8, 0, 0) };
        _sqlOrderList = new ListBox { Dock = DockStyle.Fill };

        _wizardPage.Controls.Add(_wizardPlaceholder);

        var configLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(4),
            AutoSize = true
        };

        configLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        configLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        configLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        configLayout.Controls.Add(_defaultDirectoryLabel, 0, 0);
        configLayout.Controls.Add(_backupDirectoryLabel, 0, 1);
        configLayout.Controls.Add(new GroupBox
        {
            Text = "SQL 顺序",
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            Controls = { _sqlOrderList }
        }, 0, 2);

        _configurationPage.Controls.Add(configLayout);

        _tabs.TabPages.AddRange(new[] { _wizardPage, _configurationPage });
        Controls.Add(_tabs);

        LoadDefaultConfiguration();
    }

    private void LoadDefaultConfiguration()
    {
        var defaults = UpdateConfiguration.CreateDefaults(AppContext.BaseDirectory);
        _defaultDirectoryLabel.Text = $"默认目录: {defaults.DefaultUpdateDirectory}";
        _backupDirectoryLabel.Text = $"备份路径: {defaults.BackupDirectory}";

        _sqlOrderList.Items.Clear();
        foreach (var script in defaults.SqlExecutionOrder)
        {
            _sqlOrderList.Items.Add(script);
        }
    }
}

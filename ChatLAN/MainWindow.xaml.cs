using System;
using System.Windows.Controls;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ChatLAN;

public partial class MainWindow
{
    public new static event EventHandler Close;
    private static MetroWindow _metroWindow;
    private static Frame _frame;

    public MainWindow()
    {
        InitializeComponent();
        _metroWindow = this;
        _frame = Frame;
        Closed += (sender, args) => Close?.Invoke(sender, null);
        OpenPage(new PageMain());
    }

    public static void ShowMessage(string title, string message)
    {
        _metroWindow.Dispatcher.Invoke(() =>
        {
            _metroWindow.ShowMessageAsync(title, message);
            _metroWindow.Show();
        }, DispatcherPriority.Normal);
    }

    public static void OpenPage(object obj)
    {
        _frame.Invoke(() => _frame.Navigate(obj));
    }public static void OpenPage(string path)
    {
        _frame.Invoke(() => _frame.Navigate(new Uri(path, UriKind.Relative)));
    }
}
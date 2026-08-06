using System.Windows.Controls;

namespace ChatLAN.Server.Pages;

public partial class Server
{
    private static StackPanel _stackPanel;

    public Server()
    {
        InitializeComponent();
        _stackPanel = StackPanel;
    }

    public static void PrintText(string text) =>
        _stackPanel.Dispatcher.Invoke(() =>
        _stackPanel.Children.Add(new TextBox{Text = text}));
}
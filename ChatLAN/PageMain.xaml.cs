using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ChatLAN.Client;
using ChatLAN.Client.Pages;
using ChatLAN.Server;

namespace ChatLAN;

public partial class PageMain
{
    public PageMain()
    {
        InitializeComponent();
    }

    private void BtnJoinOnClick(object sender, RoutedEventArgs e)
    {
        PanelOfClient.Visibility = Visibility.Collapsed;
        ProgressRing.Visibility = Visibility.Visible;

        if (!ValidationAdress(TbAdress.Text))
        {
            PrintAndReturnButton("错误", "无效的IP地址");
            return;
        }

        var ipAdress = GetIpAdress(TbAdress.Text);
        var port = int.Parse(NumPort.Value.ToString());
        var login = TbLogin.Text;
        ClientCore client;

        new Thread(() =>
        {
            client = ClientCore.InicializeClient(ipAdress, port);
            if (client == null)
            {
                ClientCore.RemoveClient();
                MessageOnError(null, "好像出了什么问题，请再试一次。");
                return;
            }

            client.Error -= MessageOnError;
            client.Error += MessageOnError;
            client.Join -= OpenPageServer;
            client.Join += OpenPageServer;
            client.JoinServer(login);
        }).Start();
    }

    private void BtnStart_OnClick(object sender, RoutedEventArgs e)
    {
        var server = ServerCore.InicilizeServer(int.Parse(NumPort.Value.ToString()));
        server.Error -= MessageOnError;
        server.Error += MessageOnError;
        server.ServerStart -= OpenPageServer;
        server.ServerStart += OpenPageServer;
        server.Start();
    }

    private bool ValidationAdress(string text)
    {
        foreach (var ch in text)
            if (!(char.IsDigit(ch) | ch == '.'))
                return false;

        var digits = text.Split('.');
        if (digits.Length != 4) return false;
        foreach (var s in digits)
            if (s == string.Empty)
                return false;
        return true;
    }

    private byte[] GetIpAdress(string ipAdress)
    {
        var bytes = new byte[4];
        byte inc = 0;
        foreach (var bit in ipAdress.Split('.'))
        {
            bytes[inc] = byte.Parse(bit);
            inc++;
        }

        return bytes;
    }

    private void PrintAndReturnButton(string title, string message)
    {
        MainWindow.ShowMessage(title, message);
        Dispatcher.Invoke(() =>
        {
            ProgressRing.Visibility = Visibility.Collapsed;
            PanelOfClient.Visibility = Visibility.Visible;
        }, DispatcherPriority.Normal);
    }

    private void OpenPageServer(object sender, EventArgs e)
    {
        MainWindow.OpenPage(@"Client\Pages\PageMessager.xaml");
    }

    private void OpenPageServer(object sender, TcpListener e)
    {
        MainWindow.OpenPage(new Server.Pages.Server());
    }

    private void MessageOnError(object sender, string e)
    {
        MainWindow.ShowMessage("错误", e);
        Dispatcher.Invoke(() =>
        {
            ProgressRing.Visibility = Visibility.Collapsed;
            PanelOfClient.Visibility = Visibility.Visible;
        }, DispatcherPriority.Normal);
    }
}
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChatLAN.Client.Pages.UserControls;
using MahApps.Metro.Controls;

namespace ChatLAN.Client.Pages;

public partial class PageMessager 
{
    public PageMessager()
    {
        InitializeComponent();
        var clientCore = ClientCore.GetCore();
        clientCore.AddMessage += (sender, message) =>
            PanelMessage.Invoke(() =>
            {
                UserControl element = message.Name == ClientCore.NameUser
                    ? new ControlReplyMessage(message){Foreground = new SolidColorBrush(Color.FromArgb(255,255,218,187)) }
                    : new ControlMessage(message){Foreground =Foreground = Brushes.LightBlue };
                element.Margin = new Thickness(5);
           
                PanelMessage.Children.Add(element);
            });
        new Thread(() => clientCore.ReceiveMessage()).Start();
    }
}
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
        ClientCore clientCore = ClientCore.GetCore();
        clientCore.AddMessage += (sender, message) =>
            PanelMessage.Invoke(() =>
            {
                UserControl element;
                if (message.Name == ClientCore.NameUser)
                    element = new ControlMessagRevers(message){Foreground = new SolidColorBrush(Color.FromArgb(255,255,218,187)) };
                else
                    element = new ControlMessag(message){Foreground =Foreground = Brushes.LightBlue };
                element.Margin = new Thickness(5);
           
                PanelMessage.Children.Add(element);
            });
        new Thread(() => clientCore.ReceiveMessage()).Start();
    }
}
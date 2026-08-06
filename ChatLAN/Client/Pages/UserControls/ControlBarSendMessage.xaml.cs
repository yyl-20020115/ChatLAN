using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using ChatLAN.Objects;
using Microsoft.Win32;

namespace ChatLAN.Client.Pages.UserControls;

public partial class ControlBarSendMessage
{
    private Message _message = new Message();

    public string File
    {
        set
        {
            BtnOpenFile.Background = new SolidColorBrush(Color.FromArgb(100, 62, 190, 229));
            _message.File.Name = Path.GetFileName(value);
            _message.File.Data = Util.ReadAllBytes(value);
        }
    }

    public string Text
    {
        set => _message.Text = value;
    }

    public ControlBarSendMessage()
    {
        InitializeComponent();
    }

    private void SendMessage(object sender, RoutedEventArgs e)
    {
        _message.Name = ClientCore.NameUser;
        ClientCore.SendMessage(_message);
        BtnOpenFile.Background = null;
        TbText.Text = String.Empty;
        _message = new Message();
    }

    private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        Text = TbText.Text;
    }

    private void OpenFile(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        if (dialog.ShowDialog().HasValue)
            File = dialog.FileName;
    }
}
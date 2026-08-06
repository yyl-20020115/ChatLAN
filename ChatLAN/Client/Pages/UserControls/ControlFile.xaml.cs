using System;
using System.Windows.Input;

namespace ChatLAN.Client.Pages.UserControls;

public partial class ControlFile
{
    private bool _boolClick;

    public event EventHandler<string> Click;

    public string FileName
    {
        set => TbName.Text = value;
        get => TbName.Text;
    }

    public ControlFile()
    {
        InitializeComponent();
    }

    private void ControlFile_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_boolClick) Click?.Invoke(null, FileName);
    }

    private void ControlFile_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
        _boolClick = true;

    private void ControlFile_OnMouseLeave(object sender, MouseEventArgs e) =>
        _boolClick = false;
}
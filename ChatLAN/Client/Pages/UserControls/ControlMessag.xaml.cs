using ChatLAN.Objects;

namespace ChatLAN.Client.Pages.UserControls;

public partial class ControlMessage
{
    private File _file;
    private File File
    {
        set
        {
            if (value.Data == null) return;
            ControlFile.Visibility = Visibility;
            ControlFile.FileName = value?.Name;
            _file = value;
        }
    }

    private string Text
    {
        set => TbText.Text = value;
        get => TbText.Text;
    }

    private string Nick
    {
        set => TbName.Text = value;
        get => TbName.Text;
    }

    public ControlMessage(Message message)
    {
        InitializeComponent();
        Text = message.Text;
        Nick = message.Name;
        File = message.File;
    }

    private void ControlFile_OnClick(object sender, string e)
    {
        _file.SaveFile();
    }
}
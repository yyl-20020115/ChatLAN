using ChatLAN.Objects;

namespace ChatLAN.Client.Pages.UserControls;

public partial class ControlReplyMessage
{
    public ControlReplyMessage(Message message)
    {
        InitializeComponent();
        Text = message.Text;
        File = message.File;
    }
    private File File
    {
        set
        {
            if (value.Data == null) return;
            ControlFile.Visibility = Visibility;
            ControlFile.FileName = value.Name;
        }
    }

    private string Text
    {
        set => TbText.Text = value;
        get => TbText.Text;
    }
}

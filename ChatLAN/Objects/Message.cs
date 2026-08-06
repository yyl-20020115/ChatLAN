namespace ChatLAN.Objects;

public class Message
{
    public string Text;
    public string Name;
    public File File;

    public Message()
    {
        File = new File();
    }
}

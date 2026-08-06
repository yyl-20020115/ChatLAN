using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ChatLAN.Objects;
using File = System.IO.File;

namespace ChatLAN.Server.Utils;

public static class Serializer
{
    private static readonly string Path;

    static Serializer()
    {
        Path = $"{Environment.CurrentDirectory}/{Const.ServerPathToMessage}/messages.xml";
    }

    public static void SerializeMessage(List<Message> objSerializ)
    {
        if (File.Exists(System.IO.Path.GetDirectoryName(Path)))
        {
            try
            {
                using var stream = new FileStream(Path, FileMode.Create);
                new XmlSerializer(typeof(List<Message>)).Serialize(stream, objSerializ);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public static List<Message> DeserializeMessage()
    {
        if (File.Exists(Path))
        {
            using var stream = new FileStream(Path, FileMode.Open);
            return (List<Message>)new XmlSerializer(typeof(List<Message>)).Deserialize(stream);

        }
        return new List<Message>();
    }
}
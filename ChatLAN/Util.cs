using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;
using ChatLAN.Objects;
using File = System.IO.File;

namespace ChatLAN;

public static class Util
{
    public static event EventHandler<string> Error;

    private static void SerializeObject<TObject>(TObject objSerializ, NetworkStream stream)
    {
        var serializer = new XmlSerializer(typeof(TObject));
        serializer.Serialize(stream, objSerializ);
        using var memoryStream = new StreamWriter($"{Environment.CurrentDirectory}/log.xml", true);
        serializer.Serialize(memoryStream, objSerializ);
    }

    public static void SerializeTypeObject<TObject>(TypeSoketMessage message, TObject objSerializ,
        NetworkStream stream)
    {
        SerializeObject(new TypeMessage<TObject>(objSerializ, message), stream);
        Thread.Sleep(50);
    }

    public static TypeMessage<TObject> DeserializeTypeObject<TObject>(byte[] bytes)
    {
        Thread.Sleep(50);
        return DeserializeObject<TypeMessage<TObject>>(bytes);
    }

    private static TObject DeserializeObject<TObject>(byte[] bytes)
    {
        TObject o = default(TObject);
        try
        {
            var serializer = new XmlSerializer(typeof(TObject));
            using var memory = new MemoryStream(bytes);
                o = (TObject) serializer.Deserialize(memory);
        }
        catch (InvalidOperationException)
        {
            return o;
        }

        return o;
    }

    public enum TypeSoketMessage
    {
        Message,
        Connect,
        ConflictName,
        Ok,
        ListMessage
    }

    public static byte[] ReadAllByte(FileStream stream)
    {
        byte[] array = new byte[stream.Length];
        stream.Read(array, 0, array.Length);
        return array;
    }

    public static MemoryStream ReadAllByte(TcpClient tcpClient)
    {
        try
        {
            using var streamOut = new MemoryStream();
            {
                var streamIn = tcpClient.GetStream();
                byte[] buffer = new byte[64];
                do
                {
                    int size = streamIn.Read(buffer, 0, buffer.Length);
                    streamOut.Write(buffer, 0, size);
                } while (streamIn.DataAvailable);

                using var memoryStream =
                    new FileStream($"{Environment.CurrentDirectory}/logRead.xml", FileMode.OpenOrCreate);
                    memoryStream.Write(streamOut.ToArray(), 0, streamOut.ToArray().Length);
                return streamOut;
            }
        }
        catch (Exception)
        {
            return new MemoryStream();
        }
    }

    public static byte[] ReadAllBytes(TcpClient tcpClient)
        => ReadAllByte(tcpClient).ToArray();

    public static byte[] ReadAllBytes(string pathToFile)
    {
        if (File.Exists(pathToFile))
            using (FileStream stream = new FileStream(pathToFile, FileMode.Open))
                return ReadAllByte(stream);

        Error?.Invoke(null, $"文件 {pathToFile} 未被找到 ");
        return null;
    }
}
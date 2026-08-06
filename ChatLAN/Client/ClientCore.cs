using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ChatLAN.Objects;

namespace ChatLAN.Client;

public class ClientCore
{
    public event EventHandler<Message> AddMessage;
    public event EventHandler<string> Error;
    public event EventHandler Join;
    public static string NameUser;

    private static HashAdpess _hashAdress;
    private static ClientCore _client;
    private List<Message> _listMessage;
    private readonly TcpClient _tcpClient;

    private ClientCore(byte[] ipAdress, int port)
    {
        _hashAdress = new HashAdpess(ipAdress, port);
        try
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(new IPAddress(ipAdress), port);
        }
        catch (SocketException)
        {
            _tcpClient = null;
            Error?.Invoke(null, "连接未建立");
        }

        catch (ObjectDisposedException)
        {
            _tcpClient = null;
            Error?.Invoke(null, "连接已断开");
        }
        finally
        {
            MainWindow.Close -= Disconnect;
            MainWindow.Close += Disconnect;
        }
    }

    public static ClientCore InicializeClient(byte[] ipAdress, int port)
    {
        if (_client == null) return _client = new ClientCore(ipAdress, port);
        if (_hashAdress.Equals(ipAdress, port))
        {
            _client._tcpClient.Close();
            _client = new ClientCore(ipAdress, port);
            return _client;
        }

        if (NameUser == null) return null;
        return null;
    }

    public static void RemoveClient()
    {
        _client._tcpClient.Close();
        _client = null;
    }

    public static ClientCore GetCore()
    {
        if (_client == null) throw new NullReferenceException("需要初始化客户端‌");
        return _client;
    }

    public void JoinServer(string login)
    {
        if (_tcpClient == null)
        {
            Error?.Invoke(null, "服务器未找到");
            _client = null;
            return;
        }

        Util.SerializeTypeObject(Util.TypeSoketMessage.Connect, login, _tcpClient.GetStream());
        byte[] b = Util.ReadAllBytes(_tcpClient);
        var k = Util.DeserializeTypeObject<string>(b);
        if (k == null)
        {
            Error?.Invoke(null, "出了点问题。 \n 请重试");
            return;
        }

        if (Util.TypeSoketMessage.Ok == k.TypeSoketMessage)
        {
            Join?.Invoke(null, null);
            NameUser = login;
        }
        else if (Util.TypeSoketMessage.ConflictName == k.TypeSoketMessage)
            Error?.Invoke(null, "这个名字已被占用");
        else
            Error?.Invoke(null, "无法连接到服务器");
    }

    public static void SendMessage(Message message)
    {
        Util.SerializeTypeObject(Util.TypeSoketMessage.Message, message, _client._tcpClient.GetStream());
    }

    public void ReceiveMessage()
    {
        _listMessage = Util.DeserializeTypeObject<List<Message>>(Util.ReadAllBytes(_tcpClient)).Obj;
        foreach (var message in _listMessage)
            AddMessage?.Invoke(null, message);

        while (true)
        {
            var message = Util.DeserializeTypeObject<Message>(Util.ReadAllBytes(_tcpClient));
            if (message.TypeSoketMessage == Util.TypeSoketMessage.Message)
                AddMessage?.Invoke(null, message.Obj);
        }
    }

    private static void Disconnect(object sender, EventArgs e)
    {
        _client?._tcpClient?.Close();
        Environment.Exit(0);
    }
}

class HashAdpess(byte[] ipAdress, int port)
{
    private readonly byte[] _ipAdress = ipAdress;
    private readonly int _port = port;

    public bool Equals(byte[] ipAdress, int port)
    {
        for (int i = 0; i < 4; i++)
            if (_ipAdress[i] != ipAdress[i])
                return true;
        return _port != port;
    }
}
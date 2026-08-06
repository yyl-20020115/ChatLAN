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
            Error?.Invoke(null, "Подключение не установлено");
        }

        catch (ObjectDisposedException)
        {
            _tcpClient = null;
            Error?.Invoke(null, "Соединение разорвано");
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
        if (_client == null) throw new NullReferenceException("Необходимо инициализировать клиент");
        return _client;
    }

    public void JoinServer(string login)
    {
        if (_tcpClient == null)
        {
            Error?.Invoke(null, "Сервер не найден");
            _client = null;
            return;
        }

        Util.SerializeTypeObject(Util.TypeSoketMessage.Connect, login, _tcpClient.GetStream());
        byte[] b = Util.ReadAllBytes(_tcpClient);
        var k = Util.DeserializeTypeObject<string>(b);
        if (k == null)
        {
            Error?.Invoke(null, "Что-то пошло не так. \n Попробуйте снова");
            return;
        }

        if (Util.TypeSoketMessage.Ok == k.TypeSoketMessage)
        {
            Join?.Invoke(null, null);
            NameUser = login;
        }
        else if (Util.TypeSoketMessage.ConflictName == k.TypeSoketMessage)
            Error?.Invoke(null, "Такое имя занято");
        else
            Error?.Invoke(null, "Не удалось подключиться");
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

class HashAdpess
{
    private readonly byte[] _ipAdress;
    private readonly int _port;

    public HashAdpess(byte[] ipAdress, int port)
    {
        _ipAdress = ipAdress;
        _port = port;
    }

    public bool Equals(byte[] ipAdress, int port)
    {
        for (int i = 0; i < 4; i++)
            if (_ipAdress[i] != ipAdress[i])
                return true;
        if (_port != port)
            return true;
        return false;
    }
}
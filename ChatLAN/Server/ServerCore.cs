using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatLAN.Objects;
using ChatLAN.Server.Utils;

namespace ChatLAN.Server;

public class ServerCore
{
    public event EventHandler<TcpListener> ServerStart;
    public event EventHandler<String> Error;
    public event EventHandler<Message> MessageReceived;

    private readonly List<string> _listUserName = new List<string>();
    private readonly List<Message> _listMessage;
    private readonly TcpListener _tcpListener;
    private readonly Dictionary<string, TcpClient> _tcpClientsOnline = new Dictionary<string, TcpClient>();

    private static ServerCore _serverCore;

    public void RemoveServer()
    {
        _tcpListener.Stop();
        _serverCore = null;
    }

    public void Start()
    {
        try
        {
            _tcpListener.Start();
            ServerStart?.Invoke(null, _tcpListener);
            new Thread(Listen).Start();
        }
        catch (SocketException e)
        {
            RemoveServer();
            Error?.Invoke(e, "端口被占用");
        }
    }

    private ServerCore(int port)
    {
        Util.Error += (sender, args) => Pages.Server.PrintText(args.ToString());
        _tcpListener = new TcpListener(IPAddress.Any, port);
        MainWindow.Close += Disconnect;
        _listMessage = Serializer.DeserializeMessage();
    }

    public static ServerCore InicilizeServer(int port)
    {
        return _serverCore ?? (_serverCore = new ServerCore(port));
    }

    private void SendingMessages(Message message)
    {
        foreach (var client in _tcpClientsOnline)
            Util.SerializeTypeObject(Util.TypeSoketMessage.Message, message, client.Value.GetStream());
    }

    private void ReceivedMessage(Message message)
    {
        MessageReceived?.Invoke(null, message);
        _listMessage.Add(message);
        SendingMessages(message);
    }

    private void ListenClient(TcpClient e, string nameUser)
    {
        new Thread(() =>
        {
            Thread.Sleep(500);
            Util.SerializeTypeObject(Util.TypeSoketMessage.ListMessage, _listMessage, e.GetStream());
            while (true)
            {
                var message = Util.DeserializeTypeObject<Message>(Util.ReadAllBytes(e));
                if (message == null)
                {
                    RemoveUser(nameUser);
                    return;
                }
                Pages.Server.PrintText(message.Obj.Text);
                if (message.TypeSoketMessage == Util.TypeSoketMessage.Message)
                    ReceivedMessage(message.Obj);
            }
        }).Start();
    }

    private void AddBadClient(TcpClient client)
    {
        new Thread(() =>
        {
            while (true)
                if (ValidationUserName(client))
                    break;
        }).Start();
    }

    private bool ValidationUserName(TcpClient tcpClient)
    {
        var client = Util.DeserializeTypeObject<string>(Util.ReadAllBytes(tcpClient));
        if (client == null) return true;
        if (client.TypeSoketMessage != Util.TypeSoketMessage.Connect || _listUserName.Contains(client.Obj))
            return false;
        Util.SerializeTypeObject(Util.TypeSoketMessage.Ok,
            "系统已完成身份验证并准许访问", tcpClient.GetStream());
        AddClientOnline(tcpClient, client.Obj);
        Pages.Server.PrintText("进入聊天室 " + client.Obj); //IP Adress
        return true;
    }

    private void AddClientOnline(TcpClient client, string nameUser)
    {
        _listUserName.Add(nameUser);
        _tcpClientsOnline.Add(nameUser, client);
        ListenClient(client, nameUser);
    }


    private void Listen()
    {
        try
        {
            Pages.Server.PrintText(
                "服务器已启动。等待连接...");
            while (true)
            {
                TcpClient tcpClient = _tcpListener.AcceptTcpClient();
                if (ValidationUserName(tcpClient)) continue;
                AddBadClient(tcpClient);
                Pages.Server.PrintText("服务器出现错误 ");
                Util.SerializeTypeObject(Util.TypeSoketMessage.ConflictName, String.Empty, tcpClient.GetStream());
            }
        }
        catch (Exception)
        {
            new Thread(Listen).Start();
        }
    }

    private void RemoveUser(string nameUser)
    {
        Pages.Server.PrintText($"客户端 {nameUser} 断开连接");
        _tcpClientsOnline[nameUser].Close();
        _tcpClientsOnline.Remove(nameUser);
        _listUserName.Remove(nameUser);
    }

    private void Disconnect(object sender, EventArgs e)
    {
        Serializer.SerializeMessage(_listMessage);
        _tcpListener.Stop();
        foreach (var client in _tcpClientsOnline)
            client.Value.Close();
        Environment.Exit(0);
    }
}
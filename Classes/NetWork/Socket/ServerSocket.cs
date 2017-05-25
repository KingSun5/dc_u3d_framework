using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;  

using UnityEngine;
using System;
/// <summary>
/// 服务端socket
/// @author hannibal
/// @time 2017-5-23
/// </summary>
public class ServerSocket : SocketBase
{
    private uint m_ShareConnID = 0;
    private List<NetChannel> m_NetChannels = new List<NetChannel>();
    private Dictionary<uint, NetChannel> m_DicChannels = new Dictionary<uint, NetChannel>();
    
    public SocketBase.OnAcceptFunction OnAccept;

    public override void Setup()
    {
        base.Setup();
    }

    public override void Destroy()
    {
        OnAccept = null;

        foreach(var obj in m_NetChannels)
        {
            obj.Destroy();
        }
        m_NetChannels.Clear();
        m_DicChannels.Clear();

        base.Destroy();
    }

    public override void Update(float elapse, int game_frame)
    {
        for(int i = m_NetChannels.Count -1; i >= 0; i--)
        {
            m_NetChannels[i].Update(elapse, game_frame);
        }

        base.Update(elapse, game_frame);
    }

    public virtual bool Listen(int port, SocketBase.OnAcceptFunction accept, SocketBase.OnReceiveFunction receive, SocketBase.OnCloseFunction close)
    {
        OnAccept = accept;
        OnReceive = receive;
        OnClose = close;

        //服务器IP地址  
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            m_Socket.Bind(new IPEndPoint(ip, port));  //绑定IP地址：端口  
            m_Socket.Listen(10);    //设定最多10个排队连接请求  
        }
        catch(SocketException e)
        {
            Log.Error("server setup failed:" + e.ToString());
            return false;
        }
        m_Socket.Blocking = true;
        m_Socket.NoDelay = true;
        m_Socket.SendBufferSize = 0xFFFF;
        m_Socket.ReceiveBufferSize = 0xFFFF;
        m_Socket.SendTimeout = 0xbb8;
        m_Socket.ReceiveTimeout = 0xbb8;
        Log.Info("server setup succeed");

        //开始接受连接，异步。
        m_Socket.BeginAccept(new AsyncCallback(OnAcceptClientConnect), m_Socket);

        return true;
    }

    public override int Send(uint conn_id, ByteArray by)
    {
        NetChannel channel;
        if(m_DicChannels.TryGetValue(conn_id, out channel))
        {
            return channel.Send(by);
        }
        return 0;
    }

    /// <summary>  
    /// 监听客户端连接  
    /// </summary>  
    private void OnAcceptClientConnect(IAsyncResult ar)
    {
        //初始化一个SOCKET，用于其它客户端的连接
        Socket server_socket = (Socket)ar.AsyncState;
        Socket client_socket = server_socket.EndAccept(ar);

        NetChannel channel = new NetChannel(this, ++m_ShareConnID);
        channel.Setup(client_socket);
        m_NetChannels.Add(channel);
        m_DicChannels.Add(channel.ConnID, channel);
        if (OnAccept != null) OnAccept(channel.ConnID);

        //等待新的客户端连接
        server_socket.BeginAccept(new AsyncCallback(OnAcceptClientConnect), server_socket);
    }
    /// <summary>
    /// 网络错误
    /// </summary>
    public override void OnNetError(uint conn_id)
    {
        for (int i = 0; i < m_NetChannels.Count; ++i)
        {
            if (m_NetChannels[i].ConnID == conn_id)
            {
                m_NetChannels.RemoveAt(i);
                break;
            }
        }
        m_DicChannels.Remove(conn_id);
    }
}

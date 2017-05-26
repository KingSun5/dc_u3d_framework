using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;  

/// <summary>
/// 客户端socket
/// @author hannibal
/// @time 2017-5-23
/// </summary>
public class ClientSocket : SocketBase
{
    private NetChannel m_ClientChannel;

    public SocketBase.OnConnectedFunction OnConnected;


    public override void Setup()
    {
        base.Setup();
    }

    public override void Destroy()
    {
        OnConnected = null;

        if (m_ClientChannel != null)
        {
            m_ClientChannel.Destroy();
            m_ClientChannel = null;
        }
        base.Destroy();
    }

    public override void Update(float elapse, int game_frame)
    {
        if (m_ClientChannel != null)
        {
            m_ClientChannel.Update(elapse, game_frame);
        }
        base.Update(elapse, game_frame);
    }

    public virtual void Connect(string ip, int port, SocketBase.OnConnectedFunction connected, SocketBase.OnReceiveFunction receive, SocketBase.OnCloseFunction close)
    {
        OnConnected = connected;
        OnReceive = receive;
        OnClose = close;

        IPAddress ipAddress = IPAddress.Parse(ip);
        IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, port);
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // socket 连接成功后，执行回调OnConnect
        m_Socket.BeginConnect(ipEndpoint, new AsyncCallback(OnConnect), m_Socket);
    }

    private void OnConnect(IAsyncResult ar)
    {
        try
        {
            ar.AsyncWaitHandle.Close();
            m_Socket.EndConnect(ar);
            m_Socket.Blocking = true;
            m_Socket.NoDelay = true;
            m_Socket.SendBufferSize = 0xFFFF;
            m_Socket.ReceiveBufferSize = 0xFFFF;
            m_Socket.SendTimeout = 0xbb8;
            m_Socket.ReceiveTimeout = 0xbb8;
            m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            m_ClientChannel = new NetChannel(this, 0);
            m_ClientChannel.Setup(m_Socket);

            if(OnConnected != null)
            {
                OnConnected(0);
            }
        }
        catch (SocketException e)
        {
            Log.Error("OnConnect SocketException:" + e.Message);
            OnDisconnect();
        }
    }

    private void OnDisconnect()
    {

    }
}

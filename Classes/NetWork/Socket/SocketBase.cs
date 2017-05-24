using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;  
using System.Net;  
using System.Threading;  
using System;

/// <summary>
/// socket基类
/// @author hannibal
/// @time 2017-5-23
/// </summary>
public class SocketBase 
{
    public delegate void OnAcceptFunction(uint conn_id);
    public delegate void OnConnectedFunction(uint conn_id);
    public delegate void OnReceiveFunction(uint conn_id, PacketBase packet);
    public delegate void OnCloseFunction(uint conn_id);

    protected Socket m_Socket;

    public SocketBase.OnReceiveFunction OnReceive;
    public SocketBase.OnCloseFunction OnClose;

	public virtual void Setup()
    {

    }

    public virtual void Destroy()
    {
        Close();
    }

    public virtual void Update(float elapse, int game_frame)
    {

    }

    public virtual void Close()
    {
        if(m_Socket != null)
        {
            m_Socket.Close();
            m_Socket = null;
        }
        OnReceive = null;
        OnClose = null;
    }

    public virtual void OnNetError(uint conn_id)
    {

    }
}

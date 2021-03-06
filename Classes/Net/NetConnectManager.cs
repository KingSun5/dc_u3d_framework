﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 链接socket管理器
/// @author hannibal
/// @time 2016-8-17
/// </summary>
public class NetConnectManager : Singleton<NetConnectManager>
{
    private long m_share_conn_idx = 0;
    private Dictionary<long, TCPNetConnecter> m_connectedes = null;

    public NetConnectManager()
    {
        m_connectedes = new Dictionary<long, TCPNetConnecter>();
    }

    public void Setup()
    {

    }

    public void Destroy()
    {
        foreach (var obj in m_connectedes)
            obj.Value.Destroy();
        m_connectedes.Clear();
    }

    public void Tick()
    {
        foreach (var obj in m_connectedes)
        {
            obj.Value.Update();
        }
        foreach (var obj in m_connectedes)
        {
            if (!obj.Value.Valid)//底层已经销毁
            {
                obj.Value.Destroy();
                m_connectedes.Remove(obj.Key);
                break;
            }
        }
    }
    /// <summary>
    /// 连接主机
    /// </summary>
    public long ConnectTo(string ip, ushort port, TCPNetBase.OnConnectedFunction connected, TCPNetBase.OnReceiveFunction receive, TCPNetBase.OnCloseFunction close)
    {
        TCPNetConnecter socket = new TCPNetConnecter();
        socket.Setup();
        socket.conn_idx = ++m_share_conn_idx;
        m_connectedes.Add(socket.conn_idx, socket);
        socket.Connect(ip, port, connected, receive, close);
        return socket.conn_idx;
    }
    /// <summary>
    /// 断开连接
    /// </summary>
    public void Disconnect(long conn_idx)
    {
        TCPNetConnecter socket;
        if (m_connectedes.TryGetValue(conn_idx, out socket))
        {
            socket.Destroy();
        }
        m_connectedes.Remove(conn_idx);
    }
    public int Send(long conn_idx, ByteArray by)
    {
        TCPNetConnecter socket;
        if (m_connectedes.TryGetValue(conn_idx, out socket))
        {
            return socket.Send(conn_idx, by);
        }
        return 0;
    }
}
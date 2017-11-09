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
public sealed class ClientSocket : SocketBase
{
    #region 定义委托
    /// <summary>
    /// 连接成功
    /// </summary>
    /// <param name="conn_idx"></param>
    public delegate void OnConnectOpen(long conn_idx);
    /// <summary>
    /// 接收到客户端的数据
    /// </summary>
    /// <param name="conn_idx"></param>
    /// <param name="buff"></param>
    /// <param name="count"></param>
    public delegate void OnReceiveData(long conn_idx, ushort header, ByteArray by);
    /// <summary>
    /// 关闭连接
    /// </summary>
    /// <param name="conn_idx"></param>
    public delegate void OnConnectClose(long conn_idx);
    #endregion

    #region 定义事件
    public event OnConnectOpen OnOpen;
    public event OnReceiveData OnMessage;
    public event OnConnectClose OnClose;
    #endregion

    private NetChannel m_channel;
    private byte[] m_recv_buffer = new byte[NetID.SendRecvMaxSize];   //读缓存
    private byte[] m_send_buffer = new byte[NetID.SendRecvMaxSize];   //写缓存
    private SendRecvBufferPools m_buffer_pools = new SendRecvBufferPools();

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="connected"></param>
    /// <param name="receive"></param>
    /// <param name="close"></param>
    public void Connect(string ip, int port)
    {
        IPAddress ipAddress = IPAddress.Parse(ip);
        IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, port);
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_Socket.NoDelay = true;
        m_Socket.Blocking = false;
        m_Socket.SendTimeout = 0xbb8;
        m_Socket.ReceiveTimeout = 0xbb8;
        m_Socket.SendBufferSize = NetID.SendRecvMaxSize;
        m_Socket.ReceiveBufferSize = NetID.SendRecvMaxSize;
        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

        try
        {
            m_Socket.BeginConnect(ipEndpoint, new AsyncCallback(OnConnected), m_Socket);
        }
        catch (Exception)
        {
            if (m_Socket != null)
            {
                m_Socket.Close();
                m_Socket = null;
            }
        }
    }

    private void OnConnected(IAsyncResult ar)
    {
        if (m_Socket == null) return;
        try
        {
            ar.AsyncWaitHandle.Close();
            m_Socket.EndConnect(ar);

            m_channel = new NetChannel();
            m_channel.Setup(this, 0);

            this.BeginReceive();
            if (OnOpen != null)
            {
                OnOpen(0);
            }
        }
        catch (SocketException e)
        {
            Log.Error("OnConnect SocketException:" + e.Message);
            this.Close();
        }
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    public override void Close()
    {
        if (m_Socket != null)
        {
            try
            {
                m_Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
            m_Socket.Close();
            m_Socket = null;
        }
        if (OnClose != null)
        {
            OnClose.Invoke(0);
            OnClose = null;
        }
        base.Close();
    }

    private void BeginReceive()
    {
        if (m_Socket == null) return;
        m_Socket.BeginReceive(m_recv_buffer, 0, m_recv_buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReceiveComplete), m_recv_buffer);
    }
    /// <summary>
    /// 接收数据
    /// </summary>
    private void OnReceiveComplete(IAsyncResult ar)
    {
        if (m_Socket == null) return;
        try
        {
            ar.AsyncWaitHandle.Close();
            byte[] buf = (byte[])ar.AsyncState;
            int len = m_Socket.EndReceive(ar);

            if (len > 0)
            {
                if (m_channel != null) m_channel.HandleReceive(buf, len);
                this.BeginReceive();
            }
            else
            {
                Log.Error("OnReceive Recv Error");
                this.Close();
                return;
            }
        }
        catch (SocketException e)
        {
            if (e.ErrorCode != 10054) Log.Exception(e);
            this.Close();
        }
        catch (Exception e)
        {
            Log.Exception(e);
            this.Close();
            return;
        }
    }
    public override void HandleReceive(long conn_idx, ushort header, ByteArray by)
    {
        if (OnMessage == null) return;
        try
        {
            OnMessage(conn_idx, header, by);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }

    public void Send(byte[] message, int offset, int count)
    {
        if (m_Socket == null || !m_Socket.Connected || message == null)
            return;
        SendRecvBuffer buffer = m_buffer_pools.Spawn();
        buffer.Socket = m_Socket;
        System.Array.Copy(message, offset, buffer.Buffer, 0, count);
        try
        {
            buffer.Socket.BeginSend(buffer.Buffer, 0, count, 0, new AsyncCallback(OnSendComplete), buffer);
        }
        catch (Exception e)
        {
            Log.Error("发送失败:" + e.Message);
            this.Close();
        }
    }
    private void OnSendComplete(IAsyncResult ar)
    {
        ar.AsyncWaitHandle.Close();
        SendRecvBuffer buffer = (SendRecvBuffer)ar.AsyncState;
        //已经断开连接
        if (buffer.Socket == null || !buffer.Socket.Connected)
        {
            this.Close();
            return;
        }

        try
        {
            buffer.Socket.EndSend(ar);
            m_buffer_pools.Despawn(buffer);
        }
        catch (Exception e)
        {
            Log.Exception(e);
            this.Close();
        }
    }
}

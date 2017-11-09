using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;  
using UnityEngine;
using System;

/// <summary>
/// 服务端tcp
/// @author hannibal
/// @time 2017-5-23
/// </summary>
public sealed class TCPServerSocket : SocketBase
{        
    #region 定义委托
    /// <summary>
    /// 连接成功
    /// </summary>
    /// <param name="conn_idx"></param>
    public delegate void OnAcceptConnect(long conn_idx);
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
    public event OnAcceptConnect OnOpen;
    public event OnReceiveData OnMessage;
    public event OnConnectClose OnClose;
    #endregion

    private long m_share_conn_idx = 0;
    private object m_sync_lock = new object();
    private byte[] m_recv_buffer = new byte[NetID.SendRecvMaxSize];   //读缓存
    private byte[] m_send_buffer = new byte[NetID.SendRecvMaxSize];   //写缓存
    private SendRecvBufferPools m_buffer_pools = null;
    private UserTokenPools m_user_tokens_pools = null;
    private Dictionary<long, UserToken> m_user_tokens = null;
    private Dictionary<long, NetChannel> m_channels = null;

    public TCPServerSocket()
    {
        m_buffer_pools = new SendRecvBufferPools();
        m_user_tokens_pools = new UserTokenPools();
        m_user_tokens = new Dictionary<long, UserToken>();
        m_channels = new Dictionary<long, NetChannel>();
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    public override void Close()
    {
        Socket socket = null;
        lock (m_sync_lock)
        {
            foreach (var obj in m_channels)
            {
                obj.Value.Destroy();
                NetChannelPools.Despawn(obj.Value);
            }
            m_channels.Clear();

            foreach (var obj in m_user_tokens)
            {
                socket = obj.Value.Socket;
                if (socket != null)
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception) { }
                    socket.Close();
                }
                m_user_tokens_pools.Despawn(obj.Value);
                if (OnClose != null) OnClose(obj.Key);
            }
            m_user_tokens.Clear();
        }
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
        OnOpen = null;
        OnMessage = null;
        OnClose = null;
        base.Close();
    }
    /// <summary>
    /// 主动关闭
    /// </summary>
    public void CloseConn(long conn_idx)
    {
        UserToken token = null;
        lock (m_sync_lock)
        {
            if (m_user_tokens.TryGetValue(conn_idx, out token))
            {
                if (token.Socket != null)
                {
                    try
                    {
                        token.Socket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception) { }
                    token.Socket.Close();
                    token.Socket = null;
                }
                m_user_tokens_pools.Despawn(token);
            }
            m_user_tokens.Remove(conn_idx);

            NetChannel channel;
            if (m_channels.TryGetValue(conn_idx, out channel))
            {
                NetChannelPools.Despawn(channel);
            }
            m_channels.Remove(conn_idx);

            if (OnClose != null) OnClose(conn_idx);
        }
    }
    /// <summary>
    /// 关闭客户端:内部出现错误时调用
    /// </summary>
    private void CloseClientSocket(long conn_idx)
    {
        UserToken token = null;
        lock (m_sync_lock)
        {
            if (m_user_tokens.TryGetValue(conn_idx, out token))
            {
                if (token.Socket != null)
                {
                    try
                    {
                        token.Socket.Shutdown(SocketShutdown.Send);
                    }
                    catch (Exception) { }
                    token.Socket.Close();
                    token.Socket = null;
                }
                m_user_tokens_pools.Despawn(token);
                m_user_tokens.Remove(token.ConnId);

                NetChannel channel;
                if (m_channels.TryGetValue(conn_idx, out channel))
                {
                    NetChannelPools.Despawn(channel);
                }
                m_channels.Remove(conn_idx);

                if (OnClose != null) OnClose(token.ConnId);
            }
        }
    }
    public bool Start(ushort port)
    {
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_Socket.NoDelay = true;
        m_Socket.Blocking = false;
        m_Socket.SendBufferSize = NetID.SendRecvMaxSize;
        m_Socket.ReceiveBufferSize = NetID.SendRecvMaxSize;
        m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        try
        {
            m_Socket.Bind(new IPEndPoint(IPAddress.Any, port));  //绑定IP地址：端口  
            m_Socket.Listen(100);
            m_Socket.BeginAccept(new AsyncCallback(OnAccept), m_Socket);
            return true;
        }
        catch (Exception e)
        {
            Log.Exception(e);
            this.Close();
            return false;
        }
    }
    private void OnAccept(IAsyncResult ar)
    {
        Socket server_socket = (Socket)ar.AsyncState;
        ar.AsyncWaitHandle.Close();
        try
        {
            //初始化一个SOCKET，用于其它客户端的连接
            Socket client_socket = server_socket.EndAccept(ar);
            lock (m_sync_lock)
            {
                long conn_idx = ++m_share_conn_idx;
                //创建token
                UserToken token = m_user_tokens_pools.Spawn();
                token.ConnId = conn_idx;
                token.Socket = client_socket;
                m_user_tokens.Add(conn_idx, token);
                //创建channel
                NetChannel channel = NetChannelPools.Spawn();
                channel.Setup(this, conn_idx);
                m_channels.Add(channel.conn_idx, channel);

                if (OnOpen != null) OnOpen(conn_idx);

                //连接成功，有可能被踢出，需要再次判断是否有效
                if (m_user_tokens.ContainsKey(conn_idx))
                {
                    SendRecvBuffer buffer = m_buffer_pools.Spawn();
                    buffer.ConnId = token.ConnId;
                    buffer.Socket = token.Socket;
                    BeginReceive(buffer);
                }
                //等待新的客户端连接
                server_socket.BeginAccept(new AsyncCallback(OnAccept), server_socket);
            }
        }
        catch (Exception e)
        {
            Log.Exception(e);
            this.Close();
            return;
        }
    }
    private void BeginReceive(SendRecvBuffer buffer)
    {
        if (m_Socket == null) return;
        buffer.Socket.BeginReceive(buffer.Buffer, 0, buffer.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), buffer);
    }
    /// <summary>
    /// 接收数据
    /// </summary>
    private void OnReceive(IAsyncResult ar)
    {
        SendRecvBuffer buffer = (SendRecvBuffer)ar.AsyncState;
        ar.AsyncWaitHandle.Close();
        try
        {
            if (buffer.Socket == null) return;
            lock (m_sync_lock)
            {
                int len = buffer.Socket.EndReceive(ar);
                if (!m_user_tokens.ContainsKey(buffer.ConnId)) return;
                if (len > 0)
                {
                    NetChannel channel;
                    if(m_channels.TryGetValue(buffer.ConnId, out channel))
                    {
                        channel.HandleReceive(buffer.Buffer, len);
                    }

                    //派发消息的时候，有可能上层逻辑关闭了当前连接，必须再判断一次当前连接是否正常
                    if (m_user_tokens.ContainsKey(buffer.ConnId))
                    {
                        this.BeginReceive(buffer);
                    }
                    else
                    {
                        m_buffer_pools.Despawn(buffer);
                    }
                }
                else
                {
                    Log.Error("OnReceive Recv Error");
                    m_buffer_pools.Despawn(buffer);
                    this.CloseClientSocket(buffer.ConnId);
                }
            }
        }
        catch (SocketException e)
        {
            if (e.ErrorCode != 10054) Log.Exception(e);
            m_buffer_pools.Despawn(buffer);
            this.CloseClientSocket(buffer.ConnId);
        }
        catch (Exception e)
        {
            Log.Exception(e);
            m_buffer_pools.Despawn(buffer);
            this.CloseClientSocket(buffer.ConnId);
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

    public void Send(long conn_idx, byte[] message, int offset, int count)
    {
        UserToken token;
        if (!m_user_tokens.TryGetValue(conn_idx, out token) || token.Socket == null || !token.Socket.Connected || message == null)
            return;

        SendRecvBuffer buffer = m_buffer_pools.Spawn();
        buffer.ConnId = conn_idx;
        buffer.Socket = token.Socket;
        System.Array.Copy(message, offset, buffer.Buffer, 0, count);
        try
        {
            buffer.Socket.BeginSend(buffer.Buffer, 0, count, 0, new AsyncCallback(OnSend), buffer);
        }
        catch (Exception e)
        {
            Log.Error("发送失败:" + e.Message);
            this.CloseClientSocket(conn_idx);
            m_buffer_pools.Despawn(buffer);
        }
    }
    private void OnSend(IAsyncResult ar)
    {
        ar.AsyncWaitHandle.Close();
        SendRecvBuffer buffer = (SendRecvBuffer)ar.AsyncState;

        //已经断开连接
        if (buffer.Socket == null || !buffer.Socket.Connected)
        {
            m_buffer_pools.Despawn(buffer);
            this.CloseClientSocket(buffer.ConnId);
            return;
        }

        try
        {
            buffer.Socket.EndSend(ar);
        }
        catch (Exception e)
        {
            Log.Exception(e);
            this.CloseClientSocket(buffer.ConnId);
        }
        finally
        {
            m_buffer_pools.Despawn(buffer);
        }
    }
}
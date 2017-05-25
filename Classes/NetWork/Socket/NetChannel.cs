using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;  
using UnityEngine;
using System;

/// <summary>
/// 网络连接管道
/// @author hannibal
/// @time 2017-5-24
/// </summary>
public class NetChannel
{
    private bool m_Active;
    private uint m_ConnID;
    private Socket m_Socket;
    private SocketBase m_NetSocket;
    private byte[] m_DataBuffer = new byte[0xFFFF];
    private ByteArray m_ByBuffer;

    public NetChannel(SocketBase socket, uint conn_id)
    {
        m_NetSocket = socket;
        m_ConnID = conn_id;
        m_ByBuffer = new ByteArray(PacketID.InitByteArraySize, PacketID.MaxByteArraySize);
    }

    public void Setup(Socket socket)
    {
        m_Socket = socket;
        m_Active = true;
    }

    public void Destroy()
    {
        if(m_Socket != null)
        {
            m_Socket.Close();
            m_Socket = null;
        }
        m_Active = false;
    }

    public void Update(float elapse, int game_frame)
    {
        if (!m_Active || m_Socket == null) return;
        if(!IsSocketConnected(m_Socket))
        {
            HandleDisconnect();
            return;
        }
        HandleReceive();
    }
    /// <summary>
    /// 发射数据
    /// </summary>
    public int Send(ByteArray by)
    {
        uint data_len = by.Available();
        byte[] b = System.BitConverter.GetBytes(data_len);
        System.Array.Copy(b, 0, m_DataBuffer, 0, PacketID.PacketHeadLengthSize);
        by.Read(ref m_DataBuffer, data_len, PacketID.PacketHeadLengthSize);
        int n = m_Socket.Send(m_DataBuffer, 0, (int)(data_len + PacketID.PacketHeadLengthSize), SocketFlags.None);
        return n;
    }
    /// <summary>
    /// 接收数据
    /// </summary>
    private void HandleReceive()
    {
        if (m_Socket == null || m_Socket.Available == 0) return;

        int len = 0;
        try
        {
            len = m_Socket.Receive(m_DataBuffer, SocketFlags.None);
        }
        catch (SocketException e)
        {
            Log.Error("HandleReceive SocketException:" + e.Message);
            HandleDisconnect();
            return;
        }
        catch(Exception e)
        {
            Log.Error(e.Message);
            HandleDisconnect();
            return;
        }
        if (len == 0) return;

        m_ByBuffer.WriteBytes(m_DataBuffer, (uint)len);

        ParsePacket();
    }

    private void ParsePacket()
    {
        while (m_ByBuffer.Available() >= PacketID.PacketHeadSize)
        {
            byte[] by = new byte[PacketID.PacketHeadLengthSize];
            if(m_ByBuffer.Peek(ref by, PacketID.PacketHeadLengthSize))
            {
                ushort msg_length = BitConverter.ToUInt16(by, 0);
                if (m_ByBuffer.Available() >= msg_length + PacketID.PacketHeadLengthSize)
                {
                    //读取包数据
                    m_ByBuffer.Skip(PacketID.PacketHeadLengthSize);
                    ushort header = m_ByBuffer.ReadUShort();
                    m_ByBuffer.Read(ref m_DataBuffer, msg_length - (uint)sizeof(ushort));

                    //构建数据包
                    RecvPacket packet = new RecvPacket();
                    packet.header = header;
                    packet.data.Write(m_DataBuffer, (uint)(msg_length - sizeof(ushort)));

                    //派发数据
                    if (m_NetSocket != null && m_NetSocket.OnReceive != null)
                    {
                        m_NetSocket.OnReceive(m_ConnID, packet);
                    }
                }
                else
                    break;
            }
            else
                break;
        }
    }

    private void HandleDisconnect()
    {
        m_NetSocket.OnNetError(m_ConnID);
        if (m_NetSocket != null && m_NetSocket.OnClose != null)
        {
            m_NetSocket.OnClose(m_ConnID);
        }
    }

    private float tmpLastPollTime = 0;
    private bool IsSocketConnected(Socket s)
    {
        if (Time.realtimeSinceStartup - tmpLastPollTime < 0) return true;
        tmpLastPollTime = Time.realtimeSinceStartup + 1;
        return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
    }

    public uint ConnID
    {
        get { return m_ConnID; }
    }
}

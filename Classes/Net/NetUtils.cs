using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class NetUtils
{
    private static ByteArray tmpSendBy = new ByteArray(1024, SocketID.SendRecvMaxSize);
    public static ByteArray AllocSendPacket()
    {
        tmpSendBy.Clear();
        tmpSendBy.WriteUShort(0);//协议头，包长度
        return tmpSendBy;
    }

    /// <summary>
    /// 本机ip
    /// </summary>
    /// <returns></returns>
    public static string GetLocalIpv4()
    {
        IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
        for (int i = 0; i < IpEntry.AddressList.Length; i++)
        {
            if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
            {
                return IpEntry.AddressList[i].ToString();
            }
        }
        return "127.0.0.1";
    }
}

/// <summary>
/// 网络事件
/// </summary>
public class NetEventID
{
    public const string NET_DISCONNECT = "NET_DISCONNECT";     //网络连接断开
    public const string CONNECT_SUCCEED = "CONNECT_SUCCEED";	//服务器连接成功
    public const string CONNECT_FAILED = "CONNECT_FAILED";     //服务器连接失败
}
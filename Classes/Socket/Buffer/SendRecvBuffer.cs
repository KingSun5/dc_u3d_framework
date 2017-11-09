using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class SendRecvBuffer
{
    /// <summary>
    /// 连接id
    /// </summary>
    public long ConnId { get; set; }
    /// <summary>
    /// 通信SOKET
    /// </summary>
    public Socket Socket { get; set; }
    /// <summary>
    /// 传递给iocp的buffer
    /// </summary>
    public byte[] Buffer = new byte[NetID.SendRecvMaxSize];
}

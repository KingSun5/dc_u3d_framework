using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UserToken
{
    /// <summary>
    /// 连接id
    /// </summary>
    public long ConnId { get; set; }
    /// <summary>
    /// 远程地址
    /// </summary>
    public EndPoint Remote { get; set; }
    /// <summary>
    /// 通信SOKET
    /// </summary>
    public Socket Socket { get; set; }
    /// <summary>
    /// 传递给iocp的buffer
    /// </summary>
    public byte[] Buffer = new byte[NetID.SendRecvMaxSize];
}

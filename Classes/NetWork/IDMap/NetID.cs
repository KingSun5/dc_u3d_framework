using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetID 
{
    public const uint InitByteArraySize = 32767;
    public const uint MaxByteArraySize = 65535;

    public const uint PacketHeadSize = 4;
    public const uint PacketHeadLengthSize = 2;

    public const uint SendPacketMaxSize = 4096;
    public const uint RecvPacketMaxSize = 4096;
}
/// <summary>
/// 网络事件
/// </summary>
public class NetEventID
{
    public const string NET_DISCONNECT  = "NET_DISCONNECT";     //网络连接断开
    public const string CONNECT_SUCCEED = "CONNECT_SUCCEED";	//服务器连接成功
    public const string CONNECT_FAILED  = "CONNECT_FAILED";     //服务器连接失败
}
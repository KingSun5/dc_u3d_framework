using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

/// <summary>
/// 协议包
/// @author hannibal
/// @time 2017-5-23
/// </summary>
public class PacketBase
{
    public ushort header;
}

/// <summary>
/// 接收包
/// </summary>
public class RecvPacket : PacketBase
{
    public ByteArray data;

    public RecvPacket()
    {
        data = new ByteArray(1024, NetID.RecvPacketMaxSize);
    }
}

public class SendPacket : PacketBase
{
    public ByteArray data;

    public SendPacket()
    {
        data = new ByteArray(1024, NetID.SendPacketMaxSize);
    }
}
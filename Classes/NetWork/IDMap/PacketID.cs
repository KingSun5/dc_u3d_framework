using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketID 
{
    public const uint InitByteArraySize = 32767;
    public const uint MaxByteArraySize = 65535;

    public const uint PacketHeadSize = 4;
    public const uint PacketHeadLengthSize = 2;

    public const uint SendPacketMaxSize = 4096;
    public const uint RecvPacketMaxSize = 4096;
}

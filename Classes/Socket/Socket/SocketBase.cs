using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;  
using System.Net;  
using System.Threading;  
using System;

/// <summary>
/// socket基类
/// @author hannibal
/// @time 2017-5-23
/// </summary>
public class SocketBase 
{    
    protected Socket m_Socket = null;

    public virtual void HandleReceive(long conn_idx, ushort header, ByteArray by)
    {

    }
    public virtual void Close()
    {
        m_Socket = null;
    }
}

using System;
using System.Collections.Generic;

/// <summary>
/// socket基类
/// @author hannibal
/// @time 2016-5-30
/// </summary>
public class TCPNetBase
{
    public delegate void OnAcceptFunction(long conn_idx);
    public delegate void OnConnectedFunction(long conn_idx);
    public delegate void OnReceiveFunction(long conn_idx, ushort header, ByteArray data);
    public delegate void OnCloseFunction(long conn_idx);

    public TCPNetBase.OnAcceptFunction OnAccept = null;
    public TCPNetBase.OnConnectedFunction OnConnected = null;
    public TCPNetBase.OnReceiveFunction OnReceive = null;
    public TCPNetBase.OnCloseFunction OnClose = null;

    public virtual void Setup()
    {

    }
    /// <summary>
    /// 外部调用，销毁socket
    /// </summary>
    public virtual void Destroy()
    {
        Close();
    }

    public virtual void Update()
    {

    }

    private static long send_count = 0;
    public virtual int Send(long conn_idx, ByteArray by)
    {
        ++send_count;
        if (send_count % 100000 == 0)
            Log.Debug("已发包:" + send_count);
        return 0;
    }
    /// <summary>
    /// 内部调用或底层触发
    /// </summary>
    public virtual void Close()
    {
        OnReceive = null;
        OnClose = null;
    }
    public virtual bool Valid
    {
        get { return false; }
    }
}
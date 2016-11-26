using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.IO;



public class NetClient //: Controller
{
	public string mServerIP;
	public int mServerPort;
	public int mConnectMaxWaitTime = 10000;
	private int mConnectWaitTime = 0;
    private int mRetryConnNextTime = 0;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
	public enum ENetState
	{
		ES_UnInit = 0,	// 未连接
		ES_Connecting,	// 正在连接
		ES_Connected,	// 已连接
		ES_Disconnect,	// 断开连接
        ES_ReConnect,	// 重连中
	}
	protected ENetState mNetState;
	
	public ENetState NetState
	{
		get { return mNetState; }
	}


	protected object mLock;	// For multi-thread.


    private byte[] m_RecvBuffers = new byte[4096];
    protected int m_RecvPacketLength;
    protected int m_RecvDispatchIndex;
    private SendDataPacket m_SendDataPacket = new SendDataPacket();
    private SendDataPacket m_RequestDataPacket = new SendDataPacket();
    protected int m_SendPacketLength;

    public List<SGateProtocol> m_DispatchRecvPacketList = new List<SGateProtocol>();//分发包列表
	
	//		static public UInt16 headerByReply(ref Reply reply)
	//		{
	//			return BitConverter.ToUInt16(reply.Data.ToArray(), 0);
	//		}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
	protected Socket mSocket = null;
	protected bool mFirstConnect;
	protected int mPackNum;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
	private IAsyncResult m_ar_Connect = null;
	private IAsyncResult m_ar_Recv = null;
	private IAsyncResult m_ar_Send = null;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void Start() {
		mPackNum = 0;
		mLock = new object();
		mNetState = ENetState.ES_UnInit;
        m_RecvPacketLength = 0;
        m_RecvDispatchIndex = 0;
	}



	public void StartConnect(string ip, int port)
	{
		lock(mLock)
		{
			if (mNetState == ENetState.ES_Connecting || mNetState == ENetState.ES_Connecting)
				return;
            m_RecvPacketLength = 0;
            m_RecvDispatchIndex = 0;
			// 清空 加密key。
			mPackNum = 0;
			mNetState = ENetState.ES_UnInit;
			
			if (mSocket != null){
				Stop();
			}
			
			mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			//mThread = new Thread(new ThreadStart(_thread_run));
			//mThread.IsBackground = true;
			//mThread.Start();
			if (ip == mServerIP && port == mServerPort && mSocket.Connected)
			{
				//mListener.OnConnect();
				this.OnConnect();
				return;
			}
			
			mServerIP = ip;
			mServerPort = port;
			//异步请求socket 连接
			Connect();
			
			mNetState = ENetState.ES_Connecting;
			mConnectWaitTime = 0;
		}
	}
	
	public void Clear()
	{
		if (m_ar_Connect != null)
			m_ar_Connect.AsyncWaitHandle.Close();
		if (m_ar_Recv != null)
			m_ar_Recv.AsyncWaitHandle.Close();
		if (m_ar_Send != null)
			m_ar_Send.AsyncWaitHandle.Close();
		mPackNum = 0;
        m_RecvPacketLength = 0;
        m_RecvDispatchIndex = 0;
        m_RequestDataPacket.Clear();
	}
	
	public void Stop()
	{
		lock(mLock)
		{
			Clear();
			
			if (mSocket != null)
			{
				if (mSocket.Connected)
				{
					mSocket.Close();
				}
				
				mSocket = null;
			}
			
			mNetState = ENetState.ES_UnInit;
			
			Clear();
			
		}
	}

	
	public void SendData(byte[] oriData ,int index,int count) // twp.app.cooldown.CooldownSort.CDS_INVALID = 0xfffFFFF
	{
        if (mSocket == null)
            return;
        if (count <= 0)
            return;
        if (oriData == null)
            return;
        if (mNetState != ENetState.ES_Connected)
            return;
        lock (mLock)
        {
            byte[] buffer = new byte[count];
            Array.Copy(oriData, index, buffer, 0, count);
            mSocket.BeginSend(buffer, 0, count, SocketFlags.None, new AsyncCallback(this._scb_send), mSocket);
        }
	}
	
	protected void Connect()
	{
		mFirstConnect = true;
		IPAddress ipAddress = IPAddress.Parse(mServerIP);
		IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, mServerPort);
		// socket 连接成功后，执行回调_scb_connect
		m_ar_Connect = mSocket.BeginConnect(ipEndpoint, new AsyncCallback(_scb_connect), mSocket);
	}
	
	//侦听 服务器发来的字节流
	protected void Recv() 
	{
		try
		{
			byte[] buf = new byte[0x1000];
			this.m_ar_Recv = mSocket.BeginReceive(buf, 0, 0x1000, SocketFlags.None, new AsyncCallback(this._scb_recv), buf);
		}
		catch (SocketException e)
		{
			Debug.Log("Recv SocketException:" + e.Message);
			this.OnDisconnect("Recv SocketException:" + e.Message);
		}
	}

	void _scb_send(IAsyncResult ar)
	{
		try
		{
            lock (mLock)
            {
                ar.AsyncWaitHandle.Close();
                ((Socket)ar.AsyncState).EndSend(ar);
            }
		}
		catch (SocketException e)
		{
			Debug.Log("_scb_send SocketException:" + e.Message);
			this.OnDisconnect("_scb_send SocketException:" + e.Message);
		}
	}

	
	void _scb_recv(IAsyncResult ar)
	{
		try
		{
			ar.AsyncWaitHandle.Close();
			byte[] buf = (byte[]) ar.AsyncState;
			if(mSocket == null)
				return;
			int len = mSocket.EndReceive(ar);
			this.m_ar_Recv = null;
			if (len > 0)
			{
                if (len + m_RecvDispatchIndex > m_RecvBuffers.Length)
                {
                    int newSize = IntUtils.upper_power_of_two(len + m_RecvDispatchIndex + 1);
                    Array.Resize(ref m_RecvBuffers, newSize);
                }
                Array.Copy(buf, 0, m_RecvBuffers, m_RecvPacketLength, len);
                m_RecvPacketLength += len;
                m_RecvDispatchIndex = 0;
			}else{
				Debug.Log("EndReceive = 0");
				this.OnDisconnect("EndReceive = 0");
				return;
			}
            //处理接收的数据
            lock (mLock)
            {
                DispatchPackets();
            }
			//服务器发来的字节流回调已经用了， 继续侦听回调
			mSocket.BeginReceive(buf, 0, 0x1000, SocketFlags.None, new AsyncCallback(this._scb_recv), buf);

		}
		catch (SocketException e)
		{
			Debug.Log("_scb_recv SocketException:" + e.Message);
			this.OnDisconnect("_scb_recv SocketException:" + e.Message);
		}
	}
	
	protected void _scb_connect(IAsyncResult ar)
	{
		try
		{
			ar.AsyncWaitHandle.Close();
			mSocket.EndConnect(ar);
			this.m_ar_Connect = null;
			
			this.OnConnect();
			
			mSocket.Blocking = false;
			if (mSocket != null)
			{
				mSocket.ReceiveTimeout = 0xbb8;
				mSocket.SendTimeout = 0xbb8;
			}
			
			//开始侦听 服务器发来的字节流
			this.Recv();
		}
		catch (SocketException e)
		{ 
			UnityEngine.Debug.Log("_scb_connect SocketException:" + e.Message);
			this.OnDisconnect("_scb_connect SocketException:" + e.Message);
		}
		
	}
	
	protected virtual void OnConnect()
	{
		lock(mLock)
		{
			mNetState = ENetState.ES_Connected;
            m_SendDataPacket.Clear();
            m_RequestDataPacket.Clear();
            //派发网络连接事件
		}
	}

    public virtual bool IsConnected()
    {
        return mNetState == ENetState.ES_Connected;
    }


    protected virtual void OnConnectTimeUp()
    {
        Log.Warning("网络连接超时");
    }
	
	protected virtual void OnDisconnect(string e = "NULL")
	{
		lock(mLock)
		{
            mNetState = ENetState.ES_Disconnect;
            m_SendDataPacket.Clear();
            m_RequestDataPacket.Clear();

		}
	}

    protected virtual void revertRecvBuffers()
    {
        //将当前接收的数据流重新偏移到目标头部
        int dwRemainSize = m_RecvPacketLength - m_RecvDispatchIndex;
        if(dwRemainSize > 0 ){
            //将未读取的数据流部分转移到头部位置
            Array.Copy(m_RecvBuffers,m_RecvDispatchIndex,m_RecvBuffers,0,dwRemainSize);
        }
        m_RecvDispatchIndex = 0;
        m_RecvPacketLength = dwRemainSize;
    }
    protected virtual int DispatchPackets()
    {
        int headerSize = 5;//sizeof(header);//System.Runtime.InteropServices.Marshal.SizeOf(NetProtocolDefine.SGateProtocol);

        while (true)
        {
            int dwRemainSize = m_RecvPacketLength - m_RecvDispatchIndex;
            if (dwRemainSize < headerSize)
            {
                revertRecvBuffers();
                break;
            }
            int offsetIdx = m_RecvDispatchIndex;
            uint headerIdent = 0;
            byte headerProtocolType = 0;
            do
            {
                headerProtocolType = m_RecvBuffers[m_RecvDispatchIndex ];
                headerIdent = ByteUtils.Byte4ToUInt32(m_RecvBuffers[m_RecvDispatchIndex + 1], m_RecvBuffers[m_RecvDispatchIndex + 2],
                    m_RecvBuffers[m_RecvDispatchIndex + 3], m_RecvBuffers[m_RecvDispatchIndex + 4]);
                if (headerIdent == NetProtocol.NetPacketHeaderIdent)
                    break;
                m_RecvDispatchIndex++;
            } while (m_RecvDispatchIndex >= m_RecvPacketLength);
            if (headerIdent != NetProtocol.NetPacketHeaderIdent)
            {
                revertRecvBuffers();
                break;
            }
            int nProtocolSize = onParseProtocol(headerProtocolType, m_RecvBuffers, m_RecvDispatchIndex + headerSize);
            if (nProtocolSize == -1) //未知的大小/错误的大小
            {
                m_RecvDispatchIndex += headerSize;
                revertRecvBuffers();
                break;
            }
            else if ((headerSize + nProtocolSize) > dwRemainSize)
            {
                revertRecvBuffers();
                break;
            }
            else
            {
                SGateProtocol protocol = new SGateProtocol();
                protocol.buffers = new byte[nProtocolSize];
                protocol.m_uId = headerProtocolType;
                protocol.m_uHeader = headerIdent;
                Array.Copy(m_RecvBuffers, m_RecvDispatchIndex + headerSize, protocol.buffers, 0, nProtocolSize);
                protocol.packet = new RecvDataPacket(protocol.buffers,0, nProtocolSize);
                m_DispatchRecvPacketList.Add(protocol);
                m_RecvDispatchIndex += headerSize + nProtocolSize;
            }
        }
        return 0;
    }


    public  virtual int onParseProtocol(int uId,byte[] buffer,int bufIdx)
    {
        int nPacketSize = 0;

        return nPacketSize;
    }


    public virtual bool onDispatchProtocol(SGateProtocol protocol)
    {
        return true;
    }

	public float m_DisconnectMarkTime = -1;
	public float m_OldTimeScale = 0;
	public virtual void Update()
	{
// 		if (m_DisconnectMarkTime > 0) {
// 			if(Time.realtimeSinceStartup - m_DisconnectMarkTime > 5){
// 				UnityEngine.Debug.Log("Net_DisConnected Fired");
// 				//SendEvent(GameEvent.UIEvent.EVENT_UI_HIDE_LOADING_TRANSPARENT, -1f);
// 				Time.timeScale = m_OldTimeScale;
// 				m_DisconnectMarkTime = -1;
// 				//SendEvent(GameEvent.WebEvent.EVENT_WEB_DISCONNECT, null);
// 			}
// 		}
		if(mLock == null) return;
		lock(mLock)
		{	
			if (mNetState == ENetState.ES_Connecting)
			{
				mConnectWaitTime += (int)(UnityEngine.Time.deltaTime*1000.0f);
				if (mConnectWaitTime > mConnectMaxWaitTime)
				{
					mNetState = ENetState.ES_Disconnect;
					
					if (mSocket != null)
					{
						mSocket.Close();							
						mSocket = null;
					}
                    OnConnectTimeUp();
				}
			}
//             if (m_ar_Send == null && m_RequestDataPacket.Length > 0)
//             {
//                 m_SendDataPacket.WriteBuffer(m_RequestDataPacket.getData(),0,m_RequestDataPacket.Length);
//                 Send();
//                 m_RequestDataPacket.Clear();
//             }
            for (int i = 0; i < m_DispatchRecvPacketList.Count; i++)
            {
                onDispatchProtocol(m_DispatchRecvPacketList[i]);
            }
            m_DispatchRecvPacketList.Clear();
			
			if (mNetState == ENetState.ES_Disconnect)
            {
                mNetState = ENetState.ES_ReConnect;
                Clear();
                mRetryConnNextTime = (int)(UnityEngine.Time.deltaTime * 1000.0f) + 3000;
            }
            if (mNetState == ENetState.ES_ReConnect)
            {
                if (mRetryConnNextTime <= (UnityEngine.Time.deltaTime * 1000.0f))
                {
                    StartConnect(mServerIP,mServerPort);
                    mRetryConnNextTime = (int)(UnityEngine.Time.deltaTime * 1000.0f) + 3000;
                }
            }
		}
	}
	
	public static bool IsServerCanConnect(string ip, int port){
		Socket tmpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		try {
			tmpSocket.Connect (ip, port);
			Debug.Log("ServerCanConnect :" + ip + " " + port);
			return true;
		} catch (Exception ex) {
			Debug.Log("ServerCan not Connect :" + ip + " " + port);
			return false;			
		}
	}


	//		private void ComfirnNetDisconnect(System.Object userInfo)
	//		{
	//			Game.Instance().ResetGame();
	//		}
}
//}

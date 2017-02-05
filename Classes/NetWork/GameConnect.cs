using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/*
网络连接控制器
@author up2know
@time 20160912
*/
public class GameConnect :NetClient
{
    public static string GAME_CONNECT_GOT_PACKET_KEY = "GOT_PACKET_KEY";//获得包秘钥
    public static string GAME_CONNECT_NET_CONNECT = "GAME_CONNECT_NET_CONNECT";		//网络连接成功
    public static string GAME_CONNECT_NET_DISCONNECT = "GAME_CONNECT_NET_DISCONNECT";//网络连接断开
    public static string GAME_CONNECT_SVR_CONNECT = "GAME_CONNECT_SVR_CONNECT";		//服务器连接成功
    public static string GAME_CONNECT_SVR_CONNECT_FAILED = "GAME_CONNECT_SVR_CONNECT_FAILED";//服务器连接失败
    public static string GAME_CONNECT_SVR_LOGIN_SUCC = "GAME_CONNECT_SVR_LOGIN_SUCC";//服务器连接失败
    public static string GAME_CONNECT_SVR_LOGIN_FAILD = "GAME_CONNECT_SVR_LOGIN_FAILD";//服务器连接失败
    public static string GAME_CONNECT_SVR_LOGIN_RET = "GAME_CONNECT_SVR_LOGIN_RET";//服务器连接失败


    static public int HEAD_SIZE = 4;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    private string m_ServerHost = "";                   //服务器的连接域名
    private int m_ServerPort = 0;                       //服务器连接端口号
    private string m_ServerIpHost = "";                 //服务器连接IP地址
	private ushort m_PacketKey = 0;                     //通讯加密KEY
    private bool m_boPacketKeyGotted = false;           //是否已取得通信验证KEY
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    public uint m_ServerId; //逻辑服务器ID
    public uint m_ConnServerId; //当前连接的服务器ID
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
	private uint  m_PacketIndex = 0;                     //数据包发送索引
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    private float m_NextPingTime = 0; //

    static private GameConnect m_Instance = null;
    static public GameConnect Instance
    {
        get {
            if (m_Instance == null)
                m_Instance = new GameConnect();
            return m_Instance; 
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    //private static EventController m_packetHandlerController = new EventController();
    ////注册系统数据包处理函数
    //public void registerPacketHandler(int SysId, Action<int,RecvDataPacket> handler)
    //{
    //    m_packetHandlerController.AddEventListener(SysId.ToString(), handler);
    //}
    ////取消系统数据包侧处理注册
    //public void unregisterPacketHandler(int SysId, Action<int,RecvDataPacket> handler)
    //{
    //    m_packetHandlerController.RemoveEventListener(SysId.ToString(), handler);
    //}
    //public void dispatchPacketHandler(int SysId,int cmd,RecvDataPacket data)
    //{
    //    m_packetHandlerController.TriggerEvent(SysId.ToString(), cmd,data);
    //}

    //static public void RegisterPacketHandler(int SysId, Action<int, RecvDataPacket> handler)
    //{
    //    GameConnect.Instance.registerPacketHandler(SysId, handler);
    //}
    ////取消系统数据包侧处理注册
    //static public void UnRegisterPacketHandler(int SysId, Action<int, RecvDataPacket> handler)
    //{
    //    GameConnect.Instance.unregisterPacketHandler(SysId, handler);
    //}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void SetUp()
    {
        Clear();
        Start();
        m_boPacketKeyGotted = false;
        m_PacketIndex = ((uint)UnityEngine.Random.Range(0, 0x0FFFFFFF)) & 0x000FFFFF;
        m_PacketIndex = ((m_PacketIndex & 0x0FFF0000) >> 12) | ((m_PacketIndex & 0x0000FFF0) << 12);
    }


    public void Destroy()
    {
        Stop();
        Clear();
        m_PacketIndex = 0;
        m_boPacketKeyGotted = false;
    }

    public void SetConnectSocket(Socket socket) 
    {

    }
    public void setServerAddress(string host, int port,string ipHost = "")
    {
        m_ServerHost = host;
        m_ServerPort = port;
        m_ServerIpHost = ipHost;
    }

    public void SetServerId(uint serverId)
    {
        m_ServerId = serverId;
    }
    public bool Connect(){
        StartConnect(m_ServerHost, m_ServerPort);
        return true;
    }

    public bool DisConnect()
    {
        Stop();
        return true;
    }

    protected override void OnConnect()
	{
        base.OnConnect();
        m_NextPingTime = 0;
        EventDispatcher.TriggerEvent(GAME_CONNECT_NET_CONNECT,1);
	}
	
	protected override void OnDisconnect(string e = "NULL")
    {
        base.OnDisconnect(e);
        m_boPacketKeyGotted = false;
        m_NextPingTime = 0;
        EventDispatcher.TriggerEvent(GAME_CONNECT_NET_DISCONNECT, e);
    }

    public void Tick(float elapse, int game_frame)
    {
        Update();
        if (IsConnected())
        {
            if (m_NextPingTime < UnityEngine.Time.time)
            {
                sendPing();
                m_NextPingTime = UnityEngine.Time.time + 15.0f;
            }
        }
    }



    private uint getPacketIndex()
    {
        int tv = (int)(TimeUtils.TimeSince1970 & 0x03);
        m_PacketIndex += (uint)IntUtils.max((long)1, (long)tv);
        return m_PacketIndex;
    }


    public override int onParseProtocol(int uId, byte[] buffer, int bufIdx)
    {
        int nPacketSize = 0;
        switch (uId)
        {
            case (int)EGate2CltProtocol.EG2C_Ping:
                nPacketSize += NetProtocolDefine.SG2CPing.Size();
                break;
            case (int)EGate2CltProtocol.EG2C_KeyInit:
                nPacketSize += NetProtocolDefine.SG2CKeyInit.Size();
                break;
            case (int)EGate2CltProtocol.EG2C_RetConnect:
                nPacketSize += NetProtocolDefine.SG2CRetConnect.Size();
                break;
            case (int)EGate2CltProtocol.EG2C_RetLogin:
                nPacketSize += NetProtocolDefine.SG2CRetLogin.Size();
                break;
            case (int)EGate2CltProtocol.EG2C_ServerData:
                nPacketSize += NetProtocolDefine.SG2CServerData.Size();
                if (m_RecvPacketLength - m_RecvDispatchIndex >= nPacketSize)
                {
                    int dataLength = (int)ByteUtils.Byte4ToUInt32(buffer[bufIdx], buffer[bufIdx+1], 0,0);
                    nPacketSize += dataLength;
                }
                break;
            default:
                nPacketSize = -1;
                Log.Warning("收到了网关下发的未知协议~?.");
                break;
        }

        return nPacketSize;
    }

    public override bool onDispatchProtocol(SGateProtocol protocol)
    {
        bool ret = false ;
        switch (protocol.m_uId)
        {
            case (int)EGate2CltProtocol.EG2C_Ping:
                //回应Ping
                ret = true;
                m_NextPingTime = UnityEngine.Time.time + (float)15.0;
                break;
            case (int)EGate2CltProtocol.EG2C_KeyInit:
                //初始化网关秘钥
                ret = onHandlerKeyInit(protocol.packet);
                break;
            case (int)EGate2CltProtocol.EG2C_RetConnect:
                //连接返回
                ret = onHandlerRetConnect(protocol.packet);
                break;
            case (int)EGate2CltProtocol.EG2C_RetLogin:
                //登录返回
                //ret = onHandlerRetLogin(protocol.packet);
                break;
            case (int)EGate2CltProtocol.EG2C_ServerData:
                //服务器数据
                ret = onHandlerGameData(protocol.packet);
                break;
        }

        return ret;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    private bool onHandlerKeyInit(RecvDataPacket data)
    {
        if (m_boPacketKeyGotted)
        {
            Log.Error("多次收到了网关下发的秘钥消息");
            return false;
        }
        byte[] keyBuffer = data.ReadArray<byte>(128);
        int nIndex = 0;
        byte i1, i2;
        i1 = keyBuffer[0];
        i2 = keyBuffer[1];
        nIndex += 2;
        m_PacketKey = (ushort)(keyBuffer[nIndex + i1] | (keyBuffer[nIndex + i2] << 8));
        m_boPacketKeyGotted = true;
        m_PacketIndex = 0;
        Log.Debug("GOT PACKET KEY " + m_PacketKey.ToString() + IntUtils.ToHexString(m_PacketKey));

        EventDispatcher.TriggerEvent(GAME_CONNECT_GOT_PACKET_KEY);
        return true;
    }

    private bool onHandlerRetConnect(RecvDataPacket data)
    {
        NetProtocolDefine.SG2CRetConnect ret = new NetProtocolDefine.SG2CRetConnect();
        ret.uServerId = data.Read<uint>();
        ret.uRetCode = data.Read<byte>();
        m_ConnServerId = 0;
        if (ret.uRetCode == 0)
        {
            m_ConnServerId = ret.uServerId;
            //派发事件
            EventDispatcher.TriggerEvent(GAME_CONNECT_SVR_CONNECT, ret.uServerId);
        }
        else if (ret.uRetCode == 101)
	    {
            Log.Error("网关告知该客户已连接上了服务器了,属于多次连接");
	    }
	    else if (ret.uRetCode == 102)
	    {
            Log.Error("服务器未开启或维护中");
            //派发事件
            EventDispatcher.TriggerEvent(GAME_CONNECT_SVR_CONNECT_FAILED, ret.uRetCode);
	    }
        return true;
    }
    private bool onHandlerRetLogin(RecvDataPacket data)
    {
//         uint nErrorCode = data.Read<uint>();
//         m_nLogicServerId = data.Read<uint>();
//         string sAccount = new string(data.ReadCharArray(64));
//         string sToken = new string(data.ReadCharArray(64));
//         Log.OutPut(eLogLevel.DEBUG, "账号验证[{0}],服务器ID:[{1}]", sAccount, m_nLogicServerId);
        return true;
    }
    static private byte[] gDefaultRecvGateDataBuffer = new byte[4096];
    private bool onHandlerGameData(RecvDataPacket data)
    {
        byte[] recvData = data.getData(); 

        NetProtocolDefine.SG2CServerData srvData = new NetProtocolDefine.SG2CServerData();
        ushort uDataLength = data.Read<ushort>();
        uint uRandomKey = data.Read<uint>();
        uint uKey = NetProtocol.decrpytGateKey(uRandomKey, uDataLength);
        srvData.buffer = NetProtocol.decryptGatePacket(ref recvData, data.Position, uDataLength, uKey);
        byte sysId = data.Read<byte>();
        byte cmdId = data.Read<byte>();
        if (GlobalID.IsLogNet)
        {
            if (!((sysId == 3 && cmdId == 100) || (sysId == 3 && cmdId == 2) || (sysId == 3 && cmdId == 105) || (sysId == 3 && cmdId == 106)))
                Log.Debug("[recv] sysId:" + sysId + " cmdId:" + cmdId);
        }
        //byte[] packetInfo = new byte[data.getAvaliableLength()];
        //Array.Copy(data.getData(), data.Position, packetInfo, 0, data.getAvaliableLength());
        //派发网络消息
        //dispatchPacketHandler((int)sysId, (int)cmdId, new RecvDataPacket(recvData, data.Position, data.getAvaliableLength()));
        return true;
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    static public SendDataPacket gSendDataPacket = new SendDataPacket();
    static private NetProtocolDefine.SC2GSendData gDefaultSendData = new NetProtocolDefine.SC2GSendData();
    static private byte[] gDefaultSendBuffer = new byte[4096];
    static private int gDefaultGamePacketPosition = 0xF; //位置刚好15
    static private SendDataPacket allocDataPacket(EClt2GateProtocol uId)
    {
        gSendDataPacket.Clear();
        gSendDataPacket.Write<byte>((byte)uId);
        gSendDataPacket.Write<uint>((uint)NetProtocol.NetPacketHeaderIdent);
        return gSendDataPacket;
    }
    static public SendDataPacket allocGamePacket(int sysId,int cmdId)
    {
        SendDataPacket tempDataPacket = GameConnect.gSendDataPacket;
        tempDataPacket.Clear();
        tempDataPacket.Write<byte>((byte)EClt2GateProtocol.EC2G_SendData);
        tempDataPacket.Write<uint>((uint)NetProtocol.NetPacketHeaderIdent);
        tempDataPacket.Write<uint>(GameConnect.gDefaultSendData.uPacketIndex);
        tempDataPacket.Write<ushort>(GameConnect.gDefaultSendData.uServerId);
        tempDataPacket.Write<ushort>(GameConnect.gDefaultSendData.uPacketVerify);
        tempDataPacket.Write<ushort>(GameConnect.gDefaultSendData.uDataLength);

        tempDataPacket.Write<byte>((byte)sysId);
        tempDataPacket.Write<byte>((byte)cmdId);
        if (GlobalID.IsLogNet)
        {
            if (!((sysId == 3 && cmdId == 100) || (sysId == 3 && cmdId == 105) || (sysId == 3 && cmdId == 2) || (sysId == 3 && cmdId == 3) || (sysId == 3 && cmdId == 106)))
                Log.Debug("[send] sysId:" + sysId + " cmdId:" + cmdId);
        }
        return tempDataPacket;
    }
    static public void flushGamePacket(SendDataPacket data, int serverId = -1)
    {
        if (serverId == -1)
            serverId = (int)GameConnect.Instance.m_ServerId;
        GameConnect.Instance.flushPacket(data,serverId);
    }
    public void flushPacket(SendDataPacket data,int serverId = 0 )
    {
        byte[] buff = data.getData();
        if ((int)buff[0] == (int)EClt2GateProtocol.EC2G_SendData)
        {
            data.Position = gDefaultGamePacketPosition; //设置位置到实际内容区域
            int packetIndex = (int)getPacketIndex();
            NetProtocolDefine.SC2GSendData sendData = gDefaultSendData;
            sendData.uServerId = (ushort)serverId;
            sendData.uDataLength = (ushort)data.getAvaliableLength(); //获得实际内容的大小
            //将实际内容转移到临时内存中进行加密操作
            sendData.uPacketVerify = NetProtocol.calcPacketDataVerify(buff, data.Position, sendData.uDataLength, m_PacketKey);
            NetProtocol.encryptClientPacket(ref data.m_PackData, data.Position, sendData.uDataLength, (uint)packetIndex);
            sendData.uPacketIndex = NetProtocol.encrpytPacketIndex((uint)packetIndex, (ushort)m_PacketKey, (uint)sendData.uDataLength);

            data.Position = 5; //偏移位置到包的头部位置
            data.Write<uint>(sendData.uPacketIndex);
            data.Write<ushort>(sendData.uServerId);
            data.Write<ushort>(sendData.uPacketVerify);
            data.Write<ushort>(sendData.uDataLength);
        }
        SendData(data.getData(), 0, data.Length);
    }
    public void sendPing()
    {
        SendDataPacket pack = allocDataPacket(EClt2GateProtocol.EC2G_Ping);
        flushPacket(pack);
    }

    public void sendConnectToServer(int nServerId)
    {
        SendDataPacket pack = allocDataPacket(EClt2GateProtocol.EC2G_ConnectTo);
        pack.WriteUInt((uint)nServerId);
        flushPacket(pack);
    }
    public void sendUserLogin(string sAccount,int serverId,string sToken,ulong uSpId)
    {
        SendDataPacket pack = allocDataPacket(EClt2GateProtocol.EC2G_Login);
        pack.WriteCharArray(sAccount,64);
        pack.WriteCharArray(sToken,64);
        pack.WriteInt(serverId);
        pack.WriteULong(uSpId);
        byte[] temp =new byte[256];
        pack.WriteBuffer(temp,0,256);
        flushPacket(pack);
    }
}
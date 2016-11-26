using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
struct ClientNetPacketHeader 
{
    public uint dwIdent;//数据包包头标志，固定为NetPacketHeaderIdent
    public ushort wPacketSize;//数据包长度（不含此包头）
    public ushort wVerification;//数据效验值，仅客户端向服务器发送的数据包包头中包含对数据段的效验。
    public uint dwPacketIndex;//累积数据包索引，客户端每次发送数据包后必须增加此值，服务器检测到此值不比上一次的值大则做丢弃
};
//网关到客户端消息协议
enum EGate2CltProtocol
{
    EG2C_Ping = 0,
    EG2C_KeyInit,				//密钥数据。
    EG2C_RetConnect,			//连接返回， 0 -OK，
    EG2C_RetLogin,				//登录返回
    EG2C_ServerData,			//服务器返回的数据。 理所当然是加密的。
    EG2C_ProtocolCount
};
//客户端到网关消息协议
enum EClt2GateProtocol
{
    EC2G_Ping = 0,
    EC2G_Login,		//登录请求
    EC2G_ConnectTo,  //连接到X服务器
    EC2G_SendData,	 //发送数据到任意服务器。
    EC2G_ProtocolCount
};

class NetProtocolDefine
{

    static public SG2CPing gProtocolPing = new SG2CPing();
    static public SG2CKeyInit gProtocolKeyInit = new SG2CKeyInit();
    static public SG2CRetConnect gProtocolRetConn = new SG2CRetConnect();
    static public SG2CRetLogin gProtocolRetLogin = new SG2CRetLogin();
    static public SG2CServerData gProtocolSvrData = new SG2CServerData();



    /***********************************************
	 * 网关到客户端协议
	 **********************************************/
    public struct SG2CPing 
    {
        uint uServerTime; //服务器时间
        static public int Size() { return 4; }
    }
    public struct SG2CKeyInit 
    {
        public byte[] aryKeyBuff ;
        static public int Size() { return 128; }
    }
    public struct SG2CRetConnect 
    {
  		public uint uServerId;   //服务器ID
		public byte uRetCode;    //连接结果
        static public int Size() { return 4 + 1; }
    }
    public struct SG2CRetLogin 
    {
		public uint uErrorCode;
		public byte[] sAccount; //64字节大小
        public byte[] sToken ; //64字节大小
        public uint nServerId;
        static public int Size() { return 4 + 64 + 64 + 4; }
    }

    public struct SG2CServerData 
    {
        public ushort uDataLength;
		public uint uRandomKey ;
        public byte[] buffer ;
        static public int Size() { return 2 + 4; }
        public uint Length() { return  uDataLength; }
    }


    /***********************************************
     * 客户端到网关协议
     **********************************************/

    public struct SC2GPing
    {
        static public int Size() { return 0; }
    }
    public struct SC2GConnectTo
    {
        public int uServerId ;
        static public int Size() { return 4; }
    }
    public struct SC2GLogin 
    {
        public int uServerId ;
        public byte[] sAccount ; //本地账号信息
        public byte[] sToken ; //本地Token
        public ulong uSpID; //渠道平台标示
        //public byte temp[256]; //预留256字节提供后期扩展
        static public int Size() { return 4 + 64 + 64 +8 +256; }
    }
    public struct SC2GSendData 
    {
		public uint uPacketIndex ;
        public ushort uServerId ;
        public ushort uPacketVerify ;
        public ushort uDataLength ;
        public byte[] buffer ;
        static public int Size() { return 4 + 2 + 2 + 2; }
        public uint Length() { return uDataLength; }
   }

}
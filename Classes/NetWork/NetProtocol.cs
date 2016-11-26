using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public struct SGateProtocol
{
    public byte m_uId;
    public uint m_uHeader;
    public byte[] buffers;
    public RecvDataPacket packet;
};
public class NetProtocol
{

    //0xAA8899BB
    static public ulong NetPacketHeaderIdent = 0xAA8899BB;
    static public ulong ClientNetPacketHeaderIdent = 0xAA8899BC;

    //static public ulong NetPacketHeaderSize = sizeof(NetPacketHeader);
    /***********************************************
	 * 计算数据包内容效验值函数
	 **********************************************/
	public static ushort calcPacketDataVerify(byte[] buf,int idx, int dwSize, ushort uKey)
	{
        ushort ret = 0x9BCE;
        byte bu = (byte)(ret & 0xFF);
        byte bv = (byte)((ret >> 8) & 0xFF);
        int index = idx;
        while (dwSize > 0)
        {
            bu ^= buf[index];
            bv ^= bu;
            dwSize--;
            index++;
        }
        ret = (ushort)((ushort)bu << 0);
        ret |= (ushort)((ushort)bv << 8);
        ret ^= uKey;
        ret ^= (ushort)~(dwSize);
        return ret;
	}

    /***********************************************
	 * 数据包内容加密函数
	 **********************************************/
    public static byte[] encryptClientPacket(ref byte[] buf,int index, uint dwSize, uint uKey)
	{
        //System.Convert.ToInt32(str, 16);
        int idx = index;
        uint pv = (uint)((uKey ^ (~((uint)dwSize))) ^ 0xE162C51B);
        while (dwSize > 4)
        {
            uint udw = ByteUtils.Byte4ToUInt32(buf[idx + 0], buf[idx + 1], buf[idx + 2], buf[idx + 3]);
            pv = udw ^ (uKey ^ pv);
            ByteUtils.UInt32ToByte4(pv, out buf[idx + 0], out buf[idx + 1], out buf[idx + 2], out buf[idx + 3]);
            dwSize -= 4;
            idx += 4;
        }
		while (dwSize > 0)
		{
			buf[idx] ^= (byte)(uKey ^ 0xA6);
			dwSize--;
            idx++;
		}
        return buf;
	}

    /***********************************************
	 * 客户端数据包索引加密
	 **********************************************/
    public static uint encrpytPacketIndex(uint uPackIndex, ushort uKey, uint uLen)
	{
		uPackIndex = uPackIndex ^ (uint)(((~uLen) << 16) | uKey);
	    byte sd0,sd1,sd2,sd3  = 0;
        ByteUtils.UInt32ToByte4(uPackIndex, out sd0, out sd1, out sd2, out sd3);
		sd3 ^= sd0;
		sd2 ^= sd0;
		sd1 ^= sd0;
        uPackIndex = ByteUtils.Byte4ToUInt32(sd0,sd1,sd2,sd3);
		return uPackIndex;
	}

    public static uint decrpytGateKey(uint uRandomKey, uint uLen)
	{
        byte sd0,sd1,sd2,sd3  = 0;
        ByteUtils.UInt32ToByte4(uRandomKey, out sd0, out sd1, out sd2, out sd3);
        sd3 ^= sd0;
        sd2 ^= sd0;
		sd1 ^= sd0;
        uRandomKey = ByteUtils.Byte4ToUInt32(sd0, sd1, sd2, sd3);
        uRandomKey = uRandomKey ^ (uint)(((~uLen) << 16) | uLen);
		return uRandomKey;
	}
    /***********************************************
    * 数据包内容解密函数
    **********************************************/
    public static byte[] easyDecryptClientPacket(ref byte[] buf,int index, uint dwSize, uint uKey)
	{
		uint nv = (uint)(uKey ^ 0xA19483F4);
		byte cv = (byte)(uKey ^ 0x8B);
        int idx = index;
		while (dwSize > 4)
		{
            uint udw = ByteUtils.Byte4ToUInt32(buf[idx + 0], buf[idx + 1], buf[idx + 2], buf[idx + 3]);
			udw ^= nv;
            ByteUtils.UInt32ToByte4(udw, out buf[idx + 0], out buf[idx + 1], out buf[idx + 2], out buf[idx + 3]);
			dwSize -= 4;
			idx += 4;
		}
		while (dwSize > 0)
		{
			buf[idx] ^= cv;
			idx++;
			dwSize--;
		}
        return buf;
	}
    public static byte[] decryptGatePacket(ref byte[] buf,int index, uint dwSize, uint uKey)
	{
        uint idx = (uint)index;
		while (dwSize > 0)
		{
            buf[idx] ^= (byte)(uKey ^ 0xBA);
            idx++;
			dwSize--;
		}
        return buf;
	}
}
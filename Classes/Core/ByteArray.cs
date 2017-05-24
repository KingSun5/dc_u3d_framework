using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 字节数组
/// @author hannibal
/// @time 2017-5-23
/// </summary>
public class ByteArray
{
    protected byte[] m_Buffer;

    protected uint m_BufferLen;
    protected uint m_MaxBufferLen;

    protected uint m_Head;
    protected uint m_Tail;

    public ByteArray()
    {
        m_BufferLen = 0;
        m_MaxBufferLen = 0;
        m_Head = 0;
        m_Tail = 0;
    }

    public void Init(uint BufferSize, uint MaxBufferSize)
    {
	    m_Head = 0;
	    m_Tail = 0;

	    m_BufferLen = BufferSize;
	    m_MaxBufferLen = MaxBufferSize;

	    m_Buffer = new byte[m_BufferLen];
    }

    public void Initsize()
    {
	    m_Head = 0;
	    m_Tail = 0;

	    m_Buffer = new byte[m_BufferLen];
    }
    public bool Resize(uint size)
    {
	    size = (uint)Mathf.Max(size, (int)(m_BufferLen >> 1));
	    uint newBufferLen = m_BufferLen + size;
        if (newBufferLen > m_MaxBufferLen)
        {
            Log.Warning("ByteArray::Resize 缓冲区溢出:" + newBufferLen);
            newBufferLen = m_MaxBufferLen;
        }

	    uint len = Available();
	    if (size < 0)
	    {
		    if (newBufferLen < len)return false;
	    }

	    byte[] newBuffer = new byte[newBufferLen];
	    if (m_Head<m_Tail)
        {
            Array.Copy(m_Buffer, m_Head, newBuffer, 0, m_Tail - m_Head);
            //memcpy(newBuffer, &m_Buffer[m_Head], m_Tail - m_Head);
	    }
	    else if (m_Head>m_Tail)
        {
            Array.Copy(m_Buffer, m_Head, newBuffer, 0, m_BufferLen - m_Head);
            Array.Copy(m_Buffer, 0, newBuffer, m_BufferLen - m_Head, m_Tail);
		    //memcpy(newBuffer, &m_Buffer[m_Head], m_BufferLen - m_Head);
		    //memcpy(&newBuffer[m_BufferLen - m_Head], m_Buffer, m_Tail);
	    }

	    m_Buffer = newBuffer;
	    m_BufferLen = newBufferLen;
	    m_Head = 0;
	    m_Tail = len;

	    return true;
    }
    public void Clear()
    {
	    m_Head = 0;
	    m_Tail = 0;
    }

    private byte[] val = new byte[8];
    public byte ReadByte()
    {
	    Read(ref val, 1);
	    return val[0];
    }
    public short ReadShort()
    {
        Read(ref val, 2);
        return BitConverter.ToInt16(val, 0);
    }
    public ushort ReadUShort()
    {
        Read(ref val, 2);
        return BitConverter.ToUInt16(val, 0);
    }
    public int ReadInt()
    {
        Read(ref val, 4);
        return BitConverter.ToInt32(val, 0);
    }
    public uint ReadUint()
    {
        Read(ref val, 4);
        return BitConverter.ToUInt32(val, 0);
    }
    public long ReadLong()
    {
        Read(ref val, 8);
        return BitConverter.ToInt64(val, 0);
    }
    public ulong ReadUlong()
    {
        Read(ref val, 8);
        return BitConverter.ToUInt64(val, 0);
    }
    public float ReadFloat()
    {
        Read(ref val, 4);
        return BitConverter.ToSingle(val, 0);
    }
    public double ReadDouble()
    {
        Read(ref val, 8);
        return BitConverter.ToDouble(val, 0);
    }
    public string ReadString()
    {
        ushort length = 0;
        length = ReadUShort();
        string val = String.Empty;
        byte[] by = new byte[length];
        Read(ref by, length);
        do
        {
            val = Encoding.UTF8.GetString(by, 0, length);
        } while (false);

        return val;
    }
    //返回0表示没有读到数据
    public uint Read(ref byte[] buf, uint len)
    {
	    if (len == 0)
		    return 0;

	    if (len > Available())
		    return 0;

	    if (m_Head < m_Tail)
        {
            Array.Copy(m_Buffer, m_Head, buf, 0, len);
		    //memcpy(buf, &m_Buffer[m_Head], len);
	    }
	    else
	    {
		    uint rightLen = m_BufferLen - m_Head;
		    if (len <= rightLen)
            {
                Array.Copy(m_Buffer, m_Head, buf, 0, len);
			    //memcpy(buf, &m_Buffer[m_Head], len);
		    }
		    else
            {
                Array.Copy(m_Buffer, m_Head, buf, 0, rightLen);
                Array.Copy(m_Buffer, 0, buf, rightLen, len - rightLen);
			    //memcpy(buf, &m_Buffer[m_Head], rightLen);
			    //memcpy(&buf[rightLen], m_Buffer, len - rightLen);
		    }
	    }

	    m_Head = (m_Head + len) % m_BufferLen;

	    return len;
    }

    public uint WriteByte(byte buf)
    {
        return Write(BitConverter.GetBytes(buf), 1);
    }
    public uint WriteShort(short buf)
    {
        return Write(BitConverter.GetBytes(buf), sizeof(short));
    }
    public uint WriteUShort(ushort buf)
    {
        return Write(BitConverter.GetBytes(buf), sizeof(ushort));
    }
    public uint WriteInt(int buf)
    {
        return Write(BitConverter.GetBytes(buf), sizeof(int));
    }
    public uint WriteUInt(uint buf)
    {
        return Write(BitConverter.GetBytes(buf), sizeof(uint));
    }
    public uint WriteLong(long buf)
    {
        return Write(BitConverter.GetBytes(buf), sizeof(long));
    }
    public uint WriteULong(ulong buf)
    {
        return Write(BitConverter.GetBytes(buf), sizeof(ulong));
    }
    public uint WriteFloat(float buf)
    {
        return Write(BitConverter.GetBytes(buf), sizeof(float));
    }
    public uint WriteDouble(double buf)
    {
        return Write(BitConverter.GetBytes(buf), sizeof(double));
    }
    public uint WriteBytes(byte[] buf, uint len)
    {
        return Write(buf, len);
    }
    public void WriteString(string val)
    {
        if (val == null)
        {
            WriteUShort(0);
            WriteByte(((byte)'\0'));
        }
        else
        {
            byte[] bytes = Encoding.UTF8.GetBytes(val);
            ushort length = (ushort)bytes.Length;
            WriteUShort(length);
            WriteBytes(bytes, length);
        }
    }
    public uint Write( byte[] buf, uint len)
    {
	    //					//
	    //     T  H			//    H   T			LEN=10
	    // 0123456789		// 0123456789
	    // abcd...efg		// ...abcd...
	    //					//
	    uint nFree = ((m_Head <= m_Tail) ? (m_BufferLen - m_Tail + m_Head - 1) : (m_Head - m_Tail - 1));
	    if (len >= nFree)
	    {
		    if (!Resize(len - nFree + 1))
			    return 0;
	    }

	    if (m_Head <= m_Tail)
	    {
		    if (m_Head == 0)
		    {
                nFree = m_BufferLen - m_Tail - 1;
                Array.Copy(buf, 0, m_Buffer, m_Tail, len);
			    //memcpy(&m_Buffer[m_Tail], buf, len);
		    }
		    else
		    {
			    nFree = m_BufferLen - m_Tail;
			    if (len <= nFree)
                {
                    Array.Copy(buf, 0, m_Buffer, m_Tail, len);
				    //memcpy(&m_Buffer[m_Tail], buf, len);
			    }
			    else
                {
                    Array.Copy(buf, 0, m_Buffer, m_Tail, nFree);
                    Array.Copy(buf, nFree, m_Buffer, 0, len - nFree);
				    //memcpy(&m_Buffer[m_Tail], buf, nFree);
				    //memcpy(m_Buffer, &buf[nFree], len - nFree);
			    }
		    }
	    }
	    else
        {
            Array.Copy(buf, 0, m_Buffer, m_Tail, len);
		    //memcpy(&m_Buffer[m_Tail], buf, len);
	    }

	    m_Tail = (m_Tail + len) % m_BufferLen;

	    return len;
    }

    public bool Peek(ref byte[] buf, uint len)
    {
	    if (len == 0)
		    return false;

	    if (len > Available())
		    return false;

	    if (m_Head < m_Tail)
        {
            Array.Copy(m_Buffer, m_Head, buf, 0, len);
		    //memcpy(buf, &m_Buffer[m_Head], len);
	    }
	    else
	    {
		    uint rightLen = m_BufferLen - m_Head;
		    if (len <= rightLen)
            {
                Array.Copy(m_Buffer, m_Head, buf, 0, len);
			    //memcpy(&buf[0], &m_Buffer[m_Head], len);
		    }
		    else
            {
                Array.Copy(m_Buffer, m_Head, buf, 0, rightLen);
                Array.Copy(m_Buffer, 0, buf, rightLen, len - rightLen);
			    //memcpy(&buf[0], &m_Buffer[m_Head], rightLen);
			    //memcpy(&buf[rightLen], &m_Buffer[0], len - rightLen);
		    }
	    }

	    return true;
    }

    public bool Skip(uint len)
    {
	    if (len == 0)
		    return false;

	    if (len > Available())
		    return false;

	    m_Head = (m_Head + len) % m_BufferLen;

	    return true;
    }

    public uint Available()
    {
	    if (m_Head<m_Tail)
		    return m_Tail - m_Head;
	    else if (m_Head>m_Tail)
		    return m_BufferLen - m_Head + m_Tail;

	    return 0;
    }
}

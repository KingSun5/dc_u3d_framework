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
public struct PacketBase
{
    public ushort header;
    public RecvDataPacket data;
}

public class DataPacket
{
    static public byte[] gTempReadBuffer = new byte[4096];
    public byte[] m_PackData = null;
    protected int m_Position = 0;
    protected int m_Length = 0; //真实长度


    public byte[] getData()
    {
        return m_PackData;
    }

    public void Clear()
    {
        if (m_Length > 0)
            Array.Clear(m_PackData,0,m_Length);
        m_Length = 0;
        m_Position = 0;
    }

    public void Reverse(int count)
    {
        if (count > m_PackData.Length)
        {
            Array.Resize(ref m_PackData, (int)IntUtils.upper_power_of_two(count));
        }
    }

    public int Position { 
        get { return m_Position; }
        set { m_Position = value >= m_Length ? m_Length : value;} 
    }
    public int Length
    {
        get { return m_Length; }
        set {
            if (value > m_PackData.Length) Array.Resize(ref m_PackData, (int)IntUtils.upper_power_of_two(value));
            if (m_Length < m_Position) m_Position = m_Length;
            m_Length = value;
        }
    }
    public int getAvaliableLength() 
	{
        return m_Length - m_Position;
	}

}

public class SendDataPacket : DataPacket
{
    public SendDataPacket() 
    {
        m_PackData = new byte[8]; //默认先给个空间大小
        m_Length = 0;
        m_Position = 0;
    }
    //by hannibal:数据量比较的时候，执行效率比较低，尽量少用
    public void Write<T>(T val)
    {
        int nSize = Marshal.SizeOf(((System.Object)val).GetType());
        Reverse(m_Position + nSize);
        try
        {
            byte[] tempBuffer = ByteUtils.StructToBytes<T>(val);
            //tempBuffer.CopyTo(m_PackData,m_Position);
            Array.Copy(tempBuffer, 0, m_PackData, m_Position, nSize);
        }
        catch (Exception ex)
        {
            Log.Exception(ex.Message);
        }
        m_Position += nSize;
        if (m_Position > m_Length)
            m_Length = m_Position;
    }

    public void WriteBuffer(byte[] array, int index, int count )
    {
        if (count == 0) count = array.Length;
        Reverse(m_Position + count);
        int length = count;
        if (index + length > array.Length)
            length = array.Length - index;
        Array.Copy(array, index, m_PackData, m_Position, length);
        m_Position += count;
        if (m_Position > m_Length)
            m_Length = m_Position;
    }

    public void WriteArray<T>(T[] array,int index ,int count )
    {
        int nSize = Marshal.SizeOf(((System.Object)array[0]).GetType());
        count = (count <= 0) ? array.Length : count;
        int maxIndex = index + count;
        T obj = default(T);
        while (index < maxIndex)
        {
            if (index >= array.Length)
                Write<T>(obj);
            else Write<T>(array[index]);
            index++;
        }
    }

    public void WriteCharArray(string val, int count)
    {
        Reverse(m_Position + count);
        if (val == null)
        {
            m_PackData[m_Position ] = System.Convert.ToByte('\0');
        }
        else
        {
            byte[] bytes = Encoding.UTF8.GetBytes(val);
            int length = (count - 1) > bytes.Length ? bytes.Length : (count - 1);
            Array.Copy(bytes, 0, m_PackData, m_Position, length);
            m_PackData[m_Position + count] = System.Convert.ToByte('\0');
        }
        Offset(count);
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
            Reverse(m_Position + length);
            Array.Copy(bytes, 0, m_PackData, m_Position, length);
            m_Position += length;
            WriteByte(((byte)'\0'));
        }
    }

    public void Offset(int num)
    {
        m_Position += num;
        if (m_Position > m_Length)
            m_Length = m_Position;
    }

    public void WriteFloat(float val) { WriteBuffer(System.BitConverter.GetBytes(val), 0,4); }
    public void WriteDouble(double val) { WriteBuffer(System.BitConverter.GetBytes(val), 0,8); }
    public void WriteByte(byte val) { WriteBuffer(System.BitConverter.GetBytes(val), 0, 1); }
    public void WriteSByte(sbyte val) { WriteBuffer(System.BitConverter.GetBytes(val), 0,1); }
    public void WriteChar(char val) { WriteBuffer(System.BitConverter.GetBytes(val), 0, 1); }
    public void WriteShort(short val) { WriteBuffer(System.BitConverter.GetBytes(val), 0,2); }
    public void WriteUShort(ushort val) { WriteBuffer(System.BitConverter.GetBytes(val), 0,2); }
    public void WriteInt(int val) { WriteBuffer(System.BitConverter.GetBytes(val), 0,4); }
    public void WriteUInt(uint val) { WriteBuffer(System.BitConverter.GetBytes(val), 0,4); }
    public void WriteLong(long val) { WriteBuffer(System.BitConverter.GetBytes(val), 0,8); }
    public void WriteULong(ulong val) { WriteBuffer(System.BitConverter.GetBytes(val), 0,8); }
};


public class RecvDataPacket : DataPacket
{
    public RecvDataPacket(byte[] buffer, int index, int count) 
    {
        m_PackData = new byte[count];
        m_Length = count;
        m_Position = 0;
        Array.Copy(buffer,index,m_PackData,0,count);
    }

    public bool CheckSize(int count)
    {
        if (m_Position + count > m_Length)
            return false;
        return true;
    }

    public T Read<T>(int size )
    {
        T val = default(T);
        int nSize = size;
        if (nSize == 0)
            nSize = Marshal.SizeOf(((System.Object)val).GetType());
        
        if (!CheckSize(nSize))
        {
            m_Position = m_Length;
            return val;
        }

        Array.Copy(m_PackData, m_Position, gTempReadBuffer, 0, nSize);
        try
        {
            val = ByteUtils.BytesToStruct<T>(gTempReadBuffer, ((System.Object)val).GetType());
        }
        catch (Exception ex)
        {
            Log.Exception(ex.Message);
        }
        m_Position += nSize;
        return val;
    }

    public T Read<T>()
    {
        T val = default(T);

        int nSize =Marshal.SizeOf(((System.Object)val).GetType());
        if (!CheckSize(nSize))
        {
            m_Position = m_Length;
            return val;
        }
        
        Array.Copy(m_PackData, m_Position, gTempReadBuffer, 0, nSize);
        try
        {
            val = ByteUtils.BytesToStruct<T>(gTempReadBuffer, ((System.Object)val).GetType());
        }
        catch (Exception ex)
        {
            Log.Exception(ex.Message);
        }
        m_Position += nSize;
        return val;
    }

    public T[] ReadArray<T>(int count)
    {
        T obj = default(T);
        T[] array = new T[count];

        int nSize = Marshal.SizeOf(((System.Object)obj).GetType());
        if (!CheckSize(nSize * count))
        {
            m_Position = m_Length;
            return array;
        }

        int nIndex =0;
        while (nIndex < count)
        {
            array[nIndex] = default(T);
            array[nIndex] = Read<T>();
            nIndex++;
        }
        return array;
    }


    public char[] ReadCharArray(int count)
    {
        string val = "";
        if (!CheckSize(count))
        {
            m_Position = m_Length;
            return val.ToCharArray();
        }
        byte[] tempBuffer = new byte[count];
        Array.Copy(m_PackData, m_Position, tempBuffer,0,count);
        val = Encoding.UTF8.GetString(tempBuffer);
        m_Position += count;
        return val.ToCharArray();
    }

    public void Offset(int num)
    {
        m_Position += num;
    }


    public string ReadString()
    {
        ushort length = 0;
        length = ReadUShort();
        string val = String.Empty;
        do
        {
            if (length <= 0 || !CheckSize(length + 1))
                break;
            val = Encoding.UTF8.GetString(m_PackData, m_Position, length);
        } while (false);

        m_Position += length + 1; //跳过末尾/0
        return val;
    }

    public float ReadFloat() { if (!CheckSize(4)) return 0.0f; float val = BitConverter.ToSingle(m_PackData, m_Position); Offset(4); return val; }
    public double ReadDouble() { if (!CheckSize(8)) return 0; double val = BitConverter.ToDouble(m_PackData, m_Position); Offset(8); return val; }
    public byte ReadByte() { if (!CheckSize(1)) return 0; byte val = m_PackData[m_Position]; Offset(1); return val; }
    public sbyte ReadSByte() { if (!CheckSize(1)) return 0; sbyte val = (sbyte)m_PackData[m_Position]; Offset(1); return val; }
    public char ReadChar() { if (!CheckSize(1)) return '0'; char val = (char)m_PackData[m_Position]; Offset(1); return val; }
    public short ReadShort() { if (!CheckSize(2)) return 0; short val = BitConverter.ToInt16(m_PackData, m_Position); Offset(2); return val; }
    public ushort ReadUShort() { if (!CheckSize(2)) return 0; ushort val = BitConverter.ToUInt16(m_PackData, m_Position); Offset(2); return val; }
    public int ReadInt() { if (!CheckSize(4)) return 0; int val = BitConverter.ToInt32(m_PackData, m_Position); Offset(4); return val; }
    public uint ReadUInt() { if (!CheckSize(4)) return 0; uint val = BitConverter.ToUInt32(m_PackData, m_Position); Offset(4); return val; }
    public long ReadLong() { if (!CheckSize(8)) return 0; long val = BitConverter.ToInt64(m_PackData, m_Position); Offset(8); return val; }
    public ulong ReadULong() { if (!CheckSize(8)) return 0; ulong val = BitConverter.ToUInt64(m_PackData, m_Position); Offset(8); return val; }

};

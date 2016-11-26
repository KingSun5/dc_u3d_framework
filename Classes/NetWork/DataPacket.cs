using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System;


public  class DataPacket
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

    public void WriteBuffer(byte[] array, int index, int count)
    {
        Reverse(m_Position + count);
        Array.Copy(array, index, m_PackData, m_Position, count);
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
            byte[] bytes = Encoding.Default.GetBytes(val);
            int length = (count - 1) > bytes.Length ? bytes.Length : (count - 1);
            Array.Copy(bytes, 0, m_PackData, m_Position, length);
            m_PackData[m_Position + count] = System.Convert.ToByte('\0');
        }
        m_Position += count;
        if (m_Position > m_Length)
            m_Length = m_Position;
    }
    public void WriteString(string val)
    {
        if (val == null)
        {
            Write<ushort>(0);
            Write<byte>(((byte)'\0'));
        }
        else
        {
            byte[] bytes = Encoding.Default.GetBytes(val);
            ushort length = (ushort)bytes.Length;
            Write<ushort>(length);
            Reverse(m_Position + length);
            Array.Copy(bytes, 0, m_PackData, m_Position, length);
            m_Position += length;
            Write<byte>(((byte)'\0'));
        }
    }

    public void WriteFloat(float val) {  Write<float>(val); }
    public void WriteDecimal(decimal val) {  Write<decimal>(val); }
    public void WriteDouble(double val) {  Write<double>(val); }
    public void WriteByte(byte val) {  Write<byte>(val); }
    public void WriteSByte(sbyte val) {  Write<sbyte>(val); }
    public void WriteChar(char val) {  Write<char>(val); }
    public void WriteShort(short val) {  Write<short>(val); }
    public void WriteUShort(ushort val) {  Write<ushort>(val); }
    public void WriteInt(int val) {  Write<int>(val); }
    public void WriteUInt(uint val) {  Write<uint>(val); }
    public void WriteLong(long val) {  Write<long>(val); }
    public void WriteULong(ulong val) {  Write<ulong>(val); }
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
        val = Encoding.Default.GetString(tempBuffer);
        m_Position += count;
        return val.ToCharArray();
    }


    public string ReadString()
    {
        ushort length = 0;
        length = Read<ushort>();
        string val = Encoding.Default.GetString(m_PackData,m_Position,length);
        m_Position += length + 1; //跳过末尾/0
        return val;
    }

    public float ReadFloat() { return Read<float>();}
    public decimal ReadDecimal() { return Read<decimal>();}
    public double ReadDouble() { return Read<double>();}
    public byte ReadByte() { return Read<byte>();}
    public sbyte ReadSByte() { return Read<sbyte>();}
    public char ReadChar() { return Read<char>();}
    public short ReadShort() { return Read<short>();}
    public ushort ReadUShort() { return Read<ushort>();}
    public int ReadInt() { return Read<int>();}
    public uint ReadUInt() { return Read<uint>();}
    public long ReadLong() { return Read<long>();}
    public ulong ReadULong() { return Read<ulong>();}

};

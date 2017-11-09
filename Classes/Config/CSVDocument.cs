using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Resources;

/// <summary>  
/// 读取CSV工具类  
/// （需求：UTF-8格式）  
/// </summary>  
public class ColumElement 
{
    public string elementValue;
    private string m_DefaultIntValue = "0";
    public string Value  
    {
        get{ return elementValue;}
        set { elementValue = value; } 
    }
    public string ToString() { return elementValue; }
    public bool ToBool() { return ToInt64() !=0 ? true : false; }
    public byte ToByte() { return (byte)ToInt64(); }
    public sbyte ToSByte() { return (sbyte)ToInt64(); }
    public char ToChar() { return (char)ToInt64(); }
    public short ToInt16() { return (short)ToInt64(); }
    public ushort ToUInt16() { return (ushort)ToInt64(); }
    public int ToInt32() { return (int)ToInt64(); }
    public uint ToUInt32() { return (uint)ToInt64(); }
    public float ToFloat() { return  (float)ToDecimal(); }
    public double ToDouble() { return (double)ToDecimal(); }
    public long ToInt64()
    {
        try
        {
            return (elementValue.Length > 0) ? long.Parse(elementValue) : ((long)0);
        }
        catch (Exception e)
        {
            Log.Exception(e);
            return 0;
        }
    }
    public ulong ToUInt64()
    {
        try
        {
            return (elementValue.Length > 0) ? ulong.Parse(elementValue) : ((long)0);
        }
        catch (Exception e)
        {
            Log.Exception(e);
            return 0;
        }
    }
    public decimal ToDecimal()
    {
        try
        {
            return (elementValue.Length > 0) ? decimal.Parse(elementValue) : ((long)0);
        }
        catch (Exception e)
        {
            Log.Exception(e);
            return 0;
        }
    }
}
public class LoadCSVData 
{
    private List<List<string>> mDocumentText = null;
    private List<string> mDocumentColumNameList = null; //表头名称
    public int mRowNum; //行数量
    public int mColNum;//列数量
    static public Encoding mDefaultEncoding = System.Text.Encoding.UTF8; //CSV文件通用UTF8格式

    public void Clear()
    {
        if (mDocumentColumNameList != null )
        {
            mDocumentColumNameList.Clear();
        }
        if (mDocumentText != null)
        {
            mDocumentText.Clear();
        }
    }
    public bool Load(string text)
    {
        if (text.Length <= 1)
            return false;
        //先清除当前所有数据
        mDocumentText = new List<List<string>>();
        mDocumentColumNameList = new List<string>();
        mRowNum = 0;
        mColNum = 0;
        
        text = text.Replace("\n", "");
        string[] lineArray = text.Split('\r');
        bool rowSkip = false;
        for (int i = 0; i < lineArray.Length; i++)
        {
            string columnText = lineArray[i];
            if (columnText.Length >= 2 && columnText[0] == '/' && columnText[1] == '/')
                continue;	//如果头两个字符 是  //  代表该行无效
            if (columnText.Length >= 2 && columnText[0] == '\\' && columnText[1] == '\\')
                continue;	//如果头两个字符 是  \\  代表该行无效
            if (columnText.Length >= 2 && columnText[0] == ',' && columnText[1] == ',')
                continue;   //如果头两个字符都是‘，’表示该行数据无效
            if (columnText.Length > 2 && columnText[0] == '"' && columnText[1] == '/' && columnText[2] == '/')
                continue;	//如果头三个字符是"// 也认为该行无效
            if (columnText.Length >= 2 && columnText[0] == '/' && columnText[1] == '*' )
            {
                rowSkip = true;
                continue;	//如果头三个字符是"// 也认为该行无效
            }
            if (rowSkip && columnText.Length >= 2 && columnText[0] == '*' && columnText[1] == '/')
            {
                rowSkip = false;
                continue;	//如果头三个字符是"// 也认为该行无效
            }
            if (rowSkip)
                continue;

            string[] columnArray = readLine(columnText);
            if (i == 0)
            {
                mColNum = columnArray.Length;
                //创建列
                for (int j = 0; j < columnArray.Length; j++)
                {
                    mDocumentColumNameList.Add(columnArray[j]);
                }
            }
            else
            {
                List<string> tempLineStringList = new List<string>();
                for (int j = 0; j < columnArray.Length; j++)
                {
                    tempLineStringList.Add(columnArray[j]);
                }
                //最后一行有可能为空
                if (tempLineStringList.Count == 0 || (tempLineStringList.Count == 1 && string.IsNullOrEmpty(tempLineStringList[0])))
                    continue;
                mDocumentText.Add(tempLineStringList);
            }
        }
        mRowNum = mDocumentText.Count;
        return true;
    }

    private string[] readLine(string line)
    {
        var builder = new StringBuilder();
        var comma = false;
        var array = line.ToCharArray();
        var values = new List<string>();
        var length = array.Length;
        var index = 0;
        while (index < length)
        {
            var item = array[index++];
            switch (item)
            {
                case ',':
                    if (comma)
                    {
                        builder.Append(item);
                    }
                    else
                    {
                        values.Add(builder.ToString());
                        builder.Remove(0, builder.Length);
                    }
                    break;
                case '"':
                    comma = !comma;
                    break;
                default:
                    builder.Append(item);
                    break;
            }
        }
        if (builder.Length > 0)
            values.Add(builder.ToString());
        return values.ToArray();
    }

    //返回CSV文档行数量
	public int numRows()  { return mRowNum; }
	//返回CSV文档列数量
	public int numColumns()  { return mColNum; }
    //获取列名索引
    public int getColumnIndex(string columnName) 
    {
        //foreach(string s in mDocumentColumNameList)
        return mDocumentColumNameList.IndexOf(columnName);
    }
	//读取第rowIndex行columnIndex列的数据
    static ColumElement m_DefaultElement = new ColumElement();
    public ColumElement getValue(int rowIndex, string columnName)
    {
        m_DefaultElement.Value = "";
        int columnIndex = mDocumentColumNameList.IndexOf(columnName);
        if (rowIndex < 0 || mDocumentText.Count <= rowIndex)
            return m_DefaultElement;
        List<string> tempStrList = mDocumentText[rowIndex];
        if (columnIndex < 0 || tempStrList.Count <= columnIndex)
            return m_DefaultElement;
        m_DefaultElement.Value = tempStrList[columnIndex];
        return m_DefaultElement;
    }
}

/// <summary>  
/// 保存CSV工具类  
/// （需求：UTF-8格式）  
/// </summary>  
public class SaveCSVData
{
    private int mCurrRowNum = 0; //当前保存的行数量
    private int mCurrColNum = 0;//当前保存的列数量
    private int mMaxRowNum = 0; //当前总共行数量
    private int mMaxColNum = 0;//当前总共列数量
    private string mDocumentText = "";
    private StreamWriter mStreamWriter = null;
    public bool Open(FileStream fs)
    {
        mStreamWriter = new StreamWriter(fs, System.Text.Encoding.UTF8);
        return true;
    }

    public bool Save()
    {
        mStreamWriter.Close();
        return true;
    }
    //添加元素
    public void cat(string s)
	{
        string str = s.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
        if (str.Contains(',') || str.Contains('"') 
            || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
        {
            str = string.Format("\"{0}\"", str);
        }
        mDocumentText += str;
        if (mCurrColNum > 0)
            mDocumentText += ',';
        mCurrColNum++;
	}

    public void cat(byte val) { cat(val.ToString()); }
    public void cat(sbyte val) { cat(val.ToString()); }
    public void cat(char val) { cat(val.ToString()); }
    public void cat(short val) { cat(val.ToString()); }
    public void cat(ushort val) { cat(val.ToString()); }
    public void cat(int val) { cat(val.ToString()); }
    public void cat(uint val) { cat(val.ToString()); }
    public void cat(long val) { cat(val.ToString()); }
    public void cat(ulong val) { cat(val.ToString()); }
    public void cat(decimal val) { cat(val.ToString()); }
    public void cat(float val) { cat(val.ToString()); }
    public void cat(double val) { cat(val.ToString()); }


    //新行
    public void newRow()
    {
        mDocumentText += "\r\n";
        mCurrRowNum++;
        if (mCurrColNum >= mMaxColNum)
            mMaxColNum = mCurrColNum;
        mCurrColNum = 0;
        mMaxRowNum = mCurrRowNum;
    }
}
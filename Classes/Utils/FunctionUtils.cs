using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公共函数
/// @author hannibal
/// @time 2014-11-1
/// </summary>
public class FunctionUtils 
{
	/** 比较两个单精度浮点是否相等
	* @param 
	* @return 如符合指定精度返回true;否则返回false.
	*/
	static public bool FloatEqual(float fFirst, float fSecond, float fOffset)
	{
		return (Mathf.Abs( fFirst - fSecond) <= fOffset ? true : false);
	}
	
	/** 判断一个浮点数是否是0
	* @param fValue - 比较的数
	* @param fOffset - 比较精度
	* @return 如符合指定精度返回true;否则返回false.
	*/
	static public bool FloatEqualZERO(float fValue,float fOffset = 0.00001f)
	{
		return (Mathf.Abs(fValue) <= fOffset ? true : false);
	}

	/**
	* 测试两个数是否有交集
	* @param nValue - 待测试的数
	* @param nStatus - 需要测试的状态
	* @return true - 含有这种状态； false - 不含有
	*/
	static public bool TestHasIntersect(uint nValue, uint nStatus)
	{
		return (((nValue&nStatus) != 0) ? true : false);
	}

	/// @detail 本 算法由于在Brian Kernighan与Dennis Ritchie的《The C Programming Language》一书被展示而得 名，
	/// 是一种简单快捷的hash算法，也是Java目前采用的字符串的Hash算法（累乘因子为31）。  
	static public long BKDRHash(string str)  
	{  
		long hash = 0;  
		for(int i = 0; i < str.Length; ++i)
		{         
			hash = hash * 131 + str[i];   // 也可以乘以31、131、1313、13131、131313..         
		}  
		return hash;  
	}

	/// <summary>
	/// 分割字符串
	/// </summary>
	/// <param name="str">源字符串</param>
	/// <param name="split">分割符</param>
	public static List<T> Split<T>(string str, char split)
	{
		string[] strArr = str.Split(split);
		List<T> tmpList = new List<T>();
        if (str.Length == 0) return tmpList;
		
		if (typeof(T) == typeof(int))
		{
			for (int i = 0; i < strArr.Length; i++)
			{
				tmpList.Add((T)(object)int.Parse(strArr[i]));
			}
		}
		else if (typeof(T) == typeof(uint))
		{
			for (int i = 0; i < strArr.Length; i++)
			{
				tmpList.Add((T)(object)uint.Parse(strArr[i]));
			}
		}
		else if(typeof(T) == typeof(float))
		{
			for (int i = 0; i < strArr.Length; i++)
			{
				tmpList.Add((T)(object)float.Parse(strArr[i]));
			}
		}
		else if(typeof(T) == typeof(string))
		{
			for (int i = 0; i < strArr.Length; i++)
			{
				tmpList.Add((T)(object)strArr[i]);
			}
		}
		else if (typeof(T) == typeof(byte))
		{
			for (int i = 0; i < strArr.Length; i++)
			{
				tmpList.Add((T)(object)byte.Parse(strArr[i]));
			}
		}
		else if (typeof(T) == typeof(short))
		{
			for (int i = 0; i < strArr.Length; i++)
			{
				tmpList.Add((T)(object)short.Parse(strArr[i]));
			}
		}
		else
		{
			Debug.LogError("Split : type error");
		}
		
		return tmpList;
	}
}

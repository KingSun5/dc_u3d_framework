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
}

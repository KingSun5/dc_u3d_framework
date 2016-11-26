using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 颜色
/// @author hannibal
/// @time 2015-2-3
/// </summary>
public class ColorUtils
{
	
	/**
	 * 16进制颜色转rgba
	 * color = "ff(R)ff(G)ff(B)ff(A)"
	 */
	static public void GetRGBA(string color, out int r, out int g, out int b, out int a)
	{
		if(color.Length != 8)
		{
			r = g = b = a = 255;
			Log.Error("CommonUtils::GetRGBA - color error:" + color);
			return;
		}
		string str = color.Substring(0,2);
		r = Convert.ToInt32(str, 16);
		
		str = color.Substring(2,2);
		g = Convert.ToInt32(str, 16);
		
		str = color.Substring(4,2);
		b = Convert.ToInt32(str, 16);
		
		str = color.Substring(6,2);
		a = Convert.ToInt32(str, 16);
	}

	/**
	 * color转16进制颜色
	 * return "ff(R)ff(G)ff(B)ff(A)"
	 */
	static public string Color2RGBA(Color color)
	{
		return IntUtils.ToHexString((long)color.r*255)+
				IntUtils.ToHexString((long)color.g*255)+
				IntUtils.ToHexString((long)color.b*255)+
				IntUtils.ToHexString((long)color.a*255);
	}
}

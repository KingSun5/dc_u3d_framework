using UnityEngine;
using System.Collections;
using System.Text;
using System;


/// <summary>
/// 字符串
/// @author hannibal
/// @time 2014-11-14
/// </summary>
public class StringUtils 
{
	/**获取两个字符串中间的字符串*/
	static public string Search_string(string s, string s1, string s2) 
	{  
		int n1, n2;  
		n1 = s.IndexOf(s1, 0);  //开始位置 
		if(n1 < 0)return "";
		n1 += s1.Length;
		n2 = s.IndexOf(s2, n1);             //结束位置  
		if(n2 < 0)return "";
		return s.Substring(n1, n2 - n1);   	//取搜索的条数，用结束的位置-开始的位置,并返回  
	}  

	//～～～～～～～～～～～～～～～～～～～～～～～time~～～～～～～～～～～～～～～～～～～～～～～～//
	/**
	 * 分钟与秒格式(如-> 40:15)
	 * @param seconds
	 * @return 
	 */		
	public static string MinuteFormat(int seconds)
	{
		int min = seconds / 60;
		int sec = seconds % 60;
		
		string min_str = min < 10 ? ("0" + min.ToString()) : (min.ToString());
		string sec_str = sec < 10 ? ("0" + sec.ToString()) : (sec.ToString());
		
		return min_str + ":" + sec_str;
	}
	/**
	 * 时分秒格式(如-> 05:32:20)
	 * @param seconds(秒)
	 * @return 
	 */
	public static string HourFormat(int seconds)
	{
		int hour = seconds / 3600;
		
		string hour_str = hour < 10 ? ("0" + hour.ToString()) : (hour.ToString());

		return hour_str + ":" + MinuteFormat(seconds % 3600);
	}

	//～～～～～～～～～～～～～～～～～～～～～～～html~～～～～～～～～～～～～～～～～～～～～～～～//
	/**
	 * 设置颜色
	 * color 格式 #ff0000ff
	 */
	public static string SetFontColor(string str, string color)
	{
		if(color.Length == 0)return str;
		return "<color="+color+">" + str + "</color>";
	}
	/**
	 * 字体加粗
	 */
	public static string SetFontBold(string str)
	{
		return "<b>" + str + "</b>";
	}
	/**
	 * 斜体
	 */
	public static string SetFontItalic(string str)
	{
		return "<i>" + str + "</i>";
	}
	/**
	 * 字体大小
	 */
	public static string SetFontSize(string str, int size)
	{
		return "<size="+size.ToString()+">" + str + "</size>";
	}
    public static string SetFontColorSize(string str, string color, int size)
    {
        string strHtml = SetFontColor(str, color);
        strHtml = SetFontSize(strHtml, size);
        return strHtml;
    }
}

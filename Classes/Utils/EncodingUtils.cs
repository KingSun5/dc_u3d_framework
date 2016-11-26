using UnityEngine;
using System.Collections;
using System.Text;
using System;


/// <summary>
/// 编码转换
/// @author hannibal
/// @time 2015-1-22
/// </summary>
public class EncodingUtils
{
	/// <summary>
	/// 实现多种编码方式的转换
	/// </summary>
	/// <param name="str">要转换的字符</param>
	/// <param name="From">从哪种方式转换，如UTF-8</param>
	/// <param name="To">转换成哪种编码,如GB2312</param>
	/// <returns>转换结果</returns>
	static public string ConvertStr(string str, string From, string To)
	{
		
		byte[] bs = System.Text.Encoding.GetEncoding(From).GetBytes(str);
		bs = System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(From), System.Text.Encoding.GetEncoding(To), bs);
		string res = System.Text.Encoding.GetEncoding(To).GetString(bs);
		return res;
	}

	/// <summary>
	/// url地址栏转换
	/// </summary>
	/// <returns>The encode.</returns>
	/// <param name="str">String.</param>
	public static string UrlEncode(string str)
	{
		StringBuilder sb = new StringBuilder();
		byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
		for (int i = 0; i < byStr.Length; i++)
		{
			sb.Append(@"%" + Convert.ToString(byStr[i], 16));
		}
		
		return (sb.ToString());
	}
}

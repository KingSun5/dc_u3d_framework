using UnityEngine;
using System.Collections;
using System.IO;

public class DataUtils
{
	public static string BinToUtf8 (byte[] total)
	{
		byte[] result = total;
		if (total [0] == 0xef && total [1] == 0xbb && total [2] == 0xbf) 
		{
			// utf8文件的前三个字节为特殊占位符，要跳过
			result = new byte[total.Length - 3];
			System.Array.Copy (total, 3, result, 0, total.Length - 3);
		}
		
		string utf8string = System.Text.Encoding.UTF8.GetString (result);
		return utf8string;
	}
}

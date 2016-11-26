using UnityEngine;
using System.Collections;
using System;


public class IntUtils
{
	/**
	 * 转16进制字符串*
	 * 123->ffff
	 */
	static public string ToHexString(long value)
	{
		string str = Convert.ToString(value, 16);
		if(str.Length == 1)str = "0"+str;
		return str;
	}
    //最接近2次方的值
    static public int upper_power_of_two(int v)
    {
        v--;
        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;
        v++;
        return v;
    }

    static public long max(long v, long u)
    {
        return v > u ? v : u;
    }
    static public double max(double v, double u)
    {
        return v > u ? v : u;
    }
}

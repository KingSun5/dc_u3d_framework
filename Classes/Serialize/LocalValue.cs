using UnityEngine;
using System.Collections;

/// <summary>
/// 本地数据
/// @author hannibal
/// @time 2014-12-19
/// </summary>
public class LocalValue
{
	/// <summary>
	/// 泛型存取
	/// </summary>
	/// <param name="key">Key.键</param>
	/// <param name="value">Value.值</param>
	public static void SetValue<T>(string key, T value)
	{
		if (value.GetType() == typeof(int))
		{
			PlayerPrefs.SetInt(key, (int)(object)value);
		}
		else if(value.GetType() == typeof(string))
		{
			PlayerPrefs.SetString(key, (string)(object)value);
		}
		else if(value.GetType() == typeof(float))
		{
			PlayerPrefs.SetFloat(key, (float)(object)value);
		}
		else
		{
			Debug.LogError("SetValue : type error");
		}
	}
	public static T GetValue<T>(string key, T defaultVaule)
	{
		if (typeof(T) == typeof(int))
		{
			return (T)(object)PlayerPrefs.GetInt(key,(int)(object)defaultVaule);
		}
		else if(typeof(T) == typeof(string))
		{
			return (T)(object)PlayerPrefs.GetString(key,(string)(object)defaultVaule);
		}
		else if(typeof(T) == typeof(float))
		{
			return (T)(object)PlayerPrefs.GetFloat(key,(float)(object)defaultVaule);
		}
		else
		{
			Debug.LogError("GetValue : type error");
			return (T)(object)0;
		}
	}

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
}

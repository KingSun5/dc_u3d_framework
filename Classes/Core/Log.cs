using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public enum eLogLevel
{
	DEBUG = 0,
	INFO,
	WARNING,
	ERROR,
	EXCEPTION,
	MAX,
}

/// 日志输出
/// @author hannibal
/// @time 2014-11-24
/// </summary>
public class Log 
{
	public delegate void RegistFunction(string msg);
    
	static public eLogLevel			LogLv = eLogLevel.DEBUG;
    static public bool[]            EnableType = {true, true, true, true, true};
	
	static public bool 				StoreLog = false;	                //记录日志：开启存在问题
	static public List<string> 		ListLog = new List<string>(1000);   //记录日志
	static public RegistFunction	MsgFun = null;		                //日志监视

    static private StringBuilder    tmpStr;
	/// <summary>
    /// 临时或测试数据使用：不会记录日志
	/// </summary>
	/// <param name="msg"></param>
	static public void Debug(params string[] msg)
	{
		if (LogLv > eLogLevel.DEBUG)return;
        if (!EnableType[(int)eLogLevel.DEBUG]) return;

        tmpStr.Length = 0;
		for(int i = 0; i < msg.Length; ++i)
		{
            tmpStr = tmpStr.Append(msg[i]);
		}
        string log = "[debug]" + tmpStr.ToString();
		UnityEngine.Debug.Log(log);
		if (MsgFun != null)MsgFun(log);
	}
	/// <summary>
    /// 临时或测试数据使用：不会记录日志
	/// </summary>
	/// <param name="msg"></param>
	static public void Debug(string msg)
	{
        if (LogLv > eLogLevel.DEBUG) return;
        if (!EnableType[(int)eLogLevel.DEBUG]) return;

		string log = "[debug]"+msg;
		UnityEngine.Debug.Log(log);
		if (MsgFun != null)MsgFun(log);
	}

	static public void Info(params string[] msg)
	{
        if (LogLv > eLogLevel.INFO) return;
        if (!EnableType[(int)eLogLevel.INFO]) return;

        tmpStr.Length = 0;
		for(int i = 0; i < msg.Length; ++i)
		{
            tmpStr = tmpStr.Append(msg[i]);
		}
        string log = "[info]" + tmpStr.ToString();
		if(StoreLog)ListLog.Add(log);
		UnityEngine.Debug.Log(log);
		if (MsgFun != null)MsgFun(log);
	}
	static public void Info(string msg)
	{
        if (LogLv > eLogLevel.INFO) return;
        if (!EnableType[(int)eLogLevel.INFO]) return;

		string log = "[info]"+msg;
		if(StoreLog)ListLog.Add(log);
		UnityEngine.Debug.Log(log);
		if (MsgFun != null)MsgFun(log);
	}
	/// <summary>
	/// 警告
	/// </summary>
	/// <param name="msg"></param>
	static public void Warning(params string[] msg)
	{
        if (LogLv > eLogLevel.WARNING) return;
        if (!EnableType[(int)eLogLevel.WARNING]) return;

        tmpStr.Length = 0;
		for(int i = 0; i < msg.Length; ++i)
		{
            tmpStr = tmpStr.Append(msg[i]);
		}
        string log = "<color=yellow>[warning]</color>" + tmpStr.ToString();
		if(StoreLog)ListLog.Add(log);
		UnityEngine.Debug.LogWarning(log);
		if (MsgFun != null)MsgFun(log);
	}
    /// <summary>
    /// 警告
    /// </summary>
    /// <param name="msg"></param>
	static public void Warning(string msg)
	{
        if (LogLv > eLogLevel.WARNING) return;
        if (!EnableType[(int)eLogLevel.WARNING]) return;

		string log = "<color=yellow>[warning]</color>"+msg;
		if(StoreLog)ListLog.Add(log);
		UnityEngine.Debug.LogWarning(log);
		if (MsgFun != null)MsgFun(log);
	}
	/// <summary>
	/// 错误
	/// </summary>
	/// <param name="msg"></param>
	static public void Error(params string[] msg)
	{
        if (LogLv > eLogLevel.ERROR) return;
        if (!EnableType[(int)eLogLevel.ERROR]) return;

        tmpStr.Length = 0;
		for(int i = 0; i < msg.Length; ++i)
		{
            tmpStr = tmpStr.Append(msg[i]);
		}
        string log = "<color=red>[error]</color>" + tmpStr.ToString();
		if(StoreLog)ListLog.Add(log);
		UnityEngine.Debug.LogError(log);
		if (MsgFun != null)MsgFun(log);
	}
    /// <summary>
    /// 错误
    /// </summary>
    /// <param name="msg"></param>
	static public void Error(string msg)
	{
        if (LogLv > eLogLevel.ERROR) return;
        if (!EnableType[(int)eLogLevel.ERROR]) return;

		string log = "<color=red>[error]</color>"+msg;
		if(StoreLog)ListLog.Add(log);
		UnityEngine.Debug.LogError(log);
		if (MsgFun != null)MsgFun(log);
	}
	/// <summary>
	/// 抛出异常
	/// </summary>
	/// <param name="msg"></param>
	static public void Exception(params string[] msg)
	{
        if (LogLv > eLogLevel.EXCEPTION) return;
        if (!EnableType[(int)eLogLevel.EXCEPTION]) return;

        tmpStr.Length = 0;
		for(int i = 0; i < msg.Length; ++i)
		{
            tmpStr = tmpStr.Append(msg[i]);
		}
        if (StoreLog) ListLog.Add("[exception]" + tmpStr);
        UnityEngine.Debug.LogException(new Exception(tmpStr.ToString()));
        if (MsgFun != null) MsgFun(tmpStr.ToString());
	}
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="msg"></param>
	static public void Exception(string msg)
	{
        if (LogLv > eLogLevel.EXCEPTION) return;
        if (!EnableType[(int)eLogLevel.EXCEPTION]) return;

		if(StoreLog)ListLog.Add("[exception]"+msg);
		UnityEngine.Debug.LogException(new Exception(msg));
		if (MsgFun != null)MsgFun(msg);
	}
}

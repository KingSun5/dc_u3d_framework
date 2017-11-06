using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public enum eLogLevel
{
    LV_DEBUG = 0,
    LV_INFO,
    LV_WARNING,
    LV_ERROR,
    LV_EXCEPTION,
    LV_MAX,
}

/// 日志输出
/// @author hannibal
/// @time 2014-11-24
/// </summary>
public class Log
{
    public delegate void RegistFunction(string msg);
    static public        RegistFunction MsgFun = null;		//日志监视

    static public eLogLevel LogLv = eLogLevel.LV_DEBUG;
    static public bool[] EnableType = { true, true, true, true, true };

    /// <summary>
    /// 临时或测试数据使用：正式游戏后会关闭
    /// </summary>
    static public void Debug(params string[] msg)
    {
        if (LogLv > eLogLevel.LV_DEBUG) return;
        if (!EnableType[(int)eLogLevel.LV_DEBUG]) return;

        StringBuilder st = new StringBuilder();
        for (int i = 0; i < msg.Length; ++i)
        {
            st = st.Append(msg[i]);
        }
        string log = "[debug]" + st.ToString();
        UnityEngine.Debug.Log(log);
        if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 临时或测试数据使用：正式游戏后会关闭
    /// </summary>
    static public void Debug(string msg)
    {
        if (LogLv > eLogLevel.LV_DEBUG) return;
        if (!EnableType[(int)eLogLevel.LV_DEBUG]) return;

        string log = "[debug]" + msg;
        UnityEngine.Debug.Log(log);
        if (MsgFun != null) MsgFun(log);
    }

    static public void Info(params string[] msg)
    {
        if (LogLv > eLogLevel.LV_INFO) return;
        if (!EnableType[(int)eLogLevel.LV_INFO]) return;

        StringBuilder st = new StringBuilder();
        for (int i = 0; i < msg.Length; ++i)
        {
            st = st.Append(msg[i]);
        }
        string log = "[info]" + st.ToString();
        UnityEngine.Debug.Log(log);
        if (MsgFun != null) MsgFun(log);
    }
    static public void Info(string msg)
    {
        if (LogLv > eLogLevel.LV_INFO) return;
        if (!EnableType[(int)eLogLevel.LV_INFO]) return;

        string log = "[info]" + msg;
        UnityEngine.Debug.Log(log);
        if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 警告
    /// </summary>
    static public void Warning(params string[] msg)
    {
        if (LogLv > eLogLevel.LV_WARNING) return;
        if (!EnableType[(int)eLogLevel.LV_WARNING]) return;

        StringBuilder st = new StringBuilder();
        for (int i = 0; i < msg.Length; ++i)
        {
            st = st.Append(msg[i]);
        }
        string log = "<color=yellow>[warning]</color>" + st.ToString();
        UnityEngine.Debug.LogWarning(log);
        if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 警告
    /// </summary>
    static public void Warning(string msg)
    {
        if (LogLv > eLogLevel.LV_WARNING) return;
        if (!EnableType[(int)eLogLevel.LV_WARNING]) return;

        string log = "<color=yellow>[warning]</color>" + msg;
        UnityEngine.Debug.LogWarning(log);
        if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 错误
    /// </summary>
    static public void Error(params string[] msg)
    {
        if (LogLv > eLogLevel.LV_ERROR) return;
        if (!EnableType[(int)eLogLevel.LV_ERROR]) return;

        StringBuilder st = new StringBuilder();
        for (int i = 0; i < msg.Length; ++i)
        {
            st = st.Append(msg[i]);
        }
        string log = "<color=red>[error]</color>" + st.ToString();
        UnityEngine.Debug.LogError(log);
        if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 错误
    /// </summary>
    static public void Error(string msg)
    {
        if (LogLv > eLogLevel.LV_ERROR) return;
        if (!EnableType[(int)eLogLevel.LV_ERROR]) return;

        string log = "<color=red>[error]</color>" + msg;
        UnityEngine.Debug.LogError(log);
        if (MsgFun != null) MsgFun(log);
    }
    /// <summary>
    /// 抛出异常
    /// </summary>
    static public void Exception(params string[] msg)
    {
        if (LogLv > eLogLevel.LV_EXCEPTION) return;
        if (!EnableType[(int)eLogLevel.LV_EXCEPTION]) return;

        StringBuilder st = new StringBuilder();
        for (int i = 0; i < msg.Length; ++i)
        {
            st = st.Append(msg[i]);
        }
        UnityEngine.Debug.LogException(new Exception(st.ToString()));
        if (MsgFun != null) MsgFun(st.ToString());
    }
    /// <summary>
    /// 抛出异常
    /// </summary>
    static public void Exception(string msg)
    {
        if (LogLv > eLogLevel.LV_EXCEPTION) return;
        if (!EnableType[(int)eLogLevel.LV_EXCEPTION]) return;

        UnityEngine.Debug.LogException(new Exception(msg));
        if (MsgFun != null) MsgFun(msg);
    }
    /// <summary>
    /// 抛出异常
    /// </summary>
    static public void Exception(Exception e)
    {
        if (LogLv > eLogLevel.LV_EXCEPTION) return;
        if (!EnableType[(int)eLogLevel.LV_EXCEPTION]) return;

        UnityEngine.Debug.LogException(e);
        if (MsgFun != null) MsgFun(e.Message);
    }
}

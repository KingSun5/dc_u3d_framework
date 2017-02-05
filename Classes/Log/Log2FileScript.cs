using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

/// <summary>
/// log写入文件
/// @author hannibal
/// @time 2016-11-8
/// </summary>
public class Log2FileScript : MonoBehaviour
{
    private List<string>    mWriteTxt = new List<string>();

    [SerializeField, SetProperty("IsWrite2File")]
    private bool            m_IsWrite2File = false;
    [SerializeField]
    private bool            m_Log = false;
    [SerializeField]
    private bool            m_Warning = true;
    [SerializeField]
    private bool            m_Error = true;
    [SerializeField]
    private bool            m_Exceptioin = true;

    private string          m_OutPath = "";

	void Start ()
    {
        if (m_IsWrite2File)
        {
            m_OutPath = GlobalID.RootSDCard + "/outLog.txt";
            if (IsWrite2File && m_OutPath.Length > 0)
            {
                using (StreamWriter writer = new StreamWriter(m_OutPath, false, Encoding.UTF8))
                {
                }
            }
        }
	}

    void OnDestroy()
    {
    }

    void OnEnable()
    {
        if (m_IsWrite2File) Application.logMessageReceived += OnReceivedLog;
    }
    void OnDisable()
    {
        if (m_IsWrite2File) Application.logMessageReceived -= OnReceivedLog;
    }
 
	void Update () 
	{
        if (m_IsWrite2File && mWriteTxt.Count > 0)
		{
			string[] temp = mWriteTxt.ToArray();
            int count = 0;
            using(StreamWriter writer = new StreamWriter(m_OutPath, true, Encoding.UTF8))
            {
                string t;
                for (int i = 0; i < temp.Length; ++i )
                {
                    t = temp[i];
                    writer.WriteLine(t);
                    mWriteTxt.Remove(t);
                    if (++count > 10) break;
                }
            }
		}
	}

    void OnReceivedLog(string logString, string stackTrace, LogType type)
	{
        if (m_IsWrite2File)
        {
            switch(type)
            {
                case LogType.Log:       if (m_Log)       mWriteTxt.Add(logString); break;
                case LogType.Warning:   if (m_Warning)   mWriteTxt.Add(logString); break;
                case LogType.Error:     if (m_Error)     mWriteTxt.Add(logString); break;
                case LogType.Exception: if (m_Exceptioin)mWriteTxt.Add(logString); break;
            }
        }
		if (type == LogType.Error || type == LogType.Exception) 
		{
			HandleLog(logString);
            HandleLog(stackTrace);
		}
	}

    void HandleLog(params object[] objs)
	{
        if (m_IsWrite2File)
        {
            string text = "";
            for (int i = 0; i < objs.Length; ++i)
            {
                if (i == 0)
                {
                    text += objs[i].ToString();
                }
                else
                {
                    text += ", " + objs[i].ToString();
                }
            }
            mWriteTxt.Add(text);
        }
	}

    public bool IsWrite2File
    {
        get { return m_IsWrite2File; }
        set 
        { 
            m_IsWrite2File = value;
            m_Log = m_IsWrite2File;
            m_Warning = m_IsWrite2File;
            m_Error = m_IsWrite2File;
            m_Exceptioin = m_IsWrite2File;
        }
    }
}

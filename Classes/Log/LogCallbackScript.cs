using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

/// <summary>
/// log写入文件
/// @author hannibal
/// @time 2016-11-8
/// </summary>
public class LogCallbackScript : MonoBehaviour
{
    private static List<string> mWriteTxt = new List<string>();

    public bool             m_IsWrite2File = false;
    public string           m_AndroidPath = "";
    private StreamWriter    m_WriterHandle = null;

    private Rect            m_Rectwindow;
    private GUIStyle        m_TextStyle;
    private StringBuilder   m_ErrMsgBuffer = new StringBuilder();

	void Start ()
    {
        if (m_IsWrite2File)
        {
            string outpath = "";
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                outpath = Application.persistentDataPath + "/outLog.txt";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                outpath = m_AndroidPath + "/outLog.txt";
            }
            if (outpath.Length > 0)
            {
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(outpath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(outpath));
                    }
                    if (!File.Exists(outpath))
                    {
                        m_WriterHandle = new StreamWriter(File.Create(outpath));
                    }
                    else
                    {
                        m_WriterHandle = new StreamWriter(File.OpenWrite(outpath), Encoding.UTF8);
                    }
                    m_WriterHandle.AutoFlush = true;
                }
                finally
                {
                    m_WriterHandle = null;
                }
            }
        }

        m_TextStyle = new GUIStyle();
        m_TextStyle.wordWrap = true;
        m_TextStyle.normal.textColor = Color.yellow;
        m_TextStyle.fontSize = (int)(15.0f * (UIID.ScreenScaleX + UIID.ScreenScaleY) * 0.5f);
        m_TextStyle.alignment = TextAnchor.LowerLeft;
        m_Rectwindow = new Rect(0, 0, Screen.width*0.75f, Screen.height);
	}

    void OnDestroy()
    {
        if(m_WriterHandle != null)
        {
            m_WriterHandle.Close();
            m_WriterHandle.Dispose();
            m_WriterHandle = null;
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += OnHandleLog;
    }
    void OnDisable()
    {
        Application.logMessageReceived -= OnHandleLog;
    }
 
	void Update () 
	{
        if (m_IsWrite2File && m_WriterHandle != null && mWriteTxt.Count > 0)
		{
			string[] temp = mWriteTxt.ToArray();
            int count = 0;
            try
            {
                foreach (string t in temp)
                {
                    m_WriterHandle.WriteLine(t);
                    mWriteTxt.Remove(t);
                    if (++count > 10) break;
                }
            }
            finally
            {
                m_WriterHandle = null;
            }
		}
	}

    void OnHandleLog(string logString, string stackTrace, LogType type)
	{
        if (m_IsWrite2File)
        {
            mWriteTxt.Add(logString);
        }
		if (type == LogType.Error || type == LogType.Exception) 
		{
			Log(logString);
			Log(stackTrace);
		}
	}
 
	void Log (params object[] objs)
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
        if (m_IsWrite2File)
        {
            mWriteTxt.Add(text);
        }
		if (Application.isPlaying)
		{
            m_ErrMsgBuffer.Append(text + '\n');
            if (m_ErrMsgBuffer.Length > 5000)
            {
                m_ErrMsgBuffer.Remove(0, m_ErrMsgBuffer.Length - 5000);
            }
		}
	}
 
	void OnGUI()
	{
        if (m_ErrMsgBuffer.Length > 0)
        {
            GUI.color = Color.red;
            GUI.TextArea(m_Rectwindow, m_ErrMsgBuffer.ToString(), m_TextStyle);
        }
	}
}

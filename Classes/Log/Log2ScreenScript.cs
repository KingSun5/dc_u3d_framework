using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 错误写入屏幕
/// @author hannibal
/// @time 2016-1-19
/// </summary>
public class Log2ScreenScript : MonoBehaviour
{
    [SerializeField]
    private bool m_IsWrite2Screen = false;

    private Rect          m_Rectwindow;
    private GUIStyle      m_TextStyle;
    private StringBuilder m_ErrMsgBuffer = new StringBuilder();

    void Start()
    {
        if (m_IsWrite2Screen)
        {
            m_TextStyle = new GUIStyle();
            m_TextStyle.wordWrap = true;
            m_TextStyle.normal.textColor = Color.yellow;
            m_TextStyle.fontSize = (int)(15.0f * (UIID.ScreenScaleX + UIID.ScreenScaleY) * 0.5f);
            m_TextStyle.alignment = TextAnchor.LowerLeft;
            m_Rectwindow = new Rect(0, 0, Screen.width * 0.75f, Screen.height);
        }
    }

    void OnDestroy()
    {
    }

    void OnEnable()
    {
        if (m_IsWrite2Screen) Application.logMessageReceived += OnReceivedLog;
    }
    void OnDisable()
    {
        if (m_IsWrite2Screen) Application.logMessageReceived -= OnReceivedLog;
    }

    void OnReceivedLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            HandleLog(logString);
            HandleLog(stackTrace);
        }
    }

    void HandleLog(params object[] objs)
    {
        if (!m_IsWrite2Screen) return;

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
        m_ErrMsgBuffer.Append(text + '\n');
        if (m_ErrMsgBuffer.Length > 100)
        {
            m_ErrMsgBuffer.Remove(0, m_ErrMsgBuffer.Length - 100);
        }
    }

    void OnGUI()
    {
        if (!m_IsWrite2Screen || m_TextStyle == null) return;
        if (m_ErrMsgBuffer.Length > 0)
        {
            GUI.color = Color.red;
            GUI.TextArea(m_Rectwindow, m_ErrMsgBuffer.ToString(), m_TextStyle);
        }
    }
}

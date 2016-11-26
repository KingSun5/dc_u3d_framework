using UnityEngine;
using System.Collections;

/// <summary>
/// 测试用对话框
/// @author hannibal
/// @time 2016-9-12
/// </summary>
public class TestAlert : MonoBehaviour
{
	private bool 		m_IsShow = false;
	private string 		m_Msg = "";
	private string 		m_Title = "";

	private GUIStyle 	m_TextStyle;
	private Rect 		m_Rectwindow;

	private float 		m_ScaleX = 1;
	private float 		m_ScaleY = 1;

	private static TestAlert m_Instance;
	public static TestAlert Instance
	{
		get
		{ 
			if(!Camera.main.gameObject.GetComponent<TestAlert>())
			{
				Camera.main.gameObject.AddComponent<TestAlert>();
			}
			return m_Instance; 
		}
	}
	void Awake()
	{
		m_Instance = this;

		m_ScaleX = Screen.width / UIID.DEFAULT_WIDTH;
		m_ScaleY = Screen.height / UIID.DEFAULT_HEIGHT;
		m_Rectwindow = new Rect(0,0,300*m_ScaleX,150*m_ScaleY);

		m_TextStyle = new GUIStyle();
		m_TextStyle.fontSize = (int)(20.0f*(m_ScaleX+m_ScaleY)*0.5f); 
		m_TextStyle.wordWrap = true;
	}

	void OnGUI () 
	{  
		if (!m_IsShow)
			return;

		m_Rectwindow.x = (Screen.width - m_Rectwindow.width) * 0.5f;
		m_Rectwindow.y = (Screen.height - m_Rectwindow.height) * 0.5f;
		GUI.Window(0,m_Rectwindow,OnWindow,m_Title);  
	}  
	void OnWindow(int id)
	{  
		GUI.TextArea(new Rect(10,30,m_Rectwindow.width - 10*2,m_Rectwindow.height-30-50*m_ScaleY),m_Msg, m_TextStyle);  
		if(GUI.Button(new Rect((m_Rectwindow.width-60*m_ScaleX)*0.5f,m_Rectwindow.height-40*m_ScaleY,60*m_ScaleX,30*m_ScaleY),"OK"))
		{  
			Hide();
		} 
	} 

	public void Show(string msg, string title)
	{
		m_IsShow = true;
		m_Msg = msg;
		m_Title = title;
	}

	public void Hide()
	{
		m_IsShow = false;
	}
}

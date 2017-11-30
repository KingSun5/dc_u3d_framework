using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 弹框
/// @author hannibal
/// @time 2016-2-15
/// </summary>
public class AlertView : UIPanelBase
{
	public Button[]     m_ArrBtn = new Button[(int)eAlertBtnType.MAX];
	public Text         m_ContentText;

	private string      m_Content;
	private object      m_Info;
    private System.Action<eAlertBtnType>        m_Fun;
    private Dictionary<eAlertBtnType, string>   m_DicBtn;

    public AlertView()
    {
        m_DicBtn = new Dictionary<eAlertBtnType, string>();
    }

	public override void Show(params object[] info)
    {
        base.Show(info);

		for(int i = 0; i < m_ArrBtn.Length; ++i)
		{
			m_ArrBtn[i].gameObject.SetActive(false);
		}

		foreach(var obj in m_DicBtn)
		{
			m_ArrBtn[(int)obj.Key].gameObject.SetActive(true);
			m_ArrBtn[(int)obj.Key].name = ((int)obj.Key).ToString();
			m_ArrBtn[(int)obj.Key].GetComponentInChildren<Text>().text = obj.Value;
		}
		m_ContentText.text = m_Content;
	}

	public override void RegisterEvent ()
	{
		for(int i = 0; i < m_ArrBtn.Length; ++i)
		{
			UIEventListener.Get(m_ArrBtn[i].gameObject).AddEventListener(eUIEventType.Click, OnBtnClick);
		}
	}
	public override void UnRegisterEvent ()
	{
		for(int i = 0; i < m_ArrBtn.Length; ++i)
		{
			UIEventListener.Get(m_ArrBtn[i].gameObject).RemoveEventListener(eUIEventType.Click, OnBtnClick);
		}
	}
	
	/**点击*/
	private void OnBtnClick(UIEvent evt)
	{
		if(m_Fun != null)
		{
            m_Fun((eAlertBtnType)(System.Convert.ToInt32(evt.target.name)));

			AlertManager.Instance.Remove();
		}
	}

	public string Content
	{
		set{ m_Content = value; }
	}
    public System.Action<eAlertBtnType> Fun
	{
		set{ m_Fun = value; }
	}
	public object Info
	{
		set{ m_Info = value; }
	}
	public Dictionary<eAlertBtnType, string> DicBtn
	{
		set{ m_DicBtn = value; }
        get { return m_DicBtn; }
	}
}

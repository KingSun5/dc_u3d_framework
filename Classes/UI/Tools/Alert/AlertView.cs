using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class AlertView : UIPanelBase
{
	public delegate void FunCallback(GameEvent evt);

	public Button[] m_ArrBtn = new Button[(int)AlertID.EBtnType.MAX];
	public Text m_ContentText;

	private string m_Content;
	private FunCallback m_Fun;
	private object m_Info;
	private Dictionary<AlertID.EBtnType, string> m_DicBtn;

	public override void OnEnable ()
	{
        base.OnEnable();

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

		EngineManager.Instance.HandlePauseGame(true);
	}
	public override void OnDisable ()
	{
        EngineManager.Instance.HandlePauseGame(false);

        GameObject.Destroy(gameObject);

        base.OnDisable();
	}
	
	public override void RegisterEvent ()
	{
		for(int i = 0; i < m_ArrBtn.Length; ++i)
		{
			UIEventTriggerListener.Get(m_ArrBtn[i].gameObject).onClick += OnBtnClick;
		}
	}
	public override void UnRegisterEvent ()
	{
		for(int i = 0; i < m_ArrBtn.Length; ++i)
		{
			UIEventTriggerListener.Get(m_ArrBtn[i].gameObject).onClick -= OnBtnClick;
		}
	}
	
	/**点击*/
	private void OnBtnClick(GameObject obj, Vector2 pos)
	{
		if(m_Fun != null)
		{
			m_Fun(new GameEvent((AlertID.EBtnType)(System.Convert.ToInt32(obj.name)), m_Info));

			AlertManager.Instance.Remove();
		}
	}

	public string Content
	{
		set{ m_Content = value; }
	}
	public FunCallback Fun
	{
		set{ m_Fun = value; }
	}
	public object Info
	{
		set{ m_Info = value; }
	}
	public Dictionary<AlertID.EBtnType, string> DicBtn
	{
		set{ m_DicBtn = value; }
	}
}

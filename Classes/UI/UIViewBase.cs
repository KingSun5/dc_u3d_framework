using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// UI基类
/// GameObject的Activity为true，脚本的enable为true时，其先后顺序为：Awake、OnEnable、Start；
/// GameObject的Activity为true，脚本的enable为false时,只运行Awake；
/// @author hannibal
/// @time 2014-12-4
/// </summary>
public class UIViewBase : MonoBehaviour
{
	/**界面ID*/
	protected int 	m_ScreenID;
	/**是否打开中*/
	protected bool 	m_IsOpen;

	//～～～～～～～～～～～～～～～～～～～～～～～基本方法~～～～～～～～～～～～～～～～～～～～～～～～//
    /// <summary>
    /// 用于取界面控件
    /// </summary>
	public virtual void Awake(){}
    /// <summary>
    /// 初始化界面
    /// </summary>
	public virtual void Start(){}
	public virtual void Update(){}
    /// <summary>
    /// 外部数据传入
    /// </summary>
    public virtual void Show(params object[] info)
    {
    }

	public virtual void OnEnable()
    {
        m_IsOpen = true;
		RegisterEvent();
	}
	public virtual void OnDisable()
	{
        m_IsOpen = false;
		UnRegisterEvent();
	}
    public virtual void OnDestroy()
    {
        
    }
	/**事件*/
	public virtual void RegisterEvent(){}
	public virtual void UnRegisterEvent(){}

	public virtual void SetVisible(bool is_Show)
	{
		if(gameObject != null)
		{
			gameObject.SetActive(is_Show);
		}
	}

    //～～～～～～～～～～～～～～～～～～～～～～～缩放动画~～～～～～～～～～～～～～～～～～～～～～～～//
    /// <summary>
    /// UI按下缩放动画
    /// </summary>
    /// <param name="receive_obj">接收事件对象</param>
    /// <param name="influence_obj">动画作用对象</param>
    /// <param name="time">缩放过程时间</param>
    /// <param name="scale">按下时的缩放比例</param>
    static public void AddPressScaleAnim(GameObject receive_obj, GameObject influence_obj, float time, float scale)
    {
        if (receive_obj == null || influence_obj == null) return;

        UIEventTriggerListener.Get(receive_obj).onDown  = delegate(GameObject go, Vector2 delta) { influence_obj.transform.DOScale(Vector3.one * scale, time); };
        UIEventTriggerListener.Get(receive_obj).onUp    = delegate(GameObject go, Vector2 delta) { influence_obj.transform.DOScale(Vector3.one, time); };
        UIEventTriggerListener.Get(receive_obj).onExit  = delegate(GameObject go, Vector2 delta) { influence_obj.transform.DOScale(Vector3.one, time); }; 
    }
    static public void RemovePressScaleAnim(GameObject receive_obj)
    {
        if (receive_obj == null) return;

        UIEventTriggerListener.Get(receive_obj).onDown = null;
        UIEventTriggerListener.Get(receive_obj).onUp = null;
        UIEventTriggerListener.Get(receive_obj).onExit = null;
    }
	//～～～～～～～～～～～～～～～～～～～～～～～get/set~～～～～～～～～～～～～～～～～～～～～～～～//
	public int screenID
	{ 
		get{ return m_ScreenID;}
		set{ m_ScreenID = value;}
	}
	public bool isOpen
	{ 
		get{ return m_IsOpen;}
		set{ m_IsOpen = value;}
	}
}



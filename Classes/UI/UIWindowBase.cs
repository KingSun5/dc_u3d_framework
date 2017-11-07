using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// UI面板基类
/// GameObject的Activity为true，脚本的enable为true时，其先后顺序为：Awake、OnEnable、Start；
/// GameObject的Activity为true，脚本的enable为false时,只运行Awake；
/// @author hannibal
/// @time 2014-12-4
/// </summary>
public class UIWindowBase : MonoBehaviour
{
	/**界面ID*/
	protected int 	m_ScreenID;
	/**是否打开中*/
	protected bool 	m_IsOpen;

    protected int   m_MaxSortingOrder;
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
    public int MaxSortingOrder
    {
        get { return m_MaxSortingOrder; }
        set { m_MaxSortingOrder = value; }
    }
}



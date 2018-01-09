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
public class UIPanelBase : MonoBehaviour
{
	/**界面ID*/
	protected int 	m_ScreenID;
	/**是否打开中*/
	protected bool 	m_IsOpen;
    /**排序用order*/
    protected int   m_MaxSortingOrder;
	//～～～～～～～～～～～～～～～～～～～～～～～基本方法～～～～～～～～～～～～～～～～～～～～～～～//
    /// <summary>
    /// 用于取界面控件
    /// </summary>
	public virtual void Awake(){}

    /// <summary>
    /// 初始化界面， 外部数据传入
    /// </summary>
    public virtual void Show(params object[] info)
    {
    }

    public virtual void Close()
    {
        UIManager.Instance.Close(m_ScreenID);
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

    /// <summary>
    /// 界面增加子界面
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static GameObject AddComponentFromPrefab(GameObject parent, string path)
    {
        UnityEngine.Object temp = ResourceLoaderManager.Instance.Load(path);
        GameObject root = GameObject.Instantiate(temp) as GameObject;
        root.transform.SetParent(parent.transform, false);
        return root;
    }
    //～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～//
    public void AddUIEventListener(string obj_name, eUIEventType type, UIEventListener.EventDelegate callBack)
    {
        AddUIEventListener(GameObjectUtils.GetChildWithName(obj_name, transform).gameObject, type, callBack);
    }
    public void AddUIEventListener(GameObject obj, eUIEventType type, UIEventListener.EventDelegate callBack)
    {
        if (obj == null)
            return;
        UIEventListener.Get(obj).AddEventListener(type, callBack);
    }
    public void RemoveUIEventListener(string obj_name, eUIEventType type, UIEventListener.EventDelegate callBack)
    {
        RemoveUIEventListener(GameObjectUtils.GetChildWithName(obj_name, transform).gameObject, type, callBack);
    }
    public void RemoveUIEventListener(GameObject obj, eUIEventType type, UIEventListener.EventDelegate callBack)
    {
        if (obj == null)
            return;
        UIEventListener.Get(obj).RemoveEventListener(type, callBack);
    }
	//～～～～～～～～～～～～～～～～～～～～～～～get/set～～～～～～～～～～～～～～～～～～～～～～～//
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



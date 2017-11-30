using UnityEngine;
using System.Collections;

/// <summary>
/// 非弹出子界面，一半是嵌入到UIPanelBase
/// @author hannibal
/// @time 2016-2-5
/// </summary>
public class UIViewBase : MonoBehaviour
{
    //～～～～～～～～～～～～～～～～～～～～～～～基本方法～～～～～～～～～～～～～～～～～～～～～～～//
    /// <summary>
    /// 用于取界面控件
    /// </summary>
    public virtual void Awake() { }

    public virtual void OnEnable()
    {
        RegisterEvent();
    }
    public virtual void OnDisable()
    {
        UnRegisterEvent();
    }
    public virtual void OnDestroy()
    {

    }
    /**事件*/
    public virtual void RegisterEvent() { }
    public virtual void UnRegisterEvent() { }

    public virtual void SetVisible(bool is_Show)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(is_Show);
        }
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
}

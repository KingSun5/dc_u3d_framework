using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public enum EUIEventType
{
    EUIEventType_None = 0,
    EUIEventType_Click =1,
    EUIEventType_Down = 2,
    EUIEventType_Up = 3,
    EUIEventType_Enter = 4,
    EUIEventType_Exit = 5,
    EUIEventType_Select = 6,
    EUIEventType_UpdateSelect = 7,
    EUIEventType_Drag = 8,
    EUIEventType_DragOut = 9,
    EUIEventType_Deselect = 10,

    EUIEventType_Max,
}
public struct UIEventHandler
{
    public EUIEventType eventType;
    public GameObject target;
    public BaseEventData eventData;
}
/// <summary>
/// ui事件
/// @author hannibal
/// @time 2014-10-22
/// </summary>
public class UIEventListener : EventTrigger
{
    public delegate void EventDelegate(UIEventHandler eventHandler);

    public EventDelegate[] m_UIEventCallBackHandler = null;

    public void SetUp()
    {
        if (m_UIEventCallBackHandler == null)
        {
            int nEventNum = (int)EUIEventType.EUIEventType_Max;
            m_UIEventCallBackHandler = new EventDelegate[nEventNum];
        }
    }
    public void Destroy()
    {
        for (int i = 0; i < m_UIEventCallBackHandler.Length; i++)
        {
            m_UIEventCallBackHandler[i] = null;
        }
    }
    static public UIEventListener Get(GameObject go)
    {
        if (go == null) return null;
        UIEventListener listener = go.GetComponent<UIEventListener>();
        if (listener == null)
        {
            listener = go.AddComponent<UIEventListener>();
        }
        listener.SetUp();
        return listener;
    }
    static public void Set(EUIEventType eventType,GameObject go, EventDelegate callBack)
    {
        if (go == null) return ;

        if (eventType == EUIEventType.EUIEventType_None)
        {
            SetAll(go, callBack);
        }
        else
        {
            UIEventListener listener = UIEventListener.Get(go);
            if (listener != null) 
                listener.m_UIEventCallBackHandler[(int)eventType] = callBack;
        }
    }
    static public void SetAll(GameObject go, EventDelegate callBack)
    {
        if (go == null) return;
        UIEventListener listener = UIEventListener.Get(go);
        for (int i = 0; i < (int)EUIEventType.EUIEventType_Max; i++)
        {
            listener.m_UIEventCallBackHandler[i] = callBack;
        }
    }

    public virtual void OnHandler(EUIEventType eventType,BaseEventData eventData)
    {
        EventDelegate currCallHandler = m_UIEventCallBackHandler[(int)eventType];
        if (currCallHandler != null)
        {
            UIEventHandler uiEventHandler = new UIEventHandler();
            uiEventHandler.eventData = eventData;
            uiEventHandler.eventType = eventType;
            uiEventHandler.target = gameObject;
            currCallHandler(uiEventHandler);
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_DragOut,eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_Drag, eventData);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_Click, eventData);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_Down, eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_Enter, eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_Exit, eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_Up, eventData);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_Select, eventData);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_UpdateSelect, eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        OnHandler(EUIEventType.EUIEventType_Deselect, eventData);        
    }
}

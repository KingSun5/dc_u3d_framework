using UnityEngine;
using UnityEngine.UI;
using System.Collections;  
using UnityEngine.EventSystems;  
using System.Collections.Generic; 

/// <summary>
/// ui事件
/// @author hannibal
/// @time 2014-10-22
/// </summary>
public sealed class UIEventListener : EventTrigger
{
    public delegate void EventDelegate(UIEventArgs args);
    public EventDelegate[] UIEventHandleList = new EventDelegate[(int)eUIEventType.Max];
	
	static public UIEventListener Get(GameObject go)
	{
		if (go == null)return null;

		UIEventListener listener = go.GetComponent<UIEventListener>();
		if (listener == null) listener = go.AddComponent<UIEventListener>();
		return listener;
	}

    #region 事件重载
    public override void OnBeginDrag(PointerEventData eventData)
    {
        OnHandler(eUIEventType.BeginDrag, eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        OnHandler(eUIEventType.Drag, eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        OnHandler(eUIEventType.DragOut, eventData);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        OnHandler(eUIEventType.Click, eventData);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        OnHandler(eUIEventType.Down, eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        OnHandler(eUIEventType.Enter, eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        OnHandler(eUIEventType.Exit, eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        OnHandler(eUIEventType.Up, eventData);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        OnHandler(eUIEventType.Select, eventData);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        OnHandler(eUIEventType.UpdateSelect, eventData);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        OnHandler(eUIEventType.Deselect, eventData);
    }
    #endregion

    #region 事件监听
    public void AddEventListener(eUIEventType type, EventDelegate callback)
    {
        this.UIEventHandleList[(int)type] += callback;
    }
    public void RemoveEventListener(eUIEventType type, EventDelegate callback)
    {
        this.UIEventHandleList[(int)type] -= callback;
    }
    public void ClearEventListener(eUIEventType type)
    {
        this.UIEventHandleList[(int)type] = null;
    }
    #endregion

    private void OnHandler(eUIEventType type, BaseEventData eventData)
    {
        if (!Interactable) return;

        EventDelegate handle = UIEventHandleList[(int)type];
        if (handle != null)
        {
            UIEventArgs args = new UIEventArgs();
            args.type = type;
            args.target = gameObject;
            args.data = eventData;
            handle(args);
        }
    }
    private bool Interactable
    {
        get
        {
            var selectable = gameObject.GetComponent<Selectable>();
            return selectable == null ? true : selectable.interactable;
        }
    }
}

/// <summary>
/// 事件参数
/// </summary>
public struct UIEventArgs
{
    public eUIEventType type;
    public GameObject target;
    public BaseEventData data;
}

/// <summary>
/// ui事件类型
/// </summary>
public enum eUIEventType
{
    Click = 1,
    Down,
    Up,
    Enter,
    Exit,
    Select,
    UpdateSelect,
    BeginDrag,
    Drag,
    DragOut,
    Deselect,

    Max,
}

/** 使用方式
	Button	button;
	Image image;
	void Start () 
	{
		button = transform.Find("Button").GetComponent<Button>();
		image = transform.Find("Image").GetComponent<Image>();
		UIEventListener.Get(button.gameObject).onClick =OnButtonClick;
		UIEventListener.Get(image.gameObject).onClick =OnButtonClick;
	}
 
	private void OnButtonClick(GameObject go)
	{
		//在这里监听按钮的点击事件
		if(go == button.gameObject)
		{
			Log.Info ("DoSomeThings");
		}
	}

 */

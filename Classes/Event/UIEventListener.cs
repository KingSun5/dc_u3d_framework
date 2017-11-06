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
public class UIEventListener : EventTrigger
{
	public delegate void VoidDelegate(GameObject go);
	public delegate void VectorDelegate(GameObject go, Vector2 delta);

    public VectorDelegate   onClick;
    public VectorDelegate   onDown;
    public VectorDelegate   onEnter;
    public VectorDelegate   onExit;
    public VectorDelegate   onUp;
	public VoidDelegate     onSelect;
	public VoidDelegate     onUpdateSelect;

    public VectorDelegate   onDragStart;
	public VectorDelegate   onDrag;
	public VoidDelegate     onDragEnd; 
	
	static public UIEventListener Get (GameObject go)
	{
		if (go == null)return null;

		UIEventListener listener = go.GetComponent<UIEventListener>();
		if (listener == null) listener = go.AddComponent<UIEventListener>();
		return listener;
	}
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onDragStart != null) onDragStart(gameObject, eventData.delta);
    }
	public override void OnDrag(PointerEventData eventData)  
	{  
		if(onDrag != null) onDrag(gameObject, eventData.delta); 		
	}
	public override void OnEndDrag(PointerEventData eventData)  
	{
        if (onDragEnd != null) onDragEnd(gameObject);  
	}
	public override void OnPointerClick(PointerEventData eventData)
	{
        if (onClick != null) onClick(gameObject, eventData.position);
	}
	public override void OnPointerDown (PointerEventData eventData)
	{
        if (onDown != null) onDown(gameObject, eventData.position);
	}
	public override void OnPointerEnter (PointerEventData eventData)
	{
        if (onEnter != null) onEnter(gameObject, eventData.position);
	}
	public override void OnPointerExit (PointerEventData eventData)
	{
        if (onExit != null) onExit(gameObject, eventData.position);
	}
	public override void OnPointerUp (PointerEventData eventData)
	{
        if (onUp != null) onUp(gameObject, eventData.position);
	}
	public override void OnSelect (BaseEventData eventData)
	{
		if(onSelect != null) onSelect(gameObject);
	}
	public override void OnUpdateSelected (BaseEventData eventData)
	{
		if(onUpdateSelect != null) onUpdateSelect(gameObject);
	}
}

/** 使用方式
	Button	button;
	Image image;
	void Start () 
	{
		button = transform.Find("Button").GetComponent<Button>();
		image = transform.Find("Image").GetComponent<Image>();
		EventTriggerListener.Get(button.gameObject).onClick =OnButtonClick;
		EventTriggerListener.Get(image.gameObject).onClick =OnButtonClick;
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

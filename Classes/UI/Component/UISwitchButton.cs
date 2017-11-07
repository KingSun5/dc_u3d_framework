using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI状态按钮
/// @author hannibal
/// @time 2017-2-25
/// </summary>
[RequireComponent(typeof(Image))]
public class UISwitchButton : UIComponentBase
{
    public enum Status
    {
        Normal,
        Select,
    }
    public bool AutoSwitch = true;  //是否自动切换状态
    public Status BtnStatus = Status.Normal;
    
    public Sprite NormalBtn;
    public Sprite SelectBtn;


    private Image ImgComponent;

    public override void Awake()
    {
        if (NormalBtn == null) Log.Error("没有设置按钮基础状态");
        ImgComponent = GetComponent<Image>();
    }

    public override void OnEnable()
    {
        SetStatus(BtnStatus);
        base.OnEnable();
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void RegisterEvent()
    {
        this.AddUIEventListener(gameObject, eUIEventType.Click, OnClick);
    }
    public override void UnRegisterEvent()
    {
        this.RemoveUIEventListener(gameObject, eUIEventType.Click, OnClick);
    }

    void OnClick(UIEventArgs args)
    {
        if (!AutoSwitch) return;
        switch (BtnStatus)
        {
            case Status.Normal:
                SetStatus(Status.Select);
                break;

            case Status.Select:
                SetStatus(Status.Normal);
                break;
        }
    }

    public void SetStatus(Status status)
    {
        switch (status)
        {
            case Status.Normal:
                if (NormalBtn != null) ImgComponent.sprite = NormalBtn;
                BtnStatus = status;
                break;

            case Status.Select:
                if (SelectBtn != null) ImgComponent.sprite = SelectBtn;
                BtnStatus = status;
                break;
        }
    }
}

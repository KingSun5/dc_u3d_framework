using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI按钮
/// @author hannibal
/// @time 2017-2-25
/// </summary>
[RequireComponent(typeof(Image))]
public class UIButton : UIComponentBase
{
    public enum Status
    {
        Normal,
        Select,
        Disable,
    }
    public Status BtnStatus = Status.Normal;

    public Sprite NormalBtn;
    public Sprite SelectBtn;
    public Sprite DisableBtn;

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
        switch(status)
        {
            case Status.Normal:
                if (NormalBtn != null) ImgComponent.sprite = NormalBtn;
                BtnStatus = status;
                break;

            case Status.Select:
                if (SelectBtn != null) ImgComponent.sprite = SelectBtn;
                BtnStatus = status;
                break;

            case Status.Disable:
                if (DisableBtn != null) ImgComponent.sprite = DisableBtn;
                BtnStatus = status;
                break;
        }
    }
}

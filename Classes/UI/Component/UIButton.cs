using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI按钮
/// @author hannibal
/// @time 2017-2-25
/// </summary>
[RequireComponent(typeof(Image))]
public class UIButton : MonoBehaviour
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

    void Awake()
    {
        if (NormalBtn == null) Log.Error("没有设置按钮基础状态");
        ImgComponent = GetComponent<Image>();
    }

    void OnEnable()
    {
        SetStatus(BtnStatus);
        RegisterEvent();
    }
    void OnDisable()
    {
        UnRegisterEvent();
    }

    void RegisterEvent()
    {
        UIEventListener.Get(gameObject).onClick += OnClick;
    }
    void UnRegisterEvent() 
    {
        UIEventListener.Get(gameObject).onClick -= OnClick;
    }

    void OnClick(GameObject go, Vector2 delta)
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

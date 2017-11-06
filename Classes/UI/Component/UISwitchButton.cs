using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI状态按钮
/// @author hannibal
/// @time 2017-2-25
/// </summary>
[RequireComponent(typeof(Image))]
public class UISwitchButton : MonoBehaviour
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

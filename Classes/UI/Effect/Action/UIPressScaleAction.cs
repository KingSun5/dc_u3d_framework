using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

/// <summary>
/// 按钮按下的缩放动画
/// @author hannibal
/// @time 2017-2-25
/// </summary>
public class UIPressScaleAction : MonoBehaviour
{
    public float time = 1;
    public float scale = 0.8f;

    public GameObject InfluenceObject;

    void Awake()
    {
    }

    void OnEnable()
    {
        RegisterEvent();
    }
    void OnDisable()
    {
        UnRegisterEvent();
    }

    void RegisterEvent()
    {
        UIEventListener.Get(gameObject).AddEventListener(eUIEventType.Down, OnDown);
        UIEventListener.Get(gameObject).AddEventListener(eUIEventType.Up, OnUp);
        UIEventListener.Get(gameObject).AddEventListener(eUIEventType.Exit, OnExit);
    }
    void UnRegisterEvent()
    {
        UIEventListener.Get(gameObject).RemoveEventListener(eUIEventType.Down, OnDown);
        UIEventListener.Get(gameObject).RemoveEventListener(eUIEventType.Up, OnUp);
        UIEventListener.Get(gameObject).RemoveEventListener(eUIEventType.Exit, OnExit);
    }

    void OnDown(UIEventArgs evt)
    {
        GameObject influence_obj = InfluenceObject != null ? InfluenceObject : gameObject;
        influence_obj.transform.DOScale(Vector3.one * scale, time);
    }
    void OnUp(UIEventArgs evt)
    {
        GameObject influence_obj = InfluenceObject != null ? InfluenceObject : gameObject;
        influence_obj.transform.DOScale(Vector3.one, time); 
    }
    void OnExit(UIEventArgs evt)
    {
        GameObject influence_obj = InfluenceObject != null ? InfluenceObject : gameObject;
        influence_obj.transform.DOScale(Vector3.one, time);
    }
}

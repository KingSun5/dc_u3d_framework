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
        UIEventTriggerListener.Get(gameObject).onDown += OnDown;
        UIEventTriggerListener.Get(gameObject).onUp += OnUp;
        UIEventTriggerListener.Get(gameObject).onExit += OnExit;
    }
    void UnRegisterEvent()
    {
        UIEventTriggerListener.Get(gameObject).onDown -= OnDown;
        UIEventTriggerListener.Get(gameObject).onUp -= OnUp;
        UIEventTriggerListener.Get(gameObject).onExit -= OnExit;
    }

    void OnDown(GameObject go, Vector2 delta)
    {
        GameObject influence_obj = InfluenceObject != null ? InfluenceObject : gameObject;
        influence_obj.transform.DOScale(Vector3.one * scale, time);
    }
    void OnUp(GameObject go, Vector2 delta)
    {
        GameObject influence_obj = InfluenceObject != null ? InfluenceObject : gameObject;
        influence_obj.transform.DOScale(Vector3.one, time); 
    }
    void OnExit(GameObject go, Vector2 delta)
    {
        GameObject influence_obj = InfluenceObject != null ? InfluenceObject : gameObject;
        influence_obj.transform.DOScale(Vector3.one, time);
    }
}

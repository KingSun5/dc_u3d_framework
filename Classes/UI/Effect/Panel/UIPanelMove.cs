using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

/// <summary>
/// UI界面移动
/// @author hannibal
/// @time 2016-3-26
/// </summary>
public class UIPanelMove : UIPanelAnimation
{
    public Vector3 m_FromPositon;
    public Vector3 m_ToPosition;
    private RectTransform rectTransform;
    public override void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = m_FromPositon;
        m_CurTick = Time.time + m_Delay;
    }

    public override void Start()
    {
    }

    private float m_CurTick;
    public override void Update()
    {
        if (m_CurTick <= Time.time && m_CurTick != -1.0f)
        {
            PlayForward();
            m_CurTick = -1.0f;
        }
    }

    public override void OnEnable()
    {
    }

    public override void OnDisable()
    {
        Reset();
    }

    public void Reset()
    {
        m_CurTick = Time.time + m_Delay;
        rectTransform.anchoredPosition = m_FromPositon;
    }

    public void PlayForward()
    {
        m_Running = true;
        Tweener tweener = rectTransform.DOAnchorPos3D(m_ToPosition, m_Duration);
        tweener.SetEase(m_easeType);
        tweener.OnComplete(() =>
        {
            m_Running = false;
        });
    }

    public override void Rollback()
    {
        m_Running = true;
        Tweener tweener = rectTransform.DOAnchorPos3D(m_FromPositon, m_Duration);
        tweener.SetEase(m_easeType);
        tweener.OnComplete(() =>
        {
            m_Running = false;
        });
    }
}

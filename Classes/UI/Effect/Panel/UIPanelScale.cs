using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

/// <summary>
/// 缩放
/// @author hannibal
/// @time 2016-3-26
/// </summary>
public class UIPanelScale : UIPanelAnimation
{
    public Vector3 m_FromScale;
    public Vector3 m_ToScale;

    public override void Awake()
    {
        gameObject.transform.localScale = m_FromScale;
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
        gameObject.transform.localScale = m_FromScale;
    }

    public void PlayForward()
    {
        gameObject.transform.DOScale(m_ToScale, m_Duration).SetEase(m_easeType);
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

/// <summary>
/// UI界面alpha渐变
/// @author hannibal
/// @time 2017-3-26
/// </summary>
public class UIPanelFade : UIPanelAnimation
{
    public float m_ToAlpha;
    public float m_FromAlpha;

    private CanvasGroup m_CanvasGroup;

    public override void Awake()
    {
        if (m_CanvasGroup == null)
        {
            m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
        }
        if (m_CanvasGroup == null)
        {
            m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        m_CurTick = Time.time + m_Delay;
        m_CanvasGroup.alpha = m_FromAlpha;
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
        m_CanvasGroup.DOFade(m_FromAlpha, 0);
    }

    public void PlayForward()
    {
        m_CanvasGroup.DOFade(m_ToAlpha, m_Duration).SetEase(m_easeType);
    }
}


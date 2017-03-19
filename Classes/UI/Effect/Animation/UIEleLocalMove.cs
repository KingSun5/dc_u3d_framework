using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class UIEleLocalMove : UIEleAnimation
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
        rectTransform.DOAnchorPos3D(m_ToPosition, m_Duration).SetEase(m_easeType);
    }
}

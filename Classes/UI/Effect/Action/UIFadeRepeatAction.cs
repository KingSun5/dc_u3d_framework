using UnityEngine;
using System.Collections;

/// <summary>
/// 闪烁
/// @author hannibal
/// @time 2017-3-26
/// </summary>
public class UIFadeRepeatAction : MonoBehaviour 
{
    public float m_ToAlpha = 0;
    public float m_Duration = 1;
    public bool m_Repeat = true;

    private bool m_Active = true;

    void OnFadeOut()
    {
        if (!m_Active) return;
        UIEffectTools.FadeIn(gameObject, m_Duration, OnFadeIn, 1);
    }

    void OnFadeIn()
    {
        if (!m_Active) return;
        UIEffectTools.FadeOut(gameObject, m_Duration, OnFadeOut, m_ToAlpha);
    }

    public void Start()
    {
        if (m_Active) return;
        m_Active = true;
        OnFadeIn();
    }

    public void Stop(float alpha)
    {
        if (!m_Active) return;
        m_Active = false;
        UIEffectTools.FadeStop(gameObject);
        UIEffectTools.FadeIn(gameObject, 0, null, alpha);
    }
}

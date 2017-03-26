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

	void Start () 
    {
        OnFadeIn();
	}

    void OnFadeOut()
    {
        UIEffectTools.FadeIn(gameObject, m_Duration, OnFadeIn);
    }

    void OnFadeIn()
    {
        UIEffectTools.FadeOut(gameObject, m_Duration, OnFadeOut);
    }
}

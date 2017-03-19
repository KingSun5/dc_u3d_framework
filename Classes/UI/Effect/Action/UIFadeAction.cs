using UnityEngine;
using System.Collections;

public class UIFadeAction : MonoBehaviour 
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

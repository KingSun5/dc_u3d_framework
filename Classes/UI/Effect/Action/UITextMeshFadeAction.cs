using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]

/// <summary>
/// alpha渐变
/// @author hannibal
/// @time 2016-3-19
/// </summary>
public class UITextMeshFadeAction : MonoBehaviour 
{
    public float m_ToAlpha = 0;
    public float m_Duration = 1;
    public bool m_Repeat = true;

    private TextMesh m_MeshComponet;
    private Color m_InitColor;

    private float m_StartTime;
    private bool m_IsFadeIn = false;

	void Start () 
    {
        m_MeshComponet = GetComponent<TextMesh>();
        m_InitColor = m_MeshComponet.color;
        if (m_Duration == 0) m_Duration = 0.01f;
        OnFadeOut();
	}

    void OnFadeOut()
    {
        m_StartTime = Time.realtimeSinceStartup;
        m_IsFadeIn = false;
    }

    void OnFadeIn()
    {
        m_StartTime = Time.realtimeSinceStartup;
        m_IsFadeIn = true;
    }

    void Update()
    {
        if (m_IsFadeIn && !m_Repeat) return;

        if (Time.realtimeSinceStartup - m_StartTime >= m_Duration)
        {
            if (m_IsFadeIn) OnFadeOut();
            else OnFadeIn();
            return;
        }
        float time_scale = (Time.realtimeSinceStartup - m_StartTime) / m_Duration;
        float alpha = 1;
        if(m_IsFadeIn)
            alpha = m_ToAlpha + (m_InitColor.a - m_ToAlpha) * time_scale;
        else
            alpha = m_InitColor.a + (m_ToAlpha - m_InitColor.a) * time_scale;
        m_MeshComponet.color = new Color(m_InitColor.r, m_InitColor.g, m_InitColor.b, alpha);
    }
}

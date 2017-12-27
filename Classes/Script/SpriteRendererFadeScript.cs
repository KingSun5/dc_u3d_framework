using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// sprite渐变动画
/// @author hannibal
/// @time 2017-12-28
/// </summary>
public class SpriteRendererFadeScript : MonoBehaviour 
{
    public enum eFadeType { In, Out };

    public eFadeType m_FadeType = eFadeType.In;
    public float m_FadeTime = 1;
    public float m_DelayTime = 0;

    private bool m_Active = false;
    private float m_Alpha = 0f;
    private SpriteRenderer[] m_Images = null;

    void Start()
    {
        this.Invoke("OnAcitve", m_DelayTime);

        m_Images = gameObject.GetComponentsInChildren<SpriteRenderer>();
        if (m_FadeType == eFadeType.In)
        {
            m_Alpha = 0f;
            foreach (SpriteRenderer vRenderer in m_Images)
            {
                vRenderer.color = new Color(vRenderer.color.r, vRenderer.color.g, vRenderer.color.b, 0);
            }
        }
        else
        {
            m_Alpha = 1f;
            foreach (SpriteRenderer vRenderer in m_Images)
            {
                vRenderer.color = new Color(vRenderer.color.r, vRenderer.color.g, vRenderer.color.b, 1);
            }
        }
    }

    void Update()
    {
        if (!m_Active) return;

        foreach (SpriteRenderer vRenderer in m_Images)
        {
            vRenderer.color = new Color(vRenderer.color.r, vRenderer.color.g, vRenderer.color.b, m_Alpha);
        }
        float offset = (Time.deltaTime) / m_FadeTime;
        if (m_FadeType == eFadeType.In)
        {
            m_Alpha += offset;
        }
        else
        {
            m_Alpha -= offset;
        }
        m_Alpha = MathUtils.Clamp(m_Alpha, 0, 1);
    }

    void OnAcitve()
    {
        m_Active = true;
    }
}

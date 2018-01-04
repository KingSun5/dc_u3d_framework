using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// tk2d sprite渐变动画
/// @author hannibal
/// @time 2017-12-29
/// </summary>
public class tk2dSpriteFadeTransformer : Transformer
{
    private enum eFadeType { In, Out };
    private eFadeType m_FadeType;
    private float m_StartAlpha;
    private float m_SpeedAlpha;
    private float m_TargetAlpha;

    private tk2dSprite[] m_Images = null;

    public static tk2dSpriteFadeTransformer FadeIn(GameObject target, float destAlpha, float time)
    {
        destAlpha = Mathf.Clamp(destAlpha, 0, 1);
        tk2dSpriteFadeTransformer transformer = new tk2dSpriteFadeTransformer();
        transformer.m_FadeType = eFadeType.In;
        transformer.m_TargetAlpha = destAlpha;
        transformer.m_fTransformTime = time;
        transformer.target = target;
        return transformer;
    }
    public static tk2dSpriteFadeTransformer FadeOut(GameObject target, float destAlpha, float time)
    {
        destAlpha = Mathf.Clamp(destAlpha, 0, 1);
        tk2dSpriteFadeTransformer transformer = new tk2dSpriteFadeTransformer();
        transformer.m_FadeType = eFadeType.Out;
        transformer.m_TargetAlpha = destAlpha;
        transformer.m_fTransformTime = time;
        transformer.target = target;
        return transformer;
    }
    public tk2dSpriteFadeTransformer()
    {
        m_Type = eTransformerID.SpriteFade;
    }
    public override void OnTransformStarted()
    {
        m_Images = target.GetComponentsInChildren<tk2dSprite>();

        if (m_FadeType == eFadeType.In)
        {
            m_StartAlpha = 0f;
            m_SpeedAlpha = (m_TargetAlpha) / m_fTransformTime;
            this.SetAlpha(0);
        }
        else if (m_FadeType == eFadeType.Out)
        {
            m_StartAlpha = 1f;
            m_SpeedAlpha = -(m_StartAlpha - m_TargetAlpha) / m_fTransformTime;
            this.SetAlpha(1);
        }
    }
    public override void runTransform(float currTime)
    {
        if (m_Images == null || m_Images.Length == 0) return;

        float alpha = 1;
        if (currTime >= m_fEndTime)
        {
            alpha = m_TargetAlpha;
        }
        else
        {
            float timeElapased = currTime - m_fStartTime;
            alpha = (m_StartAlpha + m_SpeedAlpha * timeElapased);
        }
        this.SetAlpha(alpha);
	}
    private void SetAlpha(float alpha)
    {
        alpha = Mathf.Clamp(alpha, 0, 1);
        if (m_Images != null && m_Images.Length > 0)
        {
            foreach (tk2dSprite vRenderer in m_Images)
            {
                if (vRenderer == null) continue;
                vRenderer.color = new Color(vRenderer.color.r, vRenderer.color.g, vRenderer.color.b, alpha);
            }
        }
    }
}

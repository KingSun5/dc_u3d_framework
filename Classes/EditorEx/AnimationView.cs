using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]

/// <summary>
/// 动画查看器
/// @author hannibal
/// @time 2017-11-8
/// </summary>
public class AnimationView : MonoBehaviour 
{
    private Animation m_Animations;
    private List<string> m_ListAnimation = new List<string>();

    void Awake()
    {
        m_Animations = this.GetComponent<Animation>();
        foreach (AnimationState state in m_Animations)
        {
            m_ListAnimation.Add(state.name);
        }
    }

    void Start()
    {

    }

    void OnGUI()
    {
        float start_x = 50;
        float start_y = 50;
        float w = 120;
        float h = 40;
        float x = 0;
        float y = 0;
        float cols = 0;
        float rows = 0;
        int i = 0;
        for (i = 0; i < m_ListAnimation.Count; ++i)
        {
            x = start_x;
            y = start_y + (i - cols * rows) * 50;
            if (y + h > UIID.DEFAULT_HEIGHT)
            {
                cols++;
                start_x += (w + 20);
                x = start_x;
                y = start_y;
                if (rows == 0) rows = i;
            }
            if (GUI.Button(new Rect(x * UIID.ScreenScaleX, y * UIID.ScreenScaleY, w * UIID.ScreenScaleX, h * UIID.ScreenScaleY), m_ListAnimation[i]))
            {
                string anim = m_ListAnimation[i];
                m_Animations.Play(anim);
            }
        }
    }
}

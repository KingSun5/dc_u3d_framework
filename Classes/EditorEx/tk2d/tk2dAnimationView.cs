using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画查看器
/// @author hannibal
/// @time 2017-11-8
/// </summary>
public class tk2dAnimationView : MonoBehaviour
{
    private List<string> m_ListAnimation = new List<string>();

    void Awake()
    {
        tk2dSpriteAnimator animator = this.GetComponentInChildren<tk2dSpriteAnimator>();
        if (animator != null)
        {
            foreach (tk2dSpriteAnimationClip clip in animator.Library.clips)
            {
                if (string.IsNullOrEmpty(clip.name)) continue;
                m_ListAnimation.Add(clip.name);
            }
        }
    }

	void Start () 
    {
		
	}
	
	void OnGUI ()
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
                tk2dSpriteAnimator animator = this.GetComponentInChildren<tk2dSpriteAnimator>();
                animator.Play(anim);
            }
        }
	}
}
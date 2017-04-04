using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// UI动画效果工具类
/// @author hannibal
/// @time 2016-12-2
/// </summary>
public class UIEffectTools 
{
    //～～～～～～～～～～～～～～～～～～～～～～～缩放动画~～～～～～～～～～～～～～～～～～～～～～～～//
    /// <summary>
    /// UI按下缩放动画
    /// </summary>
    /// <param name="receive_obj">接收事件对象</param>
    /// <param name="influence_obj">动画作用对象</param>
    /// <param name="time">缩放过程时间</param>
    /// <param name="scale">按下时的缩放比例</param>
    static public void AddPressScaleAnim(GameObject receive_obj, GameObject influence_obj, float time, float scale)
    {
        if (receive_obj == null || influence_obj == null) return;

        UIEventTriggerListener.Get(receive_obj).onDown = delegate(GameObject go, Vector2 delta) { influence_obj.transform.DOScale(Vector3.one * scale, time); };
        UIEventTriggerListener.Get(receive_obj).onUp = delegate(GameObject go, Vector2 delta) { influence_obj.transform.DOScale(Vector3.one, time); };
        UIEventTriggerListener.Get(receive_obj).onExit = delegate(GameObject go, Vector2 delta) { influence_obj.transform.DOScale(Vector3.one, time); };
    }
    static public void RemovePressScaleAnim(GameObject receive_obj)
    {
        if (receive_obj == null) return;

        UIEventTriggerListener.Get(receive_obj).onDown = null;
        UIEventTriggerListener.Get(receive_obj).onUp = null;
        UIEventTriggerListener.Get(receive_obj).onExit = null;
    }
    //～～～～～～～～～～～～～～～～～～～～～～～渐隐动画~～～～～～～～～～～～～～～～～～～～～～～～//
    public static void FadeIn(GameObject go, float time, System.Action fun = null, float alpha = 1)
    {
        bool is_trigger = false;
        Component[] comps = go.GetComponentsInChildren<Component>();
        for (int index = 0; index < comps.Length; index++)
        {
            Component c = comps[index];
            if (c is Graphic)
            {
                (c as Graphic).
                    DOFade(alpha, time)
                    .OnComplete(() =>
                    {
                        if (fun != null && is_trigger == false)
                        {
                            fun();
                        } 
                        is_trigger = true;
                    });
            }
        }
    }
    public static void FadeOut(GameObject go, float time, System.Action fun = null, float alpha=0)
    {
        bool is_trigger = false;
        Component[] comps = go.GetComponentsInChildren<Component>();
        for (int index = 0; index < comps.Length; index++)
        {
            Component c = comps[index];
            if (c is Graphic)
            {
                (c as Graphic).
                    DOFade(alpha, time)
                    .OnComplete(() =>
                    {
                        if (fun != null && is_trigger == false)
                        {
                            fun();
                        }
                        is_trigger = true;
                    });
            }
        }
    }
    public static void FadeStop(GameObject go)
    {
        Component[] comps = go.GetComponentsInChildren<Component>();
        for (int index = 0; index < comps.Length; index++)
        {
            Component c = comps[index];
            if (c is Graphic)
            {
                (c as Graphic).DOKill();
            }
        }
    } 
}

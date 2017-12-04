using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 屏蔽界面事件
/// @author hannibal
/// @time 2016-10-25
/// </summary>
public class UIFocusScript : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsFocus = false;
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return IsFocus;
    }
}
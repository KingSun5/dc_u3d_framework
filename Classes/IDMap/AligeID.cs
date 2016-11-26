using UnityEngine;
using System.Collections;

/// <summary>
/// 方向
/// @author hannibal
/// @time 2015-1-6
/// </summary>

[System.Serializable]
public enum eHorizontalType
{
    LEFT,
    CENTER,
    RIGHT,
}

[System.Serializable]
public enum eAligeType
{
    NONE = 0,
    RIGHT,
    RIGHT_BOTTOM,
    BOTTOM,
    LEFT_BOTTOM,
    LEFT,
    LEFT_TOP,
    TOP,
    RIGHT_TOP,
    MID,
}

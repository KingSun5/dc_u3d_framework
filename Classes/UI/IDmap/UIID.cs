using UnityEngine;
using System.Collections;

/// <summary>
/// ui
/// @author hannibal
/// @time 2016-9-21
/// </summary>
public class UIID
{
	public const float DEFAULT_WIDTH 	= 750;  //标准界面大小
	public const float DEFAULT_HEIGHT 	= 1334;

    public static float ScreenScaleX    = 1;    //界面缩放
    public static float ScreenScaleY    = 1;
    public static float InvScreenScaleX = 1;    
    public static float InvScreenScaleY = 1;

    /*不同节点下的sortLayer层数*/
    public const int ORDERLAYERINTERVAL = 1000;

}
/// <summary>
/// 界面layer
/// </summary>
public enum eUILayer
{
    BACK = 0,
    TOOLS,
    APP,
    TOP,
    MAX,
}

/// <summary>
/// 界面id
/// </summary>
public enum eInternalUIID
{
    ID_ALERT,           // 弹出框

    ID_MAX,
}
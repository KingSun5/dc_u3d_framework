using UnityEngine;
using System.Collections;

/// <summary>
/// 全局配置
/// @author hannibal
/// @time 2014-11-11
/// </summary>
public class GlobalID
{
    public static uint  StartSceneID = 0;            //启动场景id      
    public static bool  IsSingleGame = false;        //是否单机模式
    public static bool  IsDebugNet = true;           //是否显示通信协议号
}
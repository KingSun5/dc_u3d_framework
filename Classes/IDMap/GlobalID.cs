using UnityEngine;
using System.Collections;

/// <summary>
/// 全局配置
/// @author hannibal
/// @time 2014-11-11
/// </summary>
public class GlobalID
{
    public static bool IsSingleGame = false;        //是否单机模式
    public static bool IsCreateMainPlayer = false;  //单机模式下，是否由代码创建主角
    public static bool IsShowMonster = false;       //是否显示怪物
    public static bool IsUseUnityScene = false;     //是否启用unity场景

    public static bool IsLogNet = false;            //是否显示通信协议号日志
    public static bool IsLogLoad = false;           //是否显示加载日志
    public static bool IsLogBuild = false;          //是否显示对象构建日志

    public static int FPS = 24;                    //fps

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
    public static string RootSDCard = Application.persistentDataPath;
#elif UNITY_ANDROID
    public static string RootSDCard = "/sdcard/HugenStar/fps";
#elif UNITY_IPHONE
    public static string RootSDCard = Application.persistentDataPath;
#else
    public static string RootSDCard = Application.persistentDataPath;
#endif
}
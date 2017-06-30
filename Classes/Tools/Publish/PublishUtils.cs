using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublishUtils
{
    /// <summary>
    /// 平台类型转名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetPlatformNameByType(ePublishPlatformType type)
    {
        switch (type)
        {
            case ePublishPlatformType.Android: return "Android";
            case ePublishPlatformType.iOS: return "IOS";
            case ePublishPlatformType.Win: return "Win";
            case ePublishPlatformType.WebGL: return "WebGL";
        }
        return string.Empty;
    }
    /// <summary>
    /// 发布目录
    /// </summary>
    /// <returns></returns>
    public static string GetPublishPath(ePublishPlatformType type)
    {
        string target_dir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
        switch (type)
        {
            case ePublishPlatformType.Android: target_dir = target_dir + "/build/Android"; break;
            case ePublishPlatformType.iOS: target_dir = target_dir + "/build/IOS"; break;
            case ePublishPlatformType.Win: target_dir = target_dir + "/build/Win"; break;
            case ePublishPlatformType.WebGL: target_dir = target_dir + "/build/WebGL"; break;
        }
        return target_dir;
    }
    /// <summary>
    /// 读取平台配置表
    /// </summary>
    /// <returns></returns>
    public static PublishPlatformCollection ReadPlatformConfig()
    {
        TextAsset textAsset = Resources.Load(PublishID.ResourcePlatformPath) as TextAsset;
        PublishPlatformCollection platform_coll = JsonUtility.FromJson<PublishPlatformCollection>(textAsset.text);
        return platform_coll;
    }
}

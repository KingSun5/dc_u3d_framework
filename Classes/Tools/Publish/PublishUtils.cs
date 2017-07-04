using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            case ePublishPlatformType.Android:  return "Android";
            case ePublishPlatformType.iOS:      return "IOS";
            case ePublishPlatformType.Win64:    return "Win64";
            case ePublishPlatformType.Win32:    return "Win32";
            case ePublishPlatformType.WebGL:    return "WebGL";
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
            case ePublishPlatformType.Android:  target_dir = target_dir + "/build/Android"; break;
            case ePublishPlatformType.iOS:      target_dir = target_dir + "/build/IOS"; break;
            case ePublishPlatformType.Win64:    target_dir = target_dir + "/build/Win64"; break;
            case ePublishPlatformType.Win32:    target_dir = target_dir + "/build/Win32"; break;
            case ePublishPlatformType.WebGL:    target_dir = target_dir + "/build/WebGL"; break;
        }
        return target_dir;
    }
    /// <summary>
    /// 配置表全路径
    /// </summary>
    public static string GetPublishConfigPath()
    {
        return Path.Combine(Application.dataPath, PublishID.PlatformConfigPath+"/"+PublishID.PlatformConfigFile);
    }
    /// <summary>
    /// 配置表目录
    /// </summary>
    public static string GetPublishConfigDir()
    {
        return Path.Combine(Application.dataPath, PublishID.PlatformConfigPath);
    }
    /// <summary>
    /// 读取平台配置表
    /// </summary>
    /// <returns></returns>
    public static PublishPlatformCollection ReadPlatformConfig()
    {
        try
        {
            string resFilePath = GetPublishConfigPath();
            string str_text = File.ReadAllText(resFilePath);
            PublishPlatformCollection platform_coll = JsonUtility.FromJson<PublishPlatformCollection>(str_text);
            return platform_coll;
        }
        catch (Exception e)
        {
            Log.Error("读取配置表错误:" + e.Message);
            return null;
        }
    }
    /// <summary>
    /// 写入配置表数据
    /// </summary>
    public static void WritePlatformConfig(PublishPlatformCollection data)
    {
        string jsonStr = JsonUtility.ToJson(data);
        try
        {
            string resFilePath = GetPublishConfigPath();
            using (FileStream resfs = new FileStream(resFilePath, FileMode.Create))
            {
                using (StreamWriter resSw = new StreamWriter(resfs, System.Text.Encoding.UTF8))
                {
                    resSw.Write(jsonStr);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("保存配置表错误:" + e.Message);
        }
    }
}

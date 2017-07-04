using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 发布管理器
/// @author hannibal
/// @time 2017-6-30
/// </summary>
public class PublishManager : Singleton<PublishManager>
{
    private PublishPlatformCollection m_PlatformConfig = null;
    private PublishCachePlatformSet m_CachePlatformInfo = new PublishCachePlatformSet();
    private PublishCacheChannelSet m_CacheChannelInfo = new PublishCacheChannelSet();

    //缓存界面数据
    public int OrientationIndex = 3;
    public int RenderPathIndex = 2;
    public int GraphicsAPIAndIndex = 1;
    public int GraphicsAPIIOSIndex = 1;
    public int GraphicsAPIWinIndex = 1;
    public int GraphicsAPIWebIndex = 0;
    public int ScriptBackemdIndex = 1;
    public int NetLevelIndex = 1;
    public int AndroidDeviceIndex = 1;
    public int IOSDeviceIndex = 2;
    public int IOSSDKIndex = 0;
    public int IOSOptLevelIndex = 0;
    public int InstallLocationIndex = 2;
    public int MinAndroidSDK = 1;
    public int StripLevel = 3;

    public void Setup()
    {
        m_PlatformConfig = PublishUtils.ReadPlatformConfig();
    }

    public void Destroy()
    {

    }

    public void ResetData()
    {
        m_CachePlatformInfo = new PublishCachePlatformSet();
        m_CacheChannelInfo = new PublishCacheChannelSet();
        m_PlatformConfig = PublishUtils.ReadPlatformConfig();

        OrientationIndex = 3;
        RenderPathIndex = 2;
        GraphicsAPIAndIndex = 1;
        GraphicsAPIIOSIndex = 1;
        GraphicsAPIWinIndex = 1;
        GraphicsAPIWebIndex = 0;
        ScriptBackemdIndex = 1;
        NetLevelIndex = 1;
        AndroidDeviceIndex = 1;
        IOSDeviceIndex = 2;
        IOSSDKIndex = 0;
        IOSOptLevelIndex = 0;
        InstallLocationIndex = 2;
        MinAndroidSDK = 1;
        StripLevel = 3;
    }
    /// <summary>
    /// 发布完成，清理工作
    /// </summary>
    public void OnPublishComplete()
    {
        PublishUtils.WritePlatformConfig(m_PlatformConfig);
    }

    /// <summary>
    /// 获取对应平台配置表数据
    /// </summary>
    /// <param name="type">平台类型</param>
    /// <returns></returns>
    public PublishPlatformSet GetPlatformConfig(ePublishPlatformType type)
    {
        foreach(var obj in m_PlatformConfig.plats)
        {
            if (obj.type == (int)type) return obj;
        }
        return null;
    }

    public PublishCachePlatformInfo GetCachaPlatformConfig(ePublishPlatformType type)
    {
        PublishCachePlatformInfo info;
        if(m_CachePlatformInfo.DicInfo.TryGetValue(type, out info))
        {
            return info;
        }
        else
        {
            info = new PublishCachePlatformInfo();
            m_CachePlatformInfo.DicInfo.Add(type, info);
            return info;
        }
    }

    public PublishCacheChannelInfo GetCachaChannelConfig(string name)
    {
        PublishCacheChannelInfo info;
        if (m_CacheChannelInfo.DicInfo.TryGetValue(name, out info))
        {
            return info;
        }
        else
        {
            info = new PublishCacheChannelInfo();
            m_CacheChannelInfo.DicInfo.Add(name, info);
            return info;
        }
    }
}

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

    public void Setup()
    {
        m_PlatformConfig = PublishUtils.ReadPlatformConfig();
    }

    public void Destroy()
    {

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

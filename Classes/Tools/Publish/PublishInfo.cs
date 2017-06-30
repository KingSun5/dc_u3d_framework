using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 平台配置表信息
/// @author hannibal
/// @time 2017-6-30
/// </summary>
[Serializable]
public class PublishPlatformInfo
{
    public string   Name;               //平台名
    public string   BundleIdentifier;   //id
    public string   BundleVersion;      //版本号
    public int      BundleVersionCode;  //编译版本号
    public string   PackageName;        //包名
    public string   CompileDefine;      //编译选项
}

[Serializable]
public class PublishPlatformSet
{
    public int type;
    public string name;
    public List<PublishPlatformInfo> list = new List<PublishPlatformInfo>();
}

[Serializable]
public class PublishPlatformCollection
{
    public List<PublishPlatformSet> plats = new List<PublishPlatformSet>();
}

/// <summary>
/// 平台缓存
/// </summary>
public class PublishCachePlatformInfo
{
    public ScreenOrientation Orientation;//横屏竖屏
    public RenderingPath RenderPath;
    public bool EnableUnitySplash = false;  //闪屏
    public bool AutoGraphicsAPI = false;  //图像引擎
    public UnityEngine.Rendering.GraphicsDeviceType GraphicsDevice;
    public bool StaticBatch = true;
    public bool DynamicBatch = true;
    public bool GUPSkin = false;
    public eScriptingImplementation ScriptBackend;

    public string KeyStorePath;//签名相关
    public string KetStorePass;
    public string KeyAliasName;
    public string KeyAliasPass;
}

public class PublishCachePlatformSet
{
    public Dictionary<ePublishPlatformType, PublishCachePlatformInfo> DicInfo = new Dictionary<ePublishPlatformType, PublishCachePlatformInfo>();
}

/// <summary>
/// 渠道缓存
/// </summary>
public class PublishCacheChannelInfo
{
    public bool IsBuild;
}

public class PublishCacheChannelSet
{
    public Dictionary<string, PublishCacheChannelInfo> DicInfo = new Dictionary<string, PublishCacheChannelInfo>();
}
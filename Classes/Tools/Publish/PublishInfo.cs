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
    public string   CompileDefine;      //预定义宏
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
/// 平台数据缓存
/// </summary>
public class PublishCachePlatformInfo
{
    public ScreenOrientation Orientation;   //横屏竖屏
    public RenderingPath RenderPath;
    public eScriptingImplementation ScriptBackend;
    public eApiCompatibilityLevel ApiLevel;
    public eTargetDevice TargetDevice;      //目标设备
    public eInstallLocation InstallLocation;//安装目录
    public eStrippingLevel StrippingLevel;  //代码剥离
    public bool EnableUnitySplash = false;  //闪屏
    public bool AutoGraphicsAPI = false;    //图像引擎
    public UnityEngine.Rendering.GraphicsDeviceType GraphicsDevice;
    public bool StaticBatch = true;
    public bool DynamicBatch = true;
    public bool GUPSkin = false;
    public bool MultiThreadRender = true;   //多线程渲染

    //安卓
    public eAndroidSdkVersions MinAndroidSdkVersion;//最小sdk版本
    public bool SDCardPermission = true;    //SD卡

    public string KeyStorePath;             //签名
    public string KetStorePass;
    public string KeyAliasName;
    public string KeyAliasPass;

    //ios
    public eIOSSdkVerions IOSSdkVerions;    //是否真机运行
    public string OSVersionString = "6.0";  //最小目标版本
    public eIOSScriptCallOptimizationLevel IOSOptLevel;//脚本优化

    //pc
    public bool DefaultFullScreen = false;  //是否默认全屏
    public int DefaultScreenWidth = 800;
    public int DefaultScreenHeight = 600;
}

public class PublishCachePlatformSet
{
    public Dictionary<ePublishPlatformType, PublishCachePlatformInfo> DicInfo = new Dictionary<ePublishPlatformType, PublishCachePlatformInfo>();
}

/// <summary>
/// 渠道数据缓存
/// </summary>
public class PublishCacheChannelInfo
{
    public bool IsBuild;
}

public class PublishCacheChannelSet
{
    public Dictionary<string, PublishCacheChannelInfo> DicInfo = new Dictionary<string, PublishCacheChannelInfo>();
}
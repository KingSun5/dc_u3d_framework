using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

/// <summary>
/// 发布入口
/// @author hannibal
/// @time 2017-6-30
/// </summary>
public class BuildPackage
{
    #region
    [MenuItem("Tools/Build/Android")]
    static void PerformAndroidBuild()
    {
        ShowWindow(ePublishPlatformType.Android);
    }
    [MenuItem("Tools/Build/IOS")]
    static void PerformIOSBuild()
    {
        ShowWindow(ePublishPlatformType.iOS);
    }
    [MenuItem("Tools/Build/Win")]
    static void PerformWinBuild()
    {
        ShowWindow(ePublishPlatformType.Win);
    }
    [MenuItem("Tools/Build/WebGL")]
    static void PerformWebGLBuild()
    {
        ShowWindow(ePublishPlatformType.WebGL);
    }
    static void ShowWindow(ePublishPlatformType type)
    {
        PublishManager.Instance.Setup();

        string title_name = PublishUtils.GetPlatformNameByType(type);
        PublishWindow abWindow = EditorWindow.GetWindowWithRect<PublishWindow>(new Rect(0,0,1000,900), false, title_name, true);
        abWindow.Setup(type);
    }
    #endregion

    /// <summary>
    /// 设置发布选项
    /// </summary>
    /// <param name="target"></param>
    /// <param name="platform_info"></param>
    /// <param name="cache_channel_info"></param>
    /// <param name="cache_plat_info"></param>
    public static void BulidTarget(ePublishPlatformType target, PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {
        BuildTarget buildTarget = PublishWindow.GetBuildTargetByType(target);
        BuildTargetGroup targetGroup = PublishWindow.GetTargetGroupByType(target);

        ///1.全局
        PlayerSettings.companyName = "广州硕星";
        PlayerSettings.productName = "机甲军团";

        ///2.Resolution and Presentatio
        UIOrientation orientation = UIOrientation.LandscapeLeft;
        switch(cache_plat_info.Orientation)
        {
            case ScreenOrientation.AutoRotation: orientation = UIOrientation.AutoRotation; break;
            case ScreenOrientation.LandscapeLeft: orientation = UIOrientation.LandscapeLeft; break;
            case ScreenOrientation.LandscapeRight: orientation = UIOrientation.LandscapeRight; break;
            case ScreenOrientation.Portrait: orientation = UIOrientation.Portrait; break;
            case ScreenOrientation.PortraitUpsideDown: orientation = UIOrientation.PortraitUpsideDown; break;
        }
        PlayerSettings.defaultInterfaceOrientation = orientation;
        PlayerSettings.use32BitDisplayBuffer = false;

        ///3.Icon

        ///4.Splash Image
        PlayerSettings.SplashScreen.show = cache_plat_info.EnableUnitySplash;
        PlayerSettings.SplashScreen.showUnityLogo = cache_plat_info.EnableUnitySplash;

        ///5.RenderPath
        UnityEditor.Rendering.TierSettings ts = new UnityEditor.Rendering.TierSettings();
        ts.renderingPath = cache_plat_info.RenderPath;

        ///6.BundleIdentifier
        PlayerSettings.applicationIdentifier = platform_info.BundleIdentifier;
        PlayerSettings.bundleVersion = platform_info.BundleVersion;
        if (!string.IsNullOrEmpty(platform_info.CompileDefine))
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, platform_info.CompileDefine);

        ///7.SetGraphicsAPIs 
        ///TODO
        //UnityEditor.Rendering.EditorGraphicsSettings.SetTierSettings(targetGroup, UnityEngine.Rendering.GraphicsTier.Tier2, ts);
        //UnityEngine.Rendering.GraphicsDeviceType[] gdt = new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3, UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 };
        //PlayerSettings.SetGraphicsAPIs(buildTarget, gdt);

        ///8.IL2CPP
        if(target != ePublishPlatformType.WebGL)
        {
            ScriptingImplementation script = ScriptingImplementation.Mono2x;
            switch (cache_plat_info.ScriptBackend)
            {
                case eScriptingImplementation.IL2CPP: script = ScriptingImplementation.IL2CPP; break;
                case eScriptingImplementation.Mono2x: script = ScriptingImplementation.Mono2x; break;
            }
            PlayerSettings.SetScriptingBackend(targetGroup, script);
        }

        switch (target)
        {
            case ePublishPlatformType.Android:
                HandleAndroidPlayerSetting(platform_info, cache_channel_info, cache_plat_info);
                break;
            case ePublishPlatformType.iOS:
                HandleIOSPlayerSetting(platform_info, cache_channel_info, cache_plat_info);
                break;
            case ePublishPlatformType.Win:
                HandleWinPlayerSetting(platform_info, cache_channel_info, cache_plat_info);
                break;
            case ePublishPlatformType.WebGL:
                HandleWebGLPlayerSetting(platform_info, cache_channel_info, cache_plat_info);
                break;
        }
    }

    private static void HandleAndroidPlayerSetting(PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {
        PlayerSettings.Android.bundleVersionCode = platform_info.BundleVersionCode;
        if (!string.IsNullOrEmpty(cache_plat_info.KeyStorePath))
        {
            PlayerSettings.Android.keystoreName = cache_plat_info.KeyStorePath;
            PlayerSettings.Android.keystorePass = cache_plat_info.KetStorePass;
            PlayerSettings.Android.keyaliasName = cache_plat_info.KeyAliasName;
            PlayerSettings.Android.keyaliasPass = cache_plat_info.KeyAliasPass;
        }
    }
    private static void HandleIOSPlayerSetting(PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {

    }
    private static void HandleWinPlayerSetting(PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {

    }
    private static void HandleWebGLPlayerSetting(PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {

    }
}
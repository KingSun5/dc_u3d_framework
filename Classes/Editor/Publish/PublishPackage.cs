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
    #region 工具栏
    [MenuItem("Tools/Publish/Android")]
    static void PerformAndroidBuild()
    {
        ShowWindow(ePublishPlatformType.Android);
    }
    [MenuItem("Tools/Publish/IOS")]
    static void PerformIOSBuild()
    {
        ShowWindow(ePublishPlatformType.iOS);
    }
    [MenuItem("Tools/Publish/Win64")]
    static void PerformWin64Build()
    {
        ShowWindow(ePublishPlatformType.Win64);
    }
    [MenuItem("Tools/Publish/Win32")]
    static void PerformWin32Build()
    {
        ShowWindow(ePublishPlatformType.Win32);
    }
    [MenuItem("Tools/Publish/WebGL")]
    static void PerformWebGLBuild()
    {
        ShowWindow(ePublishPlatformType.WebGL);
    }
    [MenuItem("Tools/Publish/打开配置表目录")]
    static void PerformOpenConfig()
    {
        string path = PublishUtils.GetPublishConfigDir();
        path = path.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer", "/n, " + path);
    }
    [MenuItem("Tools/Publish/重置缓存数据")]
    static void PerformResetData()
    {
        PublishManager.Instance.ResetData();
    }
    static void ShowWindow(ePublishPlatformType type)
    {
        PublishManager.Instance.Setup();

        string title_name = PublishUtils.GetPlatformNameByType(type);
        PublishWindow abWindow = EditorWindow.GetWindowWithRect<PublishWindow>(new Rect(0,0,1000,800), false, title_name, true);
        abWindow.Setup(type);
    }
    #endregion

    #region 发布
    /// <summary>
    /// 发布
    /// </summary>
    public static void StartPublish(string publish_path, ePublishPlatformType target, PublishPlatformSet platform_config, PublishCachePlatformInfo cache_platform_info)
    {
        //1.有效性校验
        //判断是否有勾选发布平台
        bool has_select_build = false;
        PublishPlatformInfo platform_info;
        PublishCacheChannelInfo cache_channel_info;
        for (int i = 0; i < platform_config.list.Count; ++i)
        {
            platform_info = platform_config.list[i];
            cache_channel_info = PublishManager.Instance.GetCachaChannelConfig(platform_info.Name);
            if (cache_channel_info.IsBuild)
            {
                has_select_build = true;
                break;
            }
        }
        if (!has_select_build)
        {
            EditorUtility.DisplayDialog("错误", "没有选择需要发布的版本", "确定");
            return;
        }
        //发布路径
        if (string.IsNullOrEmpty(publish_path))
        {
            EditorUtility.DisplayDialog("错误", "发布路径错误", "确定");
            return;
        }

        ///2.发布
        PublishAll(publish_path, target, platform_config, cache_platform_info);

        ///3.发布完成
        PublishManager.Instance.OnPublishComplete();
    }
    /// <summary>
    /// 遍历所有需要发布的平台
    /// </summary>
    /// <param name="scenes"></param>
    /// <param name="target_dir"></param>
    /// <param name="build_target"></param>
    /// <param name="build_options"></param>
    private static void PublishAll(string publish_path, ePublishPlatformType target, PublishPlatformSet platform_config, PublishCachePlatformInfo cache_platform_info)
    {
        string publish_packet = "";
        PublishPlatformInfo platform_info;
        PublishCacheChannelInfo cache_channel_info;
        for (int i = 0; i < platform_config.list.Count; ++i)
        {
            platform_info = platform_config.list[i];
            cache_channel_info = PublishManager.Instance.GetCachaChannelConfig(platform_info.Name);
            if (cache_channel_info.IsBuild)
            {
                BuildOne(publish_path, target, platform_config, platform_info, cache_channel_info, cache_platform_info);
                publish_packet += platform_info.PackageName + "\n";
            }
        }

        EditorUtility.DisplayDialog("提示", "发布完成，以下是发布的版本:\n" + publish_packet, "确定");
    }
    /// <summary>
    /// 发布一个
    /// </summary>
    private static void BuildOne(string publish_path, ePublishPlatformType target, PublishPlatformSet platform_config, PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_platform_info)
    {
        Log.Info("正在发布版本:" + platform_info.PackageName);

        BuildTarget build_target = GetBuildTargetByType(target);
        BuildTargetGroup target_group = GetTargetGroupByType(target);
        string[] scenes = FindEnabledEditorScenes();

        //设置发布选项
        BulidTarget(target, platform_info, cache_channel_info, cache_platform_info);

        //发布
        EditorUserBuildSettings.SwitchActiveBuildTarget(target_group, build_target);
        BuildPipeline.BuildPlayer(scenes, GetSavePath(publish_path, target, platform_info.PackageName), build_target, BuildOptions.None);

        Log.Info("发布完成一个:" + platform_info.PackageName);
    }
    #endregion

    #region 设置
    /// <summary>
    /// 设置发布选项
    /// </summary>
    /// <param name="target"></param>
    /// <param name="platform_info"></param>
    /// <param name="cache_channel_info"></param>
    /// <param name="cache_plat_info"></param>
    public static void BulidTarget(ePublishPlatformType target, PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {
        BuildTarget buildTarget = GetBuildTargetByType(target);
        BuildTargetGroup targetGroup = GetTargetGroupByType(target);

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
        UnityEditor.Rendering.EditorGraphicsSettings.SetTierSettings(targetGroup, UnityEngine.Rendering.GraphicsTier.Tier2, ts);

        ///6.BundleIdentifier
        PlayerSettings.applicationIdentifier = platform_info.BundleIdentifier;
        PlayerSettings.bundleVersion = platform_info.BundleVersion;

        ///7.预定义宏 
        if (!string.IsNullOrEmpty(platform_info.CompileDefine))
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, platform_info.CompileDefine);

        ///8.IL2CPP
        if(target == ePublishPlatformType.Android || target == ePublishPlatformType.iOS)
        {
            switch (cache_plat_info.ScriptBackend)
            {
                case eScriptingImplementation.IL2CPP: PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.IL2CPP); break;
                case eScriptingImplementation.Mono2x: PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.Mono2x); break;
            }
        }

        ///9.net版本
        ApiCompatibilityLevel api_level = ApiCompatibilityLevel.NET_2_0_Subset;
        switch (cache_plat_info.ApiLevel)
        {
            case eApiCompatibilityLevel.NET_2_0: api_level = ApiCompatibilityLevel.NET_2_0; break;
            case eApiCompatibilityLevel.NET_2_0_Subset: api_level = ApiCompatibilityLevel.NET_2_0_Subset; break;
        }
        PlayerSettings.SetApiCompatibilityLevel(targetGroup, api_level);

        ///9.gpu蒙皮
        PlayerSettings.gpuSkinning = cache_plat_info.GUPSkin;

        switch (target)
        {
            case ePublishPlatformType.Android:
                HandleAndroidPlayerSetting(buildTarget,targetGroup, platform_info, cache_channel_info, cache_plat_info);
                break;
            case ePublishPlatformType.iOS:
                HandleIOSPlayerSetting(buildTarget, targetGroup, platform_info, cache_channel_info, cache_plat_info);
                break;
            case ePublishPlatformType.Win64:
            case ePublishPlatformType.Win32:
                HandleWinPlayerSetting(buildTarget, targetGroup, platform_info, cache_channel_info, cache_plat_info);
                break;
            case ePublishPlatformType.WebGL:
                HandleWebGLPlayerSetting(buildTarget, targetGroup, platform_info, cache_channel_info, cache_plat_info);
                break;
        }
    }

    private static void HandleAndroidPlayerSetting(BuildTarget buildTarget,BuildTargetGroup targetGroup, PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {
        //图像引擎
        PlayerSettings.SetUseDefaultGraphicsAPIs(buildTarget, cache_plat_info.AutoGraphicsAPI);
        if (!cache_plat_info.AutoGraphicsAPI)
        {
            UnityEngine.Rendering.GraphicsDeviceType[] gdt = new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3, UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 };
            PlayerSettings.SetGraphicsAPIs(buildTarget, gdt);
        }

        //多线程渲染
        PlayerSettings.MTRendering = !cache_plat_info.MultiThreadRender;
        
        //cpu架构
        switch(cache_plat_info.TargetDevice)
        {
            case eTargetDevice.FAT:PlayerSettings.Android.targetDevice = AndroidTargetDevice.FAT;break;
            case eTargetDevice.ARMv7:PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;break;
            case eTargetDevice.x86:PlayerSettings.Android.targetDevice = AndroidTargetDevice.x86;break;
        }

        //安装位置
        switch (cache_plat_info.InstallLocation)
        {
            case eInstallLocation.Auto: PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto; break;
            case eInstallLocation.ForceInternal: PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.ForceInternal; break;
            case eInstallLocation.PreferExternal: PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.PreferExternal; break;
        }

        //SD卡读写
        PlayerSettings.Android.forceSDCardPermission = cache_plat_info.SDCardPermission;

        //最小sdk版本
        switch (cache_plat_info.MinAndroidSdkVersion)
        {
            case eAndroidSdkVersions.AndroidApiLevelAuto: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto; break;
            case eAndroidSdkVersions.AndroidApiLevel16: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16; break;
            case eAndroidSdkVersions.AndroidApiLevel17: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel17; break;
            case eAndroidSdkVersions.AndroidApiLevel18: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel18; break;
            case eAndroidSdkVersions.AndroidApiLevel19: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19; break;
            case eAndroidSdkVersions.AndroidApiLevel21: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21; break;
            case eAndroidSdkVersions.AndroidApiLevel22: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22; break;
            case eAndroidSdkVersions.AndroidApiLevel23: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23; break;
            case eAndroidSdkVersions.AndroidApiLevel24: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24; break;
            case eAndroidSdkVersions.AndroidApiLevel25: PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25; break;
        }

        //代码剥离
        switch (cache_plat_info.StrippingLevel)
        {
            case eStrippingLevel.Disabled: PlayerSettings.strippingLevel = StrippingLevel.Disabled; break;
            case eStrippingLevel.StripAssemblies: PlayerSettings.strippingLevel = StrippingLevel.StripAssemblies; break;
            case eStrippingLevel.StripByteCode: PlayerSettings.strippingLevel = StrippingLevel.StripByteCode; break;
            case eStrippingLevel.UseMicroMSCorlib: PlayerSettings.strippingLevel = StrippingLevel.UseMicroMSCorlib; break;
        }

        //是否分包
        PlayerSettings.Android.useAPKExpansionFiles = cache_plat_info.APKExpansionFiles;
        
        PlayerSettings.Android.bundleVersionCode = platform_info.BundleVersionCode;
        if (!string.IsNullOrEmpty(cache_plat_info.KeyStorePath))
        {
            PlayerSettings.Android.keystoreName = cache_plat_info.KeyStorePath;
            PlayerSettings.Android.keystorePass = cache_plat_info.KetStorePass;
            PlayerSettings.Android.keyaliasName = cache_plat_info.KeyAliasName;
            PlayerSettings.Android.keyaliasPass = cache_plat_info.KeyAliasPass;
        }
    }
    private static void HandleIOSPlayerSetting(BuildTarget buildTarget, BuildTargetGroup targetGroup, PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {        
        //图像引擎
        PlayerSettings.SetUseDefaultGraphicsAPIs(buildTarget, cache_plat_info.AutoGraphicsAPI);
        if (!cache_plat_info.AutoGraphicsAPI)
        {
            UnityEngine.Rendering.GraphicsDeviceType[] gdt = new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3, UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 };
            PlayerSettings.SetGraphicsAPIs(buildTarget, gdt);
        }

        //多线程渲染
        PlayerSettings.MTRendering = !cache_plat_info.MultiThreadRender;

        //编译版本
        PlayerSettings.iOS.buildNumber = platform_info.BundleVersionCode.ToString();

        //cpu架构
        switch(cache_plat_info.TargetDevice)
        {
            case eTargetDevice.iPadOnly:PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPadOnly;break;
            case eTargetDevice.iPhoneAndiPad:PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;break;
            case eTargetDevice.iPhoneOnly:PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;break;
        }

        //SDK
        switch(cache_plat_info.IOSSdkVerions)
        {
            case eIOSSdkVerions.DeviceSDK: PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK; break;
            case eIOSSdkVerions.SimulatorSDK: PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK; break;
        }

        //目标最低版本
        PlayerSettings.iOS.targetOSVersionString = cache_plat_info.OSVersionString;

        //脚本优化
        switch (cache_plat_info.IOSOptLevel)
        {
            case eIOSScriptCallOptimizationLevel.SlowAndSafe: PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.SlowAndSafe; break;
            case eIOSScriptCallOptimizationLevel.FastButNoExceptions: PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.FastButNoExceptions; break;
        }
    }
    private static void HandleWinPlayerSetting(BuildTarget buildTarget, BuildTargetGroup targetGroup, PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {
        //图像引擎
        PlayerSettings.SetUseDefaultGraphicsAPIs(buildTarget, cache_plat_info.AutoGraphicsAPI);
        if (!cache_plat_info.AutoGraphicsAPI)
        {
            UnityEngine.Rendering.GraphicsDeviceType[] gdt = new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.Direct3D11, UnityEngine.Rendering.GraphicsDeviceType.Direct3D9 };
            PlayerSettings.SetGraphicsAPIs(buildTarget, gdt);
        }

    }
    private static void HandleWebGLPlayerSetting(BuildTarget buildTarget, BuildTargetGroup targetGroup, PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info, PublishCachePlatformInfo cache_plat_info)
    {
        //图像引擎
        PlayerSettings.SetUseDefaultGraphicsAPIs(buildTarget, cache_plat_info.AutoGraphicsAPI);
        if (!cache_plat_info.AutoGraphicsAPI)
        {
            //TODO
            //UnityEngine.Rendering.GraphicsDeviceType[] gdt = new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.we};
            //PlayerSettings.SetGraphicsAPIs(buildTarget, gdt);
        }

        //是否可以改变窗口大小
        PlayerSettings.displayResolutionDialog = cache_plat_info.DisplayResDialog ? ResolutionDialogSetting.Enabled : ResolutionDialogSetting.Disabled;
        PlayerSettings.resizableWindow = cache_plat_info.ResizeableWindow;
    }
    #endregion

    #region 工具类
    /// <summary>
    /// 保存路径
    /// </summary>
    private static string GetSavePath(string publish_path, ePublishPlatformType platform_type, string app_name)
    {
        string target_path = "";
        string target_dir = publish_path;
        switch (platform_type)
        {
            case ePublishPlatformType.Android:
                target_path = target_dir + "/" + app_name + ".apk";
                break;
            case ePublishPlatformType.iOS:
                target_path = target_dir + "/" + app_name + ".ipa";
                break;
            case ePublishPlatformType.Win32:
            case ePublishPlatformType.Win64:
                target_path = target_dir + "/" + app_name + ".exe";
                break;
            case ePublishPlatformType.WebGL:
                //TODO
                break;
        }
        //每次build删除之前的残留
        if (Directory.Exists(target_dir))
        {
            if (File.Exists(target_path))
            {
                File.Delete(target_path);
            }
        }
        else
        {
            Directory.CreateDirectory(target_dir);
        }
        return target_path;
    }
    public static BuildTarget GetBuildTargetByType(ePublishPlatformType type)
    {
        switch (type)
        {
            case ePublishPlatformType.Android: return BuildTarget.Android;
            case ePublishPlatformType.iOS: return BuildTarget.iOS;
            case ePublishPlatformType.Win64: return BuildTarget.StandaloneWindows64;
            case ePublishPlatformType.Win32: return BuildTarget.StandaloneWindows;
            case ePublishPlatformType.WebGL: return BuildTarget.WebGL;
        }
        return BuildTarget.NoTarget;
    }
    public static BuildTargetGroup GetTargetGroupByType(ePublishPlatformType type)
    {
        switch (type)
        {
            case ePublishPlatformType.Android: return BuildTargetGroup.Android;
            case ePublishPlatformType.iOS: return BuildTargetGroup.iOS;
            case ePublishPlatformType.Win32: return BuildTargetGroup.Standalone;
            case ePublishPlatformType.Win64: return BuildTargetGroup.Standalone;
            case ePublishPlatformType.WebGL: return BuildTargetGroup.WebGL;
        }
        return BuildTargetGroup.Unknown;
    }
    public static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }
    #endregion
}
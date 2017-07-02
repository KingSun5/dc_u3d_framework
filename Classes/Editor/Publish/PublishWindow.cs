using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 发布窗口
/// @author hannibal
/// @time 2017-6-30
/// </summary>
public class PublishWindow : EditorWindow
{
    private string m_PublishPath;

    private ePublishPlatformType m_PlatformType;
    private PublishPlatformSet m_PlatformConfig;
    private PublishCachePlatformInfo m_CachePlatformInfo;
    
    public void Setup(ePublishPlatformType type) 
    {
        Log.Debug("[publish]打包平台:" + PublishUtils.GetPlatformNameByType(type));

        m_PlatformType = type;

        m_CachePlatformInfo = PublishManager.Instance.GetCachaPlatformConfig(type);
        if (m_CachePlatformInfo == null)
        {
            Log.Error("[publish]平台缓存读取错误:" + type);
            Close();
            return;
        }

        m_PlatformConfig = PublishManager.Instance.GetPlatformConfig(type);
        if(m_PlatformConfig == null)
        {
            Log.Error("[publish]平台配置表信息读取错误:" + type);
            Close();
            return;
        }

        m_PublishPath = PublishUtils.GetPublishPath(m_PlatformType);
	}
	
    public void Destroy()
    {
        this.Close();
    }

    #region 渲染界面
    void OnGUI()
    {
        if (m_CachePlatformInfo == null) return;

        BeginWindows();
        GUILayout.Window(0, new Rect(20, 20, position.width - 40, position.height * 0.5f), DrawPlatformWindow, "Platforms", GUILayout.Width(position.width - 40));
        EndWindows();

        GUILayout.Space(position.height * 0.5f + 60);

        DrawDefiedWindow();

        switch(m_PlatformType)
        {
            case ePublishPlatformType.Android:
                DrawAndroidWindow();
                break;
            case ePublishPlatformType.iOS:
                DrawIOSWindow();
                break;
            case ePublishPlatformType.Win64:
            case ePublishPlatformType.Win32:
                DrawWinWindow();
                break;
            case ePublishPlatformType.WebGL:
                DrawWebGLWindow();
                break;
        }

        DrawBuildWindow();
    }

    Vector2 mScrollPos;
    void DrawPlatformWindow(int Index)
    {
        PublishPlatformInfo platform_info;
        PublishCacheChannelInfo cache_channel_info;
        mScrollPos = GUILayout.BeginScrollView(mScrollPos, GUILayout.Width(position.width - 60), GUILayout.Height(position.height * 0.5f));
        for (int i = 0; i < m_PlatformConfig.list.Count; ++i)
        {
            platform_info = m_PlatformConfig.list[i];
            cache_channel_info = PublishManager.Instance.GetCachaChannelConfig(platform_info.Name);
            DrawPlatformInfo(platform_info, cache_channel_info);
            if (i < m_PlatformConfig.list.Count-1) 
                DrawLine();
        }
        GUILayout.Space(20);
        GUILayout.EndScrollView();
    }
    /// <summary>
    /// 显示平台数据
    /// </summary>
    /// <param name="platform_info"></param>
    void DrawPlatformInfo(PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("平台名称", GUILayout.Width(100));
        GUILayout.Label(platform_info.Name, GUILayout.Width(140));
        GUILayout.Space(20);
        GUILayout.Label("BundleIdentifier:", GUILayout.Width(120));
        GUILayout.Label(platform_info.BundleIdentifier, GUILayout.Width(140));
        GUILayout.Space(20);
        GUILayout.Label("Build:", GUILayout.Width(75));
        cache_channel_info.IsBuild = EditorGUILayout.Toggle(cache_channel_info.IsBuild, GUILayout.Width(30));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("BundleVersion", GUILayout.Width(100));
        platform_info.BundleVersion = EditorGUILayout.TextField(platform_info.BundleVersion, GUILayout.Width(140));
        GUILayout.Space(20);
        GUILayout.Label("BundleVersionCode", GUILayout.Width(120));
        platform_info.BundleVersionCode = EditorGUILayout.IntField(platform_info.BundleVersionCode, GUILayout.Width(140));
        GUILayout.Space(20);
        GUILayout.Label("CompileDefine", GUILayout.Width(100));
        platform_info.CompileDefine = EditorGUILayout.TextField(platform_info.CompileDefine, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("PackageName", GUILayout.Width(100));
        platform_info.PackageName = EditorGUILayout.TextField(platform_info.PackageName, GUILayout.Width(140));
        GUILayout.Space(20);
        GUILayout.EndHorizontal();
    }

    int OrientationIndex = 3;
    int RenderPathIndex = 2;
    int GraphicsAPIAndIndex = 1;
    int GraphicsAPIIOSIndex = 1;
    int GraphicsAPIWinIndex = 1;
    int GraphicsAPIWebIndex = 0;
    int ScriptBackemdIndex = 1;
    int NetLevelIndex = 1;
    int AndroidDeviceIndex = 1;
    int IOSDeviceIndex = 2;
    int IOSSDKIndex = 0;
    int IOSOptLevelIndex = 0;
    int InstallLocationIndex = 2;
    int MinAndroidSDK = 1;
    int StripLevel = 0;
    void DrawAndroidWindow()
    {
        //渲染引擎
        GUILayout.BeginHorizontal();
        GUILayout.Label("渲染引擎", GUILayout.Width(100));
        m_CachePlatformInfo.AutoGraphicsAPI = EditorGUILayout.Toggle(m_CachePlatformInfo.AutoGraphicsAPI, GUILayout.Width(30));
        if (!m_CachePlatformInfo.AutoGraphicsAPI)
        {
            string[] graphics_options = { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2.ToString(), UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3.ToString() };
            GraphicsAPIAndIndex = EditorGUILayout.Popup(GraphicsAPIAndIndex, graphics_options);
            switch (GraphicsAPIAndIndex)
            {
                case 0: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2; break;
                case 1: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3; break;
            }
        }
        GUILayout.EndHorizontal();

        //横屏竖屏,渲染方式
        GUILayout.BeginHorizontal();
            GUILayout.Label("横屏竖屏", GUILayout.Width(100));
            string[] ori_options = { ScreenOrientation.Unknown.ToString(), ScreenOrientation.Portrait.ToString(), ScreenOrientation.PortraitUpsideDown.ToString(), ScreenOrientation.LandscapeLeft.ToString(), ScreenOrientation.LandscapeRight.ToString(), ScreenOrientation.AutoRotation.ToString() };
            OrientationIndex = EditorGUILayout.Popup(OrientationIndex, ori_options);
            switch(OrientationIndex)
            {
                case 0: m_CachePlatformInfo.Orientation = ScreenOrientation.Unknown; break;
                case 1: m_CachePlatformInfo.Orientation = ScreenOrientation.Portrait; break;
                case 2: m_CachePlatformInfo.Orientation = ScreenOrientation.PortraitUpsideDown; break;
                case 3: m_CachePlatformInfo.Orientation = ScreenOrientation.LandscapeLeft; break;
                case 4: m_CachePlatformInfo.Orientation = ScreenOrientation.LandscapeRight; break;
                case 5: m_CachePlatformInfo.Orientation = ScreenOrientation.AutoRotation; break;
            }

            GUILayout.Space(20);
            GUILayout.Label("编译环境", GUILayout.Width(100));
            string[] script_options = { ScriptingImplementation.Mono2x.ToString(), ScriptingImplementation.IL2CPP.ToString() };
            ScriptBackemdIndex = EditorGUILayout.Popup(ScriptBackemdIndex, script_options);
            switch (ScriptBackemdIndex)
            {
                case 0: m_CachePlatformInfo.ScriptBackend = eScriptingImplementation.Mono2x; break;
                case 1: m_CachePlatformInfo.ScriptBackend = eScriptingImplementation.IL2CPP; break;
            }

            GUILayout.Space(20);
            GUILayout.Label(".Net 版本", GUILayout.Width(100));
            string[] net_options = { eApiCompatibilityLevel.NET_2_0.ToString(), eApiCompatibilityLevel.NET_2_0_Subset.ToString() };
            NetLevelIndex = EditorGUILayout.Popup(NetLevelIndex, net_options);
            switch (NetLevelIndex)
            {
                case 0: m_CachePlatformInfo.ApiLevel = eApiCompatibilityLevel.NET_2_0; break;
                case 1: m_CachePlatformInfo.ApiLevel = eApiCompatibilityLevel.NET_2_0_Subset; break;
            }

            GUILayout.Space(20);
            GUILayout.Label("多线程渲染", GUILayout.Width(100));
            m_CachePlatformInfo.MultiThreadRender = EditorGUILayout.Toggle(m_CachePlatformInfo.MultiThreadRender, GUILayout.Width(30));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label("目标设备", GUILayout.Width(100));
            string[] device_options = { eTargetDevice.FAT.ToString(), eTargetDevice.ARMv7.ToString(), eTargetDevice.x86.ToString() };
            AndroidDeviceIndex = EditorGUILayout.Popup(AndroidDeviceIndex, device_options);
            switch (AndroidDeviceIndex)
            {
                case 0: m_CachePlatformInfo.TargetDevice = eTargetDevice.FAT; break;
                case 1: m_CachePlatformInfo.TargetDevice = eTargetDevice.ARMv7; break;
                case 2: m_CachePlatformInfo.TargetDevice = eTargetDevice.x86; break;
            }

            GUILayout.Space(20);
            GUILayout.Label("安装位置", GUILayout.Width(100));
            string[] install_options = { eInstallLocation.Auto.ToString(), eInstallLocation.ForceInternal.ToString(), eInstallLocation.PreferExternal.ToString() };
            InstallLocationIndex = EditorGUILayout.Popup(InstallLocationIndex, install_options);
            switch (InstallLocationIndex)
            {
                case 0: m_CachePlatformInfo.InstallLocation = eInstallLocation.Auto; break;
                case 1: m_CachePlatformInfo.InstallLocation = eInstallLocation.ForceInternal; break;
                case 2: m_CachePlatformInfo.InstallLocation = eInstallLocation.PreferExternal; break;
            }

            GUILayout.Space(20);
            GUILayout.Label("最小安卓版本", GUILayout.Width(100));
            string[] sdk_options = { eAndroidSdkVersions.AndroidApiLevelAuto.ToString(), eAndroidSdkVersions.AndroidApiLevel16.ToString(), eAndroidSdkVersions.AndroidApiLevel17.ToString(), eAndroidSdkVersions.AndroidApiLevel18.ToString(), eAndroidSdkVersions.AndroidApiLevel19.ToString(), eAndroidSdkVersions.AndroidApiLevel21.ToString(), eAndroidSdkVersions.AndroidApiLevel22.ToString(), eAndroidSdkVersions.AndroidApiLevel23.ToString(), eAndroidSdkVersions.AndroidApiLevel24.ToString(), eAndroidSdkVersions.AndroidApiLevel25.ToString() };
            MinAndroidSDK = EditorGUILayout.Popup(MinAndroidSDK, sdk_options);
            switch (MinAndroidSDK)
            {
                case 0: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevelAuto; break;
                case 1: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevel16; break;
                case 2: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevel17; break;
                case 3: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevel18; break;
                case 4: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevel19; break;
                case 5: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevel21; break;
                case 6: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevel22; break;
                case 7: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevel23; break;
                case 8: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevel24; break;
                case 9: m_CachePlatformInfo.MinAndroidSdkVersion = eAndroidSdkVersions.AndroidApiLevel25; break;
            }

            GUILayout.Space(20);
            GUILayout.Label("SD读写", GUILayout.Width(100));
            m_CachePlatformInfo.SDCardPermission = EditorGUILayout.Toggle(m_CachePlatformInfo.SDCardPermission, GUILayout.Width(30));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label("代码剥离", GUILayout.Width(100));
            string[] strip_options = { eStrippingLevel.Disabled.ToString(), eStrippingLevel.StripAssemblies.ToString(), eStrippingLevel.StripByteCode.ToString(), eStrippingLevel.UseMicroMSCorlib.ToString() };
            StripLevel = EditorGUILayout.Popup(StripLevel, strip_options);
            switch (StripLevel)
            {
                case 0: m_CachePlatformInfo.StrippingLevel = eStrippingLevel.Disabled; break;
                case 1: m_CachePlatformInfo.StrippingLevel = eStrippingLevel.StripAssemblies; break;
                case 2: m_CachePlatformInfo.StrippingLevel = eStrippingLevel.StripByteCode; break;
                case 3: m_CachePlatformInfo.StrippingLevel = eStrippingLevel.UseMicroMSCorlib; break;
            }
        GUILayout.EndHorizontal();


        //签名
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
            GUILayout.Label("StoreKeyPath", GUILayout.Width(100));
            m_CachePlatformInfo.KeyStorePath = EditorGUILayout.TextField(m_CachePlatformInfo.KeyStorePath, GUILayout.Width(300));
            GUILayout.Space(20);
            GUILayout.Label("Password", GUILayout.Width(60));
            m_CachePlatformInfo.KetStorePass = EditorGUILayout.TextField(m_CachePlatformInfo.KetStorePass, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        //签名
        GUILayout.BeginHorizontal();
            GUILayout.Label("KeyAliasPath", GUILayout.Width(100));
            m_CachePlatformInfo.KeyAliasName = EditorGUILayout.TextField(m_CachePlatformInfo.KeyAliasName, GUILayout.Width(300));
            GUILayout.Space(20);
            GUILayout.Label("Password", GUILayout.Width(60));
            m_CachePlatformInfo.KeyAliasPass = EditorGUILayout.TextField(m_CachePlatformInfo.KeyAliasPass, GUILayout.Width(100));
        GUILayout.EndHorizontal();
    }

    void DrawIOSWindow()
    {
        //渲染引擎
        GUILayout.BeginHorizontal();
            GUILayout.Label("渲染引擎", GUILayout.Width(100));
            m_CachePlatformInfo.AutoGraphicsAPI = EditorGUILayout.Toggle(m_CachePlatformInfo.AutoGraphicsAPI, GUILayout.Width(30));
            if (!m_CachePlatformInfo.AutoGraphicsAPI)
            {
                string[] graphics_options = { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2.ToString(), UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3.ToString(), UnityEngine.Rendering.GraphicsDeviceType.Metal.ToString() };
                GraphicsAPIIOSIndex = EditorGUILayout.Popup(GraphicsAPIIOSIndex, graphics_options);
                switch (GraphicsAPIIOSIndex)
                {
                    case 0: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2; break;
                    case 1: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3; break;
                    case 2: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.Metal; break;
                }
            }
        GUILayout.EndHorizontal();

        //横屏竖屏,渲染方式
        GUILayout.BeginHorizontal();
            GUILayout.Label("横屏竖屏", GUILayout.Width(100));
            string[] ori_options = { ScreenOrientation.Unknown.ToString(), ScreenOrientation.Portrait.ToString(), ScreenOrientation.PortraitUpsideDown.ToString(), ScreenOrientation.LandscapeLeft.ToString(), ScreenOrientation.LandscapeRight.ToString(), ScreenOrientation.AutoRotation.ToString() };
            OrientationIndex = EditorGUILayout.Popup(OrientationIndex, ori_options);
            switch (OrientationIndex)
            {
                case 0: m_CachePlatformInfo.Orientation = ScreenOrientation.Unknown; break;
                case 1: m_CachePlatformInfo.Orientation = ScreenOrientation.Portrait; break;
                case 2: m_CachePlatformInfo.Orientation = ScreenOrientation.PortraitUpsideDown; break;
                case 3: m_CachePlatformInfo.Orientation = ScreenOrientation.LandscapeLeft; break;
                case 4: m_CachePlatformInfo.Orientation = ScreenOrientation.LandscapeRight; break;
                case 5: m_CachePlatformInfo.Orientation = ScreenOrientation.AutoRotation; break;
            }

            GUILayout.Space(20);
            GUILayout.Label("编译环境", GUILayout.Width(100));
            string[] script_options = { ScriptingImplementation.Mono2x.ToString(), ScriptingImplementation.IL2CPP.ToString() };
            ScriptBackemdIndex = EditorGUILayout.Popup(ScriptBackemdIndex, script_options);
            switch (ScriptBackemdIndex)
            {
                case 0: m_CachePlatformInfo.ScriptBackend = eScriptingImplementation.Mono2x; break;
                case 1: m_CachePlatformInfo.ScriptBackend = eScriptingImplementation.IL2CPP; break;
            }

            GUILayout.Space(20);
            GUILayout.Label(".Net 版本", GUILayout.Width(100));
            string[] net_options = { eApiCompatibilityLevel.NET_2_0.ToString(), eApiCompatibilityLevel.NET_2_0_Subset.ToString() };
            NetLevelIndex = EditorGUILayout.Popup(NetLevelIndex, net_options);
            switch (NetLevelIndex)
            {
                case 0: m_CachePlatformInfo.ApiLevel = eApiCompatibilityLevel.NET_2_0; break;
                case 1: m_CachePlatformInfo.ApiLevel = eApiCompatibilityLevel.NET_2_0_Subset; break;
            }

            GUILayout.Space(20);
            GUILayout.Label("多线程渲染", GUILayout.Width(100));
            m_CachePlatformInfo.MultiThreadRender = EditorGUILayout.Toggle(m_CachePlatformInfo.MultiThreadRender, GUILayout.Width(30));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label("目标设备", GUILayout.Width(100));
            string[] device_options = { eTargetDevice.iPadOnly.ToString(), eTargetDevice.iPhoneOnly.ToString(), eTargetDevice.iPhoneAndiPad.ToString() };
            IOSDeviceIndex = EditorGUILayout.Popup(IOSDeviceIndex, device_options);
            switch (IOSDeviceIndex)
            {
                case 0: m_CachePlatformInfo.TargetDevice = eTargetDevice.iPadOnly; break;
                case 1: m_CachePlatformInfo.TargetDevice = eTargetDevice.iPhoneOnly; break;
                case 2: m_CachePlatformInfo.TargetDevice = eTargetDevice.iPhoneAndiPad; break;
            }

            GUILayout.Space(20);
            GUILayout.Label("真机发布", GUILayout.Width(100));
            string[] skd_options = { eIOSSdkVerions.DeviceSDK.ToString(), eIOSSdkVerions.SimulatorSDK.ToString() };
            IOSSDKIndex = EditorGUILayout.Popup(IOSSDKIndex, skd_options);
            switch (IOSSDKIndex)
            {
                case 0: m_CachePlatformInfo.IOSSdkVerions = eIOSSdkVerions.DeviceSDK; break;
                case 1: m_CachePlatformInfo.IOSSdkVerions = eIOSSdkVerions.SimulatorSDK; break;
            }

            GUILayout.Space(20);
            GUILayout.Label("脚本优化", GUILayout.Width(100));
            string[] opt_level_options = { eIOSScriptCallOptimizationLevel.SlowAndSafe.ToString(), eIOSScriptCallOptimizationLevel.FastButNoExceptions.ToString() };
            IOSOptLevelIndex = EditorGUILayout.Popup(IOSOptLevelIndex, opt_level_options);
            switch (IOSOptLevelIndex)
            {
                case 0: m_CachePlatformInfo.IOSOptLevel = eIOSScriptCallOptimizationLevel.SlowAndSafe; break;
                case 1: m_CachePlatformInfo.IOSOptLevel = eIOSScriptCallOptimizationLevel.FastButNoExceptions; break;
            }

            GUILayout.Space(157);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
            GUILayout.Label("最低IOS版本", GUILayout.Width(100));
            m_CachePlatformInfo.OSVersionString = EditorGUILayout.TextField(m_CachePlatformInfo.OSVersionString, GUILayout.Width(160));
        GUILayout.EndHorizontal();
    }

    void DrawWinWindow()
    {
        GUILayout.BeginHorizontal();
            GUILayout.Label("渲染引擎", GUILayout.Width(100));
            m_CachePlatformInfo.AutoGraphicsAPI = EditorGUILayout.Toggle(m_CachePlatformInfo.AutoGraphicsAPI, GUILayout.Width(30));
            if (!m_CachePlatformInfo.AutoGraphicsAPI)
            {
                string[] graphics_options = { UnityEngine.Rendering.GraphicsDeviceType.Direct3D9.ToString(), UnityEngine.Rendering.GraphicsDeviceType.Direct3D11.ToString(), UnityEngine.Rendering.GraphicsDeviceType.Direct3D12.ToString(), UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2.ToString(), UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3.ToString(), UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore.ToString() };
                GraphicsAPIWinIndex = EditorGUILayout.Popup(GraphicsAPIWinIndex, graphics_options);
                switch (GraphicsAPIWinIndex)
                {
                    case 0: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.Direct3D9; break;
                    case 1: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.Direct3D11; break;
                    case 2: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.Direct3D12; break;
                    case 3: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2; break;
                    case 4: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3; break;
                    case 5: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore; break;
                }
            }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label(".Net 版本", GUILayout.Width(100));
            string[] net_options = { eApiCompatibilityLevel.NET_2_0.ToString(), eApiCompatibilityLevel.NET_2_0_Subset.ToString() };
            NetLevelIndex = EditorGUILayout.Popup(NetLevelIndex, net_options);
            switch (NetLevelIndex)
            {
                case 0: m_CachePlatformInfo.ApiLevel = eApiCompatibilityLevel.NET_2_0; break;
                case 1: m_CachePlatformInfo.ApiLevel = eApiCompatibilityLevel.NET_2_0_Subset; break;
            }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label("默认全屏", GUILayout.Width(100));
            m_CachePlatformInfo.DefaultFullScreen = EditorGUILayout.Toggle(m_CachePlatformInfo.DefaultFullScreen, GUILayout.Width(30));
            if (!m_CachePlatformInfo.DefaultFullScreen)
            {
                GUILayout.Label("宽", GUILayout.Width(20));
                m_CachePlatformInfo.DefaultScreenWidth = EditorGUILayout.IntField(m_CachePlatformInfo.DefaultScreenWidth, GUILayout.Width(160));

                GUILayout.Space(20);
                GUILayout.Label("高", GUILayout.Width(20));
                m_CachePlatformInfo.DefaultScreenHeight = EditorGUILayout.IntField(m_CachePlatformInfo.DefaultScreenHeight, GUILayout.Width(160));
            }
        GUILayout.EndHorizontal();
    }

    void DrawWebGLWindow()
    {
        GUILayout.BeginHorizontal();
            GUILayout.Label("渲染引擎", GUILayout.Width(100));
            m_CachePlatformInfo.AutoGraphicsAPI = EditorGUILayout.Toggle(m_CachePlatformInfo.AutoGraphicsAPI, GUILayout.Width(30));
            if (!m_CachePlatformInfo.AutoGraphicsAPI)
            {
                //TODO
                string[] graphics_options = { UnityEngine.Rendering.GraphicsDeviceType.Null.ToString() };
                GraphicsAPIWebIndex = EditorGUILayout.Popup(GraphicsAPIWebIndex, graphics_options);
                switch (GraphicsAPIWebIndex)
                {
                    case 0: m_CachePlatformInfo.GraphicsDevice = UnityEngine.Rendering.GraphicsDeviceType.Null; break;
                }
            }
        GUILayout.EndHorizontal();
        //横屏竖屏,渲染方式
        GUILayout.BeginHorizontal();
            GUILayout.Label("横屏竖屏", GUILayout.Width(100));
            string[] ori_options = { ScreenOrientation.Unknown.ToString(), ScreenOrientation.Portrait.ToString(), ScreenOrientation.PortraitUpsideDown.ToString(), ScreenOrientation.LandscapeLeft.ToString(), ScreenOrientation.LandscapeRight.ToString(), ScreenOrientation.AutoRotation.ToString() };
            OrientationIndex = EditorGUILayout.Popup(OrientationIndex, ori_options);
            switch (OrientationIndex)
            {
                case 0: m_CachePlatformInfo.Orientation = ScreenOrientation.Unknown; break;
                case 1: m_CachePlatformInfo.Orientation = ScreenOrientation.Portrait; break;
                case 2: m_CachePlatformInfo.Orientation = ScreenOrientation.PortraitUpsideDown; break;
                case 3: m_CachePlatformInfo.Orientation = ScreenOrientation.LandscapeLeft; break;
                case 4: m_CachePlatformInfo.Orientation = ScreenOrientation.LandscapeRight; break;
                case 5: m_CachePlatformInfo.Orientation = ScreenOrientation.AutoRotation; break;
            }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Label("界面大小", GUILayout.Width(100));
            GUILayout.Label("宽", GUILayout.Width(20));
            m_CachePlatformInfo.DefaultScreenWidth = EditorGUILayout.IntField(m_CachePlatformInfo.DefaultScreenWidth, GUILayout.Width(160));

            GUILayout.Space(20);
            GUILayout.Label("高", GUILayout.Width(20));
            m_CachePlatformInfo.DefaultScreenHeight = EditorGUILayout.IntField(m_CachePlatformInfo.DefaultScreenHeight, GUILayout.Width(160));
        GUILayout.EndHorizontal();
    }

    void DrawDefiedWindow()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("渲染方式", GUILayout.Width(100));
        string[] render_options = { RenderingPath.UsePlayerSettings.ToString(), RenderingPath.VertexLit.ToString(), RenderingPath.Forward.ToString(), RenderingPath.DeferredLighting.ToString(), RenderingPath.DeferredShading.ToString() };
        RenderPathIndex = EditorGUILayout.Popup(RenderPathIndex, render_options);
        switch (RenderPathIndex)
        {
            case 0: m_CachePlatformInfo.RenderPath = RenderingPath.UsePlayerSettings; break;
            case 1: m_CachePlatformInfo.RenderPath = RenderingPath.VertexLit; break;
            case 2: m_CachePlatformInfo.RenderPath = RenderingPath.Forward; break;
            case 3: m_CachePlatformInfo.RenderPath = RenderingPath.DeferredLighting; break;
            case 4: m_CachePlatformInfo.RenderPath = RenderingPath.DeferredShading; break;
        }
        //闪屏
        GUILayout.Label("Unity Splash:", GUILayout.Width(75));
        m_CachePlatformInfo.EnableUnitySplash = EditorGUILayout.Toggle(m_CachePlatformInfo.EnableUnitySplash, GUILayout.Width(30));

        GUILayout.Space(20);
        GUILayout.Label("Static Batch", GUILayout.Width(80));
        m_CachePlatformInfo.StaticBatch = EditorGUILayout.Toggle(m_CachePlatformInfo.StaticBatch, GUILayout.Width(30));

        GUILayout.Space(20);
        GUILayout.Label("Dynamic Batch", GUILayout.Width(100));
        m_CachePlatformInfo.DynamicBatch = EditorGUILayout.Toggle(m_CachePlatformInfo.DynamicBatch, GUILayout.Width(30));

        GUILayout.Space(20);
        GUILayout.Label("GPU Skin", GUILayout.Width(60));
        m_CachePlatformInfo.GUPSkin = EditorGUILayout.Toggle(m_CachePlatformInfo.GUPSkin, GUILayout.Width(30));
        GUILayout.EndHorizontal();
    }

    void DrawBuildWindow()
    {
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        GUILayout.Label("发布路径", GUILayout.Width(100));
        m_PublishPath = EditorGUILayout.TextField(m_PublishPath, GUILayout.Width(488));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(position.width*0.5f-120);
        if (GUILayout.Button("发  布", GUILayout.Width(100), GUILayout.Height(35)))
        {
            bool need_build = false;
            do
            {
#if UNITY_ANDROID
                if (m_PlatformType == ePublishPlatformType.Android)
                {
                    need_build = true;
                    break;
                }
#elif UNITY_IPHONE
                if (m_PlatformType == ePublishPlatformType.iOS)
                {
                    need_build = true;
                    break;
                }
#elif UNITY_STANDALONE_WIN
                if (m_PlatformType == ePublishPlatformType.Win32 || m_PlatformType == ePublishPlatformType.Win64)
                {
                    need_build = true;
                    break;
                }
#elif UNITY_WEBGL
                if (m_PlatformType == ePublishPlatformType.WebGL)
                {
                    need_build = true;
                    break;
                }
#endif
                if (EditorUtility.DisplayDialog("警告", "当前资源模式与打包平台不一致，是否继续?", "是", "否"))
                {
                    need_build = true;
                    break;
                }
            }while(false);

            if(need_build)
            {
                BuildPackage.StartPublish(m_PublishPath, m_PlatformType, m_PlatformConfig, m_CachePlatformInfo);
            }
        }
        GUILayout.Space(40);
        if (GUILayout.Button("取  消", GUILayout.Width(100), GUILayout.Height(35)))
        {
            Destroy();
        }
        GUILayout.EndHorizontal();
    }

    void DrawBtn(string btnName, System.Action callback, int width, int height)
    {
        if (GUILayout.Button(btnName, GUILayout.Width(width), GUILayout.Height(height)))
            if (callback != null) callback();
    }
    void DrawLine()
    {
        GUILayout.Label("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
    }
    #endregion

}

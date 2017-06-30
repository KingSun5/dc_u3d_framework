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
	}
	
    public void Destroy()
    {
#if UNITY_ANDROID
#elif UNITY_IPHONE
#elif UNITY_STANDALONE_WIN
#elif UNITY_WEBGL
#endif
        this.Close();
    }

    #region 渲染界面
    void OnGUI()
    {
        BeginWindows();
        GUILayout.Window(0, new Rect(20, 20, position.width * 0.5f - 40, position.height * 0.4f), DrawPlatformWindow, "Platforms", GUILayout.Width(position.width - 40));
        EndWindows();

        GUILayout.Space(position.height * 0.4f + 60);
        switch(m_PlatformType)
        {
            case ePublishPlatformType.Android:
                DrawAndroidWindow();
                break;
            case ePublishPlatformType.iOS:
                DrawIOSWindow();
                break;
            case ePublishPlatformType.Win:
                DrawWinWindow();
                break;
            case ePublishPlatformType.WebGL:
                DrawWebGLWindow();
                break;
        }

        DrawDefiedWindow();

        DrawBuildWindow();
    }

    Vector2 mScrollPos;
    void DrawPlatformWindow(int Index)
    {
        PublishPlatformInfo platform_info;
        PublishCacheChannelInfo cache_channel_info;
        mScrollPos = GUILayout.BeginScrollView(mScrollPos, GUILayout.Width(position.width - 60), GUILayout.Height(position.height * 0.4f));
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
        cache_channel_info.IsBuild = EditorGUILayout.Toggle(cache_channel_info.IsBuild, GUILayout.Width(60));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("BundleVersion", GUILayout.Width(100));
        EditorGUILayout.TextField(platform_info.BundleVersion, GUILayout.Width(140));
        GUILayout.Space(20);
        GUILayout.Label("BundleVersionCode", GUILayout.Width(120));
        EditorGUILayout.IntField(platform_info.BundleVersionCode, GUILayout.Width(140));
        GUILayout.Space(20);
        GUILayout.Label("CompileDefine", GUILayout.Width(100));
        EditorGUILayout.TextField(platform_info.CompileDefine, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("PackageName", GUILayout.Width(100));
        EditorGUILayout.TextField(platform_info.PackageName, GUILayout.Width(140));
        GUILayout.Space(20);
        GUILayout.EndHorizontal();
    }

    int OrientationIndex = 3;
    int RenderPathIndex = 2;
    int GraphicsAPIAndIndex = 1;
    int GraphicsAPIIOSIndex = 1;
    int GraphicsAPIWinIndex = 1;
    int ScriptBackemdIndex = 1;
    void DrawAndroidWindow()
    {
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
        GUILayout.Label("Auto Graphics APIS", GUILayout.Width(120));
        m_CachePlatformInfo.AutoGraphicsAPI = EditorGUILayout.Toggle(m_CachePlatformInfo.AutoGraphicsAPI, GUILayout.Width(60));
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

        GUILayout.BeginHorizontal();
        GUILayout.Label("编译环境", GUILayout.Width(100));
        string[] script_options = { ScriptingImplementation.Mono2x.ToString(), ScriptingImplementation.IL2CPP.ToString()};
        ScriptBackemdIndex = EditorGUILayout.Popup(ScriptBackemdIndex, script_options);
        switch (ScriptBackemdIndex)
        {
            case 0: m_CachePlatformInfo.ScriptBackend = eScriptingImplementation.Mono2x; break;
            case 1: m_CachePlatformInfo.ScriptBackend = eScriptingImplementation.IL2CPP; break;
        }
        GUILayout.EndHorizontal();
    }

    void DrawIOSWindow()
    {
        //横屏竖屏,渲染方式
        GUILayout.BeginHorizontal();
        GUILayout.Label("横屏竖屏", GUILayout.Width(60));
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
        GUILayout.Label("Auto Graphics APIS", GUILayout.Width(120));
        m_CachePlatformInfo.AutoGraphicsAPI = EditorGUILayout.Toggle(m_CachePlatformInfo.AutoGraphicsAPI, GUILayout.Width(60));
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

        GUILayout.BeginHorizontal();
        GUILayout.Label("编译环境", GUILayout.Width(100));
        string[] script_options = { ScriptingImplementation.Mono2x.ToString(), ScriptingImplementation.IL2CPP.ToString() };
        ScriptBackemdIndex = EditorGUILayout.Popup(ScriptBackemdIndex, script_options);
        switch (ScriptBackemdIndex)
        {
            case 0: m_CachePlatformInfo.ScriptBackend = eScriptingImplementation.Mono2x; break;
            case 1: m_CachePlatformInfo.ScriptBackend = eScriptingImplementation.IL2CPP; break;
        }
        GUILayout.EndHorizontal();
    }

    void DrawWinWindow()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Auto Graphics APIS", GUILayout.Width(120));
        m_CachePlatformInfo.AutoGraphicsAPI = EditorGUILayout.Toggle(m_CachePlatformInfo.AutoGraphicsAPI, GUILayout.Width(60));
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

        GUILayout.EndHorizontal();
    }

    void DrawWebGLWindow()
    {
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
        m_CachePlatformInfo.EnableUnitySplash = EditorGUILayout.Toggle(m_CachePlatformInfo.EnableUnitySplash, GUILayout.Width(60));

        GUILayout.Space(20);
        GUILayout.Label("Static Batch", GUILayout.Width(80));
        m_CachePlatformInfo.StaticBatch = EditorGUILayout.Toggle(m_CachePlatformInfo.StaticBatch, GUILayout.Width(60));

        GUILayout.Space(20);
        GUILayout.Label("Dynamic Batch", GUILayout.Width(100));
        m_CachePlatformInfo.DynamicBatch = EditorGUILayout.Toggle(m_CachePlatformInfo.DynamicBatch, GUILayout.Width(60));

        GUILayout.Space(20);
        GUILayout.Label("GPU Skin", GUILayout.Width(60));
        m_CachePlatformInfo.GUPSkin = EditorGUILayout.Toggle(m_CachePlatformInfo.GUPSkin, GUILayout.Width(60));
        GUILayout.EndHorizontal();

        //签名
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

    void DrawBuildWindow()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(position.width*0.5f-120);
        if (GUILayout.Button("发布所有", GUILayout.Width(100), GUILayout.Height(35)))
        {
            BeginPublish();
        }
        GUILayout.Space(40);
        if (GUILayout.Button("取消", GUILayout.Width(100), GUILayout.Height(35)))
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


    #region 发布
    /// <summary>
    /// 发布
    /// </summary>
    void BeginPublish()
    {
        string[] scenes = FindEnabledEditorScenes();
        BuildTargetGroup target_group = GetTargetGroupByType(m_PlatformType);
        BuildTarget build_target = GetBuildTargetByType(m_PlatformType);
        BuildAll(scenes, target_group, build_target, BuildOptions.None);
    }
    /// <summary>
    /// 遍历所有需要发布的平台
    /// </summary>
    /// <param name="scenes"></param>
    /// <param name="target_dir"></param>
    /// <param name="build_target"></param>
    /// <param name="build_options"></param>
    void BuildAll(string[] scenes, BuildTargetGroup target_group, BuildTarget build_target, BuildOptions build_options)
    {
        PublishPlatformInfo platform_info;
        PublishCacheChannelInfo cache_channel_info;
        for (int i = 0; i < m_PlatformConfig.list.Count; ++i)
        {
            platform_info = m_PlatformConfig.list[i];
            cache_channel_info = PublishManager.Instance.GetCachaChannelConfig(platform_info.Name);
            if(cache_channel_info.IsBuild)
            {
                BuildOne(scenes, target_group, build_target, build_options, platform_info, cache_channel_info);
            }
        }
    }
    /// <summary>
    /// 发布一个
    /// </summary>
    void BuildOne(string[] scenes, BuildTargetGroup target_group, BuildTarget build_target, BuildOptions build_options, PublishPlatformInfo platform_info, PublishCacheChannelInfo cache_channel_info)
    {
        Log.Info("正在发布版本:" + platform_info.PackageName);

        //设置发布选项
        BuildPackage.BulidTarget(m_PlatformType, platform_info, cache_channel_info, m_CachePlatformInfo);

        EditorUserBuildSettings.SwitchActiveBuildTarget(target_group, build_target);
        BuildPipeline.BuildPlayer(scenes, GetBuildPath(platform_info.PackageName), build_target, build_options);
        Log.Info("发布完成:" + platform_info.PackageName);
    }

    string GetBuildPath(string app_name)
    {
        string target_path = "";
        string target_dir = PublishUtils.GetPublishPath(m_PlatformType);
        switch (m_PlatformType)
        {
            case ePublishPlatformType.Android:
                target_path = target_dir + "/" + app_name + ".apk";
                break;
            case ePublishPlatformType.iOS:
                target_path = target_dir + "/" + app_name + ".ipa";
                break;
            case ePublishPlatformType.Win:
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
    #endregion

    #region 工具类
    public static BuildTarget GetBuildTargetByType(ePublishPlatformType type)
    {
        switch (type)
        {
            case ePublishPlatformType.Android: return BuildTarget.Android;
            case ePublishPlatformType.iOS: return BuildTarget.iOS;
            case ePublishPlatformType.Win: return BuildTarget.StandaloneWindows64;
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
            case ePublishPlatformType.Win: return BuildTargetGroup.Standalone;
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

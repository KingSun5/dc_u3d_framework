using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

/// <summary>
/// ab 功能函数
/// @author hannibal
/// @time 2017-2-27
/// </summary>
public class AssetBundleUtil 
{
    /// <summary>
    /// 打包资源存放目录
    /// </summary>
    public static string PackageAssetPath
    {
        get { return (Application.dataPath + "/" + AssetBundleID.PackageAssetDir + "/").ToLower(); }
    }
    /// <summary>
    /// 下载资源存放目录
    /// </summary>
    public static string DownloadAssetPath
    {
        get
        {
            if (Application.isMobilePlatform)
            {
                string game = AssetBundleID.DownLoadAssetDir.ToLower();
                return Application.persistentDataPath + "/" + game + "/";
            }
            else
            {
                return Application.dataPath + "/" + AssetBundleID.PackageAssetDir + "/";
            }
        }
    }

    public static string GetStreamingAssetsPath()
    {
        if (Application.isEditor)
        {
            return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/");
        }
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
        {
            return Application.streamingAssetsPath;
        }
        else // For standalone player.
        {
            return "file://" + Application.streamingAssetsPath;
        }
    }

    public static string GetRelativePath()
    {
        if (Application.isEditor)
        {
            return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/" + AssetBundleID.PackageAssetDir + "/";
        }
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
        {
            return "file:///" + DownloadAssetPath;
        }
        else // For standalone player.
        {
            return "file://" + Application.streamingAssetsPath + "/";
        }
    }

    public static string GetDownloadedFilePath(string vAssetsPath)
    {
        vAssetsPath = FileUtils.ConfirmSlashAhead(vAssetsPath);

        string dir = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            dir = Application.persistentDataPath;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            dir = Application.persistentDataPath;
        }
        else
        {
            dir = Application.dataPath + "/Document";
        }
        string path = string.Format("{0}{1}", dir, vAssetsPath);

        return path;
    }

    public static string GetHttpPath(string vAssetsPath)
    {
        //GlobalValueManager.SetVersion("1.0");
        vAssetsPath = FileUtils.ConfirmSlashAhead(vAssetsPath);
        string httpServer = "http://192.168.0.248/jjjt_branches/trunk/data/";
#if UNITY_ANDROID
        httpServer += "/Android";
#elif UNITY_IPHONE
		httpServer += "/iOS";
#else
		httpServer += "/Win32";
#endif

        string path = httpServer + vAssetsPath;
        //Debug.Log("download path:" + path);
        return path;
    }
    public static string GetPlatformName()
    {
#if UNITY_EDITOR
        return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
		return GetPlatformForAssetBundles(Application.platform);
#endif
    }

#if UNITY_EDITOR
    private static string GetPlatformForAssetBundles(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.WebGL:
                return "WebGL";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return "OSX";
            default:
                return null;
        }
    }
#endif

    private static string GetPlatformForAssetBundles(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WebGLPlayer:
                return "WebGL";
            case RuntimePlatform.OSXWebPlayer:
            case RuntimePlatform.WindowsWebPlayer:
                return "WebPlayer";
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.OSXPlayer:
                return "OSX";
            default:
                return null;
        }
    }
}

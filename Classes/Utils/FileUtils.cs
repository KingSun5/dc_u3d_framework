using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System;

public class FileUtils
{
    public const string AppName         = "AutoUpdateAssetBundle";  //应用程序名称
    public const string ExtName         = ".assetbundle";           //素材扩展名
    public const string AssetDirname    = "StreamingAssets";      	//素材目录 
    public const string WebUrl          = @"file:///c:/";           //"http://192.168.1.102/";      	//测试更新地址
    public const bool   IdDebugModel    = true;                 	//是否是测试模式，为true 读的是Asset下StreamingAssets的资源

	public const string RESOURCES_PATH = "Resources";
    /// <summary>
    /// 获取不带扩展名的文件
    /// </summary>
	static string GetPathWithNoExtend (string path)
	{
		int idx = path.LastIndexOf (".");
		string noext = path.Substring (0, idx);
		return noext;
	}

	/// <summary>
    /// add "/" to the head
	/// </summary>
	public static string ConfirmSlashAhead (string name)
	{
		name = name.Trim ();
		if (name [0] != '/')
			name = "/" + name;
		return name;
	}
    /// <summary>
    /// 数据目录
    /// </summary>
    public static string AppDataPath
    {
        get { return (Application.dataPath + "/" + AssetDirname + "/").ToLower(); }
    }
    /// <summary>
    /// 下载数据存放目录
    /// </summary>
    public static string DataPath
    {
        get
        {
            string game = AppName.ToLower();
            if (Application.isMobilePlatform)
            {
                return Application.persistentDataPath + "/" + game + "/";
            }
            else
            {
                return Application.dataPath + "/" + AssetDirname + "/";
            }
        }
    }

    public static string GetStreamingAssetsPath()
    {
        if (Application.isEditor)
        {
            return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/");
        }
        else if (Application.isWebPlayer)
        {
            return Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/") + "/StreamingAssets";
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
            return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/" + AssetDirname + "/";
        }
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
        {
            return "file:///" + DataPath;
        }
        else // For standalone player.
        {
            return "file://" + Application.streamingAssetsPath + "/";
        }
    }

	public static string GetDownloadedFilePath (string vAssetsPath)
	{
		vAssetsPath = ConfirmSlashAhead (vAssetsPath);
		
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
		string path = string.Format ("{0}{1}", dir, vAssetsPath);

		return path;
	}

	public static string GetHttpPath (string vAssetsPath)
	{
		//GlobalValueManager.SetVersion("1.0");
		vAssetsPath = ConfirmSlashAhead (vAssetsPath);
		string httpServer ;
        httpServer = "http://192.168.0.248/jjjt_branches/trunk/data/";
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
	
	static public string MD5ByPathName(string pathName)
	{
		try
		{
			FileStream file = new FileStream(pathName, FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();
			
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("MD5ByPathName() fail,error:" + ex.Message);
		}
	}

	/// <summary>
	/// 拷贝目录
	/// </summary>
	static public void CopyFolderTo(string srcDir, string dstDir)
	{
#if UNITY_WEBPLAYER
		//undo
		Log.Error("FileUtils::CopyFolderTo - cannot use in web");
#else
		if(!Directory.Exists(dstDir))
		{
			Directory.CreateDirectory(dstDir);
		}
		
		DirectoryInfo info = new DirectoryInfo(srcDir);
		FileInfo[] files = info.GetFiles();
		foreach (FileInfo file in files)
		{
			string srcFile = srcDir + file.Name;
			string dstFile = dstDir + file.Name;
			if(srcFile.Contains(".xml"))
			{
				Log.Debug("srcFile:" + srcFile + " dstFile:" + dstFile);
				File.Copy(srcFile, dstFile, true);
			}
		}
#endif
	}
}

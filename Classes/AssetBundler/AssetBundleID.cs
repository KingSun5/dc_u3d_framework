using UnityEngine;
using System.Collections;

/// <summary>
/// ab id定义
/// @author hannibal
/// @time 2017-2-27
/// </summary>
public class AssetBundleID 
{
    public const bool   IdDebugModel        = true;                 	    //是否是测试模式，为true 读的是Asset下StreamingAssets的资源
    
    public const string PackageAssetDir     = "StreamingAssets";      	    //资源打包目录 

    public const string DownLoadAssetDir    = "AutoUpdateAssetBundle";      //下载资源目录
    public const string WebUrl              = @"file:///c:/";               //更新地址"http://192.168.1.102/";      	

    public const string RESOURCES_PATH      = "Resources";
}

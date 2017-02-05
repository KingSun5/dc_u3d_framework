using UnityEngine;
using System.Collections;

/// <summary>
/// 资源idmap
/// @author hannibal
/// @time 2016-12-27
/// </summary>
public class ResourceID
{
    //加载事件：本地资源加载
    public const string RESOURCE_LOAD_COMPLATE      = "RESOURCE_LOAD_COMPLATE";                     //资源加载完成
    public const string RESOURCE_LOAD_PROGRESS      = "RESOURCE_LOAD_PROGRESS";                     //资源加载进度
    public const string RESOURCE_LOAD_FAILED        = "RESOURCE_LOAD_FAILED";                       //资源加载失败
    //下载事件：资源服务器下载
    public const string RESOURCE_DOWNLOAD_COMPLATE  = "RESOURCE_DOWNLOAD_COMPLATE";                 //资源下载完成
    public const string RESOURCE_DOWNLOAD_PROGRESS  = "RESOURCE_DOWNLOAD_PROGRESS";                 //资源下载进度
    public const string RESOURCE_DOWNLOAD_FAILED    = "RESOURCE_DOWNLOAD_FAILED";                   //资源下载失败

}

/// <summary>
/// 资源加载类型
/// </summary>
public enum eResType
{
    UNDEFIED = 0,
    PREFAB,         //预制件
    SPRITE,         //2d图片
    TEXTURE,        //纹理
    MATERIAL,       //材质
    SHADER,         //着色器
    ASSET_BUNDLE,   //
    SOUND,          //声音：mp3、ogg、wav
    MOVIE,          //视频，电影： .mov、.mpg、 .mpeg、.mp4、.avi、.asf
}

/// <summary>
/// 资源加载策略
/// </summary>
public enum eResLoadStrategy
{
    FIFO = 0,       //先进先出
    FILO,           //先进后出
    PRIORITY,       //按照优先级加载: eResPriority     
}

/// <summary>
/// 资源优先级
/// </summary>
public enum eResPriority
{

}

/// <summary>
/// 加载线程作用方式
/// </summary>
public enum eResLoadThreadType
{
    SYNC = 0,       //同步
    ASYNC,          //异步
}

/// <summary>
/// 资源加载状态
/// </summary>
public enum eResChunkStage
{
    UNLOAD = 0,     //未加载
    LOADING,        //加载中
    LOADED,         //完成
}
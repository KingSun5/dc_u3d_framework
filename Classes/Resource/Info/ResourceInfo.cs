using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 资源加载信息定义
/// </summary>
public struct sResLoadChunk
{
    public ulong            ID;
    public string           Path;   //路径
    public eResType         Type;   //资源类型
    public eResChunkStage   Stage;  //加载状态
    public float            StartTime; //放入加载队列时间   
    public Action<sResLoadResult> Callback;

    public sResLoadChunk(string path, eResType type, Action<sResLoadResult> call)
    {
        ID = 0;
        Path = path;
        Type = type;
        Stage = eResChunkStage.UNLOAD;
        StartTime = Time.realtimeSinceStartup;
        Callback = call;
    }
}

/// <summary>
/// 异步资源加载结果
/// </summary>
public struct sResLoadResult
{
    public bool IsSucceed;
    public string Path;

    public sResLoadResult(bool b, string path)
    {
        IsSucceed = b;
        Path = path;
    }
}
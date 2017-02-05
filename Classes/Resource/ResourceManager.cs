using UnityEngine;
using System.Collections;

/// <summary>
/// 资源管理
/// @author hannibal
/// @time 2016-12-27
/// </summary>
public class ResourceManager : Singleton<ResourceManager> 
{
    private static ulong m_ShareGUID = 0;
    private ResourceLoadThread m_FrontLoadThread;   //同步加载线程
    private ResourceLoadThread m_BackLoadThread;    //异步加载线程

    public void Setup()
    {
        m_ShareGUID = 0;
        ResourceLoaderManager.Instance.EnableLog = GlobalID.IsLogLoad;

        m_FrontLoadThread = new ResourceLoadSyncThread();
        m_BackLoadThread = new ResourceLoadAsyncThread();

        m_FrontLoadThread.Setup(eResLoadStrategy.FIFO, eResLoadThreadType.SYNC);
        m_BackLoadThread.Setup(eResLoadStrategy.FIFO, eResLoadThreadType.ASYNC);
    }

    public void Destroy()
    {
        if (m_FrontLoadThread != null)
        {
            m_FrontLoadThread.Destroy();
            m_FrontLoadThread = null;
        }
        if (m_BackLoadThread != null)
        {
            m_BackLoadThread.Destroy();
            m_BackLoadThread = null;
        }
    }
    public void Tick(float elapse, int game_frame)
    {
        if (m_FrontLoadThread != null)
        {
            m_FrontLoadThread.Update();
        }
        if (m_BackLoadThread != null)
        {
            m_BackLoadThread.Update();
        }
        HandleGC();
    }

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～同步加载～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    /// <summary>
    /// 增加同步资源加载
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="type">资源类型</param>
    /// <returns></returns>
    public ulong AddSync(string path, eResType type)
    {
        if (m_FrontLoadThread == null || path.Length == 0) return 0;

        ulong id = ShareGUID();
        sResLoadChunk info = new sResLoadChunk(path, type, null);
        info.ID = id;
        m_FrontLoadThread.Add(info);
        return id;
    }

    public void RemoveSync(string path)
    {
        if (m_FrontLoadThread == null) return;

        m_FrontLoadThread.Remove(path);
    }

    public void ClearSync()
    {
        if (m_FrontLoadThread == null) return;

        m_FrontLoadThread.Clear();
    }

    public void StartSync()
    {
        if (m_FrontLoadThread == null) return;

        m_FrontLoadThread.Start();
    }

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～异步加载～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    /// <summary>
    /// 添加异步资源加载
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="type">资源类型</param>
    /// <param name="callback">资源成功或失败回调函数</param>
    /// 例：ResourceManager.Instance.AddAsync("Prefab/Scene/Map_1003", eResType.PREFAB, delegate(sResLoadResult info){});
    /// <returns></returns>
    public ulong AddAsync(string path, eResType type, System.Action<sResLoadResult> callback)
    {
        if (m_BackLoadThread == null) return 0;

        ulong id = ShareGUID();
        sResLoadChunk info = new sResLoadChunk(path, type, callback);
        info.ID = id;
        m_BackLoadThread.Add(info);
        return id;
    }

    public void RemoveAsync(string path)
    {
        if (m_BackLoadThread == null) return;

        m_BackLoadThread.Remove(path);
    }

    public void ClearAsync()
    {
        if (m_BackLoadThread == null) return;

        m_BackLoadThread.Clear();
    }

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～释放～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    private float tmpLastProcessGCTime = 0;
    /// <summary>
    /// 内存自动释放：超过150M，每3分钟执行一次
    /// </summary>
    public void HandleGC()
    {
        if(tmpLastProcessGCTime < Time.realtimeSinceStartup)
        {
            uint total_mem = (uint)(Profiler.GetTotalAllocatedMemory() * MathUtils.BYTE_TO_M);
            if (total_mem > 150)
            {
                ProcessGC();
            }
            tmpLastProcessGCTime = Time.realtimeSinceStartup + 3 * 60;
        }
    }

    public void ProcessGC()
    {
        uint begin_total_mem = (uint)(Profiler.GetTotalAllocatedMemory() * MathUtils.BYTE_TO_K);
        Log.Debug("[mem] begin ProcessGC:" + begin_total_mem);
        ResourceLoaderManager.Instance.Clear();
        ResourceLoaderManager.Instance.UnloadUnusedAssets();
        uint end_total_mem = (uint)(Profiler.GetTotalAllocatedMemory() * MathUtils.BYTE_TO_K);
        Log.Debug("[mem] end ProcessGC:" + end_total_mem + " free mem:" + (begin_total_mem - end_total_mem));
    }

    private ulong ShareGUID()
    {
        return ++m_ShareGUID;
    }

}

using UnityEngine;
using System.Collections;

/// <summary>
/// 资源异步加载
/// @author hannibal
/// @time 2017-1-5
/// </summary>
public class ResourceLoadAsyncThread : ResourceLoadThread
{
    private string          m_CurLoadResPath = "";
    private ResourceRequest m_LoadRequest = null;

    public ResourceLoadAsyncThread()
    {
        
    }

    public override void Setup(eResLoadStrategy strategy, eResLoadThreadType thread_type)
    {
        base.Setup(strategy, thread_type);
        m_Active = true;
    }

    public override void Update()
    {
        if (!m_Active) return;

        ///1.判断现有的是否加载完成
        if(m_LoadRequest != null)
        {
            if(m_LoadRequest.isDone)
            {
                sResLoadChunk info = m_LoadQueue[0];
                if(m_CurLoadResPath == info.Path)
                {
                    ResourceLoaderManager.Instance.AddResource(info.Path, m_LoadRequest.asset);
                    info.Stage = eResChunkStage.LOADED;
                    if (info.Callback != null) info.Callback(new sResLoadResult(true, info.Path));
                    m_LoadQueue.RemoveAt(0);
                    m_TotalCount--;
                }
                else
                {
                    Log.Error("[load] ResourceLoadAsyncThread::Update fatal error");
                }
                m_LoadRequest = null;
            }
        }
        ///2.加载新资源
        if (m_LoadRequest == null && m_LoadQueue.Count > 0)
        {
            ProcessLoad();
        }

        base.Update();
    }

    public override void Destroy()
    {
        m_CurLoadResPath = "";
        m_LoadRequest = null;
        base.Destroy();
    }

    /// <summary>
    /// 添加到加载队列
    /// </summary>
    public override bool Add(sResLoadChunk info)
    {
        return base.Add(info);
    }
    /// <summary>
    /// 从加载队列移除未加载的资源
    /// </summary>
    public override bool Remove(string path)
    {
        if (path == m_CurLoadResPath) return false;
        return base.Remove(path);
    }

    public override void Clear()
    {
        m_CurLoadResPath = "";
        m_LoadRequest = null;
        base.Clear();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Stop()
    {
        base.Stop();
    }

    public override void Pause()
    {
        base.Pause();
    }

    public override void Resume()
    {
        base.Resume();
    }
    /// <summary>
    /// 同步加载
    /// </summary>
    private void ProcessLoad()
    {
        if (m_LoadQueue.Count == 0) return;

        {//执行加载
            sResLoadChunk info = m_LoadQueue[0];
            info.Stage = eResChunkStage.LOADING;
            m_CurLoadResPath = "";

            //先判断是否已经加载过
            Object res = ResourceLoaderManager.Instance.GetResource(info.Path);
            if (res != null)
            {
                info.Stage = eResChunkStage.LOADED;
                if (info.Callback != null) info.Callback(new sResLoadResult(true, info.Path));

                m_LoadQueue.RemoveAt(0);
                m_TotalCount--;
            }
            else
            {
                ResourceRequest req = LoadOnce(info);
                if (req == null)
                {
                    info.Stage = eResChunkStage.LOADED;
                    if (info.Callback != null) info.Callback(new sResLoadResult(false, info.Path));
                    m_LoadQueue.RemoveAt(0);
                    m_TotalCount--;
                }
                else
                {
                    m_CurLoadResPath = info.Path;
                    info.Stage = eResChunkStage.LOADING;
                    m_LoadRequest = req;
                }
                if (GlobalID.IsLogLoad) Log.Debug("[load]async load res:" + info.Path);
            }
        }
    }
    /// <summary>
    /// 加载一个
    /// </summary>
    private ResourceRequest LoadOnce(sResLoadChunk info)
    {
        ResourceRequest req = null;
        switch (info.Type)
        {
            case eResType.UNDEFIED:
            case eResType.PREFAB:
                req = ResourceLoaderManager.Instance.LoadAsync(info.Path);
                break;

            case eResType.SOUND:
                req = ResourceLoaderManager.Instance.LoadAsyncSound(info.Path);
                break;
        }
        //if (GlobalID.IsLogLoad) Log.Info("[load]async end load:" + info.Path);
        return req;
    }
}

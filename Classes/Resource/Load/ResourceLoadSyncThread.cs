using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 异步加载
/// @author hannibal
/// @time 2016-12-27
/// </summary>
public class ResourceLoadSyncThread : ResourceLoadThread
{
    private ResourceRequest m_LoadRequest = null;

    public ResourceLoadSyncThread()
    {

    }

    public override void Setup(eResLoadStrategy strategy, eResLoadThreadType thread_type)
    {
        base.Setup(strategy, thread_type);
        m_Active = false;
    }

    public override void Update()
    {
        if (!m_Active) return;

        ///1.判断现有的是否加载完成
        if (m_LoadRequest != null)
        {
            if (m_LoadRequest.isDone)
            {
                sResLoadChunk info = m_LoadQueue[0];

                ResourceLoaderManager.Instance.AddResource(info.Path, m_LoadRequest.asset);
                info.Stage = eResChunkStage.LOADED;
                m_LoadQueue.RemoveAt(0);
                m_LoadRequest = null;
                EventController.TriggerEvent(ResourceID.RESOURCE_LOAD_PROGRESS, m_TotalCount - m_LoadQueue.Count, m_TotalCount, info.Path);

                CheckLoadComplate();
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
        m_LoadRequest = null;
        base.Destroy();
    }

    /// <summary>
    /// 添加到加载队列
    /// </summary>
    public override bool Add(sResLoadChunk info)
    {
        if (m_Active)
        {
            Log.Error("加载队列正在执行:" + info.Path);
            return false;
        }
        foreach (var item in m_LoadQueue)
        {
            if (item.Path == info.Path)
            {
                Log.Warning("ResourceLoadAsyncThread::Add - 相同资源已经在队列中:" + info.Path);
                break;
            }
        }
        return base.Add(info);
    }
    /// <summary>
    /// 从加载队列移除未加载的资源
    /// </summary>
    public override bool Remove(string path)
    {
        return base.Remove(path);
    }

    public override void Clear()
    {
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

            //先判断是否已经加载过
            Object res = ResourceLoaderManager.Instance.GetResource(info.Path);
            if (res != null)
            {
                info.Stage = eResChunkStage.LOADED;
                m_LoadQueue.RemoveAt(0);
                EventController.TriggerEvent(ResourceID.RESOURCE_LOAD_PROGRESS, m_TotalCount - m_LoadQueue.Count, m_TotalCount, info.Path);
            }
            else
            {
                ResourceRequest req = LoadOnce(info);
                if (req == null)
                {
                    info.Stage = eResChunkStage.LOADED;
                    m_LoadQueue.RemoveAt(0);
                    EventController.TriggerEvent(ResourceID.RESOURCE_LOAD_PROGRESS, m_TotalCount - m_LoadQueue.Count, m_TotalCount, info.Path);
                }
                else
                {
                    info.Stage = eResChunkStage.LOADING;
                    m_LoadRequest = req;
                }
                if (GlobalID.IsLogLoad) Log.Debug("[load]sync load res:" + info.Path);
            }
        }
        CheckLoadComplate();
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

    /// <summary>
    /// 判断是否完成
    /// </summary>
    private bool CheckLoadComplate()
    {
        if (m_LoadQueue.Count == 0)
        {
            Log.Info("[load]load complate");
            EventController.TriggerEvent(ResourceID.RESOURCE_LOAD_COMPLATE, m_TotalCount);
            Stop();
            return true;
        }
        return false;
    }
}

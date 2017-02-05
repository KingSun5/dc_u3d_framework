using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 负责资源加载管理：分同步和异步
/// @author hannibal
/// @time 2016-12-27
/// </summary>
public class ResourceLoadThread
{
    protected bool                  m_Active;
    protected eResLoadStrategy      m_LoadStrategy;
    protected eResLoadThreadType    m_LoadThreadType;

    protected int                   m_TotalCount;
    protected List<sResLoadChunk>   m_LoadQueue;

    public ResourceLoadThread()
    {

    }

    public virtual void Setup(eResLoadStrategy strategy, eResLoadThreadType thread_type)
    {
        m_TotalCount = 0;
        m_LoadStrategy = strategy;
        m_LoadThreadType = thread_type;
        m_LoadQueue = new List<sResLoadChunk>();
    }

    public virtual void Update()
    {
    }

    public virtual void Destroy()
    {
        m_Active = false;
        m_TotalCount = 0;
        if (m_LoadQueue != null)
        {
            m_LoadQueue.Clear();
            m_LoadQueue = null;
        }
    }

    /// <summary>
    /// 添加到加载队列
    /// </summary>
    public virtual bool Add(sResLoadChunk info)
    {
        if (GlobalID.IsLogLoad) Log.Info("[load]add load info:" + info.Path);
        switch(m_LoadStrategy)
        {
            case eResLoadStrategy.FIFO:
                m_LoadQueue.Add(info);
                break;

            case eResLoadStrategy.FILO:
                m_LoadQueue.Insert(0, info);
                break;

            case eResLoadStrategy.PRIORITY:
                //TODO
                m_LoadQueue.Add(info);
                break;

            default:
                m_LoadQueue.Add(info);
                break;
        }
        m_TotalCount++;
        return true;
    }
    /// <summary>
    /// 从加载队列移除未加载的资源
    /// </summary>
    public virtual bool Remove(string path)
    {
        foreach (var info in m_LoadQueue)
        {
            if(info.Path == path && info.Stage == eResChunkStage.UNLOAD)
            {//只有处于未加载状态的资源才能取消
                m_LoadQueue.Remove(info);
                m_TotalCount--;
                return true;
            }
        }
        return false;
    }

    public virtual void Clear()
    {
        m_LoadQueue.Clear();
        m_TotalCount = 0;
    }

    public virtual void Start()
    {
        m_Active = true;
        if (GlobalID.IsLogLoad) Log.Info("[load]begin load total:" + m_TotalCount);
    }

    public virtual void Stop()
    {
        m_Active = false;
    }

    public virtual void Pause()
    {
        m_Active = false;
    }

    public virtual void Resume()
    {
        m_Active = true;
    }
}

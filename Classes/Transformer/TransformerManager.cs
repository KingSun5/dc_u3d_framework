using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 变换管理器
/// @author hannibal
/// @time 2017-4-5
/// </summary>
public class TransformerManager : Singleton<TransformerManager>
{
    private List<Transformer> m_UpdateAllList = new List<Transformer>();

    public void Setup()
    {
    }

    public void Destroy()
    {
        StopAll();
        m_UpdateAllList.Clear();
    }

    public void Tick(float elapse, int game_frame)
    {
        update(Time.realtimeSinceStartup);
    }

    private void update(float currTime)
	{
        for (int i = m_UpdateAllList.Count - 1; i > -1; --i)
        {
            m_UpdateAllList[i].update(currTime);
            if (m_UpdateAllList[i].completed())
                m_UpdateAllList.RemoveAt(i);
        }
	}

    public void Add(Transformer transformer)
    {
        m_UpdateAllList.Add(transformer);
    }

    public void Remove(ref Transformer transformer)
    {
        if (transformer != null)
            transformer.stop();
        transformer = null;
    }

    //停掉对象上所有的Transformer
    public void StopByTarget(GameObject target)
    {
        for (int i = m_UpdateAllList.Count - 1; i > -1; --i)
        {
            if (m_UpdateAllList[i].target == target)
                m_UpdateAllList.RemoveAt(i);
        }
    }
    public void StopTargetByType(GameObject target, eTransformerID type)
    {
        for (int i = m_UpdateAllList.Count - 1; i > -1; --i)
        {
            if (m_UpdateAllList[i].target == target && m_UpdateAllList[i].Type == type)
                m_UpdateAllList.RemoveAt(i);
        }
    }
    //停止所有变换器
    public void StopAll()
    {
        for(int i = 0; i < m_UpdateAllList.Count; ++i)
        {
            m_UpdateAllList[i].stop();
        }
    }
}
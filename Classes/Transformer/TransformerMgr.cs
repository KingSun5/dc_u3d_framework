using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class TransformerMgr : Singleton<TransformerMgr>
{
    private List<Transformer> m_UpdateAllList = new List<Transformer>();
    public void Add(Transformer transformer)
    {
        m_UpdateAllList.Add(transformer);
    }
    public virtual bool Tick(float elapse, int game_frame)
    {
        update(Time.realtimeSinceStartup);
        return true;
    }

    public void update(float currTime)
	{
        for (int i = m_UpdateAllList.Count - 1; i > -1; --i)
        {
            m_UpdateAllList[i].update(currTime);
            if (m_UpdateAllList[i].completed())
                m_UpdateAllList.RemoveAt(i);
        }
	}

    //停掉对象上所有的Transformer
    public void stopByTarget(GameObject target)
    {
        for (int i = m_UpdateAllList.Count - 1; i > -1; --i)
        {
            if (m_UpdateAllList[i].target == target)
                m_UpdateAllList.RemoveAt(i);
        }
    }

    //停止所有变换器
    public void stop()
    {
        m_UpdateAllList.Clear();
    }
    public void Remove(ref Transformer transformer)
    {
        if (transformer != null)
            transformer.stop();
        transformer = null;
    }
}
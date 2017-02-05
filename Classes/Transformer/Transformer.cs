using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/**
 * 显示对象变换器
 * 显示对象变换器提供了统一抽象的显示对象自由变换操作方式。对于任何显示对象，
 * 其可进行的变换包括缩放、旋转、平移、颜色变换以及是否可见的变换中的一种或多
 * 种的组合。通过使用不同的变换类，可实现不同的组合变换效果来呈现更为丰富和具
 * 有动感的显示效果。
 *
 * 显示对象变换器支持相互连接和并排，可以讲变换器C连接到另一个变换器B上，则C
 * 被连接后将在B的变换结束后开始执行变换；同样可以讲变换器B并排到变换器A上，
 * 则A被运行时B也同时开始执行变换。基于此特性可以十分轻松的构造出一个变换器树，
 * 树在运行期间既具有顺序变换，也具有同时变换。
 *
 * 被连接或并排的变换器无需调用start函数启动变换器，只需对变换树的根节变换器
 * 调用start函数起动变换，则整个树中的所有变换器会按照顺序或并排关系在正确的
 * 时间点被启动。
 *
 */
public class Transformer
{
    GameObject m_Target = null;
    public float m_fStartTime = 0;//变换开始时间
    public float m_fEndTime = 0;//变换结束时间
    public float m_fTransformTime = 0;//变换持续时间
    public float m_fRootTimeOffset = 0;//在变换树中的时间偏移
    public Transformer m_Root = null; //根变换器
    List<Transformer> m_ChildrenList = new List<Transformer>();//子级变换器列表
    public bool m_boEnded = false;//是否已结束自身变换
    public bool m_boAllChildrenEnded = false;//是否已结束整个变换树
    public bool m_boSelfControlChildren = false;//是否由变换器自身控制子成员列表



    //返回变换器是否正在运行
    public bool runing()
    {
        return m_fStartTime != 0 && !completed();
    }

    //返回变换器是否完成
    public bool completed()
    {
        return m_boEnded && m_boAllChildrenEnded;
    }

		//停止变换
    public void stop()
    {
        m_fEndTime = 0;
        m_boAllChildrenEnded = true;
        m_boEnded = true;
    }

    public GameObject target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }

    public Transformer root() 
    { 
        return m_Root != null ? m_Root : this; 
    }

    //连接一个变换器到此变换器之后
    //被连接的变换器将在此变换器变换结束后开始执行变换
    //函数返回值为被连接的变换器，可以使用a->concat(b)->concat(c)方式
    //组合一个变换器列表，这些变换器将按顺序一个接一个执行变换。
    //如果欲连接的变换器已经与其他变换器连接或并排，则操作将失败并返回NULL。
    public Transformer concat(Transformer transformer)
    {
        if (transformer.m_Root != null || transformer == this || m_Root == transformer)
            return null;
		Transformer root = (!m_boSelfControlChildren && m_Root != null) ? m_Root : this;
		transformer.m_fRootTimeOffset = m_fRootTimeOffset + m_fTransformTime;
		transformer.m_Root = root;
        root.m_ChildrenList.Add(transformer);
		return transformer;
    }

    //并排一个变换器到此变换器位置
    //被并排的变换器将与从变换器同时开始执行变换
    //函数返回值为被连接的变换器，可以使用a->abreast(b)->abreast(c)方式
    //组合一个并列的变换器列表，这些变换器将同时执行变换。
    //如果欲连接的变换器已经与其他变换器连接或并排，则操作将失败并返回NULL。
    public Transformer abreast(Transformer transformer)
    {
        if (transformer.m_Root != null || transformer == this || m_Root == transformer)
			return null;
		Transformer root = (!m_boSelfControlChildren && m_Root != null) ? m_Root : this;
		transformer.m_fRootTimeOffset = m_fRootTimeOffset;
		transformer.m_Root = root;
        root.m_ChildrenList.Add(transformer);
        return transformer;
    }

    //开始运行变换器
    //参数taregtStage为变换器运行所处的目标舞台；
    //参数timeOffset表示变换器开始运行起始的时间偏移，例如想跳过变换器前
    //0.5秒的变换而直接从0.5秒开始执行变换，则timeOffset参数值指定为0.5
    //即可达到所需效果。如果指定负数值，则将起到延迟变换起始时间的效果；
    //参数autoManage表示是否使用默认变换管理器进行自动更新以及生命期管理，
    //使用变换器管理器将能够以统一的方式对所有任意时刻产生的变换器进行自动
    //化的更新以及生命期管理，变换器管理器能够在变换器运行完成后自动尝试移
    //除此变换器。使用变换器时仅需要调用变换器的更新函数即可实现自动更新，
    //如果使用SG2DEX::Application框架，则无需手动调用变换管理器的更新，
    //Application在进行帧更新前会自动更新变换管理器；
    public void start(float timeOffset = 0, bool autoManage = true)
    {
        m_boEnded = false;
        m_boAllChildrenEnded = false;
        m_fStartTime = Time.realtimeSinceStartup - timeOffset;
        m_fEndTime = m_fStartTime + m_fTransformTime;
        resetAllChildren();
        transformStarted();
        if (autoManage)
        {
            TransformerMgr.Instance.Add(this);
        }
    }


    public virtual void update(float currTime)
	{
		if (!m_boEnded && currTime >= m_fStartTime)//支持延迟启动
		{
			runTransform(currTime);
			if (currTime >= m_fEndTime)
			{
				m_boEnded = true;
				transformCompleted();
			}
		}

		if (!m_boAllChildrenEnded)
		{
			int aliveChildrenCount = 0;

            int count = m_ChildrenList.Count;
            float timeOffset = currTime - m_fStartTime;
            aliveChildrenCount = count;
            for (int i = 0; i < m_ChildrenList.Count; ++i)
            {
                Transformer transformer = m_ChildrenList[i];
                if (timeOffset >= transformer.m_fRootTimeOffset)
                {
                    if (transformer.m_fStartTime == 0)
                        transformer.start(timeOffset - transformer.m_fRootTimeOffset, false);
                    transformer.update(currTime);
                    if (transformer.completed())
                        aliveChildrenCount--;
                }
            }
			if (aliveChildrenCount == 0)
				m_boAllChildrenEnded = true;
		}
	}

    //重置所有子孙变换器
    public void resetAllChildren()
    {
        for (int i = 0; i < m_ChildrenList.Count; i++)
        {
            Transformer children = m_ChildrenList[i];
            children.m_fStartTime = 0;
        }
    }
    //循环更新变换器
    public virtual void runTransform(float currTime)
    {

    }
    //变换器开始更新变换函数
    public virtual void transformStarted()
    {

    }
    //变换器完成变换函数
    public virtual void transformCompleted()
    {

    }
}
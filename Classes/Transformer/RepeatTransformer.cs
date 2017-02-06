using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/**
 * 重复运行变换器
 */
public class RepeatTransformer : Transformer
{
    public int m_nRunTwice;
    public int m_nMaxTwice;
    public static RepeatTransformer repeat(GameObject target, int twice)
    {
        RepeatTransformer transformer = new RepeatTransformer();
        transformer.m_nRunTwice = 0;
        transformer.m_nMaxTwice = twice;
        transformer.target = target;
        transformer.m_fTransformTime = 0;
        transformer.m_boSelfControlChildren = true;
        return transformer;
    }
    public override void transformStarted()
	{
		m_fEndTime = m_fStartTime + 72 * 60 * 60;
	}
    public override void runTransform(float currTime)
	{
		if (m_boAllChildrenEnded)
		{
			m_nRunTwice++;
			if (m_nMaxTwice != 0 && m_nRunTwice >= m_nMaxTwice)
			{
				transformCompleted();
				m_fEndTime = currTime;
			}
			else
			{
				m_fStartTime = currTime;
				resetAllChildren();
				m_boAllChildrenEnded = false;
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/**
 * 回调变换器
 */
public class ActionTransformer : Transformer
{
    public Action m_CallBack;
    public static ActionTransformer FadeTo(GameObject target, Action callBack, float time)
    {
        ActionTransformer transformer = new ActionTransformer();
        transformer.m_CallBack = callBack;
        transformer.m_fTransformTime = time;
        transformer.target = target;
        return transformer;
    }

    public override void runTransform(float currTime)
    {
        if (currTime >= m_fEndTime)
        {
            m_CallBack();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/**
 * 空变换器，占据变换时间而不做任何变换
 */
public class NullTransformer : Transformer
{
    public static NullTransformer holdTime(GameObject target, float time)
    {
        NullTransformer result = new NullTransformer();
        result.m_fTransformTime = time;
        result.target = target;
        return result;
    }
    public override void runTransform(float currTime)
    {

    }
}
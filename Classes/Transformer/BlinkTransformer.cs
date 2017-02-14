using UnityEngine;
using System.Collections;

/// <summary>
/// 闪烁变换器
/// @author hannibal
/// @time 2017-2-14
/// </summary>
public class BlinkTransformer : Transformer 
{
    public bool m_boVisible;       //变换完成后目标可视状态
    public bool m_defaultVisible;  //默认是否可见
    public float m_switchTiime;    //变换时间
    public float m_lastSwitchTime;

    /// <summary>
    /// 构建器
    /// </summary>
    /// <param name="target">目标对象</param>
    /// <param name="time">变换间隔：一次显示或隐藏的时长</param>
    /// <param name="count">变换次数，显示和隐藏分别算一次变换</param>
    /// <param name="defaultVisible">默认是否可见，变换结束后，会回到默认状态</param>
    /// <returns></returns>
    public static BlinkTransformer blink(GameObject target, float time, int count, bool defaultVisible=true)
    {
        BlinkTransformer transformer = new BlinkTransformer();
        transformer.m_boVisible = defaultVisible;
        transformer.m_defaultVisible = defaultVisible;
        transformer.m_switchTiime = time;
        transformer.m_lastSwitchTime = 0;
        transformer.m_fTransformTime = count * time;
        transformer.target = target;
        transformer.target.SetActive(transformer.m_boVisible);
        return transformer;
    }
    public BlinkTransformer()
    {
        m_Type = eTransformerID.Blink;
    }
    public override void runTransform(float currTime)
    {
        if (currTime >= m_fEndTime)
        {
            target.SetActive(m_defaultVisible);
            return;
        }

        m_lastSwitchTime += Time.deltaTime;
        if (m_lastSwitchTime >= m_switchTiime)
        {
            m_lastSwitchTime = 0;
            m_boVisible = !m_boVisible;
            target.SetActive(m_boVisible);
        }
    }
}

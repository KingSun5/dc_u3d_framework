using UnityEngine;
using System.Collections;

/// <summary>
/// 位移变换器
/// @author hannibal
/// @time 2016-2-14
/// </summary>
public class MoveTransformer : Transformer 
{
    public int m_nStartType;
    public float m_fStartX;
    public float m_fStartY;
    public float m_fSpeedX;
    public float m_fSpeedY;
    public float m_fTargetX;
    public float m_fTargetY;

    /// <summary>
    /// 移动到目标点
    /// </summary>
    /// <param name="target">目标对象</param>
    /// <param name="x">目标位置：x</param>
    /// <param name="y">目标位置：y</param>
    /// <param name="time">变换时长</param>
    /// <returns></returns>
    public static MoveTransformer moveTo(GameObject target, float x, float y, float time)
    {
        MoveTransformer transformer = new MoveTransformer();
        transformer.m_nStartType = 0;
        transformer.m_fTargetX = x;
        transformer.m_fTargetY = y;
        transformer.m_fTransformTime = time;
        transformer.target = target;
        return transformer;
    }
    /// <summary>
    /// 基于当前点移动
    /// </summary>
    /// <param name="target">目标对象</param>
    /// <param name="speedX">x方向速度</param>
    /// <param name="speedY">y方向速度</param>
    /// <param name="time">变换时长</param>
    /// <returns></returns>
    public static MoveTransformer moveBy(GameObject target, float speedX, float speedY, float time)
    {
        MoveTransformer transformer = new MoveTransformer();
        transformer.m_nStartType = 1;
        transformer.m_fSpeedX = speedX;
        transformer.m_fSpeedY = speedY;
        transformer.m_fTransformTime = time;
        transformer.target = target;
        return transformer;
    }
    public MoveTransformer()
    {
        m_Type = eTransformerID.Move;
    }
    public override void OnTransformStarted()
    {
        //获得当前对象的缩放
        Vector3 position = target.transform.localPosition;
        m_fStartX = position.x;
        m_fStartY = position.y;
        if (m_nStartType == 0)
        {
            m_fSpeedX = (m_fTargetX - position.x) / m_fTransformTime;
            m_fSpeedY = (m_fTargetY - position.y) / m_fTransformTime;
        }
        else if (m_nStartType == 1)
        {
            m_fTargetX = position.x + m_fSpeedX * m_fTransformTime;
            m_fTargetY = position.x + m_fSpeedY * m_fTransformTime;
        }
        base.OnTransformStarted();
    }
    public override void runTransform(float currTime)
    {
        if (currTime >= m_fEndTime)
        {
            target.transform.localPosition = new Vector3(m_fTargetX, m_fTargetY, target.transform.localPosition.z);
        }
        else
        {
            float timeElapased = currTime - m_fStartTime;
            target.transform.localPosition = new Vector3(m_fStartX + m_fSpeedX * timeElapased, m_fStartY + m_fSpeedY * timeElapased, target.transform.localPosition.z);
        }
    }
}

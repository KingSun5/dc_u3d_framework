using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]

/// <summary>
/// 震屏效果
/// @author hannibal
/// @time 2016-11-15
/// </summary>
public class ShakeCameraScript : MonoBehaviour
{
    public float    m_TotalTime = 1;    //总时间
    public float    m_Power = 1;        //强度
    public float    m_Interval = 0.03f; //间隔
    private float   m_PassTime = 0;

    private float   tmpLastShakeTime = 0;
    private Vector3 tmpDeltaPos;
    public void StartShake(float total_time, float power, float interval = 0.03f)
    {
        m_TotalTime = total_time;
        m_PassTime = 0;
        m_Power = power;
        m_Interval = interval;
        tmpDeltaPos = Vector3.zero;
        tmpLastShakeTime = Time.realtimeSinceStartup;
    }

    void LateUpdate()
    {
        if (m_PassTime > m_TotalTime)return;

        if (Time.realtimeSinceStartup - tmpLastShakeTime < m_Interval)return;

        tmpLastShakeTime += m_Interval;
        m_PassTime += Time.deltaTime;

        transform.localPosition -= tmpDeltaPos;
        tmpDeltaPos = Random.insideUnitSphere / 3.0f * m_Power;
        transform.localPosition += tmpDeltaPos;
    }
}
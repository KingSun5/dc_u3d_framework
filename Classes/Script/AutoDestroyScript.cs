using UnityEngine;
using System.Collections;

/// <summary>
/// 超时自动销毁
/// @author hannibal
/// @time 2016-11-18
/// </summary>
public class AutoDestroyScript : MonoBehaviour 
{
    private float   m_StartTime = 0;
    public float    m_TotalTime = 0;
    public bool     m_AutoDestroy = true;

    public delegate void FunComplate();
    public FunComplate m_DestroyCallback = null;
	
	void OnEnable () 
    {
        m_StartTime = Time.realtimeSinceStartup;
	}
	
	void Update ()
    {
	    if(Time.realtimeSinceStartup - m_StartTime >= m_TotalTime)
        {
            if(m_DestroyCallback != null) m_DestroyCallback();
            if(m_AutoDestroy) GameObject.Destroy(gameObject);
        }
	}
}

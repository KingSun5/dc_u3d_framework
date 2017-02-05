using UnityEngine;
using System.Collections;

/// <summary>
/// 自动消耗粒子系统对象
/// @author hannibal
/// @time 2016-12-5
/// </summary>
public class ParticleAutoDestroyScript : MonoBehaviour
{
    public float    m_TotalTime = 3;
    public bool     m_AutoDestroy = true;

    private ParticleSystem[] particleSystems;

    public delegate void FunComplate();
    public FunComplate m_DestroyCallback = null;

    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        if (particleSystems != null && particleSystems.Length > 0) m_TotalTime = 0;
    }

    void Update()
    {
        if ((particleSystems == null || particleSystems.Length == 0) && m_TotalTime <= 0) return;

        bool allStopped = true;

        if ((particleSystems != null && particleSystems.Length != 0))
        {
            for (int i = 0; i < particleSystems.Length; ++i)
            {
                ParticleSystem ps = particleSystems[i];
                if (ps && !ps.isStopped)
                {
                    allStopped = false;
                }
            }
        }
        else if (m_TotalTime > 0)
        {
            m_TotalTime -= Time.deltaTime;
            if (m_TotalTime > 0)
                allStopped = false;
        }

        if (allStopped)
        {
            if (m_DestroyCallback != null) m_DestroyCallback();
            if (m_AutoDestroy) GameObject.Destroy(gameObject);
        }
    } 
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

/// <summary>
/// 自动消耗声音脚本
/// @author hannibal
/// @time 2016-12-16
/// </summary>
public class AutoDestroyAudio : MonoBehaviour
{
    private bool        m_Enable;

    private AudioSource m_AudioCompnent;      // 音频
    private float       m_StartTime = 0;      // 记录开始时间
    private int         m_PlayCount = 1;      // 是否循环：循环音效需要手动清除
    private float       m_DespawnDelay=0;     // 消失时间：如果不设置，默认不播放时会自动销毁

    public delegate void FunComplate();
    private FunComplate m_DestroyCallback = null;

    void Awake()
    {
    }

    void OnEnable()
    {
        m_Enable = true;
        m_StartTime = Time.realtimeSinceStartup;
        m_AudioCompnent = GetComponent<AudioSource>();
    }

    void OnDisable()
    {
    }

    /// <summary>
    /// 处理消失
    /// </summary>
    public void Despawn()
    {
        m_Enable = false;
        if (m_DestroyCallback != null) m_DestroyCallback();
    }

    void Update()
    {
        if (!m_Enable) return;

        if (m_DespawnDelay > 0)
        {
            if (Time.realtimeSinceStartup - m_StartTime > m_DespawnDelay)
            {
                Despawn();
            }
        }
        else
        {
            if (m_AudioCompnent != null && !m_AudioCompnent.isPlaying)
            {
                Despawn();
            }
        }
    }
    public int PlayCount
    {
        set 
        {
            m_PlayCount = value;

            if (m_PlayCount > 1)
                m_DespawnDelay = m_PlayCount * m_AudioCompnent.clip.length;
            else
                m_DespawnDelay = 0;
        }
    }
    public FunComplate DestroyCallback
    {
        set { m_DestroyCallback = value; }
    }
    public bool Enable
    {
        get { return m_Enable; }
    }
}

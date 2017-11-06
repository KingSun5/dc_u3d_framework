using UnityEngine;
using System.Collections;

/// <summary>
/// 挂到对象上播放的声音
/// @author hannibal
/// @time 2016-12-11
/// </summary>
public class ObjPlaySound : MonoBehaviour
{
    /// <summary>
    /// 触发条件
    /// </summary>
    public enum eTriggerType
    {
        Active,     //对象激活时播放
        External,   //外部调用
    }

    public AudioClip m_AudioClip;
    public eTriggerType m_TriggerTime = eTriggerType.Active;
    public bool m_Loop = false;
    public float m_MinDistance = 5;
    public float m_MaxDistance = 50;

    private AudioSource m_AudioSource = null;

    void Awake()
    {
    }

    void OnEnable()
    {
        if (m_TriggerTime == eTriggerType.Active)
        {
            play();
        }
    }
    void OnDisable()
    {
        if (m_TriggerTime == eTriggerType.Active)
        {
            stop();
        }
    }
    void OnDestroy()
    {
        stop();
    }
    void Update()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.transform.position = transform.position;
        }
    }

    /// <summary>
    /// 外部调用
    /// </summary>
    public void Play()
    {
        play();
    }
    public void Stop()
    {
        stop();
    }

    void play()
    {
        if (m_AudioClip != null)
        {
            m_AudioSource = SoundManager.Instance.PlaySoundEffect(m_AudioClip, transform.position, m_MinDistance, m_MaxDistance, m_Loop);
        }
    }
    void stop()
    {
        if (m_AudioSource != null)
        {
            SoundManager.Instance.StopSoundEffect(m_AudioSource);
            m_AudioSource = null;
        }
    }
}

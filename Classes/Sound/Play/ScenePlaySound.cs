using UnityEngine;
using System.Collections;

/// <summary>
/// 场景环境音效
/// @author hannibal
/// @time 2016-12-11
/// </summary>
public class ScenePlaySound : MonoBehaviour 
{
    /// <summary>
    /// 触发条件
    /// </summary>
    public enum eTriggerType
    {
        Enter,      //对象进入范围时播放
        External,   //外部调用
    }

    public AudioClip    m_AudioClip;
    public eTriggerType m_TriggerTime = eTriggerType.Enter;
    public bool         m_Loop = true;
    public float        m_Range = 50;
    public Color        m_color = new Color(0.2f,0.2f,0.2f,0.5f);
    
    private AudioSource m_AudioSource = null;
    private Transform   m_Listener;     //听众 

    void Awake()
    {
    }

    void OnEnable()
    {
        EventDispatcher.AddEventListener(SoundID.SOUND_LISTENER_ENTER, OnListenerEvt);
        EventDispatcher.AddEventListener(SoundID.SOUND_LISTENER_LEAVE, OnListenerEvt);
    }
    void OnDisable()
    {
        EventDispatcher.RemoveEventListener(SoundID.SOUND_LISTENER_ENTER, OnListenerEvt);
        EventDispatcher.RemoveEventListener(SoundID.SOUND_LISTENER_LEAVE, OnListenerEvt);
        stop();
    }

    void OnListenerEvt(GameEvent evt)
    {
        switch(evt.type)
        {
            case SoundID.SOUND_LISTENER_ENTER:
                m_Listener = evt.Get<Transform>(0);
                break;
            case SoundID.SOUND_LISTENER_LEAVE:
                m_Listener = null;
                stop();
                break;
        }
    }

    private float tmpCalListenerTime = 1;
    private float tmpLastCalTime = 0;
    void Update()
    {
        if (m_Listener == null) return;

        if(Time.realtimeSinceStartup - tmpLastCalTime > tmpCalListenerTime)
        {
            Vector3 listener_pos = m_Listener.gameObject.transform.position;
            if(Vector3.Distance(listener_pos, transform.position) <= m_Range)
            {
                play();
            }
            else
            {
                stop();
            }
            tmpLastCalTime = Time.realtimeSinceStartup;
        }
    }
    void OnDestroy()
    {
        if(m_AudioSource != null)
        {
            SoundManager.Instance.StopSoundEffect(m_AudioSource);
            m_AudioSource = null;
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
            if(m_AudioSource == null)
            {
                m_AudioSource = SoundManager.Instance.PlaySoundEffect(m_AudioClip, transform.position, 0, m_Range, m_Loop);
                if (m_AudioSource == null) return;
                AutoDestroyAudio component = m_AudioSource.GetComponent<AutoDestroyAudio>();
                if (component != null) component.IsLoop = m_Loop;
            }
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
    }
    void stop()
    {
        if (m_AudioSource != null && m_AudioSource.isPlaying)
        {
            m_AudioSource.Stop();
        }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        DrawSphere();
#endif
    }

    void DrawSphere()
    {
        if (m_Range > 0)
        {
            // 设置矩阵
            Matrix4x4 defaultMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            // 设置颜色
            Color defaultColor = Gizmos.color;
            Gizmos.color = m_color;

            Gizmos.DrawSphere(transform.localPosition, m_Range);

            // 恢复默认颜色
            Gizmos.color = defaultColor;

            // 恢复默认矩阵
            Gizmos.matrix = defaultMatrix;
        }
    }
}



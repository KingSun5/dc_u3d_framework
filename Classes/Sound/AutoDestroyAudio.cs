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
    private float       StartTime = 0;      // 记录开始时间
    public bool         IsLoop = false;     // 是否循环：循环音效需要手动清除
    public float        DespawnDelay;       // 消失时间：如果不设置，默认不播放时会自动销毁
    private AudioSource AudioCompnent;      // 音频

    public delegate void FunComplate();
    public FunComplate m_DestroyCallback = null;

    void Awake()
    {
    }

    void OnEnable()
    {
        StartTime = Time.realtimeSinceStartup;
        AudioCompnent = GetComponent<AudioSource>();
    }

    void OnDisable()
    {
    }

    /// <summary>
    /// 处理消失
    /// </summary>
    public void Despawn()
    {
        if (m_DestroyCallback != null) m_DestroyCallback();
    }

    void Update()
    {
        if (IsLoop) return;

        if (DespawnDelay > 0)
        {
            if (Time.realtimeSinceStartup - StartTime > DespawnDelay)
            {
                Despawn();
            }
        }
        else
        {
            if(AudioCompnent != null && !AudioCompnent.isPlaying)
            {
                Despawn();
            }
        }
    }
}

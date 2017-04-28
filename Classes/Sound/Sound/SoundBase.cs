using UnityEngine;
using System.Collections;

/// <summary>
/// 声音基类
/// @author hannibal
/// @time 2017-1-14
/// </summary>
public class SoundBase : IPoolsObject
{
    public static string POOLS_SOUND_BG     = "POOLS_SOUND_BG";
    public static string POOLS_SOUND_EFFECT = "POOLS_SOUND_EFFECT";

    protected ulong m_ObjectUID = 0;    //对象唯一ID
    protected bool  m_Active = false; 	//是否激活中
    protected bool  m_IsPlay = false;

    protected string    m_FileName;
    protected Transform m_ParentNode;
    protected Vector3   m_Position;
    protected float     m_MinDistance;
    protected float     m_MaxDistance;
    protected int       m_PlayCount;

    protected AudioSource m_SoundSource = null;

    public SoundBase()
    {
    }
    public virtual string GetPoolsType()
    {
        return "";
    }

    public virtual void Init()
    {
    }
    public virtual void Release()
    {
        if (m_SoundSource != null)
        {
            m_SoundSource.Stop();
            AudioPools.instance.DespawnAudio(m_SoundSource.transform);
            m_SoundSource = null;
        }
    }
    public virtual void Setup(string fileName, Vector3 pos, Transform parent, float min_distance, float max_distance, int count = 1)
    {
        m_Active = true;
        m_FileName = fileName;
        m_Position = pos;
        m_ParentNode = parent;
        m_MinDistance = min_distance;
        m_MaxDistance = max_distance;
        m_PlayCount = count;
        m_IsPlay = true;
    }
    public virtual void Update()
    {
    }

    public virtual void Play()
    {
        m_IsPlay = true;
    }

    public virtual void Stop()
    {
        m_IsPlay = false;
        if (m_SoundSource != null)
        {
            m_SoundSource.Stop();
        }
    }

    public virtual void PauseSound()
    {
        if (m_SoundSource != null)
        {
            m_SoundSource.Pause();
        }
    }
    public virtual void ResumeSound()
    {
        if (m_SoundSource != null)
        {
            m_SoundSource.UnPause();
        }
    }
    /// <summary>
    /// 加载内部资源
    /// </summary>
    public virtual void LoadResource()
    {

    }
    public virtual void OnLoadComplete()
    {

    }

    public void SetPosition(Vector3 pos)
    {
        m_Position = pos;
        if (m_SoundSource != null)
        {
            m_SoundSource.transform.position = pos;
        }
    }
    public void SetVolume(float volume)
    {
        if (m_SoundSource != null)
        {
            m_SoundSource.volume = volume;
        }
    }
    public ulong ObjectUID
    {
        get { return m_ObjectUID; }
        set { m_ObjectUID = value; }
    }
    public bool isPlaying
    {
        get { return m_IsPlay; }
    }
    public AudioSource SoundSource
    {
        get { return m_SoundSource; }
    }
}

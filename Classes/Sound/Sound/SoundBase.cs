using UnityEngine;
using System.Collections;

/// <summary>
/// 声音基类
/// @author hannibal
/// @time 2017-1-14
/// </summary>
public class SoundBase : IPoolsObject
{
    public static string POOLS_SOUND_BG     = "SOUND_BG";
    public static string POOLS_SOUND_EFFECT = "SOUND_EFFECT";

    protected ulong     m_ObjectUID = 0;    //对象唯一ID
    protected bool      m_Active = false; 	//是否激活中
    protected bool      m_IsPlay = false;

    protected string    m_FileName;
    protected Transform m_ParentNode;
    protected Vector3   m_Position;
    protected float     m_Volume;
    protected float     m_MinDistance;
    protected float     m_MaxDistance;
    protected bool      m_IsLoop = false;

    protected AudioSource m_SoundSource = null;

    public SoundBase()
	{
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
    public virtual string GetPoolsType()
    {
        return "";
    }
    public virtual void Setup(string fileName, Vector3 pos, Transform parent, float volume, float min_distance, float max_distance, bool loop = false)
    {
        m_Active = true;
        m_FileName = fileName;
        m_Position = pos;
        m_ParentNode = parent;
        m_Volume = volume;
        m_MinDistance = min_distance;
        m_MaxDistance = max_distance;
        m_IsLoop = loop;
        m_IsPlay = true;
	}
    public virtual void Update()
    {
    }

    public virtual void Play()
    {
        m_IsPlay = true;
        if(m_SoundSource != null)
        {
            m_SoundSource.Play();
        }
    }

    public virtual void Stop()
    {
        m_IsPlay = false;
        if (m_SoundSource != null)
        {
            m_SoundSource.Stop();
        }
    }

    /// <summary>
    /// 加载内部资源
    /// </summary>
    public virtual void LoadResource()
    {
        if(m_FileName.Length == 0)
        {
            ObjectFactoryManager.Instance.RecoverObject(this);
            return;
        }

        Object res = ResourceLoaderManager.Instance.GetResource(m_FileName);
        if (res == null)
        {
            ResourceManager.Instance.AddAsync(m_FileName, eResType.SOUND, delegate(sResLoadResult info)
            {
                if (!m_Active)
                {
                    ObjectFactoryManager.Instance.RecoverObject(this);
                    return;
                }

                OnLoadComplete();
            }
            );
        }
        else
        {
            OnLoadComplete();
        }
    }
    public virtual void OnLoadComplete()
    {
        AudioSource aSrc = AudioPools.instance.SpawnAudioByFile(m_FileName, m_Position, m_ParentNode);
        if (aSrc != null)
        {
            aSrc.pitch = 1;
            aSrc.volume = m_Volume;
            aSrc.loop = m_IsLoop;
            aSrc.minDistance = m_MinDistance;
            aSrc.maxDistance = m_MaxDistance;
            if (m_IsPlay) aSrc.Play();
            else aSrc.Stop();

            AutoDestroyAudio component = aSrc.gameObject.GetComponent<AutoDestroyAudio>();
            if (component == null) component = aSrc.gameObject.AddComponent<AutoDestroyAudio>();

            component.IsLoop = m_IsLoop;
            component.m_DestroyCallback = OnComponentDestroy;
            m_SoundSource = aSrc;
        }
        else
        {
            ObjectFactoryManager.Instance.RecoverObject(this);
            return;
        }
    }
    /// <summary>
    /// 组件自动销毁回调
    /// </summary>
    private void OnComponentDestroy()
    {
        m_Active = false;

        ObjectFactoryManager.Instance.RecoverObject(this);
    }

    public void SetPosition(Vector3 pos)
    {
        m_Position = pos;
        if(m_SoundSource != null)
        {
            m_SoundSource.transform.position = pos;
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
}

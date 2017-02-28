using UnityEngine;
using System.Collections;

public class EffectSound : SoundBase
{
    static public IPoolsObject CreateObject()
    {
        return new EffectSound();
    }
    public override string GetPoolsType()
    {
        return SoundBase.POOLS_SOUND_EFFECT;
    }

    public override void Init()
    {
        base.Init();
    }
    public override void Release()
    {
        if (m_SoundSource != null)
        {
            m_SoundSource.Stop();
            AudioPools.instance.DespawnAudio(m_SoundSource.transform);
            m_SoundSource = null;
        }
    }
    public override void Setup(string fileName, Vector3 pos, Transform parent, float min_distance, float max_distance, bool loop = false)
    {
        base.Setup(fileName, pos, parent, min_distance, max_distance, loop);
    }
    public override void Play()
    {
        if (SoundManager.Instance.IsCloseEffectSound) return;
        base.Play();
        if (m_SoundSource != null)
        {
            m_SoundSource.volume = SoundManager.Instance.EffectSoundVolume;
        }
    }
    public override void Stop()
    {
        base.Stop();
    }
    /// <summary>
    /// 加载内部资源
    /// </summary>
    public override void LoadResource()
    {
        if (m_FileName.Length == 0)
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
    public override void OnLoadComplete()
    {
        AudioSource aSrc = AudioPools.instance.SpawnAudioByFile(m_FileName, m_Position, m_ParentNode);
        if (aSrc != null)
        {
            aSrc.pitch = 1;
            aSrc.volume = SoundManager.Instance.EffectSoundVolume;
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
}

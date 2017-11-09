using UnityEngine;
using System.Collections;

public class EffectSound : SoundBase
{
    public override void Setup(string fileName, Vector3 pos, Transform parent, float min_distance, float max_distance, int count = 1)
    {
        base.Setup(fileName, pos, parent, min_distance, max_distance, count);
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
            this.Destroy();
            CommonObjectPools.Despawn(this);
            return;
        }

        Object res = ResourceLoaderManager.Instance.GetResource(m_FileName);
        if (res == null)
        {
            ResourceManager.Instance.AddAsync(m_FileName, eResType.SOUND, delegate(sResLoadResult info)
            {
                if (!m_Active)
                {
                    this.Destroy();
                    CommonObjectPools.Despawn(this);
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
        AudioSource aSrc = AudioSourcePools.instance.SpawnByFile(m_FileName, m_Position, m_ParentNode);
        if (aSrc != null && aSrc.clip != null)
        {
            aSrc.pitch = 1;
            aSrc.volume = SoundManager.Instance.EffectSoundVolume;
            aSrc.loop = m_PlayCount > 1 ? true : false;
            aSrc.minDistance = m_MinDistance;
            aSrc.maxDistance = m_MaxDistance;
            if (m_IsPlay) aSrc.Play();
            else aSrc.Stop();

            AutoDestroyAudio component = aSrc.gameObject.GetComponent<AutoDestroyAudio>();
            if (component == null) component = aSrc.gameObject.AddComponent<AutoDestroyAudio>();

            component.PlayCount = m_PlayCount;
            component.DestroyCallback = OnComponentDestroy;
            m_SoundSource = aSrc;
        }
        else
        {
            this.Destroy();
            CommonObjectPools.Despawn(this);
            return;
        }
    }
    /// <summary>
    /// 组件自动销毁回调
    /// </summary>
    private void OnComponentDestroy()
    {
        m_Active = false;
        this.Destroy();
        CommonObjectPools.Despawn(this);
    }
}

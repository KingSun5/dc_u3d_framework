using UnityEngine;
using System.Collections;

/// <summary>
/// 背景声音
/// @author hannibal
/// @time 2016-2-20
/// </summary>
public class BackgroundSound : SoundBase
{
    private GameObject m_AudioSourceParent;

    public override void Setup(string fileName, Vector3 pos, Transform parent, float min_distance, float max_distance, int count = 1)
    {
        base.Setup(fileName, pos, parent, min_distance, max_distance, count);

        AudioClip clip = ResourceLoaderManager.Instance.Load(fileName) as AudioClip;
        if (clip == null)
        {
            Log.Error("SoundManager::PlayBGSound - not load sound, file:" + fileName);
            return;
        }
        AudioListener listener = SoundManager.Instance.GetDefaultListener();
        if (listener != null)
        {
            m_AudioSourceParent = new GameObject("AudioSourceParent", typeof(AudioSource));
            m_AudioSourceParent.transform.SetParent(listener.transform, false);
            m_SoundSource = m_AudioSourceParent.GetComponent<AudioSource>();
            m_SoundSource.clip = clip;
            m_SoundSource.loop = count > 1 ? true : false;
            m_SoundSource.volume = SoundManager.Instance.BGSoundVolume;
            m_SoundSource.Play();
        }
    }
    public override void Destroy()
    {
        base.Destroy();
        if (m_AudioSourceParent != null)
        {
            GameObject.Destroy(m_AudioSourceParent);
            m_AudioSourceParent = null;
        }
    }
    public override void Play()
    {
        if (SoundManager.Instance.IsCloseBGSound) return;
        base.Play();
        if (m_SoundSource != null)
        {
            m_SoundSource.volume = SoundManager.Instance.BGSoundVolume;
        }
    }
    public override void Stop()
    {
        base.Stop();
    }
    public override void PauseSound()
    {
        base.PauseSound();
    }
    public override void ResumeSound()
    {
        if (SoundManager.Instance.IsCloseBGSound) return;
        base.ResumeSound();
        if (m_SoundSource != null)
        {
            m_SoundSource.volume = SoundManager.Instance.BGSoundVolume;
        }
    }
}

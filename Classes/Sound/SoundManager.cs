using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 声音
/// @author hannibal
/// @time 2014-12-3
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
	private AudioListener           m_Listener;     //听众 
    private List<BackgroundSound>   m_ListBGAudio;  //背景声音

    private bool    m_IsCloseBGSound = false;
    private bool    m_IsCloseEffectSound = false;
    private float   m_BGSoundVolume = 1;
    private float   m_EffectSoundVolume = 1;

    public SoundManager()
    {
        m_ListBGAudio = new List<BackgroundSound>();
    }

	public void Setup()
    {
        m_IsCloseBGSound = LocalValue.GetValue<int>(SoundID.LOCAL_BG_SOUND_CLOSE, 0) == 1 ? true : false;
        m_IsCloseEffectSound = LocalValue.GetValue<int>(SoundID.LOCAL_EFFECT_SOUND_CLOSE, 0) == 1 ? true : false;
        m_BGSoundVolume = LocalValue.GetValue<float>(SoundID.LOCAL_BGSOUND_VOLUME, 1) ;
        m_EffectSoundVolume = LocalValue.GetValue<float>(SoundID.LOCAL_EFFECT_SOUND_VOLUME, 1);

        ObjectFactoryManager.Instance.RegisterFactory(SoundBase.POOLS_SOUND_EFFECT, EffectSound.CreateObject);
        ObjectFactoryManager.Instance.RegisterFactory(SoundBase.POOLS_SOUND_BG, BackgroundSound.CreateObject);
        RegisterEvent();
	}
	
	public void Destroy()
    {
        ObjectFactoryManager.Instance.UnregisterFactory(SoundBase.POOLS_SOUND_EFFECT);
        ObjectFactoryManager.Instance.UnregisterFactory(SoundBase.POOLS_SOUND_BG);
        UnRegisterEvent();
	}

    public void Tick(float elapse, int game_frame)
    {
    }
    /// <summary>
    /// 停止所有声音
    /// </summary>
    public void StopAll()
    {
        StopBGSound();
        AudioPools.instance.StopAll();
    }
    /// <summary>
    /// 释放音效资源
    /// </summary>
    public void Clear()
    {
        ClearBGSound();
        AudioPools.instance.Clear();
    }
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～背景声音～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    /**背景声音*/
    public BackgroundSound PlayBGSound(string fileName, bool loop)
    {
        if (m_IsCloseBGSound) return null;
        if (string.IsNullOrEmpty(fileName)) return null;

        BackgroundSound sound = ObjectFactoryManager.Instance.CreateObject(SoundBase.POOLS_SOUND_BG) as BackgroundSound;
        sound.Setup(fileName, Vector3.zero, null, 0, 0, loop ? int.MaxValue : 1);
        sound.LoadResource();
        m_ListBGAudio.Add(sound);

        return sound;
    }

    public void PauseBGSound()
    {
        for (int i = 0; i < m_ListBGAudio.Count; ++i)
        {
            m_ListBGAudio[i].PauseSound();
        }
    }
    public void ResumeBGSound()
    {
        for (int i = 0; i < m_ListBGAudio.Count; ++i)
        {
            m_ListBGAudio[i].ResumeSound();
        }
    }

    public void SetBGSoundVolume(float volume)
    {
        BGSoundVolume = volume;
        for (int i = 0; i < m_ListBGAudio.Count; ++i)
        {
            m_ListBGAudio[i].SetVolume(volume);
        }
    }
    public void RemoveBGSound(BackgroundSound sound)
    {
        if (sound != null)
        {
            sound.Stop();
            ObjectFactoryManager.Instance.RecoverObject(sound);
            m_ListBGAudio.Remove(sound);
        }
    }
    public void StopBGSound()
    {
        for (int i = 0; i < m_ListBGAudio.Count; ++i)
        {
            m_ListBGAudio[i].Stop();
        }
    }
    public void ClearBGSound()
    {
        for (int i = 0; i < m_ListBGAudio.Count; ++i)
        {
            SoundBase sound = m_ListBGAudio[i];
            sound.Stop();
            ObjectFactoryManager.Instance.RecoverObject(sound);
        }
        m_ListBGAudio.Clear();
    }
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～音效～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
	/// <summary>
    /// 音效
	/// </summary>
	/// <param name="fileName">资源文件</param>
	/// <param name="pos">播放位置</param>
    /// <param name="loop">是否循环：如果是循环音效，需要手动清理(AudioPools.instance.DespawnAudio)</param>
	/// <returns></returns>
    public SoundBase PlaySoundEffect(string fileName, Vector3 pos, float min_distance, float max_distance, int count = 1)
    {
        if (IsCloseEffectSound || fileName.Length == 0) return null;

        EffectSound sound = ObjectFactoryManager.Instance.CreateObject(SoundBase.POOLS_SOUND_EFFECT) as EffectSound;
        sound.Setup(fileName, pos, null, min_distance, max_distance, count);
        sound.LoadResource();

        return sound;
    }
    public AudioSource PlaySoundEffect(AudioClip clip, Vector3 pos, float min_distance, float max_distance, bool loop = false)
    {
        if (IsCloseEffectSound || clip == null) return null;

        AudioSource aSrc = AudioPools.instance.SpawnAudioByClip(clip, Vector3.zero);
        aSrc.gameObject.transform.position = pos;
        aSrc.pitch = 1;
        aSrc.volume = m_EffectSoundVolume;
        aSrc.loop = loop;
        aSrc.minDistance = min_distance;
        aSrc.maxDistance = max_distance;
        aSrc.Play();

        return aSrc;
    }
    public AudioSource PlayUISoundEffect(AudioClip clip, bool loop = false)
    {
        if (IsCloseEffectSound || clip == null || AudioPools.instance == null) return null;

        AudioSource aSrc = AudioPools.instance.SpawnAudioByClip(clip, Vector3.zero);
        aSrc.pitch = 1;
        aSrc.volume = m_EffectSoundVolume;
        aSrc.loop = loop;
        aSrc.transform.SetParent(GetDefaultListener().transform, false);
        aSrc.Play();

        return aSrc;
    }
    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopSoundEffect(AudioSource aSrc)
    {
        if (aSrc == null) return;

        aSrc.Stop();
        AudioPools.instance.DespawnAudio(aSrc.gameObject.transform);
    }
    public void StopSoundEffect(SoundBase sound)
    {
        if (sound == null) return;

        sound.Stop();
        ObjectFactoryManager.Instance.RecoverObject(sound);
    }
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～其他～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    /// <summary>
    /// 忽视距离声音
    /// </summary>
    public SoundBase PlayIgnoreListenerSound(string fileName, int count = 1)
    {
        if (IsCloseEffectSound || fileName.Length == 0) return null;

        EffectSound sound = ObjectFactoryManager.Instance.CreateObject(SoundBase.POOLS_SOUND_EFFECT) as EffectSound;
        sound.Setup(fileName, Vector3.zero, GetDefaultListener().transform, 0, 500, count);
        sound.LoadResource();

        return sound;
    }

    /**听众*/
    public AudioListener GetDefaultListener()
	{
		if (m_Listener == null)
		{
			AudioListener[] listeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
			
			if (listeners != null && listeners.Length > 0)
			{
                m_Listener = listeners[0];
			}
			
			if (m_Listener == null)
			{
				Camera cam = Camera.main;
				if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
				if (cam != null) m_Listener = cam.gameObject.AddComponent<AudioListener>();
			}
		}
        return m_Listener;
	}
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    private void RegisterEvent()
    {
        EventController.AddEventListener(SoundID.SWITCH_BG_SOUND, OnSoundEvent);
        EventController.AddEventListener(SoundID.SWITCH_EFFECT_SOUND, OnSoundEvent);
        EventController.AddEventListener(SoundID.ADJUST_BG_VOLUME, OnSoundEvent);
        EventController.AddEventListener(SoundID.ADJUST_EFFECT_VOLUME, OnSoundEvent);
    }
    private void UnRegisterEvent()
    {
        EventController.RemoveEventListener(SoundID.SWITCH_BG_SOUND, OnSoundEvent);
        EventController.RemoveEventListener(SoundID.SWITCH_EFFECT_SOUND, OnSoundEvent);
        EventController.RemoveEventListener(SoundID.ADJUST_BG_VOLUME, OnSoundEvent);
        EventController.RemoveEventListener(SoundID.ADJUST_EFFECT_VOLUME, OnSoundEvent);
    }
    private void OnSoundEvent(GameEvent evt)
	{
        string evt_type = evt.type;
        switch(evt_type)
        {
            case SoundID.SWITCH_BG_SOUND:
                IsCloseBGSound = evt.Get<bool>(0);
                if(m_IsCloseBGSound)
                    PauseBGSound();
                else
                    ResumeBGSound();
                break;

            case SoundID.SWITCH_EFFECT_SOUND:
                IsCloseEffectSound = evt.Get<bool>(0);
                break;

            case SoundID.ADJUST_BG_VOLUME:
                m_BGSoundVolume = evt.Get<float>(0);
                m_BGSoundVolume = Mathf.Clamp(m_BGSoundVolume, 0, 1);
                SetBGSoundVolume(m_BGSoundVolume);
                break;

            case SoundID.ADJUST_EFFECT_VOLUME:
                m_EffectSoundVolume = evt.Get<float>(0);
                EffectSoundVolume = Mathf.Clamp(m_EffectSoundVolume, 0, 1);
                break;
        }
	}
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～get/set～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    public bool IsCloseBGSound
    {
        get { return m_IsCloseBGSound; }
        set 
        {
            m_IsCloseBGSound = value;
            LocalValue.SetValue<int>(SoundID.LOCAL_BG_SOUND_CLOSE, m_IsCloseBGSound ? 1 : 0);
        }
    }
    public bool IsCloseEffectSound
    {
        get { return m_IsCloseEffectSound; }
        set
        {
            m_IsCloseEffectSound = value;
            LocalValue.SetValue<int>(SoundID.LOCAL_EFFECT_SOUND_CLOSE, m_IsCloseEffectSound ? 1 : 0);
        }
    }
    public float BGSoundVolume
    {
        get { return m_BGSoundVolume; }
        set
        {
            m_BGSoundVolume = value;
            LocalValue.SetValue<float>(SoundID.LOCAL_BGSOUND_VOLUME, m_BGSoundVolume);
        }
    }
    public float EffectSoundVolume
    {
        get { return m_EffectSoundVolume; }
        set 
        {
            m_EffectSoundVolume = value;
            LocalValue.SetValue<float>(SoundID.LOCAL_EFFECT_SOUND_VOLUME, m_EffectSoundVolume);
        }
    }
}

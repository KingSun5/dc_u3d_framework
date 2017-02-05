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
	private AudioListener   m_Listener;     //听众 
    private AudioSource     m_BackAudio;    //当前背景声音

    private bool    m_IsCloseBGSound = false;
    private bool    m_IsCloseEffectSound = false;
    private float   m_BGSoundVolume = 1;
    private float   m_EffectSoundVolume = 1;

	public void Setup()
    {
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
    public bool PlayBGSound(string fileName, bool loop)
	{
        if (m_IsCloseBGSound) return false;
        //Log.Debug("PlayOneBGSound:" + fileName);
        AudioClip clip = ResourceLoaderManager.Instance.Load(fileName) as AudioClip;
		if(clip == null)
		{
            Log.Error("SoundManager::PlayOneBGSound - not load sound, file:" + fileName);
			return false;
		}
        GetDefaultListener();
		if (m_Listener != null && m_Listener.enabled)
		{
            m_BackAudio = m_Listener.GetComponent<AudioSource>();
            if (m_BackAudio == null) m_BackAudio = m_Listener.gameObject.AddComponent<AudioSource>();
            m_BackAudio.clip = clip;
            m_BackAudio.loop = loop;
            m_BackAudio.volume = m_BGSoundVolume;
            m_BackAudio.Play();
			return true;
		}
		
		return true;
	}

    public void PauseBGSound()
    {
        if (m_BackAudio != null)
        {
            m_BackAudio.Pause();
        }
    }
    public void ResumeBGSound()
    {
        if (m_BackAudio != null)
        {
            m_BackAudio.UnPause();
        }
    }

    public void StopBGSound()
    {
        if(m_BackAudio != null)
        {
            m_BackAudio.Stop();
        }
    }
    public void ClearBGSound()
    {
        if (m_BackAudio != null)
        {
            GameObject.Destroy(m_BackAudio);
            m_BackAudio = null;
        }
    }
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～音效～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
	/// <summary>
    /// 音效
	/// </summary>
	/// <param name="fileName">资源文件</param>
	/// <param name="pos">播放位置</param>
    /// <param name="loop">是否循环：如果是循环音效，需要手动清理(AudioPools.instance.DespawnAudio)</param>
	/// <returns></returns>
    public SoundBase PlaySoundEffect(string fileName, Vector3 pos, float min_distance, float max_distance, bool loop = false)
    {
        if (IsCloseEffectSound || fileName.Length == 0) return null;

        EffectSound sound = ObjectFactoryManager.Instance.CreateObject(SoundBase.POOLS_SOUND_EFFECT) as EffectSound;
        sound.Setup(fileName, pos, null, m_EffectSoundVolume, min_distance, max_distance, loop);
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
    public SoundBase PlayIgnoreListenerSound(string fileName, bool loop = false)
    {
        if (IsCloseEffectSound || fileName.Length == 0) return null;

        EffectSound sound = ObjectFactoryManager.Instance.CreateObject(SoundBase.POOLS_SOUND_EFFECT) as EffectSound;
        sound.Setup(fileName, Vector3.zero, GetDefaultListener().transform, m_EffectSoundVolume, 0, 0, loop);
        sound.LoadResource();

        return sound;
    }

    /**听众*/
    private AudioListener GetDefaultListener()
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
        EventDispatcher.AddEventListener(SoundID.SWITCH_BG_SOUND, OnSoundEvent);
        EventDispatcher.AddEventListener(SoundID.SWITCH_EFFECT_SOUND, OnSoundEvent);
        EventDispatcher.AddEventListener(SoundID.ADJUST_BG_VOLUME, OnSoundEvent);
        EventDispatcher.AddEventListener(SoundID.ADJUST_EFFECT_VOLUME, OnSoundEvent);
    }
    private void UnRegisterEvent()
    {
        EventDispatcher.RemoveEventListener(SoundID.SWITCH_BG_SOUND, OnSoundEvent);
        EventDispatcher.RemoveEventListener(SoundID.SWITCH_EFFECT_SOUND, OnSoundEvent);
        EventDispatcher.RemoveEventListener(SoundID.ADJUST_BG_VOLUME, OnSoundEvent);
        EventDispatcher.RemoveEventListener(SoundID.ADJUST_EFFECT_VOLUME, OnSoundEvent);
    }
    private void OnSoundEvent(GameEvent evt)
	{
        string evt_type = evt.type;
        switch(evt_type)
        {
            case SoundID.SWITCH_BG_SOUND:
                m_IsCloseBGSound = evt.Get<int>(0) == 0 ? true : false;
                if(m_IsCloseBGSound)
                    PauseBGSound();
                else
                    ResumeBGSound();
                break;

            case SoundID.SWITCH_EFFECT_SOUND:
                m_IsCloseEffectSound = evt.Get<int>(0) == 0 ? true : false;
                break;

            case SoundID.ADJUST_BG_VOLUME:
                m_BGSoundVolume = evt.Get<float>(0);
                m_BGSoundVolume = Mathf.Clamp(m_BGSoundVolume, 0, 1);
                if (m_BackAudio != null) m_BackAudio.volume = m_BGSoundVolume;
                break;

            case SoundID.ADJUST_EFFECT_VOLUME:
                m_EffectSoundVolume = evt.Get<float>(0);
                m_EffectSoundVolume = Mathf.Clamp(m_EffectSoundVolume, 0, 1);
                break;
        }
	}
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～get/set～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    public bool IsCloseBGSound
    {
        get { return m_IsCloseBGSound; }
    }
    public bool IsCloseEffectSound
    {
        get { return m_IsCloseEffectSound; }
    }
    public float BGSoundVolume
    {
        get { return m_BGSoundVolume; }
    }
    public float EffectSoundVolume
    {
        get { return m_EffectSoundVolume; }
    }
}

using UnityEngine;
using System.Collections;

/// <summary>
/// 声音
/// @author hannibal
/// @time 2014-12-3
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
	private AudioListener m_Listener;
	private bool m_IsCloseSound = false;

	public void Setup()
	{
		//EventDispatcher.AddEventListener (EventID.STOP_GAME, OnStopGame);
		//EventDispatcher.AddEventListener (EventID.CLOSE_SOUND, OnCloseSound);
	}
	
	public void Destroy()
	{
		//EventDispatcher.RemoveEventListener (EventID.STOP_GAME, OnStopGame);
		//EventDispatcher.RemoveEventListener (EventID.CLOSE_SOUND, OnCloseSound);
	}
	/**背景声音*/
	public bool PlayBackgroundSound(string fileName, bool loop, float volume=1f, float pitch=1f)
	{
		AudioClip clip = ResourceLoaderManager.Instance.Load(fileName) as AudioClip;
		if(clip == null)
		{
			Log.Error("SoundManager::PlaySoundEffect - not load sound, name:"+fileName);
			return false;
		}
		GetListener();
		if (m_Listener != null && m_Listener.enabled)
		{
			AudioSource source = m_Listener.GetComponent<AudioSource>();
			if (source == null) source = m_Listener.gameObject.AddComponent<AudioSource>();
			source.clip = clip;
			source.loop = loop;
			source.Play();
			return true;
		}
		
		return true;
	}
	/**音效*/
	public bool PlaySoundEffect(string fileName, float volume=1f, float pitch=1f)
	{
		if (IsCloseSound ())
			return false;

		AudioClip clip = ResourceLoaderManager.Instance.Load(fileName) as AudioClip;
		if(clip == null)
		{
			Log.Error("SoundManager::PlaySoundEffect - not load sound, name:"+fileName);
			return false;
		}
		PlaySoundEffByClip(clip, volume, pitch);

		return true;
	}
	/**音效*/
	public void PlaySoundEffByClip(AudioClip clip, float volume=1f, float pitch=1f)
	{
		if (IsCloseSound ())
			return;

		GetListener();
		AudioSource source = m_Listener.GetComponent<AudioSource>();
		if (source == null) source = m_Listener.gameObject.AddComponent<AudioSource>();
		source.pitch = pitch;
		source.PlayOneShot(clip, volume);
	}

	public void CloseSound(bool is_close)
	{
		AudioListener.pause = is_close;
	}
	public bool IsCloseSound()
	{
		return AudioListener.pause;
	}

	/**根据距离计算音量*/
	public float CalSceneSoundVolume(Vector3 listener_pos, Vector3 play_pos, float max_dis)
	{
		float dis = (listener_pos-play_pos).magnitude;
		if(dis >= max_dis)
			return 0f;
		return ((max_dis-dis)/max_dis);
	}
	
	private void GetListener()
	{
		if (m_Listener == null)
		{
			AudioListener[] listeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
			
			if (listeners != null)
			{
				for (int i = 0; i < listeners.Length; ++i)
				{
					m_Listener = listeners[i];
					break;
				}
			}
			
			if (m_Listener == null)
			{
				Camera cam = Camera.main;
				if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
				if (cam != null) m_Listener = cam.gameObject.AddComponent<AudioListener>();
			}
		}
	}
    private void OnStopGame(GameEvent evt)
	{
        bool is_stop = evt.Get<bool>(0);
		if(!m_IsCloseSound)
		{
			CloseSound(is_stop);
		}
	}
	private void OnCloseSound(GameEvent evt)
	{
        m_IsCloseSound = evt.Get<bool>(0);
		CloseSound(m_IsCloseSound);
	}
}

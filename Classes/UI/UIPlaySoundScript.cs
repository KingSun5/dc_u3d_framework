using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIPlaySoundScript : MonoBehaviour 
{
	public AudioClip audioClip;

	public float volume = 1f;
	public float pitch = 1f;

	void OnEnable()
	{
		UIEventTriggerListener.Get(gameObject).onClick += OnClick;
	}
	
	//当摇杆不可用时移除事件
	void OnDisable()
	{
		UIEventTriggerListener.Get(gameObject).onClick -= OnClick;
	}

    private void OnClick(GameObject obj, Vector2 pos)
	{
		SoundManager.Instance.PlaySoundEffByClip(audioClip,1,1);
	}
}

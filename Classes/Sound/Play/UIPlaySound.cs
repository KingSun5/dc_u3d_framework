using UnityEngine;

/// <summary>
/// UI播放声音
/// @author hannibal
/// @time 2016-11-11
/// </summary>
public class UIPlaySound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
	}
    public Trigger  trigger = Trigger.OnClick;

    public AudioClip    audioClip;

    void Awake()
    {
    }

	void OnEnable()
	{
        UIEventTriggerListener.Get(gameObject).onClick += OnClick;
	}

    void OnDestroy()
    {
        UIEventTriggerListener.Get(gameObject).onClick -= OnClick;
    }

    void OnClick(GameObject go, Vector2 delta)
    {
        play();
    }

	public void Play ()
	{
        play();
	}

    void play()
    {
        if (audioClip != null)
        {
            SoundManager.Instance.PlayUISoundEffect(audioClip);
        }
    }
}

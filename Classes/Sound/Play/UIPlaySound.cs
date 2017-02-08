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
    public Trigger trigger = Trigger.OnClick;

    public AudioClip audioClip;

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

    public void Play()
    {
        play();
    }

    void play()
    {
        if (audioClip != null)
        {
            AudioSource aSrc = SoundManager.Instance.PlayUISoundEffect(audioClip);
            if (aSrc != null)
            {
                AutoDestroyAudio component = aSrc.gameObject.GetComponent<AutoDestroyAudio>();
                if (component == null) component = aSrc.gameObject.AddComponent<AutoDestroyAudio>();

                component.IsLoop = false;
                component.m_DestroyCallback = delegate()
                {
                    SoundManager.Instance.StopSoundEffect(aSrc);
                };
            }
        }
    }
}

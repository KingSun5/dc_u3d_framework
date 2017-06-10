using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI播放声音
/// @author hannibal
/// @time 2016-11-11
/// </summary>
public class UIPlaySound : MonoBehaviour, IPointerClickHandler
{
    public enum Trigger
    {
        OnClick,
        Enable,
        Disable,
    }
    public Trigger trigger = Trigger.OnClick;
    public AudioClip audioClip;

    void Awake()
    {
    }

    void OnEnable()
    {
        switch (trigger)
        {
            case Trigger.Enable:
                {
                    PlaySound();
                }
                break;
        }
    }

    void OnDisable()
    {
        switch (trigger)
        {
            case Trigger.Disable:
                {
                    PlaySound();
                }
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (trigger == Trigger.OnClick)
        {
            PlaySound();
        }
    }

    public void Play()
    {
        PlaySound();
    }

    void PlaySound()
    {
        if (audioClip != null)
        {
            AudioSource aSrc = SoundManager.Instance.PlayUISoundEffect(audioClip);
            if (aSrc != null)
            {
                AutoDestroyAudio component = aSrc.gameObject.GetComponent<AutoDestroyAudio>();
                if (component == null) component = aSrc.gameObject.AddComponent<AutoDestroyAudio>();

                component.PlayCount = 1;
                component.DestroyCallback = delegate()
                {
                    SoundManager.Instance.StopSoundEffect(aSrc);
                };
            }
        }
    }
}

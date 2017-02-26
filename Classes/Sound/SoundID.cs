using UnityEngine;
using System.Collections;

public class SoundID 
{
    //声音事件
    public const string SWITCH_BG_SOUND      = "SWITCH_BG_SOUND";		//切换背景声音(开启/关闭)
    public const string SWITCH_EFFECT_SOUND  = "SWITCH_EFFECT_SOUND";	//切换音效(开启/关闭)
    public const string ADJUST_BG_VOLUME     = "ADJUST_BG_VOLUME";		//调节背景音量(0-1)
    public const string ADJUST_EFFECT_VOLUME = "ADJUST_EFFECT_VOLUME";	//调节音效音量(0-1)

    public const string SOUND_LISTENER_ENTER = "SOUND_LISTENER_ENTER";		//声音听众
    public const string SOUND_LISTENER_LEAVE = "SOUND_LISTENER_LEAVE";		//声音听众

    //本地数据缓存
    public const string LOCAL_BGSOUND_VOLUME        = "LOCAL_BG_SOUND_VOLUME";      //音乐:0-1
    public const string LOCAL_EFFECT_SOUND_VOLUME   = "LOCAL_EFFECT_SOUND_VOLUME";  //音效:0-1
    public const string LOCAL_BG_SOUND_CLOSE        = "LOCAL_BG_SOUND_CLOSE";        //音乐:0/1
    public const string LOCAL_EFFECT_SOUND_CLOSE    = "LOCAL_EFFECT_SOUND_CLOSE";    //音效:0/1
}
/// <summary>
/// 背景声音播放模式
/// </summary>
public enum eBGSoundPlayMode
{
    ONCE,           //播放一次
    SINGLE_CYCLE,   //单曲循环
    SEQUENCE,       //顺序
    RANDOM,         //随机
}
using UnityEngine;
using System.Collections;

public class BackgroundSound : SoundBase
{
    static public IPoolsObject CreateObject()
    {
        return new BackgroundSound();
    }
    public override string GetPoolsType()
    {
        return SoundBase.POOLS_SOUND_BG;
    }
    public override void Init()
    {
        base.Init();
    }
}

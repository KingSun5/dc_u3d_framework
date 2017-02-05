using UnityEngine;
using System.Collections;

public class EffectSound : SoundBase
{
    static public IPoolsObject CreateObject()
    {
        return new EffectSound();
    }
    public override string GetPoolsType()
    {
        return SoundBase.POOLS_SOUND_EFFECT;
    }

    public override void Init()
    {
        base.Init();
    }

}

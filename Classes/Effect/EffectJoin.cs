using UnityEngine;
using System.Collections;

/// <summary>
/// 挂接特效
/// @author hannibal
/// @time 2014-12-8
/// </summary>
public class EffectJoint : EffectBase
{
	private Transform m_ParentNode;

	public EffectJoint()
	{
	}

    public override void Awake()
    {
        base.Awake();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void Update()
    {
        base.Update();
    }

    public Transform ParentNode
    {
        get { return m_ParentNode; }
        set { m_ParentNode = value; }
    }
}

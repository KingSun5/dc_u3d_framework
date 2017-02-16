using UnityEngine;
using System.Collections;

/// <summary>
/// 挂接特效
/// @author hannibal
/// @time 2014-12-8
/// </summary>
public class Effect2DJoin : Effect2DBase
{
	private Transform m_ParentNode;

    public Effect2DJoin()
	{
	}

    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

	public override void Update ()
	{
		base.Update ();
	}

	public override void OnLoadComplate ()
	{
		base.OnLoadComplate();

		transform.SetParent(m_ParentNode, false);
	}

	public Transform ParentNode
	{
		get{ return m_ParentNode; }
		set{ m_ParentNode = value; }
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// UI界面特效
/// @author hannibal
/// @time 2016-12-6
/// </summary>
public class EffectUI : EffectBase
{
	private Transform m_ParentNode = null;

    public EffectUI()
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

    public override void Update()
	{
        base.Update();
	}

    public override void OnLoadComplete(Transform obj)
    {
        base.OnLoadComplete(obj);

        //提示特效层级
        UIPanelBase parent_component = GetComponentInParent<UIPanelBase>();
        if (parent_component != null)
        {
            UIDepth effectDepth = gameObject.AddComponent<UIDepth>();
            if (effectDepth != null)
            {
                effectDepth.SetOrder(parent_component.MaxSortingOrder + 1);
                parent_component.MaxSortingOrder++;
            }
        }
    }

	public Transform ParentNode
	{
		get{ return m_ParentNode; }
		set{ m_ParentNode = value; }
	}
}

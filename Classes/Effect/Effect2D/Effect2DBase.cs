﻿using UnityEngine;
using System.Collections;

/// <summary>
/// 特效
/// @author hannibal
/// @time 2014-12-8
/// </summary>
public class Effect2DBase : MonoBehaviour
{
    [SerializeField, Tooltip("对象唯一ID(readonly)")]
    protected ulong m_ObjectUID = 0;
    [SerializeField, Tooltip("是否激活中")]
    protected bool m_Active = false;
    [SerializeField, Tooltip("是否循环动画")]
    protected bool m_IsLoop = false;
	/**动画*/
    protected tk2dSpriteAnimator m_Animation = null;
    /**完成*/
    private System.Action OnComplete = null;

    public Effect2DBase()
	{
	}
    public virtual void Awake()
    {
    }
    public virtual void Start()
    {
        m_Active = true;
        OnLoadComplate();
    }
    public virtual void OnDestroy()
    {
        OnComplete = null;
		m_Animation = null;
	}
    public virtual void Update()
    {
    }
    public virtual void OnLoadComplate()
	{
        m_Animation = gameObject.GetComponentInChildren<tk2dSpriteAnimator>();
        m_Animation.Play("Anim");
        m_Animation.AnimationCompleted = OnAnimationEnd;
	}
	protected void OnAnimationEnd(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clipId)
    {
        if (!m_IsLoop && m_Active)
        {
            m_Active = false;
            if (OnComplete != null) OnComplete();
            Effect2DManager.Instance.RemoveEffect(this);
        }
    }
    public void OnCompleted(System.Action callback)
    {
        OnComplete = callback;
    }
	public bool IsLoop
	{
		get{ return m_IsLoop; }
		set{ m_IsLoop = value; }
	}
}

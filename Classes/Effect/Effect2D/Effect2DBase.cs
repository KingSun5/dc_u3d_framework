using UnityEngine;
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


    public Effect2DBase()
	{
	}
    public virtual void Awake()
    {
    }
    public virtual void Start()
    {
        OnLoadComplate();
    }
    public virtual void OnDestroy()
	{
		m_Animation = null;
	}
    public virtual void Update()
    {
    }
    public virtual void OnLoadComplate()
	{
        m_Animation = gameObject.GetComponent<tk2dSpriteAnimator>();
        m_Animation.Play("Skill");
        m_Animation.AnimationCompleted = OnAnimationEnd;
	}
	protected void OnAnimationEnd(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clipId)  
	{  
		if(!m_IsLoop)
		{
            Effect2DManager.Instance.RemoveEffect(this);
		}
	} 
	public bool IsLoop
	{
		get{ return m_IsLoop; }
		set{ m_IsLoop = value; }
	}
}

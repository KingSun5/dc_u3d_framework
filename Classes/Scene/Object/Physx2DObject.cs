using UnityEngine;
using System.Collections;

/// <summary>
/// 2d物理对象
/// @author hannibal
/// @time 2014-11-1
/// </summary>
public abstract class Physx2DObject : Map2DObject
{
    [Header("Physx2DObject")]
    [SerializeField, Tooltip("是否激活物理效果")]
    protected bool          m_ActivePhysx = false;

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～基础方法～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    public override void Awake()
    {
        base.Awake();
    }

    public override void Setup(object info)
    {
        base.Setup(info);
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～物理～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    /**被碰到*/
    public virtual bool OnByContact(object info, Collision2D hitInfo)
    {
        return false;
    }

    /**产生碰撞*/
    public virtual void OnCollision2DEnter(Collision2D collisionInfo)
    {
        //Log.Info("发生碰撞", collisionInfo.collider.name, " ", collisionInfo.gameObject.name);
    }

    /**一个碰撞器或刚体触动另一个刚体或碰撞器，在每帧都会调用OnCollisionStay，直到它们之间离开不接触。*/
    public virtual void OnCollision2DStay(Collision2D collisionInfo)
    {
        //Log.Info("碰撞中", collisionInfo.collider.name);
    }

    /**退出碰撞*/
    public virtual void OnCollision2DExit(Collision2D collisionInfo)
    {
    }

    /**
     当碰撞器other进入触发器时OnTriggerEnter被调用。
     这个消息是发送给触发碰撞器和刚体（或如果没有刚体的碰撞器）。注意如果其中一个碰撞器也附加了刚体，触发事件才会发送。
     */
    public virtual void OnTrigger2DEnter(Collider2D other)
    {
        Log.Info("发生碰撞OnTriggerEnter", other.GetComponent<Collider2D>().name, " ", other.gameObject.name);
    }

    /**
     每个碰撞器other触动触发器，几乎在所有的帧OnTriggerStay被调用。通俗的说，
     每个碰撞器从进入触发器那一刻到退出触发器之前，几乎每帧都会调用OnTriggerStay。
     */
    public virtual void OnTrigger2DStay(Collider2D other)
    {
    }

    /**离开触发器时，销毁所有物体*/
    public virtual void OnTrigger2DExit(Collider2D other)
    {
        //Log.Info("发生碰撞3");
    }

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～get/set～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    public bool ActivePhysx
    {
        get { return m_ActivePhysx; }
        set
        {
            m_ActivePhysx = value;
            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().isTrigger = !m_ActivePhysx;
            }
        }
    }
}
using UnityEngine;
using System.Collections;

/// <summary>
/// 地图对象
/// @author hannibal
/// @time 2014-11-1
/// </summary>
public class MapObject : BaseObject, IGridObject
{
    [Header("MapObject")]
    [SerializeField, Tooltip("速度方向(readonly)")]
    protected Vector3   m_VelocityDir = Vector3.zero;
    [SerializeField, Tooltip("速度(readonly)")]
    protected float     m_VelocityPower = 0;

	/*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～基础方法～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
	public MapObject()
	{
	}

    public override void Awake()
	{
        base.Awake();
	}

    public override void Setup(object info)
    {
        base.Setup(info);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void OnDestroy()
	{
        base.OnDestroy();
	}

	/**
	* 所在格子发生改变
	*/		
	public virtual void OnMapGridChangle(int new_row, int new_col)
	{
	}
    public override void SetPosition(Vector3 pos)
    {
        base.SetPosition(pos);
    }
    public override void OnPositionChange()
    {
        base.OnPositionChange();
        m_Observer.TriggerEvent(ObjectEvent.MAP_OBJ_POS, this.ObjectUID, transform.position);
        EventController.TriggerEvent(ObjectEvent.MAP_OBJ_POS, this.ObjectUID, transform.position);
    }
    public virtual float GetMoveSpeed()
    {
        return m_VelocityPower;
    }
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～get/set～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/

    public Vector3 VelocityDir
    {
        get { return m_VelocityDir; }
        set { m_VelocityDir = value; }
    }
    public virtual float VelocityPower
    {
        get { return m_VelocityPower; }
        set { m_VelocityPower = value; }
    }
}

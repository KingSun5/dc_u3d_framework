using UnityEngine;
using System.Collections;

/// <summary>
/// 2D地图对象
/// @author hannibal
/// @time 2014-11-1
/// </summary>
public class Map2DObject : BaseObject, IGridObject
{
    [Header("MapObject")]
    [SerializeField, Tooltip("所在地图的行(readonly)")]
	protected int       m_RowIndex = 0;
    [SerializeField, Tooltip("所在地图的列(readonly)")]
	protected int       m_ColIndex = 0;

	/**对象所在的格子*/
	protected TerrainGrid  m_PathGrid = null;

    [SerializeField, Tooltip("速度方向(readonly)")]
    protected Vector3   m_VelocityDir = Vector3.zero;
    [SerializeField, Tooltip("速度(readonly)")]
    protected float     m_VelocityPower = 0;
	/*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～基础方法～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    public Map2DObject()
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
		if(m_PathGrid != null)
		{
			m_PathGrid.removeObject(this);
			m_PathGrid = null;
		}
		
		m_ColIndex = new_col;
		m_RowIndex = new_row;
        m_Observer.TriggerEvent(ObjectEvent.MAP_GRID_CHANGE, m_ObjectUID, m_RowIndex, m_ColIndex);
        EventController.TriggerEvent(ObjectEvent.MAP_GRID_CHANGE, m_ObjectUID, m_RowIndex, m_ColIndex);
		
		m_PathGrid = TerrainGridMap.Instance.getNode(m_ColIndex, m_RowIndex);
		if(m_PathGrid != null)
		{
			m_PathGrid.addObject(this);
		}
	}
    public override void SetPosition(float x, float y, float z)
    {
        base.SetPosition(x, y);
    }
    public override void SetPosition(Vector3 pos)
    {
        this.SetPosition((Vector2)pos);
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
	public int RowIndex
	{
		get{ return m_RowIndex; }
	}
	public int ColIndex
	{
		get{ return m_ColIndex; }
	}

	public TerrainGrid TerrainGrid
	{
		get{ return m_PathGrid; }
	}

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

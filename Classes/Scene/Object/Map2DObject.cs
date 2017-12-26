using UnityEngine;
using System.Collections;

/// <summary>
/// 2D地图对象
/// @author hannibal
/// @time 2014-11-1
/// </summary>
public class Map2DObject : BaseObject, IGridObject
{
    [Header("Map2DObject")]
    [SerializeField, Tooltip("所在地图的行(readonly)")]
	protected int           m_RowIndex = 0;
    [SerializeField, Tooltip("所在地图的列(readonly)")]
	protected int           m_ColIndex = 0;

	/**对象所在的格子*/
    protected TerrainGrid   m_TerrainGrid = null;

    [SerializeField, Tooltip("速度方向(readonly)")]
    protected Vector3       m_VelocityDir = Vector3.zero;
    [SerializeField, Tooltip("速度(readonly)")]
    protected float         m_VelocityPower = 0;
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

    public override void Destroy()
    {
        if (m_TerrainGrid != null)
        {
            m_TerrainGrid.removeObject(this);
            m_TerrainGrid = null;
        }
        base.Destroy();
	}

	/// <summary>
    /// 所在格子发生改变
	/// </summary>
	public virtual void OnMapGridChangle(int new_row, int new_col)
	{
		if(m_TerrainGrid != null)
		{
			m_TerrainGrid.removeObject(this);
			m_TerrainGrid = null;
		}
		
		m_ColIndex = new_col;
		m_RowIndex = new_row;
		
		m_TerrainGrid = TerrainGridMap.Instance.getNode(m_ColIndex, m_RowIndex);
		if(m_TerrainGrid != null)
		{
			m_TerrainGrid.addObject(this);
        }
        EventController.TriggerEvent(ObjectEvent.MAP_GRID_CHANGE, m_ObjectUID, m_RowIndex, m_ColIndex);
	}
    public override void SetPosition(float x, float y, float z)
    {
        base.SetPosition(x, y);
    }
    public override void SetPosition(Vector3 pos)
    {
        this.SetPosition((Vector2)pos);
    }
    /// <summary>
    /// 深度
    /// </summary>
    /// <param name="z_depth"></param>
    public virtual void SetDepth(float z_depth)
    {
        MathUtils.TMP_VECTOR3.Set(transform.localPosition.x, transform.localPosition.y, z_depth);
        transform.localPosition = MathUtils.TMP_VECTOR3;
    }
    public override void OnPositionChange()
    {
        base.OnPositionChange();

        //所在格子是否发生变化
        TerrainGrid grid = TerrainGridMap.Instance.getNodeByPostion(this.Position.x, this.Position.y);
        if(grid != null && grid != m_TerrainGrid)
        {
            this.OnMapGridChangle(grid.row, grid.col);
        }

        EventController.TriggerEvent(ObjectEvent.MAP_OBJ_POS, this.ObjectUID, transform.position);
    }
    public virtual float GetMoveSpeed()
    {
        return m_VelocityPower;
    }
    /// <summary>
    /// 朝向
    /// </summary>
    /// <returns></returns>
    public override Vector3 Forward
    {
        get { return Math2DUtils.Radians2Point(this.Rotate2D); }
    }
    /// <summary>
    /// 设置朝向
    /// </summary>
    /// <param name="dir">忽视z轴</param>
    public override void SetForward(Vector3 dir)
    {
        //转换成角度
        float angle = Math2DUtils.Point2Radians(dir.x, dir.y);
        this.SetRotate2D(angle);
    }
    /// <summary>
    /// 绕z轴旋转弧度值
    /// </summary>
    public virtual float Rotate2D
    {
        get { return transform.localEulerAngles.z * Mathf.Deg2Rad; }
    }
    /// <summary>
    /// 2d旋转绕z轴
    /// </summary>
    /// <param name="angle">弧度</param>
    public virtual void SetRotate2D(float angle)
    {
        float degree = angle * Mathf.Rad2Deg;
        transform.Rotate(Vector3.forward, MathUtils.Cleap0_360(degree), Space.World);
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
		get{ return m_TerrainGrid; }
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

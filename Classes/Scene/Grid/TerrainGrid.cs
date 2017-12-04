using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 单个地图格子
/// @author hannibal
/// @time 2015-8-10
/// </summary>
public class TerrainGrid
{
	public int col = 0;   
	public int row = 0;
    public Rect rect = new Rect();
    public TerrainGrid parent = null;   
	
	public float f = 0f;   
	public float g = 0f;   
	public float h = 0f;    

	/**土路节点肯定比高速公路的代价大，沼泽或是高山节点代价可能大很多。不一样的代价可以通过引用附加的属性来添加 */		
	private float m_cost = 1f; 
	private bool m_walkable = true;
	
	private float m_alpha = 1f;

	/**格子上的对象列表*/
	private List<IGridObject> m_arr_grid_obj = new List<IGridObject>();

	public TerrainGrid(int row = 0, int col = 0, float w = 0, float h = 0)   
	{     
		this.row = row;
		this.col = col; 
		rect.x = col*w;
		rect.y = row*h;
		rect.width = w;
		rect.height = h;
	} 
	
	public void reset()
	{
		col = row = 0;
		f = g = h = 0;
		parent = null;
		cost = 1.0f;
		rect.x = 0;
		rect.y = 0;
		rect.width = 0;
		rect.height = 0;
	}

    public void setPosition(float x, float y)
    {
        rect.x = x;
        rect.y = y;
    }

    public Vector2 pos
    {
        get { return rect.center; }
    }
	
	public bool equal(TerrainGrid g)
	{
		return ((this.col == g.col && this.row == g.row) ? true : false);
	}
	
	public float cost
	{
		get{ return m_cost; }
		set
		{
			m_cost = value;
			if(m_cost < 1)m_cost = 1;
			m_walkable = m_cost >= PathFinderID.OBSTACLE ? false : true;
		}
	}

	public bool walkable
	{
		get{ return m_walkable; }
	}
	
	public float alpha
	{
		get{ return m_alpha; }
		set{ m_alpha = value; }
	}
	/*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～grid obj～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
	public List<IGridObject> arr_grid_obj
	{
		get{return m_arr_grid_obj;}
	}
	public void addObject(IGridObject obj)
	{
		if(obj == null)return;
		
		if(m_arr_grid_obj.Contains(obj))return;
		
		m_arr_grid_obj.Add(obj);
	}
	public void removeObject(IGridObject obj)
	{
		if(obj == null)return;
		
		m_arr_grid_obj.Remove(obj);
	}
}

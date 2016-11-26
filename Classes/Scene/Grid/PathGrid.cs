using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathGrid
{
	public int col = 0;   
	public int row = 0;  
	public Rect rect_collide = new Rect();
	
	public float f = 0f;   
	public float g = 0f;   
	public float h = 0f;    
	public PathGrid parent = null;   
	/**土路节点肯定比高速公路的代价大，沼泽或是高山节点代价可能大很多。不一样的代价可以通过引用附加的属性来添加 */		
	private float m_costMultiplier = 1f; 
	private bool m_walkable = true;
	
	private float m_alpha = 1f;

	/**格子上的对象列表*/
	private List<IGridObject> m_arr_grid_obj = new List<IGridObject>();

	public PathGrid(int row = 0, int col = 0, float w = 0, float h = 0)   
	{     
		this.row = row;
		this.col = col; 
		rect_collide.x = col*w;
		rect_collide.y = row*h;
		rect_collide.width = w;
		rect_collide.height = h;
	} 
	
	public void reset()
	{
		col = row = 0;
		f = g = h = 0;
		parent = null;
		costMultiplier = 1.0f;
		rect_collide.x = 0;
		rect_collide.y = 0;
		rect_collide.width = 0;
		rect_collide.height = 0;
	}
	
	public bool equal(PathGrid g)
	{
		return ((this.col == g.col && this.row == g.row) ? true : false);
	}
	
	public float costMultiplier
	{
		get{ return m_costMultiplier; }
		set
		{
			m_costMultiplier = value;
			if(m_costMultiplier < 1)m_costMultiplier = 1;
			m_walkable = m_costMultiplier >= PathFinderID.OBSTACLE ? false : true;
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

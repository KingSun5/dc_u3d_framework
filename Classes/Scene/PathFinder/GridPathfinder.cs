using UnityEngine;
using System.Collections;

/// <summary>
/// 格子寻路
/// @author hannibal
/// @time 2015-8-10
/// </summary>
public class GridPathfinder : Singleton<GridPathfinder>
{
	/**
	 * 寻路时的步长 
	 */		
	static public float STEP_LENGTH = 0.5f;
	/**
	 * 寻路成功时的目标点 
	 */		
	private float m_targetPos_X;
	private float m_targetPos_Y;
	
	private float m_last_pos_x;
	private float m_last_pos_y;
	
	private Rect m_temp_collide_rect;

	private eFaceType m_walkDir = eFaceType.NONE;
	
	/**地图障碍数据*/
	private PathGridMap m_grid_map = null; 
	private PathGrid m_cur_grid = null;

	/**************************************************************************/
	/*公共方法																  */
	/**************************************************************************/
	public GridPathfinder()
	{
		m_temp_collide_rect = new Rect();
		m_targetPos_X = 0;
		m_targetPos_Y = 0;
	}

	public void setup(PathGridMap grid_map)
	{
		m_grid_map = grid_map; 
		
		m_targetPos_X = 0;
		m_targetPos_Y = 0;
	}
	public void destroy()
	{
		m_grid_map = null;
		m_cur_grid = null;
	}
	public void update(PathGridMap grid_map)
	{
		m_grid_map = grid_map; 
		
		m_targetPos_X = 0;
		m_targetPos_Y = 0;
	}
	/**
	 * 寻路接口 
	 * @param cur_x
	 * @param cur_y
	 * @param target_x
	 * @param target_y
	 * @param walkDir
	 * @return 
	 * 
	 */		
	public PathFinderID.EFinderResult search(float cur_x, float cur_y, float target_x, float target_y, float width, float height)
	{
		if(m_grid_map == null)return PathFinderID.EFinderResult.FAILED;

		m_walkDir = 0;
		m_cur_grid = null;
		m_temp_collide_rect.width = width;
		m_temp_collide_rect.height = height;

		//步长个数
		float stepCount = Math2DUtils.distance(cur_x, cur_y, target_x, target_y)/STEP_LENGTH;
		
		//上一步的位置
		m_last_pos_x = cur_x;
		m_last_pos_y = cur_y;
		
		//步长增长方向
		Vector2 pt_step = Math2DUtils.normalPoint(cur_x, cur_y, target_x, target_y, STEP_LENGTH);
		float x_step_inc = pt_step.x;
		float y_step_inc = pt_step.y;
		stepCount = Math2DUtils.distance(cur_x, cur_y, target_x, target_y)/STEP_LENGTH;
		
		//确定移动方向
		PathFinderID.EFinderResult result = PathFinderID.EFinderResult.FAILED;
		float angle = MathUtils.ToDegree(Math2DUtils.LineRadians(cur_x, cur_y, target_x, target_y));
		m_walkDir = (eFaceType)Math2DUtils.getFace(angle, 8);
		
		m_cur_grid = m_grid_map.getNodeByPostion(cur_x, cur_y);
		if(m_cur_grid == null)return PathFinderID.EFinderResult.FAILED;
		
		//重新设置方向
		m_walkDir = resetDir(m_cur_grid, m_walkDir);
		switch((eFaceType)m_walkDir)
		{ 
		case eFaceType.RIGHT:
			y_step_inc = 0;
			break;
			
		case eFaceType.DOWN:
			x_step_inc = 0;
			break;
			
		case eFaceType.LEFT:
			y_step_inc = 0;
			break;
			
		case eFaceType.UP:
			x_step_inc = 0;
			break;
		}
		//开始寻路，每次移动一个步长
		int i = 1;
		for(i=1; i<=stepCount; i++)//从1开始，当前位置不判断
		{
			if(_isWalkableRect(cur_x+x_step_inc*i, cur_y+y_step_inc*i))
			{
				m_last_pos_x=cur_x + x_step_inc*i;
				m_last_pos_y=cur_y + y_step_inc*i;
				result = PathFinderID.EFinderResult.SUCCEEDED;
			}
			else
			{
				result = PathFinderID.EFinderResult.SUCCEEDED_NEAREST;
				break;
			}
		}
		m_targetPos_X = m_last_pos_x;
		m_targetPos_Y = m_last_pos_y;

		return result;
	}
	
	/**************************************************************************/
	/*公共属性																  */
	/**************************************************************************/
	public float getTargetPos_X()
	{
		return this.m_targetPos_X;
	}
	public float getTargetPos_Y()
	{
		return this.m_targetPos_Y;
	}
	/**************************************************************************/
	/*私有方法																  */
	/**************************************************************************/
	/**
	 * 是否可行走
	 */	
	private bool _isWalkableRect(float x, float y)
	{
		m_temp_collide_rect.x = x - m_temp_collide_rect.width*0.5f;
		m_temp_collide_rect.y = y - m_temp_collide_rect.height*0.5f;
		
		PathGrid grid = m_grid_map.getNodeByPostion(x, y);
		//在障碍里面
		if(grid == null || !grid.walkable)
		{
			return false;
		}
		
		int i;
		int j;
		int grid_row = grid.row;
		int grid_col = grid.col;
		PathGrid tempTile = null;
		switch(m_walkDir)
		{ 
		case eFaceType.RIGHT://校验相邻格子
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkBlock(tempTile))return false;
			return true;
			
		case eFaceType.DOWN:
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkBlock(tempTile))return false;
			return true;
			
		case eFaceType.LEFT:
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkBlock(tempTile))return false;
			return true;
			
		case eFaceType.UP:
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkBlock(tempTile))return false;
			return true;
			
		case eFaceType.RIGHT_DOWN:
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkBlock(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkBlock(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row+1,grid_col+1);
			if(!checkBlock(tempTile))return false;
			return true;
			
		case eFaceType.RIGHT_UP:
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkBlock(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkBlock(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row-1,grid_col+1);
			if(!checkBlock(tempTile))return false;
			return true;
			
		case eFaceType.LEFT_DOWN:
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkBlock(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkBlock(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row+1,grid_col-1);
			if(!checkBlock(tempTile))return false;
			return true;
			
		case eFaceType.LEFT_UP:
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkBlock(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkBlock(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row-1,grid_col-1);
			if(!checkBlock(tempTile))return false;
			return true;
		}
		
		return false;
	}
	private eFaceType resetDir(PathGrid cur_tile, eFaceType dir)
	{
		if(cur_tile == null)return dir;

		int grid_row = cur_tile.row;
		int grid_col = cur_tile.col;
		PathGrid tempTile = null;
		switch(dir)
		{ 
		case eFaceType.RIGHT_DOWN:
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkBlock(tempTile))return eFaceType.RIGHT;
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkBlock(tempTile))return eFaceType.DOWN;
			break;
			
		case eFaceType.RIGHT_UP:
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkBlock(tempTile))return eFaceType.RIGHT;
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkBlock(tempTile))return eFaceType.UP;
			break;
			
		case eFaceType.LEFT_DOWN:
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkBlock(tempTile))return eFaceType.LEFT;
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkBlock(tempTile))return eFaceType.DOWN;
			break;
			
		case eFaceType.LEFT_UP:
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkBlock(tempTile))return eFaceType.LEFT;
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkBlock(tempTile))return eFaceType.UP;
			break;
		}
		return dir;
	}
	/**
	 * 判断是否可移动
	 */		
	private bool checkBlock(PathGrid tile)
	{
		if(tile == null)return false;

		if(!tile.walkable && Math2DUtils.intersectRect(tile.rect_collide, m_temp_collide_rect))
			return false;
		return true;
	}
}

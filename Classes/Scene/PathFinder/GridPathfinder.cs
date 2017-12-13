using UnityEngine;
using System.Collections;

/// <summary>
/// 格子寻路
/// @author hannibal
/// @time 2015-8-10
/// </summary>
public class GridPathfinder : Singleton<GridPathfinder>
{
	/// <summary>
	/// 寻路时的步长
	/// </summary>
	private float STEP_LENGTH = 0.1f;
	/// <summary>
	/// 寻路成功时的目标点 
	/// </summary>
	private float m_target_pos_x;
	private float m_target_pos_y;
	
	private float m_last_pos_x;
	private float m_last_pos_y;
	/// <summary>
	/// 碰撞盒
	/// </summary>
	private Rect m_collide_rect = new Rect();

	/**地图障碍数据*/
	private TerrainGridMap m_grid_map = null; 
	private TerrainGrid m_cur_grid = null;

	public void setup(TerrainGridMap grid_map, float step_length = 0.1f)
	{
		m_grid_map = grid_map;
        STEP_LENGTH = step_length;

		m_target_pos_x = 0;
		m_target_pos_y = 0;
	}
	public void destroy()
	{
		m_grid_map = null;
		m_cur_grid = null;
	}

	/// <summary>
    /// 寻路接口 
	/// </summary>
	/// <param name="cur_x"></param>
	/// <param name="cur_y"></param>
	/// <param name="target_x"></param>
	/// <param name="target_y"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <returns></returns>
	public eFinderResult search(float cur_x, float cur_y, float target_x, float target_y, float width, float height)
	{
		if(m_grid_map == null)return eFinderResult.FAILED;

        eFace8Type walk_dir = eFace8Type.NONE;
		m_cur_grid = null;
		m_collide_rect.width = width;
		m_collide_rect.height = height;

		//上一步的位置
		m_last_pos_x = cur_x;
		m_last_pos_y = cur_y;
		
		//步长增长方向
		Vector2 pt_step = Math2DUtils.normalPoint(cur_x, cur_y, target_x, target_y, STEP_LENGTH);
		float x_step_inc = pt_step.x;
        float y_step_inc = pt_step.y;
        //步长个数
        int stepCount = (int)Mathf.Ceil(Math2DUtils.distance(cur_x, cur_y, target_x, target_y) / STEP_LENGTH);
		
		//确定移动方向
		float angle = MathUtils.ToDegree(Math2DUtils.LineRadians(cur_x, cur_y, target_x, target_y));
        walk_dir = (eFace8Type)Math2DUtils.getFace(angle, 8);
		
		m_cur_grid = m_grid_map.getNodeByPostion(cur_x, cur_y);
		if(m_cur_grid == null)return eFinderResult.FAILED;
		
		//重新设置方向
        walk_dir = resetDir(m_cur_grid, walk_dir);
        switch (walk_dir)
        {
        case eFace8Type.LEFT:
		case eFace8Type.RIGHT:
			y_step_inc = 0;
			break;

        case eFace8Type.UP:
		case eFace8Type.DOWN:
			x_step_inc = 0;
			break;
		}
		//开始寻路，每次移动一个步长
        eFinderResult result = eFinderResult.FAILED;
		for(int i=1; i<=stepCount; i++)//从1开始，当前位置不判断
		{
            if (isWalkableRect(walk_dir, cur_x + x_step_inc * i, cur_y + y_step_inc * i))
			{
				m_last_pos_x=cur_x + x_step_inc*i;
				m_last_pos_y=cur_y + y_step_inc*i;
				result = eFinderResult.SUCCEEDED;
			}
			else
			{
                //如果第一步都不能走，设置查询失败
                if(i == 1)
                    result = eFinderResult.FAILED;
                else
                    result = eFinderResult.SUCCEEDED_NEAREST;
				break;
			}
		}
		m_target_pos_x = m_last_pos_x;
		m_target_pos_y = m_last_pos_y;

		return result;
	}
	
    /// <summary>
    /// 最终可移动点
    /// </summary>
    /// <returns></returns>
	public float target_x
	{
        get { return this.m_target_pos_x; }
	}
    public float target_y
    {
        get { return this.m_target_pos_y; }
    }
	/// <summary>
    /// 是否可行走
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private bool isWalkableRect(eFace8Type dir, float x, float y)
	{
		m_collide_rect.x = x - m_collide_rect.width*0.5f;
		m_collide_rect.y = y - m_collide_rect.height*0.5f;
		
		TerrainGrid grid = m_grid_map.getNodeByPostion(x, y);
		//在障碍里面
		if(grid == null || !grid.walkable)
		{
			return false;
		}
		
		int grid_row = grid.row;
		int grid_col = grid.col;
		TerrainGrid tempTile = null;
        switch (dir)
		{ 
		case eFace8Type.RIGHT://校验相邻格子
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkCanMove(tempTile))return false;
			return true;
			
		case eFace8Type.DOWN:
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkCanMove(tempTile))return false;
			return true;
			
		case eFace8Type.LEFT:
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkCanMove(tempTile))return false;
			return true;
			
		case eFace8Type.UP:
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkCanMove(tempTile))return false;
			return true;
			
		case eFace8Type.RIGHT_DOWN:
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkCanMove(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkCanMove(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row+1,grid_col+1);
			if(!checkCanMove(tempTile))return false;
			return true;
			
		case eFace8Type.RIGHT_UP:
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkCanMove(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkCanMove(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row-1,grid_col+1);
			if(!checkCanMove(tempTile))return false;
			return true;
			
		case eFace8Type.LEFT_DOWN:
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkCanMove(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkCanMove(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row+1,grid_col-1);
			if(!checkCanMove(tempTile))return false;
			return true;
			
		case eFace8Type.LEFT_UP:
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkCanMove(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkCanMove(tempTile))return false;
			tempTile = m_grid_map.getNode(grid_row-1,grid_col-1);
			if(!checkCanMove(tempTile))return false;
			return true;
		}
		
		return false;
	}
	private eFace8Type resetDir(TerrainGrid cur_tile, eFace8Type dir)
	{
		if(cur_tile == null)return dir;

		int grid_row = cur_tile.row;
		int grid_col = cur_tile.col;
		TerrainGrid tempTile = null;
		switch(dir)
		{ 
		case eFace8Type.RIGHT_DOWN:
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkCanMove(tempTile))return eFace8Type.RIGHT;
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkCanMove(tempTile))return eFace8Type.DOWN;
			break;
			
		case eFace8Type.RIGHT_UP:
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkCanMove(tempTile))return eFace8Type.RIGHT;
			tempTile = m_grid_map.getNode(grid_row,grid_col+1);
			if(!checkCanMove(tempTile))return eFace8Type.UP;
			break;
			
		case eFace8Type.LEFT_DOWN:
			tempTile = m_grid_map.getNode(grid_row+1,grid_col);
			if(!checkCanMove(tempTile))return eFace8Type.LEFT;
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkCanMove(tempTile))return eFace8Type.DOWN;
			break;
			
		case eFace8Type.LEFT_UP:
			tempTile = m_grid_map.getNode(grid_row-1,grid_col);
			if(!checkCanMove(tempTile))return eFace8Type.LEFT;
			tempTile = m_grid_map.getNode(grid_row,grid_col-1);
			if(!checkCanMove(tempTile))return eFace8Type.UP;
			break;
		}
		return dir;
	}
	/// <summary>
    /// 判断是否可移动
	/// </summary>
	/// <param name="tile"></param>
	/// <returns></returns>
	private bool checkCanMove(TerrainGrid tile)
	{
		if(tile == null)return false;

        if (!tile.walkable && Math2DUtils.intersectRect(tile.rect, m_collide_rect))
			return false;
		return true;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 地图格子管理
/// @author hannibal
/// @time 2015-8-10
/// </summary>
public class PathGridMap : Singleton<PathGridMap>
{
	private PathGrid m_startNode = null; 
	private PathGrid m_endNode = null; 
	private PathGrid [,] m_nodes;
	private eAligeType m_alige = eAligeType.LEFT_BOTTOM;
	/**单个格子大小*/
	private float m_gridWidth = 1;
	private float m_gridHeight = 1;
	private int m_numCols = 0; 
	private int m_numRows = 0; 
	
	/**拾取物品格子大小*/
	private float m_gridPickWidth = 1;
	private float m_gridPickHeight = 1;
	private int m_numPickCols = 0; 
	private int m_numPickRows = 0; 

	public void setup(int numRows, int numCols, float gridW = 1, float gridH = 1, float pickW = 1, float pickH = 1, eAligeType alige = eAligeType.LEFT_BOTTOM)
	{
		m_numRows = numRows; 
		m_numCols = numCols; 
		m_gridWidth = gridW;
		m_gridHeight = gridH;
		m_gridPickWidth = pickW;
		m_gridPickHeight = pickH;
		m_numPickCols = (int)((m_numCols*m_gridWidth) / m_gridPickWidth);
		m_numPickRows = (int)((m_numRows*m_gridHeight) / m_gridPickHeight);
		m_alige = alige;

		m_nodes = new PathGrid[m_numRows,m_numCols];
		for(int row = 0; row < m_numRows; row++) 
		{ 
			for(int col = 0; col < m_numCols; col++) 
			{ 
				m_nodes[row,col] = new PathGrid(row, col, m_gridWidth, m_gridHeight);
				switch(m_alige)
				{
				case eAligeType.LEFT_BOTTOM:
                    {//(0,0)格子在左下方
                        float x = -(numCols * gridW) * 0.5f + m_nodes[row, col].rect.x;
                        float y = -(numRows * gridH) * 0.5f + m_nodes[row, col].rect.y;
                        m_nodes[row, col].setPosition(x, y);
                    }
					break;
                case eAligeType.LEFT_TOP:
                    {//(0,0)格子在左上方
                        float x = m_nodes[row, col].rect.x;
                        float y = m_nodes[row, col].rect.y;
                        m_nodes[row, col].setPosition(x, y);
                    }
                    break;
				}
			}
		}
	}
	
	public void destroy()
	{
		m_startNode = null;
		m_endNode = null;
		for(int i = 0; i < m_numRows; i++) 
		{ 
			for(int j = 0; j < m_numCols; j++) 
			{ 
				m_nodes[i,j] = null;
			} 
		}
		m_nodes = null;
	}

	public PathGrid getNode(int row, int col)
	{ 
		if(isValidRowCol(row, col))
			return m_nodes[row,col] as PathGrid; 
		return null;
	}
	public void setEndNode(int row, int col) 
	{ 
		if(!isValidRowCol(row, col))
			return;
		m_endNode = m_nodes[row,col] as PathGrid; 
	} 
	public void setStartNode(int row, int col) 
	{ 
		if(!isValidRowCol(row, col))
			return;
		m_startNode = m_nodes[row,col] as PathGrid; 
	} 
	public void setCost(int row, int col, float cost) 
	{ 
		if(!isValidRowCol(row, col) || m_nodes[row,col] == null)
			return;
        m_nodes[row, col].cost = cost; 
	}
	
	public void setAlpha(int row, int col, float alpha) 
	{ 
		if(!isValidRowCol(row, col) || m_nodes[row,col] == null)
			return;
		m_nodes[row,col].alpha = alpha; 
	}
	public float getAlpha(int row, int col) 
	{ 
		if(m_nodes == null || !isValidRowCol(row, col) || m_nodes[row,col] == null)
			return 1;
		return m_nodes[row,col].alpha; 
	}
    /// <summary>
    /// 是否有效坐标
    /// </summary>
    /// <param name="x">unity坐标系</param>
    /// <param name="y">unity坐标系</param>
    /// <returns></returns>
	public bool isValidPos(float x, float y)
	{
		switch(m_alige)
		{
		case eAligeType.LEFT_BOTTOM:
			x += m_gridWidth*m_numCols*0.5f;
			y += m_gridHeight*m_numRows*0.5f;
			break;
		}
		if(x < 0 || y < 0 || y > m_numRows*m_gridHeight || x > m_numCols*m_gridWidth)
			return false;
		return true;
	}
	public bool isValidRowCol(int row, int col)
	{
		if(row>=0 && row<m_numRows && col>=0 && col<m_numCols)
			return true;
		return false;
	}

	public PathGrid endNode
	{ 
		get { return m_endNode; }
	} 

	public int numCols
	{ 
		get { return m_numCols; }
	} 
	public int numRows
	{ 
		get { return m_numRows; }
	} 
	public PathGrid startNode 
	{ 
		get { return m_startNode; }
	} 
	
	public float gridWidth 
	{ 
		get { return m_gridWidth;  }
	} 
	public float gridHeight 
	{ 
		get { return m_gridHeight; } 
	} 
	public float width 
	{ 
		get { return m_gridWidth * m_numCols; } 
	} 
	public float height 
	{ 
		get { return m_gridHeight * m_numRows; } 
	} 
	/**
	 * 根据位置获得寻路格子
	 */		
	public int getNodeColByPos(float x)
	{
		switch(m_alige)
		{
		case eAligeType.LEFT_BOTTOM:
			x += m_gridWidth*m_numCols*0.5f;
			break;
		}
		return (int)(x / m_gridWidth);
	}
	public int getNodeRowByPos(float y)
	{
		switch(m_alige)
		{
		case eAligeType.LEFT_BOTTOM:
			y += m_gridHeight*m_numRows*0.5f;
			break;
		}
		return (int)(y / m_gridHeight);
	}
	/**
	 * 根据格子获得位置 
	 */		
	public float getPosXByGridCol(int col)
	{
		float x = m_gridWidth * col;
		switch(m_alige)
		{
		case eAligeType.LEFT_BOTTOM:
			x -= m_gridWidth*m_numCols*0.5f;
			break;
		}
		return x;
	}
	public float getPosYByGridRow(int row)
	{
		float y = m_gridHeight * row;
		switch(m_alige)
		{
		case eAligeType.LEFT_BOTTOM:
			y -= m_gridHeight*m_numRows*0.5f;
			break;
		}
		return y;
	}
    /// <summary>
    /// 根据坐标获取格子
    /// </summary>
    /// <param name="x">unity坐标系</param>
    /// <param name="y">unity坐标系</param>
    /// <returns></returns>
	public PathGrid getNodeByPostion(float x, float y)
	{
		if(!isValidPos(x, y))return null;

		int row = getNodeRowByPos(y);
		int col = getNodeColByPos(x);
		if(isValidRowCol(row, col))
			return m_nodes[row,col];

		return null;
	}
	
	public int getPickNodeColByPos(float x)
	{
		switch(m_alige)
		{
			case eAligeType.LEFT_BOTTOM:
				x += m_gridWidth*m_numCols*0.5f;
			break;
		}
		return (int)((x) / m_gridPickWidth);
	}
	public int getPickNodeRowByPos(float y)
	{
		switch(m_alige)
		{
			case eAligeType.LEFT_BOTTOM:
				y += m_gridHeight*m_numRows*0.5f;
			break;
		}
		return (int)((y) / m_gridPickHeight);
	}
 
	public uint getColor(PathGrid node)   
	{  
		if(node == null || !node.walkable)
			return 0;  
		return 0xffffff;   
	}

	public int numPickCols
	{
		get { return m_numPickCols; }
		set { m_numPickCols = value; }
	}
	public int numPickRows
	{
		get { return m_numPickRows; }
		set { m_numPickRows = value; }
	}

	public PathGrid [,] nodes
	{
		get { return m_nodes; }
	}
}

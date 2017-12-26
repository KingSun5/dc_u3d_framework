using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
A_star 运算法则   
    Grid ：本质上就是方形网格里的某一个方格, 由此可以看出，路径将会由起点节点，终点节点，还有从起点到终点经过的节点组成。  
    代价（cost） ：这是对节点优劣分级的值。代价小的节点肯定比代价大节点更好。代价由两部分组成：从起点到达当前点的代价和从这个点到终点的估计代价。代价一般由变量 f，g 和 h，具体如下。  
        f：特定节点的全部代价。由 g+h 决定。  
        g：从起点到当前点的代价。它是确定的，因为你肯定知道从起点到这一点的实际路径。  
        h：从当前点到终点的估计代价。是用估价函数（heuristic function）计算的。它只能一个估算，因为你不知道具体的路线——你将会找出的那一条。 
    估价函数（heuristic） ：计算从当前点到终点估计代价的公式。通常有很多这样的公式，但他们的运算结果，速度等都有差异（yujjj 注：估价公式计算的估计值越接近实际值，需要计算的节点越少；估价公式越简单，每个节点的计算速度越快）。  
    待考察表（open list） ：一组已经估价的节点。表里代价最小的节点将是下一次的计算的起点。  
    已考察表（closed list） ：从待考察表中取代价最小的节点作为起点，对它周围 8 个方向的节点进行估价，然后把它放入“已考察表”。  
    父节点（parent node）:以一个点计算周围节点时，这个点就是其它节点的父节点。当我们到达终点节点，你可以一个一个找出父节点直到起点节点。因为父节点总是带考察表里的小代价节点，这样可以确保你找出最佳路线。
	
    现在我们来看以下具体的运算方法：  
    1. 添加起点节点到待考察表  
    2. 主循环  
        a. 找到待考察表里的最小代价的节点，设为当前节点。  
        b. 如果当前点是终点节点，你已经找到路径了。跳到第四步。  
        c. 考察每一个邻节点（直角坐标网格里，有 8 个这样的节点 ）对于每一个邻节点： 
            (1).如果是不能通过的节点，或者已经在带考察表或已考察表中，跳过，继续下一节点，
                否则继续
            (2).计算它的代价  
            (3).把当前节点定义为这个点的父节点添加到待考察表  
            (4).添加当前节点到已考察表  
    3. 更新待考察表，重复第二步。  
    4. 你已经到达终点，创建路径列表并添加终点节点  
    5. 添加终点节点的父节点到路径列表  
    6. 重复添加父节点直到起点节点。路径列表就由一组节点构成了最佳路径
*/
public class AStarPathfinder : Singleton<AStarPathfinder>
{
    private bool m_active = false;

    /**默认最大搜索次数*/
    private const int MAX_DEFAULT_SEARCH_COUNT = 200;
    private int m_max_search_count = MAX_DEFAULT_SEARCH_COUNT;

    /**开启和封闭路点列表*/
    private List<TerrainGrid> m_array_open = null;
    private List<TerrainGrid> m_array_closed = null;

    /**地图障碍数据*/
    private TerrainGridMap m_grid_map = null;

    /**寻路起点和终点*/
    private TerrainGrid m_start_node;
    private TerrainGrid m_end_node;
    private Vector2 m_start_pos;
    private Vector2 m_end_pos;

    /**最终寻路结果*/
    private List<TerrainGrid> m_array_search_path = null;
    //寻路失败可以到达的最近点
    private TerrainGrid m_failed_last_node = null;

    /**基础代价*/
    private float m_straight_cost = 1.0f;
    private float m_diag_cost = 1.414f;

    /**************************************************************************/
    /*公共方法																  */
    /**************************************************************************/
    public AStarPathfinder()
    {
    }

    public void setup(TerrainGridMap grid_map)
    {
        m_active = true;

        m_grid_map = grid_map;

        m_start_node = new TerrainGrid();
        m_end_node = new TerrainGrid();
        m_array_search_path = new List<TerrainGrid>();
        m_array_open = new List<TerrainGrid>();
        m_array_closed = new List<TerrainGrid>();
    }

    public void destroy()
    {
        m_array_search_path.Clear();
        m_array_open.Clear();
        m_array_closed.Clear();
        m_grid_map = null;
        m_start_node = null;
        m_end_node = null;
        m_array_open = null;
        m_array_closed = null;
        m_failed_last_node = null;
        m_array_search_path = null;

        m_active = false;
    }
    /**
     * 寻路接口 
     * @param startNode 起点
     * @param endNode 终点
     * @return true - 寻路成功；false - 寻路失败
     */
    public eFinderResult search(Vector2 startPos, Vector2 endPos, bool isFindNearstPath = false)
    {
        if (!m_active || m_grid_map == null) return eFinderResult.FAILED;

        m_array_search_path.Clear();
        m_array_open.Clear();
        m_array_closed.Clear();

        m_start_pos = startPos;
        m_end_pos = endPos;
        m_start_node = m_grid_map.getNodeByPostion(startPos.x, startPos.y);
        m_end_node = m_grid_map.getNodeByPostion(endPos.x, endPos.y);
//			if(!m_start_node || !m_start_node.walkable)
//			{
//				PaperLogger.error("AStarPathfinder::findPath - 角色起点在障碍里面");
//				return eFinderResult.FAILED;
//			}
        if (m_start_node == null)
        {
            Log.Error("AStarPathfinder::findPath - 角色起点在障碍里面");
            return eFinderResult.FAILED;
        }
        if (m_start_node == null || !m_end_node.walkable)
        {
            return eFinderResult.FAILED;
        }
        m_start_node.g = 0;
        m_start_node.h = onDiagonal(m_start_node);
        m_start_node.f = m_start_node.g + m_start_node.h;

        //时间
        float old_time = Time.realtimeSinceStartup;

        //执行寻路
        eFinderResult result = travel(isFindNearstPath);
        if (result == eFinderResult.FAILED)
        {
            if (isFindNearstPath && m_failed_last_node != null &&
                m_failed_last_node.equal(m_start_node) == false)
            {//找最近点
                m_array_search_path.Add(m_start_node);
                m_array_search_path.Add(m_failed_last_node);
                result = eFinderResult.SUCCEEDED_NEAREST;
            }
        }

        Log.Debug("[AI]AStarPathfinder::search - 寻路总用时:", (Time.realtimeSinceStartup - old_time) + "s");

        return result;
    }

    /**************************************************************************/
    /*公共属性																  */
    /**************************************************************************/
    public List<TerrainGrid> path()
    {
        return m_array_search_path;
    }

    /**************************************************************************/
    /*私有方法																  */
    /**************************************************************************/
    private eFinderResult travel(bool isFindNearstPath)
    {
        TerrainGrid node = m_start_node;
        int search_count = 0;
        while (!node.equal(m_end_node) && search_count < m_max_search_count)
        {
            ++search_count;
            int startX = Mathf.Max(0, node.col - 1);
            int endX = Mathf.Min(m_grid_map.numCols - 1, node.col + 1);
            int startY = Mathf.Max(0, node.row - 1);
            int endY = Mathf.Min(m_grid_map.numRows - 1, node.row + 1);
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    TerrainGrid test = m_grid_map.getNode(i, j);
                    if (test == null || test.equal(node) ||
                        !test.walkable ||
                        !m_grid_map.getNode(node.col, test.row).walkable ||   //拐角不能通过
                        !m_grid_map.getNode(test.col, node.row).walkable)
                    {
                        continue;
                    }
                    float cost = m_straight_cost;
                    if (!((node.col == test.col) || (node.row == test.row)))
                    {
                        cost = m_diag_cost;
                    }
                    /*
                    经过前面的这些，留下的就是需要计算的节点。首先计算从开始节点到测试节点的代价（g），
                    方法是当前节点的 g 值加上当前节点到测试节点的代价。简化以后就是水平、竖直方向直接加上
                    _straightCost，对角加上_diagCost.h 通过估价函数计算，然后g和h 求和，得到 f（总代价）
                    */
                    float g = node.g + cost * test.cost;
                    float h = onDiagonal(test);
                    float f = g + h;

                    /*
                    下面这个部分有一点小技巧，之前我们并没有谈到。开始的时候，我说过如果一个节点在待考
                    察表/已考察表里，因为它已经被考察过了，所以我们不需要再考察。不过这次计算出的结果有可
                    能小于你之前计算的结果（比如说，上次计算时是对角，而这次确是上下或左右关系，代价就小一
                    些）。所以，就算一个节点在待考察表/已考察表里面，最好还是比较一下当前值和之前值之间的大
                    小。具体做法是比较测试节点的总代价与以前计算出来的总代价。如果以前的大，我们就找到了更
                    好的节点，我们就需要重新给测试点的 f，g，h 赋值，同时，我们还要把测试点的父节点设为当前
                    点。这就要我们向后追溯
                    */
                    if (isOpen(test) || isClosed(test))
                    {
                        if (test.f > f)
                        {
                            test.f = f;
                            test.g = g;
                            test.h = h;
                            test.parent = node;
                        }
                    }
                    else
                    {
                        /*
                        如果测试节点不再待考察表/已考察表里面，我们只需要赋值给 f，g，h 和父节点。然后把测
                        试点加到待考察表，然后是下一个测试点，找出最佳点
                        */
                        test.f = f;
                        test.g = g;
                        test.h = h;
                        test.parent = node;
                        m_array_open.Add(test);
                    }
                }
            }
            m_array_closed.Add(node);
            if (m_array_open.Count == 0)
            {
                return eFinderResult.FAILED;
            }
            m_array_open.Sort();
            node = m_array_open[0];
            m_array_open.RemoveAt(0);
        }

        if (search_count >= m_max_search_count)
        {
            return eFinderResult.FAILED;
        }
        buildPath(node);
        return eFinderResult.SUCCEEDED;
    }

    private void buildPath(TerrainGrid end_node)
    {
        TerrainGrid node = end_node;
        m_array_search_path.Add(node);
        while (!node.equal(m_start_node))
        {
            node = node.parent;
            m_array_search_path.Insert(0,node);
        }
    }

    private bool isOpen(TerrainGrid test)
	{
        foreach (TerrainGrid t in m_array_open)
		{
			if(t.equal(test))
			{
				return true;
			}
		}
		return false;
	}

    private bool isClosed(TerrainGrid test)
	{
        foreach (TerrainGrid t in m_array_closed)
		{
			if(t.equal(test))
			{
				return true;
			}
		}
		return false;	
	}
    /// <summary>
    /// 对角线估价法 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private float onDiagonal(TerrainGrid node)
    {
        int dx = Mathf.Abs(node.col - m_end_node.col);
        int dy = Mathf.Abs(node.row - m_end_node.row);
        int diag = Mathf.Min(dx, dy);
        int straight = dx + dy;
        return m_diag_cost * diag + m_straight_cost * (straight - 2 * diag);
    }

    public int max_search_count()
    {
        return m_max_search_count;
    }

    public void max_search_count(int value)
    {
        m_max_search_count = value;
    }
}
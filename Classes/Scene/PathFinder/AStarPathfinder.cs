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
    private const int MAX_DEFAULT_SEARCH_COUNT = 2000;
    private int m_max_search_count = MAX_DEFAULT_SEARCH_COUNT;

    /**开启和封闭路点列表*/
    private List<PathGrid> m_array_open = new List<PathGrid>();
    private List<PathGrid> m_array_closed = new List<PathGrid>();

    /**寻路起点和终点*/
    private PathGrid m_start_node = new PathGrid();
    private PathGrid m_end_node = new PathGrid();

    /**最终寻路结果*/
    private List<Vector2> m_array_search_path = new List<Vector2>();
    private List<PathGrid> m_array_search_grid = new List<PathGrid>();

    /**基础代价*/
    private float m_straight_cost = 1.0f;
    private float m_diag_cost = 1.414f;

    /// <summary>
    /// 寻路接口 
    /// </summary>
    /// <param name="grid_map">地图格子列表</param>
    /// <param name="startPos">起点</param>
    /// <param name="endPos">终点</param>
    /// <param name="isFindNearstPath">失败时，是否返回最近可移动路径</param>
    /// <returns></returns>
    public eFinderResult search(PathGridMap grid_map, Vector2 startPos, Vector2 endPos, bool isFindNearstPath = false)
    {
        if (!m_active || grid_map == null) return eFinderResult.FAILED;

        m_array_open.Clear();
        m_array_closed.Clear();
        m_array_search_path.Clear();
        m_array_search_grid.Clear();

        m_start_node = grid_map.getNodeByPostion(startPos.x, startPos.y);
        m_end_node = grid_map.getNodeByPostion(endPos.x, endPos.y);

        //Log.Debug(string.Format("起点{0},{1};终点{2},{3}", m_start_node.row, m_start_node.col, m_end_node.row, m_end_node.col));
        if (m_start_node == null)
        {
            Log.Error("AStarPathfinder::findPath - 角色起点在障碍里面");
            return eFinderResult.FAILED;
        }
        if (m_end_node == null || (!m_end_node.walkable && !isFindNearstPath))
        {
            Log.Error("AStarPathfinder::findPath - 角色终点在障碍里面");
            return eFinderResult.FAILED;
        }
        m_start_node.g = 0;
        m_start_node.h = onDiagonal(m_start_node);
        m_start_node.f = m_start_node.g + m_start_node.h;

        //执行寻路
        //float old_time = Time.realtimeSinceStartup;
        eFinderResult result = travel(grid_map, isFindNearstPath);
        //Log.Debug("[AI]AStarPathfinder::search - 寻路总用时:", (Time.realtimeSinceStartup - old_time) + "s");

        return result;
    }

    public List<Vector2> paths
    {
        get { return m_array_search_path; }
    }
    public List<PathGrid> grids
    {
        get { return m_array_search_grid; }
    }

    public int max_search_count
    {
        set { m_max_search_count = value; }
    }

    #region 寻路
    private eFinderResult travel(PathGridMap grid_map, bool isFindNearstPath)
    {
        PathGrid node = m_start_node;
        int search_count = 0;
        while (!node.equal(m_end_node) && search_count < m_max_search_count)
        {
            ++search_count;
            int start_col = Mathf.Max(0, node.col - 1);
            int end_col = Mathf.Min(grid_map.numCols - 1, node.col + 1);
            int start_row = Mathf.Max(0, node.row - 1);
            int end_row = Mathf.Min(grid_map.numRows - 1, node.row + 1);
            for (int col = start_col; col <= end_col; col++)
            {
                for (int row = start_row; row <= end_row; row++)
                {
                    PathGrid test = grid_map.getNode(row, col);
                    if (test == null || test.equal(node) ||
                        !test.walkable ||
                        !grid_map.getNode(node.row, test.col).walkable ||   //拐角不能通过
                        !grid_map.getNode(test.row, node.col).walkable
                        )
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

    private void buildPath(PathGrid end_node)
    {
        PathGrid node = end_node;
        m_array_search_path.Add(new Vector2(node.row, node.col));
        m_array_search_grid.Add(node);
        while (!node.equal(m_start_node))
        {
            node = node.parent;
            m_array_search_path.Add(new Vector2(node.row, node.col));
            m_array_search_grid.Add(node);
        }
        m_array_search_path.Reverse();
        m_array_search_grid.Reverse();
    }

    private bool isOpen(PathGrid test)
	{
        foreach (PathGrid t in m_array_open)
		{
			if(t.equal(test))
			{
				return true;
			}
		}
		return false;
	}

    private bool isClosed(PathGrid test)
	{
        foreach (PathGrid t in m_array_closed)
		{
			if(t.equal(test))
			{
				return true;
			}
		}
		return false;	
	}
    #endregion

    #region
    /// <summary>
    /// 对角线估价法 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private float onDiagonal(PathGrid node)
    {
        int dx = Mathf.Abs(node.col - m_end_node.col);
        int dy = Mathf.Abs(node.row - m_end_node.row);
        int diag = Mathf.Min(dx, dy);
        int straight = dx + dy;
        return m_diag_cost * diag + m_straight_cost * (straight - 2 * diag);
    }
    /// <summary>
    /// 几何估价法（Euclidian heuristic）
	/// 它计算出两点之间的直线距离，本质公式为勾股定理
    /// </summary>
    private float onEuclidian(PathGrid node)
    {
        int dx = node.col - m_end_node.col;
        int dy = node.row - m_end_node.row;
        return Mathf.Sqrt(dx * dx + dy * dy) * m_straight_cost;
    }
    /// <summary>
    ///  曼哈顿估价法(Manhattan heuristic)
    ///  它忽略所有的对角移动，只添加起点节点和
    ///  终点节点之间的行、列数目。就像你在曼哈顿的大街上一样，比如说，你在（5，40），到（8，43），
    ///  你必须先在一个方向上走过 3 个节点，然后另一个方向上的 3 个节点。有可能是先横走完，在竖走
    ///  完，反之亦然；或者横、竖、横、竖、横、竖，每边都要走 3 个 
    /// </summary>
    private float onManhattan(PathGrid node)
    {
        return Mathf.Abs(node.col - m_end_node.col) * m_straight_cost +
            Mathf.Abs(node.row + m_end_node.row) * m_straight_cost;
    }
    #endregion
}
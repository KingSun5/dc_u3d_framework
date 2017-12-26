using UnityEngine;
using System.Collections;

/// <summary>
/// 格子地形
/// @author hannibal
/// @time 2015-8-14
/// </summary>
public interface IGridTerrain 
{
	PathGrid GetGrid(int row, int col);
}

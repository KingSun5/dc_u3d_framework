using UnityEngine;
using System.Collections;

public class PathFinderID
{
	/**
	 * 寻路结果 
	 */		
	public enum EFinderResult
	{
		FAILED,				//失败
		SUCCEEDED,			//成功
		SUCCEEDED_NEAREST,	//没有找到目标点，而是比较靠近目标的点
	}

	/**
	* 节点类型：也是寻路代价
	*/	
	public static float DEFAULT = 0;	//默认   可走，陆地
	public static float GRASS = 5;		//权重5  可走，草地
	public static float OBSTACLE = 10;	//不可走
}

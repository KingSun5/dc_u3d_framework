using UnityEngine;
using System.Collections;

/// <summary>
/// 对象池接口类
/// @author hannibal
/// @time 2014-11-22
/// </summary>
public interface IPoolsObject
{
	void Init();
	void Release();
	string GetPoolsType();
}

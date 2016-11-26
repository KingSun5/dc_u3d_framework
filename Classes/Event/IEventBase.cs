using UnityEngine;
using System.Collections;

/// <summary>
/// 事件接口
/// @author hannibal
/// @time 2014-12-10
/// </summary>
public interface IEventBase 
{
	void RegisterEvent ();
	void UnRegisterEvent ();
}

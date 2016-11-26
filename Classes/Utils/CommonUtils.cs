using UnityEngine;
using System.Collections;

/// <summary>
/// 公共
/// @author hannibal
/// @time 2014-11-14
/// </summary>
public class CommonUtils 
{
	/**
	* A unique device identifier. It is guaranteed to be unique for every
	* device (Read Only). You can use it (for example) to store high scores in a central server
	* 一个唯一的设备标识符。这是保证为每一台设备是唯一的（只读）。可以使用它在中央服务器来储存高分表。
	*/
	static public string UDID()
	{
		return SystemInfo.deviceUniqueIdentifier;
	}
}

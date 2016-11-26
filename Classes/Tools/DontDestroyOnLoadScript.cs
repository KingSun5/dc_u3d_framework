using UnityEngine;
using System.Collections;

/// <summary>
/// 场景切换时不销毁
/// @author hannibal
/// @time 2014-12-27
/// </summary>
public class DontDestroyOnLoadScript : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad(this.gameObject);
	}
}

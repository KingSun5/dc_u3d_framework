using UnityEngine;
using System.Collections;

/// <summary>
/// 协程
/// @author hannibal
/// @time 2014-12-1
/// </summary>
public class CoroutineManager : MonoBehaviour 
{
	public delegate void Fun(object info);
	static private CoroutineManager m_Instance;

	static public CoroutineManager Instance
	{
		get{return m_Instance;}
	}

	void Awake()
	{
		m_Instance = this;
	}

	void Start () 
	{

	}
    /// <summary>
    /// 延迟回调
    /// </summary>
	public void Add(float time, Fun fun, object info)
	{
		StartCoroutine(HandleFun(time, fun, info));
	}
    /// <summary>
    /// 执行一个函数
    /// </summary>
    public void Add(IEnumerator coroutineFunc)
    {
        StartCoroutine(coroutineFunc);
    }

	IEnumerator HandleFun(float time, Fun fun, object info)  
	{  
		yield return new WaitForSeconds(time);
		fun(info);
	} 
}

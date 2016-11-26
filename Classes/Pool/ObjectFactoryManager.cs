using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 对象创建工厂
/// @author hannibal
/// @time 2014-11-22
/// </summary>
public class ObjectFactoryManager : Singleton<ObjectFactoryManager>
{
	public delegate IPoolsObject funCreateObject();

	private Dictionary<string, funCreateObject> m_DicObjectFactory;

	public ObjectFactoryManager()
	{
        m_DicObjectFactory = new Dictionary<string, funCreateObject>();
	}

	public void Setup()
	{

	}
	public void Destroy()
	{
		m_DicObjectFactory.Clear();
	}

    public bool RegisterFactory(string nPoolsType, funCreateObject fun)
	{
		funCreateObject temp;
		if(m_DicObjectFactory.TryGetValue(nPoolsType, out temp))
		{
			Log.Error("ObjectFactoryManager::registerFactory - has register key:" + nPoolsType.ToString());
			return false;
		}

		m_DicObjectFactory.Add(nPoolsType, fun);

		return true;
	}

    public bool UnregisterFactory(string nPoolsType)
	{
		return m_DicObjectFactory.Remove(nPoolsType);
	}

	public IPoolsObject CreateObject(string nPoolsType)
	{
		IPoolsObject obj = ObjectPoolsManager.Instance.GetObj(nPoolsType);
		if(obj == null)
		{
			funCreateObject fun;
			if(m_DicObjectFactory.TryGetValue(nPoolsType, out fun) == false)
			{
				Log.Error("ObjectFactoryManager::createObject - 没有注册对象工厂:"+nPoolsType.ToString());
				return null;
			}
			//创建
			obj = fun();
		}

		obj.Init();
		return obj;
	}
	public void RecoverObject(IPoolsObject pObj)
	{
		if(pObj == null)return;

		pObj.Release();
		ObjectPoolsManager.Instance.RecoverObj(pObj.GetPoolsType(), pObj);
	}
}


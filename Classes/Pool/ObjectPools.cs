using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 存储所有对象
/// @author hannibal
/// @time 2014-11-22
/// </summary>
public class ObjectPools
{
    private static Dictionary<string, List<IPoolsObject>> m_DicObjects = new Dictionary<string, List<IPoolsObject>>();

    public static IPoolsObject GetObj(string type)
	{
		List<IPoolsObject> listObject;
		if(m_DicObjects.TryGetValue(type, out listObject) == false)
		{
			return null;
		}
		if(listObject.Count <= 0)
		{
			return null;
		}
		IPoolsObject obj = listObject[0];
		listObject.RemoveAt(0);
		return obj;
	}

    public static void RecoverObj(string type, IPoolsObject obj)
	{
		List<IPoolsObject> listObject;
		if(m_DicObjects.TryGetValue(type, out listObject) == false)
		{
			listObject = new List<IPoolsObject>();
			m_DicObjects.Add(type, listObject);
		}
		listObject.Add(obj);
	}

    public static void Clear()
    {
        foreach (var obj in m_DicObjects)
        {
            obj.Value.Clear();
        }

        m_DicObjects.Clear();
    }
}

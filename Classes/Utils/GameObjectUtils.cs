using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GameObject相关
/// @author hannibal
/// @time 2014-11-17
/// </summary>
public class GameObjectUtils
{
    static public GameObject BuildObject(string file)
    {
        if (file.Length == 0)
            return null;

        UnityEngine.Object res = ResourceLoaderManager.Instance.Load(file);
        if (res == null)
        {
            Log.Error("ObjectManager::NewObject - not build file:" + file);
            return null;
        }
        GameObject obj = GameObject.Instantiate(res) as GameObject;
        return obj;
    }
    /// <summary>
    /// 根据名称获得对象或子对象
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Target"></param>
    /// <returns></returns>
	static public Transform GetChildWithName(string Name ,Transform Target)
	{
		if (Target.name == Name)
			return Target;
		
		for (int i=0; i<Target.childCount; i++) 
		{
			Transform Tmp = GetChildWithName(Name,Target.GetChild(i));
			if(Tmp!=null)
				return Tmp;
		}
		
		return null;
	}
    static public void GetChildsWithName(string Name, Transform Target, ref List<Transform> list)
    {
        if (Target.name == Name)
            list.Add(Target);

        for (int i = 0; i < Target.childCount; i++)
        {
            GetChildsWithName(Name, Target.GetChild(i), ref list);
        }

        return;
    }
	static public void RemoveAllChild(Transform Target, bool recursion)
	{
		while (Target.childCount > 0) 
		{
			Transform obj = Target.GetChild(0);
			if(recursion)RemoveAllChild(obj, true);
			GameObject.Destroy(obj.gameObject);
			obj.parent = null;
		}
	}

	static public void InitTransform(Transform obj)
	{
		obj.localPosition = Vector3.zero;
		//obj.position = Vector3.zero;
		obj.localScale = Vector3.one;
		obj.Rotate(Vector3.zero);
	}
    /// <summary>
    /// 设置子节点active
    /// </summary>
    /// <param name="Target">父节点</param>
    /// <param name="active"></param>
    /// <param name="recursion">是否递归</param>
    static public void SetActiveAllChild(Transform Target, bool active, bool recursion)
    {
        for (int i = 0; i < Target.childCount; i++)
        {
            Transform obj = Target.GetChild(i);
            if (recursion) SetActiveAllChild(obj, active, recursion);
            obj.gameObject.SetActive(active);
        }
    }

    static public void SetLayer(GameObject go, int layer)
    {
        if (layer <= 31)
        {
            go.layer = layer;
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                SetLayer(go.transform.GetChild(i).gameObject, layer);
            }
        }
    }

    static public void SetName(GameObject go, string name, bool recursion)
    {
        go.name = name;
        if (recursion)
        {
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                SetName(go.transform.GetChild(i).gameObject, name, recursion);
            }
        }
    }
}

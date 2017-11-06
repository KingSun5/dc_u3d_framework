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
        if (string.IsNullOrEmpty(file))
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
        if (Target == null) return null;

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
        if (Target == null) return;

        if (Target.name == Name)
            list.Add(Target);

        for (int i = 0; i < Target.childCount; i++)
        {
            GetChildsWithName(Name, Target.GetChild(i), ref list);
        }

        return;
    }

    /// <summary>
    /// 删除子节点
    /// </summary>
    /// <param name="Target">父节点</param>
    /// <param name="recursion">是否递归</param>
	static public void RemoveAllChild(Transform Target, bool recursion)
    {
        if (Target == null) return;
		while (Target.childCount > 0) 
		{
			Transform obj = Target.GetChild(0);
			if(recursion)RemoveAllChild(obj, true);
			GameObject.Destroy(obj.gameObject);
            obj.SetParent(null);
		}
	}

    /// <summary>
    /// 初始化方位信息
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="is_world"></param>
	static public void InitTransform(Transform obj, bool is_world = false)
    {
        if (obj == null) return;
        if(!is_world)
		    obj.localPosition = Vector3.zero;
        else
            obj.position = Vector3.zero;
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
        if (Target == null) return;
        for (int i = 0; i < Target.childCount; i++)
        {
            Transform obj = Target.GetChild(i);
            if (recursion) SetActiveAllChild(obj, active, recursion);
            obj.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// 设置对象层级
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layer"></param>
    static public void SetLayer(GameObject go, int layer)
    {
        if (go == null) return;
        if (layer <= 31)
        {
            go.layer = layer;
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                SetLayer(go.transform.GetChild(i).gameObject, layer);
            }
        }
    }

    /// <summary>
    /// 设置节点名
    /// </summary>
    /// <param name="go"></param>
    /// <param name="name"></param>
    /// <param name="recursion">是否影响子节点</param>
    static public void SetName(GameObject go, string name, bool recursion)
    {
        if (go == null) return;
        go.name = name;
        if (recursion)
        {
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                SetName(go.transform.GetChild(i).gameObject, name, recursion);
            }
        }
    }

    /// <summary>
    /// 判断节点是否激活
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static public bool IsEnable(GameObject obj)
    {
        if (!obj) return false;
        if (!obj.activeSelf) return false;
        if (obj.transform.parent != null)
            return IsEnable(obj.transform.parent.gameObject);
        return true;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIHelper
{
    public static T GetComponent<T>(GameObject go, bool auto_add = true) where T : Component
    {
        T ret = go.GetComponent<T>();
        if (ret == null && auto_add) ret = go.AddComponent<T>();
        return ret;
    }
    public static T GetComponent<T>(GameObject go, Type classType, bool auto_add = true) where T : Component
    {
        T ret = go.GetComponent(classType) as T;
        if (ret == null && auto_add) ret = go.AddComponent(classType) as T;
        return ret;
    }
    public static void RemoveAllChild(Transform Target)
    {
        while (Target.childCount > 0)
        {
            Transform obj = Target.GetChild(0);
            obj.SetParent(null);
            GameObject.Destroy(obj.gameObject);
        }
    }
}
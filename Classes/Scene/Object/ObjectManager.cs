using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// 对象管理
/// @author hannibal
/// @time 2014-11-19
/// </summary>
public class ObjectManager : Singleton<ObjectManager>
{
    private static uint m_ShareObjID = 0;

    /**对象集合*/
    private Dictionary<ulong, BaseObject>    m_DicObject;
    private Dictionary<ulong, BaseObject>    m_DicServerObject;

    /**需要释放的对象*/
    private LinkedList<BaseObject>          m_ListReleaseObject;

    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～基础方法～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    public ObjectManager()
    {
        m_DicObject = new Dictionary<ulong, BaseObject>();
        m_DicServerObject = new Dictionary<ulong, BaseObject>();
        m_ListReleaseObject = new LinkedList<BaseObject>();
    }
    public void Setup()
    {
    }
    public void Destroy()
    {
        ReleaseAllObject();
    }

    public void Tick(float elapse, int game_frame)
    {
        if (m_DicObject.Count > 0)
        {
            var list_values = new List<BaseObject>(m_DicObject.Values);
            foreach (var obj in list_values)
            {
                if (obj.Active && obj.Tick(elapse, game_frame))
                {
                }
                else
                {
                    m_ListReleaseObject.AddLast(obj);
                }
            }
        }

        ProcessReleaseObject();
    }
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～对象集合～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <typeparam name="T">脚本文件</typeparam>
    /// <param name="file">资源路径</param>
    public BaseObject NewObject<T>(string file, Transform parent = null)
    {
        if (string.IsNullOrEmpty(file)) file = "Prefab/EmptyNode";
        //构建对象
        GameObject obj = GameObjectUtils.BuildObject(file);
        if (obj == null) return null;

        //父节点
        if (parent == null)
            SceneLayerUtils.AddChild(obj.transform);
        else
            obj.transform.SetParent(parent, false);

        //脚本
        BaseObject base_obj = null;
        if (obj.GetComponent(typeof(T)) == null)
            base_obj = obj.AddComponent(typeof(T)) as BaseObject;
        else
            base_obj = obj.GetComponent(typeof(T)) as BaseObject;

        return base_obj;
    }
    /// <summary>
    /// 创建2D对象
    /// </summary>
    public BaseObject New2DObject<T>(string file, float z_depth, Transform parent = null)
    {
        if (string.IsNullOrEmpty(file)) file = "Prefab/EmptyNode";
        //构建对象
        GameObject obj = GameObjectUtils.BuildObject(file);
        if (obj == null) return null;

        //父节点
        SceneLayerUtils.Add2DChild(obj.transform, z_depth);
        if(parent != null)
            obj.transform.SetParent(parent, false);

        //脚本
        BaseObject base_obj = null;
        if (obj.GetComponent(typeof(T)) == null)
            base_obj = obj.AddComponent(typeof(T)) as BaseObject;
        else
            base_obj = obj.GetComponent(typeof(T)) as BaseObject;

        return base_obj;
    }
    /// <summary>
    /// 移除对象
    /// </summary>
    /// <param name="pObj"></param>
    /// <param name="focus">是否立刻删除，否则下一帧 </param>
    public void RemoveObject(BaseObject pObj, bool focus = true)
    {
        if (pObj == null) return;
        if (focus)
        {
            pObj.Active = false;
            ReleaseObject(pObj);
        }
        else
        {
            pObj.Active = false;
        }
    }
    /// <summary>
    /// 释放对象
    /// </summary>
    private void ReleaseObject(BaseObject pObj)
    {
        if (pObj == null) return;

        DetachObject(pObj);
        pObj.Destroy();
        if (pObj.gameObject != null)
        {
            GameObject.Destroy(pObj.gameObject);
        }
    }
    /// <summary>
    /// 释放所有对象
    /// </summary>
    public void ReleaseAllObject()
    {
        var list_values = new List<BaseObject>(m_DicObject.Values);
        for (int i = list_values.Count - 1; i >= 0; --i)
        {
            ReleaseObject(list_values[i]);
        }

        m_DicObject.Clear();
        m_DicServerObject.Clear();
        m_ListReleaseObject.Clear();
    }
    /// <summary>
    /// 加入对象管理器
    /// </summary>
    public void AttachObject(BaseObject pObj)
    {
        m_DicObject[pObj.ObjectUID] = pObj;

        if (pObj.ObjectServerID > 0)
        {
            m_DicServerObject[pObj.ObjectServerID] = pObj;
        }
    }
    public void DetachObject(BaseObject pObj)
    {
        if (m_DicObject.ContainsKey(pObj.ObjectUID))
        {
            m_DicObject.Remove(pObj.ObjectUID);
        }
        if (pObj.ObjectServerID > 0 && m_DicServerObject.ContainsKey(pObj.ObjectServerID))
        {
            m_DicServerObject.Remove(pObj.ObjectServerID);
        }
    }
    /// <summary>
    /// 每帧结束前调用，释放当期帧需要删除的对象，并回收对象
    /// </summary>
    private void ProcessReleaseObject()
    {
        if (m_ListReleaseObject.Count == 0)
            return;

        foreach (BaseObject obj in m_ListReleaseObject)
        {
            ReleaseObject(obj);
        }

        m_ListReleaseObject.Clear();
    }

    public BaseObject GetObjectByID(ulong id)
    {
        BaseObject obj;
        if (m_DicObject.TryGetValue(id, out obj))
        {
            return obj;
        }
        return null;
    }
    /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～get/set～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
    static public uint ShareGUID()
    {
        return ++m_ShareObjID;
    }

    static public string AppendNameByID(string name, ulong id)
    {
        return name + "ID(" + id.ToString() + ")";
    }
    static public ulong GetIDByObjectName(string name)
    {
        string id = StringUtils.Search_string(name, "ID(", ")");
        if (id.Length <= 0)return 0;

        return System.Convert.ToUInt64(id);
    }
    public Dictionary<ulong, BaseObject> DicObject
    {
        get { return m_DicObject; }
    }
}

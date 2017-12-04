using UnityEngine;
using System.Collections;

/// <summary>
/// 场景层
/// @author hannibal
/// @time 2014-12-13
/// </summary>
public class SceneLayerUtils
{
    static private Transform m_RootLayer;

    static public void Setup(Transform rootLayer)
    {
        m_RootLayer = rootLayer;
    }

    static public void Destroy()
    {
        if (m_RootLayer != null)
        {
            m_RootLayer = null;
        }
    }

    static public void AddChild(Transform obj, bool worldPosStays=false)
    {
        if (obj != null)
        {
            obj.SetParent(m_RootLayer, worldPosStays);
        }
    }
    static public void Add2DChild(Transform obj, float z_depth)
    {
        if (obj != null)
        {
            obj.SetParent(m_RootLayer, false);
            obj.localPosition = new Vector3(obj.localPosition.x, obj.localPosition.y, z_depth);
        }
    }
    static public void SetLayer(Transform obj, float layer)
    {
        obj.localPosition = new Vector3(obj.localPosition.x, obj.localPosition.y, layer);
    }

    static public void ClearAllChild()
    {
        for (int i = 0; i < m_RootLayer.childCount; ++i)
        {
            Transform obj = m_RootLayer.GetChild(i);
            if (obj.gameObject != null)
            {
                RemoveGameObject(obj.gameObject, true);
            }
        }
    }
    static public void RemoveGameObject(GameObject root, bool isChild)
    {
        if (root == null) return;

        if (isChild)
        {
            for (int i = 0; i < root.transform.childCount; ++i)
            {
                Transform obj = root.transform.GetChild(i);
                if (obj.gameObject != null)
                {
                    RemoveGameObject(obj.gameObject, isChild);
                }
            }
        }

        GameObject.Destroy(root);
    }

    static public Transform RootLayer
    {
        get
        {
            return m_RootLayer;
        }
    }
}

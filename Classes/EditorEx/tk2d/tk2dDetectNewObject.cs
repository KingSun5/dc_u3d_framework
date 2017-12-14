using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 检测是否有新对象加入子节点
/// @author hannibal
/// @time 2017-11-8
/// </summary>
public class tk2dDetectNewObject : MonoBehaviour 
{
    /// <summary>
    /// 当游戏对象的变换的子元素列表已经改变时，该函数被调用。
    /// </summary>
    void OnTransformChildrenChanged()
    {
        Transform cur_child = null;
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = this.transform.GetChild(i);
            if (cur_child == null)
            {
                cur_child = child;
                tk2dAnimationView view = cur_child.GetComponentInChildren<tk2dAnimationView>();
                if (view == null)
                    cur_child.gameObject.AddComponent<tk2dAnimationView>();
            }
            else
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
    /// <summary>
    /// 当GameObject的变换的父属性发生变化时，将调用此函数。
    /// </summary>
    void OnTransformParentChanged()
    {

    }
}

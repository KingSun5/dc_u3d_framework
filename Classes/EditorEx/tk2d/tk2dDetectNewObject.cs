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
	void Start () 
    {
		
	}
	
	void Update () 
    {
        Transform cur_child = null;
        for (int i = this.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = this.transform.GetChild(i);
            if (cur_child == null)
            {
                cur_child = child;
                tk2dAnimationView view = cur_child.GetComponent<tk2dAnimationView>();
                if(view == null)
                    cur_child.gameObject.AddComponent<tk2dAnimationView>();
            }
            else
            {
                GameObject.Destroy(child.gameObject);
            }
        }
	}
}

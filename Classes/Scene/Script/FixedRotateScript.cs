using UnityEngine;
using System.Collections;

/// <summary>
/// 固定obj旋转：相对于父节点
/// @author hannibal
/// @time 2016-9-23
/// </summary>
public class FixedRotateScript : MonoBehaviour 
{
    public Transform m_RootParent;

    private Quaternion m_InitRotate;
    private Vector3 m_InitPosition;

	void Awake () 
    {
        m_InitPosition = gameObject.transform.localPosition;
        m_InitRotate = gameObject.transform.localRotation;
	}
	
	// Update is called once per frame
	void LateUpdate () 
    {
        gameObject.transform.forward = m_RootParent.forward;
        gameObject.transform.position = gameObject.transform.parent.position + m_InitPosition;
	}
}

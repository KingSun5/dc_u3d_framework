using UnityEngine;
using System.Collections;

/// <summary>
/// 固定obj旋转
/// @author hannibal
/// @time 2016-9-23
/// </summary>
public class FixedRotateScript : MonoBehaviour 
{
    public bool m_FixedX;
    public bool m_FixedY;
    public bool m_FixedZ;

    private Vector3 m_InitRotate;

	void Awake () 
    {
        m_InitRotate = gameObject.transform.eulerAngles;
	}
	
	// Update is called once per frame
	void LateUpdate () 
    {
        Vector3 rotate = gameObject.transform.eulerAngles;
        if (m_FixedX) rotate.x = m_InitRotate.x;
        if (m_FixedY) rotate.y = m_InitRotate.y;
        if (m_FixedZ) rotate.z = m_InitRotate.z;

        gameObject.transform.rotation = Quaternion.Euler(rotate);
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// 漫游场景
/// @author hannibal
/// @time 2016-9-23
/// </summary>
public class FreeCameraScript : MonoBehaviour
{
    public float m_MoveStep = 1;
    public float m_RotateStep = 0.1f;

	void Start () 
    {
	
	}

    private Vector3 tmpLastMousePos = Vector3.zero;
    private bool tmpIsDrag = false;
	void Update () 
    {
        ///1.移动
        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            x = -m_MoveStep;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            x = m_MoveStep;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            y = m_MoveStep;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            y = -m_MoveStep;
        }

        if(x != 0 || y != 0)
        {
            Vector3 direction = new Vector3(x, 0, y);
            direction = transform.TransformDirection(direction);
            transform.Translate(x, 0, y);
        }

        ///2.旋转
        if (Input.GetButtonDown("Fire1"))
        {
            tmpLastMousePos = Input.mousePosition;
            tmpIsDrag = true;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            tmpIsDrag = false;
        }
        if (tmpIsDrag && Input.GetButton("Fire1"))
        {
            if (Input.mousePosition != tmpLastMousePos)
            {
                Vector3 offset = Input.mousePosition - tmpLastMousePos;
                //transform.rotation *= Quaternion.AngleAxis(offset.x * m_RotateStep, Vector3.up);
                transform.rotation *= Quaternion.AngleAxis(-offset.y * m_RotateStep, transform.right);

                tmpLastMousePos = Input.mousePosition;
            }
        }
	}
}

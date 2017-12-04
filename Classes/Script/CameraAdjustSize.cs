using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]

/// <summary>
/// 自动调整2dcamera size
/// @author hannibal
/// @time 2014-12-4
/// </summary>
public class CameraAdjustSize : MonoBehaviour 
{
    public float m_Scale = 1.0f;
	void Start ()
    {
        GameObject canvas_obj = GameObject.Find("UICanvas") as GameObject;
        if (canvas_obj == null) return;
        Canvas canvas = canvas_obj.GetComponent<Canvas>();
        if (canvas == null) return;
        float size = canvas.pixelRect.size.y / 100 / 2;
        this.GetComponent<Camera>().orthographicSize = size * m_Scale;
	}
}

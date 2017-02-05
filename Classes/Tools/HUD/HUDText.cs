using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDText : MonoBehaviour
{
    public Text m_Text = null;
    public RectTransform Rect;
    public int m_Type;
    public float m_ElapsedTime = 0;

    [SerializeField, Tooltip("是否alpha渐变")]
    public bool m_EnableAlpha = true;
    [SerializeField, Tooltip("渐变速度")]
    public float m_FadeSpeed;
    [SerializeField, Tooltip("开始渐变时间")]
    public float m_FadeStartTime;

    [SerializeField, Tooltip("时长，如果设置了")]
    public float m_TotalTime = 0;
    [HideInInspector, Tooltip("移除时间")]
    public float m_RemoveTime = 0;

    [SerializeField]
    public float m_Speed = 1;
    [SerializeField]
    public float m_Yquickness = 1;
    [SerializeField]
    public float m_YquicknessScaleFactor = 0;

   [HideInInspector] public eHUDGuidance movement;
   [HideInInspector] public float Xcountervail;
   [HideInInspector] public float Ycountervail;
   [HideInInspector] public string text;
   [HideInInspector] public Vector3 InitPos;
}
using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(LineRenderer))]

/// <summary>
/// 绘制贝塞尔曲线
/// @author hannibal
/// @time 2017-2-8
/// </summary>
public class BezierScript : MonoBehaviour
{
    public Transform[] controlPoints;
    private LineRenderer lineRenderer;

    public int layerOrder = 0;
    public int segmentNum = 50;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.sortingLayerID = layerOrder;
    }

    void Update()
    {
        DrawCurve();
    }

    void DrawCurve()
    {
        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            int numSections = controlPoints.Length - 2;
            int nodeIndex = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            Vector3 pixel = CalculateCubicBezierPoint(t, controlPoints[nodeIndex].position,
                controlPoints[nodeIndex + 1].position, controlPoints[nodeIndex + 2].position);
            lineRenderer.SetVertexCount(i);
            lineRenderer.SetPosition(i - 1, pixel);
        }

    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}
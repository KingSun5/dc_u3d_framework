using UnityEngine;
using System.Collections;

/// <summary>
/// 放置指定位置
/// @author hannibal
/// @time 2016-2-14
/// </summary>
public class PlaceTransformer : SpeedTransformer 
{
    Vector3 m_position;

    public static PlaceTransformer visible(GameObject target, Vector3 pos)
    {
        PlaceTransformer transformer = new PlaceTransformer();
        transformer.m_position = pos;
        return transformer;
    }
    public PlaceTransformer()
    {
        m_Type = eTransformerID.Place;
    }
    public override void OnTransformStarted()
    {
        m_Target.transform.localPosition = m_position;
    }
}

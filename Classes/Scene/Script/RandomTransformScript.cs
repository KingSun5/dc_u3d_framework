using UnityEngine;
using System.Collections;

/// <summary>
/// 随机位置，缩放，旋转
/// @author hannibal
/// @time 2016-4-14
/// </summary>
public class RandomTransformScript : MonoBehaviour 
{
    private Vector3 defaultScale;
    private Vector3 defaultPosition;        

    public bool RandomPosition, RandomScale, RandomRotation;

    public float MinPositionX, MaxPositionX;
    public float MinPositionY, MaxPositionY;
    public float MinPositionZ, MaxPositionZ;    
    public float MinScale, MaxScale;            
    public float MinRotation, MaxRotaion;      

    void Awake()
    {
        defaultScale = transform.localScale;
        defaultPosition = transform.localPosition;
    }

    void OnEnable()
    {
        if (RandomPosition)
            transform.localPosition = defaultPosition + new Vector3(Random.Range(MinPositionX, MaxPositionX), Random.Range(MinPositionY, MaxPositionY), Random.Range(MinPositionZ, MaxPositionZ));

        if (RandomScale)
            transform.localScale = defaultScale * Random.Range(MinScale, MaxScale);

        if (RandomRotation)
            transform.rotation *= Quaternion.Euler(0, 0, Random.Range(MinRotation, MaxRotaion));
    }
}

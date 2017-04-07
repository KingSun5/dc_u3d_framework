using UnityEngine;
using System.Collections;

/// <summary>
/// 数学
/// @author hannibal
/// @time 2014-11-1
/// </summary>
public class MathUtils
{
	/**字节转换M*/
	static public float BYTE_TO_M = 1.0f/(1024*1024);
	/**字节转换K*/
	static public float BYTE_TO_K = 1.0f/(1024);

	static public Vector3 MAX_VECTOR3 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
	static public Vector3 MIN_VECTOR3 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
	static public Vector3 TMP_VECTOR3 = new Vector3(0f,0f,0f);
	static public Vector2 TMP_VECTOR2 = new Vector2(0f,0f);

	public static float Min(float first, float second)
	{
		return (first < second ? first : second);
	}
	public static float Max(float first, float second)
	{
		return (first > second ? first : second);
	}
	/**
	 * 获得一个数的符号(-1,0,1)
	 * 结果：大于0：1，小于0：-1，等于0:0
	 */		
	public static int Sign(float value)
	{
		return (value < 0 ? -1 : (value > 0 ? 1 : 0));
	}

	/**
	 * 产生随机数
	 * 结果：x>=param1 && x<param2
	 */		
	public static float RandRange(float param1, float param2) 
	{
		float loc = Random.Range(param1, param2);
		return loc;
	}
	/**
	 * 产生随机数
	 * 结果：x>=param1 && x<param2
	 */	
	public static int RandRange_Int(int param1, int param2) 
	{
		int loc = param1 + (int)(Random.Range(0f, 1f)*(param2-param1));
		return loc;
	}
	/**
	 * 从数组中产生随机数[-1,1,2]
	 * 结果：-1/1/2中的一个
	 */	
	public static T RandRange_Array<T>(T[] arr) 
	{
		T loc = arr[RandRange_Int(0, arr.Length)];
		return loc;
	}
	/**
	 * 随机1/-1
	 * 结果：1/-1
	 */	
	public static int Rand_Sign() 
	{
		int[] arr = new int[2]{-1, 1};
		int loc = RandRange_Array<int>(arr);
		return loc;
	}
    /// <summary>
    /// 产生-factor到factor的随机向量
    /// </summary>
    /// <param name="factor">长度</param>
    /// <returns></returns>
    public static Vector3 Rand_Vector3(float length)
    {
        float x = (1 - 2 * Random.value) * length;
        float y = (1 - 2 * Random.value) * length;
        float z = (1 - 2 * Random.value) * length;
        return new Vector3(x, y, z);
    }
	/// <summary>
    /// 限制范围
	/// </summary>
	public static float Clamp(float n, float min, float max)
	{
		if(min > max)
		{
			var i = min;
			min = max;
			max = i;
		}
		
		return (n < min ? min : (n > max ? max : n));
	}
	/**
	 * 把一个数转换到0-360之间
	 * @param num 需要转换的数
	 * @return 转换后的数
	 */
	public static float Cleap0_360(float num)
	{
		num = num % 360;
		num = num < 0 ? num + 360 : num;
		
		return num;
	}

	/**
	 * 测试 flag中是否包含testFlag状态:
	 * @param flag
	 * @param testFlag
	 * @return true-包含；false-不包含。比如testFlag(3, 1)返回true
	 */		
	public static bool TestFlag(uint flag, uint testFlag)
	{
		return ((uint)(flag&testFlag) != 0 ? true : false);
	}
    /// <summary>
    /// 合并两个bounds
    /// </summary>
	public static Bounds UnionBounds(Bounds first, Bounds sec)
	{
		Vector3 vec = new Vector3();
		Bounds result = new Bounds();
		vec.x = MathUtils.Min(first.min.x, sec.min.x);
		vec.y = MathUtils.Min(first.min.y, sec.min.y);
		vec.z = MathUtils.Min(first.min.z, sec.min.z);
		result.min = vec;
		vec.x = MathUtils.Max(first.max.x, sec.max.x);
        vec.y = MathUtils.Max(first.max.y, sec.max.y);
        vec.z = MathUtils.Max(first.max.z, sec.max.z);
		result.max = vec;
		return result;
	}
    /// <summary>
    /// bound1是否包含bound2
    /// </summary>
    /// <param name="bound1"></param>
    /// <param name="bound2"></param>
    /// <returns></returns>
    public static bool ContainerBounds(Bounds bound1, Bounds bound2)
    {
        return true;
    }
	/**
	 * 弧度转化为度 
	 */		
	public static float ToDegree(float radian)
	{
		return radian * (180.0f/3.1415926f);
	}
	/// <summary>
    /// 度转化为弧度 
	/// </summary>
	public static float ToRadian(float degree)
	{
		return degree * (3.1415926f/180.0f);
	}
	/// <summary>
    /// 投影到平面
	/// </summary>
	Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
	{
		return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
	}
	
	public static float HorizontalAngle (Vector3 direction) 
	{
		return Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
	}
    /// <summary>
    /// 点到直线距离
    /// </summary>
    /// <param name="point">点坐标</param>
    /// <param name="linePoint1">直线上一个点的坐标</param>
    /// <param name="linePoint2">直线上另一个点的坐标</param>
    /// <returns></returns>
    public static float DisPoint2Line(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    {
        Vector3 vec1 = point - linePoint1;
        Vector3 vec2 = linePoint2 - linePoint1;
        Vector3 vecProj = Vector3.Project(vec1, vec2);
        float dis = Mathf.Sqrt(Mathf.Pow(Vector3.Magnitude(vec1), 2) - Mathf.Pow(Vector3.Magnitude(vecProj), 2));
        return dis;
    }
    /// <summary>
    /// 判断一个点是否在相机视锥体
    /// </summary>
    /// <param name="pt"></param>
    /// <returns></returns>
    public static bool PtInFrustum(Camera camera, Vector3 pos)
    {
        Vector3 pt = camera.WorldToScreenPoint(pos);
        if(pt.x < 0 || pt.x > Screen.width || pt.y < 0 || pt.y > Screen.height || pt.z <= 0)
            return false;
        return true;
    }
    /// <summary>
    /// 转换为水平方向
    /// </summary>
    public static Vector3 ToHorizontal(Vector3 vec)
    {
        vec.y = 0;
        return vec;
    }
    /// <summary>
    /// 水平距离
    /// </summary>
    public static float HorizontalDistance(Vector3 vec1, Vector3 vec2)
    {
        vec1.y = 0;
        vec2.y = 0;
        return (vec1 - vec2).magnitude;
    }
    /// <summary>
    /// 两点垂直夹角
    /// </summary>
    public static float VerticalAngle(Vector3 pt1, Vector3 pt2)
    {
        Vector3 pt_top, pt_bottom;
        if (pt1.y > pt2.y)
        { pt_top = pt1; pt_bottom = pt2; }
        else
        { pt_top = pt2; pt_bottom = pt1; }
        Vector3 pt = new Vector3(pt_top.x, pt_bottom.y, pt_top.z);
        return Vector3.Angle((pt_bottom - pt_top), (pt - pt_top));
    }
}

using UnityEngine;
using System.Collections;

/// <summary>
/// 数学-2D
/// @author hannibal
/// @time 2014-12-2
/// </summary>
public class Math2DUtils
{
	/**
	 * 距离平方值
	 */		
	public static float distance_square(float x1, float y1, float x2, float y2)
	{
		return (x1-x2)*(x1-x2) + (y1-y2)*(y1-y2);
	}
	/**
	 * 距离
	 */		
	public static float distance(float x1, float y1, float x2, float y2)
	{
		return Mathf.Sqrt((x1-x2)*(x1-x2) + (y1-y2)*(y1-y2));
	}
	public static float distance(float x, float y)
	{
		return Mathf.Sqrt(x*x + y*y);
	}
    /**
     * 两点间的弧度
            ^(y)
            |(90)
            |
            |
(180)-------|--------->(x)
            |		  (0)
            |
            |
            |(270)
     */
    public static float LineRadians(float x1, float y1, float x2, float y2)
    {
        return Mathf.Atan2(y2 - y1, x2 - x1);
    }
    public static float Point2Radians(float x, float y)
    {
        return Mathf.Atan2(y, x);
    }
	public static Vector2 Radians2Point(float radian)
	{
        Vector2 pt = new Vector2();
        pt.x = Mathf.Cos(radian);
        pt.y = Mathf.Sin(radian);
        return pt;
	}
	/**
	 * 根据度数获得朝向
	 * X轴正方向为1，顺时钟方向为加 
	 * angle 度数(0-360)
	 */		
	public static uint getFace(float angle, uint chunkNums)
	{
		float perAngle = 360/chunkNums;
		uint nFace = (uint)((MathUtils.Cleap0_360(angle)+perAngle*0.5f)/perAngle)+1;//从1开始
		nFace = nFace > chunkNums ? nFace-chunkNums : nFace;
		return nFace;
	}

	public static float getAngleByFace8(int face)
	{
		float angle = 0;
		if(face >= 0 && face <= 8)
			angle = (face-1)*45;
		return angle;
	}
	/**
	 * 左边还是右边
	 * @param num 度数
	 * @return 0~89或271~360=1，90或270=0,91~269=-1
	 * 
	 */	
	public static int getLeftOrRightFace(int num)
	{
		num = (int)MathUtils.Cleap0_360((float)num);
		if(num >= 0 && num <= 90 || num >= 270 && num <= 360)
		{
			return 1;
		}
		
		return -1;
	}
	/**
	 * 获取左中右
	 * @param i
	 * @return 
	 */		
	static public int getSide(float i)
	{
		return i > 0 ? 1 : (i == 0 ? 0 : -1);
	}
	/**
	* 获得象限：x右方向为0，顺时针转
	* @param fDegree - 0～360度
	* @param chunkNums - 划分几份
	* @return 
	*/
	static public uint GetQuadrant(float fDegree, uint chunkNums)
	{
		if (chunkNums == 0)
		{
			return 0;
		}
		float perAngle = 360.0f/chunkNums;
		uint nFace = (uint)((fDegree+perAngle*0.5f+90)/perAngle)+1;//从1开始
		nFace = nFace > chunkNums ? nFace-chunkNums : nFace;
		return nFace;
	}
	/**
	 * 获得所处区间 
	 * @param cur 当前值
	 * @param total 总值
	 * @param section_count 区间总数
	 * @return 1开头的区间值
	 */		
	public static int getSection(float cur, float total, int section_count)
	{
		float perAngle = total/section_count;
		int sec = (int)(cur/perAngle+1);//从1开始
		sec = (int)MathUtils.Clamp(sec, 1, section_count);
		return sec;
	}
	/**
	 * 单位化
	 */		
	public static Vector2 normalPoint(float x1, float y1, float x2, float y2, float power = 1)
	{
		Vector2 pt = new Vector2(x2-x1, y2-y1);
		pt.Normalize();
		pt *= power;
		return pt;
	}

	/**
	 * 两个矩形是否相交 
	 * @param rect1
	 * @param rect2
	 * @return 
	 * 
	 */		
	public static bool intersectRect(Rect rect1,Rect rect2)
	{
		if ((rect1.xMax > rect2.xMin) &&
            (rect1.xMin < rect2.xMax) &&
		    (rect1.yMax > rect2.yMin) &&
            (rect1.yMin < rect2.yMax) ||
            (rect2.xMax > rect1.xMin) &&
            (rect2.xMin < rect1.xMax) &&
            (rect2.yMax > rect1.yMin) &&
            (rect2.yMin < rect1.yMax))
		{
			return true;
		}
		return false;
	}

    /// <summary>
    /// 两个向量夹角
    /// </summary>
    public static float AngleBetween(Vector2 vector1, Vector2 vector2)
    {
        float _sin = vector1.x * vector2.y - vector2.x * vector1.y;
        float _cos = vector1.x * vector2.x + vector1.y * vector2.y;

        float fATan = Mathf.Atan2(_sin, _cos);

        // [0~360]
        if (fATan > 2 * Mathf.PI)
            fATan %= 2 * Mathf.PI;
        else if (fATan < 0)
            fATan += 2 * Mathf.PI;
        return 2 * Mathf.PI - fATan;
    }
}

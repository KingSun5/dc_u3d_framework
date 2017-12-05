using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

/// <summary>
/// 地图格画线
/// @author hannibal
/// @time 2017-12-1
/// </summary>
public class EDrawSpriteGrid : MonoBehaviour
{
    public Vector3 m_MapCenter = Vector3.zero;
    public int m_GridWidth = 30;
    public int m_GridHeight = 30;

    /// <summary>
    /// 网格线是否显示
    /// </summary>
    public bool m_ShowGrid = true;
    /// <summary>
    /// 网格线宽度
    /// </summary>
    public float m_LineWidth = 0.01f;
    /// <summary>
    /// 网格线颜色
    /// </summary>
    public Color m_LineColor = Color.red;

    /// <summary>
    /// 贴图
    /// </summary>
    private Sprite m_Sprite = null;
    private bool m_HadBuilder = false;
    /// <summary>
    /// 行列数
    /// </summary>
    private int m_GridRows = 0;
    private int m_GridCols = 0;
    /// <summary>
    /// 网格线
    /// </summary>
    private GameObject[,] m_lines;

    void Awake()
    {
        m_Sprite = this.GetComponent<SpriteRenderer>().sprite;
        m_GridRows = (int)Mathf.Ceil(m_Sprite.rect.height / m_GridHeight);
        m_GridCols = (int)Mathf.Ceil(m_Sprite.rect.width / m_GridWidth);
        m_lines = new GameObject[m_GridRows+1, m_GridCols+1];
    }

	void Update () 
    {
		if(m_ShowGrid)
        {
            if(!m_HadBuilder)
            {
                this.Builder();
                m_HadBuilder = true;
            }
        }
        else
        {
            if(m_HadBuilder)
            {
                this.Clear();
                m_HadBuilder = false;
            }
        }
	}

    void OnDestroy()
    {
        this.Clear();
    }

    void Builder()
    {
        float width = m_Sprite.bounds.size.x;
        float height = m_Sprite.bounds.size.y;
        float cell_width = width / m_GridCols;
        float cell_height = height / m_GridRows;
        float z_depth = this.transform.position.z - 0.01f;
        //行
        for (int row = 0; row <= m_GridRows; ++row)
        {
            Vector3 start_pos = Vector3.zero, end_pos = Vector3.zero;
            start_pos.x = m_Sprite.bounds.min.x;
            start_pos.y = m_Sprite.bounds.min.y + row * cell_height;
            start_pos.z = z_depth;
            end_pos.x = m_Sprite.bounds.min.x + (cell_width * m_GridCols);
            end_pos.y = m_Sprite.bounds.min.y + row * cell_height;
            end_pos.z = z_depth;
            Vector3[] pos = { (start_pos + m_MapCenter), (end_pos + m_MapCenter) };
            this.CreatLine(row, 0, pos);
        }
        //列
        for (int col = 0; col < m_GridCols; ++col)
        {
            Vector3 start_pos = Vector3.zero, end_pos = Vector3.zero;
            start_pos.x = m_Sprite.bounds.min.x + col * cell_width;
            start_pos.y = m_Sprite.bounds.min.y;
            start_pos.z = z_depth;
            end_pos.x = m_Sprite.bounds.min.x + col * cell_width;
            end_pos.y = m_Sprite.bounds.min.y + (cell_height * m_GridRows);
            end_pos.z = z_depth;
            Vector3[] pos = { (start_pos + m_MapCenter), (end_pos + m_MapCenter) };
            this.CreatLine(0, col, pos);
        }
    }

    void Clear()
    {
        foreach (var obj in m_lines)
        {
            if (obj != null)
                GameObject.Destroy(obj);
        }
        m_HadBuilder = false;
    }

    /// <summary>
    /// 创建网格线
    /// </summary>
    void CreatLine(int row, int col, Vector3[] pos)
    {
        //创建对象
        if(this.m_lines[row, col] != null)
        {
            GameObject.Destroy(this.m_lines[row, col]);
        }
        GameObject obj = new GameObject();
        obj.transform.SetParent(this.transform, false);
        this.m_lines[row, col] = obj;

        //设置line属性
        LineRenderer _lineRenderer = obj.AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = this.m_LineColor;
        _lineRenderer.endColor = this.m_LineColor;
        _lineRenderer.startWidth = this.m_LineWidth;
        _lineRenderer.endWidth = this.m_LineWidth;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = pos.Length;
        for (int i = 0; i < pos.Length; ++i)
        {
            _lineRenderer.SetPosition(i, pos[i]);
        }

        obj.name = "CreateLine " + row + "  " + col;
    }
}

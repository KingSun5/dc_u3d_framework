using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(GridLayoutGroup))]

/// <summary>
/// 数字排列组件：需要配合GridLayoutGroup一块使用
/// @author hannibal
/// @time 2015-1-6
/// </summary>
public class UIImageNumber : UIComponentBase
{
    public string m_RootPathName = "";

    private int m_NumValue = int.MaxValue;
    private List<GameObject> m_NumImage = new List<GameObject>();

    public override void OnEnable()
	{
        if (m_NumValue != int.MaxValue) SetData(m_NumValue);
        base.OnEnable();
	}
    public override void OnDisable()
	{
        Clear();
        m_NumImage.Clear();
        base.OnDisable();
	}

	public void SetData(int num)
	{
		m_NumValue = num;
		Clear();
        string arr = m_NumValue.ToString();
		for(int i = 0; i < arr.Length; ++i)
		{
            if (m_NumImage.Count <= i )
            {
                GameObject obj = new GameObject();
                obj.AddComponent<Image>();
                m_NumImage.Add(obj);
            }
            Image image = m_NumImage[i].GetComponent<Image>();
            image.transform.SetParent(transform);
            image.transform.localScale = Vector3.one;
            image.gameObject.SetActive(true);
            if (image.sprite != null) SpritePools.Despawn(image.sprite);
            image.sprite = SpritePools.Spawn(m_RootPathName + arr[i]);
		}
	}

	private void Clear()
	{
        for (int i = 0; i < m_NumImage.Count; i++)
        {
            if (m_NumImage[i] == null)
                continue;
            Image image = m_NumImage[i].GetComponent<Image>();
            if (image != null && image.sprite != null) SpritePools.Despawn(image.sprite);
            m_NumImage[i].gameObject.SetActive(false);
        }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 修改UI元素渲染层级类
/// 使用方法：直接在UI元素中加入，调用SetOrder即可。注：SetOrder会把子节点所有元素都设为该层级
/// 1.界面基类UIViewBase里都有个m_UIMaxSortingOrder，
///   每次使用这个类修改了层级，都要重新设置m_UIMaxSortingOrder
///   这样做的原因：保证每个UI里的特效都不会越界，渗透到其他UI界面
/// 2.RefreshRender可以重新把子阶段所有元素设置为order层级
/// 3.该类会使代码量增加，但比起直接用rendererTexture特效，要降低消耗
/// 4.该类暂时不支持scrollview滚动时隐藏特效，如需该功能，配合修改shader即可。
/// @author Qc_Chao
/// @time 2016-12-30
/// </summary>
public class UIDepth : MonoBehaviour
{
    public int order;
    public bool isUI = true;
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;

        GraphicRaycaster cast = gameObject.GetComponent<GraphicRaycaster>();
        if (cast == null)
        {
            cast = gameObject.AddComponent<GraphicRaycaster>();
        }

    }

    public void SetOrder(int nOrder)
    {
        order = nOrder;

        RefreshRender();
    }

    public void RefreshRender()
    {
        Renderer[] renders = GetComponentsInChildren<Renderer>();

        foreach (Renderer render in renders)
        {
            render.sortingOrder = order;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ui遮罩，实现两个功能
/// 1.屏蔽下层点击，一般是弹出对话框时，防止下层界面被点中
/// 2.点击空白区域，自动关闭窗口
/// @author hannibal
/// @time 2016-2-5
/// </summary>
[RequireComponent(typeof(Image))]
public class UIMaskScript : MonoBehaviour, IPointerClickHandler 
{
    public bool m_IsAutoClose = false;
    public string m_ParamInfo = "";

    void Awake()
    {
        Image img = this.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 1 / 255.0f);
        img.raycastTarget = true;
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 2, Screen.height * 2);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(m_IsAutoClose)
        {
            UIPanelBase panel = this.GetComponentInParent<UIPanelBase>();
            if (panel != null)
            {//如果是panel
                panel.Close();
            }
            else
            {
                UIViewBase view = this.GetComponentInParent<UIViewBase>();
                if(view != null)
                {//如果是view
                    GameObject.Destroy(view.gameObject);
                }
                else
                {
                    GameObject.Destroy(this.gameObject);
                }
            }
        }
    }
}

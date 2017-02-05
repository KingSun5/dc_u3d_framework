using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HUDTextPools : MonoBehaviour
{
    public static HUDTextPools instance;

    public GameObject[] HUDPrefab;

    private List<HUDText>                       m_list_texts;
    private Dictionary<int, List<GameObject>>   m_text_pools;

    void Awake()
    {
        if (instance != null) Log.Exception("实例已经存在:HUDTextPools");
        instance = this;

        m_list_texts = new List<HUDText>();
        m_text_pools = new Dictionary<int, List<GameObject>>();
    }

    void OnDisable()
    {
        if (m_text_pools == null) return;
        for (int i = 0; i < m_text_pools.Count; i++)
        {
            foreach (var obj in m_text_pools[i])Destroy(obj);
        }
        m_text_pools.Clear();
        for (int i = 0; i < m_list_texts.Count; i++ )
        {
            Destroy(m_list_texts[i].Rect.gameObject);
        }
        m_list_texts.Clear();
    }

    void Update()
    {
        for (int i = 0; i < m_list_texts.Count; i++)
        {
            HUDText cur_text = m_list_texts[i];
            cur_text.m_ElapsedTime += Time.deltaTime;

            if (cur_text.m_EnableAlpha && cur_text.m_ElapsedTime >= cur_text.m_FadeStartTime) cur_text.m_Text.color -= new Color(0f, 0f, 0f, (Time.deltaTime * cur_text.m_FadeSpeed));
            if (cur_text.m_Text.color.a <= 0f || (cur_text.m_TotalTime > 0 && cur_text.m_RemoveTime < Time.realtimeSinceStartup))
            {
                List<GameObject> list;
                if (m_text_pools.TryGetValue(cur_text.m_Type, out list))
                {
                    list.Add(cur_text.Rect.gameObject);
                }
                else
                {
                    list = new List<GameObject>();
                    list.Add(cur_text.Rect.gameObject);
                    m_text_pools.Add(cur_text.m_Type, list);
                }
                cur_text.Rect.gameObject.SetActive(false);
                m_list_texts.Remove(cur_text);
            }
            else
            {
                float mov = Camera.main.WorldToScreenPoint(cur_text.InitPos).y;

                cur_text.m_Yquickness += Time.deltaTime * cur_text.m_YquicknessScaleFactor;
                switch (cur_text.movement)
                {
                    case eHUDGuidance.Up:
                        cur_text.Ycountervail += (((Time.deltaTime * cur_text.m_Speed))) * cur_text.m_Yquickness;
                        break;
                    case eHUDGuidance.Down:
                        cur_text.Ycountervail -= (((Time.deltaTime * cur_text.m_Speed))) * cur_text.m_Yquickness;
                        break;
                    case eHUDGuidance.Left:
                        cur_text.Xcountervail -= ((Time.deltaTime * cur_text.m_Speed));
                        break;
                    case eHUDGuidance.Right:
                        cur_text.Ycountervail += ((Time.deltaTime * cur_text.m_Speed));
                        break;
                    case eHUDGuidance.RightUp:
                        cur_text.Ycountervail += (((Time.deltaTime * cur_text.m_Speed))) * cur_text.m_Yquickness;
                        cur_text.Xcountervail += ((Time.deltaTime * cur_text.m_Speed));
                        break;
                    case eHUDGuidance.RightDown:
                        cur_text.Ycountervail -= (((Time.deltaTime * cur_text.m_Speed))) * cur_text.m_Yquickness;
                        cur_text.Xcountervail += ((Time.deltaTime * cur_text.m_Speed));
                        break;
                    case eHUDGuidance.LeftUp:
                        cur_text.Ycountervail += (((Time.deltaTime * cur_text.m_Speed))) * cur_text.m_Yquickness;
                        cur_text.Xcountervail -= ((Time.deltaTime * cur_text.m_Speed));
                        break;
                    case eHUDGuidance.LeftDown:
                        cur_text.Ycountervail -= (((Time.deltaTime * cur_text.m_Speed))) * cur_text.m_Yquickness;
                        cur_text.Xcountervail -= ((Time.deltaTime * cur_text.m_Speed));
                        break;
                }
                UpdateText(cur_text);
            }
        }
    }

    private void UpdateText(HUDText item)
    {
        Vector2 v = Camera.main.WorldToViewportPoint(item.InitPos);
        Vector2 v2 = new Vector2((v.x) + item.Xcountervail, -(v.y - item.Ycountervail));
        item.Rect.anchorMax = v;
        item.Rect.anchorMin = v;
        item.Rect.anchoredPosition = v2;

        item.m_Text.text = item.text;
    }

    public GameObject NewText(string text, Vector3 pos, eHUDGuidance movement, int type)
    {
        GameObject go = null;
        List<GameObject> list;
        if (m_text_pools.TryGetValue(type, out list))
        {
            if (list.Count > 0)
            {
                go = list[list.Count - 1];
                go.SetActive(true);
                list.RemoveAt(list.Count - 1);
            }
        }
        if (go == null)
        {
            go = Instantiate(HUDPrefab[type]) as GameObject;
        }
        HUDText item = go.GetComponent<HUDText>();

        item.m_Type = type;
        item.InitPos = pos;
        item.text = text;
        item.movement = movement;
        item.Xcountervail = 0;
        item.Ycountervail = 0;
        if (item.m_TotalTime > 0)
            item.m_RemoveTime = Time.realtimeSinceStartup + item.m_TotalTime;
        else
            item.m_RemoveTime = 0;

        item.m_ElapsedTime = 0;
        item.m_Text.color = new Color(item.m_Text.color.r, item.m_Text.color.g, item.m_Text.color.b, 1);

        go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        go.GetComponent<RectTransform>().localScale = Vector3.one;

        m_list_texts.Add(item);
        UpdateText(item);

        return go;
    }
}
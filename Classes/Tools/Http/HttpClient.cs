using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// http上报数据
/// @author hannibal
/// @time 2017-4-5
/// </summary>
public class HttpClient : MonoBehaviour 
{
    public string m_URL;

    void Awake()
    {
        m_instance = this;
    }
    private static HttpClient m_instance;
    public static HttpClient Instance
    {
        get { return m_instance; }
    }

    public void PostData(Dictionary<string, string> postDatas, System.Action<string> onPostDoneAction)
    {
        if (string.IsNullOrEmpty(m_URL))
        {
            Log.Error("HttpClient.PostData, url为空");
            return;
        }

        if (!m_URL.StartsWith("http") && m_URL.StartsWith("https"))
        {
            Log.Error("HttpClient.PostData, url地址格式不正确, {0}", m_URL);
            return;
        }

        WWWForm dataForm = new WWWForm();
        if (postDatas != null)
        {
            foreach (var postData in postDatas)
            {
                dataForm.AddField(postData.Key, postData.Value);
            }
        }

        StartCoroutine(AsyncPostData(m_URL, dataForm, onPostDoneAction));
    }

    private IEnumerator AsyncPostData(string url, WWWForm dataForm, System.Action<string> onPostDoneAction)
    {
        Log.Info("start post data");
        using (WWW www = new WWW(url, dataForm))
        {
            yield return www;
            Log.Info("end post data");
            if (!string.IsNullOrEmpty(www.error))
            {
                Log.Error("AsyncPostData error:" + www.error);
            }
            else
            {
                Log.Info("recv data:" + www.text);
                if (onPostDoneAction != null)
                    onPostDoneAction(www.text);
            }
        }
    }
}

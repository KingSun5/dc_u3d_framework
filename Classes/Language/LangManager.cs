using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

/// <summary>
/// 语言包
/// @author hannibal
/// @time 2017-2-14
/// </summary>
public class LangText
{
    public List<string> m_LangList = new List<string>();
    public string m_LangFileName;

    public string this[int i]
    {
        get
        {
            if (m_LangList == null ||
                i <= 0 ||
                i >= m_LangList.Count)
            {
                return "";
            }
            return m_LangList[i];
        }
    }
    public bool Load()
    {
        ReadTxtConfig(m_LangFileName, OnLoad);
        return true;
    }
    public void Unload()
    {
        m_LangList.Clear();
    }
    public void OnLoad(string[] strList)
    {
        m_LangList.Clear();
        m_LangList.Add(""); //第一行为空 这样可以和文本文件的行数保持一致
        m_LangList.AddRange(strList);
    }
    private bool ReadTxtConfig(string fileName, Action<string[]> handler)
    {
        Log.Info("ReadTxtConfig:" + fileName);
        TextAsset textAsset = ResourceLoaderManager.Instance.LoadTextAssetInResources(fileName);
        if (textAsset == null)
        {
            Log.Error("ConfigManager::ReadTxtConfig - load error:" + fileName);
            return false;
        }
        string sText = textAsset.text.Replace("\n", "").Replace("\\n", "\n");
        handler(sText.Split('\r'));
        textAsset = null;
        return true;
    }
}

public class LangManager : Singleton<LangManager>
{
    static public LangText lang = new LangText(); //语言包

    public void Setup(string path)
    {
        lang.m_LangFileName = path;
    }
    public void Destroy()
    {
        lang.Unload();
    }

    public void LoadAll()
    {
        lang.Load();
    }

    static public string Format(string format,params object[] list)
    {
        if (format.Length <= 1)
            return format;
        format = format.Replace("\\n", "\n");  
        int paramNum = list.Length;
        for (int i = 0; i < list.Length; i++)
        {
            object  paramVal = list[i];
            string paramString = "{" + (i+1).ToString() +"}";
            format = format.Replace(paramString, paramVal.ToString());
        }
        return format;
    }
}
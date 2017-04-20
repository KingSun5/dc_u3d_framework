using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

/// <summary>
/// 语音表
/// @author hannibal
/// @time 2015-1-21
/// </summary>
public class LangManager
{
    public const string LANGUAGE_ENGLISH = "EN";
    public const string LANGUAGE_CHINESE = "zh-CN";
    public const string LANGUAGE_CHINESE_TW = "zh-TW";
    public const string LANGUAGE_JAPANESE = "JP";
    public const string LANGUAGE_FRENCH = "FR";
    public const string LANGUAGE_GERMAN = "GE";
    public const string LANGUAGE_ITALY = "IT";
    public const string LANGUAGE_KOREA = "KR";
    public const string LANGUAGE_RUSSIA = "RU";
    public const string LANGUAGE_SPANISH = "SP";

    private static string m_LanguageFile = "";
    private static SystemLanguage m_Language = SystemLanguage.Chinese;
    private static Dictionary<int, string> m_DicInfo = new Dictionary<int, string>();

    public static void Load(string file, SystemLanguage _language)
    {
        m_LanguageFile = file;
        SetLanguage(_language);
    }
    public static void Unload()
    {
        m_DicInfo.Clear();
    }

    public static void SetLanguage(SystemLanguage _language)
    {
        m_Language = _language;
        ReadXmlConfig(m_LanguageFile, OnReadFile);
    }

    private static bool ReadXmlConfig(string fileName, Action<XmlDocument> handler)
    {
        Log.Info("ReadXmlConfig:" + fileName);
        TextAsset textAsset = ResourceLoaderManager.Instance.LoadTextAsset(fileName);
        if (textAsset == null)
        {
            Log.Error("LangConfig::ReadXmlConfig - load error:" + fileName);
            return false;
        }
        else
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(textAsset.text);
            handler(xmlDoc);
            xmlDoc = null;
            ResourceLoaderManager.Instance.UnloadAsset(textAsset);
            textAsset = null;
        }
        return true;
    }

    private static void OnReadFile(XmlDocument doc)
    {
        m_DicInfo.Clear();
        string language = GetLanguageAB(m_Language);

        XmlNodeList nodeList = doc.SelectSingleNode("Lines").ChildNodes;
        foreach (XmlElement xe in nodeList)
        {
            int ID = XmlConvert.ToInt32(xe.GetAttribute("KEY"));
            string value = xe.GetAttribute(language);

            m_DicInfo.Add(ID, value);
        }
    }

    public static string GetText(int key)
    {
        if (m_DicInfo.ContainsKey(key))
        {
            return m_DicInfo[key];
        }
        return "[NoDefine]" + key;
    }

    public static string Format(string format, params object[] list)
    {
        if (format.Length <= 1)
            return format;
        format = format.Replace("\\n", "\n");
        int paramNum = list.Length;
        for (int i = 0; i < list.Length; i++)
        {
            object paramVal = list[i];
            string paramString = "{" + (i + 1).ToString() + "}";
            format = format.Replace(paramString, paramVal.ToString());
        }
        return format;
    }

    private static string GetLanguageAB(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Afrikaans:
            case SystemLanguage.Arabic:
            case SystemLanguage.Basque:
            case SystemLanguage.Belarusian:
            case SystemLanguage.Bulgarian:
            case SystemLanguage.Catalan:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                return LANGUAGE_CHINESE;
            case SystemLanguage.ChineseTraditional:
                return LANGUAGE_CHINESE_TW;
            case SystemLanguage.Czech:
            case SystemLanguage.Danish:
            case SystemLanguage.Dutch:
            case SystemLanguage.English:
            case SystemLanguage.Estonian:
            case SystemLanguage.Faroese:
            case SystemLanguage.Finnish:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.French:
                return LANGUAGE_FRENCH;
            case SystemLanguage.German:
                return LANGUAGE_GERMAN;
            case SystemLanguage.Greek:
            case SystemLanguage.Hebrew:
            case SystemLanguage.Icelandic:
            case SystemLanguage.Indonesian:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.Italian:
                return LANGUAGE_ITALY;
            case SystemLanguage.Japanese:
                return LANGUAGE_JAPANESE;
            case SystemLanguage.Korean:
                return LANGUAGE_KOREA;
            case SystemLanguage.Latvian:
            case SystemLanguage.Lithuanian:
            case SystemLanguage.Norwegian:
            case SystemLanguage.Polish:
            case SystemLanguage.Portuguese:
            case SystemLanguage.Romanian:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.Russian:
                return LANGUAGE_RUSSIA;
            case SystemLanguage.SerboCroatian:
            case SystemLanguage.Slovak:
            case SystemLanguage.Slovenian:
                return LANGUAGE_ENGLISH;
            case SystemLanguage.Spanish:
                return LANGUAGE_SPANISH;
            case SystemLanguage.Swedish:
            case SystemLanguage.Thai:
            case SystemLanguage.Turkish:
            case SystemLanguage.Ukrainian:
            case SystemLanguage.Vietnamese:
            case SystemLanguage.Unknown:
                return LANGUAGE_ENGLISH;
        }
        return LANGUAGE_CHINESE;
    }
}
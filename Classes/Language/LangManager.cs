
using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 多语言
/// @author hannibal
/// @time 2017-3-23
/// </summary>
public class LangManager
{
    public const string LANGUAGE_ENGLISH    = "EN";
    public const string LANGUAGE_CHINESE    = "zh-CN";
    public const string LANGUAGE_CHINESE_TW = "zh-TW";
    public const string LANGUAGE_JAPANESE   = "JP";
    public const string LANGUAGE_FRENCH     = "FR";
    public const string LANGUAGE_GERMAN     = "GE";
    public const string LANGUAGE_ITALY      = "IT";
    public const string LANGUAGE_KOREA      = "KR";
    public const string LANGUAGE_RUSSIA     = "RU";
    public const string LANGUAGE_SPANISH    = "SP";

    private const string KEY_CODE = "KEY";
    private static string languageFile = "";
    private static SystemLanguage language = SystemLanguage.Chinese;
    private static Dictionary<int, string> m_DicData = new Dictionary<int, string>();

    public static void Init(string file_name)
    {
        languageFile = file_name;
        SetLanguage(Application.systemLanguage);
        ReadData(file_name);
    }

    public static void Init(string file_name, SystemLanguage setLanguage)
    {
        languageFile = file_name;
        SetLanguage(setLanguage);
        ReadData(file_name);
    }

    public static void SetLanguage(SystemLanguage _language)
    {
        language = _language;
        ReadData(languageFile);
    }

    public static void Release()
    {
        m_DicData.Clear();
    }

    public static string GetText(int key)
    {
        if (m_DicData.ContainsKey(key))
        {
            return m_DicData[key];
        }
        return "[NoDefine]" + key;
    }

    private static void ReadData(string file_name)
    {
        m_DicData.Clear();
        string csvStr = (ResourceLoaderManager.Instance.LoadTextAsset(file_name)).text;
        CSVLoader loader = new CSVLoader();
        loader.ReadMultiLine(file_name);
        int languageIndex = loader.GetFirstIndexAtRow(GetLanguageAB(language), 0);
        if (-1 == languageIndex)
        {
            Debug.LogError("未读取到" + language + "任何数据，请检查配置表");
            return;
        }
        int tempRow = loader.GetRow();
        for (int i = 2; i < tempRow; ++i)
        {
            m_DicData.Add(int.Parse(loader.GetValueAt(0, i)), loader.GetValueAt(languageIndex, i));
        }
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
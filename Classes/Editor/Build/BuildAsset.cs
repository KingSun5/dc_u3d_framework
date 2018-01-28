using System.IO;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 打包assertbound
/// @author hannibal
/// @time 2016-9-19
/// </summary>
public class BuildAsset
{
    const string PC = "PC";
    const string IOS = "IOS";
    const string Android = "Android";
    const string AssetBundles = "/AssetBundles";
    const string AssetBundlesPath = "/../AssetBundles/";

    static List<string> m_Paths = new List<string>();
    static List<string> m_Files = new List<string>();

    [MenuItem("Assets/SetAssetBundleName", false, 110)]
    public static void SetAssetBundleName()
    {
        string selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(selectPath)) return;

        List<string> file_list = new List<string>();
        FileUtils.GetDirectoryFiles(selectPath, ref file_list);
        Log.Debug("[ab]文件数量:" + file_list.Count);
        if (file_list.Count > 0)
        {
            AssetBundleBuild[] buildMap = new AssetBundleBuild[file_list.Count];
            for (int i = 0; i < file_list.Count; ++i)
            {
                string file_name = file_list[i];
                Log.Debug("[ab]文件:" + file_name);
                var importer = AssetImporter.GetAtPath(file_name);
                if (importer)
                {
                    importer.assetBundleName = file_name;
                }
            }
        }
    }

    [MenuItem("Assets/Build iPhone Resource", false, 111)]
    public static void BuildiPhoneResource()
    {
        BuildTarget target;
        target = BuildTarget.iOS;
        BuildAssetResource(target);
    }

    [MenuItem("Assets/Build Android Resource", false, 112)]
    public static void BuildAndroidResource()
    {
        BuildAssetResource(BuildTarget.Android);
    }

    [MenuItem("Assets/Build Windows Resource", false, 113)]
    public static void BuildWindowsResource()
    {
        BuildAssetResource(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Assets/Build Select iPhone Resource", false, 114)]
    public static void BuildSelectiPhoneResource()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) return;
        BuildAssetResource(BuildTarget.iOS, path);
    }

    [MenuItem("Assets/Build Select Android Resource", false, 115)]
    public static void BuildSelectAndroidResource()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) return;
        //path = path.Substring(path.IndexOf("Assets/") + "Assets/".Length);
        BuildAssetResource(BuildTarget.Android, path);
    }

    [MenuItem("Assets/Build Select Windows Resource", false, 116)]
    public static void BuildSelectWindowsResource()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) return;
        BuildAssetResource(BuildTarget.StandaloneWindows, path);
    }

    /// <summary>
    /// 打包接口
    /// </summary>
    /// <param name="target">Target.</param>
    public static void BuildAssetResource(BuildTarget target, string selectPath = "")
    {
        // 根据平台得到输出目录
        string dstPath = GetPlatformPath(target);
        // 执行Build操作
        if (!Directory.Exists(dstPath))
            Directory.CreateDirectory(dstPath);
        ///2.生成assetbundle
        if (string.IsNullOrEmpty(selectPath))
        {
            BuildPipeline.BuildAssetBundles(dstPath, BuildAssetBundleOptions.None, target);
        }
        else
        {
            List<string> file_list = new List<string>();
            FileUtils.GetDirectoryFiles(selectPath, ref file_list);
            Log.Debug("[ab]需要打包的文件数量:" + file_list.Count);
            if (file_list.Count > 0)
            {
                AssetBundleBuild[] buildMap = new AssetBundleBuild[file_list.Count];
                for (int i = 0; i < file_list.Count; ++i)
                {
                    string file_name = file_list[i];
                    Log.Debug("[ab]打包文件列表:" + file_name);
                    buildMap[i].assetBundleName = file_name;
                    buildMap[i].assetNames = new string[] { file_name };
                }
                Log.Debug("[ab]开始打包:" + file_list.Count);
                BuildPipeline.BuildAssetBundles(dstPath, buildMap, BuildAssetBundleOptions.None, target);
                Log.Debug("[ab]结束打包:" + file_list.Count);
            }
        }
        EditorUtility.ClearProgressBar();

        ///3.创建文件列表
        CreateFileList(dstPath);

        // 将最新生成的AssetBundles拷贝到StreamingAssets目录
        FileUtils.DirectoryCopy(dstPath, Application.streamingAssetsPath + "/AssetBundles", true);

        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 获取输出目录
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    static string GetPlatformPath(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
                return Application.dataPath + AssetBundlesPath + PC + AssetBundles;
            case BuildTarget.iOS:
                return Application.dataPath + AssetBundlesPath + IOS + AssetBundles;
            case BuildTarget.Android:
                return Application.dataPath + AssetBundlesPath + Android + AssetBundles;
            default:
                return null;
        }
    }

    /// <summary>
    /// 创建文件列表
    /// </summary>
    static void CreateFileList(string resPath)
    {
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        m_Paths.Clear();
        m_Files.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < m_Files.Count; i++)
        {
            string file = m_Files[i];
            if (file.EndsWith(".meta") || file.Contains(".DS_Store"))
            {
                continue;
            }

            string md5 = FileUtils.MD5ByPathName(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        sw.Close();
        fs.Close();
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            m_Files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            m_Paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }
}
using System.IO;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 打包assertbound
/// @author hannibal
/// @time 2016-9-19
/// </summary>
public class BuildAsset
{
	static List<string> m_Paths = new List<string>();
	static List<string> m_Files = new List<string>();

    [MenuItem("Assets/Build IOS Resource", false, 111)]
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

    [MenuItem("Assets/Build WebGL Resource", false, 114)]
    public static void BuildWebGLResource()
    {
        BuildAssetResource(BuildTarget.WebGL);
    }

    [MenuItem("Assets/Build Select IOS Resource", false, 120)]
	public static void BuildSelectiPhoneResource()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) return;
        BuildAssetResource(BuildTarget.iOS, path);
	}

    [MenuItem("Assets/Build Select Android Resource", false, 121)]
	public static void BuildSelectAndroidResource()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) return;
        //path = path.Substring(path.IndexOf("Assets/") + "Assets/".Length);
		BuildAssetResource(BuildTarget.Android, path);
	}

    [MenuItem("Assets/Build Select Windows Resource", false, 122)]
	public static void BuildSelectWindowsResource()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) return;
        BuildAssetResource(BuildTarget.StandaloneWindows, path);
	}
    [MenuItem("Assets/Build Select WebGL Resource", false, 122)]
    public static void BuildSelectWebGLResource()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) return;
        BuildAssetResource(BuildTarget.WebGL, path);
    }
    /// <summary>
    /// 打包接口
    /// </summary>
    /// <param name="target">目标平台</param>
    /// <param name="selectPath">选择的文件夹</param>
	public static void BuildAssetResource(BuildTarget target, string selectPath="")
	{
		///1.创建目录
        string dataPath = AssetBundleUtil.DownloadAssetPath;
		if (Directory.Exists(dataPath))
		{
			Directory.Delete(dataPath, true);
		}
        string resPath = AssetBundleUtil.PackageAssetPath;
		if (!Directory.Exists(resPath))
		{
			Directory.CreateDirectory(resPath);
		}
		
		///2.生成assetbundle
        if(string.IsNullOrEmpty(selectPath))
        {
            BuildPipeline.BuildAssetBundles(resPath, BuildAssetBundleOptions.None, target);
        }
        else
        {
            List<string> file_list = new List<string>();
            FileUtils.GetFullDirectoryFiles(selectPath, ref file_list);
            Log.Debug("[ab]需要打包的文件数量:" + file_list.Count);
            if(file_list.Count > 0)
            {
                AssetBundleBuild[] buildMap = new AssetBundleBuild[file_list.Count];
                for (int i = 0; i < file_list.Count; ++i)
                {
                    string file_name = file_list[i];
                    Log.Debug("[ab]打包文件列表:" + file_name);
                    buildMap[i].assetBundleName = file_name;
                    buildMap[i].assetNames = new string[] { file_name};
                }
                Log.Debug("[ab]开始打包:" + file_list.Count);
                BuildPipeline.BuildAssetBundles(resPath, buildMap, BuildAssetBundleOptions.None, target);
                Log.Debug("[ab]结束打包:" + file_list.Count);
            }
        }
		EditorUtility.ClearProgressBar();

		///3.创建文件列表
		CreateFileList(resPath);
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
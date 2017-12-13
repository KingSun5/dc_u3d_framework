using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 同步所有场景到Build Setting
/// @author hannibal
/// @time 2015-3-4
/// </summary>
public class SyncScene : MonoBehaviour 
{
	[MenuItem("Tools/添加场景到SceneSetting")]
	static void CheckSceneSetting()
	{
		List<string> dirs = new List<string>();
		GetDirs(Application.dataPath+"/Scene",ref dirs);
		EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[dirs.Count];
		for(int i =0; i< newSettings.Length;i++)
		{
			newSettings[i] = new EditorBuildSettingsScene(dirs[i],true);
		}
		EditorBuildSettings.scenes = newSettings;
		AssetDatabase.SaveAssets();
	}
	static void GetDirs(string dirPath, ref List<string> dirs)
	{
		foreach (string path in Directory.GetFiles(dirPath))
		{
			if(System.IO.Path.GetExtension(path) == ".unity") 
			{
				dirs.Add(path.Substring(path.IndexOf("Assets/")));
			}
		}
		if (Directory.GetDirectories(dirPath).Length > 0)
		{
			foreach (string path in Directory.GetDirectories(dirPath))
			{
				GetDirs(path,ref dirs);
			}
		}
	}
}

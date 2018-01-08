using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;

/// <summary>
/// 通过配置表切割动画
/// 方式：
/// 1.在模型fbx文件夹创建一个同名的.txt帧配置表文件
/// 2.编辑配置表文件，格式是：帧范围，循环类型，动作名
/// 3.删除fbx模型文件，重新拖入到unity资源目录
/// 4.在弹出的对话框中选择Import按钮,等待结束
/// @author hannibal
/// @time 2016-1-3
/// </summary>
public class ModelImportProcess : AssetPostprocessor
{
    public void OnPreprocessModel()
    {
        if (Path.GetExtension(assetPath).ToLower() == ".fbx" && !assetPath.Contains("@"))
        {
            try
            {
                string path = Path.GetDirectoryName(assetPath);
                string file_name = Path.GetFileNameWithoutExtension(assetPath);
                string full_txt_path = path + "/" + file_name + ".txt";
                if (!File.Exists(full_txt_path)) return;

                using (StreamReader file_stream = new StreamReader(full_txt_path))
                {
                    string sAnimList = file_stream.ReadToEnd();

                    if (EditorUtility.DisplayDialog("FBX Animation Import from file", file_name, "Import", "Cancel"))
                    {
                        List<ModelImporterClipAnimation> list_clip = new List<ModelImporterClipAnimation>();
                        ParseAnimFile(sAnimList, ref list_clip);

                        ModelImporter modelImporter = assetImporter as ModelImporter;
                        modelImporter.animationType = ModelImporterAnimationType.Legacy;
                        modelImporter.meshCompression = ModelImporterMeshCompression.Medium;
                        modelImporter.clipAnimations = list_clip.ToArray();

                        EditorUtility.DisplayDialog("Imported animations", "Number of imported clips: " + modelImporter.clipAnimations.GetLength(0).ToString(), "OK");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("导入出错:" + e.Message);
                EditorUtility.DisplayDialog("Imported animations", e.Message, "OK");
            }   
        }
    }

    static void ParseAnimFile(string sAnimList, ref List<ModelImporterClipAnimation> list_clip)
    {
        Regex regexString = new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(Loop|Once|Clamp Forever| )) *(?<name>[^\r^\n]*[^\r^\n^ ])",RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        Match match = regexString.Match(sAnimList, 0);
        while (match.Success)
        {
            ModelImporterClipAnimation clip = new ModelImporterClipAnimation();

            if (match.Groups["firstFrame"].Success)
            {
                clip.firstFrame = System.Convert.ToInt32(match.Groups["firstFrame"].Value, 10);
            }
            if (match.Groups["lastFrame"].Success)
            {
                clip.lastFrame = System.Convert.ToInt32(match.Groups["lastFrame"].Value, 10);
            }
            if (match.Groups["loop"].Success)
            {
                string type = match.Groups["loop"].Value;
                switch(type)
                {
                    case "Loop": clip.wrapMode = WrapMode.Loop; break;
                    case "Once": clip.wrapMode = WrapMode.Once; break;
                    case "Clamp Forever": clip.wrapMode = WrapMode.ClampForever; break;
                }
            }
            if (match.Groups["name"].Success)
            {
                clip.name = match.Groups["name"].Value;
            }

            list_clip.Add(clip);

            match = regexString.Match(sAnimList, match.Index + match.Length);
        }
    }
}
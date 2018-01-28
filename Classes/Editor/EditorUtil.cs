using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

public class EditorUtil : Editor
{
    /// <summary>
    /// 依赖关系
    /// </summary>
    [MenuItem("Assets/Get Dependences", false, 11)]
    static void GetDependences()
    {
        Caching.CleanCache();
        string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        string[] dependences = AssetDatabase.GetDependencies(new string[] { selectionPath });
        foreach (string dependence in dependences)
            Debug.Log(dependence);
    }
}

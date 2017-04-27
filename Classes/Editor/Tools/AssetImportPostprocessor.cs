using System.Collections;
using UnityEngine;
using UnityEditor;

/// <summary>
/// unity导入资源监控
/// @author hannibal
/// @time 2017-1-3
/// </summary>
public class AssetImportPostprocessor : AssetPostprocessor
{
    //模型导入之前调用  
    public void OnPreprocessModel()
    {
    }
    //模型导入之后调用  
    public void OnPostprocessModel(GameObject go)
    {
    }
    //纹理导入之前调用，针对入到的纹理进行设置  
    void OnPreprocessTexture()
    {
        var importer = assetImporter as TextureImporter;
        var path = importer.assetPath;

        importer.maxTextureSize = 2048;
        importer.textureFormat = TextureImporterFormat.AutomaticCompressed;

        importer.ClearPlatformTextureSettings("Android");
        importer.ClearPlatformTextureSettings("iPhone");

        if (path.StartsWith("Assets/Assets/UI"))
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.spritePackingTag = "";
            
            importer.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.AutomaticTruecolor);
        }
        else
        {
            importer.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.AutomaticCompressed, 100, false);
        }
    }
    public void OnPostprocessTexture(Texture2D tex)
    {
        //Debug.Log("OnPostProcessTexture=" + this.assetPath);
    }

    public void OnPostprocessAudio(AudioClip clip)
    {

    }
    public void OnPreprocessAudio()
    {

    }
    //TODO
    public void OnPreprocessFont()
    {
        //Debug.Log("OnPreprocessTrueTypeFont=" + this.assetPath);
        TrueTypeFontImporter impor = this.assetImporter as TrueTypeFontImporter;
        impor.characterSpacing = 1;
    }
    //所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的  
    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //Debug.Log("OnPostprocessAllAssets");
        //foreach (string str in importedAsset)
        //{
        //    Debug.Log("importedAsset = " + str);
        //}
        //foreach (string str in deletedAssets)
        //{
        //    Debug.Log("deletedAssets = " + str);
        //}
        //foreach (string str in movedAssets)
        //{
        //    Debug.Log("movedAssets = " + str);
        //}
        //foreach (string str in movedFromAssetPaths)
        //{
        //    Debug.Log("movedFromAssetPaths = " + str);
        //}
    }
}
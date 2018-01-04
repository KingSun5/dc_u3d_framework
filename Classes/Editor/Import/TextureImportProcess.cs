using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureImportProcess : AssetPostprocessor
{
    void OnPreprocessTexture() {
        var importer = assetImporter as TextureImporter;
        var path = importer.assetPath;

        importer.textureCompression = TextureImporterCompression.Uncompressed;

        if (path.StartsWith("Assets/Assets/UI"))
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.spritePackingTag = "";

            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
            setting.textureCompression = TextureImporterCompression.Uncompressed;
            setting.maxTextureSize = 2048;
            setting.name = "iPhone";
            setting.format = TextureImporterFormat.RGBA32;
            setting.overridden = true;

            TextureImporterPlatformSettings pcSetting = new TextureImporterPlatformSettings();
            pcSetting.textureCompression = TextureImporterCompression.Uncompressed;
            pcSetting.maxTextureSize = 2048;
            pcSetting.name = "Standalone";
            pcSetting.format = TextureImporterFormat.RGBA32;
            pcSetting.overridden = true;

            importer.SetPlatformTextureSettings(setting);
            importer.SetPlatformTextureSettings(pcSetting);
        }
        else if (path.StartsWith("Assets/Resources/Pic"))
        {
            //if (path.StartsWith("Assets/Resources/Pic/Scene/Map"))
            //{//µØÍ¼±³¾°
            //    TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
            //    setting.textureCompression = TextureImporterCompression.Compressed;
            //    setting.maxTextureSize = 4096;
            //    setting.name = "iPhone";
            //    setting.format = TextureImporterFormat.RGBA32;
            //    setting.overridden = true;

            //    TextureImporterPlatformSettings pcSetting = new TextureImporterPlatformSettings();
            //    pcSetting.textureCompression = TextureImporterCompression.Compressed;
            //    pcSetting.maxTextureSize = 4096;
            //    pcSetting.name = "Standalone";
            //    pcSetting.format = TextureImporterFormat.RGBA32;
            //    pcSetting.overridden = true;

            //    importer.SetPlatformTextureSettings(setting);
            //    importer.SetPlatformTextureSettings(pcSetting);
            //}
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.spritePackingTag = "";
        }
        else if (path.StartsWith("Assets/Plugins")) 
        {
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
        }
        else 
        {
            var name = path.Substring(0, path.LastIndexOf('.'));
            if (name.EndsWith("_full")) 
            {
                importer.textureCompression = TextureImporterCompression.CompressedHQ;
            }
            else
            {
                //TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
                //setting.textureCompression = TextureImporterCompression.Compressed;
                //setting.maxTextureSize = 2048;
                //setting.name = "iPhone";
                //setting.allowsAlphaSplitting = false;
                //setting.overridden = true;
                //importer.SetPlatformTextureSettings(setting);
            }
        }
    }
}
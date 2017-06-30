using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublishID 
{
    public static string ResourcePlatformPath = "Publish/platform";
}

public enum ePublishPlatformType
{
    Android,
    iOS,
    Win,
    WebGL,
}

public enum eScriptingImplementation
{
    Mono2x = 0,
    IL2CPP = 1,
}
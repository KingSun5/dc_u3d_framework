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
    Win64,
    Win32,
    WebGL,
}

public enum eScriptingImplementation
{
    Mono2x = 0,
    IL2CPP = 1,
}
/// <summary>
/// 脚本库
/// </summary>
public enum eApiCompatibilityLevel
{
    NET_2_0 = 1,
    NET_2_0_Subset = 2,
}
/// <summary>
/// 目标设备
/// </summary>
public enum eTargetDevice
{
    FAT = 0,
    ARMv7 = 3,
    x86 = 4,

    iPhoneOnly = 0,
    iPadOnly = 1,
    iPhoneAndiPad = 2,
}
/// <summary>
/// 安装目录
/// </summary>
public enum eInstallLocation
{
    Auto = 0,
    PreferExternal = 1,
    ForceInternal = 2,
}
/// <summary>
/// 安卓sdk版本
/// </summary>
public enum eAndroidSdkVersions
{
    AndroidApiLevelAuto = 0,
    AndroidApiLevel16 = 16,
    AndroidApiLevel17 = 17,
    AndroidApiLevel18 = 18,
    AndroidApiLevel19 = 19,
    AndroidApiLevel21 = 21,
    AndroidApiLevel22 = 22,
    AndroidApiLevel23 = 23,
    AndroidApiLevel24 = 24,
    AndroidApiLevel25 = 25,
}
/// <summary>
/// 代码剥离级别，可以建设apk大小
/// </summary>
public enum eStrippingLevel
{
    Disabled = 0,
    StripAssemblies = 1,
    StripByteCode = 2,
    UseMicroMSCorlib = 3,
}

public enum eIOSSdkVerions
{
    DeviceSDK = 988,
    SimulatorSDK = 989,
}
public enum eIOSScriptCallOptimizationLevel
{
    SlowAndSafe = 0,
    FastButNoExceptions = 1,
}
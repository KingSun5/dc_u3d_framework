using UnityEngine;
using System.Collections;

/// <summary>
/// 平台相关
/// @author hannibal
/// @time 2014-11-26
/// </summary>
public class PlatformUtils
{
	static public bool IsPCPlatform()
	{
        if (Application.isMobilePlatform)
		{
			return false;
		}
		return true;
	}
}

/*
设备属性
您可以访问一系列特定设备的属性：

SystemInfo.deviceUniqueIdentifier    唯一的设备标识。
SystemInfo.deviceName    用户指定的设备名称。
SystemInfo.deviceModel    设备型号。
SystemInfo.operatingSystem    操作系统名称和版本。
反盗版检查
通过删除 AppStore DRM 保护来破解应用程序，并使其变成免费的的应用，这一现象非常普遍。Unity 配备了反盗版检查，可确定在提交至 AppStore 之后，应用程序是否经过修改。

可通过 Application.genuine 属性检查应用程序是否为正版（非盗版）。如果属性的返回值是 false，那么可以通知应用程序的用户，他/她正在使用破解的应用程序，或者可以禁止其访问应用程序上的某些功能。

注意： Application.genuineCheckAvailable 应与 Application.genuine 一并使用，以确认应用程序的完整性可得到实际验证。访问 Application.genuine 属性是一项相当昂贵的操作，不应用于帧更新期间或其他时序要求严格的代码。

震动支持
可通过调用 Handheld.Vibrate 触发震动。但是，缺乏震动硬件的设备将直接忽略这一调用。

活动指示器
移动操作系统配有内置的活动指示器，您可以在缓慢运行期间使用。请参阅 Handheld.StartActivityIndicator docs 作为使用范本。

屏幕方向
Unity iOS/Android 允许您控制当前屏幕的方向。如果您需要创建取决于用户如何持有设备的游戏行为，那么，检测方向更改或强制保持某些特定的方向将成为非常实用的功能。

可通过访问 Screen.orientation 属性检索设备方向。方向可能是以下一种：

纵向模式 (Portrait)    设备处于纵向模式，设备垂直放置，home 键位于下方。
纵向倒置模式 (PortraitUpsideDown)    设备处于纵向模式，但是上下颠倒。设备颠倒放置，home 键位于上方。
水平向左模式 (LandscapeLeft)    设备处于水平模式：设备垂直放置，home 键在右边。
水平向右模式 (LandscapeRight)    设备处于水平模式：设备垂直放置，home 键在左边。
可以将 Screen.orientation 设置成上述一种模式来控制屏幕方向，如需自动旋转，可将其设置成 ScreenOrientation.AutoRotation 。您可以根据具体情况禁用某些方向。

Screen.autorotateToPortrait    允许垂直方向。
Screen.autorotateToPortraitUpsideDown    允许垂直颠倒方向。
Screen.autorotateToLandscapeLeft    允许水平向左方向。
Screen.autorotateToLandscapeRight    允许水平向左方向。


*/
/*
RuntimePlatform
		OSXEditor
		In the Unity editor on Mac OS X.
		在Mac OS X 平台的Unity 编辑器中。
		OSXPlayer
		In the player on Mac OS X.
		在Mac OS X 平台的播放器中。
		WindowsPlayer
		In the player on Windows.
		在Windows平台的播放器中。
		OSXWebPlayer
		In the web player on Mac OS X.
		在Mac OS X平台的web播放器中。
		OSXDashboardPlayer
		In the Dashboard widget on Mac OS X.
		在Mac OS X平台的Dashboard widget（仪表板小工具）中。
		WindowsWebPlayer
		In the web player on Windows.
		在Windows平台的web播放器中。
		WiiPlayer
		In the player on Nintendo Wii.
		在Nintendo Wii平台的播放器中。
		WindowsEditor
		In the Unity editor on Windows.
		在Windows平台的Unity编辑器中。
		IPhonePlayer
		In the player on the iPhone.
		在iPhone平台的播放器中。
		XBOX360
		In the player on the XBOX360
		在XBOX360平台的播放器中。
		PS3
		In the player on the Play Station 3
		在PS3平台的播放器中。
		Android
		In the player on Android devices.
		在Android平台的播放器中。

*/
/**
UNITY_EDITOR	define directive for calling Unity Editor scripts from your game code.
UNITY_EDITOR_WIN	define directive for Editor code on Windows.
UNITY_EDITOR_OSX	define directive for Editor code on Mac OSX.
UNITY_STANDALONE_OSX	define directive for compiling/executing code specifically for OS X (including Universal, PPC and Intel architectures).
UNITY_STANDALONE_WIN	define directive for compiling/executing code specifically for Windows standalone applications.
UNITY_STANDALONE_LINUX	define directive for compiling/executing code specifically for Linux standalone applications.
UNITY_STANDALONE	define directive for compiling/executing code for any standalone platform (OS X, Windows or Linux).
UNITY_WII	define directive for compiling/executing code for the Wii console.
UNITY_IOS	define directive for compiling/executing code for the iOS platform.
UNITY_IPHONE	Deprecated. Use UNITY_IOS instead.
UNITY_ANDROID	define directive for the Android platform.
UNITY_PS3	define directive for running PlayStation 3 code.
UNITY_PS4	define directive for running PlayStation 4 code.
UNITY_SAMSUNGTV	define directive for executing Samsung TV code.
UNITY_XBOX360	define directive for executing Xbox 360 code.
UNITY_XBOXONE	define directive for executing Xbox One code.
UNITY_TIZEN	define directive for the Tizen platform.
UNITY_TVOS	define directive for the Apple TV platform.
UNITY_WP_8	define directive for Windows Phone 8.
UNITY_WP_8_1	define directive for Windows Phone 8.1.
UNITY_WSA	define directive for Windows Store Apps. Additionally, NETFX_CORE is defined when compiling C# files against .NET Core.
UNITY_WSA_8_0	define directive for Windows Store Apps when targeting SDK 8.0.
UNITY_WSA_8_1	define directive for Windows Store Apps when targeting SDK 8.1.
UNITY_WSA_10_0	define directive for Windows Store Apps when targeting Universal Windows 10 Apps. Additionally WINDOWS_UWP and NETFX_CORE are defined when compiling C# files against .NET Core.
UNITY_WINRT	Equivalent to UNITY_WP_8 \ UNITY_WSA.
UNITY_WINRT_8_0	Equivalent to UNITY_WP_8 \ UNITY_WSA_8_0.
UNITY_WINRT_8_1	Equivalent to UNITY_WP_8_1 \ UNITY_WSA_8_1. This is also defined when compiling against Universal SDK 8.1.
UNITY_WINRT_10_0	Equivalent to UNITY_WSA_10_0
UNITY_WEBGL	define directive for WebGL.
UNITY_ADS	define directive for calling Unity Ads methods from your game code. Version 5.2 and above.
UNITY_ANALYTICS	define directive for calling Unity Analytics methods from your game code. Version 5.2 and above.
UNITY_ASSERTIONS	define directive for assertions control process. 
 */
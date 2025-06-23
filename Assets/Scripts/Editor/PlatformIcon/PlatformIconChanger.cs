#if UNITY_EDITOR_WIN && CUSTOM_UNITY_ICON
using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Core;

[InitializeOnLoad]
public class PlatformIconChanger
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired,
        uint fuLoad);

    private const int WM_SETICON = 0x0080;
    private const int ICON_SMALL = 0;
    private const int ICON_BIG = 1;
    private const uint IMAGE_ICON = 1;
    private const uint LR_LOADFROMFILE = 16;

    private static IntPtr mainEditorWindowHandle;
    private static BuildTarget lastKnownBuildTarget = BuildTarget.NoTarget;

    static PlatformIconChanger()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        if (EditorUserBuildSettings.activeBuildTarget != lastKnownBuildTarget)
        {
            lastKnownBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            SetWindowTitleIcon();
        }
    }
    
    private static void SetWindowTitleIcon()
    {
        if (mainEditorWindowHandle == IntPtr.Zero)
        {
            mainEditorWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
        }

        if (mainEditorWindowHandle == IntPtr.Zero)
        {
            CustomDebug.LogWarning(LogCategory.Editor, "Could not find the main Unity Editor window handle.");
            return;
        }

        string iconName = EditorUserBuildSettings.activeBuildTarget + ".ico";
        string iconPath = Path.Combine(Application.dataPath, "Editor", "Icons", iconName);

        if (!File.Exists(iconPath))
        {
            CustomDebug.LogWarning(LogCategory.Editor,
                $"Icon file for platform '{iconName}' not found at path: {iconPath}. Reverting to default icon.");
            SendMessage(mainEditorWindowHandle, WM_SETICON, ICON_BIG, IntPtr.Zero);
            SendMessage(mainEditorWindowHandle, WM_SETICON, ICON_SMALL, IntPtr.Zero);
            return;
        }

        IntPtr iconHandle = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 0, 0, LR_LOADFROMFILE);

        if (iconHandle == IntPtr.Zero)
        {
            CustomDebug.LogError(LogCategory.Editor,
                "Failed to load icon from file: " + iconPath + ". Please check the .ico file format.");
            return;
        }

        SendMessage(mainEditorWindowHandle, WM_SETICON, ICON_BIG, iconHandle);
        SendMessage(mainEditorWindowHandle, WM_SETICON, ICON_SMALL, iconHandle);

        CustomDebug.Log(LogCategory.Editor,
            $"Editor icon changed for platform: {EditorUserBuildSettings.activeBuildTarget}");
    }
}
#endif
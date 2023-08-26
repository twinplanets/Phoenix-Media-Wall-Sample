using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class PhoenixSDKInstaller : EditorWindow
{
    Texture2D logo;

    private static bool isRefreshing = false; // Variable to keep track of the refresh state
    private static float refreshTime = 10f;
    private static float timer;

    [InitializeOnLoadMethod]
    private static void Init()
    {
        timer = 0f;
        string versionFilePath = "Packages/com.twinplanets.phoenix-media-wall-unity-sdk/version.txt";
        string packageJsonPath = "Packages/com.twinplanets.phoenix-media-wall-unity-sdk/package.json";

        string currentVersion = GetVersionFromPackageJson(packageJsonPath);

        if (File.Exists(versionFilePath))
        {
            string savedVersion = File.ReadAllText(versionFilePath);
            if (savedVersion == currentVersion)
            {
                // Don't show installer window
                return;
            }
        }

        // Show installer window
        // Your code to show installer window here
        EditorApplication.update += RunOnce;

        // Update version.txt with the current version
        File.WriteAllText(versionFilePath, currentVersion);
    }

    static string GetVersionFromPackageJson(string path)
    {
        string jsonText = File.ReadAllText(path);
        string versionKey = "\"version\": \"";
        int startIndex = jsonText.IndexOf(versionKey) + versionKey.Length;
        int endIndex = jsonText.IndexOf("\"", startIndex);
        return jsonText.Substring(startIndex, endIndex - startIndex);
    }

    private static void RunOnce()
    {
        EditorApplication.update -= RunOnce;
        ShowWindow();
    }

    private static void StaticUpdate()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Refresh the Asset Database
            AssetDatabase.Refresh();
            Debug.Log("Asset Database Refreshed");

            // Update the refresh state
            isRefreshing = false;

            // Remove the update function since it's no longer needed
            EditorApplication.update -= StaticUpdate;
        }
    }

    public static void ShowWindow()
    {
        GetWindow<PhoenixSDKInstaller>("Package Installer");
    }

    [MenuItem("Window/Show Phoenix SDK Installer")]
    public static void ShowWindowFromMenu()
    {
        ShowWindow();
    }

    private void OnEnable()
    {
        // Load your logo here
        logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.twinplanets.phoenix-media-wall-unity-sdk/Resources/Logo.png");
    }

    void OnGUI()
    {
        // Display logo
        if (logo != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logo, GUILayout.MaxHeight(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        // Display package name
        string titleText = isRefreshing ? "Installing Native Websocket..." : "Package Installer";
        GUILayout.Label("Package Installer", EditorStyles.boldLabel);

        // Padding for buttons
        float padding = 10f;

        // Calculate button dimensions
        float buttonWidth = position.width - 2 * padding;
        float buttonHeight = 3 * EditorGUIUtility.singleLineHeight;

        // Center buttons
        GUILayout.BeginArea(new Rect(padding, (position.height / 2) - (1.5f * buttonHeight), buttonWidth, 3 * buttonHeight + 2 * padding));

        // Button to install Native Websockets
        if (GUILayout.Button("Install Native Websockets", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            InstallNativeWebsockets();
        }

        // Add some vertical space between buttons
        GUILayout.Space(padding);

        // Button to move WebGLTemplate
        if (GUILayout.Button("Install WebGLTemplate", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            MoveWebGLTemplate();
        }

        // Add some vertical space between buttons
        GUILayout.Space(padding);

        if (GUILayout.Button("Install StreamingAssets", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            MoveStreamingAssets();
        }

        GUILayout.EndArea();
    }

    void InstallNativeWebsockets()
    {
        string manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
        string manifestText = File.ReadAllText(manifestPath);
        string newPackage = "\"com.endel.nativewebsocket\": \"https://github.com/endel/NativeWebSocket.git#upm\"";

        if (!manifestText.Contains(newPackage))
        {
            int index = manifestText.IndexOf("\"dependencies\": {");
            index += "\"dependencies\": {".Length;

            manifestText = manifestText.Insert(index, "\n    " + newPackage + ",");
            File.WriteAllText(manifestPath, manifestText);

            AssetDatabase.Refresh();
            Debug.Log("Native Websockets installed at the top of the manifest.");

            isRefreshing = true;
            timer = refreshTime;
            EditorApplication.update += StaticUpdate;
            AddDefineSymbol();
        }
        else
        {
            Debug.Log("Native Websockets already installed.");
        }
    }

    public static void MoveWebGLTemplate()
    {
        string sourcePath = "Packages/com.twinplanets.phoenix-media-wall-unity-sdk/WebGLTemplates";
        string destPath = "Assets/WebGLTemplates";

        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
        }

        CopyDirectory(sourcePath, destPath);

        AssetDatabase.Refresh();
        Debug.Log("WebGLTemplate moved to Assets folder.");
    }

    public static void MoveStreamingAssets()
    {
        string sourcePath = "Packages/com.twinplanets.phoenix-media-wall-unity-sdk/StreamingAssets";
        string destPath = "Assets/StreamingAssets";

        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
        }

        CopyDirectory(sourcePath, destPath);

        AssetDatabase.Refresh();
        Debug.Log("StreamingAssets moved to Assets folder.");
    }

    public static void CopyDirectory(string sourceDirectory, string targetDirectory)
    {
        DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
        DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

        CopyAll(diSource, diTarget);
    }

    public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        // Create the target directory only if it doesn't exist.
        if (!Directory.Exists(target.FullName))
        {
            Directory.CreateDirectory(target.FullName);
        }

        // Copy each file into the new directory.
        foreach (FileInfo fi in source.GetFiles())
        {
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
    }

    public static void AddDefineSymbol()
    {
        foreach (BuildTargetGroup group in System.Enum.GetValues(typeof(BuildTargetGroup)))
        {
            if (group == BuildTargetGroup.Unknown)
            {
                Debug.Log("Skipping BuildTargetGroup.Unknown");
                continue;
            }

            try
            {
                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
                if (!defines.Contains("NATIVE_WEBSOCKETS_INSTALLED"))
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines + ";NATIVE_WEBSOCKETS_INSTALLED");
                    Debug.Log("Set define for " + group.ToString());
                }
                else
                {
                    Debug.Log("Define already set for " + group.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to set define for " + group.ToString() + ": " + e.Message);
            }
        }
    }
}
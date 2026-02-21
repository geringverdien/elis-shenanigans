using BepInEx;
using BepInEx.Logging;
using ComputerysModdingUtilities;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: StraftatMod(isVanillaCompatible: true)]

namespace elisShenanigans;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]

public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static Plugin Instance; // store instance

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        Logger.LogInfo("Plugin loaded, waiting for scene load...");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Logger.LogInfo($"Scene loaded: {scene.name}");
        Main.Init();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
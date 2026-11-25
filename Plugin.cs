using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace DallasSkinChanger;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private Harmony _harmony;
    internal static LiveryLoader _liveryLoader;
    public static Texture2D texture = new Texture2D(2, 2);
    public static AssetBundle bundle;
    public static string selectedTexture = "test2.png";

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        _harmony = new Harmony("com.dallas.skinchanger");
        _harmony.PatchAll();

        _liveryLoader = new LiveryLoader();
    }
}

[HarmonyPatch(typeof(MissionManager), nameof(MissionManager.InstantiatePlayer))]
public static class PlayerHeliPatch
{
    [HarmonyPostfix]
    public static void Postfix(MissionManager __instance)
    {
        var heliShell = __instance.PlayerHeli.transform.parent.gameObject;

        if (!Plugin._liveryLoader.TryLoadLivery(heliShell))
        {
            return;
        }
    }
}
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
    internal static LiveryMenu _liveryMenu;
    public static Texture2D texture = new Texture2D(2, 2);
    public static AssetBundle bundle;
    public static string selectedTexture = "test.png";

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        _liveryLoader = new LiveryLoader();
        _liveryMenu = new LiveryMenu();

        _harmony = new Harmony("com.dallas.skinchanger");
        _harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(escmenu), nameof(escmenu.RunInitialSetUp))]
public static class EscMenuPatch
{
    [HarmonyPostfix]
    public static void PostFix(escmenu __instance)
    {
        Plugin._liveryMenu.BuildMenu(__instance);
    }
}

[HarmonyPatch(typeof(MissionManager), nameof(MissionManager.InstantiatePlayer))]
public static class PlayerHeliPatch
{
    [HarmonyPostfix]
    public static void Postfix(MissionManager __instance)
    {
        Plugin._liveryLoader.LoadLivery(__instance.PlayerHeli.transform.parent.gameObject);
    }
}
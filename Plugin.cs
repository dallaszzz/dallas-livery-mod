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
    public static string selectedTexture = "test1.png";

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



        //var materials = new List<Material>();
        //var livery = Plugin.bundle.LoadAsset<Material>("Assets/livery.mat");

        //// Body model
        //MeshFilter bodyMesh = heliShell.GetComponent<MeshFilter>();
        //bodyMesh.sharedMesh = Plugin.bundle.LoadAllAssets<Mesh>().Where(m => m.name == "SubMesh_0.002").FirstOrDefault();

        //// tail model
        //MeshFilter tailMesh = heliShell.GetComponentsInChildren<MeshFilter>(true).Where(m => m.name == "tail").FirstOrDefault();
        //tailMesh.sharedMesh = Plugin.bundle.LoadAllAssets<Mesh>().Where(m => m.name == "SubMesh_0.004").FirstOrDefault();

        //// body textures
        //MeshRenderer bodyRenderer = heliShell.GetComponent<MeshRenderer>();
        //bodyRenderer.GetMaterials(materials);
        //livery.shader = materials[1].shader;
        //materials[0] = livery;
        //materials[3] = livery;
        //materials[0].SetTexture("_texture", Plugin.texture);
        //materials[0].SetTexture("_MainTex", Plugin.texture);
        //materials[3].SetTexture("_texture", Plugin.texture);
        //materials[3].SetTexture("_MainTex", Plugin.texture);
        //bodyRenderer.SetMaterials(materials);

        //// tail textures
        //MeshRenderer tailRenderer = heliShell.GetComponentsInChildren<MeshRenderer>(true).Where(m => m.name == "tail").FirstOrDefault();
        //tailRenderer.GetMaterials(materials);
        //materials[1] = livery;
        //materials[1].SetTexture("_texture", Plugin.texture);
        //materials[1].SetTexture("_MainTex", Plugin.texture);
        //tailRenderer.SetMaterials(materials);
    }
}
using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace DallasSkinChanger
{
    public class LiveryLoader
    {
        public static List<ModelConfig> ModelConfigs = [];
        public static Dictionary<string, Mesh[]> LoadedMeshes = [];
        private bool ConfigsLoaded = false;
        public Texture2D LiveryTex = new(2, 2);
        public Material LiveryMat;
        public GameObject PlayerShell;

        // Loads a model and livery to an aircraft
        public void LoadLivery(GameObject heliShell)
        {
            if (!ConfigsLoaded)
                LoadHardcodedConfigs();
            PlayerShell = heliShell;

            var heliName = PlayerShell.name.Replace("(Clone)", "");

            var config = ModelConfigs.Where(i => i.ObjectName == heliName).FirstOrDefault();
            if (config.ObjectName == null)
            {
                Plugin.Logger.LogError($"Failed to find a model config for: {heliName}.");
                return;
            }

            // Material and texture loading
            if (LiveryMat == null && !LoadLiveryMaterial())
                return;

            LiveryTex = TryLoadLiveryTexture(Plugin.selectedTexture);
            LiveryMat.SetTexture("_texture", LiveryTex);
            LiveryMat.SetTexture("_MainTex", LiveryTex);



            foreach (var partConfig in config.Parts)
            {
                GameObject part = null;
                if (partConfig.PartName != heliName)
                {
                    var foundPart = PlayerShell.transform.Find(partConfig.PartName);
                    if (foundPart == null)
                    {
                        Plugin.Logger.LogWarning($"Part {partConfig.PartName} not found.");
                        continue;
                    }
                    part = foundPart.gameObject;
                }
                else
                { 
                    part = PlayerShell;
                }

                if (!part.TryGetComponent<MeshFilter>(out var filter) ||
                    !part.TryGetComponent<MeshRenderer>(out var renderer))
                    continue;

                var mesh = LoadedMeshes[config.AssetBundleName].Where(i => i.name == partConfig.MeshName).FirstOrDefault();
                if (mesh == null)
                {
                    Plugin.Logger.LogWarning($"Mesh {partConfig.MeshName} for Part {partConfig.PartName} not found.");
                    continue;
                }
                var materials = renderer.materials;
                filter.sharedMesh = mesh;

                foreach (var i in partConfig.TexturedSubmeshes)
                {
                    if (i >= 0 && i < materials.Length)
                        materials[i] = LiveryMat;
                }
                renderer.materials = materials;

                Plugin.Logger.LogInfo($"Part {partConfig.PartName} has been edited.");
            }

            Plugin.Logger.LogInfo($"Successfully loaded livery and mesh onto {heliName} with {config.Parts.Count()} parts.");
            return;
        }

        public void ChangeSelectedLiveryTexture(string name)
        {
            Plugin.selectedTexture = name + ".png";
            LiveryTex = TryLoadLiveryTexture(Plugin.selectedTexture);
            LiveryMat.SetTexture("_texture", LiveryTex);
            LiveryMat.SetTexture("_MainTex", LiveryTex);
        }

        public List<string> GetAllTextureNames()
        {
            List<string> files = [.. Directory.GetFiles(Paths.PluginPath + "/DallasSkinChanger/Liverys")];
            List<string> fileNames = [];
            foreach (var file in files)
            {
                fileNames.Add(Path.GetFileName(file).Replace(".png", ""));
            }
            return fileNames;
        }

        private bool LoadLiveryMaterial()
        {
            var shader = Shader.Find("Shader Graphs/ToonShadingMaterial");
            var bundle = AssetBundle.LoadFromFile(Path.Combine(Paths.PluginPath, "DallasSkinChanger/Resources", "liveryshared"));
            var material = bundle.LoadAsset<Material>("livery");
            if (material == null || shader == null)
            {
                Plugin.Logger.LogError($"Failed to load material or find toon shader. Aborting load.");
                return false;
            }
            material.shader = shader;
            LiveryMat = material;
            return true;
        }

        // Loads a texture from a file in the livery folder
        private Texture2D TryLoadLiveryTexture(string name)
        {
            Texture2D livery = new(2, 2);
            string texturePath = Path.Combine(Paths.PluginPath, "DallasSkinChanger/Liverys", name);
            byte[] imageData = File.ReadAllBytes(texturePath);

            if (!livery.LoadImage(imageData))
            {
                Plugin.Logger.LogError($"Failed to load texture: {name}");
                return livery;
            }
            Plugin.Logger.LogInfo($"Loaded texture: {name} ({livery.width}x{livery.height})");
            LiveryTex = livery;
            return livery;
        }

        // Loads all meshes from an asset bundle and returns them in a list
        private Mesh[] LoadMeshesFromBundle(string bundleName)
        {
            var bundle = AssetBundle.LoadFromFile(Path.Combine(Paths.PluginPath, "DallasSkinChanger/Resources", bundleName));
            return bundle.LoadAllAssets<Mesh>();
        }

        private void LoadHardcodedConfigs()
        {
            var defender = new ModelConfig()
            {
                ObjectName = "MD-500MG_Shell",
                AssetBundleName = "liverydefender",
                Parts =
                [
                    new PartConfig()
                    {
                        PartName = "MD-500MG_Shell",
                        MeshName = "SubMesh_0.002",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "healthtail/tail",
                        MeshName = "SubMesh_0.004",
                        TexturedSubmeshes = [1],
                    },
                    new PartConfig()
                    {
                        PartName = "healthskidL/SkidModelL",
                        MeshName = "SubMesh_0.006",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "healthskidR/SkidModelR",
                        MeshName = "SubMesh_0.007",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "pylonLMount/Reardoor",
                        MeshName = "SubMesh_0.010",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "pylonRMount/Reardoor",
                        MeshName = "SubMesh_0.011",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "Player_MD500Series(Clone)/CockpitParent/FrontDoors/doorleft",
                        MeshName = "SubMesh_0.009",
                        TexturedSubmeshes = [1],
                    },
                    new PartConfig()
                    {
                        PartName = "Player_MD500Series(Clone)/CockpitParent/FrontDoors/doorright",
                        MeshName = "SubMesh_0.008",
                        TexturedSubmeshes = [1],
                    },
                    new PartConfig()
                    {
                        PartName = "healthmms/mmshit",
                        MeshName = "SubMesh_0.014",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "healthmms/sightbase/MMsmodel",
                        MeshName = "SubMesh_0.013",
                        TexturedSubmeshes = [1],
                    },
                    new PartConfig()
                    {
                        PartName = "MastBase",
                        MeshName = "SubMesh_0.012",
                        TexturedSubmeshes = [0],
                    },
                ]
            };
            ModelConfigs.Add(defender);

            var ah1f = new ModelConfig()
            {
                ObjectName = "AH-1F_Shell",
                AssetBundleName = "liveryah1f",
                Parts =
                [
                    new PartConfig()
                    {
                        PartName = "AH-1F_Shell",
                        MeshName = "SubMesh_0",
                        TexturedSubmeshes = [0, 1],
                    },
                    new PartConfig()
                    {
                        PartName = "healthtail/tailbox1model",
                        MeshName = "SubMesh_0.003",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "healthtail/SyncElevator",
                        MeshName = "SubMesh_0.023",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "healthskidL/SkidModelL",
                        MeshName = "SubMesh_0.019",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "healthskidR/SkidModelR",
                        MeshName = "SubMesh_0.020",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "AmmoBayLanimator/AmmobayL",
                        MeshName = "SubMesh_0.016",
                        TexturedSubmeshes = [0, 1],
                    },
                    new PartConfig()
                    {
                        PartName = "AmmoBayRanimator/AmmobayR",
                        MeshName = "SubMesh_0.015",
                        TexturedSubmeshes = [0, 1],
                    },
                    new PartConfig()
                    {
                        PartName = "Player_AH1F(Clone)/CockpitParent/CanopyDoors/DoorLeftAnglePivot/doorleft",
                        MeshName = "SubMesh_0.018",
                        TexturedSubmeshes = [2],
                    },
                    new PartConfig()
                    {
                        PartName = "Player_AH1F(Clone)/CockpitParent/CanopyDoors/DoorRightAnglePivot/doorright",
                        MeshName = "SubMesh_0.017",
                        TexturedSubmeshes = [2],
                    },
                    new PartConfig()
                    {
                        PartName = "pylonLMount/healthpylonL/pylonVisualL",
                        MeshName = "SubMesh_0.021",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "pylonRMount/healthpylonR/pylonVisualR",
                        MeshName = "SubMesh_0.022",
                        TexturedSubmeshes = [0],
                    },
                    new PartConfig()
                    {
                        PartName = "healthTSU/sightbase/TSUvisual",
                        MeshName = "SubMesh_0.024",
                        TexturedSubmeshes = [0],
                    },
                ]
            };
            ModelConfigs.Add(ah1f);

            foreach (var config in ModelConfigs)
            {
                var meshes = LoadMeshesFromBundle(config.AssetBundleName);
                if (meshes.Count() == 0)
                    Plugin.Logger.LogWarning($"Found no meshes in bundle {config.AssetBundleName}.");
                LoadedMeshes[config.AssetBundleName] = meshes;
            }
            ConfigsLoaded = true;
        }
    }
}

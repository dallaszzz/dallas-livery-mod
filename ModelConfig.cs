using System.Collections.Generic;

namespace DallasSkinChanger
{
    // This is a config for aircraft to lay out replacement models and materials.
    // This is required because base game models lack proper UV mapping for livery textures so we replace them with edited versions.
    public struct ModelConfig
    {
        /// <summary>
        /// The name of the heli 'shell' object that this config applies to.
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Name of the asset bundle which contains all needed replacement models.
        /// </summary>
        public string AssetBundleName { get; set; }

        /// <summary>
        /// A list of part configs to apply to the shell or children.
        /// </summary>
        public List<PartConfig> Parts { get; set; }
    }

    public struct PartConfig
    {
        /// <summary>
        /// Name of the part to edit.
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        /// Name of the mesh to replace on the part. Must be the mesh name and not the mesh objects name.
        /// </summary>
        public string MeshName { get; set; }

        /// <summary>
        /// Indexes of submeshes that will have the livery applied to them.
        /// </summary>
        public List<int> TexturedSubmeshes {  get; set; }
    }
}

using FlaxEngine;

namespace FPS.Data.Impacts
{
    /// <summary>
    /// SurfaceType Script.
    /// </summary>
    public class SurfaceType
    {
        public Texture Albedo;
        [AssetReference(typeof(Surface))]
        public JsonAsset Surface;
    }
}

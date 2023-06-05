using System.Collections.Generic;
using FlaxEngine;

namespace FPS.Data.Impacts
{
    [ContentContextMenu("New/Data/Impact System/Surface Effect")]
    public class SurfaceEffect
    {
        [AssetReference(typeof(SpawnObjectEffect))]
        public List<JsonAsset> SpawnObjectEffects;
        [AssetReference(typeof(List<PlayAudioEffect>))]
        public List<JsonAsset> PlayAudioEffects;
    }
}

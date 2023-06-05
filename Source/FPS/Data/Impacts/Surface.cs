using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FPS.Data.Impacts
{
    /// <summary>
    /// Surface Script.
    /// </summary>
    [ContentContextMenu("New/Data/Impact System/Surface")]
    public class Surface
    {
        [Serializable]
        public class SurfaceImpactTypeEffect
        {
            public ImpactType ImpactType;
            [AssetReference(typeof(SurfaceEffect))]
            public JsonAsset SurfaceEffect;
        }
        public List<SurfaceImpactTypeEffect> ImpactTypeEffects;
    }
}

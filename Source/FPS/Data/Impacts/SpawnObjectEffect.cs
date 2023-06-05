using FlaxEngine;

namespace FPS.Data.Impacts
{
    [ContentContextMenu("New/Data/Impact System/Spawn Object Effect")]
    public class SpawnObjectEffect
    {
        public Prefab Prefab;
        public float Probability = 1;
        public bool ShouldRandomizeRotation;
        [Tooltip("Zero values will lock the rotation on that axis. Values up to 360 are sensible for each X,Y,Z")]
        public Vector3 RandomizedRotationMultiplier = Vector3.Zero;
    }
}

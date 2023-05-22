using FlaxEngine;

namespace FPS.Data.Guns
{
    [ContentContextMenu("New/Data/Guns/ShootConfig")]
    public class ShootConfig
    {
        public LayersMask HitMask;
        public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
        public float FireRate = 0.25f;
    }
}

using FlaxEngine;

namespace FPS.Data
{
    /// <summary>
    /// WeaponData Script.
    /// </summary>
    [ContentContextMenu("New/Data/Weapon Data")]
    public class WeaponData : ISerializable
    {
        [Range(0, 20), EditorOrder(0), EditorDisplay("Weapon Data")]
        public float FireRate = 20f;

        [Range(1000, 10000), EditorOrder(1), EditorDisplay("Weapon Data")]
        public float ProjectileVelocity = 1000f;

        [Limit(0, 20), EditorOrder(2), EditorDisplay("Weapon Data")]
        public int BulletLifetime = 10;

        [EditorOrder(3), EditorDisplay("Weapon Data")]
        public Model BulletModel;

        [EditorOrder(3), EditorDisplay("Weapon Data")]
        public Vector3 BulletScale = new Vector3(0.1f);

        [EditorOrder(4), EditorDisplay("Weapon Stats")]
        public float Damage = 15f;
    }
}

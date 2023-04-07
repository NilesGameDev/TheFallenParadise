using FlaxEngine;
using FPS.Data;

namespace FPS.Combat
{
    /// <summary>
    /// Weapon Script.
    /// </summary>
    public class Weapon : Script
    {
        [EditorOrder(0), AssetReference(typeof(WeaponData))]
        public JsonAsset WeaponData;
        public Actor BulletSpawn;

        private WeaponData _weaponData;

        /// <inheritdoc/>
        public override void OnStart()
        {
            base.OnStart();
            if (WeaponData == null)
            {
                Debug.LogWarning($"No data for the weapon were found. Actor name: {Actor.Name}");
                return;
            }
            if (BulletSpawn == null)
            {
                Debug.LogWarning($"No bullet spawn point for the weapon were found. Actor name: {Actor.Name}");
            }

            _weaponData = WeaponData.CreateInstance<WeaponData>();
        }
        
        public void SpawnBullet()
        {
            var bullet = new RigidBody
            {
                Name = "Bullet",
                StaticFlags = StaticFlags.None,
                UseCCD = true,
            };
            new StaticModel
            {
                Model = _weaponData.BulletModel,
                Parent = bullet,
                StaticFlags = StaticFlags.None
            };
            new SphereCollider
            {
                Parent = bullet,
                StaticFlags = StaticFlags.None
            };
            bullet.Transform = bullet.Transform = new Transform(BulletSpawn.Position, Quaternion.Identity, _weaponData.BulletScale);
            Level.SpawnActor(bullet);
            bullet.LinearVelocity = BulletSpawn.Direction * _weaponData.ProjectileVelocity;
            Destroy(bullet, _weaponData.BulletLifetime);
        }
    }
}

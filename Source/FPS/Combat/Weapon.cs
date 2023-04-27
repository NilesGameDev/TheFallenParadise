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
        private float _timeToShoot;

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
            if (Time.GameTime >= _timeToShoot)
            {
                UpdateNextTimeToShoot();

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
                var collider = new SphereCollider
                {
                    Parent = bullet,
                    StaticFlags = StaticFlags.None
                };

                // TODO: Find a way to better implement this damage deal
                collider.CollisionEnter += (Collision collision) =>
                {
                    if (collision.OtherActor.TryGetScript(out EnemyStats enemyStats))
                    {
                        enemyStats.TakeDamage(_weaponData.Damage);
                    }
                };

                bullet.Transform = bullet.Transform = new Transform(BulletSpawn.Position, Quaternion.Identity, _weaponData.BulletScale);
                Level.SpawnActor(bullet);

                // TODO: Better bullet velocity implementation
                bullet.LinearVelocity = BulletSpawn.Direction * _weaponData.ProjectileVelocity;
                Destroy(bullet, _weaponData.BulletLifetime);
            }
        }

        private void UpdateNextTimeToShoot()
        {
            _timeToShoot = Time.GameTime + 1f / _weaponData.FireRate;
        }
    }
}

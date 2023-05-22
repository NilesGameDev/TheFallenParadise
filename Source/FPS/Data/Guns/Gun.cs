using FlaxEngine;
using FlaxEngine.Utilities;
using FPS.Combat;

namespace FPS.Data.Guns
{
    [ContentContextMenu("New/Data/Guns/Gun")]
    public class Gun
    {
        public GunType Type;
        public string Name;
        public Prefab ModelPrefab;
        public Vector3 SpawnPoint;
        public Vector3 SpawnRotation;
        public ParticleSystem TrailSystem;

        [AssetReference(typeof(ShootConfig))]
        public JsonAsset ShootConfig;
        [AssetReference(typeof(TrailConfig))]
        public JsonAsset TrailConfig;

        private ShootConfig _shootConfig;
        private TrailConfig _trailConfig;
        private Script _activeScript;
        private Actor _modelActor;
        private float _lastShootTime;
        private Actor _bulletSpawn;
        private ParticleEffect _particleEffect; // TODO: consider using particle effect instead of Particle System
        
        public void Shoot()
        {
            if (Time.GameTime > _shootConfig.FireRate + _lastShootTime)
            {
                _lastShootTime = Time.GameTime;
                var randomSpread = new Vector3(
                    RandomUtil.Random.NextFloat(-_shootConfig.Spread.X, _shootConfig.Spread.X),
                    RandomUtil.Random.NextFloat(-_shootConfig.Spread.Y, _shootConfig.Spread.Y),
                    RandomUtil.Random.NextFloat(-_shootConfig.Spread.Z, _shootConfig.Spread.Z)
                );
                var shootDirection = _bulletSpawn.Transform.Forward + randomSpread;
                shootDirection.Normalize();

                if (Physics.RayCast(_bulletSpawn.Position, shootDirection, out RayCastHit hit, float.MaxValue, _shootConfig.HitMask))
                {
                    var effect = TrailSystem.Spawn(_bulletSpawn, _bulletSpawn.Transform, true);
                    effect.SetParameterValue("PE_BulletTrail", "AllowShoot", true);
                    effect.SetParameterValue("PE_BulletTrail", "HitTarget", hit.Point);
                    effect.SetParameterValue("PE_BulletTrail", "SimulationSpeed", _trailConfig.SimulationSpeed);
                } else
                {
                    var effect = TrailSystem.Spawn(_bulletSpawn, _bulletSpawn.Transform, true);
                    effect.SetParameterValue("PE_BulletTrail", "AllowShoot", true);
                    effect.SetParameterValue("PE_BulletTrail", "HitTarget", _bulletSpawn.Position + shootDirection * _trailConfig.MissDistance);
                    effect.SetParameterValue("PE_BulletTrail", "SimulationSpeed", _trailConfig.SimulationSpeed);
                }
            }
        }

        public void Spawn(Actor parent, Script activeScript)
        {
            _activeScript = activeScript;
            _lastShootTime = 0;

            _shootConfig = ShootConfig.CreateInstance<ShootConfig>();
            _trailConfig = TrailConfig.CreateInstance<TrailConfig>();

            _modelActor = PrefabManager.SpawnPrefab(ModelPrefab, parent);
            // TODO: Will implement this later after finish IK
            //_modelActor.LocalPosition = SpawnPoint;
            //_modelActor.LocalOrientation = Quaternion.Euler(SpawnRotation);

            _bulletSpawn = _modelActor.GetScript<Weapon>().BulletSpawn;
        }
    }
}

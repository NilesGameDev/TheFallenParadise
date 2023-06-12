using FlaxEngine;

using FPS.Core;
using FPS.Combat;
using FPS.Data.Impacts;

namespace FPS.Data.Guns
{
    [ContentContextMenu("New/Data/Guns/Gun")]
    public class Gun
    {
        // TODO: Check null for the nullable fields
        public GunType Type;
        public string Name;
        public Prefab GunPrefab;
        public Vector3 SpawnPoint;
        public Vector3 SpawnRotation;
        public ParticleSystem TrailSystem;
        public ImpactType ImpactType;
        public AudioClip ShotSFX;

        [AssetReference(typeof(ShootConfig))]
        public JsonAsset ShootConfig;
        [AssetReference(typeof(TrailConfig))]
        public JsonAsset TrailConfig;

        private ShootConfig _shootConfig;
        private TrailConfig _trailConfig;
        private Script _activeScript;
        private Actor _gunActor;
        private Actor _bulletSpawn;
        private float _lastShootTime;
        private float _initClickTime;
        private float _stopShootingTime;
        private bool _lastFrameWantedToShoot;
        private ParticleEffect _particleEffect; // TODO: consider using particle effect instead of Particle System

        public void Tick(bool wantToShoot)
        {
            _gunActor.LocalOrientation = Quaternion.Lerp(
                _gunActor.LocalOrientation,
                Quaternion.Euler(SpawnRotation),
                Time.DeltaTime * _shootConfig.RecoilRecoverySpeed
            );

            if (wantToShoot)
            {
                _lastFrameWantedToShoot = true;
                Shoot();
            }
            else if (!wantToShoot && _lastFrameWantedToShoot)
            {
                _stopShootingTime = Time.GameTime;
                _lastFrameWantedToShoot = false;
            }
        }

        public void Shoot()
        {
            if (Time.GameTime - _lastShootTime - _shootConfig.FireRate > Time.DeltaTime)
            {
                float lastDuration = Mathf.Clamp(0, _stopShootingTime - _initClickTime, _shootConfig.MaxSpreadTime);
                float lerpTime = (_shootConfig.RecoilRecoverySpeed - (Time.GameTime - _stopShootingTime)) / _shootConfig.RecoilRecoverySpeed;
                _initClickTime = Time.GameTime - Mathf.Lerp(0, lastDuration, Mathf.Saturate(lerpTime));
            }

            if (Time.GameTime > _shootConfig.FireRate + _lastShootTime)
            {
                _lastShootTime = Time.GameTime;

                var spreadAmount = _shootConfig.GetSpread(Time.GameTime - _initClickTime);
                _gunActor.Orientation += Quaternion.LookRotation(spreadAmount);
                var shootDirection = _gunActor.Transform.Forward;

                if (Physics.RayCast(_bulletSpawn.Position, shootDirection, out RayCastHit hit, float.MaxValue, _shootConfig.HitMask))
                {
                    var effect = TrailSystem.Spawn(_bulletSpawn, _bulletSpawn.Transform, true);
                    effect.SetParameterValue("PE_BulletTrail", "AllowShoot", true);
                    effect.SetParameterValue("PE_BulletTrail", "HitTarget", hit.Point);
                    effect.SetParameterValue("PE_BulletTrail", "SimulationSpeed", _trailConfig.SimulationSpeed);

                    if (hit.Collider != null)
                    {
                        SurfaceManager.Instance.HandleImpact(hit.Collider, hit.Point, hit.Normal, ImpactType);
                    }
                }
                else
                {
                    var effect = TrailSystem.Spawn(_bulletSpawn, _bulletSpawn.Transform, true);
                    effect.SetParameterValue("PE_BulletTrail", "AllowShoot", true);
                    effect.SetParameterValue("PE_BulletTrail", "HitTarget", _bulletSpawn.Position + shootDirection * _trailConfig.MissDistance);
                    effect.SetParameterValue("PE_BulletTrail", "SimulationSpeed", _trailConfig.SimulationSpeed);
                }

                var audioSource = new AudioSource();
                audioSource.Parent = _bulletSpawn;
                audioSource.PlayOnStart = false;
                audioSource.Clip = ShotSFX;
                audioSource.Play();
            }
        }

        public void Spawn(Actor parent, Script activeScript)
        {
            _activeScript = activeScript;
            _lastShootTime = 0;

            _shootConfig = ShootConfig.CreateInstance<ShootConfig>();
            _trailConfig = TrailConfig.CreateInstance<TrailConfig>();

            _shootConfig.CacheTexturePixels();

            _gunActor = PrefabManager.SpawnPrefab(GunPrefab, parent);
            // TODO: Will implement this later after finish IK
            //_gunActor.LocalPosition = SpawnPoint;
            //_gunActor.LocalOrientation = Quaternion.Euler(SpawnRotation);

            _bulletSpawn = _gunActor.GetScript<Weapon>().BulletSpawn;
        }
    }
}

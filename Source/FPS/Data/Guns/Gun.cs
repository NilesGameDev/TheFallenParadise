using FlaxEngine;

using FPS.Core;
using FPS.Data.Impacts;

namespace FPS.Data.Guns
{
    [ContentContextMenu("New/Data/Guns/Gun")]
    public class Gun
    {
        public string Name;
        public GunType Type;
        public ImpactType ImpactType;
        public Prefab GunPrefab;
        public Vector3 SpawnPoint;
        public Vector3 SpawnRotation;
        public ParticleSystem TrailSystem;
        public AudioClip ShotSFX;

        [Header("Grip Points")]
        public Vector3 RightGripPosition;
        public Vector3 LeftGripPosition;

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
                var spreadDirection = _gunActor.Transform.TransformDirection(spreadAmount);
                var targetRotation = Quaternion.Slerp(
                    _gunActor.Orientation,
                    Quaternion.LookRotation(spreadDirection, _gunActor.Transform.Up) * 0.1f,
                    10 * Time.DeltaTime // TODO: Consider use a configurable to change this lerp rate
                );
                _gunActor.Orientation = targetRotation;
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

                //var audioSource = new AudioSource();
                //audioSource.Parent = _bulletSpawn;
                //audioSource.PlayOnStart = false;
                //audioSource.Clip = ShotSFX;
                //audioSource.Play();
            }
        }

        public void Spawn(Actor parent, Script activeScript)
        {
            ValidateGunProperties();

            _activeScript = activeScript;
            _lastShootTime = 0;

            _shootConfig = ShootConfig.CreateInstance<ShootConfig>();
            _trailConfig = TrailConfig.CreateInstance<TrailConfig>();

            _shootConfig.CacheTexturePixels();

            _gunActor = PrefabManager.SpawnPrefab(GunPrefab, parent);
            _gunActor.LocalPosition = SpawnPoint;

            _bulletSpawn = _gunActor.GetScript<Weapon>()?.BulletSpawn;
            if (_bulletSpawn is null)
            {
                Debug.LogWarning("Can not find bullet spawn actor in the given gun prefab. Make sure to have the script Weapon attached to it and assign a bullet spawn!");
            }
        }

#if FLAX_EDITOR
        private void ValidateGunProperties()
        {
            if (GunPrefab is null)
            {
                Debug.LogWarning("GunPrefab is null, remember to assign JsonAsset to it!");
            }

            if (TrailSystem is null)
            {
                Debug.LogWarning("TrailSystem is null, without it the effect of bullet will not be handled correctly!");
            }
        }
#endif
    }
}

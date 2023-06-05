using FlaxEngine;

namespace FPS.Core
{
    /// <summary>
    /// PoolableParticleFX Script.
    /// </summary>
    public class PoolableParticleFX : Poolable
    {
        private float _duration;
        private float _currentDuration;

        /// <inheritdoc/>
        public override void OnStart()
        {
            var particleEffect = Actor.As<ParticleEffect>();
            if (particleEffect is null)
            {
                Debug.LogWarning("This script should be attached to a Particle Effect actor");
                return;
            }
            var particleSystem = particleEffect.ParticleSystem;
            _duration = particleSystem.Duration;
        }

        public override void OnUpdate()
        {
            _currentDuration += Time.DeltaTime;
            if (Mathf.Approximately(_currentDuration, _duration))
            {
                _currentDuration = 0;
                Actor.IsActive = false;
            }
        }
    }
}

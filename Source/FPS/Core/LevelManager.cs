using FlaxEngine;

namespace FPS.Core
{
    /// <summary>
    /// LevelManager Script.
    /// </summary>
    public class LevelManager : Script
    {
        /// <inheritdoc/>
        public override void OnStart()
        {
            ObjectPool.ResetPool();
        }

        public override void OnDestroy()
        {
            ObjectPool.ResetPool();
        }
    }
}

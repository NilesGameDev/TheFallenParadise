using FlaxEngine; 

namespace FPS.Core
{
    public class Poolable : Script
    {
        [HideInEditor]
        public ObjectPool ParentPool;

        public override void OnDisable()
        {
            ParentPool.ReleaseToPool(Actor);
        }
    }
}

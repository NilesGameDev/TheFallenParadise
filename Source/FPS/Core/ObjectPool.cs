using System.Collections.Generic;
using FlaxEngine;

namespace FPS.Core
{
    /// <summary>
    /// ObjectPool script - to handle creating and pooling actors within scenes <br/>
    /// Noted that logic to return to pool should be handle outside of this class
    /// </summary>
    public class ObjectPool
    {
        private Actor _parent;
        private Prefab _prefab;
        private int _size;
        private List<Actor> _availableObjectsPool;
        private static Dictionary<Prefab, ObjectPool> _objectPools = new Dictionary<Prefab, ObjectPool>();

        private ObjectPool(Prefab prefab, int size)
        {
            _prefab = prefab;
            _size = size;
            _availableObjectsPool = new List<Actor>(size);
        }

        public static ObjectPool CreateInstance(Prefab prefab, int size)
        {
            ObjectPool pool;

            if (_objectPools.ContainsKey(prefab))
            {
                pool = _objectPools[prefab];
            }
            else
            {
                pool = new ObjectPool(prefab, size);
                pool._parent = new EmptyActor();
                pool._parent.Name = prefab.TypeName + " Pool";
                Level.SpawnActor(pool._parent);
                pool.CreateObjects(); // TODO: Consider not pre-create some objects

                _objectPools.Add(prefab, pool);
            }

            return pool;
        }

        public static void ResetPool()
        {
            _objectPools?.Clear();
        }

        public Actor GetObject(Transform transform)
        {
            if (_availableObjectsPool.Count == 0)
            {
                CreateObject();
            }

            var instance = _availableObjectsPool[0];
            _availableObjectsPool.RemoveAt(0);
            instance.Transform = transform;
            instance.IsActive = true;
                
            return instance;
        }

        public Actor GetObject(Vector3 position, Quaternion orientation)
        {
            var transform = new Transform(position, orientation);
            return GetObject(transform);
        }

        public Actor GetObject()
        {
            return GetObject(Vector3.Zero, Quaternion.Identity);
        }

        public void ReleaseToPool(Actor poolableActor)
        {
            _availableObjectsPool.Add(poolableActor);
        }

        private void CreateObjects()
        {
            for (int i = 0; i < _size; i++)
            {
                CreateObject();
            }
        }

        private void CreateObject()
        {
            var actor = PrefabManager.SpawnPrefab(_prefab, _parent);
            if (!actor.TryGetScript(out Poolable poolable))
            {
                Debug.LogWarning("Prefab does not have Poolable, remember to add the script to use Object Pooling");
            }
            
            poolable.ParentPool = this;
            actor.IsActive = false;
        }
    }
}

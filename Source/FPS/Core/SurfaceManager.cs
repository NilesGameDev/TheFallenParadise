using System.Collections.Generic;
using System.Threading.Tasks;
using FlaxEditor;
using FlaxEngine;
using FPS.Data.Impacts;

namespace FPS.Core
{
    // TODO: Check null of the fields
    public class SurfaceManager : Script
    {
        public static SurfaceManager Instance { get; private set; }

        public List<SurfaceType> Surfaces;
        public int DefaultPoolSize = 10;
        [AssetReference(typeof(Surface))]
        public JsonAsset DefaultSurface;

        public override void OnAwake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void HandleImpact(PhysicsColliderActor hitColliderActor, Vector3 hitPoint, Vector3 hitNormal, ImpactType inputImpact)
        {
            if (hitColliderActor is Terrain)
            {
                var terrain = hitColliderActor.As<Terrain>();
                var activeTextures = GetActiveTexturesFromTerrain(terrain, hitPoint);
                foreach (var activeTexture in activeTextures)
                {
                    var surfaceType = Surfaces.Find(surface => surface.Albedo.Equals(activeTexture)); // TODO: probably move this to a dict?
                    if (surfaceType != null)
                    {
                        var surface = surfaceType.Surface.CreateInstance<Surface>();
                        foreach (var typeEffect in surface.ImpactTypeEffects)
                        {
                            if (inputImpact == typeEffect.ImpactType)
                            {
                                var surfaceEffect = typeEffect.SurfaceEffect.CreateInstance<SurfaceEffect>();
                                PlayEffects(hitPoint, hitNormal, surfaceEffect, activeTexture.Weight);
                            }
                        }
                    }
                    else
                    {
                        var defaultSurface = DefaultSurface.CreateInstance<Surface>();
                        foreach (var typeEffect in defaultSurface.ImpactTypeEffects)
                        {
                            if (inputImpact == typeEffect.ImpactType)
                            {
                                var surfaceEffect = typeEffect.SurfaceEffect.CreateInstance<SurfaceEffect>();
                                PlayEffects(hitPoint, hitNormal, surfaceEffect, 1);
                            }
                        }
                    }
                }
            }
            else if (hitColliderActor is Collider)
            {
                var staticMesh = hitColliderActor.Parent;

            }
            //var matBase = hitColliderActor.As<Collider>().Parent.As<StaticModel>;
            //var activeTextures = GetActiveTexturesFromTerrain(terrain, hitPoint);
        }

        private List<TextureWeight> GetActiveTexturesFromTerrain(Terrain terrain, Vector3 hitPoint)
        {
            // TODO: Move this code block to a new Utils class
            var terrainSize = terrain.ChunkSize * Terrain.PatchEdgeChunksCount * 100; // as world unit
            var splatMapSize = terrain.ChunkSize * Terrain.PatchEdgeChunksCount + 1;
            terrain.GetPatchCoord(0, out var pathCoord);

            var actualHitPosition = hitPoint - terrain.Position;
            var splatMapPosition = new Vector2(
                actualHitPosition.X / terrainSize * splatMapSize,
                actualHitPosition.Z / terrainSize * splatMapSize
                );
            int x = Mathf.FloorToInt(splatMapPosition.X);
            int z = Mathf.FloorToInt(splatMapPosition.Y);

            var splatMapData = default(Color32);
            unsafe
            {
                Color32* splatMapPtr = TerrainTools.GetSplatMapData(terrain, ref pathCoord, 0);
                splatMapData = splatMapPtr[z * splatMapSize + x];
            }

            // Calculate weight
            var activeTextures = new List<TextureWeight>();
            var terrainMat = terrain.Material;
            var matParams = terrainMat.Parameters;

            var totalWeight = splatMapData.R + splatMapData.G + splatMapData.B + splatMapData.A;
            var layerWeight0 = splatMapData.R / totalWeight;
            var layerWeight1 = splatMapData.G / totalWeight;
            var layerWeight2 = splatMapData.B / totalWeight;
            var layerWeight3 = splatMapData.A / totalWeight;

            if (layerWeight0 > 0)
            {
                activeTextures.Add(new TextureWeight()
                {
                    Weight = layerWeight0,
                    Texture = (Texture)matParams[0].Value
                });
            }
            if (layerWeight1 > 0)
            {
                activeTextures.Add(new TextureWeight()
                {
                    Weight = layerWeight1,
                    Texture = (Texture)matParams[1].Value
                });
            }
            if (layerWeight2 > 0)
            {
                activeTextures.Add(new TextureWeight()
                {
                    Weight = layerWeight2,
                    Texture = (Texture)matParams[2].Value
                });
            }
            if (layerWeight3 > 0)
            {
                activeTextures.Add(new TextureWeight()
                {
                    Weight = layerWeight3,
                    Texture = (Texture)matParams[3].Value
                });
            }

            return activeTextures;
        }

        private Texture GetActiveTextureFromModel(StaticModel staticModel, int faceIndex)
        {
            var usedModel = staticModel.Model;
            var meshArray = usedModel.LODs[0].Meshes;
            return null;
        }

        private void PlayEffects(Vector3 hitPoint, Vector3 hitNormal, SurfaceEffect surfaceEffect, float soundOffset)
        {
            // TODO: Optimize this code, maybe using tasks?
            foreach (var each in surfaceEffect.SpawnObjectEffects)
            {
                var spawnObjectEffect = each.CreateInstance<SpawnObjectEffect>();
                if (spawnObjectEffect.Probability > RandomUtil.Rand())
                {
                    var objectPool = ObjectPool.CreateInstance(spawnObjectEffect.Prefab, DefaultPoolSize);
                    var instance = objectPool.GetObject(hitPoint + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal));
                    var instanceTrans = instance.Transform;

                    // instanceTrans.Forward = hitNormal;
                    if (spawnObjectEffect.ShouldRandomizeRotation)
                    {
                        var offset = new Vector3(
                            RandomUtil.Rand() * 180 * spawnObjectEffect.RandomizedRotationMultiplier.X,
                            RandomUtil.Rand() * 180 * spawnObjectEffect.RandomizedRotationMultiplier.Y,
                            RandomUtil.Rand() * 180 * spawnObjectEffect.RandomizedRotationMultiplier.Z
                        );
                        instanceTrans.Orientation = Quaternion.Euler(Transform.Orientation.EulerAngles + offset);
                    }
                    instance.Transform = instanceTrans;
                }
            }

            foreach (var each in surfaceEffect.PlayAudioEffects)
            {
                var playAudioEffect = each.CreateInstance<PlayAudioEffect>();
                int randIdx = RandomUtil.Random.Next(playAudioEffect.AudioClips.Count);
                var clip = playAudioEffect.AudioClips[randIdx];
                var objectPool = ObjectPool.CreateInstance(playAudioEffect.AudioSourcePrefab, DefaultPoolSize);
                var audioSource = objectPool.GetObject(hitPoint, Quaternion.Identity)
                    .As<AudioSource>();

                var randVolume = RandomUtil.Rand() * (playAudioEffect.VolumRange.Y - playAudioEffect.VolumRange.X) + playAudioEffect.VolumRange.X;
                audioSource.Clip = clip;
                audioSource.Volume = soundOffset * randVolume;
                audioSource.Play();
                Task.Run(async () => await DisableAudioSource(audioSource, clip.Length));
            }
        }

        private async Task DisableAudioSource(AudioSource audioSource, float time)
        {
            await Task.Delay((int)(time * 1000));
            audioSource.IsActive = false;
        }

        private class TextureWeight
        {
            public float Weight;
            public Texture Texture;
        }
    }
}

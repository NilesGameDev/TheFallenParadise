using System.Linq;

using FlaxEngine;
using FlaxEngine.Utilities;

using FPS.Utilities;

namespace FPS.Data.Guns
{
    [ContentContextMenu("New/Data/Guns/ShootConfig")]
    public class ShootConfig
    {
        public LayersMask HitMask;
        public float FireRate = 0.25f;
        public float RecoilRecoverySpeed = 1f;
        public float MaxSpreadTime = 1f;
        public BulletSpreadType SpreadType = BulletSpreadType.None;

        [Header("Simple Spread")]
        public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);

        [Header("Texture-based Spread"), Range(0.001f, 5f)]
        public float SpreadMultiplier = 0.1f;
        public Texture SpreadTexture;

        private Color32[] _texturePixels;

        public void CacheTexturePixels()
        {
            if (SpreadType != BulletSpreadType.TextureBased)
            {
                return;
            }

            if (SpreadTexture == null)
            {
                Debug.LogWarning("SpreadTexture should be assigned!");
                return;
            }

            if (!SpreadTexture.GetPixels(out _texturePixels))
            {
                Debug.LogError("Failed to cache pixels from given texture");
            }
        }

        public Vector3 GetSpread(float shootTime = 0)
        {
            var spread = Vector3.Zero;

            if (SpreadType == BulletSpreadType.Simple)
            {
                spread = Vector3.Lerp(spread, new Vector3(
                    RandomUtil.Random.NextFloat(-Spread.X, Spread.X),
                    RandomUtil.Random.NextFloat(-Spread.Y, Spread.Y),
                    RandomUtil.Random.NextFloat(-Spread.Z, Spread.Z)
                    ),
                    Mathf.Saturate(shootTime / MaxSpreadTime));
            }
            else if (SpreadType == BulletSpreadType.TextureBased)
            {
                spread = GetTextureDirection(shootTime);
                spread *= SpreadMultiplier;
            }

            return spread;
        }

        private Vector3 GetTextureDirection(float shootTime)
        {
            var midTexture = new Vector2(SpreadTexture.Width / 2, SpreadTexture.Height / 2);
            var scanRadiusRate = Mathf.CeilToInt(
                Mathf.Lerp(1, midTexture.X, Mathf.Saturate(shootTime / MaxSpreadTime))
            );

            int minX = Mathf.FloorToInt(midTexture.X) - scanRadiusRate;
            int minY = Mathf.FloorToInt(midTexture.Y) - scanRadiusRate;

            var scanRadiusSize = scanRadiusRate * 2;
            Color32[] samplePixels = TextureUtils.GetPixelsSample(_texturePixels, SpreadTexture.Width, minX, minY, scanRadiusSize, scanRadiusSize);
            float[] grayScalePixels = TextureUtils.ConvertToGrayScale(samplePixels);
            float totalGrayValue = grayScalePixels.Sum();
            float randomGray = RandomUtil.Rand() * totalGrayValue;

            // Start sampling texture
            int i = 0;
            for (; i < grayScalePixels.Length; i++)
            {
                randomGray -= grayScalePixels[i];
                if (randomGray <= 0) break;
            }

            int x = minX + i % scanRadiusSize;
            int y = minY + i / scanRadiusSize;

            var targetPostition = new Vector2(x, y);
            var direction = (targetPostition - midTexture) / midTexture.X;

            return new Vector3(direction, 0);
        }
    }
}

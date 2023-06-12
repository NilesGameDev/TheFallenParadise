using System;
using FlaxEngine;

namespace FPS.Utilities
{
    public class TextureUtils
    {
        public static Color32[] GetPixelsSample(Color32[] pixels, int textureWidth, int startingX, int startingY, int blockWidth, int blockHeight)
        {
            var sampleTexture = new Color32[blockWidth * blockHeight];

            for (int y = startingY, i = 0; y < blockHeight; y++)
            {
                for (int x = startingX; x < blockWidth; x++)
                {
                    sampleTexture[i++] = pixels[y * textureWidth + x];
                }
            }

            return sampleTexture;
        }

        public static float[] ConvertToGrayScale(Color32[] originalPixels)
        {
            return Array.ConvertAll(originalPixels, pixel =>
            {
                return 0.299f * pixel.R + 0.587f * pixel.G + 0.114f * pixel.B;
            });
        }
    }
}

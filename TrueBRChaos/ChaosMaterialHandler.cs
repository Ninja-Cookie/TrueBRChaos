using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace TrueBRChaos
{
    internal static class ChaosMaterialHandler
    {
        private static readonly Dictionary<System.Guid, Material> HashedMaterials = new Dictionary<System.Guid, Material>();
        private static bool ResourcesLoaded = false;

        internal static Material GetMaterial(Bitmap bitmap, Shader shader = null)
        {
            if (!ResourcesLoaded)
                throw new System.Exception("Attempted to get material before resources loaded.");

            if (HashedMaterials.TryGetValue(bitmap.RawFormat.Guid, out var material))
                return material;
            return null;
        }

        internal static void LoadAllResourceMaterials()
        {
            if (ResourcesLoaded)
                return;

            foreach (var property in typeof(Properties.Resources).GetProperties(Extensions.flags).Where(x => x.PropertyType == typeof(Bitmap)))
            {
                ConvertToMaterial((Bitmap)property.GetValue(null, null));
                Debug.Log($"Material File \"{property.Name}\" Loaded.");
            }
            ResourcesLoaded = true;
        }

        private static void ConvertToMaterial(Bitmap bitmap, Shader shader = null)
        {
            Material material = Texture2DToMaterial(BitmapToTexture2D(bitmap));
            HashedMaterials.Add(bitmap.RawFormat.Guid, material);
        }

        private static Texture2D BitmapToTexture2D(Bitmap bitmap)
        {
            int width   = bitmap.Width;
            int height  = bitmap.Height;

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            for (int y = 0; y < width; y++) { for (int x = 0; x < height; x++)
            {
                System.Drawing.Color pixelColor = bitmap.GetPixel(x, (height - 1) - y);
                texture.SetPixel(x, y, new Color32(pixelColor.R, pixelColor.G, pixelColor.B, pixelColor.A));
            }}
            texture.Apply();
            return texture;
        }

        private static Material Texture2DToMaterial(Texture2D texture, Shader shader = null)
        {
            if (shader == null)
                shader = Shader.Find("Standard");

            Material material       = new Material(shader);
            material.mainTexture    = texture;

            return material;
        }
    }
}

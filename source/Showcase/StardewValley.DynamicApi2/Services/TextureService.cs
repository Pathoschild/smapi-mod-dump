using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Igorious.StardewValley.DynamicApi2.Compatibility;
using Igorious.StardewValley.DynamicApi2.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public sealed class TextureService
    {
        private static readonly Lazy<TextureService> Lazy = new Lazy<TextureService>(() => new TextureService());
        public static TextureService Instance => Lazy.Value;

        private IList<TextureModule> Modules { get; } = new List<TextureModule>();

        private TextureService()
        {
            EntoaroxFrameworkСompatibilityLayout.Instance.ContentIsReadyToOverride += OnLoadContent;
        }

        public TextureModule RegisterModule(string path)
        {
            var module = new TextureModule(path);
            Modules.Add(module);
            return module;
        }

        private void OnLoadContent()
        {
            EntoaroxFrameworkСompatibilityLayout.Instance.ContentIsReadyToOverride -= OnLoadContent;
            OverrideSprites();
        }

        private void OverrideSprites()
        {
            LoadFurnitureTexture();
            OverrideTexture(ref Furniture.furnitureTexture, m => m.FurnitureOverrides, TextureInfo.Furnitures);

            // TODO: Other textures.
        }

        private void LoadFurnitureTexture()
        {
            new Furniture(0, Vector2.Zero);
        }

        private void OverrideTexture(ref Texture2D originalTexture, Func<TextureModule, IReadOnlyDictionary<int, TextureRect>> getOverrides, TextureInfo info)
        {
            var modulesInfo = Modules
                .Select(m => new {Path = m.ResourcesPath, Overrides = getOverrides(m)})
                .Where(m => m.Overrides.Any())
                .ToList();

            if (!modulesInfo.Any()) return;

            var allOverrides = modulesInfo.SelectMany(m => m.Overrides).ToDictionary(kv => kv.Key, kv => kv.Value);
            ExtendTexture(ref originalTexture, allOverrides, info);

            foreach (var moduleInfo in modulesInfo)
            {
                using (var imageStream = new FileStream(Path.Combine(moduleInfo.Path, $"{info.Name}.png"), FileMode.Open))
                {
                    var overrideTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
                    foreach (var spriteOverride in moduleInfo.Overrides)
                    {
                        var textureRect = spriteOverride.Value;
                        if (textureRect.Height > 1)
                        {
                            var data = new Color[info.SpriteWidth * textureRect.Length * info.SpriteHeigth * textureRect.Height];
                            overrideTexture.GetData(0, info.GetSourceRect(overrideTexture, textureRect.Index, textureRect.Length, textureRect.Height), data, 0, data.Length);
                            originalTexture.SetData(0, info.GetSourceRect(originalTexture, spriteOverride.Key, textureRect.Length, textureRect.Height), data, 0, data.Length);
                        }
                        else
                        {
                            for (var i = 0; i < textureRect.Length; ++i)
                            {
                                var data = new Color[info.SpriteWidth * info.SpriteHeigth];
                                overrideTexture.GetData(0, info.GetSourceRect(overrideTexture, textureRect.Index + i), data, 0, data.Length);
                                originalTexture.SetData(0, info.GetSourceRect(originalTexture, spriteOverride.Key + i), data, 0, data.Length);
                            }
                        }
                    }
                }
            }

            foreach (var moduleInfo in modulesInfo)
            {
                SaveTempTexture(originalTexture, moduleInfo.Path, info.Name);
            }
        }

        [Conditional("DEBUG")]
        private void SaveTempTexture(Texture2D texture, string path, string name)
        {
            using (var imageStream = new FileStream(Path.Combine(path, $"{name}.temp.png"), FileMode.OpenOrCreate))
            {
                texture.SaveAsPng(imageStream, texture.Width, texture.Height);
            }
        }

        private void ExtendTexture(ref Texture2D originalTexture, IReadOnlyDictionary<int, TextureRect> spriteOverrides, TextureInfo info)
        {
            var texture = originalTexture;
            var maxHeight = spriteOverrides.Select(so => info.GetSourceRect(texture, so.Key, so.Value.Length, so.Value.Height)).Max(r => r.Bottom);
            if (maxHeight > originalTexture.Height)
            {
                var allData = new Color[originalTexture.Width * originalTexture.Height];
                originalTexture.GetData(allData);

                var newData = new Color[originalTexture.Width * maxHeight];
                Array.Copy(allData, newData, allData.Length);

                originalTexture = new Texture2D(Game1.graphics.GraphicsDevice, originalTexture.Width, maxHeight);
                originalTexture.SetData(newData);
            }
        }
    }
}
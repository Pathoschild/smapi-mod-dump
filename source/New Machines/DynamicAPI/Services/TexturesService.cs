using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using IDrawable = Igorious.StardewValley.DynamicAPI.Interfaces.IDrawable;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class TexturesService
    {
        #region Private Data

        private string RootPath { get; }

        private readonly Dictionary<int, TextureRect> _craftableSpriteOverrides = new Dictionary<int, TextureRect>();
        private readonly Dictionary<int, TextureRect> _itemSpriteOverrides = new Dictionary<int, TextureRect>();
        private readonly Dictionary<int, TextureRect> _cropSpriteOverrides = new Dictionary<int, TextureRect>();
        private readonly Dictionary<int, TextureRect> _treeSpriteOverrides = new Dictionary<int, TextureRect>();
        private bool NeedOverrideIridiumQualityStar { get; set; }

        #endregion

        #region	Constructor

        public TexturesService(string rootPath)
        {
            RootPath = rootPath;
            GameEvents.LoadContent += OnLoadContent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Override sprites in specific texture.
        /// </summary>
        /// <param name="textureType"></param>
        /// <param name="drawable"></param>
        public void Override(TextureType textureType, IDrawable drawable)
        {
            if (drawable.ResourceIndex == null) return;

            var allOverrides = new[] { null, _craftableSpriteOverrides, _itemSpriteOverrides, _cropSpriteOverrides, _treeSpriteOverrides };
            var overrides = allOverrides[(int)textureType];
            if (overrides == null) return;

            var key = drawable.TextureIndex;
            var newValue = new TextureRect(drawable.ResourceIndex.Value, drawable.ResourceLength, drawable.ResourceHeight);
            TextureRect oldValue;
            if (!overrides.TryGetValue(key, out oldValue))
            {
                overrides.Add(key, newValue);
            }
            else if (newValue != oldValue)
            {
                Log.Fail($"Texture for ${drawable.GetType().Name} already has another mapping {key}->{oldValue} (current: {newValue})");
            }
        }

        public void OverrideIridiumQualityStar()
        {
            NeedOverrideIridiumQualityStar = true;
        }

        #endregion

        #region	Auxiliary Methods

        private void OnLoadContent(object sender, EventArgs eventArgs)
        {
            OverrideSprites();
            GameEvents.LoadContent -= OnLoadContent;
        }

        private void OverrideSprites()
        {
            OverrideTexture(ref Game1.bigCraftableSpriteSheet, _craftableSpriteOverrides, TextureType.Craftables);
            OverrideTexture(ref Game1.objectSpriteSheet, _itemSpriteOverrides, TextureType.Items);
            OverrideTexture(ref Game1.cropSpriteSheet, _cropSpriteOverrides, TextureType.Crops);

            new FruitTree().loadSprite();
            OverrideTexture(ref FruitTree.texture, _treeSpriteOverrides, TextureType.Trees);

            if (!NeedOverrideIridiumQualityStar) return;
            Log.Info("Using overrides from \"Resources\\Other.png\"...");
            using (var imageStream = new FileStream(Path.Combine(RootPath, @"Resources\Other.png"), FileMode.Open))
            {
                var overrides = Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
                var data = new Color[8 * 8];
                overrides.GetData(0, new Rectangle(0, 0, 8, 8), data, 0, data.Length);
                Game1.mouseCursors.SetData(0, new Rectangle(338 + 2 * 8, 400, 8, 8), data, 0, data.Length);
            }
        }

        private void OverrideTexture(ref Texture2D originalTexture, Dictionary<int, TextureRect> spriteOverrides, TextureType textureType)
        {
            if (spriteOverrides.Count == 0) return;

            var overridingTexturePath = $@"Resources\{textureType}.png";
            Log.Info($"Using overrides from \"{overridingTexturePath}\"...");
            var info = TextureInfo.Default[textureType];
            var tileWidth = info.SpriteWidth;
            var tileHeight = info.SpriteHeight;

            ExtendTexture(ref originalTexture, spriteOverrides, textureType);

            using (var imageStream = new FileStream(Path.Combine(RootPath, overridingTexturePath), FileMode.Open))
            {
                var overrideTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
                foreach (var spriteOverride in spriteOverrides)
                {
                    var textureRect = spriteOverride.Value;
                    try
                    {
                        if (textureRect.Height > 1)
                        {
                            var data = new Color[tileWidth * textureRect.Length * tileHeight * textureRect.Height];
                            overrideTexture.GetData(0, info.GetSourceRect(overrideTexture, textureRect.Index, textureRect.Length, textureRect.Height), data, 0, data.Length);
                            originalTexture.SetData(0, info.GetSourceRect(originalTexture, spriteOverride.Key, textureRect.Length, textureRect.Height), data, 0, data.Length);
                        }
                        else
                        {
                            for (var i = 0; i < textureRect.Length; ++i)
                            {
                                var data = new Color[tileWidth * tileHeight];
                                overrideTexture.GetData(0, info.GetSourceRect(overrideTexture, textureRect.Index + i), data, 0, data.Length);
                                originalTexture.SetData(0, info.GetSourceRect(originalTexture, spriteOverride.Key + i), data, 0, data.Length);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Fail during overriding {textureType}: {textureRect}", e);
                    }
                }
            }
        }

        private void ExtendTexture(ref Texture2D originalTexture, Dictionary<int, TextureRect> spriteOverrides, TextureType textureType)
        {
            var textureInfo = TextureInfo.Default[textureType];
            var maxHeight = spriteOverrides.Select(so => textureInfo.GetSourceRect(so.Key, so.Value.Length, so.Value.Height)).Max(r => r.Bottom);
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

        #endregion
    }
}
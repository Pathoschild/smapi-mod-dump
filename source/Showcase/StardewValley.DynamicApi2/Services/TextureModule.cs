/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Igorious.StardewValley.DynamicApi2.Data;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public sealed class TextureModule
    {
        private readonly Dictionary<int, TextureRect> _furnitureOverrides = new Dictionary<int, TextureRect>();
        public string ResourcesPath { get; }
        private readonly Dictionary<string, Texture2D> _loadedTextures = new Dictionary<string, Texture2D>();

        internal TextureModule(string path)
        {
            ResourcesPath = path;
            GameEvents.LoadContent += OnLoadContent;
        }

        public IReadOnlyDictionary<int, TextureRect> FurnitureOverrides => _furnitureOverrides;

        public TextureModule OverrideFurniture(TextureRect source, int target)
        {
            _furnitureOverrides.Add(target, source);
            return this;
        }

        public TextureModule LoadTexture(string name)
        {
            _loadedTextures.Add(name, null);
            return this;
        }

        public Texture2D GetTexture(string name) => _loadedTextures[name];

        private void OnLoadContent(object sender, EventArgs eventArgs)
        {
            foreach (var textureName in _loadedTextures.Keys.ToList())
            {
                _loadedTextures[textureName] = TextureLoader.Instance.Load(Path.Combine(ResourcesPath, $"{textureName}.png"));
            }
        }
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using MoreGrass.Models;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace MoreGrass
{
    /// <summary>Represents a collection of a custom and base game sprites of grass for a season.</summary>
    public class SpritePool
    {
        /*********
        ** Fields
        *********/
        /// <summary>Whether the default sprites should be including in the resulting sprite collection.</summary>
        private bool _IncludeDefaultGrass;

        /// <summary>The default grass sprites in the sprite pool.</summary>
        private readonly List<Texture2D> DefaultGrassSprites = new List<Texture2D>();

        /// <summary>The custom grass sprites in the sprite pool.</summary>
        private readonly List<GrassSprite> CustomGrassSprites = new List<GrassSprite>();

        /// <summary>The cached results of <see cref="GetSprites(string)"/>.</summary>
        private readonly Dictionary<string, List<Texture2D>> GetSpriteCache = new Dictionary<string, List<Texture2D>>();


        /*********
        ** Accessors
        *********/
        /// <summary>Gets the number of sprites in the sprite pool.</summary>
        public int Count => IncludeDefaultGrass ? DefaultGrassSprites.Count + CustomGrassSprites.Count : CustomGrassSprites.Count;

        /// <summary>The default grass sprites in the sprite pool.</summary>
        public List<Texture2D> DefaultSprites => new List<Texture2D>(DefaultGrassSprites);

        /// <summary>Whether the default sprites should be including in the resulting sprite collection.</summary>
        public bool IncludeDefaultGrass
        {
            get => _IncludeDefaultGrass || CustomGrassSprites.Count == 0;
            set => _IncludeDefaultGrass = value;
        }


        /*********
        ** Public Methods
        *********/
        /// <summary>Removes all the default grass sprites from the sprite pool.</summary>
        public void ClearDefaultGrass() => DefaultGrassSprites.Clear();

        /// <summary>Adds grass to the default part of the sprite pool.</summary>
        /// <param name="sprite">The sprite to add to the sprite pool.</param>
        public void AddDefaultGrass(Texture2D sprite)
        {
            if (sprite != null)
                DefaultGrassSprites.Add(sprite);
        }

        /// <summary>Adds grass to the custom part of the sprite pool.</summary>
        /// <param name="sprite">The sprite to add to the sprite pool.</param>
        public void AddCustomGrass(Texture2D sprite, List<string> whiteListedLocations, List<string> blackListedLocations)
        {
            if (sprite != null)
                CustomGrassSprites.Add(new GrassSprite(sprite, whiteListedLocations, blackListedLocations));
        }

        /// <summary>Gets the sprites from the sprite pool for a specified location.</summary>
        /// <param name="locationName">The name of the location the grass is in.</param>
        /// <returns>The sprites from the sprite pool that can show up in the specified location.</returns>
        public List<Texture2D> GetSprites(string locationName)
        {
            if (locationName == null)
                return DefaultSprites;

            // check if the result has been cached
            locationName = locationName.ToLower();
            if (GetSpriteCache.TryGetValue(locationName, out var sprites))
                return sprites;

            // calculate result and cache it
            sprites = CustomGrassSprites.Where(grassSprite =>
                    (grassSprite.WhiteListedLocations.Count == 0 || Utilities.ContainsLocation(locationName, grassSprite.WhiteListedLocations))       
                 && (grassSprite.BlackListedLocations.Count == 0 || !Utilities.ContainsLocation(locationName, grassSprite.BlackListedLocations)))
                .Select(grassSprite => grassSprite.Sprite)
                .ToList();

            if (IncludeDefaultGrass || sprites.Count == 0)
                sprites.AddRange(DefaultGrassSprites);

            GetSpriteCache[locationName] = sprites;

            return sprites;
        }

        /// <summary>Gets a random sprite from the sprite pool for a specified location.</summary>
        /// <param name="locationName">The name of the location the grass is in.</param>
        /// <returns>A random sprite from the sprite pool that can show up in the specified location.</returns>
        public Texture2D GetRandomSprite(string locationName)
        {
            var sprites = GetSprites(locationName);
            return sprites[Game1.random.Next(sprites.Count)];
        }
    }
}

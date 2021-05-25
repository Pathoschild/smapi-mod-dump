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
using StardewValley;
using System;
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
        private List<Texture2D> DefaultGrassSprites = new List<Texture2D>();

        /// <summary>The custom grass sprites in the sprite pool.</summary>
        private List<Texture2D> CustomGrassSprites = new List<Texture2D>();


        /*********
        ** Accessors
        *********/
        /// <summary>Gets the number of sprites in the sprite pool.</summary>
        public int Count => Sprites.Count;

        /// <summary>The default grass sprites in the sprite pool.</summary>
        public List<Texture2D> DefaultSprites => new List<Texture2D>(DefaultGrassSprites);

        /// <summary>The currently active sprites in the sprite pool.</summary>
        public List<Texture2D> Sprites => (IncludeDefaultGrass) ? AllGrassSprites : new List<Texture2D>(CustomGrassSprites);

        /// <summary>Whether the default sprites should be including in the resulting sprite collection.</summary>
        public bool IncludeDefaultGrass
        {
            get => _IncludeDefaultGrass || CustomGrassSprites.Count == 0;
            set => _IncludeDefaultGrass = value;
        }

        /// <summary>The <see cref="DefaultGrassSprites"/> and <see cref="CustomGrassSprites"/> concatenated.</summary>
        private List<Texture2D> AllGrassSprites => DefaultGrassSprites.Concat(CustomGrassSprites).ToList();


        /*********
        ** Public Methods
        *********/
        /// <summary>Removes all the default grass sprites from the sprite pool.</summary>
        public void ClearDefaultGrass() => DefaultGrassSprites.Clear();

        /// <summary>Adds grass to the default part of the sprite pool.</summary>
        /// <param name="sprites">The sprites to add to the sprite pool.</param>
        public void AddDefaultGrass(params Texture2D[] sprites)
        {
            var grassSprites = sprites.Where(sprite => sprite != null);
            DefaultGrassSprites.AddRange(grassSprites);
        }

        /// <summary>Adds grass to the custom part of the sprite pool.</summary>
        /// <param name="sprites">The sprites to add to the sprite pool.</param>
        public void AddCustomGrass(params Texture2D[] sprites)
        {
            var grassSprites = sprites.Where(sprite => sprite != null);
            CustomGrassSprites.AddRange(grassSprites);
        }

        /// <summary>Gets a random sprite from the sprite pool.</summary>
        /// <param name="random">The <see cref="Random"/> object to use (if <see langword="null"/> is specified, then the <see cref="Game1.random"/> instance will be used).</param>
        /// <returns>A random sprite from the sprite pool.</returns>
        public Texture2D GetRandomSprite(Random random = null)
        {
            random ??= Game1.random;

            if (IncludeDefaultGrass)
                return AllGrassSprites[random.Next(AllGrassSprites.Count)];
            else
                return CustomGrassSprites[random.Next(CustomGrassSprites.Count)];
        }
    }
}

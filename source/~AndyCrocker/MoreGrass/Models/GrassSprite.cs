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
using System;
using System.Collections.Generic;

namespace MoreGrass.Models
{
    /// <summary>Represents a grass sprite.</summary>
    public class GrassSprite
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The grass sprite.</summary>
        public Texture2D Sprite { get; }

        /// <summary>The locations the sprite is allowed in.</summary>
        public List<string> WhiteListedLocations { get; }

        /// <summary>The locations the sprite isn't allowed in.</summary>
        public List<string> BlackListedLocations { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="sprite">The grass sprite.</param>
        /// <param name="whiteListedLocations">The locations the sprite is allowed in.</param>
        /// <param name="blackListedLocations">The locations the sprite isn't allowed in.</param>
        public GrassSprite(Texture2D sprite, List<string> whiteListedLocations, List<string> blackListedLocations)
        {
            Sprite = sprite;
            WhiteListedLocations = whiteListedLocations ?? new List<string>();
            BlackListedLocations = blackListedLocations ?? new List<string>();
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;

namespace Pathoschild.Stardew.SmallBeachFarm.Framework.Config
{
    /// <summary>The mod's hardcoded data.</summary>
    internal class ModData
    {
        /// <summary>Tile areas in the farm map where both river and ocean fish can be caught.</summary>
        public Rectangle[] MixedFishAreas { get; set; } = Array.Empty<Rectangle>();
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mushymato/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

// Better Sprinklers Plus
// https://github.com/jamescodesthings/smapi-better-sprinklers
// https://www.nexusmods.com/stardewvalley/mods/17767

namespace SprinklerAttachments.Framework.Integration
{
    /// <summary>The API which provides access to Better Sprinklers for other mods.</summary>
    public interface IBetterSprinklersApi
    {
        public static readonly string[] ModIds = new string[] {
            "com.CodesThings.BetterSprinklersPlus",
            "com.gingajamie.BetterSprinklersPlus"
        };
        /// <summary>Get the maximum sprinkler coverage supported by this mod (in tiles wide or high).</summary>
        int GetMaxGridSize();

        /// <summary>Get the relative tile coverage by supported sprinkler ID.</summary>
        IDictionary<int, Vector2[]> GetSprinklerCoverage();
    }
}
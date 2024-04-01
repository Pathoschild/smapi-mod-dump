/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

// Public API interface from Item Extensions' source code: https://github.com/misty-spring/StardewMods/blob/main/ItemExtensions/Api.cs

namespace ItemExtensions
{
    public interface IApi
    {
        /// <summary>
        /// Checks for resource data in the mod.
        /// </summary>
        /// <param name="id">Qualified item ID</param>
        /// <param name="health">MinutesUntilReady value</param>
        /// <param name="itemDropped">Item dropped by ore</param>
        /// <returns>Whether the object has ore data.</returns>
        bool IsResource(string id, out int? health, out string itemDropped);

        /// <summary>
        /// Checks mod's menu behaviors. If a target isn't provided, it'll search whether any exist.
        /// </summary>
        /// <param name="qualifiedItemId">Qualified item ID.</param>
        /// <param name="target">Item to search behavior for. (Qualified item ID)</param>
        /// <returns>Whether this item has menu behavior for target.</returns>
        bool HasBehavior(string qualifiedItemId, string target);

        bool IsClump(string qualifiedItemId);

        bool TrySpawnClump(string itemId, Vector2 position, string locationName, out string error, bool avoidOverlap = false);

        bool TrySpawnClump(string itemId, Vector2 position, GameLocation location, out string error, bool avoidOverlap = false);

        List<string> GetCustomSeeds(string itemId, bool includeSource, bool parseConditions = true);
    }
}

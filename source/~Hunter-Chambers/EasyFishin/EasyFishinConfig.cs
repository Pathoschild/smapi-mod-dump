/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace EasyFishin
{
    /// <summary>The mod configuration.</summary>
    internal class EasyFishinConfig
    {
        /// <summary>Whether the game should consider every catch to be perfectly executed, even if it wasn't.</summary>
        public bool AlwaysPerfect { get; set; } = true;

        /// <summary>Whether to always find treasure.</summary>
        public bool AlwaysFindTreasure { get; set; } = false;

        /// <summary>Whether to catch fish instantly.</summary>
        public bool InstantCatchFish { get; set; } = true;

        /// <summary>Whether to catch treasure instantly.</summary>
        public bool InstantCatchTreasure { get; set; } = true;

        /// <summary>Whether fishing tackles last forever.</summary>
        public bool InfiniteTackle { get; set; } = true;

        /// <summary>Whether fishing bait lasts forever.</summary>
        public bool InfiniteBait { get; set; } = true;

        /// <summary>Whether to automatically hook the fish without player interaction or not.</summary>
        public bool AutoHook { get; set; } = true;

        /// <summary>Whether to disable controller vibration or not.</summary>
        public bool DisableVibration { get; set; } = false;
    }
}

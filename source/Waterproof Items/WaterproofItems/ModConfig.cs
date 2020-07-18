using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Harmony;

namespace WaterproofItems
{
    /// <summary>A collection of this mod's config.json file settings.</summary>
    public class ModConfig
    {
        /// <summary>If true, the "floating on waves" visual effect is applied to items in water.</summary>
        public bool EnableCosmeticFloatingEffect { get; set; } = true;
        /// <summary>If true, items in water should teleport to the nearest player (if any).</summary>
        public bool TeleportItemsOutOfWater { get; set; } = false;
    }
}

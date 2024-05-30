/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace BetterBeachBridgeRepair
{
    public sealed class ModConfig
    {
        public bool Disable { get; set; } = false;
        public bool HideGMCM { get; set; } = false;
        public int Price { get; set; } = 50;
        public int BridgeTileX { get; set; } = 58;
        public int BridgeTileY { get; set; } = 13;
    }
}

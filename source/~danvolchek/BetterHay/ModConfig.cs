/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace BetterHay
{
    public class ModConfig
    {
        public bool EnableTakingHayFromHoppersAnytime { get; set; } = true;
        public bool EnableGettingHayFromGrassAnytime { get; set; } = true;
        public bool DropHayOnGroundIfNoRoomInInventory { get; set; } = true;
        public double ChanceToDropGrassStarterInsteadOfHay { get; set; } = 0.0;
    }
}

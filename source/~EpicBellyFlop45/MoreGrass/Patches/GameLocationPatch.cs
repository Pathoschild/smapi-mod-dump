/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using StardewValley;

namespace MoreGrass.Patches
{
    /// <summary>Contains patches for patching game code in the StardewValley.GameLocation class.</summary>
    internal class GameLocationPatch
    {
        /// <summary>This is code that will run before some game code, this is ran everything some grass tries to be grown.</summary>
        /// <returns>Whether the base method should get ran (Whether grass should grow).</returns>
        internal static bool GrowWeedGrassPrefix()
        {
            switch (Game1.currentSeason)
            {
                case "spring":
                    {
                        return ModEntry.Config.CanGrassGrowInSpring;
                    }
                case "summer":
                    {
                        return ModEntry.Config.CanGrassGrowInSummer;
                    }
                case "fall":
                    {
                        return ModEntry.Config.CanGrassGrowInFall;
                    }
                case "winter":
                    {
                        return ModEntry.Config.CanGrassGrowInWinter;
                    }
            }

            return false;
        }
    }
}

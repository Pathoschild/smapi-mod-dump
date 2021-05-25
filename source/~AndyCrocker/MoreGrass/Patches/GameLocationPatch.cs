/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using StardewValley;

namespace MoreGrass.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="GameLocation"/> class.</summary>
    internal class GameLocationPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="GameLocation.growWeedGrass(int)"/> method.</summary>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (whether grass can grow).</returns>
        /// <remarks>This is used to determine if grass can grow based on the mod configuration.</remarks>
        internal static bool GrowWeedGrassPrefix()
        {
            switch (Game1.currentSeason)
            {
                case "spring": return ModEntry.Instance.Config.CanGrassGrowInSpring;
                case "summer": return ModEntry.Instance.Config.CanGrassGrowInSummer;
                case "fall": return ModEntry.Instance.Config.CanGrassGrowInFall;
                case "winter": return ModEntry.Instance.Config.CanGrassGrowInWinter;
                default: return false;
            }
        }
    }
}

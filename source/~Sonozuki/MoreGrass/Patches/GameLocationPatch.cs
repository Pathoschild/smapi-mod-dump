/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace MoreGrass.Patches;

/// <summary>Contains patches for patching game code in the <see cref="GameLocation"/> class.</summary>
internal class GameLocationPatch
{
    /*********
    ** Internal Methods
    *********/
    /// <summary>The prefix for the <see cref="GameLocation.growWeedGrass(int)"/> method.</summary>
    /// <param name="__instance">The current <see cref="GameLocation"/> instance that is being patched.</param>
    /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/> (whether grass can grow).</returns>
    /// <remarks>This is used to determine if grass can grow based on the mod configuration.</remarks>
    internal static bool GrowWeedGrassPrefix(GameLocation __instance) =>
        __instance.GetSeason() switch
        {
            Season.Spring => ModEntry.Instance.Config.CanGrassGrowInSpring,
            Season.Summer => ModEntry.Instance.Config.CanGrassGrowInSummer,
            Season.Fall => ModEntry.Instance.Config.CanGrassGrowInFall,
            Season.Winter => ModEntry.Instance.Config.CanGrassGrowInWinter,
            _ => false
        };
}

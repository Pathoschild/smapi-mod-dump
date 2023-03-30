/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewValley.Menus;

namespace GrowableGiantCrops.HarmonyPatches.Niceties;

/// <summary>
/// Patches on the dwarf's shop stock.
/// </summary>
[HarmonyPatch(typeof(Utility))]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Named for Harmony.")]
internal static class DwarfShopPatches
{
    [HarmonyPriority(Priority.VeryLow)]
    [HarmonyPatch(nameof(Utility.getDwarfShopStock))]
    private static void Postfix(Dictionary<ISalable, int[]> __result)
    {
        try
        {
            if (Game1.player.hasMagicInk)
            {
                SObject boulder = new(Vector2.Zero, 78) { Fragility = SObject.fragility_Removable };
                __result.TryAdd(boulder, new[] { 1000, ShopMenu.infiniteStock });
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed to add a decorative boulder to the dwarf's shop stock:\n\n{ex}", LogLevel.Error);
        }
    }
}

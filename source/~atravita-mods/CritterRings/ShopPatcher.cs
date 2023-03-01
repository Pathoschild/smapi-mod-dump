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

using StardewValley.Menus;
using StardewValley.Objects;

namespace CritterRings;

/// <summary>
/// Adds the rings to Marlon's shop.
/// </summary>
[HarmonyPatch(typeof(Utility))]
internal static class ShopPatcher
{
    [HarmonyPatch(nameof(Utility.getAdventureShopStock))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(Dictionary<ISalable, int[]> __result)
    {
        try
        {
            if (!Game1.player.hasSkullKey)
            {
                return;
            }

            const int RING_COST = 2_500;

            if (ModEntry.FireFlyRing > 0)
            {
                __result.TryAdd(new Ring(ModEntry.FireFlyRing), new[] { RING_COST, ShopMenu.infiniteStock });
            }

            if (ModEntry.ButterflyRing > 0)
            {
                __result.TryAdd(new Ring(ModEntry.ButterflyRing), new[] { RING_COST, ShopMenu.infiniteStock });
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed when trying to add to adventure guild shop:\n\n{ex}", LogLevel.Error);
        }
    }
}

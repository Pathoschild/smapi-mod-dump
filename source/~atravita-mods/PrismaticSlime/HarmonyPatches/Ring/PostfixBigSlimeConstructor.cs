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
using StardewValley.Monsters;
using StardewValley.Objects;

namespace PrismaticSlime.HarmonyPatches.RingPatches;

/// <summary>
/// Holds patches against BigSlime's Vector2, int constructor.
/// </summary>
[HarmonyPatch(typeof(BigSlime))]
internal static class PostfixBigSlimeConstructor
{
    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(MethodType.Constructor, typeof(Vector2), typeof(int))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void PostfixConstructor(BigSlime __instance)
    {
        if (__instance.heldObject?.Value is not null || __instance.heldObject is null)
        {
            return;
        }
        try
        {
            if (ModEntry.PrismaticSlimeRing != -1
                && Game1.random.NextDouble() < 0.01 + (Game1.player.team.AverageDailyLuck() / 10.0) + (Game1.player.LuckLevel / 200.0))
            {
                __instance.heldObject.Value = new SObject(ModEntry.PrismaticSlimeRing, 1);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in postfix for BigSlime's constructor.\n\n{ex}", LogLevel.Error);
        }
    }

    // swap out the SObject out for a proper ring.
    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(BigSlime.getExtraDropItems))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void GetExtraDropItemsPostfix(List<Item> __result)
    {
        for (int i = 0; i < __result.Count; i++)
        {
            if (__result[i].ParentSheetIndex == ModEntry.PrismaticSlimeRing && ModEntry.PrismaticSlimeRing != -1)
            {
                __result[i] = new Ring(ModEntry.PrismaticSlimeRing);
            }
        }
    }
}
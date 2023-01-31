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

using StardewValley.Monsters;

namespace PrismaticSlime.HarmonyPatches.EggPatches;

/// <summary>
/// Adds the prismatic slime egg as a possible drop to prismatic slimes.
/// </summary>
[HarmonyPatch(typeof(GreenSlime))]
internal static class PostfixGreenSlime
{
    [UsedImplicitly]
    [HarmonyPatch(nameof(GreenSlime.getExtraDropItems))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(GreenSlime __instance,  List<Item> __result)
    {
        if (ModEntry.PrismaticSlimeEgg != -1
            && __instance.prismatic.Value
            && Game1.random.Next(2) == 0)
        {
            __result.Add(new SObject(ModEntry.PrismaticSlimeEgg, 1));
        }
    }
}

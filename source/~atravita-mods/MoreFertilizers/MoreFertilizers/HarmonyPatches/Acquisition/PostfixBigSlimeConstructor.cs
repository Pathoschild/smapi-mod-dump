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

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Holds patches against BigSlime's Vector2, int constructor.
/// </summary>
[HarmonyPatch(typeof(BigSlime))]
internal static class PostfixBigSlimeConstructor
{
    [UsedImplicitly]
    [HarmonyPatch(MethodType.Constructor, typeof(Vector2), typeof(int))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Postfix(BigSlime __instance, int mineArea)
    {
        if (__instance.heldObject?.Value is not null || __instance.heldObject is null)
        {
            return;
        }
        try
        {
            if (mineArea >= 120
                && Game1.mine?.GetAdditionalDifficulty() is > 0
                && Game1.random.NextDouble() < 0.15)
            {
                if (ModEntry.DeluxeFruitTreeFertilizerID != -1 && Game1.random.NextDouble() < 0.33)
                {
                    __instance.heldObject.Value = new SObject(ModEntry.DeluxeFruitTreeFertilizerID, 1);
                }
                else if (ModEntry.DeluxeFishFoodID != -1 && Game1.random.NextDouble() < 0.5)
                {
                    __instance.heldObject.Value = new SObject(ModEntry.DeluxeFishFoodID, 1);
                }
                else if (ModEntry.SecretJojaFertilizerID != -1 && (Utility.hasFinishedJojaRoute() || Game1.random.NextDouble() < 0.2))
                {
                    __instance.heldObject.Value = new SObject(ModEntry.SecretJojaFertilizerID, 1);
                }
                return;
            }
            if (ModEntry.LuckyFertilizerID != -1
                && mineArea >= 120
                && Game1.mine?.GetAdditionalDifficulty() is <= 0
                && Game1.random.NextDouble() < 0.05)
            {
                __instance.heldObject.Value = new SObject(ModEntry.LuckyFertilizerID, 1);
                return;
            }
            if (ModEntry.WisdomFertilizerID != -1
                && mineArea <= 120
                && Game1.random.NextDouble() < 0.15)
            { // big slimes are exceptionally rare in the normal mines.
                __instance.heldObject.Value = new SObject(ModEntry.WisdomFertilizerID, 1);
                return;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in postfix for BigSlime's constructor.\n\n{ex}", LogLevel.Error);
        }
    }
}
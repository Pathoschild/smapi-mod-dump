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
using Netcode;
using StardewValley.Locations;
using StardewValley.Objects;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Postfix to add fertilizers to breakable barrels in the mines.
/// </summary>
[HarmonyPatch(typeof(BreakableContainer))]
internal static class BreakableContainerPostfix
{
    [HarmonyPatch(nameof(BreakableContainer.releaseContents))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Postfix(GameLocation location, BreakableContainer __instance, NetInt ___containerType)
    {
        if (Game1.random.NextDouble() > 0.01 + (Game1.player.DailyLuck / 20) + (Math.Min(3, Game1.player.LuckLevel) / 100.0))
        {
            return;
        }
        int objectID = ___containerType.Value switch
        {
            BreakableContainer.barrel => location is MineShaft shaft && shaft.GetAdditionalDifficulty() > 0
                ? (Game1.random.Next(3) == 0 ? ModEntry.TreeTapperFertilizerID : ModEntry.MiraculousBeveragesID)
                : (Game1.random.Next(2) == 0 ? ModEntry.LuckyFertilizerID : ModEntry.PaddyCropFertilizerID),
            BreakableContainer.frostBarrel => location is MineShaft shaft && shaft.GetAdditionalDifficulty() > 0
                ? (Game1.random.Next(3) == 0 ? ModEntry.RapidBushFertilizerID : ModEntry.DeluxeFruitTreeFertilizerID)
                : (Game1.random.Next(2) == 0 ? ModEntry.SeedyFertilizerID : ModEntry.WisdomFertilizerID),
            BreakableContainer.darkBarrel => location is MineShaft shaft && shaft.GetAdditionalDifficulty() > 0
                ? (Utility.hasFinishedJojaRoute() && Game1.random.Next(16) == 0 ? ModEntry.SecretJojaFertilizerID : ModEntry.DeluxeJojaFertilizerID)
                : (Game1.random.Next(2) == 0 ? ModEntry.JojaFertilizerID : ModEntry.RadioactiveFertilizerID),
            BreakableContainer.desertBarrel => (location is MineShaft shaft && shaft.GetAdditionalDifficulty() > 0)
                ? (Game1.random.Next(2) == 0 ? ModEntry.BountifulFertilizerID : ModEntry.FruitTreeFertilizerID)
                : (Game1.random.Next(3) == 0 ? ModEntry.BountifulBushID : ModEntry.OrganicFertilizerID),
            BreakableContainer.volcanoBarrel =>
                Game1.random.Next(5) switch
                {
                    0 => ModEntry.FishFoodID,
                    1 => ModEntry.EverlastingFertilizerID,
                    2 => ModEntry.DeluxeFishFoodID,
                    3 => ModEntry.EverlastingFruitTreeFertilizerID,
                    _ => ModEntry.DomesticatedFishFoodID,
                },
            _ => ModEntry.FishFoodID, // should never happen.
        };
        Game1.createMultipleObjectDebris(
            index: objectID,
            xTile: (int)__instance.TileLocation.X,
            yTile: (int)__instance.TileLocation.Y,
            number: Game1.random.Next(1, Math.Clamp(Game1.player.MiningLevel / 2, 2, 6)),
            location: location);
    }
}
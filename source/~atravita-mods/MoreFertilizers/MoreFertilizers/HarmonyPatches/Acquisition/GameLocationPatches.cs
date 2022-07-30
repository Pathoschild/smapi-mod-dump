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
using StardewModdingAPI.Utilities;
using StardewValley.Monsters;
using StardewValley.Objects;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Holds patches against GameLocation so monsters on farm drop fertilizer.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class GameLocationPatches
{
#pragma warning disable SA1310 // Field names should not contain underscore. Reviewed.
    private const int MIN_MONSTER_HEALTH = 40;
    private const double DEFAULT_DROP_CHANCE = 0.25;
    private static readonly PerScreen<double> DropChance = new(() => DEFAULT_DROP_CHANCE);
#pragma warning restore SA1310 // Field names should not contain underscore

    /// <summary>
    /// Resets the dropchance, once per day.
    /// </summary>
    internal static void Reinitialize() => DropChance.Value = DEFAULT_DROP_CHANCE;

    [HarmonyPatch(nameof(GameLocation.monsterDrop))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Postfix(GameLocation __instance, Monster monster, int x, int y, Farmer who)
    {
        if(__instance is not Farm || who is null || Game1.random.NextDouble() > DropChance.Value || monster.MaxHealth < MIN_MONSTER_HEALTH)
        {
            return;
        }
        DropChance.Value *= 0.75;

        try
        {
            int passes = 1;
            do
            {
                int fertilizerToDrop = who.combatLevel.Value.GetRandomFertilizerFromLevel();
                if (fertilizerToDrop != -1)
                {
                    __instance.debris.Add(
                        monster.ModifyMonsterLoot(
                            new Debris(
                                item: new SObject(
                                    parentSheetIndex: fertilizerToDrop,
                                    initialStack: Game1.random.Next(1, Math.Clamp(monster.MaxHealth / MIN_MONSTER_HEALTH, 1, 4))),
                                debrisOrigin: new Vector2(x, y),
                                targetLocation: who.Position)));
                }
            }
            while(passes-- > 0 && who.isWearingRing(Ring.burglarsRing));
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod failed while adding additional monster drops!\n\n{ex}", LogLevel.Error);
        }
    }
}
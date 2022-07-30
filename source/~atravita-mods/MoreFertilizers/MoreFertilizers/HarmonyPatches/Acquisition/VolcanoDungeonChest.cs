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

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Postfixes Volcano Dungeon Chest to spawn fertilizers in there.
/// </summary>
[HarmonyPatch(typeof(VolcanoDungeon))]
internal static class VolcanoDungeonChest
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(VolcanoDungeon.PopulateChest))]
    private static void PostfixPopulateChest(NetObjectList<Item> items, Random chest_random)
    {
        if (chest_random.NextDouble() < 0.3)
        {
            int fertilizerToDrop = Game1.player.miningLevel.Value.GetRandomFertilizerFromLevel();
            if (fertilizerToDrop != -1)
            {
                items.Add(new SObject(fertilizerToDrop, Game1.random.Next(1, 4)));
            }
        }
    }
}
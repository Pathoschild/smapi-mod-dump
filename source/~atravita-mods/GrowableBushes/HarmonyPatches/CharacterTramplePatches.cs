/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

using AtraBase.Toolkit;

using GrowableBushes.Framework;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace GrowableBushes.HarmonyPatches;

/// <summary>
/// Holds patches that lets npcs trample bushes.
/// </summary>
[HarmonyPatch(typeof(Character))]
internal static class CharacterTramplePatches
{
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPriority(Priority.HigherThanNormal)]
    [HarmonyPatch(nameof(Character.MovePosition))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Prefix(Character __instance, GameLocation currentLocation)
    {
        if (!ModEntry.Config.ShouldNPCsTrampleBushes || __instance is not NPC npc || !npc.isVillager()
            || currentLocation?.largeTerrainFeatures is null)
        {
            return;
        }

        try
        {
            Rectangle nextPosition = npc.nextPosition(npc.FacingDirection);

            for (int i = 0; i < currentLocation.largeTerrainFeatures.Count; i++)
            {
                LargeTerrainFeature feature = currentLocation.largeTerrainFeatures[i];
                if (feature is Bush bush && bush.getBoundingBox().Contains(nextPosition)
                    && bush.modData.ContainsKey(InventoryBush.BushModData))
                {
                    bush.health = -1f;
                    Axe axe = new() { UpgradeLevel = 3 };
                    bush.performToolAction(axe, 0, bush.currentTileLocation, currentLocation);
                    currentLocation.largeTerrainFeatures.RemoveAt(i);
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in trying to trample a bush:\n\n{ex}", LogLevel.Error);
        }
    }
}

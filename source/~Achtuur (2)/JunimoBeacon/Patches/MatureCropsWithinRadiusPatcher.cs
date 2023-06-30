/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Patches;
using HarmonyLib;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace JunimoBeacon.Patches;

internal class MatureCropsWithinRadiusPatcher : GenericPatcher
{

    public static Vector2 lastKnownCropLocationTile = new Vector2(0, 0);
    public static List<JunimoHarvester> JunimoHarvesters = new List<JunimoHarvester>();

    // areThereMatureCropsWithinRadius postfix to also check radius around beacons
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            original: this.GetOriginalMethod<JunimoHut>(nameof(JunimoHut.areThereMatureCropsWithinRadius)),
            postfix: this.GetHarmonyMethod(nameof(Postfix))
        );
    }


    private static void Postfix(JunimoHut __instance, ref bool __result)
    {
        if (!Context.IsWorldReady)
            return;

        lastKnownCropLocationTile = __instance.lastKnownCropLocation.ToVector2();
        JunimoHarvesters = __instance.myJunimos;

        // Get hut-beacon group __instance is part of
        JunimoGroup hutGroup = ModEntry.Instance.GetHutGroup(__instance);

        if (hutGroup is null) 
            return;

        Farm farm = Game1.getFarm();
        // Search in beacon area for crops
        foreach (Vector2 tile in hutGroup.GetBeaconTiles())
        {
            // this line taken from source code JunimoHut -> areThereMatureCropsWithinRadius
            if (farm.isCropAtTile((int) tile.X, (int) tile.Y) && (farm.terrainFeatures[tile] as HoeDirt).readyForHarvest())
            {
                __instance.lastKnownCropLocation = new Point((int)tile.X, (int)tile.Y);
                __result = true;
                lastKnownCropLocationTile = __instance.lastKnownCropLocation.ToVector2();
                return;
            }
        }
        

        // If execution never finds valid tile then:
        // lastKnownCropLocation is 0 due to it being unchanged
        // __result is false due to it being unchanged
    }
}

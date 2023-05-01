/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace BNWCore
{
    public static class Tools_Patches
    {
        public static void Tree_PerformToolAction(ref Tree __instance, Tool t, int explosion)
        {
            if (t is Axe axe && axe.UpgradeLevel == 4 && Game1.MasterPlayer.mailReceived.Contains("wind_combat_blessing") && explosion <= 0 && ModEntry.ModHelper.Reflection.GetField<NetFloat>(__instance, "health").GetValue() > -99f)
            {
                __instance.health.Value = 0.0f;
            }
        }
        public static void FruitTree_PerformToolAction(ref FruitTree __instance, Tool t, int explosion)
        {
            if (t is Axe axe && axe.UpgradeLevel == 4 && Game1.MasterPlayer.mailReceived.Contains("wind_combat_blessing") && explosion <= 0 && ModEntry.ModHelper.Reflection.GetField<NetFloat>(__instance, "health").GetValue() > -99f)
            {
                __instance.health.Value = 0.0f;
            }
        }
        public static void Pickaxe_DoFunction(ref Pickaxe __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            if (__instance.UpgradeLevel == 4 && Game1.MasterPlayer.mailReceived.Contains("wind_combat_blessing"))
            {
                if (location.Objects.TryGetValue(new Vector2(x / 64, y / 64), out SObject obj))
                {
                    if (obj.Name == "Stone")
                    {
                        obj.MinutesUntilReady = 0;
                    }
                }
            }
        }
        public static void ResourceClump_PerformToolAction(ref ResourceClump __instance, Tool t, int damage, Vector2 tileLocation, GameLocation location)
        {
            if (t is Axe && t.UpgradeLevel == 4 && Game1.MasterPlayer.mailReceived.Contains("wind_combat_blessing") && (__instance.parentSheetIndex.Value == 600 || __instance.parentSheetIndex.Value == 602))
            {
                __instance.health.Value = 0;
            }
        }
        public static void Tool_TilesAffected_Postfix(ref List<Vector2> __result, Vector2 tileLocation, int power, Farmer who)
        {

            if (power == 5 && Game1.MasterPlayer.mailReceived.Contains("wind_combat_blessing"))
            {
                __result.Clear();
                Vector2 direction;
                Vector2 orth;
                int radius = ModEntry.Config.BNWCoreToolWidth;
                int length = ModEntry.Config.BNWCoreToolLength;
                switch (who.FacingDirection)
                {
                    case 0: direction = new Vector2(0, -1); orth = new Vector2(1, 0); break;
                    case 1: direction = new Vector2(1, 0); orth = new Vector2(0, 1); break;
                    case 2: direction = new Vector2(0, 1); orth = new Vector2(-1, 0); break;
                    case 3: direction = new Vector2(-1, 0); orth = new Vector2(0, -1); break;
                    default: direction = Vector2.Zero; orth = Vector2.Zero; break;
                }
                for (int i = 0; i < length; i++)
                {
                    __result.Add(direction * i + tileLocation);
                    for (int j = 1; j <= radius; j++)
                    {
                        __result.Add(direction * i + orth * j + tileLocation);
                        __result.Add(direction * i + orth * -j + tileLocation);
                    }
                }
            }
        }
    }
}
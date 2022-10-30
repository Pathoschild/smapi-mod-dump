/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace InstantBuildingConstruction
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(BluePrint), new Type[] { typeof(string) })]
        [HarmonyPatch(MethodType.Constructor)]
        public class Blueprint_Patch
        {
            public static void Postfix(BluePrint __instance, string name)
            {
                if (!Config.ModEnabled)
                    return;
                __instance.daysToConstruct = 0;
            }
        }

        [HarmonyPatch(typeof(CarpenterMenu), nameof(CarpenterMenu.receiveLeftClick))]
        public class CarpenterMenu_receiveLeftClick_Patch
        {
            public static void Postfix()
            {
                if (!Config.ModEnabled)
                    return;
                var buildings = Game1.getFarm().buildings;
                for (int i = 0; i < buildings.Count; i++)
                {
                    if (buildings[i].daysUntilUpgrade.Value > 0)
                    {
                        string upgrade = buildings[i].getNameOfNextUpgrade();
                        SMonitor.Log($"Upgrading {buildings[i].buildingType.Value} to {upgrade}");
                        buildings[i].daysUntilUpgrade.Value = 0;
                        Game1.player.checkForQuestComplete(null, -1, -1, null, upgrade, 8, -1);
                        BluePrint CurrentBlueprint = new BluePrint(upgrade);
                        buildings[i].buildingType.Value = CurrentBlueprint.name;
                        buildings[i].tilesHigh.Value = CurrentBlueprint.tilesHeight;
                        buildings[i].tilesWide.Value = CurrentBlueprint.tilesWidth;
                        if (buildings[i].indoors.Value is not null)
                        {
                            buildings[i].indoors.Value.mapPath.Value = "Maps\\" + CurrentBlueprint.mapToWarpTo;
                            buildings[i].indoors.Value.name.Value = CurrentBlueprint.mapToWarpTo;
                            if (buildings[i].indoors.Value is AnimalHouse)
                            {
                                ((AnimalHouse)buildings[i].indoors.Value).resetPositionsOfAllAnimals();
                                ((AnimalHouse)buildings[i].indoors.Value).animalLimit.Value += 4;
                                ((AnimalHouse)buildings[i].indoors.Value).loadLights();
                            }
                            buildings[i].updateInteriorWarps(buildings[i].indoors.Value);
                        }
                        buildings[i].resetTexture();
                    }
                }
            }
        }

    }
}
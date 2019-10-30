using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;

namespace AnimalHusbandryMod.animals
{
    public class FarmAnimalOverrides
    {
        public static void dayUpdate(FarmAnimal __instance)
        {
            if (__instance.harvestType.Value == FarmAnimal.layHarvestType 
                && __instance.daysSinceLastLay.Value == 0 
                && AnimalContestController.HasFertilityBonus(__instance)
                && !DataLoader.ModConfig.DisableContestBonus)
            {
                GameLocation homeIndoors = __instance.home.indoors.Value;
                StardewValley.Object originalLayedObject = homeIndoors.Objects[__instance.getTileLocation()];
                if (originalLayedObject.Category == StardewValley.Object.EggCategory)
                {
                    __instance.setRandomPosition(homeIndoors);
                    if (!homeIndoors.Objects.ContainsKey(__instance.getTileLocation()))
                    {
                        homeIndoors.Objects.Add(__instance.getTileLocation(), new StardewValley.Object(Vector2.Zero, originalLayedObject.ParentSheetIndex, (string)null, false, true, false, true)
                        {
                            Quality = originalLayedObject.Quality
                        });
                    }
                }
            }
            else if (!DataLoader.ModConfig.DisableContestBonus && AnimalContestController.HasProductionBonus(__instance))
            {
                if (__instance.harvestType.Value == FarmAnimal.grabHarvestType)
                {
                    __instance.produceQuality.Value = 4;
                }
                else if (__instance.daysSinceLastLay.Value == 0)
                {
                    __instance.home.indoors.Value.Objects[__instance.getTileLocation()].Quality = 4;
                }
            }
        }
    }
}

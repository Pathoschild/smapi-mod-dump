/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Events;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnimalBirthEveryNight
{
    class Patch
    {
        public static bool PickPersonalFarmEventPrefix(ref FarmEvent __result)
        {
            if (Game1.weddingToday)
            {
                Game1.showGlobalMessage($"Cannot attempt animal birth - wedding today");

                return true;
            }

            if (Game1.player.isMarried() && Game1.player.GetSpouseFriendship().DaysUntilBirthing == 0)
            {
                Game1.showGlobalMessage($"Cannot attempt animal birth - days until birthing child is 0");

                return true;
            }

            if (!Game1.IsMasterGame)
            {
                Game1.showGlobalMessage($"Cannot toggle attempt animal birth - not master game");

                return true;
            }

            __result = (FarmEvent)new QuestionEvent(2);

            return false;
        }

        public static bool SetUpPrefix(ref bool __result, ref QuestionEvent __instance)
        {
            FieldInfo field = __instance.GetType().GetField("whichQuestion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            int whichQuestion = (int)field.GetValue(__instance);

            if (whichQuestion == 2)
            {
                Dictionary<int, Building> buildings = new Dictionary<int, Building>();
                Dictionary<int, List<FarmAnimal>> farmAnimals = new Dictionary<int, List<FarmAnimal>>();
                int index = -1;

                foreach (Building building in Game1.getFarm().buildings)
                {
                    index++;

                    if (!building.owner.Equals((object)Game1.uniqueIDForThisGame) && Game1.IsMultiplayer)
                    {
                        continue;
                    }

                    if (!building.buildingType.Contains("Barn") || building.buildingType.Equals((object)"Barn"))
                    {
                        continue;
                    }

                    if ((building.indoors.Value as AnimalHouse).isFull())
                    {
                        continue;
                    }

                    // Check if has eligible animals
                    farmAnimals.Add(index, new List<FarmAnimal>());

                    foreach (long animalId in (building.indoors.Value as AnimalHouse).animalsThatLiveHere)
                    {
                        FarmAnimal farmAnimal = Utility.getAnimal(animalId);

                        if (farmAnimal.isBaby())
                        {
                            continue;
                        }

                        if (!farmAnimal.allowReproduction.Value)
                        {
                            continue;
                        }

                        farmAnimals[index].Add(farmAnimal);
                    }

                    // No animals in the building that are eligible
                    if (farmAnimals[index].Count <= 0)
                    {
                        continue;
                    }

                    // Add as eligible building
                    buildings.Add(index, building);
                }

                if (buildings.Count <= 0)
                {
                    Game1.showGlobalMessage($"Cannot attempt animal birth - no eligible animals found in any building");

                    __result = true;

                    return false;
                }

                // Get a random building
                int randomKey = buildings.Keys.ElementAt(Game1.random.Next(buildings.Count));

                Building chosenBuilding = buildings[randomKey];
                FarmAnimal chosenFarmAnimal = farmAnimals[randomKey][Game1.random.Next(farmAnimals[randomKey].Count)];

                FieldInfo animalHouseField = __instance.GetType().GetField("animalHouse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                animalHouseField.SetValue(__instance, chosenBuilding.indoors.Value as AnimalHouse);

                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Events:AnimalBirth", (object)chosenFarmAnimal.displayName, (object)chosenFarmAnimal.shortDisplayType()));
                Game1.messagePause = true;

                // Set the chosen animal
                __instance.animal = chosenFarmAnimal;

                __result = false;

                return false;
            }

            return true;
        }
    }
}

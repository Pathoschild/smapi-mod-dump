/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace AnimalsDie
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using Netcode;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Patcher
    {
        private static AnimalsDie mod;

        public static void PatchAll(AnimalsDie animalsDie)
        {
            mod = animalsDie;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchDayUpdate)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(QuestionEvent), nameof(QuestionEvent.setUp)),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(DetectPregnancy)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(SoundInTheNightEvent), nameof(SoundInTheNightEvent.makeChangesToLocation)),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(DetectAnimalAttack)));
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        public static void DetectAnimalAttack(NetInt ___behavior, Building ___targetBuilding)
        {
            try
            {
                if (!Game1.IsMasterGame)
                {
                    return;
                }

                if (___behavior.Value != SoundInTheNightEvent.dogs)
                {
                    return;
                }

                if (___targetBuilding == null || ___targetBuilding.GetIndoors() is not AnimalHouse animalHouse)
                {
                    return;
                }

                long idOfRemove = 0L;
                foreach (long animalID in animalHouse.animalsThatLiveHere)
                {
                    if (!animalHouse.animals.ContainsKey(animalID))
                    {
                        idOfRemove = animalID;
                        break;
                    }
                }

                // this event can only happen on the farm
                if (!Game1.getFarm().animals.ContainsKey(idOfRemove))
                {
                    return;
                }

                mod.WildAnimalVictim = Game1.getFarm().animals[idOfRemove];
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static void DetectPregnancy(QuestionEvent __instance, bool __result, int ___whichQuestion)
        {
            try
            {
                if (!__result && ___whichQuestion == QuestionEvent.barnBirth && __instance.animal != null)
                {
                    __instance.animal.modData.TryGetValue($"{mod.ModManifest.UniqueID}/illness", out string moddata);

                    // add one point of illness due to pregnancy stress
                    int illness = 1;

                    if (!string.IsNullOrEmpty(moddata))
                    {
                        illness += int.Parse(moddata);
                    }

                    mod.VerboseLog($"{__instance.animal.Name} received illness point due to pregnancy");

                    __instance.animal.modData[$"{mod.ModManifest.UniqueID}/illness"] = illness.ToString();
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        [HarmonyPriority(Priority.High)]
        public static void PatchDayUpdate(FarmAnimal __instance, GameLocation environment)
        {
            try
            {
                FarmAnimal animal = __instance;
                if (animal.home == null)
                {
                    mod.DebugLog($"{animal.Name} has no home anymore! This should have been fixed at the start of the day. Please report this to the mod page.");
                    return;
                }

                if (environment == null)
                {
                    mod.DebugLog($"{animal.Name} is nowhere? Please report this to the mod page. A game update or another mod probably caused this.");
                    return;
                }

                if (mod.CheckedToday.Contains(__instance))
                {
                    // sometimes the base game calls FarmAnimal.dayUpdate twice because it gets called by both the farm and the animal house
                    return;
                }
                else
                {
                    mod.CheckedToday.Add(__instance);
                }

                bool wasLeftOutLastNight = false;
                GameLocation insideHome = __instance.home?.GetIndoors();

                if (insideHome != null && !__instance.IsHome)
                {
                    if (!__instance.home?.animalDoorOpen.Value == true)
                    {
                        wasLeftOutLastNight = true;
                    }
                }

                int actualFullness = animal.fullness.Value;

                if (!wasLeftOutLastNight)
                {
                    if (actualFullness < 200 && environment is AnimalHouse)
                    {
                        // technically, this isn't correct if you don't have enough hay for all your animals (since I don't remove the hay here), but... whatever
                        KeyValuePair<Vector2, StardewValley.Object>[] array = environment.objects.Pairs.ToArray();
                        for (int i = 0; i < array.Length; i++)
                        {
                            KeyValuePair<Vector2, StardewValley.Object> pair = array[i];
                            if (pair.Value.QualifiedItemId == "(O)178")
                            {
                                actualFullness = 255;
                                break;
                            }
                        }
                    }
                }

                int starvation = mod.CalculateStarvation(animal, actualFullness);

                if (mod.Config.DeathByStarvation && starvation >= mod.Config.DaysToDieDueToStarvation)
                {
                    mod.AnimalsToKill.Add(new Tuple<FarmAnimal, string>(animal, Cause.starvation.ToString()));
                    return;
                }

                bool gotWater = false;

                if (mod.WaterMod != null)
                {
                    gotWater = !mod.WaterMod.WasAnimalLeftThirstyYesterday(animal);

                    int dehydration = mod.CalculateDehydration(animal, gotWater);

                    if (mod.Config.DeathByDehydrationWithAnimalsNeedWaterMod && dehydration >= mod.Config.DaysToDieDueToDehydrationWithAnimalsNeedWaterMod)
                    {
                        mod.AnimalsToKill.Add(new Tuple<FarmAnimal, string>(animal, Cause.dehydration.ToString()));
                        return;
                    }
                }

                int illness = mod.CalculateIllness(animal, actualFullness, gotWater, wasLeftOutLastNight);

                if (mod.Config.DeathByIllness && illness >= mod.Config.IllnessScoreToDie)
                {
                    mod.AnimalsToKill.Add(new Tuple<FarmAnimal, string>(animal, Cause.illness.ToString()));
                    return;
                }
                else if (illness >= mod.Config.IllnessScoreToDie / 2)
                {
                    mod.SickAnimals.Add(animal);
                }

                if (mod.Config.DeathByOldAge && mod.ShouldDieOfOldAge(animal))
                {
                    mod.AnimalsToKill.Add(new Tuple<FarmAnimal, string>(animal, Cause.oldAge.ToString()));
                    return;
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }
    }
}
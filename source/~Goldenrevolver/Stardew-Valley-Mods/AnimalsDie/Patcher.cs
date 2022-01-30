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
    using Netcode;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Events;
    using System;

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
                   original: AccessTools.Method(typeof(FarmAnimal), "dayUpdate"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchDayUpdate)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(QuestionEvent), "setUp"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(DetectPregnancy)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(SoundInTheNightEvent), "makeChangesToLocation"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(DetectAnimalAttack)));
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        public static bool DetectAnimalAttack(NetInt ___behavior, Building ___targetBuilding)
        {
            try
            {
                if (!Game1.IsMasterGame)
                {
                    return true;
                }

                if (___behavior.Value == SoundInTheNightEvent.dogs && ___targetBuilding != null)
                {
                    AnimalHouse indoors = ___targetBuilding.indoors.Value as AnimalHouse;
                    long idOfRemove = 0L;
                    foreach (long a in indoors.animalsThatLiveHere)
                    {
                        if (!indoors.animals.ContainsKey(a))
                        {
                            idOfRemove = a;
                            break;
                        }
                    }

                    if (!Game1.getFarm().animals.ContainsKey(idOfRemove))
                    {
                        return true;
                    }

                    mod.WildAnimalVictim = Game1.getFarm().animals[idOfRemove];
                }

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static void DetectPregnancy(ref QuestionEvent __instance, ref bool __result, int ___whichQuestion)
        {
            try
            {
                if (!__result && ___whichQuestion == 2 && __instance.animal != null)
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

        // yes, there is a typo in environtment in the base game and whenever it gets fixed this doesn't work anymore
        [HarmonyPriority(Priority.High)]
        public static bool PatchDayUpdate(ref FarmAnimal __instance, ref GameLocation environtment)
        {
            try
            {
                FarmAnimal animal = __instance;
                if (animal.home == null)
                {
                    mod.DebugLog($"{animal.Name} has no home anymore! This should have been fixed at the start of the day. Please report this to the mod page.");
                    return true;
                }

                if (environtment == null)
                {
                    mod.DebugLog($"{animal.Name} is nowhere? Please report this to the mod page. A game update or another mod probably caused this.");
                    return true;
                }

                if (mod.CheckedToday.Contains(__instance))
                {
                    // sometimes the base game calls FarmAnimal.dayUpdate twice because it gets called by both the farm and the animal house
                    return true;
                }
                else
                {
                    mod.CheckedToday.Add(__instance);
                }

                bool wasLeftOutLastNight = false;
                if (!(animal.home.indoors.Value as AnimalHouse).animals.ContainsKey(animal.myID.Value) && environtment is Farm)
                {
                    if (!animal.home.animalDoorOpen.Value)
                    {
                        wasLeftOutLastNight = true;
                    }
                }

                byte actualFullness = animal.fullness.Value;

                if (!wasLeftOutLastNight)
                {
                    if (actualFullness < 200 && animal.home.indoors.Value is AnimalHouse)
                    {
                        for (int i = animal.home.indoors.Value.objects.Count() - 1; i >= 0; i--)
                        {
                            if (animal.home.indoors.Value.objects.Pairs.ElementAt(i).Value.Name.Equals("Hay"))
                            {
                                actualFullness = byte.MaxValue;
                            }
                        }
                    }
                }

                int starvation = mod.CalculateStarvation(animal, actualFullness);

                if (mod.Config.DeathByStarvation && starvation >= mod.Config.DaysToDieDueToStarvation)
                {
                    mod.AnimalsToKill.Add(new Tuple<FarmAnimal, string>(animal, Cause.starvation.ToString()));
                    return true;
                }

                bool gotWater = false;

                if (mod.WaterMod != null)
                {
                    gotWater = !mod.WaterMod.WasAnimalLeftThirstyYesterday(animal);

                    int dehydration = mod.CalculateDehydration(animal, gotWater);

                    if (mod.Config.DeathByDehydrationWithAnimalsNeedWaterMod && dehydration >= mod.Config.DaysToDieDueToDehydrationWithAnimalsNeedWaterMod)
                    {
                        mod.AnimalsToKill.Add(new Tuple<FarmAnimal, string>(animal, Cause.dehydration.ToString()));
                        return true;
                    }
                }

                int illness = mod.CalculateIllness(animal, actualFullness, gotWater, wasLeftOutLastNight);

                if (mod.Config.DeathByIllness && illness >= mod.Config.IllnessScoreToDie)
                {
                    mod.AnimalsToKill.Add(new Tuple<FarmAnimal, string>(animal, Cause.illness.ToString()));
                    return true;
                }
                else if (illness >= mod.Config.IllnessScoreToDie / 2)
                {
                    mod.SickAnimals.Add(animal);
                }

                if (mod.Config.DeathByOldAge && mod.ShouldDieOfOldAge(animal))
                {
                    mod.AnimalsToKill.Add(new Tuple<FarmAnimal, string>(animal, Cause.oldAge.ToString()));
                    return true;
                }

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }
    }
}
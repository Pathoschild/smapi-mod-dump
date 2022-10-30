/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StarAmy/BreedingOverhaul
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Buildings;
using xTile.Dimensions;
using StardewModdingAPI;
using System.Reflection;
using StardewModdingAPI.Utilities;

namespace BreedingOverhaul
{
    public class IncubatorPatch
    {
        public class ParentAnimalData
        {
            public FarmAnimal? animal;
            public FarmAnimal? childAnimal;
        }

        public static FarmAnimal? CreateFarmAnimal(string type, string name = null, StardewValley.Buildings.Building building = null)
        {
            var ahm = Type.GetType("Paritee.StardewValley.Core.Characters.FarmAnimal, Paritee.StardewValley.Core");
            if (ahm == null)
            {
                ModEntry.MyMonitor.Log($"NO TYPE FOR FarmAnimal", LogLevel.Trace);
            }
            else
            {
                var m = ahm.GetMethod("CreateFarmAnimal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                return m.Invoke(null, new object?[] { type, ModEntry.MyHelper.Multiplayer.GetNewID(), name, building, 0L }) as FarmAnimal;
            }
            return null;
        }

        public static void RemovePregnancy(FarmAnimal animal)
        {
            var ahm = Type.GetType("AnimalHusbandryMod.animals.PregnancyController, AnimalHusbandryMod");
            var m = ahm.GetMethod("RemovePregnancy");
            m.Invoke(null, new object?[] { animal });
        }

        public static bool getIncubatorHatchEventPatch(StardewValley.AnimalHouse animalHouse, ref StardewValley.Event __result, string message = null)
        {
            var ahm = Type.GetType("Paritee.StardewValley.Core.Utilities.Content, Paritee.StardewValley.Core");
            var m = ahm.GetMethod("LoadString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            // Use the same messaging for all types of "eggs"
            string str = message ?? m.Invoke(null, new object?[] { "Strings\\Locations:AnimalHouse_Incubator_Hatch_RegularEgg" }) as string;

            ModEntry.MyMonitor.Log($"incubator hatch event, creating now", LogLevel.Debug);
            // __result = new global::StardewValley.Event("none/-1000 -1000/farmer 2 9 0/pause 250/message \"" + str + "\"/pause 500/animalNaming/pause 500/message \"Woah! " + str + "\"/pause 500/animalNaming/pause 500/end");
            __result = new global::StardewValley.Event("none/-1000 -1000/farmer 2 9 0/pause 250/message \"" + str + "\"/pause 500/animalNaming/pause 500/end");
            return false;
        }

        public static bool performObjectDropInActionPrefix(StardewValley.Object __instance, Item dropInItem, bool probe, Farmer who, ref int __state, ref bool __result)
        {
            ModEntry.MyMonitor.Log($"object drop in action patch! on {__instance.Name}, drop in {dropInItem.Name} probe is {probe}", LogLevel.Trace);
            if (__instance.Name.Equals("Incubator") && probe == false)
            {
                __state = new int();
                __state = dropInItem.Category;
                string dialogue = "";
                if (ModEntry.incubatorData.IncubatorItems.ContainsKey(dropInItem.Name))
                {
                    ModEntry.MyMonitor.Log($"making {dropInItem.Name} an egg catagory temporarily {__state} -> -5", LogLevel.Trace);
                    dropInItem.Category = -5;
                }
                else if (dropInItem.Category == -5)
                {
                    ModEntry.MyMonitor.Log($"making {dropInItem.Name} NOT an egg temporarily {__state} -> 0", LogLevel.Trace);
                    dropInItem.Category = 0;

                    dialogue = ModEntry.i18n.Get("BigCraftable.Incubator.NoHatch");
                }
                else
                {
                    ModEntry.MyMonitor.Log($"not in list or an egg", LogLevel.Trace);

                    dialogue = ModEntry.i18n.Get("BigCraftable.Incubator.NoHatch");
                }

                if (dialogue.Length > 0)
                {
                    if (who != null && Game1.player.Equals(who))
                    {
                        DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    }
                }
            }

            return true;
        }

        public static void performObjectDropInActionPostfix(StardewValley.Object __instance, Item dropInItem, bool probe, Farmer who, ref int __state, ref bool __result)
        {
            if (__instance.Name.Equals("Incubator") && __state == -5 && probe == false)
            {
                ModEntry.MyMonitor.Log($"restoring {dropInItem.Name} category {dropInItem.Category} -> {__state} -> 0", LogLevel.Trace);
                dropInItem.Category = __state;
            } else
            {
                ModEntry.MyMonitor.Log($" no work to be done postfix {dropInItem.Name} category {dropInItem.Category} into {__instance.Name}", LogLevel.Trace);
            }
        }

        public static bool addNewHatchedAnimalPrefix(string name, ref ParentAnimalData __state)
        {
            ModEntry.MyMonitor.Log($"add new hatched animal name: {name}", LogLevel.Trace);
            var ahm = Type.GetType("AnimalHusbandryMod.animals.PregnancyController, AnimalHusbandryMod");
            var paf = ahm.GetField("parentAnimal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            PerScreen<FarmAnimal> parentAnimal = paf.GetValue(null) as PerScreen<FarmAnimal>;
            __state = new ParentAnimalData();
            __state.animal = parentAnimal.Value;
            string customAnimal = ModEntry.pregnancyData.GetRandomOffspringType(parentAnimal.Value);
            if (customAnimal != "")
            {
                try {
                    Building building = parentAnimal.Value.home;
                    FarmAnimal farmAnimal = CreateFarmAnimal(customAnimal, name, building); 
                    ModEntry.MyMonitor.Log($"addNewHatchedAnimalPatch, custom animal type for parent type  {parentAnimal.Value.type.Value} is {customAnimal}", LogLevel.Trace);
                    ModEntry.MyMonitor.Log($"addNewHatchedAnimalPatch, child is type {farmAnimal.type.Value}", LogLevel.Trace);
                    __state.childAnimal = farmAnimal;
                    farmAnimal.parentId.Value = parentAnimal.Value.myID.Value;
                    farmAnimal.homeLocation.Value = new Vector2((float)building.tileX.Value, (float)building.tileY.Value);
                    farmAnimal.ownerID.Value = parentAnimal.Value.ownerID.Value != 0
                        ? parentAnimal.Value.ownerID.Value
                        : (long)Game1.player.UniqueMultiplayerID;
                    farmAnimal.setRandomPosition(farmAnimal.home.indoors.Value);
                    AnimalHouse animalHouse = (building.indoors.Value as AnimalHouse);
                    animalHouse?.animals.Add(farmAnimal.myID.Value, farmAnimal);
                    animalHouse?.animalsThatLiveHere.Add(farmAnimal.myID.Value);

                    parentAnimal.Value.allowReproduction.Value = false;
                    RemovePregnancy(parentAnimal.Value);
                }
                catch (Exception e)
                {
                    ModEntry.MyMonitor.Log($"Error while adding born baby '{name}'. The birth will be skipped.", LogLevel.Error);
                    ModEntry.MyMonitor.Log($"Message from birth error above: {e.Message}");
                }
                finally
                {
                    parentAnimal.Value = null;
                    Game1.exitActiveMenu();
                }

            }
            else
            {
                ModEntry.MyMonitor.Log($"addNewHatchedAnimalPatch, no custom animal for parent type {parentAnimal.Value.type.Value}", LogLevel.Trace);
            }

            return false;
        }

        public static bool getRandomTypeFromIncubatorPatch(StardewValley.Object incubator, Dictionary<string, List<string>> restrictions, ref string __result)
        {
            // get the egg type
            ModEntry.MyMonitor.Log($"getRandomTypePatch, this is used to return the type of animal based on egg.", LogLevel.Trace);
            if (ModEntry.incubatorData == null)
            {
                ModEntry.MyMonitor.Log($"getRandomTypePatch, no json loaded", LogLevel.Trace);
                return true;
            }
            if (incubator == null)
            {
                ModEntry.MyMonitor.Log($"getRandomTypePatch, no instance of object", LogLevel.Trace);
                return true;
            }
            StardewValley.Object egg = incubator.heldObject.Value;
            if (egg != null)
            {
                ModEntry.MyMonitor.Log($"getRandomTypePatch, input egg is a {egg.Name}, {egg.DisplayName}", LogLevel.Trace);

                if (ModEntry.incubatorData.IncubatorItems.ContainsKey(egg.Name))
                {
                    List<string> animals = ModEntry.incubatorData.IncubatorItems[egg.Name];
                    string animal = animals[new Random().Next(animals.Count)];
                    ModEntry.MyMonitor.Log($"getRandomTypePatch, input egg is a {egg.Name}, random animal is {animal}", LogLevel.Trace);
                    __result = animal;
                    return false;
                } else
                {
                    ModEntry.MyMonitor.Log($"getRandomTypePatch, no config for input egg {egg.Name}", LogLevel.Trace);
                }
            }
            else
            {
                ModEntry.MyMonitor.Log($"no egg in incubator", LogLevel.Trace);
            }
            return true;
        }

        public static bool getRandomTypeFromProducePatch(StardewValley.FarmAnimal animal, Dictionary<string, List<string>> restrictions, ref string __result)
        {
            ModEntry.MyMonitor.Log($"getRandomTypeFromProducePatch", LogLevel.Trace);
            if (animal == null || ModEntry.pregnancyData == null)
            {
                return true;
            }
            string customAnimal = ModEntry.pregnancyData.GetRandomOffspringType(animal);
            if (customAnimal == "")
            {
                ModEntry.MyMonitor.Log($"getRandomTypeFromProducePatch, custom animal is {customAnimal}", LogLevel.Trace);
                __result = customAnimal;
                return false;
            }
            return true;
        }

    }
}

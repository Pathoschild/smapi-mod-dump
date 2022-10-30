/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StarAmy/BreedingOverhaul
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace BreedingOverhaul
{
    internal class AnimalHusbandryModPatch
    {

        public static bool CheckCorrectProductPrefix(FarmAnimal animal, StardewValley.Object o, ref bool __result)
        {
            ModEntry.MyMonitor.Log($"CheckCorrectProduct prefix", LogLevel.Trace);
            if (animal == null || o == null) return true;

            if (ModEntry.pregnancyData.PreganancyItems.ContainsKey(animal.displayType))
            {
                __result = ModEntry.pregnancyData.MatchingPregnancyItem(animal, o);
                return false;
            }
            return true;
        }

        public static bool canThisBeAttachedPrefix(object[] __args, ref bool __result)
        {
            ModEntry.MyMonitor.Log($"canThisBeAttached prefix", LogLevel.Trace);

            Tool __instance = __args[0] as Tool;

            if (__args[1] == null)
            {
                ModEntry.MyMonitor.Log($"canThisBeAttached prefix on syringe, no object", LogLevel.Trace);
                return true;
            }
            StardewValley.Object o = __args[1] as StardewValley.Object;

            if (!__instance.modData.ContainsKey("DIGUS.ANIMALHUSBANDRYMOD/InseminationSyringe")) return true;

            ModEntry.MyMonitor.Log($"canThisBeAttached prefix on syringe", LogLevel.Trace);


            if (ModEntry.pregnancyData.PreganancyItems.ContainsValue(o.Name))
            {
                ModEntry.MyMonitor.Log($"canThisBeAttached prefix, found matching value, allow it", LogLevel.Trace);

                __result = true;
                return false;
            }
            ModEntry.MyMonitor.Log($"canThisBeAttached prefix, run rest of code", LogLevel.Trace);

            return true;
        }

        public static bool beginUsingPrefix(object[] __args, ref bool __result)
        {
            ModEntry.MyMonitor.Log($"beginUsing prefix", LogLevel.Trace);

            Tool __instance = __args[0] as Tool;
            string InseminationSyringeKey = "DIGUS.ANIMALHUSBANDRYMOD/InseminationSyringe";
            if (!__instance.modData.ContainsKey(InseminationSyringeKey)) return true;

            string inseminationSyringeId = __instance.modData[InseminationSyringeKey];

            GameLocation location = __args[1] as GameLocation;
            Dictionary<string, FarmAnimal> Animals = new Dictionary<string, FarmAnimal>();

            StardewValley.Farmer who = __args[4] as StardewValley.Farmer;
            int x = (int)who.GetToolLocation(false).X;
            int y = (int)who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize);

            if (location is Farm)
            {
                foreach (FarmAnimal farmAnimal in (location as Farm).animals.Values)
                {
                    if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                    {
                        Animals[inseminationSyringeId] = farmAnimal;
                        break;
                    }
                }
            }
            else if (location is AnimalHouse)
            {
                foreach (FarmAnimal farmAnimal in (location as AnimalHouse).animals.Values)
                {
                    if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                    {
                        Animals[inseminationSyringeId] = farmAnimal;
                        break;
                    }
                }
            }

            var ahm = Type.GetType("AnimalHusbandryMod.tools.InseminationSyringeOverrides, AnimalHusbandryMod");
            var m = ahm.GetMethod("CheckCorrectProduct", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            bool ret = true;

            Animals.TryGetValue(inseminationSyringeId, out FarmAnimal animal);
            if (animal != null)
            {
                if (animal.displayName.Contains("Male"))
                {
                    if (who != null && Game1.player.Equals(who))
                    {
                        string dialogue = ModEntry.i18n.Get("Tool.InseminationSyringe.CantBeInseminated", new { animalName = animal.displayName });
                        DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    }
                    Animals[inseminationSyringeId] = null;
                    ret =  false;
                }
                else if (__instance.attachments != null && __instance.attachments[0] != null && (m.Invoke(null, new object[]{ animal, __instance.attachments[0]}) as bool?) == false)
                {
                    string customProduceName = ModEntry.pregnancyData.GetPregnancyItemName(animal);
                    if (customProduceName != "")
                    {
                        if (who != null && Game1.player.Equals(who))
                        {
                            string dialogue = ModEntry.i18n.Get("Tool.InseminationSyringe.CorrectItem", new { itemName = customProduceName });
                            DelayedAction.showDialogueAfterDelay(dialogue, 150);
                        }
                        ret = false;
                    }
                }
            }

            if (!ret)
            {
                who.Halt();
                int currentFrame = who.FarmerSprite.currentFrame;
                if (animal != null)
                {
                    ModEntry.MyMonitor.Log($"beginUsing prefix, has animal", LogLevel.Trace);
                    who.FarmerSprite.animateOnce(287 + who.FacingDirection, 50f, 4);
                }
                else
                {
                    ModEntry.MyMonitor.Log($"beginUsing prefix, null animal", LogLevel.Trace);
                    who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(currentFrame, 0, false, who.FacingDirection == 3, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                }
                who.FarmerSprite.oldFrame = currentFrame;
                //who.UsingTool = false;
                //who.CanMove = false;

                __result = false;
            }

            ModEntry.MyMonitor.Log($"beginUsing prefix, returning {ret}", LogLevel.Trace);
            return ret;
        }

    }
}
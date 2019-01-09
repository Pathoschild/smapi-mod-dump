using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPots
{
    class ModEntry : Mod
    {
        private static ModConfig Config;
        private static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            // Read the config file for late use
            Config = this.Helper.ReadConfig<ModConfig>();
            ModMonitor = this.Monitor;

            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Get the methods we want to patch
            MethodInfo dayUpdateTargetMethod = AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate));
            MethodInfo checkForActionTargetMethod = AccessTools.Method(typeof(CrabPot), nameof(CrabPot.checkForAction));

            // Get the patches that was created
            MethodInfo dayUpdatePrefix = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.dayUpdatePrefix));
            MethodInfo checkForActionPrefix = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.checkForActionPrefix));

            // Apply the patches
            harmony.Patch(dayUpdateTargetMethod, prefix: new HarmonyMethod(dayUpdatePrefix));
            harmony.Patch(checkForActionTargetMethod, prefix: new HarmonyMethod(checkForActionPrefix));
        }

        private static bool dayUpdatePrefix(GameLocation location, ref CrabPot __instance)
        {
            // A strange issue was occuring where new numbers wouldn't be regenerated (the previos crabpot id was used) causing all crab pots to have the same object in it. This was the only way I could find to fix that from happening
            System.Threading.Thread.Sleep(250);

            // Check if the current crabpot has bait and requires it and doesn't already have an item to be collected
            if ((__instance.bait.Value == null && Config.RequiresBait) || __instance.heldObject.Value != null)
            {
                if (Config.EnablePassiveTrash)
                {
                    List<int> possiblePassiveTrash = new List<int>();

                    if (Config.WhatCanBeFoundAsPassiveTrash.Count() == 0)
                    {
                        possiblePassiveTrash.Add(168);
                        possiblePassiveTrash.Add(169);
                        possiblePassiveTrash.Add(170);
                        possiblePassiveTrash.Add(171);
                        possiblePassiveTrash.Add(172);
                    }
                    else
                    {
                        foreach (var item in Config.WhatCanBeFoundAsPassiveTrash)
                        {
                            for (int i = 0; i < item.Value; i++)
                            {
                                possiblePassiveTrash.Add(item.Key);
                            }
                        }
                    }

                    int percentChanceForPassiveTrash = Config.PercentChanceForPassiveTrash;

                    // Ensure the percent value is between 0 and 100
                    percentChanceForPassiveTrash = Math.Max(0, percentChanceForPassiveTrash);
                    percentChanceForPassiveTrash = Math.Min(100, percentChanceForPassiveTrash);

                    // Generate a random number to see if trash should be given (+1 to start with 1 instead of 0)
                    int randomValue = new Random().Next(100) + 1;

                    // If the percentage chance for trash is higher than the generated number, give them trash
                    if (percentChanceForPassiveTrash >= randomValue && percentChanceForPassiveTrash != 0)
                    {
                        int id = new Random().Next(possiblePassiveTrash.Count());
                        __instance.heldObject.Value = new StardewValley.Object(possiblePassiveTrash[id], 1, false, -1, 0);
                    }
                }

                return false;
            }

            __instance.tileIndexToShow = 714;
            __instance.readyForHarvest.Value = true;

            List<int> possibleItems = new List<int>();
            List<int> possibleTrash = new List<int>();

            // Get a list of possible stuff to find in the crabpot
            if (location is Beach)
            {
                if (Config.WhatCanBeFoundInOcean.Count() == 0)
                {
                    possibleItems.Add(715);
                    possibleItems.Add(327);
                    possibleItems.Add(717);
                    possibleItems.Add(718);
                    possibleItems.Add(719);
                    possibleItems.Add(720);
                    possibleItems.Add(723);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInOcean)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInOcean_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInOcean_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }

            }

            else if (location is Farm)
            {
                if (Config.WhatCanBeFoundInFarmLand.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInFarmLand)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInFarmLand_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInFarmLand_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location is Forest)
            {
                if (Config.WhatCanBeFoundInCindersapForest.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInCindersapForest)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInCindersapForest_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInCindersapForest_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location is Mountain)
            {
                if (Config.WhatCanBeFoundInMountainsLake.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMountainsLake)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInMountainsLake_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMountainsLake_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location is Town)
            {
                if (Config.WhatCanBeFoundInTown.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInTown)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInTown_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInTown_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location is MineShaft mine20 && mine20.mineLevel == 20)
            {
                if (Config.WhatCanBeFoundInMines_Layer20.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMines_Layer20)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInMines_Layer20_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMines_Layer20_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location is MineShaft mine60 && mine60.mineLevel == 60)
            {
                if (Config.WhatCanBeFoundInMines_Layer60.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMines_Layer60)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInMines_Layer60_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMines_Layer60_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location is MineShaft mine100 && mine100.mineLevel == 100)
            {
                if (Config.WhatCanBeFoundInMines_Layer100.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMines_Layer100)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInMines_Layer100_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMines_Layer100_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location.Name == "BugLand")
            {
                if (Config.WhatCanBeFoundInMutantBugLair.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMutantBugLair)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInMutantBugLair_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInMutantBugLair_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location.Name == "WitchSwamp")
            {
                if (Config.WhatCanBeFoundInWitchsSwamp.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInWitchsSwamp)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInWitchsSwamp_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInWitchsSwamp_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location is Woods)
            {
                if (Config.WhatCanBeFoundInSecretWoods.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInSecretWoods)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInSecretWoods_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInSecretWoods_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location is Desert)
            {
                if (Config.WhatCanBeFoundInDesert.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInDesert)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInDesert_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInDesert_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }

            else if (location is Sewer)
            {
                if (Config.WhatCanBeFoundInSewers.Count() == 0)
                {
                    possibleItems.Add(716);
                    possibleItems.Add(721);
                    possibleItems.Add(722);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInSewers)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleItems.Add(item.Key);
                        }
                    }
                }

                if (Config.WhatCanBeFoundInSewers_AsTrash.Count() == 0)
                {
                    possibleTrash.Add(168);
                    possibleTrash.Add(169);
                    possibleTrash.Add(170);
                    possibleTrash.Add(171);
                    possibleTrash.Add(172);
                }
                else
                {
                    foreach (var item in Config.WhatCanBeFoundInSewers_AsTrash)
                    {
                        for (int i = 0; i < item.Value; i++)
                        {
                            possibleTrash.Add(item.Key);
                        }
                    }
                }
            }
            
            // Check if trash is findable
            if (Config.EnableTrash)
            {
                int percentChanceForTrash = Config.PercentChanceForTrash;

                // Ensure the percent value is between 0 and 100
                percentChanceForTrash = Math.Max(0, percentChanceForTrash);
                percentChanceForTrash = Math.Min(100, percentChanceForTrash);

                // Generate a random number to see if trash should be given (+1 to start with 1 instead of 0)
                int randomValue = new Random().Next(100) + 1;

                // If the percentage chance for trash is higher than the generated number, give them trash
                if (percentChanceForTrash >= randomValue && percentChanceForTrash != 0)
                {
                    int id = new Random().Next(possibleTrash.Count());
                    __instance.heldObject.Value = new StardewValley.Object(possibleTrash[id], 1, false, -1, 0);
                }
            }

            // Check that no trash has been assigned to it, to give a non-trash item
            if (__instance.heldObject.Value == null)
            {
                bool isRing = false;
                int id = new Random().Next(possibleItems.Count());

                // Check if the item is a ring as a ring needs to be spawned differently to be wearable
                if (id >= 516 && id <= 534)
                {
                    isRing = true;
                }

                if (Config.EnableBetterQuality)
                {
                    int skillLevel = Game1.player.getEffectiveSkillLevel(1);
                    int quality = 0;

                    if (skillLevel > 0)
                    {
                        int randomValue = new Random().Next(skillLevel);

                        // Choose a quality based on the random number
                        if (randomValue >= 0 && randomValue <= 2)
                        {
                            quality = 0;
                        }
                        else if (randomValue >= 3 && randomValue <= 5)
                        {
                            quality = 1;
                        }
                        else if (randomValue >= 6 && randomValue <= 8)
                        {
                            quality = 2;
                        }
                        else
                        {
                            quality = 4;
                        }
                    }

                    __instance.heldObject.Value = new StardewValley.Object(possibleItems[id], 1, false, -1, quality);
                }
                else
                {
                    __instance.heldObject.Value = new StardewValley.Object(possibleItems[id], 1, false, -1, 0);
                }
            }

            ModMonitor.Log($"Crabpot contains item id: {__instance.heldObject.Value.ParentSheetIndex}", LogLevel.Trace);

            return false;
        }

        private static bool checkForActionPrefix(Farmer who, bool justCheckingForActivity, ref bool __result, ref CrabPot __instance)
        {
            if (__instance.tileIndexToShow == 714)
            {
                if (justCheckingForActivity)
                {
                    __result = true;
                    return false;
                }

                StardewValley.Object @object = __instance.heldObject.Value;
                __instance.heldObject.Value = (StardewValley.Object)null;

                if (who.IsLocalPlayer && !who.addItemToInventoryBool((Item)@object, false))
                {
                    __instance.heldObject.Value = @object;
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"), Color.Red, 3500f));

                    __result = false;
                    return false;
                }

                __instance.readyForHarvest.Value = false;
                __instance.tileIndexToShow = 710;
                //__instance.lidFlapping = true;
                //__instance.lidFlapTimer = 60f;
                __instance.bait.Value = (StardewValley.Object)null;

                who.animateOnce(279 + who.FacingDirection);
                who.currentLocation.playSound("fishingRodBend");
                DelayedAction.playSoundAfterDelay("coin", 500, (GameLocation)null);

                who.gainExperience(1, 5);
                //__instance.shake = Vector2.Zero;
                __instance.shakeTimer = 0;

                __result = true;
                return false;
            }

            if (__instance.bait.Value == null)
            {
                if (justCheckingForActivity)
                {
                    __result = true;
                    return false;
                }

                if (Game1.player.addItemToInventoryBool(__instance.getOne(), false))
                {
                    Game1.playSound("coin");
                    Game1.currentLocation.objects.Remove((Vector2)((NetFieldBase<Vector2, NetVector2>)__instance.tileLocation));

                    __result = true;
                    return false;
                }

                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }

            __result = false;
            return false;
        }
    }
}

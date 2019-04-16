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
        public static Random random;

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

            random = new Random();
        }

        private static bool dayUpdatePrefix(GameLocation location, ref CrabPot __instance)
        {
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
                    int randomValue = ModEntry.random.Next(100) + 1;

                    // If the percentage chance for trash is higher than the generated number, give them trash
                    if (percentChanceForPassiveTrash >= randomValue && percentChanceForPassiveTrash != 0)
                    {
                        int id = ModEntry.random.Next(possiblePassiveTrash.Count());
                        __instance.heldObject.Value = new StardewValley.Object(possiblePassiveTrash[id], 1, false, -1, 0);

                        ModMonitor.Log($"Crabpot contains item id: {__instance.heldObject.Value.ParentSheetIndex}", LogLevel.Trace);
                    }
                    else
                    {
                        ModMonitor.Log($"Crabpot contains item id: [null]", LogLevel.Trace);
                    }
                }
                
                return false;
            }

            __instance.tileIndexToShow = 714;
            __instance.readyForHarvest.Value = true;

            Dictionary<int, int> possibleItems = new Dictionary<int, int>();
            Dictionary<int, int> possibleTrash = new Dictionary<int, int>();
            
            // Get a list of possible stuff to find in the crabpot
            if (location is Beach)
            {
                if (Config.Beach.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(715, 1);
                    possibleItems.Add(327, 1);
                    possibleItems.Add(717, 1);
                    possibleItems.Add(718, 1);
                    possibleItems.Add(719, 1);
                    possibleItems.Add(720, 1);
                    possibleItems.Add(723, 1);
                }
                else
                {
                    foreach (var item in Config.Beach.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.Beach.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.Beach.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is Farm)
            {
                if (Config.FarmLand.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.FarmLand.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.FarmLand.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.FarmLand.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is Forest)
            {
                if (Config.CindersapForest.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.CindersapForest.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.CindersapForest.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.CindersapForest.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is Mountain)
            {
                if (Config.MountainsLake.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.MountainsLake.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.MountainsLake.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.MountainsLake.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is Town)
            {
                if (Config.Town.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.Town.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.Town.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.Town.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is MineShaft mine20 && mine20.mineLevel == 20)
            {
                if (Config.Mines_Layer20.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.Mines_Layer20.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.Mines_Layer20.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.Mines_Layer20.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is MineShaft mine60 && mine60.mineLevel == 60)
            {
                if (Config.Mines_Layer60.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.Mines_Layer60.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.Mines_Layer60.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.Mines_Layer60.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is MineShaft mine100 && mine100.mineLevel == 100)
            {
                if (Config.Mines_Layer100.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.Mines_Layer100.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.Mines_Layer100.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.Mines_Layer100.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location.Name == "BugLand")
            {
                if (Config.MutantBugLair.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.MutantBugLair.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.MutantBugLair.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.MutantBugLair.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location.Name == "WitchSwamp")
            {
                if (Config.WitchsSwamp.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.WitchsSwamp.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.WitchsSwamp.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.WitchsSwamp.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is Woods)
            {
                if (Config.SecretWoods.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.SecretWoods.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.SecretWoods.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.SecretWoods.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is Desert)
            {
                if (Config.Desert.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.Desert.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.Desert.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.Desert.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }
            }

            else if (location is Sewer)
            {
                if (Config.Sewers.WhatCanBeFound.Count() == 0 && Config.AllWater.WhatCanBeFound.Count() == 0)
                {
                    possibleItems.Add(716, 1);
                    possibleItems.Add(721, 1);
                    possibleItems.Add(722, 1);
                }
                else
                {
                    foreach (var item in Config.Sewers.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleItems.ContainsKey(item.Id))
                            {
                                possibleItems.Add(item.Id, item.Quantity);
                            }
                        }
                    }
                }

                if (Config.Sewers.WhatTrashCanBeFound.Count() == 0 && Config.AllWater.WhatTrashCanBeFound.Count() == 0)
                {
                    possibleTrash.Add(168, 1);
                    possibleTrash.Add(169, 1);
                    possibleTrash.Add(170, 1);
                    possibleTrash.Add(171, 1);
                    possibleTrash.Add(172, 1);
                }
                else
                {
                    foreach (var item in Config.Sewers.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
                        }
                    }

                    foreach (var item in Config.AllWater.WhatTrashCanBeFound)
                    {
                        for (int i = 0; i < item.Chance; i++)
                        {
                            if (!possibleTrash.ContainsKey(item.Id))
                            {
                                possibleTrash.Add(item.Id, item.Quantity);
                            }
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
                int randomValue = ModEntry.random.Next(100) + 1;

                // If the percentage chance for trash is higher than the generated number, give them trash
                if (percentChanceForTrash >= randomValue && percentChanceForTrash != 0)
                {
                    int id = ModEntry.random.Next(possibleTrash.Count());
                    
                    __instance.heldObject.Value = new StardewValley.Object(possibleTrash.Keys.ElementAt(id), possibleTrash.Values.ElementAt(id), false, -1, 0);

                    ModMonitor.Log($"Crabpot contains item id: {__instance.heldObject.Value.ParentSheetIndex}", LogLevel.Trace);
                }
            }

            // Check that no trash has been assigned to it, to give a non-trash item
            if (__instance.heldObject.Value == null)
            {
                bool isRing = false;
                int id = ModEntry.random.Next(possibleItems.Count());

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
                        int randomValue = ModEntry.random.Next(skillLevel);

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

                    __instance.heldObject.Value = new StardewValley.Object(possibleItems.Keys.ElementAt(id), possibleItems.Values.ElementAt(id), false, -1, quality);
                }
                else
                {
                    __instance.heldObject.Value = new StardewValley.Object(possibleItems.Keys.ElementAt(id), possibleItems.Values.ElementAt(id), false, -1, 0);
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
                
                // Check if the item is in Fish.xnb and that they are a fish to add to the collection screen
                Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                if (dictionary.ContainsKey((int)((NetFieldBase<int, NetInt>)@object.parentSheetIndex)))
                {
                    string[] strArray = dictionary[(int)((NetFieldBase<int, NetInt>)@object.parentSheetIndex)].Split('/');
                    
                    if (strArray[1] != "trap")
                    {
                        int minValue = strArray.Length > 5 ? Convert.ToInt32(strArray[3]) : 1;
                        int num = strArray.Length > 5 ? Convert.ToInt32(strArray[4]) : 10;
                        who.caughtFish((int)((NetFieldBase<int, NetInt>)@object.parentSheetIndex), Game1.random.Next(minValue, num + 1));
                    }
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

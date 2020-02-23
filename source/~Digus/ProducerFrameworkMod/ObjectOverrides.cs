using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using ProducerFrameworkMod.ContentPack;
using StardewValley;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod
{

    internal class ObjectOverrides
    {
        [HarmonyPriority(800)]
        internal static bool PerformObjectDropInAction(Object __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            if (__instance.isTemporarilyInvisible || !(dropInItem is Object))
                return false;
            Object input = (Object) dropInItem;

            if (ProducerController.GetProducerItem(__instance.name, input) is ProducerRule producerRule)
            {
                if (__instance.heldObject.Value != null && !__instance.name.Equals("Crystalarium") || input.bigCraftable.Value)
                {
                    return true;
                }
                if (ProducerRuleController.IsInputExcluded(producerRule, input))
                {
                    return true;
                }

                if (__instance.bigCraftable.Value && !probe && __instance.heldObject.Value == null)
                {
                    __instance.scale.X = 5f;
                }
                try
                {
                    ProducerConfig producerConfig = ProducerController.GetProducerConfig(__instance.Name);

                    GameLocation location = who.currentLocation;
                    if (producerConfig != null)
                    {
                        if (!producerConfig.CheckLocationCondition(location))
                        {
                            throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Condition.Location"));
                        }
                        if (!producerConfig.CheckSeasonCondition())
                        {
                            throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Condition.Season"));
                        }
                    }

                    ProducerRuleController.ValidateIfInputStackLessThanRequired(producerRule, input);
                    ProducerRuleController.ValidateIfAnyFuelStackLessThanRequired(producerRule, who);

                    OutputConfig outputConfig = ProducerRuleController.ProduceOutput(producerRule, __instance,
                        (i, q) => who.hasItemInInventory(i, q), who, location, producerConfig, input, probe);
                    if (outputConfig != null)
                    {
                        if (!probe)
                        {
                            foreach (var fuel in producerRule.FuelList)
                            {
                                RemoveItemsFromInventory(who, fuel.Item1, fuel.Item2);
                            }

                            foreach (var fuel in outputConfig.FuelList)
                            {
                                RemoveItemsFromInventory(who, fuel.Item1, fuel.Item2);
                            }

                            input.Stack -= producerRule.InputStack;
                            __result = input.Stack <= 0;
                        }
                        else
                        {
                            __result = true;
                        }
                    }
                }
                catch (RestrictionException e)
                {
                    __result = false;
                    if (e.Message != null && !probe && who.IsLocalPlayer)
                    {
                        Game1.showRedMessage(e.Message);
                    }
                }
                return false;
            }
            return true;
        }

        private static bool RemoveItemsFromInventory(Farmer farmer, int index, int stack)
        {
            if (farmer.hasItemInInventory(index, stack, 0))
            {
                for (int index1 = 0; index1 < farmer.items.Count; ++index1)
                {
                    if (farmer.items[index1] != null && farmer.items[index1] is Object object1 && (object1.ParentSheetIndex == index || object1.Category == index))
                    {
                        if (farmer.items[index1].Stack > stack)
                        {
                            farmer.items[index1].Stack -= stack;
                            return true;
                        }
                        stack -= farmer.items[index1].Stack;
                        farmer.items[index1] = (Item)null;
                    }
                    if (stack <= 0)
                        return true;
                }
            }
            return false;
        }

        internal static void checkForActionPostfix(Object __instance, bool justCheckingForActivity, bool __result)
        {
            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig && __instance.heldObject.Value == null && __instance.MinutesUntilReady <= 0)
            {
                __instance.showNextIndex.Value = false;
            }
        }

        internal static bool minutesElapsedPrefix(Object __instance, ref int minutes, GameLocation environment)
        {
            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig)
            {
                if (!producerConfig.CheckWeatherCondition())
                {
                    return false;
                }

                if (!producerConfig.CheckTimeCondition(ref minutes))
                {
                    return false;
                }
            }
            return true;
        }

        internal static void minutesElapsedPostfix(Object __instance)
        {
            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig && __instance.heldObject.Value != null && __instance.MinutesUntilReady <= 0)
            {
                __instance.showNextIndex.Value = producerConfig.AlternateFrameWhenReady;
            }
        }

        public static IEnumerable<CodeInstruction> draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            LinkedList<CodeInstruction> newInstructions = new LinkedList<CodeInstruction>(instructions);
            CodeInstruction codeInstruction = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Call && c.operand?.ToString() == "Microsoft.Xna.Framework.Vector2 getScale()");
            LinkedListNode<CodeInstruction> linkedListNode = newInstructions.Find(codeInstruction);
            if (linkedListNode != null && codeInstruction != null)
            {
                linkedListNode.Value = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ObjectOverrides), "getScale", new Type[] {typeof(Object)}));
            }
            return newInstructions;
        }

        public static Vector2 getScale(Object __instance)
        {
            if(ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig && __instance.MinutesUntilReady > 0 && __instance.heldObject.Value != null)
            {
                if (producerConfig.DisableBouncingAnimationWhileWorking)
                {
                    return Vector2.Zero;
                }
                else if (!producerConfig.CheckLocationCondition(Game1.currentLocation))
                {
                    return Vector2.Zero;
                }
                else if (!producerConfig.CheckSeasonCondition())
                {
                    return Vector2.Zero;
                }
                else if (!producerConfig.CheckWeatherCondition())
                {
                    return Vector2.Zero;
                }
                else if (producerConfig.WorkingTime != null)
                {
                    if (producerConfig.WorkingTime.Begin <= producerConfig.WorkingTime.End)
                    {
                        if (Game1.timeOfDay < producerConfig.WorkingTime.Begin || Game1.timeOfDay >= producerConfig.WorkingTime.End)
                        {
                            return Vector2.Zero;
                        }
                    
                    }
                    else
                    {
                        if (Game1.timeOfDay >= producerConfig.WorkingTime.End && Game1.timeOfDay < producerConfig.WorkingTime.Begin)
                        {
                            return Vector2.Zero;
                        }
                    }
                }
            }
            return __instance.getScale();
        }

        [HarmonyPriority(800)]
        internal static bool performDropDownAction(Object __instance, Farmer who, bool __result)
        {
            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig)
            {
                if (producerConfig.NoInputStartMode != null)
                {
                    try
                    {
                        if (!producerConfig.CheckLocationCondition(who.currentLocation))
                        {
                            throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Condition.Location"));
                        }
                        else if (producerConfig.CheckSeasonCondition() && NoInputStartMode.Placement == producerConfig.NoInputStartMode)
                        {
                            if (ProducerController.GetProducerItem(__instance.Name, null) is ProducerRule producerRule)
                            {
                                ProducerRuleController.ProduceOutput(producerRule, __instance, (i, q) => who.hasItemInInventory(i, q), who, who.currentLocation, producerConfig);
                            }
                        }
                    }
                    catch (RestrictionException e)
                    {
                        if (e.Message != null && who.IsLocalPlayer)
                        {
                            Game1.showRedMessage(e.Message);
                        }
                    }
                    return __result = false;
                }
            }
            return true;
        }

        internal static bool checkForActionPrefix(Object __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            if (__instance.isTemporarilyInvisible
                || !__instance.readyForHarvest.Value
                || justCheckingForActivity)
            {
                return true;
            }

            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig)
            {
                if (producerConfig.NoInputStartMode != null || producerConfig.IncrementStatsOnOutput.Count > 0)
                {
                    Object previousObject = __instance.heldObject.Value;
                    if (who.isMoving())
                    {
                        Game1.haltAfterCheck = false;
                    }
                    __instance.heldObject.Value = (Object)null;
                    if (who.IsLocalPlayer)
                    {
                        if (!who.addItemToInventoryBool((Item)previousObject, false))
                        {
                            __instance.heldObject.Value = previousObject;
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                            return __result = false;
                        }
                        Game1.playSound("coin");
                        foreach (KeyValuePair<StardewStats, string> keyValuePair in producerConfig.IncrementStatsOnOutput)
                        {
                            if (keyValuePair.Value == null
                                || keyValuePair.Value == previousObject.Name
                                || keyValuePair.Value == previousObject.ParentSheetIndex.ToString()
                                || keyValuePair.Value == previousObject.Category.ToString()
                                || previousObject.HasContextTag(keyValuePair.Value))
                            {
                                StatsController.IncrementStardewStats(keyValuePair.Key, previousObject.Stack);
                                if (!producerConfig.MultipleStatsIncrement)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    __instance.readyForHarvest.Value = false;
                    __instance.showNextIndex.Value = false;
                    __result = true;
                    if (producerConfig.NoInputStartMode == NoInputStartMode.Placement)
                    {
                        if (ProducerController.GetProducerItem(__instance.Name, null) is ProducerRule producerRule)
                        {
                            try { 
                                if (!producerConfig.CheckLocationCondition(who.currentLocation))
                                {
                                    throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Condition.Location"));
                                }
                                else if(producerConfig.CheckSeasonCondition())
                                {
                                    __result = ProducerRuleController.ProduceOutput(producerRule, __instance, (i, q) => who.hasItemInInventory(i, q), who, who.currentLocation, producerConfig) != null;
                                }
                            }
                            catch (RestrictionException e)
                            {
                                __result = false;
                                if (e.Message != null && who.IsLocalPlayer)
                                {
                                    Game1.showRedMessage(e.Message);
                                }
                            }
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        [HarmonyPriority(Priority.First)]
        public static bool DayUpdate(Object __instance, GameLocation location)
        {
            if (__instance.bigCraftable.Value)
            {
                if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig)
                {
                    if (ProducerController.GetProducerItem(__instance.Name, null) is ProducerRule producerRule)
                    {
                        if (!producerConfig.CheckSeasonCondition() || ! producerConfig.CheckLocationCondition(location))
                        {
                            ProducerRuleController.ClearProduction(__instance);
                            return false;
                        }
                        else if (producerConfig.NoInputStartMode != null)
                        {
                            if (producerConfig.NoInputStartMode == NoInputStartMode.DayUpdate || (producerConfig.NoInputStartMode == NoInputStartMode.Placement))
                            {
                                if (__instance.heldObject.Value == null)
                                {
                                    try
                                    {
                                        Farmer who = Game1.getFarmer((long)__instance.owner);
                                        ProducerRuleController.ProduceOutput(producerRule, __instance, (i, q) => who.hasItemInInventory(i, q), who, who.currentLocation, producerConfig);
                                    }
                                    catch (RestrictionException)
                                    {
                                        //Does not show the restriction error since the machine is auto-starting.
                                    }
                                }
                            }
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal static bool LoadDisplayName(Object __instance, ref string __result)
        {
            if (NameUtils.HasCustomNameForIndex(__instance.ParentSheetIndex) && !__instance.preserve.Value.HasValue && __instance.ParentSheetIndex != 463 && __instance.ParentSheetIndex != 464)
            {
                IDictionary<int, string> objects = Game1.objectInformation;
                string translation = NameUtils.GetCustomNameFromIndex(__instance.ParentSheetIndex);
                
                if (objects.TryGetValue(__instance.ParentSheetIndex, out var instanceData) && ObjectUtils.GetObjectParameter(instanceData, (int)ObjectParameter.Name) != __instance.Name)
                {
                    if (translation.Contains("{outputName}"))
                    {
                        translation = translation.Replace("{outputName}",ObjectUtils.GetObjectParameter(instanceData, (int) ObjectParameter.DisplayName));
                    }
                }
                else
                {
                    __result = __instance.Name;
                    return false;
                }
                
                if (translation.Contains("{inputName}"))
                {
                    if (objects.TryGetValue(__instance.preservedParentSheetIndex.Value, out var preservedData))
                    {
                        translation = translation.Replace("{inputName}", ObjectUtils.GetObjectParameter(preservedData,(int)ObjectParameter.DisplayName));
                    }
                    else
                    {
                        __result = __instance.Name;
                        return false;
                    }
                }
                if (translation.Contains("{farmName}"))
                {
                    translation = translation.Replace("{farmName}", Game1.player.farmName.Value);
                }
                if (translation.Contains("{farmerName}"))
                {
                    string farmerName = Game1.getAllFarmers().FirstOrDefault(f => __instance.Name.Contains(f.name))?.Name ?? Game1.player.Name;
                    translation = translation.Replace("{farmerName}", farmerName);
                }
                __result = translation;
                return false;
            }
            return true;
        }

        internal static bool initializeLightSource(Object __instance, Vector2 tileLocation)
        {
            if (__instance.bigCraftable.Value)
            {
                if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig && producerConfig.LightSource is ContentPack.LightSourceConfig lightSourceConfig)
                {
                    if (__instance.minutesUntilReady > 0 || lightSourceConfig.AlwaysOn)
                    {
                        LightSourceConfigController.CreateLightSource(__instance, tileLocation, lightSourceConfig);
                    }
                    return false;
                }
            }
            return true;
        }
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProducerFrameworkMod.ContentPack;
using ProducerFrameworkMod.Controllers;
using ProducerFrameworkMod.Utils;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using Object = StardewValley.Object;
using StardewValley.GameData.Machines;
#pragma warning disable IDE1006

namespace ProducerFrameworkMod
{

    internal class ObjectOverrides
    {
        [HarmonyPriority(Priority.First)]
        internal static bool PerformObjectDropInAction(Object __instance, Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed, ref bool __result)
        {
            if (__instance.isTemporarilyInvisible || dropInItem is not Object input)
                return false;

            bool failLocationCondition = false;
            bool failSeasonCondition = false;

            if (__instance.heldObject.Value != null && !__instance.name.Equals("Crystalarium") || input.bigCraftable.Value)
            {
                return true;
            }

            ProducerConfig producerConfig = ProducerController.GetProducerConfig(__instance.QualifiedItemId);

            GameLocation location = who.currentLocation;
            if (producerConfig != null)
            {
                if (!producerConfig.CheckLocationCondition(location))
                {
                    failLocationCondition = true;
                }
                if (!producerConfig.CheckSeasonCondition(location))
                {
                    failSeasonCondition = true;
                }
                if (producerConfig.NoInputStartMode != null)
                {
                    return false;
                }
            }

            if (ProducerController.GetProducerItem(__instance.QualifiedItemId, input) is ProducerRule producerRule)
            {
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
                    if (failLocationCondition)
                    {
                        throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Condition.Location"));
                    }
                    if (failSeasonCondition)
                    {
                        throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Condition.Season"));
                    }

                    ProducerRuleController.ValidateIfInputStackLessThanRequired(producerRule, input, probe);
                    ProducerRuleController.ValidateIfAnyFuelStackLessThanRequired(producerRule, who, probe);

                    OutputConfig outputConfig = ProducerRuleController.ProduceOutput(producerRule, __instance,
                        (i, q) => who.getItemCountInList(Object.autoLoadFrom ?? who.Items, i) >= q, who, location, producerConfig, input, probe);
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

                            Object.ConsumeInventoryItem(who,input, outputConfig.RequiredInputStack ?? producerRule.InputStack);
                            __result = !returnFalseIfItemConsumed;
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
                    if (e.ShowMessage && e.Message != null && !probe && who.IsLocalPlayer && Object.autoLoadFrom == null)
                    {
                        Game1.showRedMessage(e.Message);
                    }
                }
                return false;
            }
            return !failLocationCondition && !failSeasonCondition;
        }

        private static bool RemoveItemsFromInventory(Farmer farmer, string index, int stack)
        {
            IInventory inventory = (Object.autoLoadFrom ?? farmer.Items);
            if (farmer.getItemCountInList(inventory,index) >= stack)
            {
                for (int index1 = 0; index1 < inventory.Count; ++index1)
                {
                    if (inventory[index1] != null && inventory[index1] is Object object1 && (object1.QualifiedItemId == index || (int.TryParse(index, out int c) && object1.Category == c)))
                    {
                        if (inventory[index1].Stack > stack)
                        {
                            inventory[index1].Stack -= stack;
                            return true;
                        }
                        stack -= inventory[index1].Stack;
                        inventory[index1] = (Item)null;
                    }
                    if (stack <= 0)
                        return true;
                }
            }
            return false;
        }

        internal static void checkForActionPostfix(Object __instance, Farmer who, bool justCheckingForActivity, bool __result, bool __state)
        {
            if (ProducerController.GetProducerConfig(__instance.QualifiedItemId) is { } producerConfig && __instance.heldObject.Value == null)
            {
                if (__instance.MinutesUntilReady <= 0)
                {
                    __instance.showNextIndex.Value = false;
                }

                if (!__state && !justCheckingForActivity && __result && producerConfig.LightSource?.AlwaysOn == true)
                {
                    int identifier = LightSourceConfigController.GenerateIdentifier(__instance.TileLocation);
                    if (who.currentLocation.hasLightSource(identifier))
                    {
                        who.currentLocation.removeLightSource(identifier);
                        __instance.initializeLightSource(__instance.TileLocation);
                    }
                }
            }
        }

        internal static bool minutesElapsedPrefix(Object __instance, ref int minutes, out bool __state)
        {
            GameLocation environment = __instance.Location;
            __state = false;
            if (ProducerController.GetProducerConfig(__instance.QualifiedItemId) is { } producerConfig)
            {
                if (!producerConfig.CheckWeatherCondition())
                {
                    return false;
                }

                if (!producerConfig.CheckElapsedTimeCondition(ref minutes))
                {
                    return false;
                }

                if (producerConfig.ProducerQualifiedItemId == "(BC)10" && producerConfig.WorkingOutdoors != true)
                {
                    if (Game1.IsMasterGame)
                    {
                        __instance.MinutesUntilReady -= minutes;
                    }
                    if (__instance.MinutesUntilReady <= 0)
                    {
                        if (!__instance.readyForHarvest.Value)
                        {
                            environment.playSound("dwop");
                        }
                        __instance.readyForHarvest.Value = true;
                        __instance.MinutesUntilReady = 0;
                        __instance.onReadyForHarvest();
                        __instance.showNextIndex.Value = true;
                        if (__instance.lightSource != null)
                        {
                            environment.removeLightSource(__instance.lightSource.Identifier);
                            __instance.lightSource = (LightSource)null;
                        }
                    }
                    if (!__instance.readyForHarvest.Value && Game1.random.NextDouble() < 0.33)
                    {
                        __instance.addWorkingAnimation();
                    }
                    return false;
                }

                if (producerConfig.LightSource?.AlwaysOn == true && __instance.MinutesUntilReady - minutes <= 0 && __instance.heldObject.Value != null && !__instance.readyForHarvest.Value)
                {
                    __state = true;
                }
            }
            return true;
        }

        internal static void minutesElapsedPostfix(Object __instance, bool __state)
        {
            if (ProducerController.GetProducerConfig(__instance.QualifiedItemId) is { } producerConfig && __instance.heldObject.Value != null && __instance.MinutesUntilReady <= 0)
            {
                __instance.showNextIndex.Value = producerConfig.AlternateFrameWhenReady;
            }
            if (__state)
            {
                __instance.initializeLightSource(__instance.TileLocation);
            }
        }

        public static IEnumerable<CodeInstruction> draw_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            LinkedList<CodeInstruction> newInstructions = new(instructions);
            CodeInstruction codeInstruction = newInstructions.FirstOrDefault(c => c.opcode == OpCodes.Callvirt && c.operand?.ToString() == "Microsoft.Xna.Framework.Vector2 getScale()");
            LinkedListNode<CodeInstruction> linkedListNode = newInstructions.Find(codeInstruction);
            if (linkedListNode != null && codeInstruction != null)
            {
                linkedListNode.Value = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ObjectOverrides), "getScale", new Type[] {typeof(Object)}));
            }

            codeInstruction = newInstructions.LastOrDefault(c => c.opcode == OpCodes.Callvirt && c.operand?.ToString() == "Void Draw(Microsoft.Xna.Framework.Graphics.Texture2D, Microsoft.Xna.Framework.Rectangle, System.Nullable`1[Microsoft.Xna.Framework.Rectangle], Microsoft.Xna.Framework.Color, Single, Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Graphics.SpriteEffects, Single)");
            linkedListNode = newInstructions.Find(codeInstruction);
            if (linkedListNode != null && codeInstruction != null)
            {
                newInstructions.AddBefore(linkedListNode, new CodeInstruction(OpCodes.Ldarg_0, null));
                linkedListNode.Value = new CodeInstruction(OpCodes.Call, AccessTools.FirstMethod(typeof(ObjectOverrides), m=> m.Name == "DrawAnimation"));
            }

            return newInstructions;
        }

        public static Vector2 getScale(Object __instance)
        {
            if(ProducerController.GetProducerConfig(__instance.QualifiedItemId) is { } producerConfig && __instance.MinutesUntilReady > 0 && __instance.heldObject.Value != null)
            {
                if (producerConfig.DisableBouncingAnimationWhileWorking)
                {
                    return Vector2.Zero;
                }
                else if (!producerConfig.CheckSeasonCondition(Game1.currentLocation))
                {
                    return Vector2.Zero;
                }
                else if (!producerConfig.CheckWeatherCondition())
                {
                    return Vector2.Zero;
                }
                else if (!producerConfig.CheckCurrentTimeCondition())
                {
                    return Vector2.Zero;
                }
            }
            return __instance.getScale();
        }

        public static void DrawAnimation(
            SpriteBatch spriteBatch,
            Texture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteEffects effects,
            float layerDepth,
            Object producer)
        {
            if (producer.heldObject.Value != null && ProducerController.GetProducerConfig(producer.QualifiedItemId) is ProducerConfig producerConfig)
            {
                if (producerConfig.ProducingAnimation is { } producingAnimation && producer.MinutesUntilReady > 0 && producerConfig.CheckSeasonCondition(Game1.currentLocation) && producerConfig.CheckWeatherCondition() && producerConfig.CheckCurrentTimeCondition())
                {
                    List<int> animationList;
                    if (producingAnimation.AdditionalAnimationsId.ContainsKey(producer.heldObject.Value.ItemId)) 
                    {
                        animationList = producingAnimation.AdditionalAnimationsId[producer.heldObject.Value.ItemId];
                    } 
                    else if (producingAnimation.AdditionalAnimationsId.ContainsKey(producer.heldObject.Value.Category.ToString()))
                    {
                        animationList = producingAnimation.AdditionalAnimationsId[producer.heldObject.Value.Category.ToString()];
                    } 
                    else
                    {
                        animationList = producingAnimation.RelativeFrameIndex;
                    }
                    if (animationList.Any())
                    {
                        int frame = animationList[((Game1.ticks + GetLocationSeed(producer.TileLocation)) % (animationList.Count * producingAnimation.FrameInterval)) / producingAnimation.FrameInterval];
                        spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(Object.getSourceRectForBigCraftable(texture,producer.ParentSheetIndex + frame)), color, rotation, origin, effects, layerDepth);
                        return;
                    }
                }
                else if (producerConfig.ReadyAnimation is { } readyAnimation && producer.readyForHarvest.Value)
                {
                    List<int> animationList;
                    if (readyAnimation.AdditionalAnimationsId.TryGetValue(producer.heldObject.Value.ItemId, out var value1))
                    {
                        animationList = value1;
                    }
                    else if (readyAnimation.AdditionalAnimationsId.TryGetValue(producer.heldObject.Value.Category.ToString(), out var value2))
                    {
                        animationList = value2;
                    }
                    else
                    {
                        animationList = readyAnimation.RelativeFrameIndex;
                    }
                    if (animationList.Any())
                    {
                        int frame = animationList[((Game1.ticks + GetLocationSeed(producer.TileLocation)) % (animationList.Count * readyAnimation.FrameInterval)) / readyAnimation.FrameInterval];
                        spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(Object.getSourceRectForBigCraftable(texture,producer.ParentSheetIndex + frame)), color, rotation, origin, effects, layerDepth);
                        return;
                    }
                }
            }
            spriteBatch.Draw(texture,destinationRectangle,sourceRectangle,color,rotation,origin,effects,layerDepth);
            
        }

        private static int GetLocationSeed(Vector2 tileLocation)
        {
            return (int)tileLocation.X * (int)tileLocation.X * 13 + (int)tileLocation.Y * (int)tileLocation.Y * 1019;
        }

        [HarmonyPriority(Priority.First)]
        internal static bool performDropDownAction(Object __instance, Farmer who, bool __result)
        {
            if (ProducerController.GetProducerConfig(__instance.QualifiedItemId) is { } producerConfig)
            {
                try
                {
                    if (!producerConfig.CheckLocationCondition(who.currentLocation))
                    {
                        throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Condition.Location"));
                    }
                    if (producerConfig.NoInputStartMode != null)
                    {
                        if (producerConfig.CheckSeasonCondition(who.currentLocation) && NoInputStartMode.Placement == producerConfig.NoInputStartMode)
                        {
                            if (ProducerController.GetProducerItem(__instance.QualifiedItemId, null) is { } producerRule)
                            {
                                ProducerRuleController.ProduceOutput(producerRule, __instance, (i, q) => who.getItemCountInList(Object.autoLoadFrom ?? who.Items, i) >= q, who, who.currentLocation, producerConfig);
                            }
                        }
                        return __result = false;
                    }
                }
                catch (RestrictionException e)
                {
                    if (e.ShowMessage && e.Message != null && who.IsLocalPlayer)
                    {
                        Game1.showRedMessage(e.Message);
                    }
                    return __result = false;
                }
            }
            return true;
        }

        internal static bool checkForActionPrefix(Object __instance, Farmer who, bool justCheckingForActivity, ref bool __result, out bool __state)
        {
            __state = false;
            if (__instance.isTemporarilyInvisible
                || !__instance.readyForHarvest.Value
                || justCheckingForActivity)
            {
                return true;
            }

            ProducerRuleController.PrepareOutput(__instance, who.currentLocation, who);

            if (ProducerController.GetProducerConfig(__instance.QualifiedItemId) is { } producerConfig)
            {
                if (producerConfig.NoInputStartMode != null || producerConfig.IncrementStatsOnOutput.Count > 0 || producerConfig.IncrementStatsLabelOnOutput.Count > 0)
                {
                    if (who.isMoving())
                    {
                        Game1.haltAfterCheck = false;
                    }
                    bool check_for_reload = false;
                    Object previousObject = __instance.heldObject.Value;
                    if (who.IsLocalPlayer)
                    {
                        __instance.heldObject.Value = (Object)null;
                        if (!who.addItemToInventoryBool(previousObject))
                        {
                            __instance.heldObject.Value = previousObject;
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                            return __result = false;
                        }
                        Game1.playSound("coin");
                        check_for_reload = true;
                        producerConfig.IncrementStats(previousObject);
                    }
                    ProducerRuleController.ClearProduction(__instance, who.currentLocation);
                    __state = true;
                    __result = true;
                    if (producerConfig.NoInputStartMode == NoInputStartMode.Placement)
                    {
                        if (ProducerController.GetProducerItem(__instance.QualifiedItemId, null) is { } producerRule)
                        {
                            try { 
                                if (!producerConfig.CheckLocationCondition(who.currentLocation))
                                {
                                    throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Condition.Location"));
                                }
                                else if(producerConfig.CheckSeasonCondition(who.currentLocation))
                                {
                                    __result = ProducerRuleController.ProduceOutput(producerRule, __instance, (i, q) => false, who, who.currentLocation, producerConfig) != null;
                                }
                            }
                            catch (RestrictionException e)
                            {
                                if (e.ShowMessage && e.Message != null && who.IsLocalPlayer)
                                {
                                    Game1.showRedMessage(e.Message);
                                }
                            }
                        }
                    }
                    MachineData machineData = __instance.GetMachineData();
                    if (machineData is { ExperienceGainOnHarvest: not null })
                    {
                        string[] expSplit = machineData.ExperienceGainOnHarvest.Split(' ');
                        for (int i = 0; i < expSplit.Length; i += 2)
                        {
                            int skill = Farmer.getSkillNumberFromName(expSplit[i]);
                            if (skill != -1 && ArgUtility.TryGetInt(expSplit, i + 1, out var amount, out var _))
                            {
                                who.gainExperience(skill, amount);
                            }
                        }
                    }
                    if (check_for_reload && producerConfig.NoInputStartMode == null)
                    {
                        __instance.AttemptAutoLoad(who);
                    }
                    return false;
                }
            }
            return true;
        }

        [HarmonyPriority(Priority.First)]
        public static bool DayUpdate(Object __instance)
        {
            GameLocation location = __instance.Location;
            if (__instance.bigCraftable.Value)
            {
                if (ProducerController.GetProducerConfig(__instance.QualifiedItemId) is ProducerConfig producerConfig)
                {
                    if (ProducerController.GetProducerItem(__instance.QualifiedItemId, null) is ProducerRule producerRule)
                    {
                        if (!producerConfig.CheckSeasonCondition(location) || ! producerConfig.CheckLocationCondition(location))
                        {
                            ProducerRuleController.ClearProduction(__instance, location);
                            return false;
                        }
                        else if (producerConfig.NoInputStartMode != null)
                        {
                            if (producerConfig.NoInputStartMode == NoInputStartMode.DayUpdate || (producerConfig.NoInputStartMode == NoInputStartMode.Placement))
                            {
                                if (__instance.GetMachineData()?.ClearContentsOvernightCondition == "TRUE")
                                {
                                    ProducerRuleController.ClearProduction(__instance, location);
                                }
                                if (__instance.heldObject.Value == null)
                                {
                                    try
                                    {
                                        Farmer who = Game1.getFarmer((long)__instance.owner.Value);
                                        ProducerRuleController.ProduceOutput(producerRule, __instance, (i, q) => who.getItemCountInList(Object.autoLoadFrom ?? who.Items, i) >= q, who, who.currentLocation, producerConfig);
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
            if (__instance.GetCustomName() is { } customName && !__instance.preserve.Value.HasValue && __instance.ParentSheetIndex != 463 && __instance.ParentSheetIndex != 464 && __instance.ParentSheetIndex != 340 )
            {
                string translation = customName;
                
                if (ItemRegistry.GetData(__instance.QualifiedItemId) is { } instanceData && instanceData.InternalName != __instance.Name)
                {
                    if (translation.Contains("{outputName}"))
                    {
                        translation = translation.Replace("{outputName}", instanceData.DisplayName);
                    }
                }
                else
                {
                    __result = __instance.Name;
                    return false;
                }
                
                if (translation.Contains("{inputName}"))
                {
                    if (__instance.preservedParentSheetIndex.Value == "-1")
                    {
                        translation = translation.Replace("{inputName}", __instance.GetGenericParentName());
                    }
                    else if (ItemRegistry.GetData(__instance.preservedParentSheetIndex.Value) is { } preservedData)
                    {
                        translation = translation.Replace("{inputName}", preservedData.DisplayName);
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
                    string farmerName = Game1.getAllFarmers().FirstOrDefault(f => __instance.Name.Contains(f.Name))?.Name ?? Game1.player.Name;
                    translation = translation.Replace("{farmerName}", farmerName);
                }
                Regex regex = new("[ ]{2,}", RegexOptions.None);

                __result = regex.Replace(translation, " ");
                return false;
            }
            return true;
        }

        internal static bool initializeLightSource(Object __instance, Vector2 tileLocation)
        {
            if (__instance.bigCraftable.Value)
            {
                if (ProducerController.GetProducerConfig(__instance.QualifiedItemId) is { LightSource: { } lightSourceConfig })
                {
                    if (__instance.MinutesUntilReady > 0 || lightSourceConfig.AlwaysOn)
                    {
                        LightSourceConfigController.CreateLightSource(__instance, tileLocation, lightSourceConfig);
                    }
                    return false;
                }
            }
            return true;
        }

        internal static bool TryApplyFairyDust(Object __instance, bool probe, ref bool __result)
        {
            if (__instance.GetMachineData() != null && !ProducerController.HasProducerConfig(__instance.QualifiedItemId)) return true;
            if (__instance.MinutesUntilReady > 0
                && ProducerController.GetProducerConfig(__instance.QualifiedItemId) is not { AllowFairyDust: false }
                && (ProducerController.HasProducerConfig(__instance.QualifiedItemId) || __instance.GetMachineData() is not { AllowFairyDust: false }))
            {
                if (!probe)
                {
                    Utility.addSprinklesToLocation(__instance.Location, (int)__instance.tileLocation.X,
                        (int)__instance.tileLocation.Y, 1, 2, 400, 40, Color.White);
                    Game1.playSound("yoba");
                    __instance.MinutesUntilReady = 10;
                    DelayedAction.functionAfterDelay(delegate { __instance.minutesElapsed(10); }, 50);
                }
                __result = true;
            }
            else
            {
                __result = false;
            }
            return false;
        }
    }
}
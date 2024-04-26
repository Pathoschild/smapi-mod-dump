/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using CombineMachines.Helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Machines;
using StardewValley.Inventories;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace CombineMachines.Patches
{
    public static class ProcessingPatches
    {
        private const string ModDataExecutingFunctionKey = "CombineMachines_ExecutingFunction";

        internal static void Entry(IModHelper helper, Harmony Harmony)
        {
            //  StardewValley.Object.OutputMachine is generally when the machine's heldObject and its minutesUntilReady are set
            //  So whenever this function is invoked, apply a postfix patch that will recalculate the heldObject.Stack or the minutesUntilReady, depending on the combined processing power of the machine
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.OutputMachine)),
                prefix: new HarmonyMethod(typeof(OutputMachinePatch), nameof(OutputMachinePatch.Prefix)),
                postfix: new HarmonyMethod(typeof(OutputMachinePatch), nameof(OutputMachinePatch.Postfix))
            );

            //  The logic we need in StardewValley.Object.OutputMachine's Postfix depends on which function is calling it. 
            //  If it's invoked from StardewValley.Object.PlaceInMachine, we must cap the processing power based on how many of the required inputs the player has.
            //  For example, if ProcessingPower=900% and player has 40 copper ore to insert into a furnace, the max multiplier is x8 instead of x9.
            //  The following patches just keep track of which calling function is executing when OutputMachine is invoked so that our postfix can implement different logic based on where its being called from
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performDropDownAction)),
                prefix: new HarmonyMethod(typeof(PerformDropDownActionPatch), nameof(PerformDropDownActionPatch.Prefix)),
                postfix: new HarmonyMethod(typeof(PerformDropDownActionPatch), nameof(PerformDropDownActionPatch.Postfix))
            );
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "CheckForActionOnMachine" /*nameof(SObject.CheckForActionOnMachine)*/),
                prefix: new HarmonyMethod(typeof(CheckForActionOnMachinePatch), nameof(CheckForActionOnMachinePatch.Prefix)),
                postfix: new HarmonyMethod(typeof(CheckForActionOnMachinePatch), nameof(CheckForActionOnMachinePatch.Postfix))
            );
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.PlaceInMachine)),
                prefix: new HarmonyMethod(typeof(PlaceInMachinePatch), nameof(PlaceInMachinePatch.Prefix)),
                postfix: new HarmonyMethod(typeof(PlaceInMachinePatch), nameof(PlaceInMachinePatch.Postfix))
            );
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.DayUpdate)),
                prefix: new HarmonyMethod(typeof(DayUpdatePatch), nameof(DayUpdatePatch.Prefix)),
                postfix: new HarmonyMethod(typeof(DayUpdatePatch), nameof(DayUpdatePatch.Postfix))
            );

            //  Tappers don't get their outputs from StardewValley.Object.OutputMachine
            //  so we need to patch StardewValley.TerrainFeatures.Tree.UpdateTapperProduct
            Harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.UpdateTapperProduct)),
                //prefix: new HarmonyMethod(typeof(UpdateTapperProductPatch), nameof(UpdateTapperProductPatch.Prefix)),
                postfix: new HarmonyMethod(typeof(UpdateTapperProductPatch), nameof(UpdateTapperProductPatch.Postfix))
            );
        }

        [HarmonyPatch(typeof(Tree), nameof(Tree.UpdateTapperProduct))]
        public static class UpdateTapperProductPatch
        {
            //public static bool Prefix(Tree __instance, SObject tapper, SObject previousOutput = null, bool onlyPerformRemovals = false)
            //{
            //    if (Game1.IsMasterGame)
            //    {
            //        ModEntry.Logger.Log($"{nameof(UpdateTapperProductPatch)}.{nameof(Prefix)}: {tapper.DisplayName} ({tapper.TileLocation})", ModEntry.InfoLogLevel);
            //        //__instance.modData[ModDataExecutingFunctionKey] = nameof(Tree.UpdateTapperProduct);
            //    }
            //    return true;
            //}

            public static void Postfix(Tree __instance, SObject tapper, SObject previousOutput = null, bool onlyPerformRemovals = false)
            {
                if (!Context.IsWorldReady)
                    return; // I guess UpdateTapperProduct is invoked while initially loading the game, which could cause tapper products to multiply on each save load

                if (Game1.IsMasterGame && tapper.TryGetCombinedQuantity(out int CombinedQty) && CombinedQty > 1 && tapper.heldObject.Value != null)
                {
                    //ModEntry.Logger.Log($"{nameof(UpdateTapperProductPatch)}.{nameof(Postfix)}: {tapper.DisplayName} ({tapper.TileLocation})", ModEntry.InfoLogLevel);
                    //_ = __instance.modData.Remove(ModDataExecutingFunctionKey);

                    const string ModDataKey = "CombineMachines_HasModifiedTapperOutput"; // After modifying the tapper product, set a flag to true so we don't accidentally keep modifying it and stack the effects
                    if (!tapper.heldObject.Value.modData.TryGetValue(ModDataKey, out string IsModifiedString) || !bool.TryParse(IsModifiedString, out bool IsModified) || !IsModified)
                    {
                        if (OutputMachinePatch.TryUpdateMinutesUntilReady(tapper, CombinedQty, out int PreviousMinutes, out int NewMinutes, out double DurationMultiplier))
                        {
                            ModEntry.Logger.Log($"{nameof(UpdateTapperProductPatch)}.{nameof(Postfix)}: " +
                                $"Set {tapper.DisplayName} MinutesUntilReady from {PreviousMinutes} to {NewMinutes} " +
                                $"({(DurationMultiplier * 100.0).ToString("0.##")}%, " +
                                $"Target value before weighted rounding = {(DurationMultiplier * PreviousMinutes).ToString("0.#")})", ModEntry.InfoLogLevel);

                            tapper.heldObject.Value.modData[ModDataKey] = "true";
                        }

                        if (ModEntry.UserConfig.ShouldModifyInputsAndOutputs(tapper))
                        {
                            double ProcessingPower = ModEntry.UserConfig.ComputeProcessingPower(CombinedQty);

                            int PreviousOutputStack = tapper.heldObject.Value.Stack;

                            double DesiredNewValue = PreviousOutputStack * Math.Max(1.0, ProcessingPower);
                            int NewOutputStack = RNGHelpers.WeightedRound(DesiredNewValue);

                            tapper.heldObject.Value.Stack = NewOutputStack;
                            ModEntry.LogTrace(CombinedQty, tapper, tapper.TileLocation, "HeldObject.Stack", PreviousOutputStack, DesiredNewValue, NewOutputStack, ProcessingPower);

                            tapper.heldObject.Value.modData[ModDataKey] = "true";
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SObject), nameof(SObject.performDropDownAction))]
        public static class PerformDropDownActionPatch
        {
            public static bool Prefix(SObject __instance, Farmer who)
            {
                if (Game1.IsMasterGame)
                {
                    //ModEntry.Logger.Log($"{nameof(PerformDropDownActionPatch)}.{nameof(Prefix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                    __instance.modData[ModDataExecutingFunctionKey] = nameof(SObject.performDropDownAction);
                }
                return true;
            }

            public static void Postfix(SObject __instance, Farmer who)
            {
                if (Game1.IsMasterGame)
                {
                    //ModEntry.Logger.Log($"{nameof(PerformDropDownActionPatch)}.{nameof(Postfix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                    _ = __instance.modData.Remove(ModDataExecutingFunctionKey);
                }
            }
        }

        [HarmonyPatch(typeof(SObject), "CheckForActionOnMachine" /*nameof(SObject.CheckForActionOnMachine)*/)]
        public static class CheckForActionOnMachinePatch
        {
            public static bool Prefix(SObject __instance, Farmer who, bool justCheckingForActivity = false)
            {
                if (Game1.IsMasterGame && !justCheckingForActivity)
                {
                    //ModEntry.Logger.Log($"{nameof(CheckForActionOnMachinePatch)}.{nameof(Prefix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                    __instance.modData[ModDataExecutingFunctionKey] = "CheckForActionOnMachine" /*nameof(SObject.CheckForActionOnMachine)*/;
                }
                return true;
            }

            public static void Postfix(SObject __instance, Farmer who, bool justCheckingForActivity = false)
            {
                if (Game1.IsMasterGame && !justCheckingForActivity)
                {
                    //ModEntry.Logger.Log($"{nameof(CheckForActionOnMachinePatch)}.{nameof(Postfix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                    _ = __instance.modData.Remove(ModDataExecutingFunctionKey);
                }
            }
        }

        [HarmonyPatch(typeof(SObject), nameof(SObject.PlaceInMachine))]
        public static class PlaceInMachinePatch
        {
            public static bool Prefix(SObject __instance, MachineData machineData, Item inputItem, bool probe, Farmer who, bool showMessages = true, bool playSounds = true)
            {
                if (Game1.IsMasterGame && !probe)
                {
                    //ModEntry.Logger.Log($"{nameof(PlaceInMachinePatch)}.{nameof(Prefix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                    __instance.modData[ModDataExecutingFunctionKey] = nameof(SObject.PlaceInMachine);
                }
                return true;
            }

            public static void Postfix(SObject __instance, MachineData machineData, Item inputItem, bool probe, Farmer who, bool showMessages = true, bool playSounds = true)
            {
                if (Game1.IsMasterGame && !probe)
                {
                    //ModEntry.Logger.Log($"{nameof(PlaceInMachinePatch)}.{nameof(Postfix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                    _ = __instance.modData.Remove(ModDataExecutingFunctionKey);
                }
            }
        }

        [HarmonyPatch(typeof(SObject), nameof(SObject.DayUpdate))]
        public static class DayUpdatePatch
        {
            public static bool Prefix(SObject __instance)
            {
                if (Game1.IsMasterGame && __instance.IsCombinedMachine())
                {
                    //ModEntry.Logger.Log($"{nameof(DayUpdatePatch)}.{nameof(Prefix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                    __instance.modData[ModDataExecutingFunctionKey] = nameof(SObject.DayUpdate);
                }
                return true;
            }

            public static void Postfix(SObject __instance)
            {
                if (Game1.IsMasterGame && __instance.IsCombinedMachine())
                {
                    //ModEntry.Logger.Log($"{nameof(DayUpdatePatch)}.{nameof(Postfix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                    _ = __instance.modData.Remove(ModDataExecutingFunctionKey);
                }
            }
        }

        [HarmonyPatch(typeof(SObject), nameof(SObject.OutputMachine))]
        public static class OutputMachinePatch
        {
            private static readonly IReadOnlyList<string> HandledCallerFunctions = new List<string>()
            { 
                nameof(SObject.performDropDownAction),
                nameof(SObject.DayUpdate),
                nameof(SObject.PlaceInMachine),
                "CheckForActionOnMachine", // nameof(SObject.CheckForActionOnMachine)
            };
            private static readonly string CoalQualifiedId = "(O)382";
            private static readonly string CrystalariumQualifiedId = "(BC)21";

            public static bool Prefix(SObject __instance, MachineData machine, MachineOutputRule outputRule, Item inputItem, Farmer who, GameLocation location, bool probe)
            {
                try
                {
                    if (!probe && Game1.IsMasterGame && __instance.TryGetCombinedQuantity(out int CombinedQty))
                    {
                        //ModEntry.Logger.Log($"{nameof(OutputMachinePatch)}.{nameof(Prefix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    ModEntry.Logger.Log($"Unhandled Error in {nameof(OutputMachinePatch)}.{nameof(Prefix)}:\n{ex}", LogLevel.Error);
                    return true;
                }
            }

            public static void Postfix(SObject __instance, MachineData machine, MachineOutputRule outputRule, Item inputItem, Farmer who, GameLocation location, bool probe)
            {
                try
                {
                    if (probe || !Game1.IsMasterGame || !__instance.TryGetCombinedQuantity(out int CombinedQty) || CombinedQty <= 1)
                        return;

                    //ModEntry.Logger.Log($"Begin {nameof(OutputMachinePatch)}.{nameof(Postfix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);

                    if (!__instance.modData.TryGetValue(ModDataExecutingFunctionKey, out string CallerName) || __instance.IsTapper())
                        return;

                    SObject Machine = __instance;
                    who ??= Game1.MasterPlayer;

                    if (TryUpdateMinutesUntilReady(Machine, CombinedQty, out int PreviousMinutes, out int NewMinutes, out double DurationMultiplier))
                    {
                        ModEntry.Logger.Log($"{nameof(OutputMachinePatch)}.{nameof(Postfix)}: " +
                            $"Set {Machine.DisplayName} MinutesUntilReady from {PreviousMinutes} to {NewMinutes} " +
                            $"({(DurationMultiplier * 100.0).ToString("0.##")}%, " +
                            $"Target value before weighted rounding = {(DurationMultiplier * PreviousMinutes).ToString("0.#")})", ModEntry.InfoLogLevel);
                    }

                    if (ModEntry.UserConfig.ShouldModifyInputsAndOutputs(Machine) && Machine.heldObject.Value != null && HandledCallerFunctions.Contains(CallerName))
                    {
                        //  Compute the output Stack multiplier
                        //  If the output item required no inputs, then the multiplier is equal to the machine's processing power.
                        //  Otherwise it might be capped depending on how many more of the input items the player has
                        double ActualProcessingPower;
                        switch (CallerName)
                        {
                            case nameof(SObject.performDropDownAction):
                            case nameof(SObject.DayUpdate):
                            case "CheckForActionOnMachine": // nameof(SObject.CheckForActionOnMachine)
                                if (inputItem != null)
                                {
                                    //  Crystalariums have an inputItem equal to whatever gem was inserted into it, but are a special-case because more inputs don't need to be consumed during this function
                                    //  (Extra inputs for Crystalariums only need to be consumed during the SObject.PlaceInMachine function)
                                    if (Machine.QualifiedItemId != CrystalariumQualifiedId)
                                        throw new Exception($"Calling {nameof(OutputMachinePatch)}.{nameof(Postfix)} from {CallerName}: Expected null input item for machine '{Machine.DisplayName}'. (Actual input item: {inputItem.DisplayName})");
                                }
                                ActualProcessingPower = ModEntry.UserConfig.ComputeProcessingPower(CombinedQty);
                                break;
                            case nameof(SObject.PlaceInMachine):
                                if (inputItem == null)
                                    throw new Exception($"Calling {nameof(OutputMachinePatch)}.{nameof(Postfix)} from {CallerName}: Expected non-null input item.");

                                //  Get the trigger rule being used to generate the output item
                                if (!MachineDataUtility.TryGetMachineOutputRule(__instance, machine, MachineOutputTrigger.ItemPlacedInMachine, inputItem, who, location,
                                    out MachineOutputRule rule, out MachineOutputTriggerRule triggerRule, out MachineOutputRule ruleIgnoringCount, out MachineOutputTriggerRule triggerIgnoringCount))
                                {
                                    return;
                                }

                                IInventory Inventory = SObject.autoLoadFrom ?? who.Items;
                                double MaxMultiplier = ModEntry.UserConfig.ComputeProcessingPower(CombinedQty);
                                bool MultiplyCoalInputs = ModEntry.UserConfig.FurnaceMultiplyCoalInputs;

                                //  Note: Some machines (such as Fish Smokers) don't require a specific input item, so the triggerrule's RequiredItemId would be null
                                string MainIngredientId = triggerRule.RequiredItemId ?? inputItem.QualifiedItemId;

                                //  Cap the multiplier based on how many of the main input item the player has.
                                //  EX: If inserting copper ore, a bar requires 5 ore. If player has 40 ore, the max multiplier cannot exceed 40/5=8.0
                                int MainInputQty = Inventory.CountId(MainIngredientId);
                                MaxMultiplier = Math.Min(MaxMultiplier, MainInputQty * 1.0 / triggerRule.RequiredCount);

                                //  Cap the multiplier based on how many of the secondary input item(s) the player has.
                                //  Typically this would be things like Coal for smelting bars or using the fish smoker.
                                if (machine.AdditionalConsumedItems != null)
                                {
                                    foreach (MachineItemAdditionalConsumedItems SecondaryIngredient in machine.AdditionalConsumedItems)
                                    {
                                        if (!MultiplyCoalInputs && SecondaryIngredient.ItemId == CoalQualifiedId)
                                            continue;

                                        int SecondaryInputQty = Inventory.CountId(SecondaryIngredient.ItemId);
                                        MaxMultiplier = Math.Min(MaxMultiplier, SecondaryInputQty * 1.0 / SecondaryIngredient.RequiredCount);
                                    }
                                }

                                ActualProcessingPower = MaxMultiplier;

                                //  Consume the extra inputs
                                if (ActualProcessingPower > 1.0)
                                {
                                    //  Consume main input
                                    Inventory.ReduceId(MainIngredientId, RNGHelpers.WeightedRound((ActualProcessingPower - 1.0) * triggerRule.RequiredCount)); // "(ActualProcessingPower - 1.0)" because 100% of inputs have already been consumed by vanilla game functions

                                    //  Consume secondary input(s)
                                    if (machine.AdditionalConsumedItems != null)
                                    {
                                        foreach (MachineItemAdditionalConsumedItems SecondaryIngredient in machine.AdditionalConsumedItems)
                                        {
                                            if (!MultiplyCoalInputs && SecondaryIngredient.ItemId == CoalQualifiedId)
                                                continue;

                                            Inventory.ReduceId(SecondaryIngredient.ItemId, RNGHelpers.WeightedRound((ActualProcessingPower - 1.0) * SecondaryIngredient.RequiredCount));
                                        }
                                    }
                                }
                                break;
                            default:
                                throw new NotImplementedException($"Calling {nameof(OutputMachinePatch)}.{nameof(Postfix)} from {CallerName}. Expected {nameof(CallerName)} to be one of the following: {string.Join(",", HandledCallerFunctions)}");
                        }

                        int PreviousOutputStack = Machine.heldObject.Value.Stack;

                        double DesiredNewValue = PreviousOutputStack * Math.Max(1.0, ActualProcessingPower);
                        int NewOutputStack = RNGHelpers.WeightedRound(DesiredNewValue);

                        Machine.heldObject.Value.Stack = NewOutputStack;
                        ModEntry.LogTrace(CombinedQty, Machine, Machine.TileLocation, "HeldObject.Stack", PreviousOutputStack, DesiredNewValue, NewOutputStack, ActualProcessingPower);
                    }

                    //ModEntry.Logger.Log($"End {nameof(OutputMachinePatch)}.{nameof(Postfix)}: {__instance.DisplayName} ({__instance.TileLocation})", ModEntry.InfoLogLevel);
                }
                catch (Exception ex)
                {
                    ModEntry.Logger.Log($"Unhandled Error in {nameof(MinutesElapsedPatch)}.{nameof(Postfix)}:\n{ex}", LogLevel.Error);
                }
            }

            public static bool TryUpdateMinutesUntilReady(SObject Machine) => TryUpdateMinutesUntilReady(Machine, Machine.TryGetCombinedQuantity(out int CombinedQty) ? CombinedQty : 1);
            public static bool TryUpdateMinutesUntilReady(SObject Machine, int CombinedQty) => TryUpdateMinutesUntilReady(Machine, CombinedQty, out _, out _, out _);
            public static bool TryUpdateMinutesUntilReady(SObject Machine, int CombinedQty, out int PreviousMinutes, out int NewMinutes, out double DurationMultiplier)
            {
                PreviousMinutes = Machine.MinutesUntilReady;
                NewMinutes = PreviousMinutes;
                DurationMultiplier = 1.0;

                if (ModEntry.UserConfig.ShouldModifyProcessingSpeed(Machine))
                {
                    DurationMultiplier = 1.0 / ModEntry.UserConfig.ComputeProcessingPower(CombinedQty);
                    double TargetValue = DurationMultiplier * PreviousMinutes;
                    NewMinutes = RNGHelpers.WeightedRound(TargetValue);

                    //  Round to nearest 10 since the game processes machine outputs every 10 game minutes
                    //  EX: If NewValue = 38, then there is a 20% chance of rounding down to 30, 80% chance of rounding up to 40
                    int SmallestDigit = NewMinutes % 10;
                    NewMinutes -= SmallestDigit; // Round down to nearest 10
                    if (RNGHelpers.RollDice(SmallestDigit / 10.0))
                        NewMinutes += 10; // Round up

                    //  There seems to be a bug where there is no product if the machine is instantly done processing.
                    NewMinutes = Math.Max(10, NewMinutes); // temporary fix - require at least one 10-minute processing cycle

                    if (NewMinutes < PreviousMinutes)
                    {
                        Machine.MinutesUntilReady = NewMinutes;
                        if (NewMinutes <= 0)
                            Machine.readyForHarvest.Value = true;

                        return true;
                    }
                }

                return false;
            }
        }
    }

    /// <summary>Intended to detect the moment that a CrabPots output is ready for collecting, and at that moment, apply the appropriate multiplier to the output item's stack size based on the machine's combined quantity.</summary>
    [HarmonyPatch(typeof(SObject), nameof(SObject.minutesElapsed))]
    public static class MinutesElapsedPatch
    {
        private static bool? WasReadyForHarvest = null;

        internal const int MinutesPerHour = 60;

        /// <summary>Returns the decimal number of hours between the given times (<paramref name="Time1"/> - <paramref name="Time2"/>).<para/>
        /// For example: 800-624 = 1hr36m=1.6 hours</summary>
        internal static double HoursDifference(int Time1, int Time2)
        {
            return Time1 / 100 - Time2 / 100 + (double)(Time1 % 100 - Time2 % 100) / MinutesPerHour;
        }

        /// <summary>Adds the given number of decimal hours to the given time, and returns the result in the same format that the game uses to store time (such as 650=6:50am)<para/>
        /// EX: AddHours(640, 1.5) = 810 (6:40am + 1.5 hours = 8:10am)</summary>
        internal static int AddHours(int Time, double Hours, bool RoundUpToMultipleOf10)
        {
            int OriginalHours = Time / 100;
            int HoursToAdd = (int)Hours;

            int OriginalMinutes = Time % 100;
            int MinutesToAdd = (int)((Hours - Math.Floor(Hours)) * MinutesPerHour);

            int TotalHours = OriginalHours + HoursToAdd;
            int TotalMinutes = OriginalMinutes + MinutesToAdd;
            if (RoundUpToMultipleOf10 && TotalMinutes % 10 != 0)
            {
                TotalMinutes += 10 - TotalMinutes % 10;
            }

            while (TotalMinutes >= MinutesPerHour)
            {
                TotalHours++;
                TotalMinutes -= MinutesPerHour;
            }

            return TotalHours * 100 + TotalMinutes;
        }

        /// <summary>The time of day that CrabPots should begin processing.</summary>
        private const int CrabPotDayStartTime = 600; // 6am
        /// <summary>The time of day that CrabPots should stop processing.</summary>
        private const int CrabPotDayEndTime = 2400; // 12am midnight (I know you can technically stay up until 2600=2am, but it seems unfair to the player to force them to stay up that late to collect from their crab pots)
        internal static readonly double CrabPotHoursPerDay = HoursDifference(CrabPotDayEndTime, CrabPotDayStartTime);

        public static bool Prefix(SObject __instance, int minutes)
        {
            try
            {
                if (__instance is CrabPot CrabPotInstance && CrabPotInstance.IsCombinedMachine() && ModEntry.UserConfig.ShouldModifyProcessingSpeed(CrabPotInstance))
                {
                    if (Game1.newDay)
                    {
                        CrabPotInstance.TryGetProcessingInterval(out double Power, out double IntervalHours, out int IntervalMinutes);
                        CrabPot_DayUpdatePatch.InvokeDayUpdate(CrabPotInstance);
                        ModEntry.Logger.Log($"Forced {nameof(CrabPot)}.{nameof(CrabPot.DayUpdate)} to execute at start of a new day for {nameof(CrabPot)} with Power={(Power * 100).ToString("0.##")}% (Interval={IntervalMinutes})", ModEntry.InfoLogLevel);
                    }
                    else
                    {
                        int CurrentTime = Game1.timeOfDay;
                        if (CurrentTime >= CrabPotDayStartTime && CurrentTime < CrabPotDayEndTime)
                        {
                            CrabPotInstance.TryGetProcessingInterval(out double Power, out double IntervalHours, out int IntervalMinutes);

                            //  Example:
                            //  If Power = 360% (3.6), and the crab pot can process items from 6am to 12am (18 hours), then we'd want to call DayUpdate once every 18/3.6=5.0 hours.
                            //  So the times to check for would be 600 (6am), 600+500=1100 (11am), 600+500+500=1600 (4pm), 600+500+500+500=2100 (9pm)
                            int Time = CrabPotDayStartTime;
                            while (Time <= CurrentTime)
                            {
                                if (CurrentTime == Time)
                                {
                                    CrabPot_DayUpdatePatch.InvokeDayUpdate(CrabPotInstance);
                                    ModEntry.Logger.Log($"Forced {nameof(CrabPot)}.{nameof(CrabPot.DayUpdate)} to execute at Time={CurrentTime} for {nameof(CrabPot)} with Power={(Power * 100).ToString("0.##")}% (Interval={IntervalMinutes}", ModEntry.InfoLogLevel);
                                    break;
                                }
                                else
                                    Time = AddHours(Time, IntervalHours, true);
                            }
                        }
                    }
                }

                WasReadyForHarvest = __instance.readyForHarvest.Value;
                return true;
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(MinutesElapsedPatch), nameof(Prefix), ex), LogLevel.Error);
                return true;
            }
        }

        public static void Postfix(SObject __instance)
        {
            try
            {
                if (WasReadyForHarvest == false && __instance.readyForHarvest.Value == true)
                {

                }
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(MinutesElapsedPatch), nameof(Postfix), ex), LogLevel.Error);
            }
        }
    }

    /// <summary>Intended to detect the moment that a machine's MinutesUntilReady is set increased, and at that moment,
    /// reduce the MinutesUntilReady by a factor corresponding to the combined machine's processing power.<para/>
    /// This action only takes effect if the config settings are set to <see cref="ProcessingMode.IncreaseSpeed"/>, or if the machine is an exclusion.<para/>
    /// See also: <see cref="UserConfig.ProcessingMode"/>, <see cref="UserConfig.ProcessingModeExclusions"/></summary>
    public static class MinutesUntilReadyPatch
    {
        private static readonly HashSet<Cask> CurrentlyModifying = new HashSet<Cask>();

        public static void Postfix(SObject __instance)
        {
            try
            {
                if (Context.IsMainPlayer)
                {
                    if (__instance is Cask CaskInstance)
                    {
                        CaskInstance.agingRate.fieldChangeEvent += (field, oldValue, newValue) =>
                        {
                            try
                            {
                                //  Prevent recursive fieldChangeEvents from being invoked when our code sets Cask.agingRate.Value
                                if (CurrentlyModifying.Contains(CaskInstance))
                                    return;

                                if (Context.IsMainPlayer && Context.IsWorldReady && oldValue != newValue)
                                {
                                    if (ModEntry.UserConfig.ShouldModifyProcessingSpeed(__instance) && __instance.TryGetCombinedQuantity(out int CombinedQuantity))
                                    {
#if LEGACY_CODE
                                        double DefaultAgingRate = CaskInstance.GetAgingMultiplierForItem(CaskInstance.heldObject.Value);

                                        bool IsTrackedValueChange = false;
                                        if (oldValue <= 0 && newValue > 0) // Handle the first time agingRate is initialized
                                            IsTrackedValueChange = true;
                                        else if (newValue == DefaultAgingRate) // Handle cases where the game tries to reset the agingRate
                                            IsTrackedValueChange = true;
#else

#if NEVER // This logic doesn't work because CaskInstance.heldObject is null in game version 1.6
                                        //  Find the AgingMultiplier for this cask's MachineData
                                        //  (It should be in OutputRule.OutputItem.CustomData.AgingMultiplier for the rule that corresponds to the cask's held item)
                                        double DefaultAgingRate;
                                        try
                                        {
                                            string HeldItemId = CaskInstance.heldObject.Value?.QualifiedItemId ?? CaskInstance.lastInputItem.Value?.QualifiedItemId;
                                            MachineOutputRule OutputRule = CaskInstance.GetMachineData().OutputRules.First(x => x.Triggers.Any(y => y.RequiredItemId == HeldItemId));
                                            DefaultAgingRate = double.Parse(OutputRule.OutputItem[0].CustomData["AgingMultiplier"]);
                                        }
                                        catch (Exception) { DefaultAgingRate = 0; }
#endif

                                        bool IsTrackedValueChange = true;
#endif


                                        if (IsTrackedValueChange)
                                        {
                                            float PreviousAgingRate = CaskInstance.agingRate.Value;
                                            double DurationMultiplier = ModEntry.UserConfig.ComputeProcessingPower(CombinedQuantity);
                                            float NewAgingRate = (float)(DurationMultiplier * PreviousAgingRate);

                                            if (NewAgingRate != PreviousAgingRate)
                                            {
                                                try
                                                {
                                                    CurrentlyModifying.Add(CaskInstance);
                                                    CaskInstance.agingRate.Value = NewAgingRate;
                                                }
                                                finally { CurrentlyModifying.Remove(CaskInstance); }

                                                ModEntry.Logger.Log(string.Format("Set {0} agingRate from {1} to {2} ({3}%)",
                                                    __instance.Name, PreviousAgingRate, NewAgingRate, (DurationMultiplier * 100.0).ToString("0.##")), ModEntry.InfoLogLevel);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception Error)
                            {
                                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}.FieldChangeEvent(Cask):\n{2}", nameof(MinutesUntilReadyPatch), nameof(Postfix), Error), LogLevel.Error);
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log($"Unhandled Error in {nameof(MinutesUntilReadyPatch)}.{nameof(Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(CrabPot), nameof(CrabPot.DayUpdate))]
    public static class CrabPot_DayUpdatePatch
    {
        /// <summary>Data that is retrieved just before <see cref="CrabPot.DayUpdate"/> executes.</summary>
        private class DayUpdateParameters
        {
            public SObject CrabPot { get; }
            public SObject PreviousHeldObject { get; }
            public SObject CurrentHeldObject { get { return CrabPot?.heldObject.Value; } }
            public int PreviousHeldObjectQuantity { get; }
            public int CurrentHeldObjectQuantity { get { return CurrentHeldObject == null ? 0 : CurrentHeldObject.Stack; } }

            public DayUpdateParameters(CrabPot CrabPot)
            {
                this.CrabPot = CrabPot;
                this.PreviousHeldObject = CrabPot.heldObject.Value;
                this.PreviousHeldObjectQuantity = PreviousHeldObject != null ? PreviousHeldObject.Stack : 0;
            }
        }

        private static DayUpdateParameters PrefixData { get; set; }

        public static bool Prefix(CrabPot __instance)
        {
            try
            {
                PrefixData = new DayUpdateParameters(__instance);
                if (__instance.IsCombinedMachine())
                {
                    if (CurrentlyModifying.Contains(__instance) || !ModEntry.UserConfig.ShouldModifyProcessingSpeed(__instance))
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(CrabPot_DayUpdatePatch), nameof(Prefix), ex), LogLevel.Error);
                return true;
            }
        }

        public static void Postfix(CrabPot __instance)
        {
            try
            {
                //  Check if the output item was just set
                if (PrefixData != null && PrefixData.CrabPot == __instance && PrefixData.PreviousHeldObject == null && PrefixData.CurrentHeldObject != null)
                {
                    //  Modify the output quantity based on the combined machine's processing power
                    if (__instance.IsCombinedMachine() && ModEntry.UserConfig.ShouldModifyInputsAndOutputs(__instance) && __instance.TryGetCombinedQuantity(out int CombinedQuantity))
                    {
                        double Power = ModEntry.UserConfig.ComputeProcessingPower(CombinedQuantity);
                        double DesiredNewValue = PrefixData.CurrentHeldObjectQuantity * Power;
                        int RoundedNewValue = RNGHelpers.WeightedRound(DesiredNewValue);
                        __instance.heldObject.Value.Stack = RoundedNewValue;
                        ModEntry.LogTrace(CombinedQuantity, PrefixData.CrabPot, PrefixData.CrabPot.TileLocation, "HeldObject.Stack", PrefixData.CurrentHeldObjectQuantity,
                            DesiredNewValue, RoundedNewValue, Power);
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(CrabPot_DayUpdatePatch), nameof(Postfix), ex), LogLevel.Error);
            }
        }

        private static HashSet<CrabPot> CurrentlyModifying = new HashSet<CrabPot>();

        internal static void InvokeDayUpdate(CrabPot instance)
        {
            if (instance == null)
                return;

            try
            {
                CurrentlyModifying.Add(instance);
                instance.DayUpdate();
            }
            finally { CurrentlyModifying.Remove(instance); }
        }
    }
}

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
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace CombineMachines.Patches
{
    /// <summary>Intended to detect when the player inserts materials into a machine that requires inputs, and multiply the input by the machine's combined quantity.<para/>
    /// For example, if 3 furnaces have been combined, then when the player inserts copper ore into the combined furnace, this mod should attempt to multiply the inputs of the furnace by 3.0.<para/>
    /// Note that the multiplied output logic is handled in the <see cref="MinutesElapsedPatch"/> Postfix.</summary>
    [HarmonyPatch(typeof(SObject), nameof(SObject.performObjectDropInAction))]
    public static class PerformObjectDropInActionPatch
    {
        /// <summary>Data that is retrieved just before <see cref="SObject.performObjectDropInAction(Item, bool, Farmer)"/> executes.</summary>
        private class PerformObjectDropInData
        {
            public Farmer Farmer { get; }
            public bool IsLocalPlayer { get { return Farmer.IsLocalPlayer; } }

            public SObject Machine { get; }

            public SObject PreviousHeldObject { get; }
            public SObject CurrentHeldObject { get { return Machine?.heldObject.Value; } }
            public int PreviousHeldObjectQuantity { get; }
            public int CurrentHeldObjectQuantity { get { return CurrentHeldObject == null ? 0 : CurrentHeldObject.Stack; } }

            public bool PreviousIsReadyForHarvest { get; }
            public bool CurrentIsReadyForHarvest { get { return Machine.readyForHarvest.Value; } }
            public int PreviousMinutesUntilReady { get; }
            public int CurrentMinutesUntilReady { get { return Machine.MinutesUntilReady; } }

            public Item Input { get; }
            public int PreviousInputQuantity { get; }
            public int CurrentInputQuantity { get { return Input?.Stack ?? 0; } }

            public int? InputInventoryIndex { get; }
            public bool WasInputInInventory { get { return InputInventoryIndex.HasValue; } }

            public PerformObjectDropInData(Farmer Farmer, SObject Machine, Item Input)
            {
                this.Farmer = Farmer;
                this.Machine = Machine;

                this.PreviousHeldObject = Machine.heldObject.Value;
                this.PreviousHeldObjectQuantity = PreviousHeldObject != null ? PreviousHeldObject.Stack : 0;
                this.PreviousIsReadyForHarvest = Machine.readyForHarvest.Value;
                this.PreviousMinutesUntilReady = Machine.MinutesUntilReady;

                this.Input = Input;
                this.PreviousInputQuantity = Input?.Stack ?? 0;
                this.InputInventoryIndex = Input != null && Farmer != null && Farmer.Items.Contains(Input) ? Farmer.Items.IndexOf(Input) : null;

#if DEBUG
                if (Input == null)
                    ModEntry.Logger.Log($"Input item for machine {Machine.DisplayName} is null in {nameof(PerformObjectDropInData)}.ctor.", LogLevel.Warn);
#endif
            }
        }

        private static PerformObjectDropInData PODIData { get; set; }

        [HarmonyPriority(Priority.First + 2)]
        public static bool WoodChipper_Prefix(WoodChipper __instance, Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed, ref bool __result)
        {
            return Prefix(__instance as SObject, dropInItem, probe, who, returnFalseIfItemConsumed, ref __result);
        }

        [HarmonyPriority(Priority.First + 2)]
        public static bool Prefix(SObject __instance, Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed, ref bool __result)
        {
            try
            {
                if (probe)
                    PODIData = null;
                else
                {
                    PODIData = new PerformObjectDropInData(who, __instance, dropInItem);
                    //ModEntry.Logger.Log(string.Format("{0} Prefix: {0} ({1})", nameof(PerformObjectDropInActionPatch), dropInItem.DisplayName, dropInItem.Stack), LogLevel.Info);
                }

                return true;
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(PerformObjectDropInActionPatch), nameof(Prefix), ex), LogLevel.Error);
                PODIData = null;
                return true;
            }
        }

        [HarmonyPriority(Priority.First + 2)]
        public static void WoodChipper_Postfix(WoodChipper __instance, Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed, ref bool __result)
        {
            Postfix(__instance as SObject, dropInItem, probe, who, returnFalseIfItemConsumed, ref __result);
        }

        [HarmonyPriority(Priority.First + 2)]
        public static void Postfix(SObject __instance, Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed, ref bool __result)
        {
            try
            {
                if (!probe)
                {
                    if (PODIData != null && PODIData.Machine == __instance && PODIData.PreviousHeldObject == null && PODIData.CurrentHeldObject != null)
                    {
                        OnInputsInserted(PODIData);
                        //ModEntry.Logger.Log(string.Format("{0} Postfix: {1} ({2})", nameof(PerformObjectDropInActionPatch), dropInItem.DisplayName, dropInItem.Stack), LogLevel.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(PerformObjectDropInActionPatch), nameof(Postfix), ex), LogLevel.Error);
            }
        }

        /// <summary>Intended to be invoked whenever the player inserts materials into a machine that requires inputs, such as when placing copper ore into a furnace.</summary>
        private static void OnInputsInserted(PerformObjectDropInData PODIData)
        {
            if (PODIData == null || PODIData.CurrentHeldObject == null || PODIData.Input == null)
                return;

            bool IsCurrentPlayer = (!Context.IsMultiplayer && !Context.IsSplitScreen) || PODIData.Farmer.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID;
            if (!IsCurrentPlayer)
                return;

            SObject Machine = PODIData.Machine;
            if (!ModEntry.UserConfig.ShouldModifyInputsAndOutputs(Machine) || !Machine.TryGetCombinedQuantity(out int CombinedQuantity))
                return;

            int SecondaryInputQuantityAvailable = int.MaxValue;
            if (PODIData.Input.IsOre() && PODIData.Farmer != null && ModEntry.UserConfig.FurnaceMultiplyCoalInputs)
            {
                SecondaryInputQuantityAvailable = PODIData.Farmer.Items.Where(x => x != null && x.IsCoal()).Sum(x => x.Stack);
                SecondaryInputQuantityAvailable += 1; // 1 coal has already been removed from the player's inventory by the time this event is invoked
            }

            //  Compute the maximum multiplier we can apply to the input and output based on how many more of the inputs the player has
            int PreviousInputQuantityUsed = PODIData.PreviousInputQuantity - PODIData.CurrentInputQuantity;
            double MaxMultiplier = Math.Min(SecondaryInputQuantityAvailable, PreviousInputQuantityUsed == 0 ? 
                PODIData.CurrentInputQuantity : 
                Math.Abs(PODIData.PreviousInputQuantity * 1.0 / PreviousInputQuantityUsed));

            //  Modify the output
            int PreviousOutputStack = PODIData.CurrentHeldObjectQuantity;
            int NewOutputStack = ComputeModifiedStack(CombinedQuantity, MaxMultiplier, PreviousOutputStack, out double OutputEffect, out double DesiredNewOutputValue);
            PODIData.CurrentHeldObject.Stack = NewOutputStack;
            Machine.SetHasModifiedOutput(true);
            ModEntry.LogTrace(CombinedQuantity, PODIData.Machine, PODIData.Machine.TileLocation, "HeldObject.Stack", PreviousOutputStack, DesiredNewOutputValue, NewOutputStack, OutputEffect);

            //  Modify the input
            int CurrentInputQuantityUsed;
            double InputEffect;
            double DesiredNewInputValue;
            if (PreviousInputQuantityUsed <= 0)
            {
                //  No clue why, but for some machines the game hasn't actually taken the input yet by the time Object.performObjectDropIn finishes.
                //  so assume the input amount was = to 1.
                CurrentInputQuantityUsed = ComputeModifiedStack(CombinedQuantity, MaxMultiplier, 1, out InputEffect, out DesiredNewInputValue) - 1 - Math.Abs(PreviousInputQuantityUsed);
            }
            else
            {
                CurrentInputQuantityUsed = ComputeModifiedStack(CombinedQuantity, MaxMultiplier, PreviousInputQuantityUsed, out InputEffect, out DesiredNewInputValue);
            }
            int NewInputStack = PODIData.PreviousInputQuantity - CurrentInputQuantityUsed;
            PODIData.Input.Stack = NewInputStack;
            if (NewInputStack <= 0)
            {
                if (PODIData.WasInputInInventory)
                    PODIData.Farmer.removeItemFromInventory(PODIData.Input);
                else
                {
                    PODIData.Input.Stack = 1; // Just a failsafe to avoid glitched out Items with zero quantity, such as if the input came from a chest due to the Automate mod
                }
            }

            if (PODIData.Input.IsOre() && PODIData.Farmer != null && ModEntry.UserConfig.FurnaceMultiplyCoalInputs)
            {
                int RemainingCoalToConsume = RNGHelpers.WeightedRound(OutputEffect) - 1; // 1 coal was already automatically consumed by the vanilla function
                for (int i = 0; i < PODIData.Farmer.Items.Count; i++)
                {
                    Item CurrentItem = PODIData.Farmer.Items[i];
                    if (CurrentItem != null && CurrentItem.IsCoal())
                    {
                        int AmountToConsume = Math.Min(CurrentItem.Stack, RemainingCoalToConsume);
                        CurrentItem.Stack -= AmountToConsume;
                        RemainingCoalToConsume -= AmountToConsume;

                        if (CurrentItem.Stack <= 0)
                            Utility.removeItemFromInventory(i, PODIData.Farmer.Items);

                        if (RemainingCoalToConsume <= 0)
                            break;
                    }
                }
            }
        }

        private static int ComputeModifiedStack(int CombinedQuantity, double MaxEffect, int PreviousValue, out double Effect, out double ValueBeforeRandomization)
        {
            Effect = Math.Min(MaxEffect, ModEntry.UserConfig.ComputeProcessingPower(CombinedQuantity));
            double DesiredNewValue = PreviousValue * Effect;
            ValueBeforeRandomization = DesiredNewValue;
            return RNGHelpers.WeightedRound(DesiredNewValue);
        }
    }

    /// <summary>Intended to detect the moment that a machine's output is ready for collecting, and at that moment, apply the appropriate multiplier to the output item's stack size based on the machine's combined quantity.</summary>
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
                        ModEntry.Logger.Log(string.Format("Forced {0}.{1} to execute at start of a new day for {2} with Power={3}% (Interval={4})",
                            nameof(CrabPot), nameof(CrabPot.DayUpdate), nameof(CrabPot), (Power * 100).ToString("0.##"), IntervalMinutes), ModEntry.InfoLogLevel);
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
                                    ModEntry.Logger.Log(string.Format("Forced {0}.{1} to execute at Time={2} for {3} with Power={4}% (Interval={5}",
                                        nameof(CrabPot), nameof(CrabPot.DayUpdate), CurrentTime, nameof(CrabPot), (Power * 100).ToString("0.##"), IntervalMinutes), ModEntry.InfoLogLevel);
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
                    OnReadyForHarvest(__instance);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(MinutesElapsedPatch), nameof(Postfix), ex), LogLevel.Error);
            }
        }

        private static void OnReadyForHarvest(SObject Machine)
        {
            if (Context.IsMainPlayer)
            {
                try
                {
                    if (Machine.heldObject.Value != null && ModEntry.UserConfig.ShouldModifyInputsAndOutputs(Machine) && 
                        Machine.TryGetCombinedQuantity(out int CombinedQuantity) && !Machine.HasModifiedOutput())
                    {
                        int PreviousOutputStack = Machine.heldObject.Value.Stack;

                        double OutputEffect = ModEntry.UserConfig.ComputeProcessingPower(CombinedQuantity);
                        double DesiredNewValue = PreviousOutputStack * OutputEffect;
                        int NewOutputStack = RNGHelpers.WeightedRound(DesiredNewValue);

                        Machine.heldObject.Value.Stack = NewOutputStack;
                        ModEntry.LogTrace(CombinedQuantity, Machine, Machine.TileLocation, "HeldObject.Stack", PreviousOutputStack, DesiredNewValue, NewOutputStack, OutputEffect);
                    }
                }
                finally { Machine.SetHasModifiedOutput(false); }
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
                    else
                    {
                        __instance.minutesUntilReady.fieldChangeEvent += (field, oldValue, newValue) =>
                        {
                            try
                            {
                                if (Context.IsMainPlayer && Context.IsWorldReady && oldValue != newValue && oldValue < newValue && newValue > 0)
                                {
                                    if (ModEntry.UserConfig.ShouldModifyProcessingSpeed(__instance) && __instance.TryGetCombinedQuantity(out int CombinedQuantity))
                                    {
                                        int PreviousMinutes = __instance.MinutesUntilReady;
                                        double DurationMultiplier = 1.0 / ModEntry.UserConfig.ComputeProcessingPower(CombinedQuantity);
                                        double TargetValue = DurationMultiplier * PreviousMinutes;
                                        int NewMinutes = RNGHelpers.WeightedRound(TargetValue);

                                        //  Round to nearest 10 since the game processes machine outputs every 10 game minutes
                                        //  EX: If NewValue = 38, then there is a 20% chance of rounding down to 30, 80% chance of rounding up to 40
                                        int SmallestDigit = NewMinutes % 10;
                                            NewMinutes = NewMinutes - SmallestDigit; // Round down to nearest 10
                                        if (RNGHelpers.RollDice(SmallestDigit / 10.0))
                                                NewMinutes += 10; // Round up

                                        //  There seems to be a bug where there is no product if the machine is instantly done processing.
                                        NewMinutes = Math.Max(10, NewMinutes); // temporary fix - require at least one 10-minute processing cycle

                                        if (NewMinutes != PreviousMinutes)
                                        {
                                            __instance.MinutesUntilReady = NewMinutes;
                                            if (NewMinutes <= 0)
                                                __instance.readyForHarvest.Value = true;

                                            ModEntry.Logger.Log(string.Format("Set {0} MinutesUntilReady from {1} to {2} ({3}%, Target value before weighted rounding = {4})",
                                                __instance.Name, PreviousMinutes, NewMinutes, (DurationMultiplier * 100.0).ToString("0.##"), TargetValue.ToString("0.#")), ModEntry.InfoLogLevel);
                                        }
                                    }
                                }
                            }
                            catch (Exception Error)
                            {
                                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}.FieldChangeEvent:\n{2}", nameof(MinutesUntilReadyPatch), nameof(Postfix), Error), LogLevel.Error);
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(MinutesUntilReadyPatch), nameof(Postfix), ex), LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(CrabPot), nameof(CrabPot.DayUpdate))]
    public static class CrabPot_DayUpdatePatch
    {
        /// <summary>Data that is retrieved just before <see cref="CrabPot.DayUpdate(GameLocation)"/> executes.</summary>
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

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
using Harmony;
using StardewModdingAPI;
using StardewValley;
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
            public SObject CurrentHeldObject { get { return Machine?.heldObject; } }
            public int PreviousHeldObjectQuantity { get; }
            public int CurrentHeldObjectQuantity { get { return CurrentHeldObject == null ? 0 : CurrentHeldObject.Stack; } }

            public bool PreviousIsReadyForHarvest { get; }
            public bool CurrentIsReadyForHarvest { get { return Machine.readyForHarvest; } }
            public int PreviousMinutesUntilReady { get; }
            public int CurrentMinutesUntilReady { get { return Machine.MinutesUntilReady; } }

            public Item Input { get; }
            public int PreviousInputQuantity { get; }
            public int CurrentInputQuantity { get { return Input.Stack; } }

            public int? InputInventoryIndex { get; }
            public bool WasInputInInventory { get { return InputInventoryIndex.HasValue; } }

            public PerformObjectDropInData(Farmer Farmer, SObject Machine, Item Input)
            {
                this.Farmer = Farmer;
                this.Machine = Machine;

                this.PreviousHeldObject = Machine.heldObject;
                this.PreviousHeldObjectQuantity = PreviousHeldObject != null ? PreviousHeldObject.Stack : 0;
                this.PreviousIsReadyForHarvest = Machine.readyForHarvest.Value;
                this.PreviousMinutesUntilReady = Machine.MinutesUntilReady;

                this.Input = Input;
                this.PreviousInputQuantity = Input.Stack;
                this.InputInventoryIndex = Farmer != null && Farmer.Items.Contains(Input) ? Farmer.Items.IndexOf(Input) : (int?)null;
            }
        }

        private static PerformObjectDropInData PODIData { get; set; }

        [HarmonyPriority(Priority.First + 2)]
        public static bool Prefix(SObject __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
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
        public static void Postfix(SObject __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
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
            if (PODIData == null || PODIData.CurrentHeldObject == null || PODIData.Input == null || !Context.IsMainPlayer)
                return;

            SObject Machine = PODIData.Machine;
            if (Machine == null || !Machine.TryGetCombinedQuantity(out int CombinedQuantity))
                return;

            //  Compute the maximum multiplier we can apply to the input and output based on how many more of the inputs the player has
            int PreviousInputQuantityUsed = PODIData.PreviousInputQuantity - PODIData.CurrentInputQuantity;
            double MaxMultiplier = PreviousInputQuantityUsed == 0 ? PODIData.CurrentInputQuantity : Math.Abs(PODIData.PreviousInputQuantity * 1.0 / PreviousInputQuantityUsed);

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

        public static bool Prefix(SObject __instance, int minutes, GameLocation environment)
        {
            try
            {
                WasReadyForHarvest = __instance.readyForHarvest.Value;
                return true;
            }
            catch (Exception ex)
            {
                ModEntry.Logger.Log(string.Format("Unhandled Error in {0}.{1}:\n{2}", nameof(MinutesElapsedPatch), nameof(Prefix), ex), LogLevel.Error);
                return true;
            }
        }

        public static void Postfix(SObject __instance, GameLocation environment)
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
                    if (Machine.heldObject.Value != null && Machine.TryGetCombinedQuantity(out int CombinedQuantity) && !Machine.HasModifiedOutput())
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
}

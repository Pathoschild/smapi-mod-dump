/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
**
*************************************************/

using HarmonyLib;
using MachineAugmentors.Items;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace MachineAugmentors.Harmony
{
    public static class GamePatches
    {
        /// <summary>Data that is retrieved just before <see cref="StardewValley.Object.performObjectDropInAction(Item, bool, Farmer)"/> executes.</summary>
        public class PerformObjectDropInData
        {
            public Farmer Farmer { get; }
            public bool IsLocalPlayer { get { return Farmer.IsLocalPlayer; } }

            public Object Machine { get; }

            public Object PreviousHeldObject { get; }
            public Object CurrentHeldObject { get { return Machine?.heldObject.Value; } }
            public int PreviousHeldObjectQuantity { get; }
            public int CurrentHeldObjectQuantity { get { return CurrentHeldObject == null ? 0 : CurrentHeldObject.Stack; } }

            public bool PreviousIsReadyForHarvest { get; }
            public bool CurrentIsReadyForHarvest { get { return Machine.readyForHarvest.Value; } }
            public int PreviousMinutesUntilReady { get; }
            public int CurrentMinutesUntilReady { get { return Machine.MinutesUntilReady; } }

            public Item Input { get; }
            public int PreviousInputQuantity { get; }
            public int CurrentInputQuantity { get { return Input.Stack; } }

            public int? InputInventoryIndex { get; }
            public bool WasInputInInventory { get { return InputInventoryIndex.HasValue; } }

            public PerformObjectDropInData(Farmer Farmer, Object Machine, Item Input)
            {
                this.Farmer = Farmer;
                this.Machine = Machine;

                this.PreviousHeldObject = Machine.heldObject.Value;
                this.PreviousHeldObjectQuantity = PreviousHeldObject != null ? PreviousHeldObject.Stack : 0;
                this.PreviousIsReadyForHarvest = Machine.readyForHarvest.Value;
                this.PreviousMinutesUntilReady = Machine.MinutesUntilReady;

                this.Input = Input;
                this.PreviousInputQuantity = Input.Stack;
                this.InputInventoryIndex = Farmer != null && Farmer.Items.Contains(Input) ? Farmer.Items.IndexOf(Input) : (int?)null;
            }
        }

        private static PerformObjectDropInData PODIData { get; set; }

        [HarmonyPriority(Priority.First + 1)]
        public static bool PerformObjectDropInAction_Prefix(Object __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            try
            {
                if (probe)
                    PODIData = null;
                else
                {
                    PODIData = new PerformObjectDropInData(who, __instance, dropInItem);
                    //MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Prefix: {0} ({1})", dropInItem.DisplayName, dropInItem.Stack), LogLevel.Info);
                }

                return true;
            }
            catch (Exception ex)
            {
                MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Unhandled Error in {0}:\n{1}", nameof(PerformObjectDropInAction_Prefix), ex), LogLevel.Error);
                PODIData = null;
                return true;
            }
        }

        [HarmonyPriority(Priority.First + 1)]
        public static void PerformObjectDropInAction_Postfix(Object __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            try
            {
                if (!probe)
                {
                    if (PODIData != null && PODIData.Machine == __instance && PODIData.PreviousHeldObject == null && PODIData.CurrentHeldObject != null)
                    {
                        PlacedAugmentorsManager.Instance?.OnInputsInserted(PODIData);
                        //ModInstance.Monitor.Log(string.Format("Postfix: {0} ({1})", dropInItem.DisplayName, dropInItem.Stack), LogLevel.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Unhandled Error in {0}:\n{1}", nameof(PerformObjectDropInAction_Postfix), ex), LogLevel.Error);
            }
        }

        /// <summary>Data that is retrieved just before <see cref="StardewValley.Object.checkForAction(Farmer, bool)"/> executes.</summary>
        public class CheckForActionData
        {
            public Farmer Farmer { get; }
            public bool IsLocalPlayer { get { return Farmer.IsLocalPlayer; } }

            public Object Machine { get; }

            public Object PreviousHeldObject { get; }
            public Object CurrentHeldObject { get { return Machine?.heldObject.Value; } }
            public int PreviousHeldObjectQuantity { get; }
            public int CurrentHeldObjectQuantity { get { return CurrentHeldObject == null ? 0 : CurrentHeldObject.Stack; } }

            public bool PreviousIsReadyForHarvest { get; }
            public bool CurrentIsReadyForHarvest { get { return Machine.readyForHarvest.Value; } }
            public int PreviousMinutesUntilReady { get; }
            public int CurrentMinutesUntilReady { get { return Machine.MinutesUntilReady; } }

            public CheckForActionData(Farmer Farmer, Object Machine)
            {
                this.Farmer = Farmer;
                this.Machine = Machine;

                this.PreviousHeldObject = Machine.heldObject.Value;
                this.PreviousHeldObjectQuantity = PreviousHeldObject != null ? PreviousHeldObject.Stack : 0;
                this.PreviousIsReadyForHarvest = Machine.readyForHarvest.Value;
                this.PreviousMinutesUntilReady = Machine.MinutesUntilReady;
            }
        }

        internal static bool SkipNextCheckForAction { get; set; }
        private static CheckForActionData CFAData { get; set; }

        public static bool CheckForAction_Prefix(Object __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            try
            {
                if (SkipNextCheckForAction)
                {
                    SkipNextCheckForAction = false;
                    return false;
                }

                if (justCheckingForActivity)
                    CFAData = null;
                else
                {
                    CFAData = new CheckForActionData(who, __instance);
                    //MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Prefix: {0} ({1})", CFAData.PreviousHeldObject.DisplayName, CFAData.PreviousHeldObject.Stack), LogLevel.Info);
                }

                return true;
            }
            catch (Exception ex)
            {
                MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Unhandled Error in {0}:\n{1}", nameof(CheckForAction_Prefix), ex), LogLevel.Error);
                PODIData = null;
                return true;
            }
        }

        public static void CheckForAction_Postfix(Object __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            try
            {
                if (!justCheckingForActivity)
                {
                    //  Not sure, but I think __result is false if checkForAction failed, such as if your inventory was full when attempting to collect the products
                    if (__result && CFAData != null && CFAData.Machine == __instance && CFAData.PreviousIsReadyForHarvest && !CFAData.CurrentIsReadyForHarvest)
                    {
                        PlacedAugmentorsManager.Instance?.OnProductsCollected(CFAData);
                        //MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Postfix: {0} ({1})", CFAData.CurrentHeldObject.DisplayName, CFAData.CurrentHeldObject.Stack), LogLevel.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Unhandled Error in {0}:\n{1}", nameof(CheckForAction_Postfix), ex), LogLevel.Error);
            }
        }

        public static void Draw_Postfix(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            try
            {
                //  Draw the augmentors that are attached to this machine, if any
                if (PlacedAugmentorsManager.Instance != null)
                {
                    if (PlacedAugmentorsManager.Instance.TryFindAugmentedTile(__instance, x, y, out AugmentedTile AT))
                    {
                        List<AugmentorType> Types = AT.GetAugmentorQuantities().Where(KVP => KVP.Value > 0).Select(KVP => KVP.Key).ToList();
                        Augmentor.DrawIconsOnTile(spriteBatch, Types, x, y, 0.6f);
                    }
                }

            }
            catch (Exception ex)
            {
                MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Unhandled Error in {0}:\n{1}", nameof(Draw_Postfix), ex), LogLevel.Error);
            }
        }

        public static void MonsterDrop_Postfix(GameLocation __instance, Monster monster, int x, int y, Farmer who)
        {
            try
            {
                if (who == Game1.player)
                {
                    //  Make the monster drop an augmentor if you're lucky
                    double Chance = MachineAugmentorsMod.UserConfig.MonsterLootSettings.GetAugmentorDropChance(__instance, monster,
                        out double BaseChance, out double LocationMultiplier, out double ExpMultiplier, out double HPMultiplier);
                    bool Success = Augmentor.Randomizer.NextDouble() <= Chance;
                    string LogMessage;
                    if (Success)
                    {
                        int SpawnDirection = Augmentor.Randomizer.Next(4);
                        int NumTypes = Enum.GetValues(typeof(AugmentorType)).Length;
                        AugmentorType Type = (AugmentorType)Augmentor.Randomizer.Next(NumTypes);
                        int Quantity = Augmentor.RollDice(0.1) ? 2 : 1;
                        Game1.createItemDebris(Augmentor.CreateInstance(Type, Quantity), Game1.player.getStandingPosition(), SpawnDirection, null, -1);

                        LogMessage = string.Format("Succeeded drop chance: Location = {0}, monster.ExperienceGained = {1}, monster.MaxHealth = {2}\n"
                            + "BaseChance = {3} ({4}%), LocationMultiplier = {5} (+{6}%), ExpMultiplier = {7}, HPMultiplier = {8} (+{9}%), TotalChance = {10} ({11}%)",
                            __instance.Name, monster.ExperienceGained, monster.MaxHealth,
                            BaseChance, (BaseChance * 100.0).ToString("0.##"), LocationMultiplier, ((LocationMultiplier - 1.0) * 100.0).ToString("0.##"), 
                            ExpMultiplier.ToString("#.####"), HPMultiplier, ((HPMultiplier - 1.0) * 100.0).ToString("0.##"), Chance, (Chance * 100.0).ToString("0.###"));
                    }
                    else
                    {
                        LogMessage = string.Format("Failed drop chance: Location = {0}, monster.ExperienceGained = {1}, monster.MaxHealth = {2}\n"
                            + "BaseChance = {3} ({4}%), LocationMultiplier = {5} (+{6}%), ExpMultiplier = {7}, HPMultiplier = {8} (+{9}%), TotalChance = {10} ({11}%)",
                            __instance.Name, monster.ExperienceGained, monster.MaxHealth,
                            BaseChance, (BaseChance * 100.0).ToString("0.##"), LocationMultiplier, ((LocationMultiplier - 1.0) * 100.0).ToString("0.##"), 
                            ExpMultiplier.ToString("#.####"), HPMultiplier, ((HPMultiplier - 1.0) * 100.0).ToString("0.##"), Chance, (Chance * 100.0).ToString("0.###"));
                    }
#if DEBUG
                    MachineAugmentorsMod.ModInstance.Monitor.Log(LogMessage, LogLevel.Debug);
#else
                    MachineAugmentorsMod.ModInstance.Monitor.Log(LogMessage, LogLevel.Trace);
#endif
                }
            }
            catch (Exception ex)
            {
                MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Unhandled Error in {0}:\n{1}", nameof(MonsterDrop_Postfix), ex), LogLevel.Error);
            }
        }
    }
}

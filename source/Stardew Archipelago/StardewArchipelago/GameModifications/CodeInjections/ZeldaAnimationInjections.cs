/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public static class ZeldaAnimationInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static bool _shouldPrankOnFishDay;
        private static bool _shouldPrankOnOtherDays;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _shouldPrankOnFishDay = true;
            _shouldPrankOnOtherDays = false;
        }

        // public void holdUpItemThenMessage(Item item, bool showMessage = true)
        public static bool HoldUpItemThenMessage_SkipBasedOnConfig_Prefix(Farmer __instance, Item item, bool showMessage)
        {
            try
            {
                if (Game1.isFestival())
                {
                    return true;
                }

                // We skip this whole method when skipping hold up animations is true
                return !ModEntry.Instance.Config.SkipHoldUpAnimations || ShouldPrank();
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(HoldUpItemThenMessage_SkipBasedOnConfig_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public Item addItemToInventory(Item item, int position)
        public static void AddItemToInventory_Position_PrankDay_Postfix(Farmer __instance, Item item, int position, Item __result)
        {
            try
            {
                if (!ShouldPrank())
                {
                    return;
                }

                DoPrankZeldaAnimation(__instance, item, true);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddItemToInventory_Position_PrankDay_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public Item addItemToInventory(Item item, List<Item> affected_items_list)
        public static void AddItemToInventory_AffectedItems_PrankDay_Postfix(Farmer __instance, Item item, List<Item> affected_items_list, Item __result)
        {
            try
            {
                if (!ShouldPrank())
                {
                    return;
                }

                DoPrankZeldaAnimation(__instance, item, true);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddItemToInventory_AffectedItems_PrankDay_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void DoPrankZeldaAnimation(Farmer farmer, Item item, bool showMessage)
        {
            if (Game1.random.NextDouble() > 0.05)
            {
                return;
            }

            farmer.completelyStopAnimatingOrDoingAction();
            if (showMessage)
            {
                DelayedAction.playSoundAfterDelay("getNewSpecialItem", 750);
            }
            farmer.faceDirection(2);
            farmer.freezePause = 4000;
            farmer.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[3]
            {
                new(57, 0),
                new(57, 2500, false, false, Farmer.showHoldingItem),
                showMessage ?
                    new FarmerSprite.AnimationFrame((short) farmer.FarmerSprite.CurrentFrame, 500, false, false, Farmer.showReceiveNewItemMessage, true) :
                    new FarmerSprite.AnimationFrame((short) farmer.FarmerSprite.CurrentFrame, 500, false, false)
            });
            farmer.mostRecentlyGrabbedItem = item;
            farmer.canMove = false;

            if (Game1.random.NextDouble() < 0.2)
            {
                Game1.chatBox.addMessage("April's Fool! use !!fool to disable", Color.Gold);
            }
        }

        public static bool IsPrankDay()
        {
            return DateTime.Now.Month == 4 && DateTime.Now.Day == 1;
        }

        internal static bool ShouldPrank()
        {
            return IsPrankDay() ? _shouldPrankOnFishDay : _shouldPrankOnOtherDays;
        }

        internal static void TogglePrank()
        {
            if (IsPrankDay())
            {
                if (_shouldPrankOnFishDay)
                {
                    _shouldPrankOnFishDay = false;
                    Game1.chatBox.addMessage("Oh, the fun's already over?", Color.Gold);
                }
                else
                {
                    _shouldPrankOnFishDay = true;
                    Game1.chatBox.addMessage("Welcome back", Color.Gold);
                }
            }
            else
            {
                if (_shouldPrankOnOtherDays)
                {
                    _shouldPrankOnOtherDays = false;
                    Game1.chatBox.addMessage("That's what I thought.", Color.Gold);
                }
                else
                {
                    _shouldPrankOnOtherDays = true;
                    Game1.chatBox.addMessage("Really? You actually like this?", Color.Gold);
                }
            }
        }
    }
}

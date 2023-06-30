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
using StardewArchipelago.Archipelago;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Constants;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class BackpackInjections
    {
        private const int UNINITIALIZED = -1;
        private const string LARGE_PACK = "Large Pack";
        private const string DELUXE_PACK = "Deluxe Pack";
        private const string PREMIUM_PACK = "Premium Pack";
        private const string PROGRESSIVE_BACKPACK = "Progressive Backpack";

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        private static uint _dayLastUpdateBackpackDisplay;
        private static int _maxItemsForBackpackDisplay;
        private static int _realMaxItems;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _realMaxItems = UNINITIALIZED;
            UpdateMaxItemsForBackpackDisplay();
        }

        private static void UpdateMaxItemsForBackpackDisplay()
        {
            var numReceivedBackpacks = _archipelago.GetReceivedItemCount(PROGRESSIVE_BACKPACK);
            if (_locationChecker.IsLocationMissingAndExists(LARGE_PACK))
            {
                _maxItemsForBackpackDisplay = 12;
            }
            else if (_locationChecker.IsLocationMissingAndExists(DELUXE_PACK) && numReceivedBackpacks >= 1)
            {
                _maxItemsForBackpackDisplay = 24;
            }
            else if (_locationChecker.IsLocationMissingAndExists(PREMIUM_PACK) && numReceivedBackpacks >= 2)
            {
                _maxItemsForBackpackDisplay = 36;
            }
            else
            {
                _maxItemsForBackpackDisplay = 48;
            }
            
            _dayLastUpdateBackpackDisplay = Game1.stats.DaysPlayed;
        }

        public static bool AnswerDialogueAction_BackPackPurchase_Prefix(GameLocation __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (questionAndAnswer != "Backpack_Purchase")
                {
                    return true; // run original logic
                }

                __result = true;

                if (_locationChecker.IsLocationNotChecked(LARGE_PACK) && Game1.player.Money >= 2000)
                {
                    Game1.player.Money -= 2000;
                    _locationChecker.AddCheckedLocation(LARGE_PACK);
                    UpdateMaxItemsForBackpackDisplay();
                    return false; // don't run original logic
                }

                if (_locationChecker.IsLocationNotChecked(DELUXE_PACK) && Game1.player.Money >= 10000)
                {
                    Game1.player.Money -= 10000;
                    _locationChecker.AddCheckedLocation(DELUXE_PACK);
                    UpdateMaxItemsForBackpackDisplay();
                    return false; // don't run original logic
                }

                if (_locationChecker.IsLocationMissingAndExists(PREMIUM_PACK) && Game1.player.Money >= 50000)
                {
                    Game1.player.Money -= 50000;
                    _locationChecker.AddCheckedLocation(PREMIUM_PACK);
                    UpdateMaxItemsForBackpackDisplay();
                    return false; // don't run original logic
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_BackPackPurchase_Prefix)}:\n{ex}",
                    LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool PerformAction_BuyBackpack_Prefix(GameLocation __instance, string action, Farmer who, Location tileLocation, ref bool __result)
        {
            try
            {
                if (action == null || !who.IsLocalPlayer)
                {
                    return true; // run original logic
                }

                var actionParts = action.Split(' ');
                var actionName = actionParts[0];
                if (actionName == "BuyBackpack")
                {
                    BuyBackPackArchipelago(__instance, out __result);
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformAction_BuyBackpack_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void BuyBackPackArchipelago(GameLocation __instance, out bool __result)
        {
            __result = true;
            var numReceivedBackpacks = _archipelago.GetReceivedItemCount(PROGRESSIVE_BACKPACK);

            var responsePurchaseLevel1 = new Response("Purchase",
                Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response2000"));
            var responsePurchaseLevel2 = new Response("Purchase",
                Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response10000"));
            var responseDontPurchase = new Response("Not",
                Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_ResponseNo"));
            if (_locationChecker.IsLocationNotChecked(LARGE_PACK))
            {
                __instance.createQuestionDialogue(
                    Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question24"),
                    new Response[2]
                    {
                        responsePurchaseLevel1,
                        responseDontPurchase
                    }, "Backpack");
            }
            else if (_locationChecker.IsLocationNotChecked(DELUXE_PACK) && numReceivedBackpacks >= 1)
            {
                __instance.createQuestionDialogue(
                    Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question36"),
                    new Response[2]
                    {
                        responsePurchaseLevel2,
                        responseDontPurchase
                    }, "Backpack");
            }
            else if (_archipelago.SlotData.Mods.HasMod(ModNames.BIGGER_BACKPACK) && _locationChecker.IsLocationMissingAndExists(PREMIUM_PACK) && numReceivedBackpacks >= 2)
            {
                Response yes = new Response("Purchase", "Purchase (50,000g)");
                Response no = new Response("Not", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_ResponseNo"));
                Response[] resps = new Response[] { yes, no };
                __instance.createQuestionDialogue("Backpack Upgrade -- 48 slots", resps, "Backpack");
            }
        }

        // public override void draw(SpriteBatch b)
        public static bool Draw_SeedShopBackpack_Prefix(SeedShop __instance, SpriteBatch b)
        {
            try
            {
                if (Game1.stats.daysPlayed != _dayLastUpdateBackpackDisplay)
                {
                    UpdateMaxItemsForBackpackDisplay();
                }

                if (_realMaxItems == UNINITIALIZED)
                {
                    _realMaxItems = Game1.player.MaxItems;
                }

                Game1.player.MaxItems = _maxItemsForBackpackDisplay;
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Draw_SeedShopBackpack_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }        
        
        // public override void draw(SpriteBatch b)
        public static void Draw_SeedShopBackpack_Postfix(SeedShop __instance, SpriteBatch b)
        {
            try
            {
                if (_realMaxItems != UNINITIALIZED)
                {
                    Game1.player.MaxItems = _realMaxItems;
                }
                _realMaxItems = UNINITIALIZED;
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Draw_SeedShopBackpack_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}

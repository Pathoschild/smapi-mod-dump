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
using StardewArchipelago.Items.Unlocks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class FishingRodInjections
    {
        private const string PROGRESSIVE_FISHING_ROD = "Progressive Fishing Rod";
        private const string TRAINING_ROD = "Purchase Training Rod";
        private const string FIBERGLASS_ROD = "Purchase Fiberglass Rod";
        private const string IRIDIUM_ROD = "Purchase Iridium Rod";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        public static bool SkipEvent_BambooPole_Prefix(Event __instance)
        {
            try
            {
                if (__instance.id != 739330)
                {
                    return true; // run original logic
                }

                SkipBambooPoleEventArchipelago(__instance);
                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SkipEvent_BambooPole_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool AwardFestivalPrize_BambooPole_Prefix(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            try
            {
                var festivalWinnersField = _modHelper.Reflection.GetField<HashSet<long>>(__instance, "festivalWinners");
                if (__instance.id != 739330 || festivalWinnersField.GetValue().Contains(Game1.player.UniqueMultiplayerID) || split.Length <= 1 || split[1].ToLower() != "rod")
                {
                    return true; // run original logic
                }

                OnCheckBambooPoleLocation();

                if (Game1.activeClickableMenu == null)
                    __instance.CurrentCommand++;
                __instance.CurrentCommand++;

                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AwardFestivalPrize_BambooPole_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void SkipBambooPoleEventArchipelago(Event __instance)
        {
            if (__instance.playerControlSequence)
            {
                __instance.EndPlayerControlSequence();
            }

            Game1.playSound("drumkit6");

            var actorPositionsAfterMoveField = _modHelper.Reflection.GetField<Dictionary<string, Vector3>>(__instance, "actorPositionsAfterMove");
            actorPositionsAfterMoveField.GetValue().Clear();

            foreach (var actor in __instance.actors)
            {
                var ignoreStopAnimation = actor.Sprite.ignoreStopAnimation;
                actor.Sprite.ignoreStopAnimation = true;
                actor.Halt();
                actor.Sprite.ignoreStopAnimation = ignoreStopAnimation;
                __instance.resetDialogueIfNecessary(actor);
            }

            __instance.farmer.Halt();
            __instance.farmer.ignoreCollisions = false;
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.dialogueTyping = false;
            Game1.pauseTime = 0.0f;

            OnCheckBambooPoleLocation();

            __instance.endBehaviors(new string[4]
            {
                "end",
                "position",
                "43",
                "36",
            }, Game1.currentLocation);
        }

        public static bool GetFishShopStock_Prefix(Farmer who, ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                var fishShopStock = new Dictionary<ISalable, int[]>();
                AddFishingObjects(fishShopStock);
                AddFishingToolsAPLocations(fishShopStock);
                AddFishingTools(fishShopStock);
                AddFishingFurniture(fishShopStock);
                AddItemsFromPlayerToSell(fishShopStock);
                __result = fishShopStock;

                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetFishShopStock_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void AddFishingObjects(Dictionary<ISalable, int[]> fishShopStock)
        {
            fishShopStock.Add(new StardewValley.Object(TROUT_SOUP_ID, 1), new[] { 250, int.MaxValue });
            if (Game1.player.fishingLevel.Value >= 2)
            {
                fishShopStock.Add(new StardewValley.Object(BAIT_ID, 1), new[] { 5, int.MaxValue });
            }

            if (Game1.player.fishingLevel.Value >= 3)
            {
                fishShopStock.Add(new StardewValley.Object(CRAB_POT_ID, 1), new[] { 1500, int.MaxValue });
            }

            if (Game1.player.fishingLevel.Value >= 6)
            {
                fishShopStock.Add(new StardewValley.Object(SPINNER_ID, 1), new[] { 500, int.MaxValue });
                fishShopStock.Add(new StardewValley.Object(TRAP_BOBBER_ID, 1), new[] { 500, int.MaxValue });
                fishShopStock.Add(new StardewValley.Object(LEAD_BOBBER_ID, 1), new[] { 200, int.MaxValue });
            }

            if (Game1.player.fishingLevel.Value >= 7)
            {
                fishShopStock.Add(new StardewValley.Object(TREASURE_HUNTER_ID, 1), new[] { 750, int.MaxValue });
                fishShopStock.Add(new StardewValley.Object(CORK_BOBBER_ID, 1), new[] { 750, int.MaxValue });
            }

            if (Game1.player.fishingLevel.Value >= 8)
            {
                fishShopStock.Add(new StardewValley.Object(BARBED_HOOK_ID, 1), new[] { 1000, int.MaxValue });
                fishShopStock.Add(new StardewValley.Object(DRESSED_SPINNER_ID, 1), new[] { 1000, int.MaxValue });
            }

            if (Game1.player.fishingLevel.Value >= 9)
            {
                fishShopStock.Add(new StardewValley.Object(MAGNET_ID, 1), new[] { 1000, int.MaxValue });
            }
        }

        private static void AddFishingToolsAPLocations(Dictionary<ISalable, int[]> fishShopStock)
        {
            if (!_archipelago.SlotData.ToolProgression.HasFlag(ToolProgression.Progressive))
            {
                return;
            }

            var priceMultiplier = _archipelago.SlotData.ToolPriceMultiplier;
            var myActiveHints = _archipelago.GetMyActiveHints();
            if (_locationChecker.IsLocationMissingAndExists(TRAINING_ROD))
            {
                var trainingRodAPlocation = new PurchaseableArchipelagoLocation("Training Rod", TRAINING_ROD, _modHelper,
                    _locationChecker, _archipelago, myActiveHints);
                fishShopStock.Add(trainingRodAPlocation, new[] { (int)(25 * priceMultiplier), 1 });
            }
            if (Game1.player.fishingLevel.Value >= 2 && _locationChecker.IsLocationMissingAndExists(FIBERGLASS_ROD))
            {
                var fiberglassRodAPlocation = new PurchaseableArchipelagoLocation("Fiberglass Rod", FIBERGLASS_ROD, _modHelper,
                    _locationChecker, _archipelago, myActiveHints);
                fishShopStock.Add(fiberglassRodAPlocation, new[] { (int)(1800 * priceMultiplier), 1 });
            }
            if (Game1.player.fishingLevel.Value >= 6 && _locationChecker.IsLocationMissingAndExists(IRIDIUM_ROD))
            {
                var iridiumRodAPLocation = new PurchaseableArchipelagoLocation("Iridium Rod", IRIDIUM_ROD, _modHelper,
                    _locationChecker, _archipelago, myActiveHints);
                fishShopStock.Add(iridiumRodAPLocation, new[] { (int)(7500 * priceMultiplier), 1 });
            }
        }

        private static void AddFishingTools(Dictionary<ISalable, int[]> fishShopStock)
        {
            var modData = Game1.getFarm().modData;
            var receivedFishingRodLevel = _archipelago.GetReceivedItemCount(VanillaUnlockManager.PROGRESSIVE_FISHING_ROD);
            var isVanillaTools = !_archipelago.SlotData.ToolProgression.HasFlag(ToolProgression.Progressive);
            var fishingLevel = Game1.player.fishingLevel.Value;
            var priceMultiplier = _archipelago.SlotData.ToolPriceMultiplier;

            if (isVanillaTools || receivedFishingRodLevel >= 1)
            {
                var trainingRod = new FishingRod(1);
                fishShopStock.Add(trainingRod, new[] { (int)(25 * priceMultiplier), int.MaxValue });
            }

            if (isVanillaTools || receivedFishingRodLevel >= 2)
            {
                var bambooPole = new FishingRod(0);
                fishShopStock.Add(bambooPole, new[] { (int)(500 * priceMultiplier), int.MaxValue });
            }

            if ((isVanillaTools && fishingLevel >= 2) || receivedFishingRodLevel >= 3)
            {
                var fiberglassRod = new FishingRod(2);
                fishShopStock.Add(fiberglassRod, new[] { (int)(1800 * priceMultiplier), int.MaxValue });
            }

            if ((isVanillaTools && fishingLevel >= 6) || receivedFishingRodLevel >= 4)
            {
                var iridiumRod = new FishingRod(3);
                fishShopStock.Add(iridiumRod, new[] { (int)(7500 * priceMultiplier), int.MaxValue });
            }

            if (Game1.MasterPlayer.mailReceived.Contains("ccFishTank"))
            {
                fishShopStock.Add(new Pan(), new[] { (int)(2500 * priceMultiplier), int.MaxValue });
            }
        }

        private static void AddFishingFurniture(Dictionary<ISalable, int[]> fishShopStock)
        {
            fishShopStock.Add(new FishTankFurniture(2304, Vector2.Zero), new[] { 2000, int.MaxValue });
            fishShopStock.Add(new FishTankFurniture(2322, Vector2.Zero), new[] { 500, int.MaxValue });
            if (Game1.player.mailReceived.Contains("WillyTropicalFish"))
            {
                fishShopStock.Add(new FishTankFurniture(2312, Vector2.Zero), new[] { 5000, int.MaxValue });
            }

            fishShopStock.Add(new BedFurniture(2502, Vector2.Zero), new[] { 25000, int.MaxValue });
        }

        private static void AddItemsFromPlayerToSell(Dictionary<ISalable, int[]> fishShopStock)
        {
            var locationFromName = Game1.getLocationFromName("FishShop");
            if (locationFromName is not ShopLocation fishShop) return;
            foreach (var key in fishShop.itemsFromPlayerToSell)
            {
                if (key.Stack <= 0) continue;
                var num = key.salePrice();
                if (key is StardewValley.Object)
                {
                    num = (key as StardewValley.Object).sellToStorePrice();
                }
                fishShopStock.Add(key, new[] { num, key.Stack });
            }
        }

        private static void OnCheckBambooPoleLocation()
        {
            _locationChecker.AddCheckedLocation("Bamboo Pole Cutscene");
        }

        private const int TROUT_SOUP_ID = 219;
        private const int BAIT_ID = 685;
        private const int CRAB_POT_ID = 710;
        private const int SPINNER_ID = 686;
        private const int TRAP_BOBBER_ID = 694;
        private const int LEAD_BOBBER_ID = 692;
        private const int TREASURE_HUNTER_ID = 693;
        private const int CORK_BOBBER_ID = 695;
        private const int BARBED_HOOK_ID = 691;
        private const int DRESSED_SPINNER_ID = 687;
        private const int MAGNET_ID = 703;
    }
}

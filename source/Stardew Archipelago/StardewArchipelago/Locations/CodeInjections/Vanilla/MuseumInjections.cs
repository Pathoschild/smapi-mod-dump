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
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Goals;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class MuseumInjections
    {
        public const string MUSEUMSANITY_PREFIX = "Museumsanity:";
        private const string MUSEUMSANITY_TOTAL_DONATIONS = "{0} {1} Donations";
        private const string MUSEUMSANITY_TOTAL_MINERALS = "{0} {1} Minerals";
        private const string MUSEUMSANITY_TOTAL_ARTIFACTS = "{0} {1} Artifacts";
        private const string MUSEUMSANITY_ANCIENT_SEED = "Ancient Seed";
        public const string MUSEUMSANITY_DWARF_SCROLLS = "Dwarf Scrolls";
        public const string MUSEUMSANITY_SKELETON_FRONT = "Skeleton Front";
        public const string MUSEUMSANITY_SKELETON_MIDDLE = "Skeleton Middle";
        public const string MUSEUMSANITY_SKELETON_BACK = "Skeleton Back";
        private const string ARCHAEOLOGY_QUEST = "Archaeology";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static StardewItemManager _itemManager;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager itemManager)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _itemManager = itemManager;
        }

        public static bool GetRewardsForPlayer_Museumsanity_Prefix(LibraryMuseum __instance, Farmer who,
            ref List<Item> __result)
        {
            try
            {
                var rewards = new List<Item>();
                var museumItems = new HashSet<int>(__instance.museumPieces.Values);
                var numberOfMuseumItemsMethod =
                    _modHelper.Reflection.GetMethod(__instance, "numberOfMuseumItemsOfType");
                var numberOfArtifactsDonated = numberOfMuseumItemsMethod.Invoke<int>("Arch");
                var numberOfMineralsDonated = numberOfMuseumItemsMethod.Invoke<int>("Minerals");
                var totalNumberDonated = numberOfArtifactsDonated + numberOfMineralsDonated;

                SendSpecialMuseumLetters(who, totalNumberDonated);
                CheckMilestones(numberOfArtifactsDonated, numberOfMineralsDonated, totalNumberDonated);
                CheckSpecialCollections(museumItems);
                CheckSpecificItems(museumItems);

                if (totalNumberDonated > 0)
                {
                    _locationChecker.AddCheckedLocation(ARCHAEOLOGY_QUEST);
                }

                __result = rewards;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetRewardsForPlayer_Museumsanity_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static void GetRewardsForPlayer_CheckGoalCompletion_Postfix(LibraryMuseum __instance, Farmer who, ref List<Item> __result)
        {
            try
            {
                GoalCodeInjection.CheckCompleteCollectionGoalCompletion();
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetRewardsForPlayer_CheckGoalCompletion_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void SendSpecialMuseumLetters(Farmer who, int totalNumberDonated)
        {
            SendLetterForTotal(who, "museum5", totalNumberDonated, 5);
            SendLetterForTotal(who, "museum10", totalNumberDonated, 10);
            SendLetterForTotal(who, "museum15", totalNumberDonated, 15);
            SendLetterForTotal(who, "museum20", totalNumberDonated, 20);
            SendLetterForTotal(who, "museum25", totalNumberDonated, 25);
            SendLetterForTotal(who, "museum30", totalNumberDonated, 30);
            SendLetterForTotal(who, "museum35", totalNumberDonated, 35);
            SendLetterForTotal(who, "museum40", totalNumberDonated, 40);
            SendLetterForTotal(who, "museum50", totalNumberDonated, 50);
            if (totalNumberDonated >= 60)
            {
                if (!Game1.player.eventsSeen.Contains(295672))
                    Game1.player.eventsSeen.Add(295672);
                if (!Game1.player.eventsSeen.Contains(66))
                    Game1.player.eventsSeen.Add(66);
            }

            SendLetterForTotal(who, "museum70", totalNumberDonated, 70);
            SendLetterForTotal(who, "museum80", totalNumberDonated, 80);
            SendLetterForTotal(who, "museum90", totalNumberDonated, 90);
            SendLetterForTotal(who, "museumComplete", totalNumberDonated, 95);
        }

        private static void SendLetterForTotal(Farmer who, string letter, int totalNumberDonated, int threshold)
        {
            if (!who.mailReceived.Contains(letter) && totalNumberDonated >= threshold)
            {
                who.mailReceived.Add(letter);
            }
        }

        private static void CheckMilestones(int numberOfArtifactsDonated, int numberOfMineralsDonated, int totalNumberDonated)
        {
            if (_archipelago.SlotData.Museumsanity != Museumsanity.Milestones)
            {
                return;
            }

            CheckMilestonesForType(MUSEUMSANITY_TOTAL_ARTIFACTS, numberOfArtifactsDonated);
            CheckMilestonesForType(MUSEUMSANITY_TOTAL_MINERALS, numberOfMineralsDonated);
            CheckMilestonesForType(MUSEUMSANITY_TOTAL_DONATIONS, totalNumberDonated);
        }

        private static void CheckMilestonesForType(string apLocationTemplate, int numberDonated)
        {
            for (var i = 1; i <= numberDonated; i++)
            {
                var apLocation = string.Format(apLocationTemplate, MUSEUMSANITY_PREFIX, i);
                if (_archipelago.GetLocationId(apLocation) > -1)
                {
                    _locationChecker.AddCheckedLocation(apLocation);
                }
            }
        }

        private static void CheckSpecialCollections(HashSet<int> museumItems)
        {
            if (_archipelago.SlotData.Museumsanity != Museumsanity.Milestones)
            {
                return;
            }

            CheckSpecialCollection(museumItems, new[] { 96, 97, 98, 99 }, MUSEUMSANITY_DWARF_SCROLLS);
            CheckSpecialCollection(museumItems, new[] { 114 }, MUSEUMSANITY_ANCIENT_SEED);
            CheckSpecialCollection(museumItems, new[] { 579, 581, 582 }, MUSEUMSANITY_SKELETON_FRONT);
            CheckSpecialCollection(museumItems, new[] { 583, 584 }, MUSEUMSANITY_SKELETON_MIDDLE);
            CheckSpecialCollection(museumItems, new[] { 580, 585 }, MUSEUMSANITY_SKELETON_BACK);
        }

        private static void CheckSpecialCollection(HashSet<int> museumItems, int[] requiredItems, string apSubLocation)
        {
            if (requiredItems.All(museumItems.Contains))
            {
                var apLocation = $"{MUSEUMSANITY_PREFIX} {apSubLocation}";
                if (_archipelago.GetLocationId(apLocation) > -1)
                {
                    _locationChecker.AddCheckedLocation(apLocation);
                }
            }
        }

        private static void CheckSpecificItems(HashSet<int> museumItems)
        {
            if (_archipelago.SlotData.Museumsanity == Museumsanity.Milestones)
            {
                return;
            }

            foreach (var museumItemId in museumItems)
            {
                var donatedItem = _itemManager.GetObjectById(museumItemId);
                var donatedItemName = donatedItem.Name;
                var apLocation = $"{MUSEUMSANITY_PREFIX} {donatedItemName}";
                if (_archipelago.GetLocationId(apLocation) > -1)
                {
                    _locationChecker.AddCheckedLocation(apLocation);
                }
                else
                {
                    _monitor.Log($"Unrecognized Museumsanity Location: {donatedItemName} [{museumItemId}]", LogLevel.Error);
                }
            }
        }
    }
}

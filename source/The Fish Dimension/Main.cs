/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/TheFishDimension
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ichortower.TheFishDimension
{
    internal sealed class ModEntry : Mod
    {
        private const int category_fish = -4;
        private const double chance_fishing = 0.1f / 0.39f;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            MethodInfo UtilityGRIFS = typeof(Utility).GetMethod("getRandomItemFromSeason",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] {typeof(Season), typeof(bool), typeof(Random)},
                    null);
            MethodInfo UtilityGQOTD = typeof(Utility).GetMethod("getQuestOfTheDay",
                    BindingFlags.Public | BindingFlags.Static);
            MethodInfo UtilityPCATT = typeof(Utility).GetMethod("possibleCropsAtThisTime",
                    BindingFlags.Public | BindingFlags.Static);
            harmony.Patch(UtilityGRIFS,
                    postfix: new HarmonyMethod(typeof(ModEntry),
                        "getRandomItemFromSeason_Postfix"));
            harmony.Patch(UtilityGQOTD,
                    postfix: new HarmonyMethod(typeof(ModEntry),
                        "getQuestOfTheDay_Postfix"));
            harmony.Patch(UtilityPCATT,
                    postfix: new HarmonyMethod(typeof(ModEntry),
                        "possibleCropsAtThisTime_Postfix"));

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        public static void getRandomItemFromSeason_Postfix(
                Season season,
                bool forQuest,
                Random random,
                ref string __result)
        {
            if (!forQuest) {
                return;
            }
            List<string> possibleItems = GetPossibleFish(season);
            if (possibleItems.Count > 0) {
                __result = random.ChooseFrom(possibleItems);
            }
        }

        public static void getQuestOfTheDay_Postfix(
                ref Quest __result)
        {
            if (__result is null) {
                return;
            }
            if (!(__result is FishingQuest) && !(__result is ItemDeliveryQuest)) {
                double d = Utility.CreateDaySaveRandom(1f, 39f, 23675f).NextDouble();
                if (d < chance_fishing) {
                    __result = new FishingQuest();
                }
                else {
                    __result = new ItemDeliveryQuest();
                }
            }
        }

        public static void possibleCropsAtThisTime_Postfix(
                Season season,
                ref List<string> __result)
        {
            __result = GetPossibleFish(season);
        }

        private static List<string> GetPossibleFish(Season season)
        {
            List<string> fish = new();
            var odd = (ObjectDataDefinition)ItemRegistry.GetTypeDefinition(
                    ItemRegistry.type_object);
            bool allowDesertFish = Utility.doesAnyFarmerHaveMail("ccVault");
            bool allowTalismanFish = Utility.doesAnyFarmerHaveMail("HasDarkTalisman");
            bool allowIslandFish = Utility.doesAnyFarmerHaveMail("seenBoatJourney");
            string seasonTag = $"season_{Utility.getSeasonKey(season)}";
            foreach (string id in odd.GetAllIds()) {
                Item it = ItemRegistry.Create(id);
                if (it.Category == category_fish &&
                        it.HasContextTag("!fish_mines") &&
                        it.HasContextTag("!fish_legendary") &&
                        (it.HasContextTag(seasonTag) || it.HasContextTag("season_all")) &&
                        (allowTalismanFish ||
                            (it.HasContextTag("!fish_bug_lair") &&
                             it.HasContextTag("!fish_swamp"))) &&
                        (allowIslandFish || it.HasContextTag("!fish_ginger_island")) &&
                        (allowDesertFish || it.HasContextTag("!fish_desert"))) {
                    fish.Add(it.ItemId);
                }
            }
            return fish;
        }

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects")) {
                string[] ids = {"836", "837", "838"};
                e.Edit(asset => {
                    var dict = asset.AsDictionary<string, ObjectData>();
                    foreach (string id in ids) {
                        if (!dict.Data[id].ContextTags.Contains("fish_ginger_island")) {
                            dict.Data[id].ContextTags.Add("fish_ginger_island");
                        }
                    }
                });
            }
        }
    }
}

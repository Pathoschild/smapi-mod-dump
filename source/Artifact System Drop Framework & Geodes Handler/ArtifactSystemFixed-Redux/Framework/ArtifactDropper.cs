/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmods-artifact-fix-redux/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;

using SObject = StardewValley.Object;

namespace ArtifactSystemFixed_Redux.Framework
    {
    internal class ArtifactDropper
        {
        internal const int ID_RICE_SHOOT = 273;
        internal const int ID_LOST_BOOKS = 102;
        internal const int ID_MIXED_SEEDS = 770;
        internal const int ID_SNOW_YAM = 416;
        internal const int ID_WINTER_ROOT = 412;
        internal const int ID_BONE_FRAGMENT = 881;
        internal const int ID_CLAY = 330;
        internal const int ID_WARP_TOTEM_BASE = 688;
        internal const int ID_QI_BEAN = 890;
        internal const int ID_STONE = 390;

        Dictionary<int, SObjectInfo> ArchObjects { get; } =
            Game1.objectInformation
                .Select(kvp => new SObjectInfo(kvp))
                .Where(objInf => objInf.IsArchaeology)
                .ToDictionary(objInf => objInf.ID)
                ;
        readonly ModConfig Config = ASFRedux.Config;
        readonly Random RNG;
        readonly Tool HoeTool;
        readonly GameLocation Loc;
        readonly string LocName;
        readonly int XLoc;
        readonly int YLoc;
        readonly Farmer Who;

        public ArtifactDropper(GameLocation loc, int xLocation, int yLocation, Farmer who, Random rng) {
            RNG = rng;
            HoeTool = who?.CurrentTool as Hoe;  // will be null if not Hoe
            Loc = loc;
            LocName = loc.name;
            XLoc = xLocation;
            YLoc = yLocation;
            Who = who;
            Log.Trace($"{nameof(ArtifactDropper)} for ({LocName}, {XLoc}, {YLoc}) initialized.");
            }

        ~ArtifactDropper() {
            Log.Trace($"{nameof(ArtifactDropper)}  for ({LocName}, {XLoc}, {YLoc}) is finalized.");
            }

        public void GetLoot() {

            #region Primary Table Loot

            int loot_primary = GetPrizeFromPrimaryTable();
            if (loot_primary != -1) {
                Log.Trace($"Got [{loot_primary}] from Primary Table");
                Game1.createObjectDebris(loot_primary, XLoc, YLoc, Who.UniqueMultiplayerID);
                Who.gainExperience(5, 25);
                return;
                }
            Log.Trace($"Got nothing from Primary Table, continuing");

            #endregion

            #region Inter-Table Loot

            bool generousEnchant = this.HoeTool?.hasEnchantmentOfType<GenerousEnchantment>() == true;
            float generousChance = 0.5f;
            string season = Game1.GetSeasonForLocation(this.Loc);

            if (season == "winter" && RNG.NextDouble() < 0.5 && this.Loc is not Desert) {
                int winter_forage = RNG.NextDouble() < 0.4 ? ID_SNOW_YAM : ID_WINTER_ROOT;
                Log.Trace($"Got Winter Forage [{winter_forage}]");
                Game1.createObjectDebris(winter_forage, XLoc, YLoc, Who.UniqueMultiplayerID);
                if (generousEnchant && RNG.NextDouble() < generousChance) {
                    // Spawn second forage item
                    Game1.createObjectDebris(winter_forage, XLoc, YLoc, Who.UniqueMultiplayerID);
                    }
                return;
                }

            if (RNG.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS")) {
                Log.Trace($"Got Qi Bean [{ID_QI_BEAN}]");
                Game1.createMultipleObjectDebris(ID_QI_BEAN, XLoc, YLoc, RNG.Next(2, 6), Who.UniqueMultiplayerID, Loc);
                }

            if (season == "spring" && RNG.NextDouble() < 0.0625 && Loc is not Desert && Loc is not Beach) {
                Log.Trace($"Got Rice Shoot [{ID_RICE_SHOOT}]");
                int num = RNG.Next(2, 6);
                if (generousEnchant && RNG.NextDouble() < generousChance) {
                    num += RNG.Next(2, 6);
                    }
                Game1.createMultipleObjectDebris(ID_RICE_SHOOT, XLoc, YLoc, num, Who.UniqueMultiplayerID, Loc);
                return;
                }

            if (
                RNG.NextDouble() <= 0.2
                && (
                    Game1.MasterPlayer.mailReceived.Contains("guntherBones")
                    || Game1.player.team.specialOrders.Where(spOrder => spOrder.questKey == "Gunther").Count() > 0
                    )
                ) {
                Log.Trace($"Got Bone Fragments [{ID_BONE_FRAGMENT}] in addition");
                Game1.createMultipleObjectDebris(ID_BONE_FRAGMENT, XLoc, YLoc, RNG.Next(2, 6), Who.UniqueMultiplayerID, Loc);
                }

            #endregion

            #region Secondary Table Loot

            int toDigUp = GetPrizeFromSecondaryTable();
            // Failed to grab anything from secondary table
            if (toDigUp == -1) {
                Log.Trace($"Got nothing from Secondary Table, stopping.");
                return;
                }
            Log.Trace($"Got [{toDigUp}] from Secondary Table");

            if (this.ArchObjects.ContainsKey(toDigUp) || toDigUp == ID_LOST_BOOKS) {
                if (toDigUp == ID_LOST_BOOKS && Game1.netWorldState.Value.LostBooksFound >= Constants.TOTAL_LOST_BOOKS) {
                    Log.Trace($"Overridden to [{ID_MIXED_SEEDS}] Mixed Seeds");
                    toDigUp = ID_MIXED_SEEDS;
                    }
                Game1.createObjectDebris(toDigUp, XLoc, YLoc, Who.UniqueMultiplayerID);
                return;
                }

            if (toDigUp == ID_CLAY && Loc.HasUnlockedAreaSecretNotes(Who) && RNG.NextDouble() < 0.11) {
                if (Loc.tryToCreateUnseenSecretNote(Who) is SObject secretNote) {
                    Log.Trace($"Overridden: Secret Note replacing Clay");
                    Game1.createItemDebris(secretNote, new Vector2(XLoc + 0.5f, YLoc + 0.5f) * 64f, -1, Loc);
                    return;
                    }
                }

            else if (toDigUp == ID_CLAY && Game1.stats.DaysPlayed > 28 && RNG.NextDouble() < 0.1) {
                int totem = ID_WARP_TOTEM_BASE + RNG.Next(3);
                Log.Trace($"Bonus: Warp Totem [{totem}] in addition to Clay from Secondary Table");
                Game1.createMultipleObjectDebris(totem, XLoc, YLoc, 1, Who.UniqueMultiplayerID);
                }

            Game1.createMultipleObjectDebris(toDigUp, XLoc, YLoc, RNG.Next(1, 4), Who.UniqueMultiplayerID);

            if (generousEnchant && RNG.NextDouble() < (double)generousChance) {
                Log.Trace($"Bonus: More [{toDigUp}] thanks to Generous Enchantment");
                Game1.createMultipleObjectDebris(toDigUp, XLoc, YLoc, RNG.Next(1, 4), Who.UniqueMultiplayerID);
                }
            return;

            #endregion

            }

        private int GetPrizeFromPrimaryTable() {
            bool archaeologyEnchant = this.HoeTool?.hasEnchantmentOfType<ArchaeologistEnchantment>() == true;

            static KeyValuePair<string, double> StringDoubleKVP(string k, string v) {
                double odds = Convert.ToDouble(v);
                return new KeyValuePair<string, double>(k, odds);
                }

            double origSumOfOdds = 0.0;
            double accumulator = 0.0;
            var prizesNewOdds = new List<KeyValuePair<int, double>>();
            foreach (KeyValuePair<int, SObjectInfo> kvp in this.ArchObjects) {
                string[] locationsAndItemsWeights = kvp.Value.WhenWhereWhat.Split(' ');

                KeyValuePair<string, double> locOdds;
                try {
                    locOdds = locationsAndItemsWeights.PairwiseKVP(StringDoubleKVP).SingleOrDefault(kvp => kvp.Key == this.LocName);
                    }
                catch (InvalidOperationException) {
                    Log.Error($"More than one location '{this.LocName}' for arch object {kvp.Key}, please check the input!");
                    continue;
                    }
                // If not found, itemKVP ~= KeyValuePair<>(null, null)
                if (locOdds.Key is null) continue;

                var itemID = kvp.Key;
                var odds = locOdds.Value;
                origSumOfOdds += odds;
                var foundCount = Game1.player.GetArchaeologyFound(itemID);
                if (foundCount > 0)
                    // Apply factor to weight of artifacts that have already been found
                    odds *= Math.Pow(Config.Artifact.AlreadyFoundMultiplier, foundCount);
                accumulator += archaeologyEnchant ? 2 * odds : odds;
                prizesNewOdds.Add(new KeyValuePair<int, double>(itemID, accumulator));
                }

            Log.Trace($"Primary Table item odds: original = {origSumOfOdds}, adjusted = {accumulator}");
            if (origSumOfOdds >= 1.0)
                Log.Error($"origSumOfOdds >= 1.0, items outside primary table will not be obtainable!");

            // The scalingFactor 'enlarges' each and every odds uniformly, so the odds of "getting any item" vs
            // the odds of "not getting any item" stays the same.
            double scalingFactor = origSumOfOdds / accumulator;

            /* Here we apply a simple arithmetic:

                    lottery                 < prize.Value * scalingFactor
                === lottery / scalingFactor < prize.Value * scalingFactor / scalingFactor
                === lottery / scalingFactor < prize.Value

             * thus, rather than doing an expensive fp mul per each element of prizesNewOdds, we can 'ensmallen'
             * the lottery value instead exactly once, and we'll end up with the same inequality result.
             * 
             * (The comparison happens inside the .ChooseOdds() extension)
             */
            int prizeID = RNG.ChooseOdds(prizesNewOdds, scale: scalingFactor, fallback: -1);
            Log.Trace($"Primary Table chose {prizeID}");

            // A 20% chance of getting a Lost Book instead
            if (Loc is not Farm && RNG.NextDouble() < Config.Artifact.LostBookOdds) {
                Log.Trace($"Overridden to [{ID_LOST_BOOKS}] Lost Book");
                prizeID = ID_LOST_BOOKS;
                }

            if (prizeID == ID_LOST_BOOKS && Game1.netWorldState.Value.LostBooksFound >= Constants.TOTAL_LOST_BOOKS) {
                Log.Trace($"All Lost Books found, overridden to [{ID_MIXED_SEEDS}] Mixed Seeds");
                prizeID = ID_MIXED_SEEDS;
                }

            return prizeID;
            }

        private int GetPrizeFromSecondaryTable() {
            Dictionary<string, string> locationDataMap = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            if (!locationDataMap.TryGetValue(LocName, out string locData)) {
                return -1;
                }

            string[] rawData = locData.Split('/')[8].Split(' ');
            if (rawData.Length == 0 || rawData[0].Equals("-1")) {
                return -1;
                }

            static KeyValuePair<int, double> IntDoubleKVP(string k, string v) {
                double odds = Convert.ToDouble(v);
                return new KeyValuePair<int, double>(Convert.ToInt32(k), odds);
                }

            int prizeID = RNG.ChooseOdds(
                rawData.PairwiseKVP(IntDoubleKVP).ToList(),
                fallback: -1
                );

            return prizeID;
            }


        }
    }

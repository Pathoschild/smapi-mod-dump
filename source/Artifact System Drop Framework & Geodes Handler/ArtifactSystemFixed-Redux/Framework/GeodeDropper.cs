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

using SObject = StardewValley.Object;

namespace ArtifactSystemFixed_Redux.Framework
    {
    class GeodeDropper
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

        internal const int ID_BANANA_SAPLING = 69;
        internal const int ID_MANGO_SAPLING = 835;
        internal const int ID_PINEAPPLE_SEEDS = 833;
        internal const int ID_TARO_TUBER = 831;
        internal const int ID_FOSSIL_SKULL = 820;
        internal const int ID_MAHOGANY_SEED = 292;

        internal const int ID_GOLDEN_HELMET = 75;

        internal const int ID_COPPER_ORE = 378;
        internal const int ID_IRON_ORE = 380;
        internal const int ID_COAL = 382;
        internal const int ID_GOLD_ORE = 384;
        internal const int ID_IRIDIUM_ORE = 386;

        internal const int ID_ARTIFACT_TROVE = 275;
        internal const int ID_GEODE = 535;
        internal const int ID_FROZEN_GEODE = 536;
        internal const int ID_OMNI_GEODE = 749;
        internal const int ID_GOLDEN_COCONUT = 791;

        internal const int ID_FIRE_QUARTZ = 82;
        internal const int ID_FROZEN_TEAR = 84;
        internal const int ID_EARTH_CRYSTAL = 86;
        internal const int ID_PRISMATIC_SHARD = 74;

        private readonly List<KeyValuePair<int, int>> GOLDEN_COCONUT_DROPS_LIST = Constants.GOLDEN_COCONUT_DROPS.ToList();

        readonly ModConfig Config = ASFRedux.Config;
        readonly Random RNG;
        readonly SObjectInfo Geode;

        public GeodeDropper(Item geode, Random rng) {
            RNG = rng;
            Geode = SObjectInfo.FromItem(geode);
            Log.Trace($"{nameof(GeodeDropper)} for [{Geode.ID}] {Geode.Name} initialized.");
            }

        ~GeodeDropper() {
            Log.Trace($"{nameof(GeodeDropper)} for [{Geode.ID}] {Geode.Name} is finalized.");
            }

        public bool IsValid => this.Geode.IsGeode;

        public KeyValuePair<int, int>? GetLoot(out Item special) {
            special = null;

            if (!Geode.IsGeode) {
                return null;
                }

            try {
                if (RNG.NextDouble() <= 0.1 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS")) {
                    int count = RNG.NextDouble() < Config.Geode.ManyBeansOdds ? Config.Geode.ManyBeansCount : 1;
                    return new KeyValuePair<int, int>(ID_QI_BEAN, count);
                    }
                if (Geode.ID == ID_GOLDEN_COCONUT) {
                    if (RNG.NextDouble() < Config.Geode.CoconutHatOdds && !Game1.player.hasOrWillReceiveMail("goldenCoconutHat")) {
                        Game1.player.mailReceived.Add("goldenCoconutHat");
                        special = new StardewValley.Objects.Hat(ID_GOLDEN_HELMET);
                        return null;
                        }
                    return RNG.Choose(GOLDEN_COCONUT_DROPS_LIST);
                    }

                if (Geode.ID == ID_ARTIFACT_TROVE || RNG.NextDouble() < Config.Geode.TreasureOdds) {
                    Log.Trace("Treasure code path chosen");
                    return GetGeodeTreasure();
                    }

                Log.Trace("Mineral code path chosen");

                int amount = (RNG.Next(3) * 2) + 1;

                if (RNG.NextDouble() < Config.Geode.TenMineralsOdds) {
                    amount = 10;
                    }

                if (RNG.NextDouble() < Config.Geode.TwentyMineralsOdds) {
                    amount = 20;
                    }

                if (RNG.NextDouble() >= Config.Geode.OreCoalOdds) {
                    switch (RNG.Next(4)) {
                        case 0:
                        case 1:
                            return new KeyValuePair<int, int>(ID_STONE, amount);
                        case 2:
                            return new KeyValuePair<int, int>(ID_CLAY, 1);
                        case 3: {
                            int parentSheetIndex = Geode.ID switch {
                                ID_OMNI_GEODE => ID_FIRE_QUARTZ + (RNG.Next(3) * 2),
                                ID_FROZEN_GEODE => ID_FROZEN_TEAR,
                                ID_GEODE => ID_EARTH_CRYSTAL,
                                _ => ID_FIRE_QUARTZ  // Magma Geode
                                };
                            return new KeyValuePair<int, int>(parentSheetIndex, 1);
                            }
                        }
                    }

                switch (Geode.ID) {
                    case ID_GEODE:
                        return (RNG.Next(3)) switch {
                            0 => new KeyValuePair<int, int>(ID_COPPER_ORE, amount),
                            1 => new KeyValuePair<int, int>((Game1.player.deepestMineLevel > 25) ? ID_IRON_ORE : ID_COPPER_ORE, amount),
                            // 2
                            _ => new KeyValuePair<int, int>(ID_COAL, amount)
                            };
                    case ID_FROZEN_GEODE:
                        return (RNG.Next(4)) switch {
                            0 => new KeyValuePair<int, int>(ID_COPPER_ORE, amount),
                            1 => new KeyValuePair<int, int>(ID_IRON_ORE, amount),
                            2 => new KeyValuePair<int, int>(ID_COAL, amount),
                            // 3
                            _ => new KeyValuePair<int, int>((Game1.player.deepestMineLevel > 75) ? ID_GOLD_ORE : ID_IRON_ORE, amount)
                            };
                    default:
                        return RNG.Next(5) switch {
                            0 => new KeyValuePair<int, int>(ID_COPPER_ORE, amount),
                            1 => new KeyValuePair<int, int>(ID_IRON_ORE, amount),
                            2 => new KeyValuePair<int, int>(ID_COAL, amount),
                            3 => new KeyValuePair<int, int>(ID_GOLD_ORE, amount),
                            // 4
                            _ => new KeyValuePair<int, int>(ID_IRIDIUM_ORE, (amount / 2) + 1)
                            };
                    }

                }
            catch (Exception ex) {
                Log.Error($"An exception happened! Details:\n{ex}");
                }
            Log.Trace($"How do we end up here?? Returning 1 [{ID_STONE}] Stone");
            special = new SObject(Vector2.Zero, ID_STONE, 1);
            return null;
            }

        public KeyValuePair<int, int> GetGeodeTreasure() {
            int prize;
            if (Math.Abs(Config.Geode.AlreadyFoundMultiplier) > Constants.GEODE_MULTIPLIER_CUTOFF) {
                Dictionary<int, double> TreasuresWeight = new();
                foreach (string treasure in Geode.Treasures) {
                    if (!int.TryParse(treasure, out int treasureID)) continue;
                    int timesFound = 0;
                    var treasureObj = SObjectInfo.FromIndex(treasureID);
                    if (treasureObj.IsMineral) {
                        timesFound = Game1.player.GetMineralFound(treasureID);
                        }
                    else if (treasureObj.IsArchaeology) {
                        timesFound = Game1.player.GetArchaeologyFound(treasureID);
                        }
                    double weight = timesFound == 0 ? 1.0 : Math.Pow(Config.Geode.AlreadyFoundMultiplier, timesFound);
                    TreasuresWeight[treasureID] = weight;
                    }
                prize = RNG.ChooseWeighted(TreasuresWeight, fallback: ID_STONE);  // fallback returned if all weights are 0.0 (can happen if multiplier == 0.0)
                Log.Trace($"Weighted probability chose {prize}");
                }
            else {
                prize = Convert.ToInt32(RNG.Choose(Geode.Treasures));
                Log.Trace($"Uniform distribution chose {prize}");
                }
            // Check if win the grand prize!
            if (
                Geode.ID == ID_OMNI_GEODE
                && Game1.stats.GeodesCracked > Constants.MIN_GEODES_FOR_PRISMATIC
                && RNG.NextDouble() < Constants.PRISMATIC_SHARD_ODDS
                ) {
                Log.Trace("Overridden to Prismatic Shard");
                prize = ID_PRISMATIC_SHARD;
                }
            return new KeyValuePair<int, int>(prize, 1);
            }

        }
    }

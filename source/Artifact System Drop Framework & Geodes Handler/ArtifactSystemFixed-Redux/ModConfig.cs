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

using System.Collections.Generic;

using ArtifactSystemFixed_Redux.Framework;

namespace ArtifactSystemFixed_Redux
    {
    public class ArtifactTuning
        {
        public double AlreadyFoundMultiplier { get; set; } = 0.6;
        public double LostBookOdds { get; set; } = 0.15;
        }
    public class GeodeTuning
        {
        public double AlreadyFoundMultiplier { get; set; } = 0.6;
        public double TreasureOdds { get; set; } = 0.5;
        public double CoconutHatOdds { get; set; } = 0.05;
        public double QiBeansOdds { get; set; } = 0.1;
        public double ManyBeansOdds { get; set; } = 0.25;
        public int ManyBeansCount { get; set; } = 5;
        public double TenMineralsOdds { get; set; } = 0.1;
        public double TwentyMineralsOdds { get; set; } = 0.01;
        public double OreCoalOdds { get; set; } = 0.5;
        }
    public class ModConfig
        {
        //Artifact dig spots
        public ArtifactTuning Artifact { get; set; } = new();

        //Geodes
        public GeodeTuning Geode { get; set; } = new();
        }

    internal static class Constants
        {
        internal static int TOTAL_LOST_BOOKS { get; } = 21;
        internal static double PRISMATIC_SHARD_ODDS { get; } = 0.008;
        internal static uint MIN_GEODES_FOR_PRISMATIC { get; } = 15;
        internal static double GEODE_MULTIPLIER_CUTOFF { get; } = 0.01;

        public readonly static Dictionary<int, int> GOLDEN_COCONUT_DROPS = new() {
            { GeodeDropper.ID_BANANA_SAPLING, 1 },
            { GeodeDropper.ID_MANGO_SAPLING, 1 },
            { GeodeDropper.ID_PINEAPPLE_SEEDS, 5 },
            { GeodeDropper.ID_TARO_TUBER, 5 },
            { GeodeDropper.ID_FOSSIL_SKULL, 1 },
            { GeodeDropper.ID_MAHOGANY_SEED, 1 },
            { GeodeDropper.ID_IRIDIUM_ORE, 5 }
            };

        }
    }

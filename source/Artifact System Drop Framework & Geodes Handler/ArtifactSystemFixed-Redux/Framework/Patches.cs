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
using System.Diagnostics.CodeAnalysis;

using ArtifactSystemFixed_Redux.Framework;

using StardewValley;

using SObject = StardewValley.Object;

namespace ArtifactSystemFixed_Redux
    {
    static partial class Patches
        {
        public static Random RNG {
            get {
                if (_RNG is null) _RNG = Game1.random;
                return _RNG;
                }
            internal set {
                _RNG = value;
                }
            }
        private static Random _RNG = null;

        public static Harmony.HarmonyMethod Get(string patchName) {
            return new Harmony.HarmonyMethod(typeof(Patches), patchName);
            }

        public static void WhatRNG(string prefix = null) {
            var RNGType = RNG.GetType();
            var spacer = prefix is null ? "" : " ";
            Log.Trace($"{prefix}{spacer}RNG is : {RNGType.AssemblyQualifiedName}");
            }

        // This is such a smart idea to wholly replace a function, I first saw it in the original "Artifact System Fixed".
        // Basically, have a prefix returning "false" unconditionally, then have a postfix implementing Harmony's "pass-through" facility.

        // Note: Ideas are not copyrightable. They are (in some jurisdictions) patentable, but the patent needs to be registered and granted.
        //       Therefore, as long as it is not patented, we're free to use the idea.

        #region Patch for GameLocation.digUpArtifactSpot

        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "If I don't specify this, VS2019 complains")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony requirement")]
        public static bool GameLocation_digUpArtifactSpot_prefix(int xLocation, int yLocation, Farmer who) => false;

        public static void GameLocation_digUpArtifactSpot_postfix(GameLocation __instance, int xLocation, int yLocation, Farmer who) {
            WhatRNG($"[{nameof(GameLocation_digUpArtifactSpot_postfix)}]");
            var LootDropper = new ArtifactDropper(__instance, xLocation, yLocation, who, RNG);
            LootDropper.GetLoot();
            }

        #endregion

        #region Patch for Utility.getTreasureFromGeode

        public static bool Utility_getTreasureFromGeode_prefix() => false;

        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "If I don't specify this, VS2019 complains")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony Pass-through Postfix convention")]
        public static Item Utility_getTreasureFromGeode_postfix(Item result, Item geode) {
            WhatRNG($"[{nameof(Utility_getTreasureFromGeode_postfix)}]");
            if (geode is null) return null;

            var LootDropper = new GeodeDropper(geode, RNG);
            if (!LootDropper.IsValid) return null;

            var drop = LootDropper.GetLoot(out Item special_item);

            if (special_item is not null) {
                Log.Trace($"Got special item: {special_item}");
                return special_item;
                }

            if (drop is null) {
                Log.Trace("Sorry, no prize for you.");
                return null;
                }

            var drop_item = drop.Value.Key;
            var drop_amt = drop.Value.Value;
            Log.Trace($"Got {drop_amt} of [{drop_item}]");
            return new SObject(drop_item, drop_amt);
            }

        #endregion

        }
    }

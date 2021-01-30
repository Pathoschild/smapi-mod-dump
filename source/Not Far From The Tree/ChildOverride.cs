/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-NFFTT
**
*************************************************/

/*
 * This software is licensed under the MIT License
 * https://github.com/GStefanowich/SDV-NFFTT
 *
 * Copyright (c) 2019 Gregory Stefanowich
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace NotFarFromTheTree {
    public static class ChildOverride {
        public static bool reloadSprite(Child __instance) {
            try {
                // Load the multiplayer IDs of Child Parents
                if (Game1.IsMasterGame) {
                    // Iterate farmers
                    Farmer farmer = Game1.getFarmerMaybeOffline(__instance.idOfParent.Value);
                    if (__instance.idOfParent.Value == 0L || farmer == null) {
                        long multiplayerId = Game1.MasterPlayer.UniqueMultiplayerID;
                        if (__instance.currentLocation is FarmHouse) {
                            foreach (Farmer allFarmer in Game1.getAllFarmers()) {
                                if (Utility.getHomeOfFarmer(allFarmer) == __instance.currentLocation) {
                                    multiplayerId = allFarmer.UniqueMultiplayerID;
                                    break;
                                }
                            }
                        }
                        
                        __instance.idOfParent.Value = multiplayerId;
                    }
                }
                
                ChildDat.Of(__instance)?.CleanSprite();
                return false;
            } catch (Exception e) {
                ModEntry.MONITOR.Log($"Unexpected exception in {ModEntry.MOD_ID}:\n{e}", LogLevel.Error);
                return true;
            }
        }
    }
}
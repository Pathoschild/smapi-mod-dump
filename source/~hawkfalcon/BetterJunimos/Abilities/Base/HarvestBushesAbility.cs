/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace BetterJunimos.Abilities {
    public class HarvestBushesAbility : IJunimoAbility {
        public string AbilityName() {
            return "HarvestBushes";
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
            if (location.terrainFeatures.ContainsKey(pos) && location.terrainFeatures[pos] is Bush bush) {
                return bush.tileSheetOffset.Value == 1;
            }
            return false;
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            // Don't do anything, as the base junimo handles this already (see PatchTryToHarvestHere)
            return true;
        }

        public List<int> RequiredItems() {
            return new List<int>();
        }
        
        /* older API compat */
        public bool IsActionAvailable(Farm farm, Vector2 pos, Guid guid) {
            return IsActionAvailable((GameLocation) farm, pos, guid);
        }
        
        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            return PerformAction((GameLocation) farm, pos, junimo, guid);
        }
    }
}
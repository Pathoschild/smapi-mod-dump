/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using BetterJunimos.Abilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace BetterJunimosForestry.Abilities {
    public class FertilizeTreesAbility : IJunimoAbility {
        private const int tree_fertilizer = 805;

        public string AbilityName() {
            return "FertilizeTrees";
        }

        public bool IsActionAvailable(GameLocation farm, Vector2 pos, Guid guid) {
            // in practice only seeds can be fertilized because the junimos can't get on top of saplings
            return farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is Tree t &&
                t.growthStage.Value < 5 && !farm.objects.ContainsKey(pos) && !t.fertilized.Value;
        }

        public bool PerformAction(GameLocation farm, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            var chest = Util.GetHutFromId(guid).output.Value;
            var foundItem = chest.items.FirstOrDefault(item => item is {ParentSheetIndex: tree_fertilizer});
            if (foundItem == null) return false;

            if (farm.terrainFeatures[pos] is not Tree t) return false;
            t.fertilize(farm);
            Util.RemoveItemFromChest(chest, foundItem);
            return true;

        }

        public List<int> RequiredItems() {
            return new() { tree_fertilizer };
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
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
using System.Configuration;
using BetterJunimos.Utils;
using SObject = StardewValley.Object;

namespace BetterJunimos.Abilities {
    public class HarvestCropsAbility : IJunimoAbility {
        private List<int> giantCrops = new() {190, 254, 276};

        internal HarvestCropsAbility() {
        }

        public string AbilityName() {
            return "HarvestCrops";
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
            if (!location.terrainFeatures.ContainsKey(pos) || location.terrainFeatures[pos] is not HoeDirt hd) return false;
            if (hd.crop is null) return false;
            if (!hd.readyForHarvest()) return false;
            return !ShouldAvoidHarvesting(pos, hd);
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            // calculate the experience from this harvest
            if (!BetterJunimos.Config.JunimoPayment.GiveExperience) return true;
            if (location.terrainFeatures.ContainsKey(pos) && location.terrainFeatures[pos] is HoeDirt {crop: { }} hd) {
                Game1.player.gainExperience(0, Util.ExperienceForCrop(hd.crop));
            }

            // Don't do anything, as the base junimo handles this already (see PatchTryToHarvestHere)
            return true;
        }

        public List<string> RequiredItems() {
            return new();
        }

        private bool ShouldAvoidHarvesting(Vector2 pos, HoeDirt hd) {
            var item = new StardewValley.Object(pos, hd.crop.indexOfHarvest.Value);

            // TODO: check properly if the crop will die tomorrow instead of special-casing 
            if (item.ParentSheetIndex == 421) {
                // if it's the last day of Fall, harvest sunflowers
                if (Game1.IsFall && Game1.dayOfMonth >= 28 && BetterJunimos.Config.JunimoImprovements.HarvestEverythingOn28th) return false;
            }
            else {
                // if it's the last day of the month, harvest whatever it is
                if (Game1.dayOfMonth >= 28 && BetterJunimos.Config.JunimoImprovements.HarvestEverythingOn28th) return false;
            }

            if (BetterJunimos.Config.JunimoImprovements.AvoidHarvestingGiants &&
                giantCrops.Contains(item.ParentSheetIndex)) {
                return true;
            }

            if (BetterJunimos.Config.JunimoImprovements.AvoidHarvestingFlowers &&
                item.Category == StardewValley.Object.flowersCategory) {
                return true;
            }

            return false;
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
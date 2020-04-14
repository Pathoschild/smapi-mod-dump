using BetterJunimos.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace BetterJunimos.Abilities {
    public class HarvestCropsAbility : IJunimoAbility {
        public string AbilityName() {
            return "HarvestCrops";
        }

        public bool IsActionAvailable(Farm farm, Vector2 pos) {
            if (farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is HoeDirt hd) {
                return hd.crop != null && hd.readyForHarvest() && !ShouldAvoidHarvesting(pos, hd);
            }
            return false;
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Chest chest) {
            // Don't do anything, as the base junimo handles this already (see PatchHarvestAttemptToCustom)
            return true;
        }

        public int RequiredItem() {
            return 0;
        }

        private bool ShouldAvoidHarvesting(Vector2 pos, HoeDirt hd) {
            return Util.Config.JunimoImprovements.AvoidHarvestingFlowers && new StardewValley.Object(pos, hd.crop.indexOfHarvest.Value, 0).Category == StardewValley.Object.flowersCategory;
        }
    }
}
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace BetterJunimos.Abilities {
    public class WaterAbility : IJunimoAbility {
        public string AbilityName() {
            return "Water";
        }

        public bool IsActionAvailable(Farm farm, Vector2 pos) {
            return farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is HoeDirt hd &&
                hd.state.Value != HoeDirt.watered;
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Chest chest) {
            if (farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is HoeDirt hd) {
                hd.state.Value = HoeDirt.watered;
                return true;
            }
            return false;
        }

        public int RequiredItem() {
            return 0;
        }
    }
}
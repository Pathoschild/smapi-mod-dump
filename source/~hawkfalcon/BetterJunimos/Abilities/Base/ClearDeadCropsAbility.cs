using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace BetterJunimos.Abilities {
    public class ClearDeadCropsAbility : IJunimoAbility {
    public string AbilityName() {
            return "ClearDeadCrops";
        }

        public bool IsActionAvailable(Farm farm, Vector2 pos) {
            return farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is HoeDirt hd && 
                hd.crop != null && hd.crop.dead.Value;
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Chest chest) {
            if (farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is HoeDirt hd) {
                bool animate = Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, farm);
                hd.destroyCrop(pos, animate, farm);
                return true;
            }
            return false;
        }

        public int RequiredItem() {
            return 0;
        }
    }
}
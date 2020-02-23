using System.Linq;
using BetterJunimos.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace BetterJunimos.Abilities {
    public class FertilizeAbility : IJunimoAbility {
        int ItemCategory = StardewValley.Object.fertilizerCategory;

        public string AbilityName() {
            return "Fertilize";
        }

        public bool IsActionAvailable(Farm farm, Vector2 pos) {
            return farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is HoeDirt hd && 
                hd.crop == null && !farm.objects.ContainsKey(pos) && hd.fertilizer.Value <= 0;
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Chest chest) {
            Item foundItem = chest.items.FirstOrDefault(item => item != null && item.Category == ItemCategory);
            if (foundItem == null) return false;

            Fertilize(farm, pos, foundItem.ParentSheetIndex);
            Util.RemoveItemFromChest(chest, foundItem);
            return true;
        }

        public int RequiredItem() {
            return ItemCategory;
        }

        private void Fertilize(Farm farm, Vector2 pos, int index) {
            if (farm.terrainFeatures[pos] is HoeDirt hd) {
                hd.fertilizer.Value = index;
                if (Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, farm)) {
                    farm.playSound("dirtyHit");
                }
            }
        }
    }
}
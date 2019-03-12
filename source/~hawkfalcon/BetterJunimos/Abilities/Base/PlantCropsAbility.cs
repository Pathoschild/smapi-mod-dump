using System;
using System.Linq;
using BetterJunimos.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace BetterJunimos.Abilities {
    public class PlantCropsAbility : IJunimoAbility {
        int ItemCategory = StardewValley.Object.SeedsCategory;

        public string AbilityName() {
            return "PlantCrops";
        }

        public bool IsActionAvailable(Farm farm, Vector2 pos) {
            return farm.terrainFeatures.ContainsKey(pos) && farm.terrainFeatures[pos] is HoeDirt hd && hd.crop == null &&
                !farm.objects.ContainsKey(pos);
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Chest chest) {
            Item foundItem = chest.items.FirstOrDefault(item =>
                item.Category == ItemCategory &&
                !(Util.Config.JunimoImprovements.AvoidPlantingCoffee && item.ParentSheetIndex == Util.CoffeeId)
            );
            if (foundItem == null) return false;

            bool success = Plant(farm, pos, foundItem.ParentSheetIndex);
            if (success) {
                Util.RemoveItemFromChest(chest, foundItem);
            }
            return success;
        }

        public int RequiredItem() {
            return ItemCategory;
        }

        private bool Plant(Farm farm, Vector2 pos, int index) {
            Crop crop = new Crop(index, (int)pos.X, (int)pos.Y);

            if (!crop.seasonsToGrowIn.Contains(Game1.currentSeason))
                return false;

            if (farm.terrainFeatures[pos] is HoeDirt hd) {
                CheckSpeedGro(hd, crop);
                hd.crop = crop;

                if (Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, farm)) {
                    if (crop.raisedSeeds.Value)
                        farm.playSound("stoneStep");
                    farm.playSound("dirtyHit");
                }

                ++Game1.stats.SeedsSown;
            }
            return true;
        }

        // taken from planting code
        private void CheckSpeedGro(HoeDirt hd, Crop crop) {
            if (hd.fertilizer.Value == 465 || hd.fertilizer.Value == 466 || Game1.player.professions.Contains(5)) {
                int num1 = 0;
                for (int index1 = 0; index1 < crop.phaseDays.Count - 1; ++index1)
                    num1 += crop.phaseDays[index1];
                float num2 = hd.fertilizer.Value == 465 ? 0.1f : (hd.fertilizer.Value == 466 ? 0.25f : 0.0f);
                if (Game1.player.professions.Contains(5))
                    num2 += 0.1f;
                int num3 = (int)Math.Ceiling((double)num1 * (double)num2);
                for (int index1 = 0; num3 > 0 && index1 < 3; ++index1) {
                    for (int index2 = 0; index2 < crop.phaseDays.Count; ++index2) {
                        if (index2 > 0 || crop.phaseDays[index2] > 1) {
                            crop.phaseDays[index2]--;
                            --num3;
                        }
                        if (num3 <= 0)
                            break;
                    }
                }
            }
        }
    }
}
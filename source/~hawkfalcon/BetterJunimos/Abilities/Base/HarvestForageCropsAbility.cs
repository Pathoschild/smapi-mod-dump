using System;
using BetterJunimos.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;

namespace BetterJunimos.Abilities {
    public class HarvestForageCropsAbility : IJunimoAbility {
        public string AbilityName() {
            return "HarvestForageCrops";
        }

        public bool IsActionAvailable(Farm farm, Vector2 pos) {
            Vector2 up = new Vector2(pos.X, pos.Y + 1);
            Vector2 right = new Vector2(pos.X + 1, pos.Y);
            Vector2 down = new Vector2(pos.X, pos.Y - 1);
            Vector2 left = new Vector2(pos.X - 1, pos.Y);

            Vector2[] positions = { up, right, down, left };
            foreach (Vector2 nextPos in positions) {
                if (farm.objects.ContainsKey(nextPos) && farm.objects[nextPos].isForage(farm)) {
                    return true;
                }
            }
            return false;
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Chest chest) {
            Vector2 up = new Vector2(pos.X, pos.Y + 1);
            Vector2 right = new Vector2(pos.X + 1, pos.Y);
            Vector2 down = new Vector2(pos.X, pos.Y - 1);
            Vector2 left = new Vector2(pos.X - 1, pos.Y);

            int direction = 0;
            Vector2[] positions = { up, right, down, left };
            foreach (Vector2 nextPos in positions) {
                if (farm.objects.ContainsKey(nextPos) && farm.objects[nextPos].isForage(farm)) {
                    junimo.faceDirection(direction);
                    SetForageQuality(farm, nextPos);

                    StardewValley.Object item = farm.objects[nextPos];
                    Util.AddItemToChest(farm, chest, item);

                    Util.SpawnParticles(nextPos);
                    farm.objects.Remove(nextPos);
                    return true;
                }
                direction++;
            }

            return false;
        }

        public int RequiredItem() {
            return 0;
        }

        // adapted from GameLocation.checkAction
        private void SetForageQuality(Farm farm, Vector2 pos) {
            int quality = farm.objects[pos].Quality;
            Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)pos.X + (int)pos.Y * 777);

            foreach (Farmer farmer in Game1.getOnlineFarmers()) {
                int maxQuality = quality;
                if (farmer.professions.Contains(16))
                    maxQuality = 4;
                else if (random.NextDouble() < farmer.ForagingLevel / 30.0)
                    maxQuality = 2;
                else if (random.NextDouble() < farmer.ForagingLevel / 15.0)
                    maxQuality = 1;
                if (maxQuality > quality)
                    quality = maxQuality;
            }

            farm.objects[pos].Quality = quality;
        }
    }
}
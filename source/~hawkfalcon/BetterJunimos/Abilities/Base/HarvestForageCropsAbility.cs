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
using BetterJunimos.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using System.Collections.Generic;

namespace BetterJunimos.Abilities {
    public class HarvestForageCropsAbility : IJunimoAbility {
        public string AbilityName() {
            return "HarvestForageCrops";
        }

        public bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
            Vector2 up = new Vector2(pos.X, pos.Y + 1);
            Vector2 right = new Vector2(pos.X + 1, pos.Y);
            Vector2 down = new Vector2(pos.X, pos.Y - 1);
            Vector2 left = new Vector2(pos.X - 1, pos.Y);

            Vector2[] positions = { up, right, down, left };
            foreach (Vector2 nextPos in positions) {
                if (location.objects.ContainsKey(nextPos) && location.objects[nextPos].isForage()) {
                    return true;
                }
            }
            return false;
        }

        public bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
            Chest chest = Util.GetHutFromId(guid).GetOutputChest();

            Vector2 up = new Vector2(pos.X, pos.Y + 1);
            Vector2 right = new Vector2(pos.X + 1, pos.Y);
            Vector2 down = new Vector2(pos.X, pos.Y - 1);
            Vector2 left = new Vector2(pos.X - 1, pos.Y);

            int direction = 0;
            Vector2[] positions = { up, right, down, left };
            foreach (Vector2 nextPos in positions) {
                if (location.objects.ContainsKey(nextPos) && location.objects[nextPos].isForage()) {
                    junimo.faceDirection(direction);
                    SetForageQuality(location, nextPos);

                    StardewValley.Object item = location.objects[nextPos];
                    Util.AddItemToChest(location, chest, item);

                    Util.SpawnParticles(nextPos);
                    location.objects.Remove(nextPos);
                    
                    // calculate the forage experience from this harvest
                    if (!BetterJunimos.Config.JunimoPayment.GiveExperience) return true;
                    Game1.player.gainExperience(2, 7);
                    
                    return true;
                }
                direction++;
            }

            return false;
        }

        public List<string> RequiredItems() {
            return new();
        }

        // adapted from GameLocation.checkAction
        private void SetForageQuality(GameLocation location, Vector2 pos) {
            var quality = location.objects[pos].Quality;
            var random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)pos.X + (int)pos.Y * 777);

            foreach (var farmer in Game1.getOnlineFarmers()) {
                var f = farmer.Stamina;
                var maxQuality = quality;
                if (farmer.professions.Contains(16))
                    maxQuality = 4;
                else if (random.NextDouble() < farmer.ForagingLevel / 30.0)
                    maxQuality = 2;
                else if (random.NextDouble() < farmer.ForagingLevel / 15.0)
                    maxQuality = 1;
                if (maxQuality > quality)
                    quality = maxQuality;
            }

            location.objects[pos].Quality = quality;
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
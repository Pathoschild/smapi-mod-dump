/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/micfort/betterjunimoscropfields
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using BetterJunimos.Abilities;
using BetterJunimos.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace BetterJunimosCropFields
{
    public class CropFieldsAbility : IJunimoAbility
    {
        public CropFieldsAbility(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public IMonitor Monitor { get; }
        
        public string AbilityName() => "Plant in separate crop fields";
        public bool IsActionAvailable(Farm farm, Vector2 pos, Guid guid)
        {
            if (!farm.terrainFeatures.ContainsKey(pos) || farm.terrainFeatures[pos] is not HoeDirt { crop: null } ||
                farm.objects.ContainsKey(pos)) return false;
            
            var signItem = FindSignItem(farm, pos);
            if (signItem is null) return false;

            var hutFromId = Util.GetHutFromId(guid);
            var chest = hutFromId.output.Value;
            
            var foundItem = chest.items.FirstOrDefault(item => item.ParentSheetIndex == signItem);
            if (foundItem == null) return false;
            
            return true;
        }

        public bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Guid guid)
        {
            var hutFromId = Util.GetHutFromId(guid);
            var chest = hutFromId.output.Value;
            
            var signItem = FindSignItem(farm, pos);
            if (signItem is null) return false;

            //search for crop in chest
            var foundItem = chest.items.FirstOrDefault(item => item.ParentSheetIndex == signItem);
            if (foundItem == null) return false;
            
            //plant crop
            var success = Plant(farm, pos, foundItem.ParentSheetIndex);
            if (success) {
                Util.RemoveItemFromChest(chest, foundItem);
            }
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// this is cached, and every second the cache is reset
        /// </remarks>
        /// <param name="farm"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int? FindSignItem(Farm farm, Vector2 pos)
        {
            //search for sign
            var sign = _cache.ContainsKey(pos) ? _cache[pos] : FindSign(farm, pos);
            return GetSeed(sign);
        }

        public List<int> RequiredItems()
        {
            return new List<int>() { Object.SeedsCategory };
        }

        public void UpdateTick(object? sender, UpdateTickingEventArgs e)
        {
            if (e.IsOneSecond)
            {
                _cache.Clear();
            }
        }

        private int? GetSeed(Sign? sign)
        {
            if (sign?.displayItem.Value is null) return null;

            var crops = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
            foreach (var (seedIndex, seedData) in crops)
            {
                if (int.TryParse(seedData.Split('/')[3], out var cropIndex) && cropIndex == sign.displayItem.Value.ParentSheetIndex)
                {
                    return seedIndex;
                }
            }

            return sign.displayItem.Value.ParentSheetIndex;
        }

        private readonly Dictionary<Vector2, Sign?> _cache = new();

        private Sign? FindSign(Farm farm, Vector2 startPos)
        {
            var queue = new Queue<Vector2>();
            var searched = new HashSet<Vector2>();
            queue.Enqueue(startPos);
            Sign? foundSign = null;

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();
                pos.Round();
                searched.Add(pos);
                if (farm.objects.ContainsKey(pos) && farm.objects[pos] is Sign sign)
                {
                    foundSign ??= sign;
                }

                var nextPositions = new[]
                {
                    pos - Vector2.UnitX,
                    pos + Vector2.UnitX,
                    pos - Vector2.UnitY,
                    pos + Vector2.UnitY,
                };
                foreach (var nextPosition in nextPositions)
                {
                    nextPosition.Round();
                    if (searched.Contains(nextPosition)) continue;
                    if (farm.terrainFeatures.ContainsKey(nextPosition) && farm.terrainFeatures[nextPosition] is HoeDirt)
                    {
                        queue.Enqueue(nextPosition);
                    }
                }
            }

            foreach (var pos in searched)
            {
                _cache[pos] = foundSign;
            }
            return foundSign;
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
                var num1 = 0;
                for (var index1 = 0; index1 < crop.phaseDays.Count - 1; ++index1)
                    num1 += crop.phaseDays[index1];
                var num2 = hd.fertilizer.Value == 465 ? 0.1f : (hd.fertilizer.Value == 466 ? 0.25f : 0.0f);
                if (Game1.player.professions.Contains(5))
                    num2 += 0.1f;
                var num3 = Convert.ToInt32(Math.Ceiling(num1 * num2));
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
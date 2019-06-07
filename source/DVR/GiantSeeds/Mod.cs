
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GiantSeeds
{
    public interface IJsonAssetsApi
    {
        void LoadAssets(string path);
        int GetCropId(string name);
    }
    public class ModEntry : Mod
    {
        private IJsonAssetsApi JsonAssets;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
           // helper.Events.Input.ButtonPressed += ButtonPressed;
        }
        private int numberOfTimesUsed = 0;
        IList<int> items = new List<int>
            {
                72, 797
            };
        int howManyDiamonds = 5;


        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var p = Game1.player;
            var r = Game1.random;
            
            if (!e.Button.IsUseToolButton() || Game1.activeClickableMenu != null) return;
            var weapon = Game1.player.CurrentTool as MeleeWeapon;
            if (weapon == null) return;
            if (weapon.CurrentParentTileIndex == 41)
            {
                update();
                for (var i = 0; i < howManyDiamonds; i++)
                {
                    var x = r.Next(10) - 5;
                    var y = r.Next(10) - 5;
                    var idx = r.Next(items.Count);
                    Game1.createObjectDebris(items[idx], p.getTileX() + x, p.getTileY() + y, p.UniqueMultiplayerID);
                }
            }
        }

        private void update()
        {
            numberOfTimesUsed++;
            if (numberOfTimesUsed >= 3)
            {
                howManyDiamonds = 50;
            }
            if (numberOfTimesUsed == 4)
            {
                items.Add(60);
                items.Add(70);
                items.Add(64);
                items.Add(68);
            }
            if (numberOfTimesUsed == 8)
            {
                items.Clear();
                items.Add(241);
            }
            if (numberOfTimesUsed == 10)
            {
                items.Clear();
                items.Add(72);
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var cropID = this.JsonAssets.GetCropId("Giant_Cauliflower");
            int indexOfHarvest = 0;
            if (Game1.currentSeason == "summer")
            {
                cropID = this.JsonAssets.GetCropId("Giant_Melon");
            }
            if (Game1.currentSeason == "fall")
            {
                cropID = this.JsonAssets.GetCropId("Giant_Pumpkin");
            }

            var giants = new List<Vector2>();
            var candidates = new List<Vector2>();
            var farm = Game1.getFarm();
            foreach (var kvp in farm.terrainFeatures.Pairs)
            {
                var loc = kvp.Key;
                var hd = kvp.Value as HoeDirt;
                if (hd == null || hd.crop == null)
                {
                    continue;
                }
                var crop = hd.crop;
                if (crop.rowInSpriteSheet.Value == cropID && crop.currentPhase.Value == crop.phaseDays.Count - 1)
                {
                    indexOfHarvest = crop.indexOfHarvest.Value;
                    giants.Add(loc);
                    candidates.Add(loc);
                }
            }
            var r = new Random((int)Game1.player.stats.stepsTaken);
            while (giants.Count >= 9 && candidates.Count > 0)
            {
                var c = candidates[r.Next(candidates.Count)];
                candidates.Remove(c);
                if (isValidCenter(giants, c))
                {
                    foreach (var v in neighbors(c))
                    {
                        giants.Remove(v);
                        candidates.Remove(v);
                        (farm.terrainFeatures[v] as HoeDirt).crop = null;
                    }
                    farm.resourceClumps.Add(new GiantCrop(indexOfHarvest, neighbors(c).First()));
                }
            }
        }

        private IEnumerable<Vector2> neighbors(Vector2 center)
        {
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    yield return new Vector2(center.X + x, center.Y + y);
                }
            }
        }

        private bool isValidCenter(List<Vector2> giants, Vector2 center)
        {
            foreach (var v in neighbors(center))
            {
                if (!giants.Contains(v))
                {
                    return false;
                }
            }
            return true;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // load Json Assets API
            this.JsonAssets = this.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (this.JsonAssets == null)
            {
                this.Monitor.Log("Can't access the Json Assets API. Is the mod installed correctly?", LogLevel.Error);
                return;
            }

            // inject Json Assets content pack
            this.JsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets"));

        }
    }

}

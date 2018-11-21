using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using StardewValley;
using StardewModdingAPI;
using Object = System.Object;

namespace StardewNotification
{
    public class HarvestNotification
    {
        private ITranslationHelper Trans;

        public HarvestNotification(ITranslationHelper Helper)
        {
            Trans = Helper;
        }

        private const int MUSHROOM_CAVE = 2;
        private const int FRUIT_CAVE = 1;

        public void CheckHarvestsAroundFarm()
        {
            CheckSeasonalHarvests();
        }

        public void CheckHarvestsOnFarm()
        {
            CheckFarmCaveHarvests(Game1.getLocationFromName("FarmCave"));
            CheckGreenhouseCrops(Game1.getLocationFromName("Greenhouse"));
        }

        public void CheckFarmCaveHarvests(GameLocation farmcave)
        {
            if (!StardewNotification.Config.NotifyFarmCave) return;
            if (Game1.player.caveChoice.Value == MUSHROOM_CAVE)
            {
                var numReadyForHarvest = farmcave.Objects.Values.Where(c => c.readyForHarvest.Value).Count();

                if (numReadyForHarvest > 0)
                {
                    var sampleObj = farmcave.Objects.Values.Where(c => c.readyForHarvest.Value).FirstOrDefault();
                    if (sampleObj != null) { 
                        StardewValley.Object sObj = new StardewValley.Object(sampleObj.ParentSheetIndex, sampleObj.Stack, false, sampleObj.Price, sampleObj.Quality);
                        sObj.bigCraftable.Value = sampleObj.bigCraftable.Value;
                        sObj.TileLocation = sampleObj.TileLocation;
                        sObj.Type = sampleObj.Type;
                        sObj.name = Game1.player.caveChoice.Value == MUSHROOM_CAVE ? Trans.Get("CaveMushroom") : Trans.Get("CaveFruit");
                        Game1.addHUDMessage(new HUDMessage(sObj.Type, numReadyForHarvest, true, Color.OrangeRed, sObj));
                    }
                }
            }

            else if (Game1.player.caveChoice.Value == FRUIT_CAVE && farmcave.Objects.Any())
            {
                var objList = farmcave.Objects.Values.Where(c => c.Category == 100 || c.Category == 80).ToArray();
                int iCount = objList.Count();

                if (iCount > 1)
                {
                    StardewValley.Object sObj = new StardewValley.Object(objList[1].ParentSheetIndex, objList[1].Stack, false, objList[1].Price, objList[1].Quality);
                    sObj.Type = objList[1].Type;
                    sObj.name = Game1.player.caveChoice.Value == MUSHROOM_CAVE ? Trans.Get("CaveMushroom") : Trans.Get("CaveFruit");
                    Game1.addHUDMessage(new HUDMessage(sObj.Type, iCount, true, Color.OrangeRed, sObj));

                }            
            }
        }

        public void CheckSeasonalHarvests()
        {
            if (!StardewNotification.Config.NotifySeasonalForage) return;
            string seasonal = null;
            var dayOfMonth = Game1.dayOfMonth;
            switch (Game1.currentSeason)
            {
                case "spring":
                    if (dayOfMonth > 14 && dayOfMonth < 19) seasonal = Trans.Get("Salmonberry");
                    break;
                case "summer":
                    if (dayOfMonth > 11 && dayOfMonth < 15) seasonal = Trans.Get("Seashells");
                    break;
                case "fall":
                    if (dayOfMonth > 7 && dayOfMonth < 12) seasonal = Trans.Get("Blackberry");
                    break;
                default:
                    break;
            }
            if (!(seasonal is null))
            {
                Util.ShowMessage($"{seasonal}");
            }
        }

        public void CheckGreenhouseCrops(GameLocation greenhouse)
        {
            if (!StardewNotification.Config.NotifyGreenhouseCrops) return;
            //var counter = new Dictionary<string, Pair<StardewValley.TerrainFeatures.HoeDirt, int>>();
            foreach (var pair in greenhouse.terrainFeatures.Pairs)
            {
                if (pair.Value is StardewValley.TerrainFeatures.HoeDirt hoeDirt)
                {
                    if (!hoeDirt.readyForHarvest()) continue;
                    Util.ShowMessage(Trans.Get("greenhouse_crops"));
                    break;
                }
            }
        }
    }
}

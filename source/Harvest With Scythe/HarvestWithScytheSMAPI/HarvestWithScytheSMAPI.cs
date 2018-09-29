using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestWithScytheSMAPI
{
    public class HarvestWithScytheSMAPI : Mod
    {
        private static int CountOfCropsReadyForHarvest { get; set; }

        public override void Entry(params object[] objects)
        {
            ControlEvents.MouseChanged += Events_MouseActionOnHoeDirt;
            ControlEvents.ControllerButtonPressed += Events_ControllerAction;
            LocationEvents.CurrentLocationChanged += Events_OnAreaChange;
        }

        static void Events_ControllerAction(object sender, EventArgsControllerButtonPressed e)
        {
            if (Game1.hasLoadedGame)
            {
                HarvestCropsWithAScythe();
            }
        }

        static void Events_MouseActionOnHoeDirt(object sender, EventArgs e)
        {
            if (Game1.hasLoadedGame)
            {
                HarvestCropsWithAScythe();
            }
        }

        static void Events_OnAreaChange(object sender, EventArgs e)
        {
            UpdateCountOfCurrentCropsReadyForHarvest();

            var terrFeats = Game1.currentLocation.terrainFeatures;

            if (terrFeats != null)
            {
                List<HoeDirt> hoeDirts = new List<HoeDirt>();

                foreach (var terr in terrFeats)
                {
                    if (terr.Value is HoeDirt)
                    {
                        hoeDirts.Add((HoeDirt)terr.Value);
                    }
                }

                foreach (var dirt in hoeDirts)
                {
                    if (dirt.crop != null)
                    {
                        dirt.crop.harvestMethod = 1;
                    }
                }
            }
        }

        private static void AddExperience(int indexOfHarvest)
        {
            var terrFeats = Game1.currentLocation.terrainFeatures;

            if (terrFeats != null)
            {
                var hoeDirts = terrFeats.Select(x => x.Value)
                                        .Where(x => x is HoeDirt);

                var newCountOfCrops = hoeDirts.Select(x => (HoeDirt)x)
                                              .Where(x => x.crop != null)
                                              .Where(x => x.readyForHarvest() == true)
                                              .Count();

                var difference = CountOfCropsReadyForHarvest - newCountOfCrops; 

                // Some weird random number copypasta from CA code.
                int num7 = Convert.ToInt32(Game1.objectInformation[indexOfHarvest].Split(new char[]
                 { '/' })[1]);
                float num8 = (float)(16.0 * Math.Log(0.018 * num7 + 1.0, 2.7182818284590451));

                var timesByNumberHarvested = num8 * difference;

                Game1.player.gainExperience(0, (int)Math.Round(timesByNumberHarvested));
                Console.WriteLine((int)Math.Round(timesByNumberHarvested));
                UpdateCountOfCurrentCropsReadyForHarvest();
            }
        }
   
        private static void UpdateCountOfCurrentCropsReadyForHarvest()
        {
            var terrFeats = Game1.currentLocation.terrainFeatures;
            if(terrFeats != null)
            {
                var hoeDirts = terrFeats.Select(x => x.Value)
                                    .Where(x => x is HoeDirt);


                CountOfCropsReadyForHarvest = hoeDirts.Select(x => (HoeDirt)x)
                                                      .Where(x => x.crop != null)
                                                      .Where(x => x.readyForHarvest() == true)
                                                      .Count();
            }
        }

        private static void HarvestCropsWithAScythe()
        {
            var terrFeats = Game1.currentLocation.terrainFeatures;
            var currentItem = Game1.player.Items[Game1.player.CurrentToolIndex];

            if (currentItem is MeleeWeapon && (currentItem as MeleeWeapon).Name.Equals("Scythe"))
            {
                var toolAct = Game1.player.GetToolLocation();

                if (toolAct != null && Game1.currentLocation.Name == "Farm" || Game1.currentLocation.Name == "Greenhouse" && terrFeats != null)
                {
                    var tile = terrFeats.FirstOrDefault(x => x.Key.X == ((int)toolAct.X / Game1.tileSize) && x.Key.Y == ((int)toolAct.Y / Game1.tileSize)).Value;
                    if (tile is HoeDirt)
                    {
                        var dirt = (HoeDirt)tile;
                        if (dirt.crop != null)
                        {
                            if (dirt.crop.currentPhase >= dirt.crop.phaseDays.Count() - 1 && (!dirt.crop.fullyGrown || dirt.crop.dayOfCurrentPhase <= 0))
                            {
                                if (dirt.crop.indexOfHarvest == 421)
                                {
                                    CreateSunflowerSeeds(431, ((int)toolAct.X / Game1.tileSize), ((int)toolAct.Y / Game1.tileSize), 2);
                                }

                                var numberForXP = dirt.crop.indexOfHarvest;
                                AddExperience(numberForXP);
                            }                          
                        }
                    }
                }
            }
        }

        private static void CreateSunflowerSeeds(int index, int x, int y, int quantity)
        {
            //We always spawn just two seeds, boring. Can fix later.
            Game1.createMultipleObjectDebris(index, x, y, 2);
        }


    }
}

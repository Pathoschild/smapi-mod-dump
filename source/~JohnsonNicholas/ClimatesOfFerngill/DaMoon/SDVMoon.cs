using StardewValley;
using System;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using TwilightShards.Common;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using System.Linq;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    public class SDVMoon
    {
        //encapsulated members
        private MersenneTwister Dice;
        private WeatherConfig ModConfig;

        //internal trackers
         private static int cycleLength = 14;

        //chances for various things
        private double CropGrowthChance;
        private double CropNoGrowthChance;
        private double GhostChance;
        private double BeachRemovalChance;
        private double BeachSpawnChance;

        //internal arrays
        internal readonly int[] beachItems = new int[] { 393, 397, 392, 394 };
        internal readonly int[] moonBeachItems = new int[] { 393, 394, 560, 586, 587, 589, 397 };

        public SDVMoon(WeatherConfig config, MersenneTwister rng)
        {
            Dice = rng;
            ModConfig = config;

            //set chances.
            CropGrowthChance = .09;
            CropNoGrowthChance = .09;
            BeachRemovalChance = .09;
            BeachSpawnChance = .35;
            GhostChance = .02;
        }


        public override string ToString()
        {
            return DescribeMoonPhase() + " on day " + GetDayOfCycle();
        }

        public MoonPhase CurrentPhase => GetLunarPhase();
        
        public MoonPhase GetLunarPhase()
        {
            //divide it by the cycle.
            int currentCycle = (int)Math.Floor(SDate.Now().DaysSinceStart / (double)cycleLength);
            int currentDay = GetDayOfCycle(SDate.Now());

            MoonPhase ret = SDVMoon.GetLunarPhase(currentDay);

            if (ret == MoonPhase.FullMoon)
            {
                if (Dice.NextDoublePositive() <= ModConfig.BadMoonRising)
                    return MoonPhase.BloodMoon;
            }

            return ret;
        }

        private int GetDayOfCycle()
        {
            return SDVMoon.GetDayOfCycle(SDate.Now());
        }

        private static int GetDayOfCycle(SDate Today)
        {
            return Today.DaysSinceStart % cycleLength;
        }

        /// <summary>
        /// This function returns the lunar phase for an arbitary day.
        /// </summary>
        /// <param name="Today">The day you are examining for.</param>
        /// <returns></returns>
        public static MoonPhase GetLunarPhaseForDay(SDate Today)
        {
            //divide it by the cycle.
            int currentCycle = (int)Math.Floor(Today.DaysSinceStart / (double)cycleLength);
            int currentDay = GetDayOfCycle(Today);

            return SDVMoon.GetLunarPhase(currentDay);
        }

        private static MoonPhase GetLunarPhase(int day)
        {
            //Day 0 and 16 are the New Moon, so Day 8 must be the Full Moon. Day 4 is 1Q, Day 12 is 3Q. Coorespondingly..
            switch (day)
            {
                case 0:
                    return MoonPhase.NewMoon;
                case 1:
                case 2:
                case 3:
                    return MoonPhase.WaxingCrescent;
                case 4:
                    return MoonPhase.FirstQuarter;
                case 5:
                case 6:
                    return MoonPhase.WaxingGibbeous;
                case 7:
                    return MoonPhase.FullMoon;
                case 8:
                case 9:
                    return MoonPhase.WaningGibbeous;
                case 10:
                    return MoonPhase.ThirdQuarter;
                case 11:
                case 12:
                case 13:
                    return MoonPhase.WaningCrescent;
                case 14:
                    return MoonPhase.NewMoon;
                default:
                    return MoonPhase.ErrorPhase;
            }
        }

        /// <summary>
        /// Handles events that fire at sleep.
        /// </summary>
        /// <param name="f"></param>
        public void HandleMoonAtSleep(Farm f, ITranslationHelper Helper)
        {
            if (f == null)
                return;

            if (Dice.NextDoublePositive() < .20)
                return;

            int cropsAffected = 0;

            //moon processing
            if (CurrentPhase == MoonPhase.FullMoon)
            {
                foreach (var TF in f.terrainFeatures)
                {
                    if (TF.Value is HoeDirt curr && curr.crop != null && Dice.NextDouble() < CropGrowthChance)
                    {
                        if (curr.state == HoeDirt.watered) //make sure it's watered
                        {
                            cropsAffected++;
                            int phaseDays = 0;
                            if (curr.crop.fullyGrown) {
                                curr.crop.dayOfCurrentPhase--;
                            }
                            else
                            {
                                if (curr.crop.phaseDays.Count > 0)
                                    phaseDays = curr.crop.phaseDays[Math.Min(curr.crop.phaseDays.Count - 1, curr.crop.currentPhase)];

                                curr.crop.dayOfCurrentPhase = Math.Min(curr.crop.dayOfCurrentPhase + 1, phaseDays);
                            }

                            
                            int phaseDayCount = (curr.crop.phaseDays.Count > 0 ? 
                                curr.crop.phaseDays[Math.Min(curr.crop.phaseDays.Count - 1, curr.crop.currentPhase)] : 0);

                            if (curr.crop.dayOfCurrentPhase >= phaseDayCount && 
                                curr.crop.currentPhase < curr.crop.phaseDays.Count - 1)
                            {
                                curr.crop.currentPhase = curr.crop.currentPhase + 1;
                                curr.crop.dayOfCurrentPhase = 0;
                            }
                        }
                    }
                }

                if (cropsAffected > 0)
                    Game1.addHUDMessage(new HUDMessage(Helper.Get("moon-text.fullmoon_eff", new { cropsAffected = cropsAffected })));
            }

            if (CurrentPhase == MoonPhase.NewMoon)
            {
                if (f != null)
                {
                    foreach (KeyValuePair<Vector2, TerrainFeature> TF in f.terrainFeatures)
                    {
                        if (TF.Value is HoeDirt curr && curr.crop != null)
                        {
                            if (Dice.NextDouble() < CropNoGrowthChance)
                            {
                                cropsAffected++;
                                curr.state = HoeDirt.dry; 
                            }
                        }
                    }
                }

                if (cropsAffected > 0)
                    Game1.addHUDMessage(new HUDMessage(Helper.Get("moon-text.newmoon_eff", new { cropsAffected = cropsAffected })));
            }
        }

        public void HandleMoonAfterWake(ITranslationHelper Helper)
        {
            if (Game1.getLocationFromName("Beach") is null)
                throw new Exception("... Please reinstall your game");

            Beach b = Game1.getLocationFromName("Beach") as Beach;
            int itemsChanged = 0;

            if (Dice.NextDoublePositive() < .20)
                return;

            //new moon processing
            if (CurrentPhase == MoonPhase.NewMoon)
            {
                List<KeyValuePair<Vector2, StardewValley.Object>> entries = (from o in b.objects
                                                                             where beachItems.Contains(o.Value.parentSheetIndex)
                                                                             select o).ToList();

                foreach (KeyValuePair<Vector2, StardewValley.Object> rem in entries)
                {
                        if (Dice.NextDouble() < BeachRemovalChance)
                        {
                            itemsChanged++;
                            b.objects.Remove(rem.Key);
                        }
                }

                if (itemsChanged > 0)
                    Game1.addHUDMessage(new HUDMessage(Helper.Get("moon-text.hud_message_new")));
            }

            //full moon processing
            if (CurrentPhase == MoonPhase.FullMoon)
            {
                int parentSheetIndex = 0;
                Rectangle rectangle = new Rectangle(65, 11, 25, 12);
                for (int index = 0; index < 5; ++index)
                {

                    //get the item ID to spawn
                    parentSheetIndex = moonBeachItems.GetRandomItem(Dice);
                    if (Dice.NextDouble() <= .0001)
                        parentSheetIndex = 392; //rare chance for a Nautlius Shell.

                    else if (Dice.NextDouble() > .0001 && Dice.NextDouble() <= .45)
                        parentSheetIndex = 589;

                    else if (Dice.NextDouble() > .45 && Dice.NextDouble() <= .62)
                        parentSheetIndex = 60;


                    if (Dice.NextDouble() < BeachSpawnChance)
                    {
                        Vector2 v = new Vector2((float)Game1.random.Next(rectangle.X, rectangle.Right), (float)Game1.random.Next(rectangle.Y, rectangle.Bottom));
                        itemsChanged++;
                        if (b.isTileLocationTotallyClearAndPlaceable(v))
                            b.dropObject(new StardewValley.Object(parentSheetIndex, 1, false, -1, 0), v * (float)Game1.tileSize, Game1.viewport, true, null);
                    }
                }

                if (itemsChanged > 0)
                    Game1.addHUDMessage(new HUDMessage(Helper.Get("moon-text.hud_message_full")));
            }
        }

        public static string DescribeMoonPhase(MoonPhase mp, ITranslationHelper Helper)
        {
            switch (mp)
            {
                case MoonPhase.ErrorPhase:
                    return Helper.Get("moon-text.error");
                case MoonPhase.FirstQuarter:
                    return Helper.Get("moon-text.phase-firstqrt");
                case MoonPhase.FullMoon:
                    return Helper.Get("moon-text.phase-full");
                case MoonPhase.NewMoon:
                    return Helper.Get("moon-text.phase-new");
                case MoonPhase.ThirdQuarter:
                    return Helper.Get("moon-text.phase-thirdqrt");
                case MoonPhase.WaningCrescent:
                    return Helper.Get("moon-text.phase-waningcres");
                case MoonPhase.WaningGibbeous:
                    return Helper.Get("moon-text.phase-waninggibb");
                case MoonPhase.WaxingCrescent:
                    return Helper.Get("moon-text.phase-waxingcres");
                case MoonPhase.WaxingGibbeous:
                    return Helper.Get("moon-text.phase-waxinggibb");
                default:
                    return Helper.Get("moon-text.error");
            }
        }

        private string DescribeMoonPhase()
        {
            switch (this.CurrentPhase)
            {
                case MoonPhase.ErrorPhase:
                    return "Phase Error";
                case MoonPhase.FirstQuarter:
                    return "First Quarter";
                case MoonPhase.FullMoon:
                    return "Full Moon";
                case MoonPhase.NewMoon:
                    return "New Moon";
                case MoonPhase.ThirdQuarter:
                    return "Third Quarter";
                case MoonPhase.WaningCrescent:
                    return "Waning Crescent";
                case MoonPhase.WaningGibbeous:
                    return "Waning Gibbeous";
                case MoonPhase.WaxingCrescent:
                    return "Waxing Crescent";
                case MoonPhase.WaxingGibbeous:
                    return "Waxing Gibbeous";
                default:
                    return "Text Error";
            }
        }

        public bool CheckForGhostSpawn()
        {
            if (Game1.timeOfDay > Game1.getTrulyDarkTime() && Game1.currentLocation.isOutdoors && Game1.currentLocation is Farm)
            {
                if (CurrentPhase is MoonPhase.FullMoon && Dice.NextDouble() < GhostChance)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

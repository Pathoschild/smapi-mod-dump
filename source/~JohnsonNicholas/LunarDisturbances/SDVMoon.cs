/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using System.Collections;
using StardewValley;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using System.Linq;
using StardewValley.Monsters;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;
using System;

namespace TwilightShards.LunarDisturbances
{
    public class SDVMoon
    {
        //encapsulated members
        private readonly Random Dice;
        private readonly MoonConfig ModConfig;
        private readonly ITranslationHelper Translations;

        //internal trackers
        private const int cycleLengthA = 14;
        private const int cycleLengthB = 25;

        //chances for various things
        public Color BloodMoonWater = Color.Red * 0.8f;
        private readonly IMonitor Monitor;
        internal float EclipseMods;
        internal LunarInfo MoonTracker;

        internal bool IsEclipse;

        private bool IsSuperMoon; //a relative of superman
        private bool IsBlueMoon;
        private bool IsBloodMoon;
        private bool IsHarvestMoon;

        //internal arrays
        internal readonly int[] beachItems = new int[] { 393, 397, 392, 394 };
        internal readonly int[] moonBeachItems = new int[] { 393, 394, 560, 586, 587, 589, 397 };

        public SDVMoon(MoonConfig config, Random rng, ITranslationHelper Trans, IMonitor Logger)
        {
            Dice = rng;
            ModConfig = config;
            Monitor = Logger;
            IsBloodMoon = false;
            IsSuperMoon = false;
            IsBlueMoon = false;
            IsHarvestMoon = false;
            Translations = Trans;
        }

        public void OnNewDay()
        {
            IsBloodMoon = false;
            IsSuperMoon = false;
            IsHarvestMoon = false;
            IsBlueMoon = false;

            if (CurrentPhase() == MoonPhase.FullMoon || CurrentPhase() == MoonPhase.BloodMoon)
            {
                if (Game1.currentSeason == "fall")
                {
                    if (Game1.dayOfMonth - GetMoonCycleLength < 1)
                    {
                        IsHarvestMoon = true;
                        Game1.addHUDMessage(new TCHUDMessage(Translations.Get("moon-text.harvestmoon"), CurrentPhase()));
                    }
                }
            }

            if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 27)
            {
                Game1.addHUDMessage(new TCHUDMessage(Translations.Get("moon-text.spiritmoon"), MoonPhase.SpiritsMoon));
            }

            if (Dice.NextDouble() < ModConfig.SuperMoonChances)
            {
                IsSuperMoon = true;
                Game1.addHUDMessage(new TCHUDMessage(Translations.Get("moon-text.supermoon"), CurrentPhase()));
            }

            if (MoonTracker is null)
            {
                Monitor.Log("Error: Moon Tracker is null", LogLevel.Info);
            }
            else
            {
                if (MoonTracker.FullMoonThisSeason && CurrentPhase() == MoonPhase.FullMoon || CurrentPhase() != MoonPhase.BloodMoon)
                {
                    IsBlueMoon = true;
                    Game1.addHUDMessage(new TCHUDMessage(Translations.Get("moon-text.bluemoon"), CurrentPhase()));
                    Game1.player.team.sharedDailyLuck.Value += .025;
                }
                else
                {
                    MoonTracker.FullMoonThisSeason = true;
                }
            }

            if (!(MoonTracker is null))
                MoonTracker.IsEclipseTomorrow = SetEclipseTomorrow();
            else
            {
                Monitor.Log("MoonTracker is null! Eclipse tomorrow not set", LogLevel.Error);
            }
        }

        public void TurnEclipseOn()
        {
            Monitor.Log("Turning the eclipse on!");
            IsEclipse = true;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public void TurnEclipseOn(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            TurnEclipseOn();
        }

        public float GetBrightnessQuotient()
        {
            switch (CurrentPhase())
            {
                case MoonPhase.BloodMoon:
                case MoonPhase.BlueMoon:
                    return 2f;
                case MoonPhase.HarvestMoon:
                    return 1.55f;
                case MoonPhase.SpiritsMoon:
                    return 1.15f;
                case MoonPhase.FullMoon:
                    return 1f;
                case MoonPhase.ThirdQuarter:
                case MoonPhase.FirstQuarter:
                    return .5f;
                case MoonPhase.WaxingCrescent:
                case MoonPhase.WaningCrescent:
                    return .15f;
                case MoonPhase.WaningGibbeous:
                case MoonPhase.WaxingGibbeous:
                    return .65f;
                case MoonPhase.NewMoon:
                    return 0.02f;
                default:
                    return 0.0f;
            }
        }

        public void Reset()
        {
            MoonTracker = null;
            IsBloodMoon = false;
            IsBlueMoon = false;
            IsHarvestMoon = false;
            IsSuperMoon = false;
            IsEclipse = false;
        }

        public override string ToString()
        {
            return DescribeMoonPhase() + " on day " + GetDayOfCycle();
        }

        public MoonPhase CurrentPhase()
        {
            MoonPhase def = GetLunarPhase();

            if (IsBloodMoon)
                return MoonPhase.BloodMoon;
            if (IsBlueMoon)
                return MoonPhase.BlueMoon;
            if (IsHarvestMoon)
                return MoonPhase.HarvestMoon;
            if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 27)
                return MoonPhase.SpiritsMoon;

            return def;
        }

        public MoonPhase GetLunarPhase()
        {
            //divide it by the cycle.
            int currentDay = GetDayOfCycle(SDate.Now());

            MoonPhase ret = SDVMoon.GetLunarPhase(currentDay, GetMoonCycleLength);

            if (IsBloodMoon) //restructuring.
                return MoonPhase.BloodMoon;
            if (IsBlueMoon)
                return MoonPhase.BlueMoon;
            if (IsHarvestMoon)
                return MoonPhase.HarvestMoon;

            return ret;
        }

        private int GetDayOfCycle()
        {
            return GetDayOfCycle(SDate.Now());
        }

        public int GetMoonCycleLength => (ModConfig.UseMoreMonthlyCycle ? cycleLengthB : cycleLengthA);

        private int GetDayOfCycle(SDate Today)
        {
            return Today.DaysSinceStart % GetMoonCycleLength;
        }

        private bool SetEclipseTomorrow()
        {
            bool validEclipseDate = (SDate.Now().DaysSinceStart > 2 && !Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason));
            bool validEclipsePhase = (this.CurrentPhase() == MoonPhase.NewMoon);
                    
            if (validEclipsePhase && validEclipseDate)
            {
                if (Dice.NextDouble() < (ModConfig.EclipseChance + EclipseMods))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This function returns the lunar phase for an arbitrary day.
        /// </summary>
        /// <param name="Today">The day you are examining for.</param>
        /// <returns></returns>
        public MoonPhase GetLunarPhaseForDay(SDate Today)
        {
            int currentDay = GetDayOfCycle(Today);
            return GetLunarPhase(currentDay, GetMoonCycleLength);
        }

        public void ForceBloodMoon()
        {
            IsBloodMoon = true;
            DoBloodMoonAlert();
            Game1.currentLocation.waterColor.Value = BloodMoonWater;
            LunarDisturbances.ContentManager.InvalidateCache("LooseSprites/Cursors");
        }

        public void UpdateForBloodMoon()
        {
            if (CurrentPhase() == MoonPhase.FullMoon && Dice.NextDouble() <= ModConfig.BadMoonRising && !Game1.isFestival() && !Game1.weddingToday && ModConfig.HazardousMoonEvents && !IsBlueMoon)
            {
                IsBloodMoon = true;
                DoBloodMoonAlert();
                Game1.currentLocation.waterColor.Value = BloodMoonWater;
                LunarDisturbances.ContentManager.InvalidateCache("LooseSprites/Cursors");
            }
        }

        internal void DoBloodMoonAlert()
        {
            Game1.addHUDMessage(new TCHUDMessage(LunarDisturbances.Translation.Get("moon-text.hud_message_bloodMoon"),CurrentPhase()));
        }

        private static MoonPhase GetLunarPhase(int day, int cycleLength)
        {
            if (cycleLength == 14)
            {
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
            else
            {
                switch (day)
                {
                    case 0:
                        return MoonPhase.NewMoon;
                    case int n when (n >= 1 && n <= 5):
                        return MoonPhase.WaxingCrescent;
                    case int n when (n == 6 || n == 7):
                        return MoonPhase.FirstQuarter;
                    case int n when (n >= 8 && n <= 12):
                        return MoonPhase.WaxingGibbeous;
                    case 13:
                        return MoonPhase.FullMoon;
                    case int n when (n >= 14 && n <= 18):
                        return MoonPhase.WaningGibbeous;
                    case 19:
                        return MoonPhase.ThirdQuarter;
                    case int n when (n >= 20 && n <= 24):
                        return MoonPhase.WaningCrescent;
                    case 25:
                        return MoonPhase.NewMoon;
                    default:
                        return MoonPhase.ErrorPhase;
                }
            }            
        }

        /// <summary>
        /// Handles events that fire at sleep.
        /// </summary>
        /// <param name="f"></param>
        public int HandleMoonAtSleep(Farm f, IMonitor Logger)
        {
            if (f == null)
                return 0;

            int cropsAffected = 0;

            if (CurrentPhase() == MoonPhase.FullMoon || CurrentPhase() == MoonPhase.HarvestMoon || CurrentPhase() == MoonPhase.BlueMoon)
            {
                foreach (var TF in f.terrainFeatures.Pairs)
                {
                    double diceRoll = ModConfig.CropGrowthChance;

                    if (CurrentPhase() == MoonPhase.HarvestMoon)
                        diceRoll *= 3.5;
                    if (IsSuperMoon)
                        diceRoll *= 2;

                    if (TF.Value is HoeDirt curr && curr.crop != null && Dice.NextDouble() < diceRoll)
                    {
                        if (ModConfig.Verbose)
                            Logger.Log($"Advancing crop at {TF.Key}", LogLevel.Trace);
                        SDVUtilities.AdvanceArbitrarySteps(f, curr, TF.Key);   

                        if (Dice.NextDouble() < ModConfig.HarvestMoonDoubleGrowChance)
                        {
                            if (ModConfig.Verbose)
                                Logger.Log($"Advancing crop at {TF.Key} for harvest moon", LogLevel.Trace);
                            SDVUtilities.AdvanceArbitrarySteps(f, curr, TF.Key);
                        }

                    }
                }
                return cropsAffected;
            }

            if (CurrentPhase() == MoonPhase.NewMoon && ModConfig.HazardousMoonEvents)
            {
                foreach (KeyValuePair<Vector2, TerrainFeature> TF in f.terrainFeatures.Pairs)
                {
                    double diceRoll = ModConfig.CropHaltChance;

                    if (IsSuperMoon)
                        diceRoll *= 2;

                    if (TF.Value is HoeDirt current && current.crop != null)
                    {
                        if (Dice.NextDouble() < diceRoll)
                        {
                            SDVUtilities.DeAdvanceCrop(f, current, TF.Key, 1, Logger);
                            current.state.Value = 0;
                            cropsAffected++;
                            if (ModConfig.Verbose)
                                Logger.Log($"Deadvancing crop at {TF.Key}", LogLevel.Trace);
                        }
                    }
                }

                return cropsAffected;
            }        
            return cropsAffected;
        }

        internal void TurnBloodMoonOff()
        {
            IsBloodMoon = false;
        }

        public void TenMinuteUpdate()
        {
            if (CheckForGhostSpawn() && SDVTime.CurrentIntTime > Game1.getStartingToGetDarkTime() && Game1.currentLocation is Farm && Game1.whichFarm == Farm.combat_layout)
            {
                GameLocation f = Game1.currentLocation;
                Vector2 zero = Vector2.Zero;
                switch (Game1.random.Next(4))
                {
                    case 0:
                        zero.X = Game1.random.Next(f.map.Layers[0].LayerWidth);
                        break;
                    case 1:
                        zero.X = (f.map.Layers[0].LayerWidth - 1);
                        zero.Y = Game1.random.Next(f.map.Layers[0].LayerHeight);
                        break;
                    case 2:
                        zero.Y = (f.map.Layers[0].LayerHeight - 1);
                        zero.X = Game1.random.Next(f.map.Layers[0].LayerWidth);
                        break;
                    case 3:
                        zero.Y = Game1.random.Next(f.map.Layers[0].LayerHeight);
                        break;
                }
                if (Utility.isOnScreen(zero * Game1.tileSize, Game1.tileSize))
                    zero.X -= Game1.viewport.Width;

                List<NPC> characters = f.characters.ToList();
                Ghost ghost = new Ghost(zero * Game1.tileSize)
                {
                    focusedOnFarmers = true,
                    wildernessFarmMonster = false,
                    willDestroyObjectsUnderfoot = false,
                };
                characters.Add(ghost);
            }
        }

        public string GetMenuString()
        {
            return Translations.Get("moon-desc.desc_moonphase", new { moonPhase = SDVMoon.DescribeMoonPhase(CurrentPhase(), Translations) });
        }

        public float GetTrackPosition()
        {
            int moonDuration = SDVTime.MinutesBetweenTwoIntTimes(GetMoonSetTime(), GetMoonRiseTime());
            int timeSinceRise = SDVTime.MinutesBetweenTwoIntTimes(Game1.timeOfDay, GetMoonRiseTime());

            if (Game1.timeOfDay < GetMoonRiseTime() || Game1.timeOfDay > GetMoonSetTime())
                return 0f;

            return timeSinceRise / moonDuration;            
        }

        public int GetMoonZenith()
        {
            int moonDuration = SDVTime.MinutesBetweenTwoIntTimes(GetMoonSetTime(), GetMoonRiseTime());
            SDVTime mr = new SDVTime(GetMoonRiseTime());
            mr.AddTime(moonDuration / 2);
            return mr.ReturnIntTime();
        }

        public void DayEnding()
        {
            IsEclipse = false;
        }

        public void HandleMoonAfterWake()
        {
            Beach b = Game1.getLocationFromName("Beach") as Beach;
            int itemsChanged = 0;

            if (Dice.NextDouble() < .20)
                return;

            //new moon processing
            if (CurrentPhase() == MoonPhase.NewMoon && ModConfig.HazardousMoonEvents && !(b is null))
            {
                List<KeyValuePair<Vector2, StardewValley.Object>> entries = (from o in b.objects.Pairs
                    where beachItems.Contains(o.Value.ParentSheetIndex)
                    select o).ToList();

                foreach (KeyValuePair<Vector2, StardewValley.Object> rem in entries)
                {
                    double diceRoll = ModConfig.BeachRemovalChance;
                    if (IsSuperMoon)
                        diceRoll *= 2;

                    if (Dice.NextDouble() < diceRoll)
                    {
                        itemsChanged++;
                        b.objects.Remove(rem.Key);
                    }
                }

                if (itemsChanged > 0)
                    Game1.addHUDMessage(new HUDMessage(Translations.Get("moon-text.hud_message_new")));
            }

            //full moon processing
            if (CurrentPhase() == MoonPhase.FullMoon)
            {
                Rectangle rectangle = new Rectangle(65, 11, 25, 12);
                for (int index = 0; index < 8; ++index)
                {
                    //get the item ID to spawn
                    var parentSheetIndex = moonBeachItems.GetRandomItem(Dice);
                    if (Dice.NextDouble() <= .0001)
                        parentSheetIndex = 392; //rare chance for a Nautlius Shell.

                    double emeraldChance = .2001;
                    if (IsSuperMoon)
                        emeraldChance += .10;

                    else if (Dice.NextDouble() > .0001 && Dice.NextDouble() <= emeraldChance)
                        parentSheetIndex = 60;

                    if (Dice.NextDouble() < ModConfig.BeachSpawnChance)
                    {
                        Vector2 v = new Vector2(Game1.random.Next(rectangle.X, rectangle.Right), Game1.random.Next(rectangle.Y, rectangle.Bottom));
                        itemsChanged++;
                        if (b.isTileLocationTotallyClearAndPlaceable(v))
                            b.dropObject(new StardewValley.Object(parentSheetIndex, 1, false, -1, 0), v * Game1.tileSize, Game1.viewport, true, null);
                    }
                }

                if (IsSuperMoon)
                {
                    for (int j = 0; j < 20; ++j)
                    {
                        double driftWoodChance = .25;
                        int parentSheetIndex = 388;
                        if (Dice.NextDouble() < driftWoodChance)
                        {
                            Vector2 v = new Vector2(Game1.random.Next(rectangle.X, rectangle.Right), Game1.random.Next(rectangle.Y, rectangle.Bottom));
                            itemsChanged++;
                            if (b.isTileLocationTotallyClearAndPlaceable(v))
                                b.dropObject(new StardewValley.Object(parentSheetIndex, 1, false, -1, 0), v * Game1.tileSize, Game1.viewport, true, null);
                        }
                    }
                }

                if (itemsChanged > 0)
                    Game1.addHUDMessage(new HUDMessage(Translations.Get("moon-text.hud_message_full")));
            }

            //check for eclipse tomorrow.
        }

        public string SimpleMoonPhase()
        {
            return SDVMoon.DescribeMoonPhase(this.CurrentPhase());
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
                case MoonPhase.BloodMoon:
                    return Helper.Get("moon-text.phase-blood");
                case MoonPhase.BlueMoon:
                    return Helper.Get("moon-text.blue-moon");
                case MoonPhase.HarvestMoon:
                    return Helper.Get("moon-text.harvest-moon");
                case MoonPhase.SpiritsMoon:
                    return Helper.Get("moon-text.spirits-moon");
                default:
                    return Helper.Get("moon-text.error");
            }
        }

        public static string DescribeMoonPhase(MoonPhase mp)
        {
            switch (mp)
            {
                case MoonPhase.ErrorPhase:
                    return "ErrorPhase";
                case MoonPhase.FirstQuarter:
                    return "FirstQuarter";
                case MoonPhase.FullMoon:
                    return "FullMoon";
                case MoonPhase.NewMoon:
                    return "NewMoon";
                case MoonPhase.ThirdQuarter:
                    return "ThirdQuarter";
                case MoonPhase.WaningCrescent:
                    return "WaningCrescent";
                case MoonPhase.WaningGibbeous:
                    return "WaningGibbous";
                case MoonPhase.WaxingCrescent:
                    return "WaxingCrescent";
                case MoonPhase.WaxingGibbeous:
                    return "WaxingGibbous";
                case MoonPhase.BloodMoon:
                    return "BloodMoon";
                case MoonPhase.BlueMoon:
                    return "BlueMoon";
                case MoonPhase.HarvestMoon:
                    return "HarvestMoon";
                case MoonPhase.SpiritsMoon:
                    return "SpiritsMoon";
                default:
                    return "ErrorMoon";
            }
        }

        public string DescribeMoonPhase()
        {
            return DescribeMoonPhase(this.CurrentPhase(), LunarDisturbances.Translation);
        }
        public int GetMoonRiseDisplayTime()
        {
            int MoonRise = GetMoonRiseTime();
            if (MoonRise >= 2400)
                return (MoonRise - 2400);
            else
                return MoonRise;
        }
        public int GetMoonRiseTime()
        {
            switch (this.CurrentPhase())
            {
                case MoonPhase.BloodMoon:
                case MoonPhase.HarvestMoon:
                    return 0600;
                case MoonPhase.FullMoon:
                case MoonPhase.BlueMoon:
                    return 2040;
                case MoonPhase.WaningGibbeous:
                    return 2200;
                case MoonPhase.ThirdQuarter:
                    return 2310;
                case MoonPhase.WaningCrescent:                
                    return 2430;
                case MoonPhase.NewMoon:
                case MoonPhase.SpiritsMoon:
                    return 0600;
                case MoonPhase.WaxingCrescent:
                    return 1130;
                case MoonPhase.FirstQuarter:
                    return 1500;
                case MoonPhase.WaxingGibbeous:
                    return 1340;
                case MoonPhase.ErrorPhase:
                default:
                    return 2700;
            }
        }

        public bool IsMoonUp(int time)
        {
            if (time >= GetMoonRiseTime() && time <= GetMoonSetTime())
                return true;

            return false;
        }

        public int GetMoonSetTime()
        {
            //Blood Moons don't set. More's the pity, I guess..
            switch (this.CurrentPhase())
            {
                case MoonPhase.BloodMoon:
                case MoonPhase.HarvestMoon:
                case MoonPhase.SpiritsMoon:
                    return 2700;
                case MoonPhase.FullMoon:
                case MoonPhase.BlueMoon:
                    return 2830;
                case MoonPhase.WaningGibbeous:
                    return 1020;
                case MoonPhase.ThirdQuarter:
                    return 1420;
                case MoonPhase.WaningCrescent:
                    return 1800;
                case MoonPhase.NewMoon:
                    return 2020;
                case MoonPhase.WaxingCrescent:
                    return 2130;
                case MoonPhase.FirstQuarter:
                    return 2250;
                case MoonPhase.WaxingGibbeous:
                    return 2320;
                case MoonPhase.ErrorPhase:
                default:
                    return 0700;
            }
        }    

        public bool CheckForGhostSpawn()
        {
			if (CurrentPhase() is MoonPhase.FullMoon && Dice.NextDouble() < ModConfig.GhostSpawnChance && ModConfig.HazardousMoonEvents)
			{
				return true;
			}
			
            return false;
        }

        public void MoonTrackerUpdate()
        {
            if (MoonTracker.FullMoonThisSeason && this.CurrentPhase() == MoonPhase.FullMoon && !IsBloodMoon)
            {
                IsBlueMoon = true;
                Game1.addHUDMessage(new TCHUDMessage(Translations.Get("moon-text.bluemoon"), CurrentPhase()));
                Game1.player.team.sharedDailyLuck.Value += .025;
            }
            else
            {
                MoonTracker.FullMoonThisSeason = true;
            }
        }
    }
}

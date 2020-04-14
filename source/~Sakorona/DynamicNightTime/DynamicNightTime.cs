using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using TwilightShards.Stardew.Common;
using DynamicNightTime.Integrations;
using xTile.ObjectModel;

namespace DynamicNightTime
{
    public class DynamicNightConfig
    {
        public float Latitude = 38.25f;
        public bool SunsetTimesAreMinusThirty = true;
        public int NightDarknessLevel = 1;
        public bool LessOrangeSunrise = false;
    }

    public class DynamicNightTime : Mod
    {
        public static double SunriseTemp = 2500;
        public static double SunsetTemp = 2700;
        public static double NoonTemp = 6300;
        public static double LateAfternoonTemp = 4250;
        public static double EarlyMorningTemp = 4250;
        public static DynamicNightConfig NightConfig;
        public static IMonitor Logger;
        public static bool LunarDisturbancesLoaded;
        public static ILunarDisturbancesAPI MoonAPI;
        public static bool ClimatesLoaded;
        public static IClimatesOfFerngillAPI ClimatesAPI;
        private bool resetOnWakeup;
        private bool isNightOut;
        private bool firstDaybreakTick;
        private int daybreakTickCount;
        private IDynamicNightAPI API;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            isNightOut = false;
            daybreakTickCount = 11;
            firstDaybreakTick = false;
            Logger = Monitor;
            NightConfig = Helper.ReadConfig<DynamicNightConfig>();
            resetOnWakeup = false;

            //sanity check lat
            if (NightConfig.Latitude > 64)
                NightConfig.Latitude = 64;
            if (NightConfig.Latitude < -64)
                NightConfig.Latitude = -64;
    
            var harmony = HarmonyInstance.Create("koihimenakamura.dynamicnighttime");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //patch getStartingToGetDarkTime
            MethodInfo setStartingToGetDarkTime = SDVUtilities.GetSDVType("Game1").GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().Find(m => m.Name == "getStartingToGetDarkTime");
            MethodInfo postfix = typeof(Patches.GettingDarkPatch).GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().Find(m => m.Name == "Postfix");
            Monitor.Log($"Postfixing {setStartingToGetDarkTime} with {postfix}", LogLevel.Trace);
            harmony.Patch(setStartingToGetDarkTime, postfix: new HarmonyMethod(postfix));

            //patch getTrulyDarkTime
            MethodInfo setTrulyDarkTime = SDVUtilities.GetSDVType("Game1").GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().Find(m => m.Name == "getTrulyDarkTime");
            MethodInfo postfixDark = typeof(Patches.GetFullyDarkPatch).GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().Find(m => m.Name == "Postfix");
            Monitor.Log($"Postfixing {setTrulyDarkTime} with {postfixDark}", LogLevel.Trace);
            harmony.Patch(setTrulyDarkTime, postfix: new HarmonyMethod(postfixDark));

            //patch isDarkOut
            MethodInfo isDarkOut = SDVUtilities.GetSDVType("Game1").GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().Find(m => m.Name == "isDarkOut");
            MethodInfo postfixIsDarkOut = typeof(Patches.IsDarkOutPatch).GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().Find(m => m.Name == "Postfix");
            Monitor.Log($"Postfixing {isDarkOut} with {postfixIsDarkOut}", LogLevel.Trace);
            harmony.Patch(isDarkOut, postfix: new HarmonyMethod(postfixIsDarkOut));

            //patch UpdateGameClock
            MethodInfo UpdateGameClock = helper.Reflection.GetMethod(SDVUtilities.GetSDVType("Game1"), "UpdateGameClock").MethodInfo;
            MethodInfo postfixClock = helper.Reflection.GetMethod(typeof(Patches.GameClockPatch), "Postfix").MethodInfo;
            Monitor.Log($"Postfixing {UpdateGameClock} with {postfixClock}", LogLevel.Trace);
            harmony.Patch(UpdateGameClock, postfix: new HarmonyMethod(postfixClock));

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;
           
            Helper.ConsoleCommands.Add("debug_cycleinfo", "Outputs the cycle information", OutputInformation);
            Helper.ConsoleCommands.Add("debug_outdoorlight", "Outputs the outdoor light information", OutputLight);
            Helper.ConsoleCommands.Add("debug_setlatitude", "Sets Latitude", SetLatitude);
            Helper.ConsoleCommands.Add("debug_setnightlevel", "Set Night Level", SetNightLevel);
            Helper.ConsoleCommands.Add("debug_printplayingsong", "Print Playing Song", PrintPlayingSong);
        }

        private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.isDarkOut() && Game1.currentSong.Name.Contains(Game1.currentSeason) && !Game1.currentSong.Name.Contains("ambient"))
            {
                if (Game1.getMusicTrackName(Game1.MusicContext.Default).StartsWith(Game1.currentSeason) && !Game1.getMusicTrackName(Game1.MusicContext.Default).Contains("ambient") && (!Game1.eventUp && Game1.isDarkOut()))
                    Game1.changeMusicTrack("none", true, Game1.MusicContext.Default);
                if (Game1.currentLocation.IsOutdoors && !Game1.isRaining && (!Game1.eventUp && Game1.getMusicTrackName(Game1.MusicContext.Default).Contains("day")) && Game1.isDarkOut())
                    Game1.changeMusicTrack("none", true, Game1.MusicContext.Default);
                Game1.currentLocation.checkForMusic(Game1.currentGameTime);
            }

            if (Game1.timeOfDay > GetSunriseTime() && !firstDaybreakTick && daybreakTickCount > 0)
                daybreakTickCount--;

            if (Game1.timeOfDay > GetSunriseTime() && !firstDaybreakTick && daybreakTickCount == 0)
            {
                if (Game1.currentSong.Name.Contains(Game1.currentSeason) && !Game1.currentSong.Name.Contains("ambient") && Game1.currentSong.IsStopped)
                {
                    Game1.currentSong.Resume();
                }

                else
                {
                    //check base flags
                    if (!Game1.isRaining || !Game1.isLightning || !Game1.eventUp)
                    {
                        //check locations
                        if ((Game1.currentLocation.IsOutdoors && !(Game1.currentLocation is Desert) || Game1.currentLocation is FarmHouse || Game1.currentLocation is AnimalHouse || Game1.currentLocation is Shed))
                        {
                            //check game config
                            if (Game1.options.musicVolumeLevel > 0.025 && Game1.timeOfDay < 1200)
                            {
                                //check song restrictions
                                if (Game1.currentSong.Name.Contains("ambient"))
                                {
                                    Game1.changeMusicTrack(Game1.currentSeason + Math.Max(1, Game1.currentSongIndex), true, Game1.MusicContext.Default);
                                }
                            }

                        }
                    }
                }
                firstDaybreakTick = true;
            }

        }

        
        private void PrintPlayingSong(string arg1, string[] arg2)
        {
            Console.WriteLine($"Playing song is {Game1.currentSong.Name.ToString()}, stopped? {Game1.currentSong.IsStopped} playing {Game1.currentSong.IsPlaying}");
        }

        /// <summary>Get an API that other mods can access. This is always called after <see cref="M:StardewModdingAPI.Mod.Entry(StardewModdingAPI.IModHelper)" />.</summary>
        public override object GetApi()
        {
            return API ?? (API = new DynamicNightAPI());
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            //handle the game being bad at night->day :|
            if (Game1.timeOfDay < GetSunriseTime())
            {
                Game1.currentLocation.checkForMusic(Game1.currentGameTime);
                Game1.currentLocation.switchOutNightTiles();
                isNightOut = true;
            }

            if (Game1.timeOfDay >= GetSunriseTime() && isNightOut)
            {
                SwitchOutDayTiles(Game1.currentLocation);
                if (Game1.currentSong.Name.Contains(Game1.currentSeason) && !Game1.currentSong.Name.Contains("ambient") && Game1.currentSong.IsStopped)
                    Game1.currentSong.Resume();

                else { 
                if (!Game1.eventUp && (Game1.currentLocation.IsOutdoors || Game1.currentLocation is FarmHouse || Game1.currentLocation is AnimalHouse
                    || Game1.currentLocation is Shed && Game1.options.musicVolumeLevel > 0.025 && Game1.timeOfDay < 1200) && (Game1.currentSong.Name.Contains("ambient") || Game1.currentSong.Name.Contains("none")) || Game1.currentSong.Name.Contains(Game1.currentSeason))
                {

                    if (Game1.isRaining || Game1.isLightning || (Game1.eventUp || Game1.dayOfMonth <= 0) || Game1.currentLocation.Name.Equals("Desert"))
                        return;
                    Game1.changeMusicTrack(Game1.currentSeason + Math.Max(1, Game1.currentSongIndex),true,Game1.MusicContext.Default);
                }
                }
                Helper.Reflection.GetMethod(Game1.currentLocation, "_updateAmbientLighting").Invoke();
                Game1.currentLocation.map.Properties.TryGetValue("Light", out PropertyValue propertyValue2);
                if (propertyValue2 != null && !Game1.currentLocation.ignoreLights.Value)
                {
                    string[] strArray = propertyValue2.ToString().Split(' ');
                    for (int index = 0; index < strArray.Length; index += 3)
                        Game1.currentLightSources.Add(new LightSource(Convert.ToInt32(strArray[index + 2]), new Vector2((float)(Convert.ToInt32(strArray[index]) * 64 + 32), (float)(Convert.ToInt32(strArray[index + 1]) * 64 + 32)), 1f, LightSource.LightContext.MapLight, 0L));                      
                }
                isNightOut = false;
            }
        }

        private void SwitchOutDayTiles(GameLocation loc)
        {
            try
            {
                if (Game1.timeOfDay < 1900 && (!Game1.isRaining || loc.Name.Equals((object)"SandyHouse")))
                {
                    loc.map.Properties.TryGetValue("DayTiles", out PropertyValue propertyValue3);
                    if (propertyValue3 != null)
                    {
                        string[] strArray = propertyValue3.ToString().Trim().Split(' ');
                        for (int index = 0; index < strArray.Length; index += 4)
                        {
                            if ((!strArray[index + 3].Equals("720") || !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade")) && loc.map.GetLayer(strArray[index]).Tiles[Convert.ToInt32(strArray[index + 1]), Convert.ToInt32(strArray[index + 2])] != null)
                                loc.map.GetLayer(strArray[index]).Tiles[Convert.ToInt32(strArray[index + 1]), Convert.ToInt32(strArray[index + 2])].TileIndex = Convert.ToInt32(strArray[index + 3]);
                        }
                    }
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
            }
            if (loc is MineShaft ||loc is Woods)
                return;
            loc.addLightGlows();
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            resetOnWakeup = false;
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.isDarkOut() && !Game1.currentLocation.IsOutdoors && Game1.currentLocation is DecoratableLocation loc && !resetOnWakeup)
            {
                //we need to handle the spouse's room
                if (loc is FarmHouse)
                {
                    if (Game1.timeOfDay < GetSunriseTime())
                        Game1.currentLocation.switchOutNightTiles();
                }
            }
            resetOnWakeup = false;
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            //testing for ZA MOON, YOUR HIGHNESS.
            MoonAPI = SDVUtilities.GetModApi<ILunarDisturbancesAPI>(Monitor, Helper, "KoihimeNakamura.LunarDisturbances", "1.0.7", "Lunar Disturbances");

            if (MoonAPI != null)
                LunarDisturbancesLoaded = true;

            ClimatesAPI = SDVUtilities.GetModApi<IClimatesOfFerngillAPI>(Monitor, Helper, "KoihimeNakamura.ClimatesOfFerngill", "1.5.0-beta15", "Climates of Ferngill");

            if (ClimatesAPI != null)
                ClimatesLoaded = true;

            //GMCM interaction
            var GMCMapi = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (GMCMapi != null)
            {
                GMCMapi.RegisterModConfig(ModManifest, () => NightConfig = new DynamicNightConfig(), () => Helper.WriteConfig(NightConfig));
                GMCMapi.RegisterClampedOption(ModManifest, "Latitude", "Latitude used to generate the sunrise and sunset times", () => NightConfig.Latitude,
                    (float val) => NightConfig.Latitude = val, -63.5f, 63.5f);
                GMCMapi.RegisterSimpleOption(ModManifest, "Sunset Times", "This option controls if you subtract a half hour from the generated time", () => NightConfig.SunsetTimesAreMinusThirty, (bool val) => NightConfig.SunsetTimesAreMinusThirty = val);
                GMCMapi.RegisterSimpleOption(ModManifest, "Less Orange Sunrise", "This option controls if you want a less orange sunrise", () => NightConfig.LessOrangeSunrise, (bool val) => NightConfig.LessOrangeSunrise = val);
                GMCMapi.RegisterClampedOption(ModManifest, "Night Darkness Level", "Controls the options for how dark it is at night. Higher is darker.", () => NightConfig.NightDarknessLevel,
                    (int val) => NightConfig.NightDarknessLevel = val, 1, 4);
            }

        }

        
        private void SetLatitude(string arg1, string[] arg2)
        {
           if (arg2.Length > 0)
            {
                float f = (float)Convert.ToDouble(arg2[0]);
                if (f > 64) {
                    Logger.Log("The set latitude exceeds 64 Degrees North. Resetting to 64 degrees North.", LogLevel.Info);
                    NightConfig.Latitude = 64;
                }
                else if (f < -64)
                {
                    Logger.Log("The set latitude exceeds 64 Degrees South. Resetting to 64 degrees South.", LogLevel.Info);
                    NightConfig.Latitude = -64;
                }
            }
        }

        private void SetNightLevel(string arg1, string[] arg2)
        {
            if (arg2.Length > 0)
            {
                NightConfig.NightDarknessLevel = Convert.ToInt32(arg2[0]);
            }
        }

        private void OutputLight(string arg1, string[] arg2)
        {
            Monitor.Log($"The outdoor light is {Game1.outdoorLight.ToString()}. The ambient light is {Game1.ambientLight.ToString()}",LogLevel.Info);
        }

        private void OutputInformation(string arg1, string[] arg2)
        {
            Monitor.Log($"Game date is {SDate.Now().ToString()}, with config'd latitude being {NightConfig.Latitude}", LogLevel.Info);
            Monitor.Log($"Sunrise : {GetSunrise().ToString()}, Sunset: {GetSunset().ToString()}. Solar Noon {GetSolarNoon().ToString()}", LogLevel.Info);
            Monitor.Log($"Early Morning ends at {GetEndOfEarlyMorning().ToString()}, Late Afternoon begins at {GetBeginningOfLateAfternoon().ToString()}", LogLevel.Info);
            Monitor.Log($"Morning Twilight: {GetMorningAstroTwilight().ToString()}, Evening Twilight: {GetAstroTwilight().ToString()}", LogLevel.Info);
            Monitor.Log($"Game Interval Time is {Game1.gameTimeInterval}", LogLevel.Info);
        }

        public static Color GetLunarLightDifference()
        {
          //full moon +22
          //first/third quarter + 11
          //gibbeous + 17
          //crescent +5

          //light added is only going to be visible in certain times, determined by the code.
          //Full light is when it's angle is between.. 30 and 150 degrees. For us, that should be the middle of 70% of the time.

          int timeRise = MoonAPI.GetMoonRise();
          int timeSet = MoonAPI.GetMoonSet();
          int timeElapsed = SDVTime.MinutesBetweenTwoIntTimes(Game1.timeOfDay, timeRise);
          int totalMinutes = SDVTime.MinutesBetweenTwoIntTimes(timeRise, timeSet);

            if (timeRise > Game1.timeOfDay)
                  return new Color(0,0,0);
            if (Game1.timeOfDay > timeSet)
                  return new Color(0,0,0); 


            float multiply = (float)timeElapsed/totalMinutes;
            if (multiply >= .15 && multiply <= .85)
                  multiply = 1;
             else
            {
                 if (multiply > .85)
                 {
                     multiply -= .85f;   
                 }
                  multiply = (float)(-44.44 * Math.Pow(multiply, 2.0));
            }

            //clamp values
            if (multiply > 1)
                multiply = 1;
            if (multiply < 0)
                multiply = 0; 

          byte colorValR, colorValG, colorValB, val;

          switch (MoonAPI.GetCurrentMoonPhase())
          {
                case "Third Quarter":
                case "First Quarter":
                    val = (byte)(Math.Floor(11 * multiply));
                    colorValB = colorValG = colorValR = val;
                    break;
                case "Full Moon":
                    val = (byte)(Math.Floor(22 * multiply));
                    colorValB = colorValG = colorValR = val;
                    break;
                case "Waning Gibbeous":
                case "Waxing Gibbeous":
                    val = (byte)(Math.Floor(17 * multiply));
                    colorValB = colorValG = colorValR = val;
                    break;
                case "Waning Crescent":
                case "Waxing Crescent":
                    val = (byte)(Math.Floor(5 * multiply));
                    colorValB = colorValG = colorValR = val;
                    break;
                default:
                    colorValR = colorValG = colorValB = 0;
                    break;
          }
            return new Color(colorValR, colorValG, colorValB);
        }
        
        public static byte ClampByteValue(int raw)
        {
            byte R = 0;
            
            if (raw >= 0 && raw <= 255)
                R = (byte)raw;
            else if (raw < 0)
                R = (byte)0;
            else if (raw > 255)
                R = (byte)255;

            return R;
        }

        public static int GetSunriseTime() => GetSunrise().ReturnIntTime();
        public static SDVTime GetMorningAstroTwilight() => GetTimeAtHourAngle(-0.314159265);
        public static SDVTime GetAstroTwilight() => GetTimeAtHourAngle(-0.314159265, false);
        public static SDVTime GetMorningNavalTwilight() => GetTimeAtHourAngle(-0.20944);
        public static SDVTime GetNavalTwilight() => GetTimeAtHourAngle(-0.20944, false);
        public static SDVTime GetCivilTwilight() => GetTimeAtHourAngle(-0.104719755, false);
        public static SDVTime GetMorningCivilTwilight() => GetTimeAtHourAngle(-0.104719755);
        public static SDVTime GetSunrise() => GetTimeAtHourAngle(0.01163611);
        public static SDVTime GetSunriseForDay(SDate day) => GetTimeAtHourAngle(day, 0.01163611);
        public static SDVTime GetSunsetForDay(SDate day)
        {
            SDVTime s = GetTimeAtHourAngle(day, 0.01163611, false);
            if (NightConfig.SunsetTimesAreMinusThirty) s.AddTime(-30);
            return s;
        }

        public static SDVTime GetSunset()
        {
            SDVTime s =  GetTimeAtHourAngle(0.01163611, false);
            if (NightConfig.SunsetTimesAreMinusThirty) s.AddTime(-30);
            return s;
        }

        protected internal static SDVTime GetSolarNoon()
        {
            var date = SDate.Now();
            int dayOfYear = date.DaysSinceStart % 112;
            double noon = 720 - 10 * Math.Sin(2 * (Math.PI / 112) * (dayOfYear)) + 8 * Math.Sin((Math.PI / 112) * dayOfYear);
            
            int noonTime = (int)Math.Floor(noon);

            int hr = (int)Math.Floor(noonTime / 60.0);
            SDVTime calcTime = new SDVTime(hr, noonTime - (hr * 60));
            calcTime.ClampToTenMinutes();
            return calcTime;
        }

        protected internal static SDVTime GetEndOfEarlyMorning()
        {
            SDVTime Noon = GetSolarNoon();
            SDVTime Sunrise = GetSunrise();

            int numMinutes = Noon.GetNumberOfMinutesFromMidnight() - Sunrise.GetNumberOfMinutesFromMidnight();
            int endOfEarlyMorning = (int)Math.Floor(numMinutes * .38);

            SDVTime EndOfEarlyMorning = new SDVTime(Sunrise);
            EndOfEarlyMorning.AddTime(endOfEarlyMorning);
            EndOfEarlyMorning.ClampToTenMinutes();

            return EndOfEarlyMorning;
        }

        protected internal static SDVTime GetBeginningOfLateAfternoon()
        {
            SDVTime Noon = GetSolarNoon();
            SDVTime Sunset = GetSunset();

            int numMinutes = Sunset.GetNumberOfMinutesFromMidnight() - Noon.GetNumberOfMinutesFromMidnight();
            int lateAfternoon = (int)Math.Floor(numMinutes * .62);

            SDVTime LateAfternoon  = new SDVTime(Noon);
            LateAfternoon.AddTime(lateAfternoon);
            LateAfternoon.ClampToTenMinutes();

            return LateAfternoon;
        }

        protected internal static SDVTime GetTimeAtHourAngle(SDate day, double angle, bool morning = true)
        {
            int astroTwN;
            int dayOfYear = day.DaysSinceStart % 112;
            double lat = MathHelper.ToRadians((float)NightConfig.Latitude);
            //23.45 deg * sin(2pi / 112 * (dayOfYear - 1))
            double solarDeclination = .40927971 * Math.Sin((2 * Math.PI / 112) * (dayOfYear - 1));
            double noon = 720 - 10 * Math.Sin(4 * (Math.PI / 112) * (dayOfYear - 1)) + 8 * Math.Sin(2 * (Math.PI / 112) * dayOfYear);
            double astroHA = Math.Acos((Math.Sin(angle) - Math.Sin(lat) * Math.Sin(solarDeclination)) / (Math.Cos(lat) * Math.Cos(solarDeclination)));
            double minHA = (astroHA / (2 * Math.PI)) * 1440;

            if (double.IsNaN(minHA))
            {
                minHA = noon;
            }

            if (!morning)
                astroTwN = (int)Math.Floor(noon + minHA);
            else
                astroTwN = (int)Math.Floor(noon - minHA);

            if (astroTwN < 0)
                astroTwN = 0;
            if (astroTwN > 1560)
                astroTwN = 1560;

            //Conv to an SDV compat time, then clamp it.
            int hr = (int)Math.Floor(astroTwN / 60.0);
            int min = astroTwN - (hr * 60);
            SDVTime calcTime = new SDVTime(hr, min);
            calcTime.ClampToTenMinutes();
            return calcTime;
        }

        protected internal static SDVTime GetTimeAtHourAngle(double angle, bool morning = true)
        {
            int astroTwN;
            int dayOfYear = SDate.Now().DaysSinceStart % 112;
            double lat = MathHelper.ToRadians((float)NightConfig.Latitude);
            //23.45 deg * sin(2pi / 112 * (dayOfYear - 1))
            double solarDeclination = .40927971 * Math.Sin((2 * Math.PI / 112) * (dayOfYear - 1));
            double noon = 720 - 10 * Math.Sin(4 * (Math.PI / 112) * (dayOfYear - 1)) + 8 * Math.Sin(2 * (Math.PI / 112) * dayOfYear);
            double astroHA = Math.Acos((Math.Sin(angle) - Math.Sin(lat) * Math.Sin(solarDeclination)) / (Math.Cos(lat) * Math.Cos(solarDeclination)));
            double minHA = (astroHA / (2 * Math.PI)) * 1440;

            if (double.IsNaN(minHA))
            {
                minHA = noon;
            }

            if (!morning)
                astroTwN = (int)Math.Floor(noon + minHA);
            else
                astroTwN = (int)Math.Floor(noon - minHA);

            if (astroTwN < 0)
                astroTwN = 0;
            if (astroTwN > 1560)
                astroTwN = 1560;

            //Conv to an SDV compat time, then clamp it.
            int hr = (int)Math.Floor(astroTwN / 60.0);
            int min = astroTwN - (hr * 60);
            SDVTime calcTime = new SDVTime(hr, min);
            calcTime.ClampToTenMinutes();
            return calcTime;
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using TwilightShards.Stardew.Common;
using TwilightShards.Common;
using Microsoft.Xna.Framework.Graphics;
using EnumsNET;
using HarmonyLib;
using System.Reflection;
using ClimatesOfFerngillRebuild.Patches;

namespace ClimatesOfFerngillRebuild
{
    public class ClimatesOfFerngill : Mod
    {
        /// <summary> The options file </summary>
        internal static WeatherConfig WeatherOpt { get; set; }

        /// <summary> The pRNG object </summary>
        internal static Random Dice;

        /// <summary> The current weather conditions </summary>
        internal static WeatherConditions Conditions;

        //provide common interfaces for logging, and access to SMAPI APIs
        internal static IMonitor Logger;
        internal static IReflectionHelper Reflection;
        internal static IMultiplayerHelper MPHandler;
        internal static ITranslationHelper Translator;

        /// <summary> The climate for the game </summary>
        private static FerngillClimate GameClimate;

        /// <summary> This is used to display icons on the menu </summary>
        internal static Sprites.Icons OurIcons { get; set; }
        private HUDMessage queuedMsg;

        /// <summary> This is used to allow the menu to revert back to a previous menu </summary>
        private IClickableMenu PreviousMenu;
        private Descriptions DescriptionEngine;
        private Rectangle RWeatherIcon;
        private bool Disabled = false;
        private bool HasGottenSync = false;
        private bool HasRequestedSync = false;
        private static bool IsBloodMoon = false;
        private float weatherX;
        private bool SummitRebornLoaded;

        //experimental var.
        public static float RainX = 0f;
        public static float RainY = 0f;
        public static float WindCap = -40f;
        public static float WindMin = -1.4f;
        public static float WindChance = .01f;
        public static float WindThreshold = -0.5f;

        //Integrations
        internal static bool UseLunarDisturbancesApi = false;
        internal static Integrations.ILunarDisturbancesAPI MoonAPI;
        internal static bool UseSafeLightningApi = false;
        internal static Integrations.ISafeLightningAPI SafeLightningAPI;
        internal static bool UseDynamicNightApi = false;
        internal static Integrations.IDynamicNightAPI DynamicNightAPI;

        /// <summary> Provide an API interface </summary>
        private IClimatesOfFerngillAPI API;
        public override object GetApi()
        {
            if (API == null)
                API = new ClimatesOfFerngillAPI(Conditions, WeatherOpt);

            return API;
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            RWeatherIcon = new Rectangle();
            WeatherOpt = helper.ReadConfig<WeatherConfig>();
            Logger = Monitor;
            Translator = Helper.Translation;
            Reflection = Helper.Reflection;
            MPHandler = Helper.Multiplayer;
            Dice = new Xoshiro.PRNG64.XoShiRo256starstar();
            OurIcons = new Sprites.Icons(Helper.Content);
            WeatherProcessing.Init();
            Conditions = new WeatherConditions();
            DescriptionEngine = new Descriptions(WeatherOpt, Helper.Translation);
            queuedMsg = null;
            SummitRebornLoaded = false;

            if (WeatherOpt.Verbose) Monitor.Log($"Loading climate type: {WeatherOpt.ClimateType} from file", LogLevel.Trace);

            var path = Path.Combine("assets", "climates", WeatherOpt.ClimateType + ".json");
            GameClimate = helper.Data.ReadJsonFile<FerngillClimate>(path); 
            
            if (GameClimate is null)
            {
                this.Monitor.Log($"The required '{path}' file is missing. Try reinstalling the mod to fix that.", LogLevel.Error);
                this.Monitor.Log("This mod will now disable itself.", LogLevel.Error);
                this.Disabled = true;
            }

            if (Disabled) return;

            var harmony = new Harmony("koihimenakamura.climatesofferngill");
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "drawAboveAlwaysFrontLayer"),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(GameLocationPatches), "DAAFLTranspiler")));
            Monitor.Log("Patching GameLocation::drawAboveAlwaysFrontLayer with a Transpiler.", LogLevel.Trace);

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "drawWeather"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Game1Patches), "DrawWeatherPrefix")));
            Monitor.Log("Patching Game1::drawWeather with a prefix method.", LogLevel.Trace);

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "updateWeather"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Game1Patches), "UpdateWeatherPrefix")));
            Monitor.Log("Patching Game1::updateWeather with a prefix method.", LogLevel.Trace);

            harmony.Patch(
                original: AccessTools.Method(AccessTools.TypeByName("StardewModdingAPI.Framework.SGame"), "DrawImpl"),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(SGamePatches), "DrawImplTranspiler")));
            Monitor.Log("Patching SMAPI (SGame::DrawImpl) with a transpiler.", LogLevel.Trace);

            harmony.Patch(
                original: AccessTools.Constructor(AccessTools.TypeByName("StardewValley.WeatherDebris"), new[] { typeof(Vector2), typeof(int), typeof(float), typeof(float), typeof(float) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(WeatherDebrisPatches), "CtorPostfix")));
            Monitor.Log("Patching WeatherDebris's constructor method with a postfix method.", LogLevel.Trace);

            harmony.Patch(
                original: AccessTools.Method(typeof(WeatherDebris), "draw"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(WeatherDebrisPatches), "DrawPrefix")));
            Monitor.Log("Patching WeatherDebris::draw with a prefix method.", LogLevel.Trace);

            harmony.Patch(
                original: AccessTools.Method(typeof(WeatherDebris), "update", new[] { typeof(bool) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(WeatherDebrisPatches), "UpdatePrefix")));
            Monitor.Log("Patching WeatherDebris::draw with a prefix method.", LogLevel.Trace);

            //INSERT DNT CHECKS HERE
            var modManifest = Helper.ModRegistry.Get("knakamura.dynamicnighttime");
            if (modManifest != null)
            {
                if (modManifest.Manifest.Version.IsOlderThan("1.2.11"))
                {
                    Monitor.Log("WARNING: Overcast weather may not work correctly unless you are running Dynamic Night Time 1.2.11 or later", LogLevel.Alert);
                }                
            }
            else
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), "UpdateGameClock"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(GameClockPatch), "Postfix")));
                Monitor.Log("Patching Game1::UpdateGameClock with a Postfix method. (Used only when Climates is installed and DNT is not.)", LogLevel.Trace);
            }

            ConsoleCommands.Init();
            SanityCheckConfigOptions();

            //subscribe to events
            var events = helper.Events;
            events.GameLoop.DayStarted += OnDayStarted;
            events.GameLoop.Saving += OnSaving;
            events.GameLoop.TimeChanged += OnTimeChanged;
            events.Display.MenuChanged += OnMenuChanged;
            events.GameLoop.UpdateTicked += OnUpdateTicked;
            events.GameLoop.GameLaunched += OnGameLaunched;
            events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            events.GameLoop.SaveLoaded += OnSaveLoaded;
            events.Display.RenderedWorld += Display_RenderedWorld;
            //events.Display.RenderingHud += OnRenderingHud;
            events.Display.RenderedHud += OnRenderedHud;
            events.Input.ButtonPressed += OnButtonPressed;
            events.Multiplayer.ModMessageReceived += OnModMessageRecieved;

            //console commands
            helper.ConsoleCommands
                .Add("world_tmrwweather", helper.Translation.Get("console-text.desc_tmrweather"), ConsoleCommands.TomorrowWeatherChangeFromConsole)
                .Add("world_setweather", helper.Translation.Get("console-text.desc_setweather"), ConsoleCommands.WeatherChangeFromConsole)
                .Add("debug_clearspecial", "debug command to clear special weathers", ConsoleCommands.ClearSpecial)
                .Add("debug_weatherstatus", "!", ConsoleCommands.OutputWeather)
                .Add("debug_sswa", "Show Special Weather", ConsoleCommands.ShowSpecialWeather)
                .Add("debug_vrainc", "Set Rain Amt.", ConsoleCommands.SetRainAmt)
                .Add("debug_raindef", "Set Rain Deflection.", ConsoleCommands.SetRainDef)
            /*
                .Add("debug_setwindchance", "Set Wind Chance", ConsoleCommands.SetWindChance)
                .Add("debug_setwindtresh", "Set Wind Threshold", ConsoleCommands.SetWindThreshold)
                .Add("debug_setwindrange", "Set Wind Range", ConsoleCommands.SetWindRange)
                .Add("debug_raintotal", "Get Rain Total", ConsoleCommands.DisplayRainTotal)
                .Add("debug_getcurrentwind", "Show wind values", ConsoleCommands.ShowWind)
                .Add("debug_resetwind", "Reset Global Wind", ConsoleCommands.ResetGlobalWind)
            */
                .Add("debug_printClimate", "Print Climate Tracker Data", ConsoleCommands.DisplayClimateTrackerData);
        }

        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.currentLocation.IsOutdoors)
                Conditions.DrawWeathers();

            if (Game1.isRaining && Game1.currentLocation.IsOutdoors && (Game1.currentLocation is Summit) && !SummitRebornLoaded &&
                (!Game1.eventUp || Game1.currentLocation.isTileOnMap(new Vector2((float)(Game1.viewport.X / Game1.tileSize), (float)(Game1.viewport.Y / Game1.tileSize)))))
            {
                for (int index = 0; index < Game1.rainDrops.Length; ++index)
                {
                    Game1.spriteBatch.Draw(Game1.rainTexture, Game1.rainDrops[index].position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, Game1.rainDrops[index].frame, -1, -1)), Color.White);
                }

                if (WeatherOpt.ShowSummitClouds)
                {
                    int num2 = -61 * GetPixelZoom();
                    while (num2 < Game1.viewport.Width + 61 * GetPixelZoom())
                    {
                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)num2 + this.weatherX % (float)(61 * GetPixelZoom()), (float)(-Game1.tileSize / 2)), new Rectangle?(new Rectangle(643, 1142, 61, 53)), Color.DarkSlateGray * 1f, 0.0f, Vector2.Zero, (float)GetPixelZoom(), SpriteEffects.None, 1f);
                        num2 += 61 * GetPixelZoom();
                    }
                }
            }
        }

        public static float WindOverrideSpeed = 0f;

        private void SanityCheckConfigOptions()
        {
            if (WeatherOpt.MaxRainFall < 0)
                WeatherOpt.MaxRainFall = 0;

            if (WeatherOpt.MaxRainFall > WeatherUtilities.GetRainCategoryUpperBound(RainLevels.NoahsFlood))
                WeatherOpt.MaxRainFall = WeatherUtilities.GetRainCategoryUpperBound(RainLevels.NoahsFlood);
        }

        private static int GetPixelZoom()
        {
            FieldInfo field = typeof(Game1).GetField(nameof(Game1.pixelZoom), BindingFlags.Public | BindingFlags.Static);
            if (field == null)
                throw new InvalidOperationException($"The {nameof(Game1)}.{nameof(Game1.pixelZoom)} field could not be found.");
            return (int)field.GetValue(null);
        }

        /// <summary>Raised once per second after the game state is updated.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            Conditions.SecondUpdate();
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var modManifest = Helper.ModRegistry.Get("KoihimeNakamura.summitreborn");
            if (modManifest != null)
            {
                SummitRebornLoaded = true;
                Logger.Log("Summit Reborn loaded. Disabling summit redraw.");
            }

            //testing for ZA MOON, YOUR HIGHNESS.
            MoonAPI = SDVUtilities.GetModApi<Integrations.ILunarDisturbancesAPI>(Monitor, Helper, "KoihimeNakamura.LunarDisturbances", "1.0", "Lunar Disturbances");
            SafeLightningAPI = SDVUtilities.GetModApi<Integrations.ISafeLightningAPI>(Monitor, Helper, "cat.safelightning", "1.0", "Safe Lightning");
            DynamicNightAPI = SDVUtilities.GetModApi<Integrations.IDynamicNightAPI>(Monitor, Helper, "knakamura.dynamicnighttime", "1.1-rc3", "Dynamic Night Time");

            if (MoonAPI != null)
                UseLunarDisturbancesApi = true;
                    
            if (SafeLightningAPI != null)
                UseSafeLightningApi = true;

            if (DynamicNightAPI != null)
                UseDynamicNightApi = true;
        }
        
        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //CustomTVMod.changeAction("weather", DisplayWeather);

            if (Context.IsMainPlayer)
            {
	            Conditions.trackerModel = Helper.Data.ReadSaveData<ClimateTracker>("climate-tracker");

                if (Conditions.trackerModel is null)
                    Conditions.trackerModel = new ClimateTracker();

                if (Conditions.trackerModel?.TempsOnNextDay != null || !Conditions.trackerModel.TempsOnNextDay.IsDefault() )
                    Conditions.SetTodayTemps(Conditions.trackerModel.TempsOnNextDay);
            }
        } 
        
        public void DisplayWeather(TV tv, TemporaryAnimatedSprite sprite, Farmer who, string answer)
        {
            TemporaryAnimatedSprite BackgroundSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(497, 305, 42, 28), 9999f, 1, 999999, tv.getScreenPosition(), false, false, (float)((tv.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, tv.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f, false);
            TemporaryAnimatedSprite WeatherSprite = DescriptionEngine.GetWeatherOverlay(Conditions, tv);

            string OnScreenText = "";

            if (BackgroundSprite is null)
                Monitor.Log("Background Sprite is null");
            if (WeatherSprite is null)
                Monitor.Log("Weather Sprite is null");

            string MoonPhase = "";
			bool MoonIsUp = false;
            if (UseLunarDisturbancesApi){                
				MoonPhase = MoonAPI.GetCurrentMoonPhase();
				MoonIsUp = MoonAPI.IsMoonUp(Game1.timeOfDay);
			}

            double fog = ClimatesOfFerngill.GetClimateForDay(SDate.Now().AddDays(1)).RetrieveOdds(Dice, "fog", SDate.Now().AddDays(1).Day);

            OnScreenText += DescriptionEngine.GenerateTVForecast(Conditions, Dice, fog, MoonPhase, MoonIsUp);

            //CustomTVMod.showProgram(BackgroundSprite, OnScreenText, CustomTVMod.endProgram, WeatherSprite);
        }

        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.currentLocation.IsOutdoors)
                Conditions.DrawWeathers();

            if (Game1.isRaining && Game1.currentLocation.IsOutdoors && (Game1.currentLocation is Summit) && !SummitRebornLoaded &&
                (!Game1.eventUp || Game1.currentLocation.isTileOnMap(new Vector2((float)(Game1.viewport.X / Game1.tileSize), (float)(Game1.viewport.Y / Game1.tileSize)))))
            {
                for (int index = 0; index < Game1.rainDrops.Length; ++index)
                {
                    Game1.spriteBatch.Draw(Game1.rainTexture, Game1.rainDrops[index].position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, Game1.rainDrops[index].frame, -1, -1)), Color.White);
                }

                if (WeatherOpt.ShowSummitClouds)
                {
                    int num2 = -61 * GetPixelZoom();
                    while (num2 < Game1.viewport.Width + 61 * GetPixelZoom())
                    {
                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)num2 + this.weatherX % (float)(61 * GetPixelZoom()), (float)(-Game1.tileSize / 2)), new Rectangle?(new Rectangle(643, 1142, 61, 53)), Color.DarkSlateGray * 1f, 0.0f, Vector2.Zero, (float)GetPixelZoom(), SpriteEffects.None, 1f);
                        num2 += 61 * GetPixelZoom();
                    }
                }
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            // restore previous menu on close
            if (e.OldMenu is WeatherMenu && this.PreviousMenu != null)
            {
                Game1.activeClickableMenu = this.PreviousMenu;
                this.PreviousMenu = null;
            }

            // handle new dialogue box
            else if (e.NewMenu is DialogueBox box)
            {
                double odds = Dice.NextDouble(), stormOdds = GameClimate.GetStormOdds(SDate.Now().AddDays(1), Dice);
                WeatherProcessing.HandleStormTotemInterception(box, odds, stormOdds);
            }
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer) return;

            Conditions.OnSaving();           

	        if (Conditions.trackerModel is null)
	    	    Conditions.trackerModel = new ClimateTracker();
	    
            this.Helper.Data.WriteSaveData("climate-tracker", Conditions.trackerModel);
            queuedMsg = WeatherProcessing.HandleOnSaving(Conditions, Dice); //handles crop death and any other weather conditions that need to be handled on save.
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Context.IsMultiplayer && !Context.IsMainPlayer && Context.IsPlayerFree && !HasRequestedSync)
            {
                Monitor.Log("Requesting climate information");
                this.Helper.Multiplayer.SendMessage<long>(Game1.player.UniqueMultiplayerID, "NewFarmHandJoin", new[] {"KoihimeNakamura.ClimatesOfFerngill" });
                HasRequestedSync = true;
            }

            if (Game1.currentGameTime != null)
            {
                this.weatherX += (float)Game1.currentGameTime.ElapsedGameTime.Milliseconds * 0.03f;
            }

            // check for changes
            Conditions.MoveWeathers();  
            
            if (UseLunarDisturbancesApi)
            {
                if (MoonAPI.GetCurrentMoonPhase() == "Blood Moon") 
                { 
                    IsBloodMoon = true;
                }
            }
            
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!Game1.hasLoadedGame)
                return;

            if (!Context.IsMainPlayer)
                return;

            Conditions.TenMinuteUpdate();

            if (Game1.currentLocation.IsOutdoors && Conditions.HasWeather(CurrentWeather.Lightning) && !Conditions.HasWeather(CurrentWeather.Rain) && Game1.timeOfDay < 2400)
                Utility.performLightningUpdate(Game1.timeOfDay);

            //queued messages clear
            if (Game1.timeOfDay == 610 && queuedMsg != null)
            {
                Game1.hudMessages.Add(queuedMsg);
                queuedMsg = null;
            }

            WeatherProcessing.ProcessHazardousCropWeather(Conditions, Game1.timeOfDay, Dice);

            ClimatesOfFerngill.WindOverrideSpeed = 0f; //reset once time has passed.
        }

        /// <summary>Raised after drawing the HUD (item toolbar, clock, etc) to the sprite batch, but before it's rendered to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            //float shadowMult = 0f;
            if (!Context.IsWorldReady)
                return;
          
            var weatherMenu = Game1.onScreenMenus.OfType<DayTimeMoneyBox>().FirstOrDefault();

            if (weatherMenu == null)
                return;
            // abort abort abort (maybe another mod replaced it?)

            if (WeatherOpt.EnableCustomWeatherIcon) 
            { 
                //determine icon offset  
                if (!Game1.eventUp)
                {
                    if ((int)Conditions.CurrentWeatherIcon != (int)WeatherIcon.IconError)
                    {
                        RWeatherIcon = new Rectangle(0 + 12 * (int)Conditions.CurrentWeatherIcon, Game1.isDarkOut() ? 8 : 0, 12, 8);
                    }

                    if ((int)Conditions.CurrentWeatherIcon == (int)WeatherIcon.IconBloodMoon)
                    {
                        RWeatherIcon = new Rectangle(144, 8, 12, 8);
                    }

                    if ((int)Conditions.CurrentWeatherIcon == (int)WeatherIcon.IconError)
                    {
                        RWeatherIcon = new Rectangle(144, 0, 12, 8);
                    }

                    Game1.spriteBatch.Draw(OurIcons.WeatherSource, weatherMenu.position + new Vector2(116f, 68f), RWeatherIcon, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, .1f);
                }

                //redraw mouse cursor
                if (WeatherOpt.RedrawCursor)
                    Program.gamePtr.drawMouseCursor();
            }
        }       
        
        private void OnModMessageRecieved(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == "KoihimeNakamura.ClimatesOfFerngill" && e.Type == "NewFarmHandJoin" && Context.IsMainPlayer && !HasGottenSync)
            {
                if (!Conditions.IsTodayTempSet)
                    Conditions.SetTodayTemps(GameClimate.GetTemperatures(SDate.Now(), Dice));

                WeatherSync message = Conditions.GenerateWeatherSyncMessage();
                MPHandler.SendMessage<WeatherSync>(message,"WeatherSync",new[] {"KoihimeNakamura.ClimatesOfFerngill" },new[] {e.FromPlayerID });
                HasGottenSync = true;
            }

            if (e.FromModID == "KoihimeNakamura.ClimatesOfFerngill" && e.Type == "WeatherSync")
            {
                WeatherSync message = e.ReadAs<WeatherSync>();
                if (WeatherOpt.Verbose)
                {
                    Monitor.Log($"Message contents at {Game1.timeOfDay} : {GenSyncMessageString(message)}");
                }                
                Conditions.SetSync(message);
            }
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Conditions.Reset();
            IsBloodMoon = false;
            WeatherProcessing.Reset();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Conditions.OnNewDay();
            IsBloodMoon = false;

            if (!Context.IsMainPlayer) return;

            WeatherProcessing.OnNewDay();
            UpdateWeatherOnNewDay();
            SetTomorrowWeather();            
                       
            WeatherSync message = Conditions.GenerateWeatherSyncMessage();
            MPHandler.SendMessage(message, "WeatherSync", modIDs: new[] { ModManifest.UniqueID });
        }

        private string GenSyncMessageString(WeatherSync ws)
        {
            string s = $"WeatherType: {ws.weatherType}, with today temps ({ws.todayLow},{ws.todayHigh}), tomorrow temps ({ws.tommorowLow},{ws.tommorowHigh}).";
            s += $"{Environment.NewLine}Variable Rain: {ws.isVariableRain}, overcast: {ws.isOvercast}, rainAmt: {ws.rainAmt}";
            s += $"{Environment.NewLine}Blizzard: {ws.isBlizzard}, time: ({ws.blizzWeatherBeginTime},{ws.blizzWeatherEndTime}).";
            s += $"{Environment.NewLine}Fog: {ws.isFoggy}, time: ({ws.fogWeatherBeginTime},{ws.fogWeatherEndTime}).";
            s += $"{Environment.NewLine}WhiteOut: {ws.isWhiteOut}, time: ({ws.whiteWeatherBeginTime},{ws.whiteWeatherEndTime}).";
            s += $"{Environment.NewLine}Thunder: {ws.isThunderFrenzy}, time: ({ws.thunWeatherBeginTime},{ws.thunWeatherEndTime}).";
            s += $"{Environment.NewLine}Sandstorm: {ws.isSandstorm}, time: ({ws.sandstormWeatherBeginTime},{ws.sandstormWeatherEndTime}).";

            return s;
        }

        private void SetTomorrowWeather()
        {
            //if tomorrow is a festival or wedding, we need to set the weather and leave.
            if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
            {
                Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = Game1.weather_festival;
                Conditions.HaltWeatherSystem();

                if (WeatherOpt.Verbose)
                    Monitor.Log($"Festival tomorrow. Aborting processing.", LogLevel.Trace);
                
                //if (WeatherOpt.Verbose) Monitor.Log(DebugOutput.ToString());
                return;
            }

            if (Game1.player.spouse != null && Game1.player.isEngaged() && Game1.player.friendshipData[Game1.player.spouse].CountdownToWedding == 1)
            {
                Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = Game1.weather_wedding;
                Conditions.HaltWeatherSystem();

                if (WeatherOpt.Verbose)
                    Monitor.Log($"Wedding tomorrow. Aborting processing.", LogLevel.Trace);

                return;
            }

            if (WeatherUtilities.CheckForForceDay(DescriptionEngine, SDate.Now().AddDays(1),Monitor, WeatherOpt.Verbose))
            {
                Conditions.HaltWeatherSystem();
                if (WeatherOpt.Verbose)
                    Monitor.Log($"The game will force tomorrow. Aborting processing.", LogLevel.Trace);
                return;
            }

            if (WeatherOpt.Verbose)
                Monitor.Log("Setting weather for tomorrow");

            //now set tomorrow's weather
            var OddsForTheDay = GameClimate.GetClimateForDate(SDate.Now().AddDays(1));

            //get system odds
            double rainSystem = OddsForTheDay.RetrieveSystemOdds("rain");
            double windSystem = OddsForTheDay.RetrieveSystemOdds("debris");
            double sunSystem = OddsForTheDay.RetrieveSystemOdds("sunny");
			
			if (!Game1.player.mailReceived.Contains("ccDoorUnlock")){
				rainSystem = Math.Max(rainSystem-.1, 0);
				windSystem = Math.Max(windSystem-.1,0);
				sunSystem = Math.Min(sunSystem+.2,1);
			}
			

            //get loose odds
            double rainDays = OddsForTheDay.RetrieveOdds(Dice, "rain", SDate.Now().AddDays(1).Day);
            double windyDays = OddsForTheDay.RetrieveOdds(Dice, "debris", SDate.Now().AddDays(1).Day);
            double stormDays = OddsForTheDay.RetrieveOdds(Dice, "storm", SDate.Now().AddDays(1).Day);
			
			if (!Game1.player.mailReceived.Contains("ccDoorUnlock")){
				rainDays = Math.Max(rainDays-.1, 0);
				windyDays = Math.Max(windyDays - .1, 0);
			}

            if (WeatherOpt.Verbose)
            {
                Monitor.Log($"Weather Odds are Rain: {rainDays:N3}, Windy: {windyDays:N3}, and Storm {stormDays:N3}");
                Monitor.Log($"Weather System Odds are Rain: {rainSystem:N3}, Windy: {windSystem:N3}, and Storm {sunSystem:N3}");
            }

            //set tomorrow's weather
            if (Conditions.trackerModel.IsWeatherSystem)
            {
                double SystemContinuesOdds = WeatherProcessing.GetWeatherSystemOddsForNewDay(Conditions);
                if (WeatherOpt.Verbose)
                    Monitor.Log($"Rolling system odds against {SystemContinuesOdds:N3}");

                if (Dice.NextDouble() < SystemContinuesOdds)
                {
                    if (WeatherOpt.Verbose)
                        Monitor.Log($"Continuing system odds - now for {Conditions.trackerModel.WeatherSystemDays + 1} for weather {Conditions.trackerModel.WeatherSystemType}");
                    Conditions.trackerModel.WeatherSystemDays++;
                    WeatherProcessing.SetWeatherTomorrow(Conditions.trackerModel.WeatherSystemType, Dice, GameClimate, stormDays, Conditions.GetTomorrowTemps());
                }

                else
                {
                    if (WeatherOpt.Verbose)
                        Monitor.Log($"Ending system, new weather type.");
                    Conditions.trackerModel.IsWeatherSystem = false;
                    WeatherProcessing.SetWeatherNonSystemForTomorrow(Dice, GameClimate, rainDays, stormDays, windyDays, Conditions.GetTomorrowTemps());
                }
            }
            else
            {
                if (Dice.NextDouble() < WeatherOpt.WeatherSystemChance)
                {
                    if (WeatherOpt.Verbose)
                        Monitor.Log($"Rolling system odds against {WeatherOpt.WeatherSystemChance:N3}, created system.");

                    ProbabilityDistribution<string> SystemDist = new ProbabilityDistribution<string>();

                    SystemDist.AddNewCappedEndPoint(rainSystem, "rain");
                    SystemDist.AddNewCappedEndPoint(windSystem, "debris");
                    SystemDist.AddNewCappedEndPoint(sunSystem, "sunny");

                    double distOdd = Dice.NextDouble();

                    if (!(SystemDist.GetEntryFromProb(distOdd, out string Result)))
                    {
                        Result = "sunny";
                        Monitor.Log("The weather has failed to process in some manner. Falling back to [sunny]", LogLevel.Info);
                    }

                    Conditions.SetNewWeatherSystem(Result, 1);
                    WeatherProcessing.SetWeatherTomorrow(Result, Dice, GameClimate, stormDays, Conditions.GetTomorrowTemps());
                }
                else
                    WeatherProcessing.SetWeatherNonSystemForTomorrow(Dice, GameClimate, rainDays, stormDays, windyDays, Conditions.GetTomorrowTemps());
            }
        }

        internal static FerngillClimateTimeSpan GetClimateForDay(string season, int day)
        {
            return GameClimate.GetClimateForDate(new SDate(day, season));
        }

        internal static FerngillClimateTimeSpan GetClimateForDay(SDate Day)
        {
            return GameClimate.GetClimateForDate(Day);
        }

        private void UpdateWeatherOnNewDay()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            if (Game1.dayOfMonth == 0) //do not run on day 0.
                return;

            int loopCount = 0;

            //Set Temperature for today and tommorow. Get today's conditions.
            //   If tomorrow is set, move it to today, and autoregen tomorrow.
            //   *201711 Due to changes in the object, it auto attempts to update today from tomorrow.

            if (!Conditions.IsTodayTempSet)
            {
                if (!Conditions.IsTomorrowTempSet)
                {
                    Conditions.SetTodayTemps(GameClimate.GetTemperatures(SDate.Now(), Dice));
                    if (Game1.weatherForTomorrow == Game1.weather_snow)
                    {
                        while (!WeatherConditions.IsValidWeatherForSnow(Conditions.GetTodayTemps()) && loopCount <= 1000)
                        {
                            Conditions.SetTodayTemps(GameClimate.GetTemperatures(SDate.Now(), Dice));
                            loopCount++;
                        }
                    }
                }
                else
                    Conditions.SetTodayTempsFromTomorrow();

                Conditions.SetTomorrowTemps(GameClimate.GetTemperatures(SDate.Now().AddDays(1), Dice));
            }

            
            if (WeatherOpt.Verbose)
                Monitor.Log($"Updated the temperature for tommorow and today. Setting weather for today... ", LogLevel.Trace);

            //if today is a festival or wedding, do not go further.
            if (Conditions.GetCurrentConditions().HasAnyFlags(CurrentWeather.Festival | CurrentWeather.Wedding))
            {
                if (WeatherOpt.Verbose)
                    Monitor.Log("It is a wedding or festival today. Not attempting to run special weather or fog.");

                return;
            }

            //variable rain conditions 
            WeatherProcessing.DynamicRainOnNewDay(Conditions, Dice);
            if (!Conditions.IsVariableRain)
                WeatherProcessing.CheckForStaticRainChanges(Conditions, Dice, GameClimate.ChanceForNonNormalRain);
            

            if (WeatherProcessing.TestForSpecialWeather(Conditions, GameClimate.GetClimateForDate(SDate.Now())))
            {           
                if (WeatherOpt.Verbose)
                    Monitor.Log("Special weather created!");
                Conditions.UpdateClimateTracker();
            }
        }

        internal static void ForceVariableRain()
        {
            Conditions.SetVariableRain(true);
        }

        public static bool ShouldPrecipInLocation()
        {
            return true;
        }

        public static Color GetRainColor()
        {
            if (ClimatesOfFerngill.IsBloodMoon)
                return Color.Red;
            else
               return Color.White;
        }
        public static bool ShouldDarken()
        {
            if (Game1.isRaining)
                return true;
            if (Conditions.HasWeather(CurrentWeather.Overcast))
                return true;

            return false;
        }

        public static Color GetRainBackColor()
        {
            return Color.Blue;
        }

        public static Color GetSnowColor()
        {
            if (ClimatesOfFerngill.IsBloodMoon)
                return Color.Red;
            else
                return Color.White;
        }

        #region Menu
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (WeatherOpt.WeatherMenuToggle != e.Button)  //sanity force this to exit!
                return;

            if (!Game1.hasLoadedGame)
                return;

            // perform bound action ONLY if there is no menu OR if the menu is a WeatherMenu
            if (Game1.activeClickableMenu == null || Game1.activeClickableMenu is WeatherMenu)
            {
                this.ToggleMenu();
            }
        }

        /// <summary>
        /// Toggle the menu visiblity
        /// </summary>
        private void ToggleMenu()
        {
            if (Game1.activeClickableMenu is WeatherMenu)
                this.HideMenu();
            else
                this.ShowMenu();
        }

        /// <summary>
        /// Show the menu
        /// </summary>
        private void ShowMenu()
        {
            string MoonPhase = "";
            if (UseLunarDisturbancesApi)
                MoonPhase = MoonAPI.GetCurrentMoonPhase();

            string NightTime = "";
            if (UseDynamicNightApi)
                NightTime = Helper.Translation.Get("nightTime",
                    new
                    {
                        sunrise = new SDVTime(DynamicNightAPI.GetSunriseTime()),
                        sunset = new SDVTime(DynamicNightAPI.GetSunsetTime()),
                        nighttime = new SDVTime(DynamicNightAPI.GetAstroTwilightTime())
                    });

            string MenuText = DescriptionEngine.GenerateMenuPopup(Conditions, MoonPhase, NightTime);

            // show menu
            this.PreviousMenu = Game1.activeClickableMenu;
            Game1.activeClickableMenu = new WeatherMenu(OurIcons, Conditions, MenuText);
        }

        /// <summary>
        /// Hide the menu.
        /// </summary>
        private void HideMenu()
        {
            if (Game1.activeClickableMenu is WeatherMenu)
            {
                Game1.playSound("bigDeSelect"); // match default behaviour when closing a menu
                Game1.activeClickableMenu = null;
            }
        }
        #endregion

        internal static void GetTodayTemps()
        {
            Conditions.SetTodayTemps(GameClimate.GetTemperatures(SDate.Now(), Dice));
        }

        internal static void GetTomorrowTemps()
        {
            Conditions.SetTomorrowTemps(GameClimate.GetTemperatures(SDate.Now().AddDays(1), Dice));
        }
     
    }
}
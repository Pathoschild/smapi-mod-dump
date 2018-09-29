#region headers
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using TwilightShards.Stardew.Common;
using TwilightShards.Common;
using Microsoft.Xna.Framework.Graphics;
using EnumsNET;
using PyTK.CustomTV;
#endregion

namespace ClimatesOfFerngillRebuild
{
    public class ClimatesOfFerngill : Mod
    {
        /// <summary> The options file </summary>
        private WeatherConfig WeatherOpt { get; set; }

        /// <summary> The pRNG object </summary>
        private MersenneTwister Dice;

        /// <summary> The current weather conditions </summary>
        private WeatherConditions Conditions;

        /// <summary> The climate for the game </summary>
        private FerngillClimate GameClimate;

        /// <summary> This is used to display icons on the menu </summary>
        private Sprites.Icons OurIcons { get; set; }
        private StringBuilder DebugOutput;

        /// <summary> The moon object </summary>
        private SDVMoon OurMoon;

        //for stamina management
        private StaminaDrain StaminaMngr;
        private int TicksOutside;
        private int TicksTotal;
        private int ExpireTime;
        private List<Vector2> CropList;
        private HUDMessage queuedMsg;

        /// <summary> This is used to allow the menu to revert back to a previous menu </summary>
        private IClickableMenu PreviousMenu;
        private Descriptions DescriptionEngine;
        private Rectangle RWeatherIcon;
        private Color nightColor = new Color((int)byte.MaxValue, (int)byte.MaxValue, 0);
        private bool Disabled = false;
        private int[] SeedsForDialogue;
        public bool IsEclipse { get; set; }
        public int ResetTicker { get; set; }

        private bool IsFestivalDay => Utility.isFestivalDay(SDate.Now().Day, SDate.Now().Season);

        /// <summary> Main mod function. </summary>
        /// <param name="helper">The helper. </param>
        public override void Entry(IModHelper helper)
        {
            RWeatherIcon = new Rectangle();
            WeatherOpt = helper.ReadConfig<WeatherConfig>();
            Dice = new MersenneTwister();
            DebugOutput = new StringBuilder();
            OurMoon = new SDVMoon(WeatherOpt, Dice);
            OurIcons = new Sprites.Icons(Helper.Content);
            CropList = new List<Vector2>();
            Conditions = new WeatherConditions(OurIcons, Dice, Helper.Translation, Monitor, WeatherOpt);
            StaminaMngr = new StaminaDrain(WeatherOpt, Helper.Translation, Monitor);
            SeedsForDialogue = new int[] { Dice.Next(), Dice.Next() };
            DescriptionEngine = new Descriptions(Helper.Translation, Dice, WeatherOpt, Monitor);
            queuedMsg = null;
            Vector2 snowPos = Vector2.Zero;
            TicksOutside = 0;
            TicksTotal = 0;
            ExpireTime = 0;

            if (WeatherOpt.Verbose) Monitor.Log($"Loading climate type: {WeatherOpt.ClimateType} from file", LogLevel.Trace);

            string path = Path.Combine("data", "weather", WeatherOpt.ClimateType + ".json");
            GameClimate = helper.ReadJsonFile<FerngillClimate>(path); 
            
            if (GameClimate is null)
            {
                this.Monitor.Log($"The required '{path}' file is missing. Try reinstalling the mod to fix that.", LogLevel.Error);
                this.Monitor.Log("This mod will now disable itself.", LogLevel.Error);
                this.Disabled = true;
            }

            if (!Disabled)
            {
                //subscribe to events
                TimeEvents.AfterDayStarted += HandleNewDay;
                SaveEvents.BeforeSave += OnEndOfDay;
                TimeEvents.TimeOfDayChanged += TenMinuteUpdate;
                MenuEvents.MenuChanged += MenuEvents_MenuChanged;
                GameEvents.UpdateTick += CheckForChanges;
                SaveEvents.AfterReturnToTitle += ResetMod;
                SaveEvents.AfterLoad += SaveEvents_AfterLoad;
                GraphicsEvents.OnPostRenderGuiEvent += DrawOverMenus;
                GraphicsEvents.OnPreRenderHudEvent += DrawPreHudObjects;
                GraphicsEvents.OnPostRenderHudEvent += DrawObjects;
                LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
                ControlEvents.KeyPressed += (sender, e) => this.ReceiveKeyPress(e.KeyPressed, this.WeatherOpt.Keyboard);
                MenuEvents.MenuClosed += (sender, e) => this.ReceiveMenuClosed(e.PriorMenu);

                //console commands
                helper.ConsoleCommands
                      .Add("weather_settommorow", helper.Translation.Get("console-text.desc_tmrweather"), TomorrowWeatherChangeFromConsole)
                      .Add("weather_changeweather", helper.Translation.Get("console-text.desc_setweather"), WeatherChangeFromConsole)
                      .Add("world_solareclipse", "Starts the solar eclipse.", SolarEclipseEvent_CommandFired);
            }
        }

        private void SolarEclipseEvent_CommandFired(string command, string[] args)
        {
            IsEclipse = true;
            Game1.globalOutdoorLighting = .5f; //force lightning change.
            Game1.currentLocation.switchOutNightTiles();
            Game1.ambientLight = nightColor;
            Monitor.Log("Setting the eclipse event to true");
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            CustomTVMod.changeAction("weather", DisplayWeather);
        }

        public void DisplayWeather(TV tv, TemporaryAnimatedSprite sprite, StardewValley.Farmer who, string answer)
        {
            TemporaryAnimatedSprite BackgroundSprite = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(497, 305, 42, 28), 9999f, 1, 999999, tv.getScreenPosition(), false, false, (float)((double)(tv.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, tv.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f, false);
            TemporaryAnimatedSprite WeatherSprite = DescriptionEngine.GetWeatherOverlay(Conditions, tv);

            string OnScreenText = "";

            if (BackgroundSprite is null)
                Monitor.Log("Background Sprite is null");
            if (WeatherSprite is null)
                Monitor.Log("Weather Sprite is null");

            OnScreenText += DescriptionEngine.GenerateTVForecast(Conditions, OurMoon);

            CustomTVMod.showProgram(BackgroundSprite, OnScreenText, CustomTVMod.endProgram, WeatherSprite);
        }

        private void DrawOverMenus(object sender, EventArgs e)
        {
            //revised this so it properly draws over the canon moon. :v
            if (Game1.showingEndOfNightStuff && Game1.activeClickableMenu is ShippingMenu menu && !Game1.wasRainingYesterday)
            {
                Game1.spriteBatch.Draw(OurIcons.MoonSource, new Vector2((float)(Game1.viewport.Width - 80 * Game1.pixelZoom), (float)Game1.pixelZoom), OurIcons.GetNightMoonSprite(SDVMoon.GetLunarPhaseForDay(SDate.Now().AddDays(-1))), Color.LightBlue, 0.0f, Vector2.Zero, (float)Game1.pixelZoom * 1.5f, SpriteEffects.None, 1f);
            }
        }

        private void DebugWeather(string arg1, string[] arg2)
        {
            //print a complete weather status. 
            string retString = "";
            retString += $"Weather for {SDate.Now()} is {Conditions.ToString()}. Moon Phase is {OurMoon.ToString()}. {Environment.NewLine} System flags: isRaining {Game1.isRaining} isSnowing {Game1.isSnowing} isDebrisWeather: {Game1.isDebrisWeather} isLightning {Game1.isLightning}, with tommorow's set weather being {Game1.weatherForTomorrow}";
            Monitor.Log(retString);
        }

        private void DrawPreHudObjects(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.currentLocation.IsOutdoors)
                Conditions.DrawWeathers();       
        }

        /// <summary>
        /// This handles location changes
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Parameters</param>
        private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
        {
            if (IsEclipse)
            {
                Game1.globalOutdoorLighting = .5f;
                Game1.currentLocation.switchOutNightTiles();
                Game1.ambientLight = nightColor;

                if (!Game1.currentLocation.isOutdoors && Game1.currentLocation is DecoratableLocation)
                {
                    var loc = Game1.currentLocation as DecoratableLocation;
                    foreach (Furniture f in loc.furniture)
                    {
                        if (f.furniture_type == Furniture.window)
                            Helper.Reflection.GetMethod(f, "addLights").Invoke(new object[] { Game1.currentLocation });
                    }
                }
            }

            if (Conditions.HasWeather(CurrentWeather.Fog))
            {
                if (!Game1.currentLocation.isOutdoors && Game1.currentLocation is DecoratableLocation)
                {
                    var loc = Game1.currentLocation as DecoratableLocation;
                    foreach (Furniture f in loc.furniture)
                    {
                        if (f.furniture_type == Furniture.window)
                        {
                            //if (WeatherOpt.Verbose) Monitor.Log($"Attempting to remove the light for {f.name}");
                            Helper.Reflection.GetMethod(f, "addLights").Invoke(new object[] { Game1.currentLocation });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This function grabs events when the menu changes to handle dialogue replace
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">paramaters</param>
        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (GameClimate is null)
                Monitor.Log("GameClimate is null");
            if (e.NewMenu is null)
                Monitor.Log("e.NewMenu is null");              

            if (e.NewMenu is DialogueBox box)
            {
                bool stormDialogue = false;
                double odds = Dice.NextDoublePositive(), stormOdds = GameClimate.GetStormOdds(SDate.Now().AddDays(1), Dice, DebugOutput);
                List<string> lines = Helper.Reflection.GetField<List<string>>(box, "dialogues").GetValue();
                if (lines.FirstOrDefault() == Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822"))
                {
                    if (WeatherOpt.Verbose)
                        Monitor.Log($"Rain totem interception firing with roll {odds.ToString("N3")} vs. odds {stormOdds.ToString("N3")}");

                    // rain totem used, do your thing
                    if (WeatherOpt.StormTotemChange)
                    {
                        if (odds <= stormOdds)
                        {
                            if (WeatherOpt.Verbose)
                                Monitor.Log("Replacing rain with storm..");

                            Game1.weatherForTomorrow = Game1.weather_lightning;
                            stormDialogue = true;
                        }
                    }

                    // change dialogue text
                    lines.Clear();
                    if (stormDialogue)
                        lines.Add(Helper.Translation.Get("hud-text.desc_stormtotem"));
                    else
                        lines.Add(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822"));
                }
            }
        }

        /// <summary>
        /// This function handles the end of the day.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEndOfDay(object sender, EventArgs e)
        {
            if (Conditions.HasWeather(CurrentWeather.Frost) && WeatherOpt.AllowCropDeath)
            {
                Farm f = Game1.getFarm();
                int count = 0, maxCrops = (int)Math.Floor(SDVUtilities.CropCountInFarm(f) * WeatherOpt.DeadCropPercentage);

                foreach (KeyValuePair<Vector2, TerrainFeature> tf in f.terrainFeatures)
                {
                    if (count >= maxCrops)
                        break;

                    if (tf.Value is HoeDirt curr && curr.crop != null)
                    {
                        if (Dice.NextDouble() > WeatherOpt.CropResistance)
                        {
                            CropList.Add(tf.Key);
                            count++;
                        }
                    }
                }             

                if (count > 0)
                {
                    foreach (Vector2 v in CropList)
                    {
                        HoeDirt hd = (HoeDirt)f.terrainFeatures[v];
                        hd.crop.dead = true;
                    }

                    queuedMsg = new HUDMessage(Helper.Translation.Get("hud-text.desc_frost_killed", new { deadCrops = count }), Color.SeaGreen, 5250f, true)
                    {
                        whatType = 2
                    };
                }
            }

            if (IsEclipse)
                IsEclipse = false;

            //moon works after frost does
            OurMoon.HandleMoonAtSleep(Game1.getFarm(), Helper.Translation);
        }

        /// <summary>
        /// This function gets the forecast of the weather for the TV. 
        /// </summary>
        /// <returns>A string describing the weather</returns>
        public string GetWeatherForecast()
        {
            return DescriptionEngine.GenerateTVForecast(Conditions, OurMoon);
        }

        /// <summary>
        /// This checks for things every second.
        /// </summary>
        /// <param name="sender">Object sending</param>
        /// <param name="e">event params</param>
        private void CheckForChanges(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (IsEclipse && ResetTicker > 0)
            {
                Game1.globalOutdoorLighting = .5f;
                Game1.ambientLight = nightColor;
                Game1.currentLocation.switchOutNightTiles();
                ResetTicker = 0;
            }

            Conditions.MoveWeathers();

            if (Game1.isEating)
            { 
                StardewValley.Object obj = Game1.player.itemToEat as StardewValley.Object;

                if (obj.ParentSheetIndex == 351)
                    StaminaMngr.ClearDrain();
            }

            if (Game1.currentLocation.isOutdoors)
            {
                TicksOutside++;
            }

            TicksTotal++;            
        }

        /// <summary>
        /// Handles the ten minute update tick
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Parameters</param>
        private void TenMinuteUpdate(object sender, EventArgsIntChanged e)
        {
            if (!Game1.hasLoadedGame)
                return;

            Conditions.TenMinuteUpdate();

            if (IsEclipse)
            {
                Game1.globalOutdoorLighting = .5f;
                Game1.ambientLight = nightColor;
                Game1.currentLocation.switchOutNightTiles();
                ResetTicker = 1;

                if (!Game1.currentLocation.isOutdoors && Game1.currentLocation is DecoratableLocation)
                {
                    var loc = Game1.currentLocation as DecoratableLocation;
                    foreach (Furniture f in loc.furniture)
                    {
                        if (f.furniture_type == Furniture.window)
                            Helper.Reflection.GetMethod(f, "addLights").Invoke(new object[] { Game1.currentLocation });
                    }
                }

                if ((Game1.farmEvent == null && Game1.random.NextDouble() < (0.25 - Game1.dailyLuck / 2.0))
                    && ((WeatherOpt.SpawnMonsters && Game1.spawnMonstersAtNight) || (WeatherOpt.SpawnMonstersAllFarms)))
                {
                    Monitor.Log("Spawning a monster, or attempting to.", LogLevel.Debug);
                    if (Game1.random.NextDouble() < 0.25)
                    {
                        if (this.Equals(Game1.currentLocation))
                        {
                            Game1.getFarm().spawnFlyingMonstersOffScreen();
                            return;
                        }
                    }
                    else
                    {
                        Game1.getFarm().spawnGroundMonsterOffScreen();
                    }
                }

            }

            if (Conditions.HasWeather(CurrentWeather.Fog)) 
            {
                if (!Game1.currentLocation.isOutdoors && Game1.currentLocation is DecoratableLocation)
                {
                    var loc = Game1.currentLocation as DecoratableLocation;
                    foreach (Furniture f in loc.furniture)
                    {
                        //Yes, *add* lights removes them. No, don't ask me why.
                        if (f.furniture_type == Furniture.window)
                        {
                            //if (WeatherOpt.Verbose) Monitor.Log($"Attempting to remove the light for {f.name}");
                            Helper.Reflection.GetMethod(f, "addLights").Invoke(new object[] { Game1.currentLocation });
                        }
                    }
                }
            }

            if (Game1.currentLocation.isOutdoors && Conditions.HasWeather(CurrentWeather.Lightning) && Game1.timeOfDay < 2400)
                Utility.performLightningUpdate();

            //queued messages clear
            if (Game1.timeOfDay == 610 && queuedMsg != null)
            {
                Game1.hudMessages.Add(queuedMsg);
                queuedMsg = null;
            }

            //frost works at night, heatwave works during the day
            if (Game1.timeOfDay == 1700)
            {
                if (Conditions.HasWeather(CurrentWeather.Heatwave))
                {
                    ExpireTime = 2000;
                    Farm f = Game1.getFarm();
                    int count = 0, maxCrops = (int)Math.Floor(SDVUtilities.CropCountInFarm(f) * WeatherOpt.DeadCropPercentage);

                    foreach (KeyValuePair<Vector2, TerrainFeature> tf in f.terrainFeatures)
                    {
                        if (count >= maxCrops)
                            break;

                        if (tf.Value is HoeDirt curr && curr.crop != null)
                        {
                            if (Dice.NextDouble() <= WeatherOpt.CropResistance)
                            {
                                CropList.Add(tf.Key);
                                curr.state = HoeDirt.dry;
                                count++;
                            }
                        }
                    }

                    if (CropList.Count > 0)
                    {
                        if (WeatherOpt.AllowCropDeath)
                            SDVUtilities.ShowMessage(Helper.Translation.Get("hud-text.desc_heatwave_kill"));
                        else
                            SDVUtilities.ShowMessage(Helper.Translation.Get("hud-text.desc_heatwave_dry"));
                    }
                }
            }

            if (Game1.timeOfDay == ExpireTime && WeatherOpt.AllowCropDeath)
            {
                //if it's still de watered - kill it.
                Farm f = Game1.getFarm();
                bool cDead = false;

                foreach (Vector2 v in CropList)
                {
                    HoeDirt hd = (HoeDirt)f.terrainFeatures[v];
                    if (hd.state == HoeDirt.dry)
                    {
                        hd.crop.dead = true;
                        cDead = true;
                    }
                }

                if (cDead)
                    SDVUtilities.ShowMessage(Helper.Translation.Get("hud-text.desc_heatwave_cropdeath"));
            }

            float oldStamina = Game1.player.stamina;
            Game1.player.stamina += StaminaMngr.TenMinuteTick(Conditions, TicksOutside, TicksTotal, Dice);

            if (Game1.player.stamina <= 0)
                SDVUtilities.FaintPlayer();

            TicksTotal = 0;
            TicksOutside = 0;
        }

        /// <summary>
        /// This event handles drawing to the screen.
        /// </summary>
        /// <param name="sender">Object sending</param>
        /// <param name="e">event params</param>
        private void DrawObjects(object sender, EventArgs e)
        {
            //float shadowMult = 0f;
            if (!Context.IsWorldReady)
                return;
          
            var weatherMenu = Game1.onScreenMenus.OfType<DayTimeMoneyBox>().FirstOrDefault();

            if (weatherMenu == null)
                return;
            // abort abort abort (maybe another mod replaced it?)

            //determine icon offset  
            if (!Game1.eventUp)
            {
                if ((int)Conditions.CurrentWeatherIcon != (int)WeatherIcon.IconError)
                {
                    RWeatherIcon = new Rectangle(0 + 12 * (int)Conditions.CurrentWeatherIcon, SDVTime.IsNight ? 8 : 0, 12, 8);
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
            if (Game1.activeClickableMenu == null && Game1.mouseCursor > -1 && (Mouse.GetState().X != 0 || Mouse.GetState().Y != 0) && (Game1.getOldMouseX() != 0 || Game1.getOldMouseY() != 0))
            {
                if ((double)Game1.mouseCursorTransparency <= 0.0 || !Utility.canGrabSomethingFromHere(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y, Game1.player) || Game1.mouseCursor == 3)
                {
                    if (Game1.player.ActiveObject != null && Game1.mouseCursor != 3 && !Game1.eventUp)
                    {
                        if ((double)Game1.mouseCursorTransparency > 0.0 || Game1.options.showPlacementTileForGamepad)
                        {
                            Game1.player.ActiveObject.drawPlacementBounds(Game1.spriteBatch, Game1.currentLocation);
                            if ((double)Game1.mouseCursorTransparency > 0.0)
                            {
                                bool flag = Utility.playerCanPlaceItemHere(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, Game1.player) || Utility.isThereAnObjectHereWhichAcceptsThisItem(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y) && Utility.withinRadiusOfPlayer(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, 1, Game1.player);
                                Game1.player.CurrentItem.drawInMenu(Game1.spriteBatch, new Vector2((float)(Game1.getMouseX() + Game1.tileSize / 4), (float)(Game1.getMouseY() + Game1.tileSize / 4)), flag ? (float)((double)Game1.dialogueButtonScale / 75.0 + 1.0) : 1f, flag ? 1f : 0.5f, 0.999f);
                            }
                        }
                    }
                    else if (Game1.mouseCursor == 0 && Game1.isActionAtCurrentCursorTile)
                        Game1.mouseCursor = Game1.isInspectionAtCurrentCursorTile ? 5 : 2;
                }
                if (!Game1.options.hardwareCursor)
                    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)Game1.getMouseX(), (float)Game1.getMouseY()), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.mouseCursor, 16, 16)), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, (float)Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
                Game1.wasMouseVisibleThisFrame = (double)Game1.mouseCursorTransparency > 0.0;
            }
            Game1.mouseCursor = 0;
            if (Game1.isActionAtCurrentCursorTile || Game1.activeClickableMenu != null)
                return;
            Game1.mouseCursorTransparency = 1f;
        }    

        private void ResetMod(object sender, EventArgs e)
        {
            Conditions.Reset();
            ExpireTime = 0;
            CropList.Clear(); 
            DebugOutput.Clear();
            StaminaMngr.Reset();
            TicksOutside = 0;
            TicksTotal = 0;
        }

        private void HandleNewDay(object sender, EventArgs e)
        {
            if (CropList == null)
                Monitor.Log("CropList is null!");
            if (DebugOutput == null)
                Monitor.Log("DebugOutput is null!");
            if (OurMoon == null)
                Monitor.Log("OurMoon is null");
            if (Conditions == null)
                Monitor.Log("CurrentWeather is null");
            if (StaminaMngr == null)
                Monitor.Log("StaminaMngr is null");
            if (GameClimate is null)
                Monitor.Log("GameClimate is null");

            if (Dice.NextDouble() < WeatherOpt.EclipseChance && WeatherOpt.EclipseOn && OurMoon.CurrentPhase == MoonPhase.FullMoon &&
                SDate.Now().DaysSinceStart > 2)
            {
                IsEclipse = true;
                Game1.addHUDMessage(new HUDMessage("It looks like a rare solar eclipse will darken the sky all day!"));
                Conditions.BlockFog = true;
            }

            SeedsForDialogue[0] = Dice.Next();
            SeedsForDialogue[1] = Dice.Next();
            CropList.Clear(); //clear the crop list
            DebugOutput.Clear();
            Conditions.OnNewDay();
            UpdateWeatherOnNewDay();
            SetTommorowWeather();
            OurMoon.HandleMoonAfterWake(Helper.Translation);
            StaminaMngr.OnNewDay();
            TicksOutside = 0;
            ExpireTime = 0;
            TicksTotal = 0;
        }

        private void SetTommorowWeather()
        {
            //if tomorrow is a festival or wedding, we need to set the weather and leave.
            if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
            {
                Game1.weatherForTomorrow = Game1.weather_festival;
                if (WeatherOpt.Verbose)
                    Monitor.Log($"Festival tomorrow. Aborting processing.", LogLevel.Trace);

                //if (WeatherOpt.Verbose) Monitor.Log(DebugOutput.ToString());
                return;
            }

            if (Game1.countdownToWedding == 1)
            {
                Game1.weatherForTomorrow = Game1.weather_wedding;
                if (WeatherOpt.Verbose)
                    Monitor.Log($"Wedding tomorrow. Aborting processing.", LogLevel.Trace);

                return;
            }

            if (ForceDays.CheckForForceDay(DescriptionEngine, SDate.Now().AddDays(1),Monitor, WeatherOpt.Verbose))
            {
                if (WeatherOpt.Verbose)
                    Monitor.Log($"The game will force tomorrow. Aborting processing.", LogLevel.Trace);
                return;
            }

            if (WeatherOpt.Verbose)
                Monitor.Log("Setting weather for tomorrow");

            //now set tomorrow's weather
            var OddsForTheDay = GameClimate.GetClimateForDate(SDate.Now().AddDays(1));

            double rainDays = OddsForTheDay.RetrieveOdds(Dice, "rain", SDate.Now().AddDays(1).Day, DebugOutput);
            double windyDays = OddsForTheDay.RetrieveOdds(Dice, "debris", SDate.Now().AddDays(1).Day, DebugOutput);
            double stormDays = OddsForTheDay.RetrieveOdds(Dice, "storm", SDate.Now().AddDays(1).Day, DebugOutput);

            if (WeatherOpt.Verbose)
            {
                Monitor.Log($"Odds are Rain: {rainDays.ToString("N3")}, Windy: {windyDays.ToString("N3")}, and Storm {stormDays.ToString("N3")}");
            }

            ProbabilityDistribution<string> WeatherDist = new ProbabilityDistribution<string>("sunny");
            WeatherDist.AddNewEndPoint(rainDays, "rain");
            WeatherDist.AddNewCappedEndPoint(windyDays, "debris");

            double distOdd = Dice.NextDoublePositive();

            if (WeatherOpt.Verbose)
            {
                Monitor.Log(WeatherDist.ToString());
                Monitor.Log($"Distribution odds is {distOdd}");
            }

            if (!(WeatherDist.GetEntryFromProb(distOdd, out string Result)))
            {
                Result = "sunny";
                Monitor.Log("The weather has failed to process in some manner. Falling back to [sunny]", LogLevel.Info);
            }

            if (WeatherOpt.Verbose)
                Monitor.Log($"Weather result is {Result}");

            if (!Conditions.IsTodayTempSet)
                throw new NullReferenceException("Today's temperatures have not been set!");

            //now parse the result.
            if (Result == "rain")
            {
                //snow applies first
                double MidPointTemp = Conditions.TodayHigh -
                    ((Conditions.TodayHigh - Conditions.TodayLow) / 2);

                if ((Conditions.TodayHigh <= 2 || MidPointTemp <= 0) && Game1.currentSeason != "spring")
                {
                    if (WeatherOpt.Verbose)
                        Monitor.Log($"Snow is enabled, with the High for the day being: {Conditions.TodayHigh}" +
                                    $" and the calculated midpoint temperature being {MidPointTemp}");

                    Game1.weatherForTomorrow = Game1.weather_snow;
                }
                else
                {
                    Game1.weatherForTomorrow = Game1.weather_rain;
                }

                if (!GameClimate.AllowRainInWinter && Game1.currentSeason == "winter" && Game1.weatherForTomorrow == Game1.weather_rain)
                {
                    Game1.weatherForTomorrow = Game1.weather_snow;
                }

                //apply lightning logic.
                if (Dice.NextDoublePositive() >= stormDays && Game1.weatherForTomorrow == Game1.weather_rain)
                {
                    Game1.weatherForTomorrow = Game1.weather_lightning;
                    if (SDate.Now().Year == 1 && SDate.Now().Season == "spring" && !WeatherOpt.AllowStormsSpringYear1)
                        Game1.weatherForTomorrow = Game1.weather_rain;
                }

                //tracking time! - Snow fall on Fall 28, if the flag is set.
                if (Game1.dayOfMonth == 28 && Game1.currentSeason == "fall" && WeatherOpt.SnowOnFall28)
                {
                    Conditions.ForceTodayTemps(2, -1);
                    Game1.weatherForTomorrow = Game1.weather_snow;
                }
            }

            if (Result == "debris")
            {
                Game1.weatherForTomorrow = Game1.weather_debris;
            }

            if (Result == "sunny")
            {
                Game1.weatherForTomorrow = Game1.weather_sunny;
            }

            if (WeatherOpt.Verbose)
                Monitor.Log($"We've set the weather for Tomorrow. It is: {DescriptionEngine.DescribeInGameWeather(Game1.weatherForTomorrow)}");
        }

        private void UpdateWeatherOnNewDay()
        {
            if (Game1.dayOfMonth == 0) //do not run on day 0.
                return;

            //Set Temperature for today and tommorow. Get today's conditions.
            //   If tomorrow is set, move it to today, and autoregen tomorrow.
            //   *201711 Due to changes in the object, it auto attempts to update today from tomorrow.
            Conditions.SetTodayWeather();

            if (!Conditions.IsTomorrowTempSet)
                Conditions.SetTodayTemps(GameClimate.GetTemperatures(SDate.Now(), Dice, DebugOutput));

            Conditions.SetTomorrowTemps(GameClimate.GetTemperatures(SDate.Now().AddDays(1), Dice, DebugOutput));

            if (WeatherOpt.Verbose)
                Monitor.Log($"Updated the temperature for tommorow and today. Setting weather for today... ", LogLevel.Trace);

            //if today is a festival or wedding, do not go further.
            if (Conditions.GetCurrentConditions().HasAnyFlags(CurrentWeather.Festival | CurrentWeather.Wedding))
            {
                if (WeatherOpt.Verbose)
                    Monitor.Log("It is a wedding or festival today. Not attempting to run special weather or fog.");

                //if (WeatherOpt.Verbose) Monitor.Log(DebugOutput.ToString());
                return;
            }
                    
            if (Conditions.TestForSpecialWeather(GameClimate.GetClimateForDate(SDate.Now()).RetrieveOdds(Dice, "fog", SDate.Now().Day, DebugOutput)))
            {
                Monitor.Log("Special weather created!");
            }
        }

        /* **************************************************************
         * console commands
         * **************************************************************
         */

        /// <summary>
        /// This function changes the weather (Console Command)
        /// </summary>
        /// <param name="arg1">The command used</param>
        /// <param name="arg2">The console command parameters</param>
        private void WeatherChangeFromConsole(string arg1, string[] arg2)
        {
            if (arg2.Length < 1)
                return;

            string ChosenWeather = arg2[0];

            switch (ChosenWeather)
            {
                case "rain":
                    Game1.isSnowing = Game1.isLightning = Game1.isDebrisWeather = false;
                    Game1.isRaining = true;
                    Game1.debrisWeather.Clear();
                    Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                    Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset_rain"), LogLevel.Info);
                    break;
                case "storm":
                    Game1.isSnowing = Game1.isDebrisWeather = false;
                    Game1.isLightning = Game1.isRaining = true;
                    Game1.debrisWeather.Clear();
                    Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                    Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset_storm"), LogLevel.Info);
                    break;
                case "snow":
                    Game1.isRaining = Game1.isLightning = Game1.isDebrisWeather = false;
                    Game1.isSnowing = true;
                    Game1.debrisWeather.Clear();
                    Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                    Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset_snow"), LogLevel.Info);
                    break;
                case "debris":
                    Game1.isSnowing = Game1.isLightning = Game1.isRaining = false;
                    Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                    Conditions.GetWeatherMatchingType("Fog").First().EndWeather();
                    Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    Game1.isDebrisWeather = true;
                    Game1.populateDebrisWeatherArray();
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset_debris", LogLevel.Info));
                    break;
                case "sunny":
                    Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                    Conditions.GetWeatherMatchingType("Fog").First().EndWeather();
                    Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    Game1.isSnowing = Game1.isLightning = Game1.isRaining = Game1.isRaining = false;
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset_sun", LogLevel.Info));
                    break;
                case "blizzard":
                    Game1.isRaining = Game1.isLightning = Game1.isDebrisWeather = false;
                    Game1.isSnowing = true;
                    Game1.debrisWeather.Clear();
                    Conditions.GetWeatherMatchingType("Blizzard").First().CreateWeather();
                    Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset_snow"), LogLevel.Info);
                    break;
                case "whiteout":
                    Game1.isRaining = Game1.isLightning = Game1.isDebrisWeather = false;
                    Game1.isSnowing = true;
                    Game1.debrisWeather.Clear();
                    Conditions.GetWeatherMatchingType("Blizzard").First().CreateWeather();
                    Conditions.GetWeatherMatchingType("WhiteOut").First().CreateWeather();
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset_snow"), LogLevel.Info);
                    break;
            }

            Game1.updateWeatherIcon();
            Conditions.SetTodayWeather();
        }

        /// <summary>
        /// This function changes the weather for tomorrow (Console Command)
        /// </summary>
        /// <param name="arg1">The command used</param>
        /// <param name="arg2">The console command parameters</param>
        private void TomorrowWeatherChangeFromConsole(string arg1, string[] arg2)
        {
            if (arg2.Length < 1)
                return;

            string ChosenWeather = arg2[0];
            switch (ChosenWeather)
            {
                case "rain":
                    Game1.weatherForTomorrow = Game1.weather_rain;
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset-tmrwrain"), LogLevel.Info);
                    break;
                case "storm":
                    Game1.weatherForTomorrow = Game1.weather_lightning;
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset-tmrwstorm"), LogLevel.Info);
                    break;
                case "snow":
                    Game1.weatherForTomorrow = Game1.weather_snow;
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset-tmrwsnow"), LogLevel.Info);
                    break;
                case "debris":
                    Game1.weatherForTomorrow = Game1.weather_debris;
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset-tmrwdebris"), LogLevel.Info);
                    break;
                case "festival":
                    Game1.weatherForTomorrow = Game1.weather_festival;
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset-tmrwfestival"), LogLevel.Info);
                    break;
                case "sun":
                    Game1.weatherForTomorrow = Game1.weather_sunny;
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset-tmrwsun"), LogLevel.Info);
                    break;
                case "wedding":
                    Game1.weatherForTomorrow = Game1.weather_wedding;
                    Monitor.Log(Helper.Translation.Get("console-text.weatherset-tmrwwedding"), LogLevel.Info);
                    break;
            }
        }

        #region Menu
        /// <summary>
        /// This checks the keys being pressed if one of them is the weather menu, toggles it.
        /// </summary>
        /// <param name="key">The key being pressed</param>
        /// <param name="config">The keys we're listening to.</param>
        private void ReceiveKeyPress(Keys key, Keys config)
        {
            if (config != key)  //sanity force this to exit!
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
        /// This function closes the menu. Will reopen the previous menu if it exists
        /// </summary>
        /// <param name="closedMenu">The menu being closed.</param>
        private void ReceiveMenuClosed(IClickableMenu closedMenu)
        {
            // restore the previous menu if it was hidden to show the lookup UI
            if (closedMenu is WeatherMenu && this.PreviousMenu != null)
            {
                Game1.activeClickableMenu = this.PreviousMenu;
                this.PreviousMenu = null;
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
            string MenuText = DescriptionEngine.GenerateMenuPopup(Conditions, OurMoon);

            // show menu
            this.PreviousMenu = Game1.activeClickableMenu;
            Game1.activeClickableMenu = new WeatherMenu(Monitor, this.Helper.Reflection, OurIcons, Helper.Translation, Conditions, 
                OurMoon, WeatherOpt, MenuText);
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
    }
}

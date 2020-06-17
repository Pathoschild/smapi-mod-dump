using System;
using System.Collections.Generic;
using StardewModdingAPI;
using TwilightShards.Common;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;
using StardewValley.Locations;
using SpaceCore.Events;
using StardewValley.Monsters;
using TwilightShards.Stardew.Common;
using TwilightShards.LunarDisturbances.Integrations;
using StardewValley.Events;
using Netcode;

namespace TwilightShards.LunarDisturbances
{
    public class LunarDisturbances : Mod, IAssetEditor
    {
        internal static SDVMoon OurMoon;
        private MersenneTwister Dice;
        internal static IContentHelper ContentManager;
        internal static ITranslationHelper Translation;
        private MoonConfig ModConfig;
        private HUDMessage queuedMsg;
        public static Sprites.Icons OurIcons { get; set; }
        internal static bool IsEclipse { get; set; }
        private List<string> BloodMoonTracker;
        private bool UseJsonAssetsApi = false;
        private Color nightColor = new Color((int)byte.MaxValue, (int)byte.MaxValue, 0);
        private Integrations.IJsonAssetsApi JAAPi;
        internal int ResetTicker { get; set; }
        private int SecondCount;
        private bool HasGottenSync = false;
        internal static IMultiplayerHelper MPHandler;
        private ILunarDisturbancesAPI API;

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("LooseSprites/Cursors"))
            {
                return true;
            }

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            Texture2D customTexture = this.Helper.Content.Load<Texture2D>("assets/BlankPixel.png", ContentSource.ModFolder);
            Texture2D bloodMoonTexture = this.Helper.Content.Load<Texture2D>("assets/DaytimeBloodMoon.png", ContentSource.ModFolder);

            asset
                .AsImage()
                .PatchImage(customTexture, targetArea: new Rectangle(643, 833, 41, 45));
            
            if (OurMoon.CurrentPhase() == MoonPhase.BloodMoon)
            {
                asset.AsImage().PatchImage(bloodMoonTexture, targetArea: new Rectangle(342,440,7,7));
            }
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Dice = new MersenneTwister();
            Translation = Helper.Translation;
            ModConfig = Helper.ReadConfig<MoonConfig>();          
            OurMoon = new SDVMoon(ModConfig, Dice, Helper.Translation);
            ContentManager = Helper.Content;
            MPHandler = Helper.Multiplayer;
            OurIcons = new Sprites.Icons(Helper.Content);
            queuedMsg = null;
            BloodMoonTracker = new List<string>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageRecieved;
            SpaceEvents.ChooseNightlyFarmEvent += SpaceEvents_ChooseNightlyFarmEvent;


            helper.ConsoleCommands.Add("force_bloodmoon", "Forces a bloodmoon", ForceBloodMoon)
                                  .Add("force_bloodmoonoff", "Turns bloodmoon off.", TurnBloodMoonOff)
                                  .Add("show_mooninfo", "Show moon info", ShowMoonInfo)
								  .Add("force_eclipseOn", "Turns eclipse on.", TurnEclipseOn);
        }

        private void ShowMoonInfo(string arg1, string[] arg2)
        {
            Monitor.Log($"Moon phase according to end of night: {OurMoon.GetLunarPhaseForDay(SDate.Now())}, with coords: { Sprites.Icons.GetNightMoonSprite(OurMoon.GetLunarPhaseForDay(SDate.Now()))}. According to normal phrasing: Phase is {OurMoon.DescribeMoonPhase()}, with rise {OurMoon.GetMoonRiseTime()} and set {OurMoon.GetMoonSetTime()}", LogLevel.Info);
        }

        private void SpaceEvents_ChooseNightlyFarmEvent(object sender, EventArgsChooseNightlyFarmEvent e)
        {
            if (OurMoon.CurrentPhase() == MoonPhase.BlueMoon && !Game1.weddingToday &&
                   (e.NightEvent == null || (e.NightEvent is SoundInTheNightEvent &&
                   Helper.Reflection.GetField<NetInt>(e.NightEvent, "behavior").GetValue().Value == 2)))
            {
                if (Dice.NextDouble() < 0.025 && !Game1.currentSeason.Equals("winter"))
                    e.NightEvent = new FairyEvent();
                else if (Dice.NextDouble() < 0.025)
                    e.NightEvent = new WitchEvent();
                else if (Dice.NextDouble() < 0.025)
                    e.NightEvent = new WitchEvent();
                else if (Dice.NextDouble() < 0.025)
                    e.NightEvent = new SoundInTheNightEvent(1);
                else if (Dice.NextDouble() < 0.025)
                    e.NightEvent = new SoundInTheNightEvent(0);
                else if (Dice.NextDouble() < 0.025)
                    e.NightEvent = new SoundInTheNightEvent(3);
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                OurMoon.MoonTracker = Helper.Data.ReadSaveData<LunarInfo>("moon-tracker");

                if (OurMoon.MoonTracker is null)
                    OurMoon.MoonTracker = new LunarInfo();
            }
        }

        private void OnModMessageRecieved(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == "KoihimeNakamura.LunarDisturbances" && e.Type == "NewFarmHandJoin" && Context.IsMainPlayer && !HasGottenSync)
            {

                MoonMessage message = GenerateLunarSync();
                MPHandler.SendMessage<MoonMessage>(message, "MoonMessage", new[] { "KoihimeNakamura.LunarDisturbances" }, new[] { e.FromPlayerID });
                HasGottenSync = true;
            }

            if (e.FromModID == "KoihimeNakamura.LunarDisturbances" && e.Type == "MoonMessage")
            {
                MoonMessage message = e.ReadAs<MoonMessage>();
                if (ModConfig.Verbose)
                {
                    Monitor.Log($"Message contents at {Game1.timeOfDay} : Eclipse Setting: {message.isEclipse})");
                }
                SetLunarSync(message);
            }
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            int cropCount = OurMoon.HandleMoonAtSleep(Game1.getFarm(), Monitor);
            if (cropCount != 0)
            {
                if (OurMoon.CurrentPhase() == MoonPhase.NewMoon)
                    queuedMsg = new TCHUDMessage(Helper.Translation.Get("moon-text.newmoon_eff", new { cropsAffected = cropCount }), OurMoon.CurrentPhase());
                if (OurMoon.CurrentPhase() == MoonPhase.FullMoon)
                    queuedMsg = new TCHUDMessage(Helper.Translation.Get("moon-text.fullmoon_eff", new { cropsAffected = cropCount }),OurMoon.CurrentPhase());
            }
        }
		
		private void TurnEclipseOn(string arg1, string[] arg2)
        {
            Monitor.Log("Turning the eclipse on!");
            IsEclipse = true;
        }

        private void TurnBloodMoonOff(string arg1, string[] arg2)
        {
            Monitor.Log("Turning the blood moon off!");
            OurMoon.TurnBloodMoonOff();
        }

        private void ForceBloodMoon(string arg1, string[] arg2)
        {
            Monitor.Log("Turning the blood moon on!", LogLevel.Info);
            OurMoon.ForceBloodMoon();
        }

        /// <summary>Get an API that other mods can access. This is always called after <see cref="M:StardewModdingAPI.Mod.Entry(StardewModdingAPI.IModHelper)" />.</summary>
        public override object GetApi()
        {
            return API ?? (API = new LunarDisturbancesAPI(OurMoon));
        }

        /// <summary>Raised once per second after the game state is updated.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (Context.IsPlayerFree)
            {
                if (Game1.game1.IsActive)
                    SecondCount++;
                
                if (SecondCount == 10)
                {
                    SecondCount = 0;
                    if (Game1.currentLocation.IsOutdoors && OurMoon.CurrentPhase() == MoonPhase.BloodMoon)
                    {
                        Vector2 vecSpawn = SDVUtilities.SpawnRandomMonster(Game1.currentLocation);
                        if (vecSpawn != Vector2.Zero && ModConfig.Verbose)
                        {
                            Monitor.Log($"Monster spawned at {vecSpawn}");
                        }
                    }
                }
            }
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            OurMoon.Reset();
            BloodMoonTracker.Clear();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (IsEclipse && ResetTicker > 0)
            {
                Monitor.Log("Eclipse code firing");
                Game1.globalOutdoorLighting = .5f;
                Game1.ambientLight = nightColor;
                Game1.currentLocation.switchOutNightTiles();
                ResetTicker = 0;
            }
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (IsEclipse)
                IsEclipse = false;

            if (!Context.IsMainPlayer)
                return;

            if (OurMoon.MoonTracker is null)
                OurMoon.MoonTracker = new LunarInfo();

            this.Helper.Data.WriteSaveData("moon-tracker", OurMoon.MoonTracker);

            //cleanup any spawned monsters
            foreach (GameLocation l in Game1.locations)
            {
                for (int index = l.characters.Count - 1; index >= 0; --index)
                {
                    if (l.characters[index] is Monster && !(l is SlimeHutch))
                        l.characters.RemoveAt(index);
                }
            }

            BloodMoonTracker.Clear();              
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
		    if (e.NewMenu is DialogueBox dBox && OurMoon.CurrentPhase() == MoonPhase.BloodMoon)
			{
                if (!(BloodMoonTracker.Contains(Game1.currentSpeaker.Name)))
                {
                    Game1.player.changeFriendship(-60, Game1.currentSpeaker);
                    BloodMoonTracker.Add(Game1.currentSpeaker.Name);
                }

                if (!Game1.eventUp)
                {
                    var cDBU = Helper.Reflection.GetField<Stack<string>>(dBox, "characterDialoguesBrokenUp").GetValue();
                    cDBU.Clear();
                    Helper.Reflection.GetField<Stack<string>>(dBox, "characterDialoguesBrokenUp").SetValue(cDBU);

                    Dialogue diag = Helper.Reflection.GetField<Dialogue>(dBox, "characterDialogue").GetValue();
                    diag.setCurrentDialogue(Helper.Translation.Get("moon-text.bloodmoon_villager_generic"));
                    Helper.Reflection.GetField<Dialogue>(dBox, "characterDialogue").SetValue(diag);
                    Helper.Reflection.GetMethod(dBox, "checkDialogue").Invoke(new[] { diag });
                }
            }		
			
            if (!UseJsonAssetsApi)
            {
                if (e.NewMenu is ShopMenu menu && menu.portraitPerson != null)
                {
                    float sellPrice, buyPrice;
                    switch (OurMoon.CurrentPhase())
                    {
                        case MoonPhase.BloodMoon:
                            sellPrice = .75f;
                            buyPrice = 1.85f;
                            break;
                        case MoonPhase.BlueMoon:
                            sellPrice = 1.175f;
                            buyPrice = .825f;
                            break;
                        case MoonPhase.HarvestMoon:
                            sellPrice = 1.1f;
                            buyPrice = .9f;
                            break;
                        default:
                            sellPrice = 1f;
                            buyPrice = 1f;
                            break;
                    }
                    Helper.Reflection.GetField<float>(menu, "sellPercentage").SetValue(Helper.Reflection.GetField<float>(menu, "sellPercentage").GetValue() * sellPrice);
                    
                    var itemPriceAndStock = Helper.Reflection.GetField<Dictionary<Item, int[]>>(menu, "itemPriceAndStock").GetValue();
                    foreach (KeyValuePair<Item, int[]> kvp in itemPriceAndStock)
                    {
                        kvp.Value[0] = (int)Math.Floor(kvp.Value[0] * buyPrice);
                    }
                }
            }
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Helper.Content.InvalidateCache("LooseSprites/Cursors");
            if (OurMoon.GetMoonRiseTime() <= 0600 || OurMoon.GetMoonRiseTime() >= 2600 && ModConfig.ShowMoonPhase)
                Game1.addHUDMessage(new TCHUDMessage(Helper.Translation.Get("moon-text.moonriseBefore6", new { moonPhase = OurMoon.DescribeMoonPhase(), riseTime = OurMoon.GetMoonRiseDisplayTime() }), OurMoon.CurrentPhase()));

            if (OurMoon == null)
            {
                Monitor.Log("OurMoon is null");
                return;
            }

            if (queuedMsg != null)
            {
                Game1.addHUDMessage(queuedMsg);
                queuedMsg = null;
            }

            if (Context.IsMainPlayer)
            {
                if (OurMoon.MoonTracker.CurrentSeason != Game1.currentSeason)
                {
                    OurMoon.MoonTracker.CurrentSeason = Game1.currentSeason;
                    OurMoon.MoonTracker.FullMoonThisSeason = false;
                }
            }

            if (Dice.NextDouble() < ModConfig.EclipseChance && ModConfig.EclipseOn && OurMoon.CurrentPhase() == MoonPhase.NewMoon &&
                SDate.Now().DaysSinceStart > 2 && !Utility.isFestivalDay(Game1.dayOfMonth,Game1.currentSeason))
            {
                IsEclipse = true;
                var n = new HUDMessage(Helper.Translation.Get("moon-text.solareclipse"))
                {
                    color = Color.SeaGreen,
                    fadeIn = true,
                    timeLeft = 4000,
                    noIcon = true
                };
                Game1.addHUDMessage(n);
                MoonMessage message = GenerateLunarSync();
                MPHandler.SendMessage<MoonMessage>(message, "MoonMessage", new[] { "KoihimeNakamura.LunarDisturbances" });
                Monitor.Log("There's a solar eclipse today!", LogLevel.Info);
            }

            OurMoon.UpdateForBloodMoon();
            OurMoon.OnNewDay();
            OurMoon.HandleMoonAfterWake();
        }

        MoonMessage GenerateLunarSync()
        {
            var m = new MoonMessage
            {
                isEclipse = IsEclipse,
                currentMoonTracker = OurMoon.MoonTracker
            };
            return m;
        }

        void SetLunarSync(MoonMessage m)
        {
            IsEclipse = m.isEclipse;
            OurMoon.MoonTracker = new LunarInfo(m.currentMoonTracker);
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Game1.timeOfDay == OurMoon.GetMoonRiseTime() && ModConfig.ShowMoonPhase)
                Game1.addHUDMessage(new TCHUDMessage(Helper.Translation.Get("moon-text.moonrise", new { moonPhase = OurMoon.DescribeMoonPhase() }),OurMoon.CurrentPhase()));

            if (Game1.timeOfDay == OurMoon.GetMoonSetTime() && ModConfig.ShowMoonPhase)
                Game1.addHUDMessage(new TCHUDMessage(Helper.Translation.Get("moon-text.moonset", new { moonPhase = OurMoon.DescribeMoonPhase() }), OurMoon.CurrentPhase()));

            if (IsEclipse)
            {
                Game1.globalOutdoorLighting = .5f;
                Game1.ambientLight = nightColor;
                Game1.currentLocation.switchOutNightTiles();
                ResetTicker = 1;

                if (!Game1.currentLocation.IsOutdoors && Game1.currentLocation is DecoratableLocation)
                {
                    var loc = Game1.currentLocation as DecoratableLocation;
                    foreach (Furniture f in loc.furniture)
                    {
                        if (f.furniture_type.Value == Furniture.window)
                            Helper.Reflection.GetMethod(f, "addLights").Invoke(Game1.currentLocation);
                    }
                }

                if ((Game1.farmEvent == null && Game1.random.NextDouble() < (0.25 - Game1.player.team.AverageDailyLuck() / 2.0))
                    && Game1.spawnMonstersAtNight && Context.IsMainPlayer)
                {
		    
                    if (ModConfig.Verbose)
			            Monitor.Log("Spawning a monster, or attempting to.", LogLevel.Debug);
					
                    if (Game1.random.NextDouble() < 0.25)
                    {
                        if (Game1.currentLocation.IsFarm)
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

            //moon 10-minute
            OurMoon.TenMinuteUpdate();
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!(e.IsLocalPlayer))
            {
                return;
            }

            if (IsEclipse)
            {
                Game1.globalOutdoorLighting = .5f;
                Game1.currentLocation.switchOutNightTiles();
                Game1.ambientLight = nightColor;

                if (!Game1.currentLocation.IsOutdoors && Game1.currentLocation is DecoratableLocation)
                {
                    var loc = Game1.currentLocation as DecoratableLocation;
                    foreach (Furniture f in loc.furniture)
                    {
                        if (f.furniture_type.Value == Furniture.window)
                            Helper.Reflection.GetMethod(f, "addLights").Invoke(Game1.currentLocation);
                    }
                }
            }

            if (OurMoon.CurrentPhase() == MoonPhase.BloodMoon)
            {
                Game1.currentLocation.waterColor.Value = OurMoon.BloodMoonWater;
            }
        }

        /// <summary>When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null), raised after that menu is drawn to the sprite batch but before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            bool outro = false;
            //revised this so it properly draws over the canon moon. :v
            if (Game1.activeClickableMenu is ShippingMenu ourMenu)
            {
                outro = Helper.Reflection.GetField<bool>(ourMenu, "outro").GetValue();
            }

            if (!Game1.wasRainingYesterday && !outro && Game1.activeClickableMenu is ShippingMenu currMenu && currMenu.currentPage == -1)
            {          
                float scale = Game1.pixelZoom * 1.25f;

                if (Game1.viewport.Width < 1024)
                    scale = Game1.pixelZoom * .8f;
                if (Game1.viewport.Width < 810)
                    scale = Game1.pixelZoom * .6f;
                if (Game1.viewport.Width < 700)
                    scale = Game1.pixelZoom * .35f;

                Color moonColor = Color.LightCyan;
                if (OurMoon.GetLunarPhaseForDay(SDate.Now().AddDays(-1)) == MoonPhase.BlueMoon)
                {
                    moonColor = Color.Blue;
                }

                Game1.spriteBatch.Draw(OurIcons.MoonSource, new Vector2(Game1.viewport.Width - 65 * Game1.pixelZoom, Game1.pixelZoom), Sprites.Icons.GetNightMoonSprite(OurMoon.GetLunarPhaseForDay(SDate.Now().AddDays(-1))), moonColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
            }
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            JAAPi = SDVUtilities.GetModApi<Integrations.IJsonAssetsApi>(Monitor, Helper, "spacechase0.JsonAssets", "1.1");

            if (JAAPi != null)
            {
                UseJsonAssetsApi = true;
                JAAPi.AddedItemsToShop += JAAPi_AddedItemsToShop;
            }

            var api = Helper.ModRegistry.GetApi<Integrations.GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(ModManifest, () => ModConfig = new MoonConfig(), () => Helper.WriteConfig(ModConfig));
                api.RegisterClampedOption(ModManifest, "Blood Moon Chances", "The chance for a blood moon rising. There's a bad moon on the rise.", () => (float)ModConfig.BadMoonRising, (float val) => ModConfig.BadMoonRising = val, 0f, 1f);
                api.RegisterSimpleOption(ModManifest, "Allow Eclipse", "This option controls if eclipses will happen", () => ModConfig.EclipseOn, (bool val) => ModConfig.EclipseOn = val);
                api.RegisterSimpleOption(ModManifest, "Show Moon Phase", "Show the moon phase popups on moon rise and set", () => ModConfig.ShowMoonPhase, (bool val) => ModConfig.ShowMoonPhase = val);
                api.RegisterSimpleOption(ModManifest, "Spawn Monsters", "This option controls if monsters spawn during eclipses on a wilderness farm. NOTE: They don't spawn if you've turned off monster spawns in game.", () => ModConfig.SpawnMonsters, (bool val) => ModConfig.SpawnMonsters = val);
                api.RegisterSimpleOption(ModManifest, "Hazardous Moon Events", "This controls the moon events that can hinder the player, or spawn monsters. No monsters will spawn ( the blood moon won't even happen) while this is false. This also stops the crop deadvancement on the new moon.", () => ModConfig.HazardousMoonEvents, (bool val) => ModConfig.HazardousMoonEvents = val);
                api.RegisterSimpleOption(ModManifest, "Verbose Logging", "This controls if the mod is verbose when logging for debug purposes", () => ModConfig.Verbose, (bool val) => ModConfig.Verbose = val);
                api.RegisterSimpleOption(ModManifest, "Use More Monthly Cycle", "When true, the mod uses a 25-day cycle.", () => ModConfig.UseMoreMonthlyCycle, (bool val) => ModConfig.UseMoreMonthlyCycle = val);
                api.RegisterClampedOption(ModManifest, "Eclipse Chance", "The chance for an eclipse", () => (float)ModConfig.EclipseChance, (float val) => ModConfig.EclipseChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Super Moon Chances", "The chance for a super moon", () => (float)ModConfig.SuperMoonChances, (float val) => ModConfig.SuperMoonChances = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Harvest Moon Double Grow Chance", "The chance for a crop double growth on a harvest moon", () => (float)ModConfig.HarvestMoonDoubleGrowChance, (float val) => ModConfig.HarvestMoonDoubleGrowChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Crop Growth Chance", "The chance for crops to grow on a full moon", () => (float)ModConfig.CropGrowthChance, (float val) => ModConfig.CropGrowthChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Crop Halt Chance", "The chance for crops to not grow on a new moon", () => (float)ModConfig.CropHaltChance, (float val) => ModConfig.CropHaltChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Beach Removal Chance", "The chance for items on the beach to be removed on a new moon", () => (float)ModConfig.BeachRemovalChance, (float val) => ModConfig.BeachRemovalChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Beach Spawn Chance", "The chance for items on the beach to be spawned on a full moon", () => (float)ModConfig.BeachSpawnChance, (float val) => ModConfig.BeachSpawnChance = val, 0f, 1f);
                api.RegisterClampedOption(ModManifest, "Ghost Spawn Chance", "The chance for ghosts to spawn on a full moon", () => (float)ModConfig.GhostSpawnChance, (float val) => ModConfig.GhostSpawnChance = val, 0f, 1f);
            }

            var cpAPI = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (cpAPI != null)
            {
                cpAPI.RegisterToken(this.ModManifest, "MoonPhase", () => new[] { OurMoon.SimpleMoonPhase() });
                cpAPI.RegisterToken(this.ModManifest, "MoonRise", () => new[] { OurMoon.GetMoonRiseTime().ToString() }); 
                cpAPI.RegisterToken(this.ModManifest, "MoonSet", () => new[] { OurMoon.GetMoonSetTime().ToString() });
            }
        }
        
        private void JAAPi_AddedItemsToShop(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is ShopMenu menu && menu.portraitPerson != null)
            {
                float sellPrice, buyPrice;
                switch (OurMoon.CurrentPhase())
                {
                    case MoonPhase.BloodMoon:
                        sellPrice = .75f;
                        buyPrice = 1.85f;
                        break;
                    case MoonPhase.BlueMoon:
                        sellPrice = 1.175f;
                        buyPrice = .825f;
                        break;
                    case MoonPhase.HarvestMoon:
                        sellPrice = 1.1f;
                        buyPrice = .9f;
                        break;
                    default:
                        sellPrice = 1f;
                        buyPrice = 1f;
                        break;
                }
                Helper.Reflection.GetField<float>(menu, "sellPercentage").SetValue(Helper.Reflection.GetField<float>(menu, "sellPercentage").GetValue() * sellPrice);

                var itemPriceAndStock = Helper.Reflection.GetField<Dictionary<Item, int[]>>(menu, "itemPriceAndStock").GetValue();
                foreach (KeyValuePair<Item, int[]> kvp in itemPriceAndStock)
                {
                    kvp.Value[0] = (int)Math.Floor(kvp.Value[0] * buyPrice);
                }
            }
        }
    }
}

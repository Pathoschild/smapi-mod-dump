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
using StardewValley.Monsters;
using TwilightShards.Stardew.Common;

namespace TwilightShards.LunarDisturbances
{
    public class LunarDisturbances : Mod
    {
        internal static SDVMoon OurMoon;
        private MersenneTwister Dice;
        internal static ITranslationHelper Translation;
        private MoonConfig ModConfig;
        public static Sprites.Icons OurIcons { get; set; }
        internal static bool IsEclipse { get; set; }
        private bool UseJsonAssetsApi = false;
        private Color nightColor = new Color((int)byte.MaxValue, (int)byte.MaxValue, 0);
        private Integrations.IJsonAssetsApi JAAPi;
        internal int ResetTicker { get; set; }
        private int SecondCount;

        private ILunarDisturbancesAPI API;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Dice = new MersenneTwister();
            Translation = Helper.Translation;
            ModConfig = Helper.ReadConfig<MoonConfig>();
            OurMoon = new SDVMoon(ModConfig, Dice, Helper.Translation);
            OurIcons = new Sprites.Icons(Helper.Content);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
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
                    if (Game1.currentLocation.IsOutdoors && OurMoon.CurrentPhase == MoonPhase.BloodMoon)
                    {
                        Monitor.Log("Spawning monster....");
                        SDVUtilities.SpawnMonster(Game1.currentLocation);
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
            if (!Context.IsMainPlayer)
                return;

            //cleanup any spawned monsters
            foreach (GameLocation l in Game1.locations)
            {
                for (int index = l.characters.Count - 1; index >= 0; --index)
                {
                    if (l.characters[index] is Monster && !(l.characters[index] is GreenSlime))
                        l.characters.RemoveAt(index);
                }
            }

            if (IsEclipse)
                IsEclipse = false;

            //moon works after frost does
            OurMoon.HandleMoonAtSleep(Game1.getFarm());
        }

        private void GameLoop_SaveCreating(object sender, SaveCreatingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            //cleanup any spawned monsters
            foreach (GameLocation l in Game1.locations)
            {
                for (int index = l.characters.Count - 1; index >= 0; --index)
                {
                    if (l.characters[index] is Monster && !(l.characters[index] is GreenSlime))
                        l.characters.RemoveAt(index);
                }
            }

            if (IsEclipse)
                IsEclipse = false;

            //moon works after frost does
            OurMoon.HandleMoonAtSleep(Game1.getFarm());
        }


        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!UseJsonAssetsApi)
            {
                if (e.NewMenu is ShopMenu menu && menu.portraitPerson != null)
                {
                    if (OurMoon.CurrentPhase == MoonPhase.BloodMoon)
                    {
                        Helper.Reflection.GetField<float>(menu, "sellPercentage").SetValue(.75f);
                        var itemPriceAndStock = Helper.Reflection.GetField<Dictionary<Item, int[]>>(menu, "itemPriceAndStock").GetValue();
                        foreach (KeyValuePair<Item, int[]> kvp in itemPriceAndStock)
                        {
                            kvp.Value[0] = (int)Math.Floor(kvp.Value[0] * 1.85);
                        }
                    }
                    else
                    {
                        if (Helper.Reflection.GetField<float>(menu, "sellPercentage").GetValue() != 1f)
                        {
                            Helper.Reflection.GetField<float>(menu, "sellPercentage").SetValue(1f);
                        }
                    }
                }
            }
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (OurMoon.GetMoonRiseTime() <= 0600 || OurMoon.GetMoonRiseTime() >= 2600 && ModConfig.ShowMoonPhase)
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("moon-text.moonriseBefore6", new { moonPhase = OurMoon.DescribeMoonPhase(), riseTime = OurMoon.GetMoonRiseTime() })));

            if (OurMoon == null)
            {
                Monitor.Log("OurMoon is null");
                return;
            }

            if (Dice.NextDouble() < ModConfig.EclipseChance && ModConfig.EclipseOn && OurMoon.CurrentPhase == MoonPhase.FullMoon &&
                SDate.Now().DaysSinceStart > 2)
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

                Monitor.Log("There's a solar eclipse today!", LogLevel.Info);
            }

            OurMoon.OnNewDay();
            OurMoon.HandleMoonAfterWake();
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Game1.timeOfDay == OurMoon.GetMoonRiseTime() && ModConfig.ShowMoonPhase)
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("moon-text.moonrise", new { moonPhase = OurMoon.DescribeMoonPhase() })));

            if (Game1.timeOfDay == OurMoon.GetMoonSetTime() && ModConfig.ShowMoonPhase)
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("moon-text.moonset", new { moonPhase = OurMoon.DescribeMoonPhase() })));


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

                if ((Game1.farmEvent == null && Game1.random.NextDouble() < (0.25 - Game1.dailyLuck / 2.0))
                    && ((ModConfig.SpawnMonsters && Game1.spawnMonstersAtNight) || (ModConfig.SpawnMonstersAllFarms)) && Context.IsMainPlayer)
                {
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

            if (OurMoon.CurrentPhase == MoonPhase.BloodMoon)
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

            if (Game1.showingEndOfNightStuff && !Game1.wasRainingYesterday && !outro && Game1.activeClickableMenu is ShippingMenu currMenu && 
                ModConfig.ShowMoonInEndOfNight && currMenu.currentPage == -1)
            {          
                float scale = Game1.pixelZoom * 1.25f;

                if (Game1.viewport.Width < 1024)
                    scale = Game1.pixelZoom * .8f;
                if (Game1.viewport.Width < 810)
                    scale = Game1.pixelZoom * .6f;
                if (Game1.viewport.Width < 700)
                    scale = Game1.pixelZoom * .35f;

                Game1.spriteBatch.Draw(OurIcons.MoonSource, new Vector2(Game1.viewport.Width - 65 * Game1.pixelZoom, Game1.pixelZoom), OurIcons.GetNightMoonSprite(SDVMoon.GetLunarPhaseForDay(SDate.Now().AddDays(-1))), Color.LightBlue, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
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
                Monitor.Log("JsonAssets Integration enabled", LogLevel.Info);
            }
        }

        private void OutputLight(string arg1, string[] arg2)
        {
            Monitor.Log($"The outdoor light is {Game1.outdoorLight.ToString()}. The ambient light is {Game1.ambientLight.ToString()}");
        }
        
        private void JAAPi_AddedItemsToShop(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is ShopMenu menu)
            {
                if (OurMoon.CurrentPhase == MoonPhase.BloodMoon)
                {
                    Monitor.Log("Firing off replacement...");
                    Helper.Reflection.GetField<float>(menu, "sellPercentage").SetValue(.75f);
                    var itemPriceAndStock = Helper.Reflection.GetField<Dictionary<Item, int[]>>(menu, "itemPriceAndStock").GetValue();
                    foreach (KeyValuePair<Item, int[]> kvp in itemPriceAndStock)
                    {
                        kvp.Value[0] = (int)Math.Floor(kvp.Value[0] * 1.85);
                    }
                }
                else
                {
                    if (Helper.Reflection.GetField<float>(menu, "sellPercentage").GetValue() != 1f)
                    {
                        Helper.Reflection.GetField<float>(menu, "sellPercentage").SetValue(1f);
                    }
                }
            }
        }
    }
}

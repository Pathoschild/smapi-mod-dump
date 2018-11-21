using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;

namespace SolarEclipseEvent
{
    public class SolarEclipseEvent : Mod
    {
        public EclipseConfig Config { get; set; }
        public bool GameLoaded { get; set; }
        public bool IsEclipse { get; set; }
        public int ResetTicker { get; set; }
        public bool Disabled { get; set; }

        private Color nightColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, 0);

        public override void Entry(IModHelper helper)
        {
            Disabled = false;
            if (this.Helper.ModRegistry.IsLoaded("KoihimeNakamura.ClimatesOfFerngill"))
            {
                IManifest manifest = this.Helper.ModRegistry.Get("KoihimeNakamura.ClimatesOfFerngill");
                if (manifest.Version.IsNewerThan("1.3.0-beta1") && manifest.Version.IsOlderThan("1.4.0")) {
                    Disabled = true;
                    Monitor.Log("Disabled due to version 1.3.0 - 1.4.0 of Climates of Ferngill loaded", LogLevel.Alert);
                }                        
            }

            if (this.Helper.ModRegistry.IsLoaded("KoihimeNakamura.LunarDisturbances"))
            {
                IManifest manifest = this.Helper.ModRegistry.Get("KoihimeNakamura.LunarDisturbances");
                if (manifest.Version.IsNewerThan("0.9"))
                {
                    Disabled = true;
                    Monitor.Log("Disabled due to version .9+ of Lunar Disturbances loaded", LogLevel.Alert);
                }
            }

            Config = Helper.ReadConfig<EclipseConfig>();

            if (!Disabled)
            {
                helper.ConsoleCommands
                    .Add("world_solareclipse", "Starts the solar eclipse.", SolarEclipseEvent_CommandFired);

                SaveEvents.AfterLoad += SaveEvents_AfterLoad;
                SaveEvents.BeforeSave += SaveEvents_BeforeSave;
                TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
                TimeEvents.TimeOfDayChanged += TimeEvents_TimeOfDayChanged;
                GameEvents.UpdateTick += GameEvents_UpdateTick;
                LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;

                ResetTicker = 0;
            }
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (IsEclipse && ResetTicker > 0)
            {
                Game1.globalOutdoorLighting = .5f;
                Game1.ambientLight = nightColor;
                Game1.currentLocation.switchOutNightTiles();
                ResetTicker = 0;
            }
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            Random r = new Random();
            
            if (r.NextDouble() < Config.EclipseChance)
            {
                IsEclipse = true;
                Game1.addHUDMessage(new HUDMessage("It looks like a rare solar eclipse will darken the sky all day!"));
            }
        }

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
        }

        private void TimeEvents_TimeOfDayChanged(object sender, EventArgsIntChanged e)
        {
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
                    && ((Config.SpawnMonsters && Game1.spawnMonstersAtNight) || (Config.SpawnMonstersAllFarms)))
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
        }



        private void SaveEvents_BeforeSave(object sender, System.EventArgs e)
        {
            if (IsEclipse)
                IsEclipse = false;
        }

        private void SaveEvents_AfterLoad(object sender, System.EventArgs e)
        {
            GameLoaded = true;
        }

        private void SolarEclipseEvent_CommandFired(string command, string[] args)
        {
            IsEclipse = true;
            Game1.globalOutdoorLighting = .5f; //force lightning change.
            Game1.currentLocation.switchOutNightTiles();
            Game1.ambientLight = nightColor;
            Monitor.Log("Setting the eclipse event to true");
        }

    }
}

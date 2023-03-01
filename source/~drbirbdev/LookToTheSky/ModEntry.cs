/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BirbShared;
using BirbShared.Asset;
using BirbShared.Command;
using BirbShared.Config;
using StardewModdingAPI;
using StardewValley;

namespace LookToTheSky
{
    // TODOs:
    // Multiplayer compatibility (shared animations/controls/sound)
    // Text notifications option
    // More items (balloon)
    // Screen resize functionality
    // Make sure sprites are centered correctly
    // Loot tables in JSON
    // Slingshot charge sound/cooldown???
    // Content pack functionality
    public class ModEntry : Mod
    {

        internal static ModEntry Instance;
        internal static Config Config;
        internal static Assets Assets;


        public readonly List<SkyObject> SkyObjects = new();
        public readonly List<SkyProjectile> Projectiles = new();

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Log.Init(this.Monitor);
            Config = helper.ReadConfig<Config>();
            Assets = new Assets();
            new AssetClassParser(this, Assets).ParseAssets();

            this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            this.Helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
            this.Helper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.GameLoop_OneSecondUpdateTicked;
            this.Helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
        }

        private void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            SkyObjects.Clear();
            Projectiles.Clear();
        }

        // TODO: make a factory or something...
        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.CanPlayerMove && Game1.activeClickableMenu is not SkyMenu)
            {
                return;
            }
            if (Game1.random.Next(100) < Config.SpawnChancePerSecond)
            {
                int rand = Game1.random.Next(100);
                if (rand <= 10)
                {
                    if (Game1.timeOfDay >= 1800)
                    {
                        if (Game1.currentSeason == "winter" && Game1.dayOfMonth == 25)
                        {
                            SkyObjects.Add(new Santa(rand % 5 * 10, rand % 2 == 0));
                        }
                        else
                        {
                            SkyObjects.Add(new UFO(rand % 5 * 10, rand % 2 == 0));
                        }
                    }
                    else
                    {
                        SkyObjects.Add(new Plane(rand % 5 * 10, rand % 2 == 0));
                    }
                }
                else if (rand <= 20)
                {
                    if (Game1.timeOfDay >= 1800)
                    {
                        SkyObjects.Add(new Witch(rand % 5 * 10, rand % 2 == 0));
                    }
                    else
                    {
                        SkyObjects.Add(new Bird(20, rand % 2 == 0));
                        Task.Delay(2000).ContinueWith((t) =>
                        {
                            SkyObjects.Add(new Bird(25, rand % 2 == 0));
                            SkyObjects.Add(new Bird(15, rand % 2 == 0));
                        });
                        Task.Delay(4000).ContinueWith((t) =>
                        {
                            SkyObjects.Add(new Bird(30, rand % 2 == 0));
                            SkyObjects.Add(new Bird(10, rand % 2 == 0));
                        });
                    }
                }
                else if (rand <= 30 && Game1.player.eventsSeen.Contains(10))
                {
                    SkyObjects.Add(new Robot(rand % 5 * 10, rand % 2 == 0));
                }
                else 
                {
                    SkyObjects.Add(new Bird(rand, rand % 2 == 0));
                }
            }
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            for (int i = SkyObjects.Count - 1; i >= 0; i--)
            {
                if (SkyObjects[i].X < -100 || SkyObjects[i].X > Game1.viewport.Width + 100 || SkyObjects[i].Y < -100 || SkyObjects[i].Y > Game1.viewport.Height + 100)
                {
                    SkyObjects[i].OnExit();
                    SkyObjects.RemoveAt(i);
                }
                else
                {
                    SkyObjects[i].tick();
                }
            }
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                if (Projectiles[i].Y < 0)
                {
                    Projectiles.RemoveAt(i);
                }
                else
                {
                    if (Projectiles[i].UpdatePosition(Game1.currentGameTime))
                    {
                        Projectiles.RemoveAt(i);
                    }
                }
            }
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            new ConfigClassParser(this, Config).ParseConfigs();
            new CommandClassParser(this.Helper.ConsoleCommands, new Command()).ParseCommands();
        }

        // Open and close the sky menu
        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && Context.CanPlayerMove && Game1.currentLocation.IsOutdoors && e.Button.Equals(Config.Button))
            {
                if (Game1.activeClickableMenu is SkyMenu)
                {
                    Game1.activeClickableMenu = null;
                }
                else
                {
                    Game1.activeClickableMenu = new SkyMenu();
                }
            }
        }
    }
}

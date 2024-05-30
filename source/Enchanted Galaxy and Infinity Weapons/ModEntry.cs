/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/namelessto/EnchantedGalaxyWeapons
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Locations;
using StardewValley.Tools;
using xTile.Layers;

namespace EnchantedGalaxyWeapons
{
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/

        public static Mod ModInstance;
        public static ModConfig Config;

        public static bool UnlockedGalaxy;
        public static bool UnlockedInfinity;
        public static int MaxSpawnForDay = 0;

        private HUDMessage Message = new("");

        /*********
        ** Public methods
        *********/

        /// <summary> The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.ModInstance = this;
            Config = this.Helper.ReadConfig<ModConfig>();
            Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.DailyChecks;
            helper.Events.Player.Warped += this.CheckIfMine;
        }

        /*********
        ** Private methods
        *********/
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            ModMenu.BuildMenu(this.Helper, this.ModManifest, configMenu);
        }

        /// <summary> Check if player obtained galaxy or infinity weapons</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DailyChecks(object? sender, DayStartedEventArgs e)
        {
            UnlockedGalaxy = Game1.player.mailReceived.Contains("galaxySword");
            UnlockedInfinity = Game1.player.achievements.Contains(42);
            MaxSpawnForDay = Config.DailySpawnLimit + Math.Max(0, Game1.player.LuckLevel);
        }

        /// <summary> Check if player is currently in the mines or skull cavern</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckIfMine(object? sender, WarpedEventArgs e)
        {
            GameLocation location = e.NewLocation;
            if (location is MineShaft mineShaft)
            {
                if (Config.HaveDailySpawnLimit && MaxSpawnForDay <= 0)
                {
                    return;
                }

                if (Config.SkipGalaxyCheck || Config.SkipInfinityCheck)
                {
                    TrySpawnBreakableContainer(mineShaft);
                }
                else if (UnlockedGalaxy || UnlockedInfinity)
                {
                    TrySpawnBreakableContainer(mineShaft);
                }
            }
        }

        /// <summary> Try spawn a "custom" breakable crate </summary>
        /// <param name="mine"></param>
        private void TrySpawnBreakableContainer(MineShaft mine)
        {
            Random mineRandom = new();

            bool shouldSpawnCrate = false;
            int amountToSpawn = 1;

            Layer backLayer = mine.map.GetLayer("Back");
            List<Point> points = new();
            Point p = new(mineRandom.Next(backLayer.LayerWidth), mineRandom.Next(backLayer.LayerHeight));
            points.Add(p);
            Vector2 objectPos = new(0, 0);
            if (Config.AllowMoreThanOne)
            {
                amountToSpawn += Config.AdditionalBarrels;

                while (amountToSpawn > 0)
                {
                    if (mine.isTileClearForMineObjects(p.X, p.Y))
                    {
                        objectPos = new(p.X, p.Y);
                        mine.objects.Add(objectPos, CustomBreakableObject.GetBarrelForMines(objectPos, mine));
                        amountToSpawn--;
                    }
                    p = new Point(mineRandom.Next(backLayer.LayerWidth), mineRandom.Next(backLayer.LayerHeight));
                }
                Message = HUDMessage.ForCornerTextbox(this.Helper.Translation.Get("game.aura-felt"));
                Game1.addHUDMessage(Message);
            }
            else
            {
                int numberOfTries = (int)(mine.mineLevel / 120.0 + Game1.player.DailyLuck + mineRandom.NextDouble());
                for (int i = 0; i < numberOfTries + Config.AdditionalTriesToSpawn; i++)
                {
                    if (mine.isTileClearForMineObjects(p.X, p.Y))
                    {
                        shouldSpawnCrate = true;
                        objectPos = new(p.X, p.Y);
                        break;
                    }
                    else
                    {
                        p = new Point(mineRandom.Next(backLayer.LayerWidth), mineRandom.Next(backLayer.LayerHeight));
                    }
                }
                if (shouldSpawnCrate)
                {
                    mine.objects.Add(objectPos, CustomBreakableObject.GetBarrelForMines(objectPos, mine));

                    Message = HUDMessage.ForCornerTextbox(this.Helper.Translation.Get("game.aura-felt"));
                    Game1.addHUDMessage(Message);
                }
            }
        }

    }
}

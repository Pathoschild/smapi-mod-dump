/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Incognito357/PrairieKingUIEnhancements
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static StardewValley.Minigames.AbigailGame;

namespace PrairieKingUIEnhancements
{
    class ModEntry : Mod
    {
        private Config config;
        private Save save;
        private List<Renderable> renderables;

        private static readonly string SAVE_KEY = "JOTPKProgress";

        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.Rendered += Rendered;
            helper.Events.GameLoop.UpdateTicked += Ticked;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.GameLoop.Saving += Saving;
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            
            config = Helper.ReadConfig<Config>();

            renderables = new List<Renderable>()
            {
                new LevelIndicators(Helper.Content.Load<Texture2D>("indicators.png", ContentSource.ModFolder)),
                new Stats(),
                new PowerupTimers(),
                new BossHealth(),
                new LevelTransitionColor()
            };
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.genericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                ModManifest,
                () => config = new Config(),
                () => Helper.WriteConfig(config));

            foreach (var prop in config.GetType().GetProperties())
            {
                configMenu.AddBoolOption(
                    ModManifest,
                    () => (bool)prop.GetValue(config),
                    value => prop.SetValue(config, value),
                    () => prop.Name);
            }
        }

        private void Rendered(object sender, RenderedEventArgs e)
        {
            if (Game1.currentMinigame is not AbigailGame game)
            {
                return;
            }

            if (AbigailGame.onStartMenu ||
                ((AbigailGame.gameOver || game.gamerestartTimer > 0) && !AbigailGame.endCutscene) ||
                AbigailGame.endCutscene ||
                (AbigailGame.gopherTrain && AbigailGame.gopherTrainPosition > -AbigailGame.TileSize) ||
                AbigailGame.zombieModeTimer > 8200)
            {
                return;
            }

            var b = e.SpriteBatch;

            foreach (Renderable r in renderables)
            {
                r.Render(b, config, game);
            }
        }

        private void Ticked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.currentMinigame is not AbigailGame game)
            {
                return;
            }

            foreach (Renderable r in renderables)
            {
                r.Tick(config, game);
            }
        }

        private void Saving(object sender, SavingEventArgs e)
        {
            foreach (Renderable r in renderables)
            {
                r.Save(ref save);
            }
            Helper.Data.WriteSaveData(SAVE_KEY, save);
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            save = Helper.Data.ReadSaveData<Save>(SAVE_KEY);
            if (save == null)
            {
                save = new Save();
            }

            foreach (Renderable r in renderables)
            {
                r.SaveLoaded(save);
            }
        }
    }
}

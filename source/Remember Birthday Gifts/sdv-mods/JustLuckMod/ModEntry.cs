/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zeldela/sdv-mods
**
*************************************************/

using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace JustLuckMod
{
    public class ModEntry : Mod
    {
        string fortuneMessage;
        string fortuneScoreMessage;
        double fortuneScore;
        int fortuneBuff;
        Color fortuneColor;
        Point luckCoords;
        ClickableTextureComponent luckIcon;
        ModConfig config;
        IModHelper helper;
        LuckHUD luckHUD;
        bool hideToday;

        public override void Entry(IModHelper helper)
        {
            luckHUD = new LuckHUD(helper);
            this.config = this.Helper.ReadConfig<ModConfig>();
            this.helper = helper;

            if (!config.Disable)
            {
                helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
                helper.Events.GameLoop.DayStarted += this.OnDayStarted;
                helper.Events.Display.RenderedHud += this.OnRenderedHud;
                helper.Events.Display.WindowResized += this.OnWindowResized;
                helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;

            }

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Disable",
                getValue: () => this.config.Disable,
                setValue: value => this.config.Disable = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Fortune",
                tooltip: () => "Whether to include the TV Forecast dialouge in the hover text (luck buffs can make the forecast obsolete).",
                getValue: () => this.config.Fortune,
                setValue: value => this.config.Fortune = value
            );

            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle",
                tooltip: () => "The key used to toggle the icon (resets on the next day).",
                getValue: () => this.config.Toggle,
                setValue: value => this.config.Toggle = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Monochrome",
                tooltip: () => "Keep the original icon regardless of luck score (no red-green scale).",
                getValue: () => this.config.Monochrome,
                setValue: value => this.config.Monochrome = value
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Location",
                tooltip: () => "Choose the location of the icon (default 'HUD' or 'Stamina Bar').",
                getValue: () => this.config.Location,
                setValue: value => this.config.Location = value,
                allowedValues: new string[] {"HUD", "Stamina Bar"}
            );

        }

        internal void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            fortuneScore = luckHUD.GetFortuneScore(Game1.player);
            fortuneMessage = (this.config.Fortune) ? luckHUD.GetFortuneMessage(Game1.player) : "";
            hideToday = false;
            this.Monitor.Log($"Today's luck for {Game1.player.Name}: {fortuneScore}", LogLevel.Debug);

        }

        internal void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!Context.IsWorldReady || hideToday)
                return;

            luckCoords = luckHUD.GetIconCoords(this.config);
            luckIcon = luckHUD.GetLuckIcon(luckCoords);
            fortuneScore = luckHUD.GetFortuneScore(Game1.player);
            fortuneBuff = luckHUD.GetFortuneBuff(Game1.player);
            fortuneColor = (!this.config.Monochrome) ? (fortuneBuff > 0 ? luckHUD.GetFortuneColor(fortuneScore + (fortuneBuff / 100f)) : luckHUD.GetFortuneColor(fortuneScore)) : Color.White;

            if (Game1.displayHUD && luckIcon != null)
            {
                luckIcon.bounds.X = luckCoords.X;
                luckIcon.bounds.Y = luckCoords.Y;
                luckIcon.draw(Game1.spriteBatch, fortuneColor, 1);

                if (luckIcon.containsPoint(Game1.getMouseX(), Game1.getMouseY()) && !hideToday)
                {
                    fortuneScoreMessage = $"{fortuneMessage}{luckHUD.GetFortune(fortuneScore)}{luckHUD.GetBuffString(fortuneBuff)}";
                    IClickableMenu.drawHoverText(Game1.spriteBatch, fortuneScoreMessage, Game1.smallFont);
                }

            }
        }

        internal void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (this.config.Toggle.JustPressed())
            {
                hideToday = (hideToday) ? false : true;
            } 
        }

        internal void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            if (!Context.IsWorldReady || hideToday)
                return;

            luckCoords = luckHUD.GetIconCoords(this.config);
            luckIcon = luckHUD.GetLuckIcon(luckCoords);
        }

    }
}


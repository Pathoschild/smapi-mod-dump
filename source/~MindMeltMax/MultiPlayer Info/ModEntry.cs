/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using System.Collections.Generic;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;

namespace MPInfo
{
    public enum Position
    {
        TopLeft,
        BottomLeft,
        BottomRight,
        CenterRight,
    }

    internal class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal static Config Config = null!;

        private readonly PerScreen<PlayerInfo> infoCache = new(() => new());

        private bool enabled = true;

        public bool IsEnabled => enabled;
        private PerScreen<int> playersOnline = new(() => -1); //Might not need perscreen, since it's only set and checked by the host, will probably make int in next update

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            PlayerInfoBox.Crown = helper.ModContent.Load<Texture2D>("Assets/Crown.png");
            Config = helper.ReadConfig<Config>();
            enabled = Config.Enabled;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            helper.Events.Multiplayer.ModMessageReceived += OnMultiplayerDataReceived;

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += (_, _) => Game1.player.modData.Remove(ModManifest.UniqueID);
            helper.Events.GameLoop.ReturnedToTitle += (_, _) => playersOnline.Value = -1;

            Patches.Apply(ModManifest.UniqueID);
        }

        internal void ForceUpdate()
        {
            infoCache.Value = new(Game1.player);
            Game1.player.modData[ModManifest.UniqueID] = infoCache.Value.Serialize();
            Helper.Multiplayer.SendMessage("", "MPInfo.ForceUpdate", [ModManifest.UniqueID]);
            ResetDisplays();
        }

        private void ResetDisplays()
        {
            Game1.onScreenMenus = new List<IClickableMenu>(Game1.onScreenMenus.Where(x => x is not PlayerInfoBox));
            if (Config.ShowSelf)
                Game1.onScreenMenus.Add(new PlayerInfoBox(Game1.player));
            foreach (var player in Helper.Multiplayer.GetConnectedPlayers())
                Game1.onScreenMenus.Add(new PlayerInfoBox(Game1.getFarmer(player.PlayerID)));
            PlayerInfoBox.RedrawAll();
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.Enabled.Name"),
                tooltip: () => "",
                getValue: () => Config.Enabled,
                setValue: value =>
                {
                    Config.Enabled = enabled = value;
                    ResetDisplays();
                }
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.ToggleButton.Name"),
                tooltip: () => Helper.Translation.Get("Config.ToggleButton.Description"),
                getValue: () => Config.ToggleButton,
                setValue: value => Config.ToggleButton = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.ShowSelf.Name"),
                tooltip: () => Helper.Translation.Get("Config.ShowSelf.Description"),
                getValue: () => Config.ShowSelf,
                setValue: value =>
                {
                    Config.ShowSelf = value;
                    ResetDisplays();
                }
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.ShowHostCrown.Name"),
                tooltip: () => Helper.Translation.Get("Config.ShowHostCrown.Description"),
                getValue: () => Config.ShowHostCrown,
                setValue: value => Config.ShowHostCrown = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.HideBars.Name"),
                tooltip: () => "",
                getValue: () => Config.HideHealthBars,
                setValue: value => Config.HideHealthBars = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.PositionBoxes.Name"),
                tooltip: () => Helper.Translation.Get("Config.PositionBoxes.Description"),
                getValue: () => Enum.GetName(Config.Position)!,
                setValue: value =>
                {
                    Config.Position = Enum.Parse<Position>(value);
                    PlayerInfoBox.RedrawAll();
                },
                allowedValues: Enum.GetNames<Position>()
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.XOffset.Name"),
                tooltip: () => Helper.Translation.Get("Config.XOffset.Description"),
                getValue: () => Config.XOffset,
                setValue: value =>
                {
                    Config.XOffset = value;
                    PlayerInfoBox.RedrawAll();
                }
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.YOffset.Name"),
                tooltip: () => Helper.Translation.Get("Config.YOffset.Description"),
                getValue: () => Config.YOffset,
                setValue: value =>
                {
                    Config.YOffset = value;
                    PlayerInfoBox.RedrawAll();
                }
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Config.SpaceBetween.Name"),
                tooltip: () => Helper.Translation.Get("Config.SpaceBetween.Description"),
                getValue: () => Config.SpaceBetween,
                setValue: value =>
                {
                    Config.SpaceBetween = value;
                    PlayerInfoBox.RedrawAll();
                }
             );
        }

        public void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button == Config.ToggleButton)
                enabled = !enabled;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            infoCache.Value = new(Game1.player);
            Game1.player.modData[ModManifest.UniqueID] = infoCache.Value.Serialize();
            ResetDisplays();
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (!infoCache.Value.IsMatch(Game1.player))
            {
                infoCache.Value = new(Game1.player);
                Game1.player.modData[ModManifest.UniqueID] = infoCache.Value.Serialize();
                ResetDisplays();
            }
            if (!Context.IsMainPlayer)
                return;
            if (playersOnline.Value != Game1.numberOfPlayers())
            {
                playersOnline.Value = Game1.numberOfPlayers();
                ForceUpdate();
            }
        }

        private void OnMultiplayerDataReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != Helper.ModRegistry.ModID || e.Type != "MPInfo.ForceUpdate")
                return;

            infoCache.Value = new(Game1.player);
            Game1.player.modData[ModManifest.UniqueID] = infoCache.Value.Serialize();
            ResetDisplays();

            /*var display = (PlayerInfoBox?)Game1.onScreenMenus.FirstOrDefault(x => x is PlayerInfoBox pib && pib.Who.UniqueMultiplayerID == e.FromPlayerID);
            if (display is null && e.Type != "MPInfo.ForceUpdate")
                return;

            switch (e.Type)
            {
                case "MPInfo.ForceUpdate": //Let updateticked handle the messages
                    infoCache.Value = new(Game1.player);
                    Game1.player.modData[ModManifest.UniqueID] = infoCache.Value.Serialize();
                    ResetDisplays();
                    break;
            }*/
        }
    }
}

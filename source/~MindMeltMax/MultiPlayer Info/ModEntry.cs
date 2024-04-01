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
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

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

        private int lastMaxHealth;
        private int lastHealth;
        private bool enabled;

        public bool IsEnabled => enabled;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            PlayerInfoBox.Crown = helper.ModContent.Load<Texture2D>("Assets/Crown.png");
            Config = helper.ReadConfig<Config>();
            enabled = Config.Enabled;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            helper.Events.Multiplayer.PeerConnected += OnPlayerJoin;
            helper.Events.Multiplayer.PeerDisconnected += OnPlayerLeave;
            helper.Events.Multiplayer.ModMessageReceived += OnMultiplayerDataReceived;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            Patches.Apply(ModManifest.UniqueID);
        }

        internal void ForceUpdate()
        {
            var ids = JsonConvert.SerializeObject(Helper.Multiplayer.GetConnectedPlayers().Select(x => x.PlayerID));
            Helper.Multiplayer.SendMessage(ids, "MPInfo.ForceUpdate", new[] { ModManifest.UniqueID });
            ResetDisplays();
        }

        private void ResetDisplays(IEnumerable<long>? playerIds = null)
        {
            var displays = Game1.onScreenMenus.OfType<PlayerInfoBox>().ToArray();
            for (int i = 0; i < displays.Length; i++)
                Game1.onScreenMenus.Remove(displays[i]);
            if (Config.ShowSelf)
                Game1.onScreenMenus.Add(new PlayerInfoBox(Game1.player));
            foreach (var player in playerIds ?? Helper.Multiplayer.GetConnectedPlayers().Select(x => x.PlayerID))
                Game1.onScreenMenus.Add(new PlayerInfoBox(Game1.getFarmer(player)));
            PlayerInfoBox.RedrawAll();
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new Config(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enabled",
                tooltip: () => "",
                getValue: () => Config.Enabled,
                setValue: value =>
                {
                    Config.Enabled = enabled = value;
                    PlayerInfoBox.RedrawAll();
                }
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Button to toggle display",
                tooltip: () => "",
                getValue: () => Config.ToggleButton,
                setValue: value => Config.ToggleButton = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Show Self",
                tooltip: () => "",
                getValue: () => Config.ShowSelf,
                setValue: value =>
                {
                    Config.ShowSelf = value;
                    PlayerInfoBox.RedrawAll();
                }
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Show Host Crown",
                tooltip: () => "",
                getValue: () => Config.ShowHostCrown,
                setValue: value => Config.ShowHostCrown = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Hide Health and Stamina Bars",
                tooltip: () => "",
                getValue: () => Config.HideHealthBars,
                setValue: value => Config.HideHealthBars = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Position of boxes",
                tooltip: () => "",
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
                name: () => "X Offset",
                tooltip: () => "Offset the boxes horizontally",
                getValue: () => Config.XOffset,
                setValue: value =>
                {
                    Config.XOffset = value;
                    PlayerInfoBox.RedrawAll();
                }
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Y Offset",
                tooltip: () => "Offset the boxes vertically",
                getValue: () => Config.YOffset,
                setValue: value =>
                {
                    Config.YOffset = value;
                    PlayerInfoBox.RedrawAll();
                }
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Space Between",
                tooltip: () => "The distance between displays",
                getValue: () => Config.SpaceBetween,
                setValue: value =>
                {
                    Config.SpaceBetween = value;
                    PlayerInfoBox.RedrawAll();
                }
             );
        }

        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == Config.ToggleButton)
                enabled = !enabled;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            lastHealth = Game1.player.health;
            lastMaxHealth = Game1.player.maxHealth;
            ResetDisplays();
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.player.health != lastHealth)
                Helper.Multiplayer.SendMessage(Game1.player.health, "MPInfo.Health", new[] { ModManifest.UniqueID });
            if (Game1.player.maxHealth != lastMaxHealth)
                Helper.Multiplayer.SendMessage(Game1.player.maxHealth, "MPInfo.MaxHealth", new[] { ModManifest.UniqueID });
        }

        private void OnPlayerJoin(object? sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            ForceUpdate();
        }

        private void OnPlayerLeave(object? sender, PeerDisconnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            ForceUpdate();
        }

        private void OnMultiplayerDataReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != Helper.ModRegistry.ModID)
                return;

            var display = (PlayerInfoBox?)Game1.onScreenMenus.FirstOrDefault(x => x is PlayerInfoBox pib && pib.Who.UniqueMultiplayerID == e.FromPlayerID);
            if (display is null && e.Type != "MPInfo.ForceUpdate")
                return;

            switch (e.Type)
            {
                case "MPInfo.Health":
                    display!.Who.health = lastHealth = e.ReadAs<int>();
                    break;
                case "MPInfo.MaxHealth":
                    display!.Who.maxHealth = lastMaxHealth = e.ReadAs<int>();
                    break;
                case "MPInfo.ForceUpdate": //Let updateticked handle the messages
                    lastHealth = -1;
                    lastMaxHealth = -1;
                    ResetDisplays(JsonConvert.DeserializeObject<IEnumerable<long>>(e.ReadAs<string>())?.Where(x => x != Game1.player.UniqueMultiplayerID)?.Append(e.FromPlayerID));
                    break;
            }
        }
    }
}

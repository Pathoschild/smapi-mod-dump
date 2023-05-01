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
using System.Collections.Generic;
using System.Linq;

namespace MPInfo 
{
    public enum Position
    {
        TopLeft,
        BottomLeft,
    }

    internal class ModEntry : Mod 
    {
        internal static ModEntry Instance;
        internal static Config Config = null!;

        private int lastMaxHealth;
        private int lastHealth;

        public override void Entry(IModHelper helper) 
        {
            Instance = this;
            PlayerInfoBox.Crown = helper.ModContent.Load<Texture2D>("Assets/Crown.png");
            Config = helper.ReadConfig<Config>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.Multiplayer.PeerConnected += OnPlayerJoin;
            helper.Events.Multiplayer.PeerDisconnected += OnPlayerLeave;
            helper.Events.Multiplayer.ModMessageReceived += OnMultiplayerDataReceived;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            Patches.Apply(ModManifest.UniqueID);
        }

        internal void ForceUpdate()
        {
            Helper.Multiplayer.SendMessage("", "MPInfo.ForceUpdate", new[] { ModManifest.UniqueID });
            ResetDisplays();
        }

        private void ResetDisplays() 
        {
            var displays = Game1.onScreenMenus.Where(x => x is PlayerInfoBox).OfType<PlayerInfoBox>().ToArray();
            var reportedHealthList = new List<int>(displays.Select(x => x.Who.health));
            for (int i = 0; i < displays.Length; i++)
                Game1.onScreenMenus.Remove(displays[i]);
            PlayerInfoBox display = null!;
            foreach (var player in Game1.getOnlineFarmers()) 
            {
                display = new(player);
                Game1.onScreenMenus.Add(display);
            }
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
                    Config.Enabled = value;
                    PlayerInfoBox.RedrawAll();
                }
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

        private void OnPlayerJoin(object? sender, PeerConnectedEventArgs e) => ForceUpdate();

        private void OnPlayerLeave(object? sender, PeerDisconnectedEventArgs e) => ForceUpdate();

        private void OnMultiplayerDataReceived(object? sender, ModMessageReceivedEventArgs e) 
        {
            if (e.FromModID != Helper.ModRegistry.ModID)
                return;

            var display = (PlayerInfoBox?)Game1.onScreenMenus.FirstOrDefault(x => x is PlayerInfoBox pib && pib.Who.UniqueMultiplayerID == e.FromPlayerID);
            if (display is null)
                return;

            switch (e.Type)
            {
                case "MPInfo.Health":
                    display.Who.health = lastHealth = e.ReadAs<int>();
                    break;
                case "MPInfo.MaxHealth":
                    display.Who.maxHealth = lastMaxHealth = e.ReadAs<int>();
                    break;
                case "MPInfo.ForceUpdate": //Let updateticked handle the messages
                    lastHealth = -1;
                    lastMaxHealth = -1;
                    ResetDisplays();
                    break;
            }
        }
    }
}

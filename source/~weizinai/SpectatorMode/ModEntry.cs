/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using weizinai.StardewValleyMod.Common.Log;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using weizinai.StardewValleyMod.SpectatorMode.Framework;
using weizinai.StardewValleyMod.SpectatorMode.Handler;

namespace weizinai.StardewValleyMod.SpectatorMode;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    // 轮播玩家
    private int cooldown;
    private bool isRotatingPlayers;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        SpectatorHelper.Init(this.config);
        this.InitHandler();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // 轮播玩家
        if (this.isRotatingPlayers)
        {
            if (this.cooldown >= this.config.RotationInterval)
            {
                this.cooldown = 0;
                var farmer = Game1.random.ChooseFrom(Game1.otherFarmers.Values.ToList());
                Game1.activeClickableMenu = new SpectatorMenu(this.config, farmer.currentLocation, farmer, true);
            }
            else
            {
                this.cooldown++;
                if (Game1.activeClickableMenu is not SpectatorMenu)
                {
                    this.isRotatingPlayers = false;
                    var message = new HUDMessage(I18n.UI_StopRotatePlayer())
                    {
                        noIcon = true
                    };
                    Game1.addHUDMessage(message);
                }
            }
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        // 旁观地点
        if (this.config.SpectateLocationKey.JustPressed() && Context.IsPlayerFree)
        {
            var locations = Game1.locations.Where(location => location.IsOutdoors)
                .Select(location => new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName)).ToList();
            Game1.currentLocation.ShowPagedResponses("", locations, SpectatorHelper.SpectateLocation, false, true, 10);
        }

        // 旁观玩家
        if (this.config.SpectatePlayerKey.JustPressed() && Context.IsPlayerFree)
        {
            if (Context.HasRemotePlayers)
            {
                var players = new List<KeyValuePair<string, string>>();
                foreach (var (_, farmer) in Game1.otherFarmers)
                    players.Add(new KeyValuePair<string, string>(farmer.Name, farmer.displayName));
                Game1.currentLocation.ShowPagedResponses("", players, SpectatorHelper.SpectateFarmer, false, true, 10);
            }
            else
            {
                var message = new HUDMessage(I18n.UI_NoPlayerOnline())
                {
                    noIcon = true
                };
                Game1.addHUDMessage(message);
            }
        }

        // 轮播玩家
        if (this.config.RotatePlayerKey.JustPressed())
        {
            if (Context.HasRemotePlayers)
            {
                if (this.isRotatingPlayers)
                    Game1.activeClickableMenu.exitThisMenu();
                else
                    this.cooldown = this.config.RotationInterval;

                this.isRotatingPlayers = !this.isRotatingPlayers;
                var message = new HUDMessage(this.isRotatingPlayers ? I18n.UI_StartRotatePlayer() : I18n.UI_StopRotatePlayer())
                {
                    noIcon = true
                };
                Game1.addHUDMessage(message);
            }
            else
            {
                var message = new HUDMessage(I18n.UI_NoPlayerOnline())
                {
                    noIcon = false
                };
                Game1.addHUDMessage(message);
            }
        }
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new CommandHandler(this.Helper, this.config)
        };

        foreach (var handler in handlers) handler.Init();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuForSpectatorMode(this.Helper, this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using AutoBreakGeode.Framework;
using Common.Integration;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace AutoBreakGeode;

public class ModEntry : Mod
{
    private bool autoBreakGeode;
    private bool hasFastAnimation;
    private ModConfig config = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        hasFastAnimation = helper.ModRegistry.IsLoaded("Pathoschild.FastAnimations");
        config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );
        configMenu.AddKeybindList(
            ModManifest,
            () => config.AutoBreakGeodeKey,
            value => { config.AutoBreakGeodeKey = value; },
            I18n.Config_AutoBreakGeodeKey_Name
        );
        configMenu.AddNumberOption(
            ModManifest,
            () => config.BreakGeodeSpeed,
            value => config.BreakGeodeSpeed = value,
            I18n.Config_BreakGeodeSpeed_Name,
            null,
            1,
            20
        );
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.AutoBreakGeodeKey.JustPressed()) autoBreakGeode = autoBreakGeode == false;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is GeodeMenu geodeMenu && autoBreakGeode)
        {
            if (Utility.IsGeode(geodeMenu.heldItem))
            {
                if (geodeMenu.geodeAnimationTimer <= 0)
                {
                    var x = geodeMenu.geodeSpot.bounds.Center.X;
                    var y = geodeMenu.geodeSpot.bounds.Center.Y;
                    geodeMenu.receiveLeftClick(x, y);
                }
                else
                {
                    if (!hasFastAnimation) for (var i = 0; i < config.BreakGeodeSpeed - 1; i++) geodeMenu.update(Game1.currentGameTime);
                }

                if (Game1.player.freeSpotsInInventory() == 1) autoBreakGeode = false;
            }
            else
            {
                autoBreakGeode = false;
            }
        }
        else
        {
            autoBreakGeode = false;
        }
    }
}
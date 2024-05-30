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
using AutoBreakGeode.Patches;
using Common.Patch;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace AutoBreakGeode;

internal class ModEntry : Mod
{
    public static bool AutoBreakGeode;
    private ModConfig config = new();
    private bool hasFastAnimation;

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
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new GeodeMenuPatcher(config));
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.AutoBreakGeodeKeybind.JustPressed()) AutoBreakGeode = !AutoBreakGeode;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is GeodeMenu geodeMenu && AutoBreakGeode)
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
                    if (!hasFastAnimation)
                        for (var i = 0; i < config.BreakGeodeSpeed - 1; i++)
                            geodeMenu.update(Game1.currentGameTime);
                }

                if (Game1.player.freeSpotsInInventory() == 1) AutoBreakGeode = false;
            }
            else
            {
                AutoBreakGeode = false;
            }
        }
        else
        {
            AutoBreakGeode = false;
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForAutoBreakGeode(
            Helper,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        ).Register();
    }
}
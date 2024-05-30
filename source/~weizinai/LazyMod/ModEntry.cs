/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Common;
using LazyMod.Framework;
using LazyMod.Framework.Config;
using LazyMod.Framework.Hud;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LazyMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private AutomationManger automationManger = null!;
    private MiningHud miningHud = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(Monitor);
        I18n.Init(helper.Translation);
        try
        {
            config = helper.ReadConfig<ModConfig>();
        }
        catch (Exception)
        {
            helper.WriteConfig(new ModConfig());
            config = helper.ReadConfig<ModConfig>();
            Log.Info("Read config.json file failed and was automatically fixed. Please reset the features you want to turn on.");
        }
        automationManger = new AutomationManger(helper, config);

        // 注册事件
        helper.Events.Display.RenderedHud += OnRenderedHud;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

        // 迁移
        Migrate();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        miningHud.Update();
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        miningHud.Draw(e.SpriteBatch);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        miningHud = new MiningHud(Helper, config);
        
        new GenericModConfigMenuIntegrationForLazyMod(
            Helper,
            ModManifest,
            () => config,
            () =>
            {
                config = new ModConfig();
                UpdateConfig();
            },
            () => Helper.WriteConfig(config)
        ).Register();
    }

    private void UpdateConfig()
    {
        automationManger.UpdateConfig(config);
    }

    private void Migrate()
    {
        // 1.0.8
        config.ChopOakTree.TryAdd(3, false);
        config.ChopMapleTree.TryAdd(3, false);
        config.ChopPineTree.TryAdd(3, false);
        config.ChopMahoganyTree.TryAdd(3, false);
        config.ChopPalmTree.TryAdd(3, false);
        config.ChopMushroomTree.TryAdd(3, false);
        config.ChopGreenRainTree.TryAdd(3, false);
        config.ChopMysticTree.TryAdd(3, false);
    }
}
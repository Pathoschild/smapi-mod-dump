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
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Hud;
using weizinai.StardewValleyMod.LazyMod.Framework.Integration;

namespace weizinai.StardewValleyMod.LazyMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private AutomationManger automationManger = null!;
    private MiningHud miningHud = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        try
        {
            this.config = helper.ReadConfig<ModConfig>();
        }
        catch (Exception)
        {
            helper.WriteConfig(new ModConfig());
            this.config = helper.ReadConfig<ModConfig>();
            Log.Info("Read config.json file failed and was automatically fixed. Please reset the features you want to turn on.");
        }

        this.automationManger = new AutomationManger(helper, this.config);

        // 注册事件
        helper.Events.Display.RenderedHud += this.OnRenderedHud;
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        this.miningHud.Update();
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        this.miningHud.Draw(e.SpriteBatch);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.miningHud = new MiningHud(this.Helper, this.config);
        
        new GenericModConfigMenuIntegrationForLazyMod(this.Helper, this.ModManifest,
            () => this.config,
            () =>
            {
                this.config = new ModConfig();
                this.UpdateConfig();
            },
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }

    private void UpdateConfig()
    {
        this.automationManger.UpdateConfig(this.config);
    }
}
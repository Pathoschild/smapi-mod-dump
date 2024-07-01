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
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature;

public class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        this.InitHandler();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (!Context.IsMainPlayer) Log.Info("该模组大部分功能仅在主机端有效。");
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForSomeMultiplayerFeature(this.Helper, this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new AccessShopInfoHandler(this.Helper, this.config),
            new AutoClickHandler(this.Helper, this.config),
            new CustomCommandHandler(this.Helper, this.config),
            new DelayedPlayerHandler(this.Helper, this.config),
            new IpConnectionHandler(this.Helper, this.config),
            new ItemCheatHandler(this.Helper, this.config),
            new PlayerCountHandler(this.Helper, this.config),
            new TipHandler(this.Helper, this.config),
            new UnreadyPlayerHandler(this.Helper, this.config),
            new VersionLimitHandler(this.Helper, this.config)
        };

        foreach (var handler in handlers) handler.Init();
    }
}
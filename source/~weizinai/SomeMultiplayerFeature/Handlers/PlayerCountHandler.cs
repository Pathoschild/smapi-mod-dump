/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework.UI;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class PlayerCountHandler : BaseHandler
{
    private readonly Button playerCountButton = new(new Point(64, 64), "");

    public PlayerCountHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
    }

    public override void Init()
    {
        this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnSecondUpdateTicked;
        this.Helper.Events.Display.RenderingHud += this.OnRenderingHud;
    }

    // 每秒检测当前在线玩家的数量
    private void OnSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!this.Config.ShowPlayerCount) return;

        // 如果当前没有玩家在线，则返回
        if (!Context.HasRemotePlayers) return;

        this.playerCountButton.name = I18n.UI_PlayerCount(Game1.getOnlineFarmers().Count);
    }

    // 绘制玩家数量按钮
    private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!this.Config.ShowPlayerCount) return;

        // 如果当前没有玩家在线，则返回
        if (!Context.HasRemotePlayers) return;

        this.playerCountButton.Draw(e.SpriteBatch);
    }
}
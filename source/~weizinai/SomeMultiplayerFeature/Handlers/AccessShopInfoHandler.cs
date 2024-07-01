/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework.Message;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class AccessShopInfoHandler : BaseHandler
{
    private IClickableMenu? lastShopMenu;

    public AccessShopInfoHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
    }


    public override void Init()
    {
        this.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    // 检测当前玩家是否正在访问商店并向其他玩家发送消息
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is ShopMenu shopMenu1 && this.lastShopMenu is not ShopMenu)
        {
            var message = new AccessShopInfoMessage(Game1.player.Name, shopMenu1.ShopId);
            this.Helper.Multiplayer.SendMessage(message, "AccessShopInfoMessage", new[] { "weizinai.SomeMultiplayerFeature" });
        }
        else if (this.lastShopMenu is ShopMenu shopMenu2 && Game1.activeClickableMenu is not ShopMenu)
        {
            var message = new AccessShopInfoMessage(Game1.player.Name, shopMenu2.ShopId, true);
            this.Helper.Multiplayer.SendMessage(message, "AccessShopInfoMessage", new[] { "weizinai.SomeMultiplayerFeature" });
        }

        this.lastShopMenu = Game1.activeClickableMenu;
    }

    // 当收到来自其他玩家的商店访问信息时，显示HUD信息
    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!this.Config.ShowAccessShopInfo) return;

        // 如果当前没有玩家在线，则返回
        if (!Context.HasRemotePlayers) return;

        if (e is { FromModID: "weizinai.SomeMultiplayerFeature", Type: "AccessShopInfoMessage" })
        {
            var message = e.ReadAs<AccessShopInfoMessage>();
            var hudMessage = new HUDMessage(message.ToString())
            {
                noIcon = true,
                timeLeft = 500f,
                type = message.PlayerName + message.IsExit
            };
            Game1.addHUDMessage(hudMessage);
        }
    }
}
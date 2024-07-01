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
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class IpConnectionHandler : BaseHandler
{
    public IpConnectionHandler(IModHelper helper, ModConfig config) : base(helper, config)
    {
    }

    public override void Init()
    {
        this.Helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        // 如果该功能为启用，则返回
        if (!this.Config.AutoSetIpConnection) return;
        
        // 如果当前不是多人模式或者当前玩家不是主玩家，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        if (Game1.timeOfDay == this.Config.EnableTime * 100)
        {
            if (Game1.isFestival() && !this.Config.DisableWhenFestival)
            {
                Game1.addHUDMessage(new HUDMessage("今天是节日，不打开Ip连接。") { noIcon = true });
                return;
            }
            Game1.addHUDMessage(new HUDMessage("Ip连接已打开"){ noIcon = true});
            Game1.options.ipConnectionsEnabled = true;
        }
        else if (Game1.timeOfDay == this.Config.DisableTime * 100)
        {
            Game1.addHUDMessage(new HUDMessage("Ip连接已关闭"){ noIcon = true});
            Game1.options.ipConnectionsEnabled = false;
        }
    }
}
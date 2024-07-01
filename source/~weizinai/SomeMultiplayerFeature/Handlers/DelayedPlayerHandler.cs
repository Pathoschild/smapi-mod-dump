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
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class DelayedPlayerHandler : BaseHandler
{
    private int cooldown;

    public DelayedPlayerHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
    }

    public override void Init()
    {
        this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // 如果功能未启用，则返回
        if (!this.Config.ShowDelayedPlayer) return;

        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        this.cooldown++;
        
        if (this.cooldown >= this.Config.ShowInterval)
        {
            this.cooldown = 0;
            
            var playerPing = new Dictionary<string, float>();
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                if (farmer.IsMainPlayer) continue;

                var ping = Game1.server.getPingToClient(farmer.UniqueMultiplayerID);
                if (ping >= 100) playerPing.Add(farmer.Name, ping);
            }

            if (playerPing.Any()) Log.Alert($"{playerPing.MaxBy(x => x.Value).Key}的延迟超过100ms，且其延迟最高。");
        }
    }
}
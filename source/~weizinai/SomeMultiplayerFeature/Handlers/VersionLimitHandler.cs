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

internal class VersionLimitHandler : BaseHandler
{
    private const string ModKey = "SomeMultiplayerFeature_Version";

    private int actionCount;

    public VersionLimitHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
    }

    public override void Init()
    {
        this.Helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!this.Config.VersionLimit) return;

        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        if (this.actionCount > 0)
        {
            foreach (var (id, farmer) in Game1.otherFarmers)
            {
                if (!farmer.modData.ContainsKey(ModKey) || farmer.modData[ModKey] != "0.6.0")
                {
                    this.Helper.Multiplayer.SendMessage($"{farmer.Name}已被踢出，因为其SomeMultiplayerFeature模组不是最新版。", "VersionLimit",
                        new[] { "weizinai.SomeMultiplayerFeature" }, new[] { id });
                    Game1.server.kick(id);
                }
            }

            this.actionCount--;
        }
    }

    // 当载入存档时，向玩家写入版本验证信息
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        // 如果当前玩家是主机端，则返回
        if (Context.IsMainPlayer) return;

        var farmer = Game1.player;
        if (farmer.modData.ContainsKey(ModKey))
            farmer.modData[ModKey] = "0.6.0";
        else
            farmer.modData.Add(ModKey, "0.6.0");
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        // 如果当前不是联机模式或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        this.actionCount++;
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (Context.IsMainPlayer) return;
        
        if (e is { Type: "VersionLimit", FromModID: "weizinai.SomeMultiplayerFeature" })
        {
            var message = e.ReadAs<string>();
            Log.Alert(message);
        }
    }
}
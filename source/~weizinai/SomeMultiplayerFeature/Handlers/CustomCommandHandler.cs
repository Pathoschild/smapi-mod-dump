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

internal class CustomCommandHandler : BaseHandler
{
    private const string BannedPlayerPath = "assets/BannedPlayer.json";
    private readonly Dictionary<string, string>? bannedPlayers;

    public CustomCommandHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        this.bannedPlayers = helper.Data.ReadJsonFile<Dictionary<string, string>>(BannedPlayerPath);
        if (this.bannedPlayers is null)
            Log.Error($"无法找到Json文件: {BannedPlayerPath}，如果你不是主机玩家，则可以忽略该条消息。");
    }

    public override void Init()
    {
        this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
        this.Helper.ConsoleCommands.Add("ban", "", this.BanPlayer);
        this.Helper.ConsoleCommands.Add("unban", "", this.UnbanPlayer);
        this.Helper.ConsoleCommands.Add("ping", "", this.PingPlayer);
        this.Helper.ConsoleCommands.Add("list", "", this.ListPlayer);
        this.Helper.ConsoleCommands.Add("kick", "", this.KickPlayer);
        this.Helper.ConsoleCommands.Add("kickall", "", this.KickAllPlayer);
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        // 如果当前不是联机模式或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        var id = e.Peer.PlayerID;
        var name = Game1.getFarmer(id).Name;

        if (this.bannedPlayers!.ContainsKey(id.ToString()))
        {
            Game1.server.kick(id);
            Log.Alert($"{name}在黑名单中，已被踢出。");
        }
    }

    private void BanPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var target = Game1.getOnlineFarmers().Where(x => x.Name == args[0]);
        foreach (var farmer in target)
        {
            var id = farmer.UniqueMultiplayerID;
            var name = farmer.Name;

            if (this.bannedPlayers!.ContainsKey(id.ToString()))
            {
                Log.Info($"{name}已经在黑名单中。");
            }
            else
            {
                this.bannedPlayers.Add(id.ToString(), name);
                Game1.server.kick(id);
                Log.Info($"{name}被加入黑名单。");
            }
        }

        this.Helper.Data.WriteJsonFile(BannedPlayerPath, this.bannedPlayers);
    }

    private void UnbanPlayer(string command, string[] args)
    {
        // 如果当前不是联机模式或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        var target = this.bannedPlayers!.Where(x => x.Value == args[0]).ToList();

        if (!target.Any()) Log.Info($"{args[0]}不在黑名单中。");
        foreach (var (id, name) in target)
        {
            this.bannedPlayers!.Remove(id);
            Log.Info($"{name}被移出黑名单。");
        }

        this.Helper.Data.WriteJsonFile(BannedPlayerPath, this.bannedPlayers);
    }

    private void PingPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        foreach (var (id, farmer) in Game1.otherFarmers)
            Log.Info($"{Game1.Multiplayer.getUserName(farmer.UniqueMultiplayerID)} {farmer.Name}\t{(int)Game1.server.getPingToClient(id)}ms ");
    }

    private void ListPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        Log.Info("下面是在线的玩家：");
        foreach (var farmer in Game1.getOnlineFarmers())
            Log.Info($"{farmer.Name}\t{farmer.currentLocation.DisplayName}\t{Game1.Multiplayer.getUserName(farmer.UniqueMultiplayerID)}");
        Log.Info("下面是离线的玩家");
        foreach (var farmer in Game1.getOfflineFarmhands())
            Log.Info($"{farmer.Name}");
    }

    private void KickPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        var target = Game1.getOnlineFarmers().FirstOrDefault(x => x.Name == args[0]);
        if (target is null)
        {
            Log.Info($"{args[0]}不存在。");
        }
        else
        {
            Game1.server.kick(target.UniqueMultiplayerID);
            Game1.otherFarmers.Remove(target.UniqueMultiplayerID);
            Log.Info($"{target.Name}已被踢出。");
        }
    }

    private void KickAllPlayer(string command, string[] args)
    {
        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;

        foreach (var (id, farmer) in Game1.otherFarmers)
        {
            Game1.server.kick(id);
            Log.Info($"{farmer.Name}已被踢出。");
        }
    }
}
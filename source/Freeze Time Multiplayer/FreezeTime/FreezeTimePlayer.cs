/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alcmoe/FreezeTimeMultiplayer
**
*************************************************/

namespace FreezeTime;

using StardewValley;

public class FreezeTimeChecker(FreezeTime.ModConfig config)
{
    private readonly Dictionary<long, FreezeTimePlayer> _freezeTimePlayers = [];

    public class FreezeTimePlayer
    {
        public bool Loaded {get; set; }
        
        public bool Frozen {get; set;}
    }
    
    public void AddPlayer(Farmer player)
    {
        if (!_freezeTimePlayers.ContainsKey(player.UniqueMultiplayerID)) {
            _freezeTimePlayers.Add(player.UniqueMultiplayerID, new FreezeTimePlayer());
        }
    }

    public void DelPlayer(long playerId)
    {
        _freezeTimePlayers.Remove(playerId);
    }

    public FreezeTimePlayer GetPlayer(Farmer player)
    {
        return _freezeTimePlayers[player.UniqueMultiplayerID];
    }

    public void SetPlayerLoaded(Farmer player, bool value)
    {
        if (HasPlayer(player))
        {
            GetPlayer(player).Loaded = value;
        }
    }

    public void SetPlayerFrozen(Farmer player, bool value)
    {
        if (!HasPlayer(player)) {
            return;
        }
        GetPlayer(player).Frozen = value;
    }

    public bool HasPlayer(Farmer player)
    {
        return _freezeTimePlayers.ContainsKey(player.UniqueMultiplayerID);
    }

    public bool HasPlayer(long playerId)
    {
        return _freezeTimePlayers.ContainsKey(playerId);
    }

    public bool IsFrozen()
    {
        return config.Any() ? _freezeTimePlayers.Any(freezeTimePlayer => freezeTimePlayer.Value.Frozen) : _freezeTimePlayers.All(freezeTimePlayer => freezeTimePlayer.Value.Frozen);
    }

    public Dictionary<long, Dictionary<string, bool>> FreezeTimeStatus()
    {
        Dictionary<long, Dictionary<string, bool>> status = [];
        foreach (var freezeTimePlayer in _freezeTimePlayers) {
            status.Add(freezeTimePlayer.Key, new Dictionary<string, bool>(){{"Loaded", freezeTimePlayer.Value.Loaded}, {"Frozen", freezeTimePlayer.Value.Frozen}});
        }
        return status;
    }
    
    public Dictionary<long, FreezeTimePlayer> GetCollection()
    {
        return _freezeTimePlayers;
    }

    public void LoadFromStatus(Dictionary<long, Dictionary<string, bool>> statuses)
    {
        _freezeTimePlayers.Clear();
        foreach (var status in statuses) {
            var freezeTimePlayer = new FreezeTimePlayer
            {
                Loaded = status.Value["Loaded"],
                Frozen = status.Value["Frozen"]
            };
            _freezeTimePlayers.Add(status.Key, freezeTimePlayer);
        }
    }

    public string GetFreezeTimeMessage()
    {
        string message;
        var extra   = "";
        if (IsFrozen()) {
            message = "Time has been frozen.";
            List<string> names = [];
            names.AddRange(Game1.getOnlineFarmers().Where(farmer => GetPlayer(farmer).Frozen)
                .Select(farmer => farmer.Name));
            if (names.Count > 0) {
                extra = "Because of [" + string.Join(',', names) + "].";
            }
        } else {
            message = "Time is passing.";
            List<string> names = [];
            names.AddRange(Game1.getOnlineFarmers().Where(farmer => !GetPlayer(farmer).Loaded)
                .Select(farmer => farmer.Name));
            if (names.Count > 0) {
                extra = " But [" + string.Join(',', names) + "] don't have FreezeTime mod";
            }
        }

        return message + extra;
    }
}
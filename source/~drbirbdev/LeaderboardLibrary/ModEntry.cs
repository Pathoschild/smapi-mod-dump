/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using BirbCore.Attributes;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace LeaderboardLibrary;

[SMod]
public class ModEntry : Mod
{
    private const string IDENTITY_POOL = "us-west-2:2e234341-a166-4d68-94f8-f2e1ffb14e73";
    private const string GLOBAL_DATA_KEY = "leaderboard-global";
    private static readonly RegionEndpoint REGION = RegionEndpoint.USWest2;
    internal static readonly PerScreen<GlobalModData> GLOBAL_MOD_DATA = new();

    [SMod.Instance]
    internal static ModEntry Instance;
    internal static LocalModData LocalModData;

    internal static AmazonDynamoDBClient DdbClient;

    public override void Entry(IModHelper helper)
    {
        Parser.ParseAll(this);

        GLOBAL_MOD_DATA.SetValueForScreen(0, this.Helper.Data.ReadGlobalData<GlobalModData>(GLOBAL_DATA_KEY));
        if (GLOBAL_MOD_DATA.Value is null)
        {
            Log.Debug("Creating new global leaderboard data...");
            GLOBAL_MOD_DATA.SetValueForScreen(0, new GlobalModData());
            this.Helper.Data.WriteGlobalData(GLOBAL_DATA_KEY, GLOBAL_MOD_DATA.Value);
        }
        Log.Debug($"Using leaderboard identity {GLOBAL_MOD_DATA?.Value?.UserUuid ?? ""} and secret staring with {GLOBAL_MOD_DATA?.Value?.Secret?[..3]}");


        LocalModData = this.Helper.Data.ReadJsonFile<LocalModData>("data/cached_leaderboards.json");
        if (LocalModData is null)
        {
            Log.Debug("Creating new local leaderboard cache...");
            if (GLOBAL_MOD_DATA is { Value: not null })
            {
                LocalModData = new LocalModData(GLOBAL_MOD_DATA.Value.UserUuid);
            }

            this.Helper.Data.WriteJsonFile("data/cached_leaderboards.json", LocalModData);
        }

#pragma warning disable CA2000
        CognitoAWSCredentials credentials = new(IDENTITY_POOL, REGION);
#pragma warning restore CA2000
        Log.Debug("Using temporary credentials: " + credentials.GetIdentityId());
        DdbClient = new AmazonDynamoDBClient(credentials, REGION);

        this.Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
        this.Helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
        this.Helper.Events.Multiplayer.PeerConnected += this.Multiplayer_PeerConnected;
    }

    private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
    {
        if (!Context.IsMainPlayer)
        {
            this.Helper.Multiplayer.SendMessage(GLOBAL_MOD_DATA.Value.UserUuid, "ShareUUID", [this.ModManifest.UniqueID]);
        }
    }

    private void Multiplayer_PeerConnected(object sender, StardewModdingAPI.Events.PeerConnectedEventArgs e)
    {
        this.Helper.Multiplayer.SendMessage(GLOBAL_MOD_DATA.Value.UserUuid, "ShareUUID", [this.ModManifest.UniqueID]);
        if (!e.Peer.IsSplitScreen)
        {
            return;
        }

        if (e.Peer.ScreenID == 0)
        {
            return;
        }

        GlobalModData globalData = new()
        {
            UserUuid = $"{GLOBAL_MOD_DATA.GetValueForScreen(0).UserUuid}+guest",
            Secret = GLOBAL_MOD_DATA.GetValueForScreen(0).Secret
        };

        if (e.Peer.ScreenID != null)
        {
            GLOBAL_MOD_DATA.SetValueForScreen(e.Peer.ScreenID.Value, globalData);
        }
    }

    private static void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
    {
        if (e.FromModID == Instance.ModManifest.UniqueID && e.Type == "ShareUUID" && e.FromPlayerID != Game1.player.UniqueMultiplayerID)
        {
            LocalModData.MultiplayerUuiDs.Add(e.ReadAs<string>());
        }
    }

    public override object GetApi(IModInfo mod)
    {
        if (TryAddModToCache(mod.Manifest.UniqueID))
        {
            this.Helper.Data.WriteJsonFile("data/cached_leaderboards.json", LocalModData);
        }
        return new LeaderboardApi(mod.Manifest.UniqueID);
    }

    private static bool TryAddModToCache(string modId)
    {
        bool result = false;
        if (!LocalModData.LocalLeaderboards.ContainsKey(modId))
        {
            LocalModData.LocalLeaderboards.Add(modId, new Dictionary<string, List<LeaderboardStat>>());
            result = true;
        }

        if (LocalModData.TopLeaderboards.ContainsKey(modId))
        {
            return result;
        }

        LocalModData.TopLeaderboards.Add(modId, new Dictionary<string, List<LeaderboardStat>>());
        return true;
    }
}

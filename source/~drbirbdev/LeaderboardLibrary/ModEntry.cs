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
    private static readonly string IDENTITY_POOL = "us-west-2:2e234341-a166-4d68-94f8-f2e1ffb14e73";
    private static readonly RegionEndpoint REGION = RegionEndpoint.USWest2;
    private static readonly string GLOBAL_DATA_KEY = "leaderboard-global";

    [SMod.Instance]
    internal static ModEntry Instance;
    internal static readonly PerScreen<GlobalModData> GlobalModData = new PerScreen<GlobalModData>();
    internal static LocalModData LocalModData;

    internal static AmazonDynamoDBClient DdbClient;

    public override void Entry(IModHelper helper)
    {
        Parser.ParseAll(this);

        GlobalModData.SetValueForScreen(0, this.Helper.Data.ReadGlobalData<GlobalModData>(GLOBAL_DATA_KEY));
        if (GlobalModData.Value is null)
        {
            Log.Debug("Creating new global leaderboard data...");
            GlobalModData.SetValueForScreen(0, new GlobalModData());
            this.Helper.Data.WriteGlobalData<GlobalModData>(GLOBAL_DATA_KEY, GlobalModData.Value);
        }
        Log.Debug($"Using leaderboard identity {GlobalModData?.Value?.UserUUID ?? ""} and secret staring with {GlobalModData?.Value?.Secret?[..3]}" ?? "");


        LocalModData = this.Helper.Data.ReadJsonFile<LocalModData>("data/cached_leaderboards.json");
        if (LocalModData is null)
        {
            Log.Debug("Creating new local leaderboard cache...");
            LocalModData = new LocalModData(GlobalModData.Value.UserUUID);
            this.Helper.Data.WriteJsonFile<LocalModData>("data/cached_leaderboards.json", LocalModData);
        }

#pragma warning disable CA2000
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(IDENTITY_POOL, REGION);
#pragma warning restore CA2000
        Log.Debug("Using temporary credentials: " + credentials.GetIdentityId());
        DdbClient = new AmazonDynamoDBClient(credentials, REGION);

        this.Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived;
        this.Helper.Events.Multiplayer.PeerConnected += this.Multiplayer_PeerConnected;
    }

    private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
    {
        if (!Context.IsMainPlayer)
        {
            this.Helper.Multiplayer.SendMessage<string>(GlobalModData.Value.UserUUID, "ShareUUID", new[] { this.ModManifest.UniqueID });
        }
    }

    private void Multiplayer_PeerConnected(object sender, StardewModdingAPI.Events.PeerConnectedEventArgs e)
    {
        this.Helper.Multiplayer.SendMessage<string>(GlobalModData.Value.UserUUID, "ShareUUID", new[] { this.ModManifest.UniqueID });
        if (e.Peer.IsSplitScreen)
        {
            if (e.Peer.ScreenID != 0)
            {
                GlobalModData globalData = new GlobalModData()
                {
                    UserUUID = $"{GlobalModData.GetValueForScreen(0).UserUUID}+guest",
                    Secret = GlobalModData.GetValueForScreen(0).Secret,
                };
                GlobalModData.SetValueForScreen(e.Peer.ScreenID.Value, globalData);
            }
        }
    }

    private void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
    {
        if (e.FromModID == ModEntry.Instance.ModManifest.UniqueID && e.Type == "ShareUUID" && e.FromPlayerID != Game1.player.UniqueMultiplayerID)
        {
            LocalModData.MultiplayerUUIDs.Add(e.ReadAs<string>());
        }
    }

    public override object GetApi(IModInfo mod)
    {
        if (TryAddModToCache(mod.Manifest.UniqueID))
        {
            this.Helper.Data.WriteJsonFile<LocalModData>("data/cached_leaderboards.json", LocalModData);
        }
        return new LeaderboardAPI(mod.Manifest.UniqueID);
    }

    public static bool TryAddModToCache(string modId)
    {
        bool result = false;
        if (!LocalModData.LocalLeaderboards.ContainsKey(modId))
        {
            LocalModData.LocalLeaderboards.Add(modId, new Dictionary<string, List<LeaderboardStat>>());
            result = true;
        }
        if (!LocalModData.TopLeaderboards.ContainsKey(modId))
        {
            LocalModData.TopLeaderboards.Add(modId, new Dictionary<string, List<LeaderboardStat>>());
            result = true;
        }
        return result;
    }
}

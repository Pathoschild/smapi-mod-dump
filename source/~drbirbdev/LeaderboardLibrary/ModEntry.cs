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
using BirbShared;
using BirbShared.Mod;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace LeaderboardLibrary
{
    public class ModEntry : Mod
    {
        private static string IDENTITY_POOL = "us-west-2:2e234341-a166-4d68-94f8-f2e1ffb14e73";
        private static RegionEndpoint REGION = RegionEndpoint.USWest2;
        private static string GLOBAL_DATA_KEY = "leaderboard-global";

        [SmapiInstance]
        internal static ModEntry Instance;
        [SmapiCommand]
        internal static Command Command;
        internal static AmazonDynamoDBClient DdbClient;
        internal static readonly PerScreen<GlobalModData> GlobalModData = new PerScreen<GlobalModData>();
        internal static LocalModData LocalModData;

        public override void Entry(IModHelper helper)
        {
            ModClass mod = new ModClass();
            mod.Parse(this, false);

            GlobalModData.SetValueForScreen(0, this.Helper.Data.ReadGlobalData<GlobalModData>(GLOBAL_DATA_KEY));
            if (GlobalModData.Value is null)
            {
                Log.Debug("Creating new global leaderboard data...");
                GlobalModData.SetValueForScreen(0, new GlobalModData());
                this.Helper.Data.WriteGlobalData<GlobalModData>(GLOBAL_DATA_KEY, GlobalModData.Value);
            }
            Log.Debug($"Using leaderboard identity {GlobalModData.Value.UserUUID} and secret staring with {GlobalModData.Value.Secret.Substring(0, 3)}");


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
                Helper.Multiplayer.SendMessage<string>(GlobalModData.Value.UserUUID, "ShareUUID", new[] { ModManifest.UniqueID });
            }
        }

        private void Multiplayer_PeerConnected(object sender, StardewModdingAPI.Events.PeerConnectedEventArgs e)
        {
            Helper.Multiplayer.SendMessage<string>(GlobalModData.Value.UserUUID, "ShareUUID", new[] { ModManifest.UniqueID });
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
            if (this.TryAddModToCache(mod.Manifest.UniqueID))
            {
                this.Helper.Data.WriteJsonFile<LocalModData>("data/cached_leaderboards.json", LocalModData);
            }
            return new LeaderboardAPI(mod.Manifest.UniqueID);
        }

        public bool TryAddModToCache(string modId)
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
}

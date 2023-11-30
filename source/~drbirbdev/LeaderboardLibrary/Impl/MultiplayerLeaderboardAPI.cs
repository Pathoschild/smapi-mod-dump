/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using StardewValley;

namespace LeaderboardLibrary;

class MultiplayerLeaderboardAPI : ChainableLeaderboardAPI
{
    private readonly string ModId;
    private readonly ILeaderboardAPI DelegateApi;
    public override ILeaderboardAPI Delegate => this.DelegateApi;

    public MultiplayerLeaderboardAPI(string modId)
    {
        this.ModId = modId;
        this.DelegateApi = new CachedLeaderboardAPI(modId);
        ModEntry.Instance.Helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived;
    }

    private void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
    {
        if (e.FromModID == ModEntry.Instance.ModManifest.UniqueID && e.Type == $"{this.ModId}:UploadScore" && e.FromPlayerID != Game1.player.UniqueMultiplayerID)
        {
            string name = Game1.getFarmer(e.FromPlayerID)?.Name;
            UploadScoreMessage message = e.ReadAs<UploadScoreMessage>();
            ((CachedLeaderboardAPI)this.Delegate).UpdateCache(message.Stat, message.Score, message.UserUUID, name);
        }
    }

    public override bool UploadScore(string stat, int score)
    {
        UploadScoreMessage message = new UploadScoreMessage(stat, score, ModEntry.GlobalModData.Value.UserUUID);
        ModEntry.Instance.Helper.Multiplayer.SendMessage<UploadScoreMessage>(message, $"{this.ModId}:UploadScore", new[] { ModEntry.Instance.ModManifest.UniqueID });
        return this.Delegate.UploadScore(stat, score);
    }
}

class UploadScoreMessage
{
    public string Stat;
    public int Score;
    public string UserUUID;

    public UploadScoreMessage(string stat, int score, string userUuid)
    {
        this.Stat = stat;
        this.Score = score;
        this.UserUUID = userUuid;
    }
}

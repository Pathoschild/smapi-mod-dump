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

class MultiplayerLeaderboardApi : ChainableLeaderboardApi
{
    private readonly string _modId;
    protected override ILeaderboardApi Delegate { get; }

    public MultiplayerLeaderboardApi(string modId)
    {
        this._modId = modId;
        this.Delegate = new CachedLeaderboardApi(modId);
        ModEntry.Instance.Helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived;
    }

    private void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Instance.ModManifest.UniqueID || e.Type != $"{this._modId}:UploadScore" ||
            e.FromPlayerID == Game1.player.UniqueMultiplayerID)
        {
            return;
        }

        string name = Game1.getFarmer(e.FromPlayerID)?.Name;
        UploadScoreMessage message = e.ReadAs<UploadScoreMessage>();
        ((CachedLeaderboardApi)this.Delegate).UpdateCache(message.Stat, message.Score, message.UserUuid, name);
    }

    public override bool UploadScore(string stat, int score)
    {
        UploadScoreMessage message = new(stat, score, ModEntry.GLOBAL_MOD_DATA.Value.UserUuid);
        ModEntry.Instance.Helper.Multiplayer.SendMessage(message, $"{this._modId}:UploadScore", [
            ModEntry.Instance.ModManifest.UniqueID
        ]);
        return this.Delegate.UploadScore(stat, score);
    }
}

class UploadScoreMessage(string stat, int score, string userUuid)
{
    public string Stat = stat;
    public int Score = score;
    public string UserUuid = userUuid;
}

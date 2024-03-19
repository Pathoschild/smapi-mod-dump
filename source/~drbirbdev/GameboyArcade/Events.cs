/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbCore.Attributes;
using StardewModdingAPI;

namespace GameboyArcade;

[SEvent]
internal class Events
{
    /// <summary>
    /// Allow remote players to load ROM saves which are marked as shared.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    [SEvent.ModMessageReceived]
    private void LoadRequest(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Instance.ModManifest.UniqueID || e.Type != "LoadRequest")
        {
            return;
        }

        string minigameId = e.ReadAs<string>();
        SaveState loaded =
            ModEntry.Instance.Helper.Data.ReadJsonFile<SaveState>(
                $"data/{minigameId}/{Constants.SaveFolderName}/file.json");
        ModEntry.Instance.Helper.Multiplayer.SendMessage(loaded, "LoadReceive", [
            ModEntry.Instance.ModManifest.UniqueID
        ], [e.FromPlayerID]);
    }

    /// <summary>
    /// Allow remote players to save ROM saves which are marked as shared.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    [SEvent.ModMessageReceived]
    private void SaveRequest(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Instance.ModManifest.UniqueID || !e.Type.StartsWith("SaveRequest "))
        {
            return;
        }

        string minigameId = e.Type[12..];
        if (!ModEntry.Content.ContainsKey(minigameId))
        {
            Log.Error(
                $"{e.FromPlayerID} sent save request for {minigameId}, but no such minigame exists for host computer!");
            return;
        }

        SaveState save = e.ReadAs<SaveState>();
        ModEntry.Instance.Helper.Data.WriteJsonFile(
            $"data/{minigameId}/{Constants.SaveFolderName}/file.json", save);
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace PamTries;

/// <summary>
/// Utility methods for this mod.
/// </summary>
internal static class PTUtilities
{
    internal static IDictionary<string, string>? Lexicon { get; set; }

    /// <summary>
    /// Loads the Leixcon file.
    /// </summary>
    /// <param name="contentHelper">the game content helper.</param>
    internal static void PopulateLexicon(IGameContentHelper contentHelper)
    {
        Lexicon = contentHelper.Load<Dictionary<string, string>>("Strings/Lexicon");
    }

    /// <summary>
    /// Gets a translated string from the lexicon.
    /// </summary>
    /// <param name="key">Key to search for.</param>
    /// <param name="defaultresponse">Default response if nothing is found.</param>
    /// <returns>string, if possible.</returns>
    internal static string GetLexicon(string key, string? defaultresponse = null)
    {
        string? value = null;
        if (Lexicon is not null)
        {
            Lexicon.TryGetValue(key, out value);
        }
        if (value is null)
        {
            if (string.IsNullOrWhiteSpace(defaultresponse))
            {
                value = key;
            }
            else
            {
                value = defaultresponse;
            }
        }
        return value;
    }

    /// <summary>
    /// Checks to see if any player has a specific conversation topic. If so, gives everyone the conversation topic.
    /// </summary>
    /// <param name="conversationTopic">conversation topic to sync.</param>
    internal static void SyncConversationTopics(string conversationTopic)
    {
        if (!Context.IsMultiplayer)
        {
            return;
        }
        // Rewrite this. If host has it, everyone has host's amount of days. Else, find player with it.
        if (Game1.player.activeDialogueEvents.ContainsKey(conversationTopic))
        {
            return;
        }
        if (Game1.MasterPlayer.activeDialogueEvents.TryGetValue(conversationTopic, out int conversationdays))
        {
            Game1.player.activeDialogueEvents[conversationTopic] = conversationdays;
        }
        else
        {
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                if (farmer.activeDialogueEvents.TryGetValue(conversationTopic, out conversationdays))
                {
                    Game1.player.activeDialogueEvents[conversationTopic] = conversationdays;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Syncs conversation topics between players.
    /// </summary>
    /// <param name="conversationTopics">IEnumerable of conversation topics to sync.</param>
    internal static void SyncConversationTopics(IEnumerable<string> conversationTopics)
    {
        foreach (string conversationTopic in conversationTopics)
        {
            SyncConversationTopics(conversationTopic);
        }
    }

    /// <summary>
    /// Syncs the events seen for all players.
    /// </summary>
    /// <param name="modMonitor">Monitor, to use for logging.</param>
    internal static void LocalEventSyncs(IMonitor modMonitor)
    {
        if (!Context.IsMultiplayer)
        {
            return;
        }
        if (Game1.getAllFarmers().Any((Farmer farmer) => farmer.mailForTomorrow.Contains("atravita_PamTries_PennyThanks")))
        {
            Game1.player.eventsSeen.Add(99210001);
            modMonitor.Log("Syncing event 9921001");
        }
        if (Game1.getAllFarmers().Any((Farmer farmer) => farmer.eventsSeen.Contains(99210002)))
        {
            Game1.player.eventsSeen.Add(99210002);
            modMonitor.Log("Syncing event 99210002");
        }
    }
}

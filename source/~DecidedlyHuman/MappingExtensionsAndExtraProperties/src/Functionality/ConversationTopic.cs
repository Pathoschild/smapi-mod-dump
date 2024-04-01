/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace MappingExtensionsAndExtraProperties.Functionality;

public class ConversationTopic
{
    public static bool SetConversationTopic(string topic, int days)
    {
        if (!Game1.player.activeDialogueEvents.ContainsKey(topic))
            Game1.player.activeDialogueEvents.TryAdd(topic, days);

        return false;
    }
}

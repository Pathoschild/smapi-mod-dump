/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace MappingExtensionsAndExtraProperties.Models.TileProperties;

public class AddConversationTopic : ITilePropertyData
{
    public static string PropertyKey => "MEEP_AddConversationTopic";
    private string conversationTopic;
    private int numberOfDays;

    public string ConversationTopic
    {
        get => this.conversationTopic;
    }

    public int Days
    {
        get => this.numberOfDays;
    }

    public AddConversationTopic(string ct, int days)
    {
        this.conversationTopic = ct;
        this.numberOfDays = days;
    }
}

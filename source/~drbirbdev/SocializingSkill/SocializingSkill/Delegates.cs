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
using StardewValley;
using StardewValley.Delegates;

namespace SocializingSkill;

[SDelegate]
public class Delegates
{
    [SDelegate.GameStateQuery]
    public static bool HeartLevel(string[] query, GameStateQueryContext context)
    {
        string npcName = ArgUtility.Get(query, 2, "Target", false);
        if (npcName == "Target")
        {
            if (context.CustomFields is null ||
                !context.CustomFields.TryGetValue("NPC", out object value) ||
                value is not NPC npc)
            {
                Log.Warn("FRIENDSHIP GSQ expects NPC Custom Field");
                return false;
            }

            npcName = npc.Name;
        }

        if (!ArgUtility.TryGetInt(query, 1, out int level, out string error))
        {
            return GameStateQuery.Helpers.ErrorResult(query, error);
        }

        return context.Player.getFriendshipHeartLevelForNPC(npcName) >= level;
    }
}

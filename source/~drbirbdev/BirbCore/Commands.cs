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

namespace BirbCore;

[SCommand("birbcore")]
internal class Commands
{
    [SCommand.Command]
    public static void Gsq(string query, GameLocation location = null, Farmer player = null, Item targetItem = null, Item inputItem = null)
    {
        string[] queries = GameStateQuery.SplitRaw(query);


        Log.Info($"Checking {queries.Length} queries");
        foreach (string rawQuery in queries)
        {
            bool result = GameStateQuery.CheckConditions(rawQuery, location, player, targetItem, inputItem);
            Log.Info($"\"{rawQuery}\" had result {result}");
        }

    }
}

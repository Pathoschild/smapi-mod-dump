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

namespace LeaderboardLibrary;

public sealed class LocalModData
{
    public Dictionary<string, Dictionary<string, List<LeaderboardStat>>> LocalLeaderboards { get; } = new();
    public Dictionary<string, Dictionary<string, List<LeaderboardStat>>> TopLeaderboards { get; } = new();
    public HashSet<string> MultiplayerUuiDs { get; } = [];

    public LocalModData(string uuid)
    {
        this.MultiplayerUuiDs.Add(uuid);
    }
}

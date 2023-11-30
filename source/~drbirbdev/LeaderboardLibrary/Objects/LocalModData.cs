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
    public Dictionary<string, Dictionary<string, List<LeaderboardStat>>> LocalLeaderboards { get; set; } = new Dictionary<string, Dictionary<string, List<LeaderboardStat>>>();
    public Dictionary<string, Dictionary<string, List<LeaderboardStat>>> TopLeaderboards { get; set; } = new Dictionary<string, Dictionary<string, List<LeaderboardStat>>>();
    public HashSet<string> MultiplayerUUIDs { get; set; } = new HashSet<string>();

    public LocalModData(string uuid)
    {
        this.MultiplayerUUIDs.Add(uuid);
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbCore.APIs;
using BirbCore.Attributes;
using StardewModdingAPI;

namespace JunimoKartGlobalRankings;

[SMod]
public class ModEntry : Mod
{
    [SMod.Instance]
    internal static ModEntry Instance;
    [SMod.Api("drbirbdev.LeaderboardLibrary")]
    internal static ILeaderboard LeaderboardApi;

    public override void Entry(IModHelper helper)
    {
        Parser.ParseAll(this);
    }
}

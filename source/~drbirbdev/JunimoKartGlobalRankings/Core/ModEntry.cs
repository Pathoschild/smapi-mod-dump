/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using StardewModdingAPI;
using BirbShared.APIs;
using BirbShared.Mod;

namespace JunimoKartGlobalRankings
{
    public class ModEntry : Mod
    {
        [SmapiInstance]
        internal static ModEntry Instance;
        [SmapiApi(UniqueID = "drbirbdev.LeaderboardLibrary")]
        internal static ILeaderboard LeaderboardAPI;

        public override void Entry(IModHelper helper)
        {
            ModClass mod = new ModClass();
            mod.Parse(this, true);
        }
    }
}

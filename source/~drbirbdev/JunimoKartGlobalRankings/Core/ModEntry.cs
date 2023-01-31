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
using BirbShared;
using HarmonyLib;
using BirbShared.APIs;

namespace JunimoKartGlobalRankings
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal static ILeaderboard LeaderboardAPI;


        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Log.Init(this.Monitor);

            this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            new Harmony(this.ModManifest.UniqueID).PatchAll();

            LeaderboardAPI = Helper.ModRegistry.GetApi<ILeaderboard>("drbirbdev.LeaderboardLibrary");
        }
    }
}

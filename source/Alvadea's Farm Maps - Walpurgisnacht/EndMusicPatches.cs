/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jaksha6472/WitchTower
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace AlvadeasWitchTower
{
    internal static class EndMusicPatches
    {
        public static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.cleanupBeforePlayerExit)),
                prefix: new HarmonyMethod(typeof(EndMusicPatches), nameof(EndMusicPatches.Prefix_CleanupBeforePlayerExit))
            );

        }

        private static void Prefix_CleanupBeforePlayerExit(GameLocation __instance)
        {
            if (Game1.currentLocation.NameOrUniqueName == "Custom_WalWitchtower")
            {
                Game1.changeMusicTrack("none", music_context: StardewValley.GameData.MusicContext.Default);
            }

            if (Game1.currentLocation.NameOrUniqueName == "Custom_WalWitchtowerCave")
            {
                Game1.changeMusicTrack("none", music_context: StardewValley.GameData.MusicContext.Default);
            }
        }
    }
}

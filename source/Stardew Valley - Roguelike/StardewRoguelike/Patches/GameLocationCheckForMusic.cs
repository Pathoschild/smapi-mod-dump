/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley;
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    internal class GameLocationCheckForMusic : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(GameLocation), "checkForMusic");

        public static bool Prefix(GameLocation __instance)
        {
            if (__instance is MineShaft)
                return true;
            else if (__instance is not Mine)
                return true;

            string targetTrack = "Upper_Ambient";
            if (!Game1.currentSong.IsPlaying)
                Game1.changeMusicTrack("none");

            if (targetTrack != Game1.getMusicTrackName())
                Game1.changeMusicTrack(targetTrack);

            return false;
        }
    }
}

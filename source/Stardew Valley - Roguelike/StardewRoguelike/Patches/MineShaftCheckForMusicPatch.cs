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
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Patches
{
    internal class MineShaftCheckForMusicPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "checkForMusic");

        public static bool Prefix(MineShaft __instance)
        {
            string targetTrack;
            if (Merchant.IsMerchantFloor(__instance))
            {
                if (Merchant.GetMusicTracks().Contains(Game1.getMusicTrackName()))
                    targetTrack = Game1.getMusicTrackName();
                else
                    targetTrack = Roguelike.GetRandomTrack(Merchant.GetMusicTracks());
            }
            else if (ForgeFloor.IsForgeFloor(__instance))
                targetTrack = Roguelike.GetRandomTrack(ForgeFloor.GetMusicTracks());
            else if (BossFloor.IsBossFloor(__instance))
            {
                if (BossFloor.GetMusicTracks(__instance).Contains(Game1.getMusicTrackName()))
                    targetTrack = Game1.getMusicTrackName();
                else
                    targetTrack = Roguelike.GetRandomTrack(BossFloor.GetMusicTracks(__instance));
            }
            else if (ChallengeFloor.IsChallengeFloor(__instance) && ChallengeFloor.GetMusicTracks(__instance) is not null)
            {
                if (ChallengeFloor.GetMusicTracks(__instance).Contains(Game1.getMusicTrackName()))
                    targetTrack = Game1.getMusicTrackName();
                else
                    targetTrack = Roguelike.GetRandomTrack(ChallengeFloor.GetMusicTracks(__instance));
            }
            else
            {
                if (Game1.getMusicTrackName() != __instance.getMineSong())
                    targetTrack = __instance.getMineSong();
                else
                    targetTrack = Game1.getMusicTrackName();
            }

            if (!Game1.currentSong.IsPlaying)
                Game1.changeMusicTrack("none");

            if (targetTrack != Game1.getMusicTrackName() || Game1.getMusicTrackName().EndsWith("_Ambient"))
                Game1.changeMusicTrack(targetTrack);

            return false;
        }
    }
}

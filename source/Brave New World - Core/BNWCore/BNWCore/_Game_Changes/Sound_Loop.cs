/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace BNWCore
{
    public class SoundLoop
    {
        public void CheckMusicNeedsRestarting(OneSecondUpdateTickingEventArgs e)
        {
            if (ModEntry.Config.BNWCoresetSoundLoop)
            {
                if (!Context.IsWorldReady)
                    return;
                MineShaft.timeSinceLastMusic = 999999;
                if (!Game1.currentLocation.IsOutdoors)
                    return;
                if (Game1.isDarkOut())
                    return;
                if (Game1.currentLocation.shouldHideCharacters())
                    return;
                if (Game1.eventUp)
                    return;
                if (Game1.currentSong == null || Game1.currentSong.IsStopped || Game1.requestedMusicTrack.ToLower().Contains("ambient"))
                {
                    if (Game1.currentLocation.Name == "Woods")
                    {
                        Game1.changeMusicTrack("woodsTheme");
                    }
                    else
                    {
                        Game1.playMorningSong();
                    }
                }
            }
        }
        public void ResetMusicSecretWoods(WarpedEventArgs e)
        {
            if (ModEntry.Config.BNWCoresetSoundLoop)
            {
                if (e.IsLocalPlayer && e.NewLocation.Name == "Woods" && !Game1.isDarkOut())
                {
                    Game1.changeMusicTrack("none");
                }
            } 
        }
    }
}
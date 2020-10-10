/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace DynamicNightTime.Patches
{
    class CheckForMusicPatch
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static bool Prefix(GameLocation __instance, GameTime time)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (Game1.shouldPlayMorningSong(false) && !Game1.isDarkOut())
                Game1.playMorningSong();

            if (Utility.IsDesertLocation(__instance))
                return false;
            if (Game1.getMusicTrackName(Game1.MusicContext.Default) == "sam_acoustic1" && Game1.isMusicContextActiveButNotPlaying(Game1.MusicContext.Default))
                Game1.changeMusicTrack("none", true, Game1.MusicContext.Default);
            if (!(__instance is MineShaft) && Game1.getMusicTrackName(Game1.MusicContext.Default).Contains("Ambient") && !Game1.getMusicTrackName(Game1.MusicContext.Default).Contains(Game1.currentSeason))
                Game1.changeMusicTrack("none", true, Game1.MusicContext.Default);

            if (__instance.IsOutdoors && Game1.isMusicContextActiveButNotPlaying(Game1.MusicContext.Default) && !Game1.currentSong.Name.Contains(Game1.currentSeason) && (!Game1.isRaining && !Game1.eventUp))
            {
                if (!Game1.isDarkOut())
                {
                    string currentSeason = Game1.currentSeason;
                    if (!(currentSeason == "spring"))
                    {
                        if (!(currentSeason == "summer"))
                        {
                            if (!(currentSeason == "fall"))
                            {
                                if (currentSeason == "winter")
                                    Game1.changeMusicTrack("winter_day_ambient", true, Game1.MusicContext.Default);
                            }
                            else
                                Game1.changeMusicTrack("fall_day_ambient", true, Game1.MusicContext.Default);
                        }
                        else
                            Game1.changeMusicTrack("summer_day_ambient", true, Game1.MusicContext.Default);
                    }
                    else
                        Game1.changeMusicTrack("spring_day_ambient", true, Game1.MusicContext.Default);
                }
                else if (Game1.isDarkOut())
                {
                    string currentSeason = Game1.currentSeason;
                    if (!(currentSeason == "spring"))
                    {
                        if (!(currentSeason == "summer"))
                        {
                            if (!(currentSeason == "fall"))
                            {
                                if (currentSeason == "winter")
                                    Game1.changeMusicTrack("none", true, Game1.MusicContext.Default);
                            }
                            else
                                Game1.changeMusicTrack("spring_night_ambient", true, Game1.MusicContext.Default);
                        }
                        else
                            Game1.changeMusicTrack("spring_night_ambient", true, Game1.MusicContext.Default);
                    }
                    else
                        Game1.changeMusicTrack("spring_night_ambient", true, Game1.MusicContext.Default);
                }
            }
            else if (Game1.isMusicContextActiveButNotPlaying(Game1.MusicContext.Default) && Game1.isRaining && !Game1.showingEndOfNightStuff)
                Game1.changeMusicTrack("rain", true, Game1.MusicContext.Default);
            if (!Game1.isRaining && (!Game1.currentSeason.Equals("fall") || !Game1.isDebrisWeather) && (!Game1.currentSeason.Equals("winter") && !Game1.eventUp && (Game1.timeOfDay < 1800 && __instance.Name.Equals((object)"Town"))) && (Game1.isMusicContextActiveButNotPlaying(Game1.MusicContext.Default) || Game1.getMusicTrackName(Game1.MusicContext.Default).Contains("ambient")))
            {
                Game1.changeMusicTrack("springtown", false, Game1.MusicContext.Default);
            }
            else
            {
                if (!__instance.Name.Equals((object)"AnimalShop") && !__instance.Name.Equals((object)"ScienceHouse") || !Game1.isMusicContextActiveButNotPlaying(Game1.MusicContext.Default) && !Game1.getMusicTrackName(Game1.MusicContext.Default).Contains("ambient") || __instance.currentEvent != null)
                    return false;
                Game1.changeMusicTrack("marnieShop", false, Game1.MusicContext.Default);
            }

            return false;
        }

    }
}

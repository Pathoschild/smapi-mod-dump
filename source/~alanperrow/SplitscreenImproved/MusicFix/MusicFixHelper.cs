/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewValley;
using StardewValley.GameData;

namespace SplitscreenImproved.MusicFix
{
    internal static class MusicFixHelper
    {
        private static readonly FieldInfo instanceActiveMusicContextField = AccessTools.DeclaredField(typeof(Game1), "_instanceActiveMusicContext");
        private static readonly FieldInfo instanceRequestedMusicTracksField = AccessTools.DeclaredField(typeof(Game1), "_instanceRequestedMusicTracks");

        /* DEBUG
        internal static void DrawDebugText(SpriteBatch sb)
        {
            if (!ModEntry.Config.MusicFixFeature.IsDebugMode)
            {
                return;
            }

            Game1 thisInstance = Game1.game1;

            string playerNum = $"P{thisInstance.instanceIndex + 1}: ";
            string agg = string.Empty;
            foreach (MusicContext musicContext in Enum.GetValues<MusicContext>())
            {
                string key = Game1.getMusicTrackName(musicContext);
                agg += playerNum + musicContext.ToString() + ": " + key + "\n";
            }

            agg += "--------------\n";

            MusicContext activeMusicContext = (MusicContext)instanceActiveMusicContextField.GetValue(thisInstance);
            agg += playerNum + "ActiveMusicContext: " + activeMusicContext.ToString() + "\n";

            agg += playerNum + "IsMusicContextActiveButNotPlaying(): " + IsMusicContextActiveButNotPlaying(MusicContext.Default).ToString() + "\n";

            Game1 mainInstance = GameRunner.instance.gameInstances.Single(x => x.IsMainInstance);
            string mainCurrentTrack = GetMusicTrackNameOfInstance(mainInstance, MusicContext.Default);
            agg += playerNum + "Main music track: " + mainCurrentTrack + "\n";
            string mainCurrentTrackSplitscreen = GetMusicTrackNameOfInstance(mainInstance, MusicContext.ImportantSplitScreenMusic);
            agg += playerNum + "Main splitscreen music track: " + mainCurrentTrackSplitscreen + "\n";

            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(4, 2), Microsoft.Xna.Framework.Color.Black);
            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(4, 6), Microsoft.Xna.Framework.Color.Black);
            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(2, 4), Microsoft.Xna.Framework.Color.Black);
            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(6, 4), Microsoft.Xna.Framework.Color.Black);
            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(4, 4), Microsoft.Xna.Framework.Color.White);
        }
        */

        internal static bool IsMusicContextActiveButNotPlaying(MusicContext music_context)
        {
            Game1 thisInstance = Game1.game1;
            MusicContext this_activeMusicContext = (MusicContext)instanceActiveMusicContextField.GetValue(thisInstance);

            Game1 mainInstance = GameRunner.instance.gameInstances.Single(x => x.IsMainInstance);

            // Check if this is not main instance, and if so, perform separate logic.
            if (!thisInstance.IsMainInstance)
            {
                if (this_activeMusicContext != music_context)
                {
                    return false;
                }
                if (Game1.morningSongPlayAction != null)
                {
                    return false;
                }

                string mainCurrentTrack = GetMusicTrackNameOfInstance(mainInstance, music_context);
                if (mainCurrentTrack == "none")
                {
                    return true;
                }
                if (Game1.currentSong != null && Game1.currentSong.Name == mainCurrentTrack && !Game1.currentSong.IsPlaying)
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        // Refactored base game method to include a Game1 instance parameter.
        public static string GetMusicTrackNameOfInstance(Game1 instance, MusicContext music_context = MusicContext.Default)
        {
            var requestedMusicTracks = (Dictionary<MusicContext, KeyValuePair<string, bool>>)instanceRequestedMusicTracksField.GetValue(instance);

            if (requestedMusicTracks.TryGetValue(music_context, out var value))
            {
                return value.Key;
            }

            if (music_context == MusicContext.Default)
            {
                return GetMusicTrackNameOfInstance(instance, MusicContext.SubLocation);
            }

            return "none";
        }
    }
}

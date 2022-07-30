/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sounds_Patcher.Utility;
using StardewModdingAPI;
using StardewValley;

namespace Sounds_Patcher.Patches
{
    public class Game1SoundPatches
    {
        private static Config config = ModEntry.StaticConfig;
        private static IMonitor monitor = ModEntry.StaticMonitor;

        public static bool playSound_prefix(string cueName)
        {
            try
            {
                var attempted = cueName;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch(Exception ex) { monitor.Log($"Game1SoundPatches : Something went wrong while trying to disable the sound {cueName} - {ex.Message}", LogLevel.Error); return true; }
        }

        public static bool playSoundPitched_prefix(string cueName, int pitch)
        {
            try
            {
                var attempted = cueName;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch(Exception ex) { monitor.Log($"Game1SoundPatches : Something went wrong while trying to disable the pitched sound {cueName} - {ex.Message}", LogLevel.Error); return true; }
        }

        public static bool playItemNumberSelectSound_prefix()
        {
            try
            {
                var attempted = "flute";

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch (Exception ex) { monitor.Log($"Game1SoundPatches : Something went wrong while trying to disable itemNumberSelectSound flute - {ex.Message}", LogLevel.Error); return true; }
        }

        public static bool changeMusicTrack_prefix(string newTrackName, bool track_interruptable, Game1.MusicContext music_context)
        {
            try
            {
                if (newTrackName == "none") return true;

                var attempted = newTrackName;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch(Exception ex) { monitor.Log($"Game1SoundPatches : Something went wrong while trying to disable itemNumberSelectSound flute - {ex.Message}", LogLevel.Error); return true; }
        }
    }
}

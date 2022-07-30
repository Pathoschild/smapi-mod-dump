/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Sounds_Patcher.Utility;
using StardewModdingAPI;
using StardewValley.Network;
using System;
using System.Linq;

namespace Sounds_Patcher.Patches
{
    public class GameLocationSoundPatches
    {
        private static Config config = ModEntry.StaticConfig;
        private static IMonitor monitor = ModEntry.StaticMonitor;

        public static bool playSound_prefix(string audioName, NetAudio.SoundContext soundContext)
        {
            try
            {
                var attempted = audioName;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch (Exception ex) { monitor.Log($"GameLocationSoundPatches : Something went wrong while trying to disable the sound {audioName} - {ex.Message}", LogLevel.Error); return true; }
        }

        public static bool playSoundPitched_prefix(string audioName, int pitch, NetAudio.SoundContext soundContext)
        {
            try
            {
                var attempted = audioName;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch (Exception ex) { monitor.Log($"GameLocationSoundPatches : Something went wrong while trying to disable the pitched sound {audioName} - {ex.Message}", LogLevel.Error); return true; }
        }

        public static bool playSoundAt_prefix(string audioName, Vector2 position, NetAudio.SoundContext soundContext)
        {
            try
            {
                var attempted = audioName;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch (Exception ex) { monitor.Log($"GameLocationSoundPatches : Something went wrong while trying to disable the sound {audioName} at {position.X}-{position.Y} - {ex.Message}", LogLevel.Error); return true; }
        }

        public static bool localSound_prefix(string audioName)
        {
            try
            {
                var attempted = audioName;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch (Exception ex) { monitor.Log($"GameLocationSoundPatches : Something went wrong while trying to disable the local sound {audioName} - {ex.Message}", LogLevel.Error); return true; }
        }

        public static bool localSoundAt_prefix(string audioName, Vector2 position)
        {
            try
            {
                var attempted = audioName;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch (Exception ex) { monitor.Log($"GameLocationSoundPatches : Something went wrong while trying to disable the local sound {audioName} at {position.X}-{position.Y} - {ex.Message}", LogLevel.Error); return true; }
        }
    }
}

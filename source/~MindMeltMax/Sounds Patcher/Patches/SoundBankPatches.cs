/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using Sounds_Patcher.Utility;
using StardewModdingAPI;
using System;
using System.Linq;

namespace Sounds_Patcher.Patches
{
    public class SoundBankPatches
    {
        private static Config config = ModEntry.StaticConfig;
        private static IMonitor monitor = ModEntry.StaticMonitor;

        public static bool PlayCue_prefix_1(string name)
        {
            try
            {
                var attempted = name;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch(Exception ex) { monitor.Log($"SoundBankPatches : Something went wrong while trying to disable the sound {name} - {ex.Message}"); return true; }
        }

        public static bool PlayCue_prefix_2(string name, AudioListener listener, AudioEmitter emitter)
        {
            try
            {
                var attempted = name;

                if (config.Sounds.Count > 0 && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Sounds.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;
                else if (config.Songs.Count > 0 && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Key != null && config.Songs.Where(x => x.Key == attempted).FirstOrDefault().Value) return false;

                return true;
            }
            catch (Exception ex) { monitor.Log($"SoundBankPatches : Something went wrong while trying to disable the sound {name} - {ex.Message}"); return true; }
        }
    }
}

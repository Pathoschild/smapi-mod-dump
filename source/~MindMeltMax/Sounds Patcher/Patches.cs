/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SoundsPatcher.Utility;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Audio;
using System;
using System.Linq;

namespace SoundsPatcher
{
    internal static class Patches
    {
        private static Config config = ModEntry.IConfig;

        public static void Patch(string id)
        {
            Harmony harmony = new(id);
            harmony.Patch(
                original: AccessTools.Method(typeof(SoundsHelper), nameof(SoundsHelper.PlayLocal)),
                prefix: new(typeof(Patches), nameof(PlayLocalPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SoundsHelper), nameof(SoundsHelper.PlayAll)),
                prefix: new(typeof(Patches), nameof(PlayAllPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Cue), nameof(Cue.Play)),
                prefix: new(typeof(Patches), nameof(PlayPrefix))
            );
        }

        public static bool PlayLocalPrefix(string cueName, GameLocation location, Vector2? position, int? pitch, SoundContext context, out ICue cue, ref bool __result)
        {
            cue = null;
            try
            {
                updateConfigSounds(cueName);
                return __result = canPlaySound(cueName);
            }
            catch (Exception ex)
            {
                handleError(nameof(ISoundsHelper.PlayLocal), cueName, ex);
                return true;
            }
        }

        public static bool PlayAllPrefix(string cueName, GameLocation location, Vector2? position, int? pitch, SoundContext context)
        {
            try
            {
                updateConfigSounds(cueName);
                return canPlaySound(cueName);
            }
            catch (Exception ex)
            {
                handleError(nameof(ISoundsHelper.PlayLocal), cueName, ex);
                return true;
            }
        }

        public static bool PlayPrefix(Cue __instance)
        {
            try
            {
                updateConfigSounds(__instance.Name);
                return canPlaySound(__instance.Name);
            }
            catch (Exception ex)
            {
                handleError(nameof(ISoundsHelper.PlayLocal), __instance?.Name, ex);
                return true;
            }
        }

        private static void updateConfigSounds(string sound)
        {
            if (!ModEntry.IConfig.Songs.ContainsKey(sound) && !ModEntry.IConfig.Sounds.ContainsKey(sound) && !ModEntry.IConfig.UnknownSounds.ContainsKey(sound))
                ModEntry.IConfig.UnknownSounds.Add(sound, false);
        }

        private static bool canPlaySound(string sound) => !ModEntry.IConfig.Sounds.Any(x => x.Key == sound && x.Value) && !ModEntry.IConfig.Songs.Any(x => x.Key == sound && x.Value);

        private static void handleError(string source, string cue, Exception ex)
        {
            ModEntry.IMonitor.Log($"Failed patching {source} while trying to play {cue}", LogLevel.Error);
            ModEntry.IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
        }
    }
}

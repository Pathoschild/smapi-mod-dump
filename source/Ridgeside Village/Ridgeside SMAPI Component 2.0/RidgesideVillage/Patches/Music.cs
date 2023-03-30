/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewModdingAPI.Events;
    
namespace RidgesideVillage
{
    internal static class Music
    {
        private static IMonitor Monitor { get; set; }
        private static IModHelper Helper { get; set; }

        static Dictionary<string, string> CueToSongMap;


        internal static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            Helper = helper;
            Log.Trace($"Applying Harmony Patch \"{nameof(Music)}\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getSongTitleFromCueName)),
                prefix: new HarmonyMethod(typeof(Music), nameof(getSongTitleFromCueName_prefix))
            );

            CueToSongMap = Helper.ModContent.Load<Dictionary<string, string>>("assets/MusicDisplayNames.json");

        }

        internal static bool getSongTitleFromCueName_prefix(string cueName, ref string __result)
        {
            if(CueToSongMap.TryGetValue(cueName, out var title))
            {
                __result = title;
                return false;
            }
            return true;
        }

    }
}

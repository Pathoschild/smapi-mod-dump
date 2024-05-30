/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using System;
using HarmonyLib;
using SplitscreenImproved.Layout;
using SplitscreenImproved.MusicFix;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace SplitscreenImproved
{
    [HarmonyPatch(typeof(Game1))]
    public class Game1Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Game1.SetWindowSize))]
        public static bool SetWindowSize_Prefix(Game1 __instance, int w, int h)
        {
            if (!ModEntry.Config.IsModEnabled
                || !ModEntry.Config.LayoutFeature.IsFeatureEnabled)
            {
                return true;
            }

            try
            {
                // Replace base game method call.
                LayoutManager.SetWindowSize(__instance, w, h);
                return false;
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(SetWindowSize_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Game1.isMusicContextActiveButNotPlaying))]
        public static void IsMusicContextActiveButNotPlaying_Postfix(ref bool __result, MusicContext music_context)
        {
            if (!ModEntry.Config.IsModEnabled
                || !ModEntry.Config.MusicFixFeature.IsFeatureEnabled)
            {
                return;
            }

            try
            {
                if (Game1.game1.IsMainInstance)
                {
                    return;
                }

                bool newResult = MusicFixHelper.IsMusicContextActiveButNotPlaying(music_context);
                if (newResult != __result)
                {
                    __result = newResult;
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(IsMusicContextActiveButNotPlaying_Postfix)}:\n{e}", LogLevel.Error);
            }
        }


    }
}

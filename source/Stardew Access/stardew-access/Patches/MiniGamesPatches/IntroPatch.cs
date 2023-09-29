/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using HarmonyLib;
using StardewValley.Minigames;
using stardew_access.Translation;

namespace stardew_access.Patches
{
    public class IntroPatch : IPatch
    {
        private static string introQuery = " ";

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Intro), "draw"),
                postfix: new HarmonyMethod(typeof(IntroPatch), nameof(DrawPatch))
            );
        }

        private static void DrawPatch(Intro __instance, int ___currentState)
        {
            try
            {
                if (MainClass.ModHelper == null)
                    return;

                string translationKey = "";

                if (___currentState == 3)
                {
                    translationKey = "intro-scene3";
                }
                else if (___currentState == 4)
                {
                    translationKey = "intro-scene4";
                }

                if (introQuery == translationKey) return;
                introQuery = translationKey;

                MainClass.ScreenReader.TranslateAndSay(translationKey, false,
                    translationCategory: TranslationCategory.MiniGames);
            }
            catch (System.Exception e)
            {
                Log.Error($"An error occured in intro minigame patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
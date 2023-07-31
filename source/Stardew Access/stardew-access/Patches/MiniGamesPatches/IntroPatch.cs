/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley.Minigames;
using stardew_access.Translation;

namespace stardew_access.Patches
{
    public class IntroPatch
    {
        internal static string introQuery = " ";

        internal static void DrawPatch(Intro __instance, int ___currentState)
        {
            try
            {
                if (MainClass.ModHelper == null)
                    return;

                string toSpeak = " ";

                if (___currentState == 3)
                {
                    toSpeak = Translator.Instance.Translate("intro-scene3");
                }
                else if (___currentState == 4)
                {
                    toSpeak = Translator.Instance.Translate("intro-scene4");
                }

                if (toSpeak != " " && introQuery != toSpeak)
                {
                    introQuery = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, false);
                    return;
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"An error occured in intro minigame patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class JojaCDMenuPatch
    {
        internal static string jojaCDMenuQuery = "";

        internal static void DrawPatch(JojaCDMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = "";

                for (int i = 0; i < __instance.checkboxes.Count; i++)
                {
                    ClickableComponent c = __instance.checkboxes[i];
                    if (!c.containsPoint(x, y))
                        continue;

                    if (c.name.Equals("complete"))
                    {
                        toSpeak = $"Completed {getNameFromIndex(i)}";
                    }
                    else
                    {
                        toSpeak = $"{getNameFromIndex(i)} Cost: {__instance.getPriceFromButtonNumber(i)}g Description: {__instance.getDescriptionFromButtonNumber(i)}";
                    }

                    break;
                }

                if (jojaCDMenuQuery != toSpeak)
                {
                    jojaCDMenuQuery = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static string getNameFromIndex(int i)
        {
            string name = i switch
            {
                0 => "Bus",
                1 => "Minecarts",
                2 => "Bridge",
                3 => "Greenhouse",
                4 => "Panning",
                _ => "",
            };

            if (name != "")
                return $"{name} Project";
            else
                return "unkown";
        }

        internal static void Cleanup()
        {
            jojaCDMenuQuery = "";
        }
    }
}

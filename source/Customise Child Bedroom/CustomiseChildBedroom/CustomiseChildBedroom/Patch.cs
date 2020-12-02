/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/CustomiseChildBedroom
**
*************************************************/

using StardewValley;
using StardewValley.Events;
using System.Reflection;

namespace CustomiseChildBedroom
{
    class Patch
    {
        public static bool PreventPregnancy(QuestionEvent __instance, int ___whichQuestion)
        {
            //If it's a 2 it's a barn animal, and that's ok
            if (___whichQuestion != 2)
            {
                if (!ModEntry.Config.GetCurrentFarm().GetFarmer(Game1.player.Name).ShowCrib)
                {
                    ModEntry.Log(Translation.GetString("effect.didwork"), StardewModdingAPI.LogLevel.Debug);
                    return false;
                }
            }
            return true;
        }
    }
}

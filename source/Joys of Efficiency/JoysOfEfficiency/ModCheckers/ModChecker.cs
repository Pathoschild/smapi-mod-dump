/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using StardewModdingAPI;

namespace JoysOfEfficiency.ModCheckers
{
    public class ModChecker
    {
        public static bool IsCoGLoaded(IModHelper helper) => helper.ModRegistry.IsLoaded("punyo.CasksOnGround");
        public static bool IsCaLoaded(IModHelper helper) => helper.ModRegistry.IsLoaded("CasksAnywhere");
    }
}

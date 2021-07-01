/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Harmony;
using StardewValley;

namespace JsonAssets.Overrides
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.loadForNewGame))]
    public static class GameLoadForNewGamePatch
    {
        public static void Prefix()
        {
            Mod.instance.onBlankSave();
        }
    }
}

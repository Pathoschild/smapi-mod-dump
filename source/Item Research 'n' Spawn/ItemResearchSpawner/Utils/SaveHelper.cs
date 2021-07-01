/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using StardewValley;

namespace ItemResearchSpawner.Utils
{
    internal static class SaveHelper
    {
        public static string DirectoryName => $"{Game1.player.Name}_{Game1.getFarm().NameOrUniqueName}";
    }
}
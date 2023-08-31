/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/i-saac-b/PostBoxMod
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace PostBoxMod
{
    public static class GameContentHelperExtensions
    {
        // thanks to @atravita from the SDV Discord for this utility function
        public static bool InvalidateCacheAndLocalized(this IGameContentHelper helper, string assetName) => helper.InvalidateCache(assetName)
            | (helper.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en && helper.InvalidateCache(assetName + "." + helper.CurrentLocale));

    }
}

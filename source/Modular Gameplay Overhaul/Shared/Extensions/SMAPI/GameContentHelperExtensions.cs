/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions.SMAPI;

/// <summary>Extensions for the <see cref="IModHelper"/> interface.</summary>
public static class GameContentHelperExtensions
{
    /// <summary>Invalidates the cache for the current asset in English or the current game language.</summary>
    /// <param name="helper">The <see cref="IGameContentHelper"/> API of the current <see cref="IMod"/>.</param>
    /// <param name="assetName">The asset name without extension.</param>
    /// <returns><see langword="true"/> if the corresponding asset was invalidated and re-cached, otherwise <see langword="false"/>.</returns>
    public static bool InvalidateCacheAndLocalized(this IGameContentHelper helper, string assetName)
    {
        return helper.InvalidateCache(assetName)
               | (helper.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en &&
                  helper.InvalidateCache(assetName + "." + helper.CurrentLocale));
    }
}

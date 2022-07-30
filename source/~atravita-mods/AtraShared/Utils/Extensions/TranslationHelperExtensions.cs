/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Holds extension methods for ITranslationHelper.
/// </summary>
public static class TranslationHelperExtensions
{
    /// <summary>
    /// Whether a specific translation exists.
    /// </summary>
    /// <param name="helper">Translation helper.</param>
    /// <param name="key">Key to search for.</param>
    /// <param name="tokens">Tokens to substitute in.</param>
    /// <returns>True for has translation, false otherwise.</returns>
    public static bool HasTranslation(this ITranslationHelper helper, string key, object? tokens = null)
        => tokens is null ? helper.Get(key).HasValue() : helper.Get(key, tokens).HasValue();

    /// <summary>
    /// Attempts to get a translation.
    /// </summary>
    /// <param name="helper">Translation helper.</param>
    /// <param name="key">key to search for.</param>
    /// <param name="translation">Translation, if found.</param>
    /// <returns>True if translation found, false otherwise.</returns>
    public static bool TryGetTranslation(this ITranslationHelper helper, string key, out Translation translation)
    {
        translation = helper.Get(key);
        return translation.HasValue();
    }

    /// <summary>
    /// Attempts to get a translation.
    /// </summary>
    /// <param name="helper">Translation helper.</param>
    /// <param name="key">key to search for.</param>
    /// <param name="tokens">tokens to substitute in.</param>
    /// <param name="translation">Translation, if found.</param>
    /// <returns>True if translation found, false otherwise.</returns>
    public static bool TryGetTranslation(this ITranslationHelper helper, string key, object tokens, out Translation translation)
    {
        translation = helper.Get(key, tokens);
        return translation.HasValue();
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace SonoCore.Extensions;

/// <summary>Extension methods for the <see cref="IContentPack"/> interface.</summary>
public static class IContentPackExtensions
{
    /*********
    ** Public Methods
    *********/
    /// <summary>Loads content from the content pack folder.</summary>
    /// <typeparam name="T">The expected data type to load.</typeparam>
    /// <param name="contentPack">The content pack to to load content from.</param>
    /// <param name="key">The relative file path within the content pack (case-insensitive).</param>
    /// <param name="result">The loaded asset.</param>
    /// <returns><see langword="true"/>, if the loading was successful; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty or contains invalid characters.</exception>
    /// <exception cref="ContentLoadException">Thrown if the asset couldn't be loaded (e.g. because it doesn't exist).</exception>
    public static bool TryLoadAsset<T>(this IContentPack contentPack, string key, out T? result)
        where T : class
    {
        if (contentPack.HasFile(key))
        {
            result = contentPack.ModContent.Load<T>(key);
            return true;
        }

        result = null;
        return false;
    }
}

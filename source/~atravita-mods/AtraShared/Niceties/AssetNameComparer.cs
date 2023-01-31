/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Niceties;

/// <summary>
/// A comparer to use for asset names.
/// </summary>
public class AssetNameComparer : EqualityComparer<IAssetName>
{
    private static readonly AssetNameComparer instance = new();

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static AssetNameComparer Instance => instance;

    /// <summary>
    /// Whether an asset name should be considered equal to another.
    /// </summary>
    /// <param name="lhs">left value.</param>
    /// <param name="rhs">right value.</param>
    /// <returns>whether or not they should be considered equal.</returns>
    public override bool Equals(IAssetName? lhs, IAssetName? rhs)
    {
        if (lhs is null)
        {
            return rhs is null;
        }
        if (rhs is null)
        {
            return false;
        }

        return lhs.Name.Equals(rhs.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override int GetHashCode([DisallowNull] IAssetName asset)
        => asset.GetHashCode();
}

/// <summary>
/// A comparer to use for locale-insensitive asset names.
/// </summary>
public class BaseAssetNameComparer : EqualityComparer<IAssetName>
{

    private static StringComparer comparer = StringComparer.OrdinalIgnoreCase;
    private static BaseAssetNameComparer instance = new();

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static BaseAssetNameComparer Instance => instance;

    /// <summary>
    /// Whether an asset name should be considered equal to another, ignoring locale.
    /// </summary>
    /// <param name="lhs">left value.</param>
    /// <param name="rhs">right value.</param>
    /// <returns>whether or not they should be considered equal.</returns>
    public override bool Equals(IAssetName? lhs, IAssetName? rhs)
    {
        if (lhs is null)
        {
            return rhs is null;
        }
        if (rhs is null)
        {
            return false;
        }

        return lhs.BaseName.Equals(rhs.BaseName, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override int GetHashCode([DisallowNull] IAssetName asset)
        => comparer.GetHashCode(asset.BaseName);
}

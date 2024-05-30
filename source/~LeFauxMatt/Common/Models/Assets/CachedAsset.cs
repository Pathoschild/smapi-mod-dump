/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Models.Assets;

using StardewMods.FauxCore.Common.Interfaces.Cache;

#else
namespace StardewMods.Common.Models.Assets;

using StardewMods.Common.Interfaces.Cache;
#endif

/// <inheritdoc />
internal sealed class CachedAsset<TAssetType> : ICachedAsset
    where TAssetType : notnull
{
    private readonly Func<TAssetType> getValue;

    private TAssetType? cachedValue;

    public CachedAsset(Func<TAssetType> getValue) => this.getValue = getValue;

    public TAssetType Value => this.cachedValue ??= this.getValue();

    /// <inheritdoc />
    public void ClearCache() => this.cachedValue = default(TAssetType);
}
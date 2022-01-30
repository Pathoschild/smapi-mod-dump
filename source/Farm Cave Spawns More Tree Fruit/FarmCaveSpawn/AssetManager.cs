/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/FarmCaveSpawn
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace FarmCaveSpawn;

/// <summary>
/// Handles the fake assets for this mod.
/// </summary>
internal class AssetManager : IAssetLoader
{
#pragma warning disable SA1401 // Fields should be private. This is intentional.

    /// <summary>
    /// Fake asset location for the denylist.
    /// </summary>
    public readonly string DENYLIST_LOCATION = PathUtilities.NormalizeAssetName("Mods/atravita_FarmCaveSpawn_denylist");

    /// <summary>
    /// Fake asset location for more locations that can spawn in fruit.
    /// </summary>
    public readonly string ADDITIONAL_LOCATIONS_LOCATION = PathUtilities.NormalizeAssetName("Mods/atravita_FarmCaveSpawn_additionalLocations");
#pragma warning restore SA1401 // Fields should be private

    /// <inheritdoc/>
    public bool CanLoad<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(this.DENYLIST_LOCATION) || asset.AssetNameEquals(this.ADDITIONAL_LOCATIONS_LOCATION);
    }

    /// <inheritdoc/>
    public T Load<T>(IAssetInfo asset)
    {
        if (asset.AssetNameEquals(this.DENYLIST_LOCATION))
        {
            return (T)(object)new Dictionary<string, string>
            {
            };
        }
        else if (asset.AssetNameEquals(this.ADDITIONAL_LOCATIONS_LOCATION))
        {
            return (T)(object)new Dictionary<string, string>
            {
                ["FlashShifter.SVECode"] = "Custom_MinecartCave, Custom_DeepCave",
#if DEBUG // Regex's test!
                ["atravita.FarmCaveSpawn"] = "Town:[(4;5);(34;40)]",
#endif
            };
        }
        throw new InvalidOperationException($"Should not have tried to load '{asset.AssetName}'.");
    }
}
